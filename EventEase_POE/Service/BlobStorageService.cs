using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace EventEase_POE.Service
{
    /// <summary>
    /// Service for managing file uploads to Azure Blob Storage.
    /// </summary>
    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService(string connectionString, string containerName = "eventease-images")
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        }

        /// <summary>
        /// Uploads a file to Azure Blob Storage and returns the blob URI.
        /// </summary>
        public async Task<string> UploadFileToBlobAsync(IFormFile file, string blobName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null.");

            try
            {
                // Ensure container exists
                await _containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                // Create unique blob name to avoid conflicts
                string uniqueBlobName = $"{Guid.NewGuid()}_{blobName}";

                BlobClient blobClient = _containerClient.GetBlobClient(uniqueBlobName);

                // Upload file
                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error uploading file to blob storage: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Deletes a file from Azure Blob Storage.
        /// </summary>
        public async Task<bool> DeleteFileFromBlobAsync(string blobUri)
        {
            if (string.IsNullOrEmpty(blobUri))
                return false;

            try
            {
                // Extract blob name from URI
                var uri = new Uri(blobUri);
                string blobName = uri.Segments.Last();

                BlobClient blobClient = _containerClient.GetBlobClient(blobName);
                await blobClient.DeleteIfExistsAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error deleting file from blob storage: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Validates if the uploaded file is a valid image.
        /// </summary>
        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var maxFileSize = 5 * 1024 * 1024; // 5 MB

            return allowedExtensions.Contains(fileExtension) &&
                   allowedMimeTypes.Contains(file.ContentType) &&
                   file.Length <= maxFileSize;
        }
    }
}
