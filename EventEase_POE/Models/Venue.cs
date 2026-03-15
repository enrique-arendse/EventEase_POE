using System.ComponentModel.DataAnnotations;

namespace EventEase_POE.Models
{
	public class Venue
	{
		public int VenueId { get; set; }

		[Required]
		[StringLength(100)]
		public string VenueName { get; set; }

		[Required]
		[StringLength(100)]
		public string Location { get; set; }

		[Required]
		[Range(1, 100000)]
		public int Capacity { get; set; }

		[Display(Name = "Image URL")]
		public string? ImageUrl { get; set; }

		// Navigation property
     public ICollection<Booking>? Bookings { get; set; }

      

	}
}