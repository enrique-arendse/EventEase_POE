using System.ComponentModel.DataAnnotations;

namespace EventEase_POE.Models
{
    /// <summary>
    /// View model for creating or editing a venue with image upload.
    /// </summary>
    public class VenueUploadViewModel
    {
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Venue name is required")]
        [StringLength(100, ErrorMessage = "Venue name cannot exceed 100 characters")]
        public string VenueName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 100000, ErrorMessage = "Capacity must be between 1 and 100,000")]
        public int Capacity { get; set; }

        [Display(Name = "Venue Image (Optional)")]
        [DataType(DataType.Upload)]
        public IFormFile? ImageFile { get; set; }

        // Store the existing image URL if editing
        public string? ImageUrl { get; set; }
    }

    /// <summary>
    /// View model for creating or editing an event with image upload.
    /// </summary>
    public class EventUploadViewModel
    {
        public int EventId { get; set; }

        [Required(ErrorMessage = "Event name is required")]
        [StringLength(100, ErrorMessage = "Event name cannot exceed 100 characters")]
        public string EventName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event date is required")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Display(Name = "Event Image (Optional)")]
        [DataType(DataType.Upload)]
        public IFormFile? ImageFile { get; set; }

        // Store the existing image URL if editing
        public string? ImageUrl { get; set; }
    }
}
