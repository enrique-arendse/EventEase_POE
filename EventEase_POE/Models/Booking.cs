using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase_POE.Models
{
	public class Booking
	{
		public int BookingId { get; set; }

		[Required]
		public int VenueId { get; set; }

		[Required]
		public int EventId { get; set; }

		[Required]
		[DataType(DataType.Date)]
		public DateTime BookingDate { get; set; }

		// Navigation properties
		public Venue? Venue { get; set; }
		public Event? Event { get; set; }
	}
}