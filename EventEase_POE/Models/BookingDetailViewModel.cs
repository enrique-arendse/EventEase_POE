using System.ComponentModel.DataAnnotations;

namespace EventEase_POE.Models
{
    /// <summary>
    /// View model for displaying booking information with consolidated venue and event data.
    /// Used for the enhanced booking list with search capability.
    /// </summary>
    public class BookingDetailViewModel
    {
        public int BookingId { get; set; }

        [Display(Name = "Event")]
        public string EventName { get; set; } = string.Empty;

        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; }

        [Display(Name = "Venue")]
        public string VenueName { get; set; } = string.Empty;

        [Display(Name = "Venue Location")]
        public string VenueLocation { get; set; } = string.Empty;

        [Display(Name = "Venue Capacity")]
        public int VenueCapacity { get; set; }

        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        [Display(Name = "Description")]
        public string? EventDescription { get; set; }

        public int VenueId { get; set; }
        public int EventId { get; set; }
    }
}
