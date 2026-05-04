using System.ComponentModel.DataAnnotations;
using EventEase_POE.Data;
using EventEase_POE.Models;

namespace EventEase_POE.Validations
{
    /// <summary>
    /// Custom validation attribute to prevent double booking of a venue on the same date.
    /// </summary>
    public class NoDoubleBookingAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var booking = validationContext.ObjectInstance as Booking;
            if (booking == null)
                return ValidationResult.Success;

            // Get the DbContext from the service provider
            var context = (ApplicationDbContext?)validationContext.GetService(typeof(ApplicationDbContext));
            if (context == null)
                return ValidationResult.Success;

            // Check if a booking exists for the same venue on the same date
            var existingBooking = context.Bookings
                .FirstOrDefault(b => 
                    b.VenueId == booking.VenueId && 
                    b.BookingDate == booking.BookingDate && 
                    b.BookingId != booking.BookingId); // Exclude current booking if editing

            if (existingBooking != null)
            {
                return new ValidationResult("A booking already exists for this venue on the selected date.");
            }

            return ValidationResult.Success;
        }
    }
}
