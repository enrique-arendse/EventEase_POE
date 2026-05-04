using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace EventEase_POE.Services
{
	public class AzureStorageService
	{
		private readonly BlobContainerClient _containerClient;
		private readonly IConfiguration _configuration;

		public AzureStorageService(IConfiguration configuration)
		{
			_configuration = configuration;
			var connectionString = configuration.GetConnectionString("AzureStorageConnection");
			var containerName = configuration["ConnectionStrings:AzureStorageContainerName"];

			var blobServiceClient = new BlobServiceClient(connectionString);
			_containerClient = blobServiceClient.GetBlobContainerClient(containerName);
		}

		public async Task<string> UploadImageAsync(IFormFile file)
		{
			if (file == null || file.Length == 0)
				throw new ArgumentException("File is empty");

			var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
			var blobClient = _containerClient.GetBlobClient(fileName);

			using (var stream = file.OpenReadStream())
			{
				await blobClient.UploadAsync(stream, overwrite: true);
			}

			return blobClient.Uri.ToString();
		}

		public async Task DeleteImageAsync(string imageUrl)
		{
			if (string.IsNullOrEmpty(imageUrl))
				return;

			try
			{
				var fileName = Path.GetFileName(new Uri(imageUrl).LocalPath);
				var blobClient = _containerClient.GetBlobClient(fileName);
				await blobClient.DeleteAsync();
			}
			catch (Exception ex)
			{
				// Log error but don't throw - image might already be deleted
				Console.WriteLine($"Error deleting image: {ex.Message}");
			}
		}
	}
}