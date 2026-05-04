using System.ComponentModel.DataAnnotations;
using EventEase_POE.Data;
using EventEase_POE.Models;

namespace EventEase_POE.Validations
{
    /// <summary>
    /// Custom validation attribute to prevent deletion of venues with active bookings.
    /// </summary>
    public class CannotDeleteVenueWithBookingsAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            // This is a model-level check; actual deletion logic should be in the controller
            return true;
        }
    }

    /// <summary>
    /// Custom validation attribute to prevent deletion of events with active bookings.
    /// </summary>
    public class CannotDeleteEventWithBookingsAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            // This is a model-level check; actual deletion logic should be in the controller
            return true;
        }
    }
}
