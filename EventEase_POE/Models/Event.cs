using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase_POE.Models
{
	public class Event
	{
		public int EventId { get; set; }

		[Required]
		[StringLength(100)]
		public string EventName { get; set; }

		[Required]
		[DataType(DataType.Date)]
		public DateTime EventDate { get; set; }

     [StringLength(500)]
		public string? Description { get; set; }

		// Optional image for the event
       [Display(Name = "Image URL")]
		public string? ImageUrl { get; set; }

		// Navigation property
		public ICollection<Booking>? Bookings { get; set; }
	}
}
