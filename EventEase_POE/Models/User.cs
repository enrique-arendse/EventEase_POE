namespace EventEase_POE.Models
{
	public class User
	{
		public int Id { get; set; }

		public string Name { get; set; } = default!;

		public string Email { get; set; } = default!;

		
		public string PasswordHash { get; set; }
		

		public string Role { get; set; } = "Booking";
		// "Admin" or "Booking"
	}
}
