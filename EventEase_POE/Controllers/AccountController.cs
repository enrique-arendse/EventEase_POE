using EventEase_POE.Data;
using EventEase_POE.Models;
using EventEase_POE.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace EventEase_POE.Controllers
{
	public class AccountController : Controller
	{

		private readonly ApplicationDbContext _context;

		public AccountController(ApplicationDbContext context)
		{
			_context = context;
		}
		

		// ================= LOGIN =================

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Login(string email, string password)
		{
			var user = _context.Users.FirstOrDefault(u => u.Email == email);

			if (user == null || !PasswordHasher.VerifyPassword(user.PasswordHash, password))
			{
				ViewBag.Error = "Invalid email or password.";
				return View();
			}

			// Store session data
			HttpContext.Session.SetString("UserRole", user.Role);
			HttpContext.Session.SetString("UserName", user.Name);

            // Redirect based on role
			if (string.Equals(user.Role, "Admin", System.StringComparison.OrdinalIgnoreCase))
			{
				return RedirectToAction("Admin", "Dashboard");
			}
			else if (string.Equals(user.Role, "Booking", System.StringComparison.OrdinalIgnoreCase) ||
					 string.Equals(user.Role, "Booking Specialist", System.StringComparison.OrdinalIgnoreCase) ||
					 string.Equals(user.Role, "BookingSpecialist", System.StringComparison.OrdinalIgnoreCase))
			{
				return RedirectToAction("BookingSpecialist", "Dashboard");
			}

			// fallback
			return RedirectToAction("Index", "Home");
		}

			// ================= Sign Up =================

			[HttpGet]
		public IActionResult SignUp()
		{
			// render the shared SignUp view
			return View();
		}

        [HttpPost]
        public IActionResult SignUp(User user, string Password, string role)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ViewBag.Error = "Email already exists.";
                return View();
            }

            // Normalize role values from the form
            user.Role = (role == "Admin") ? "Admin" : "Booking";

            // Hash and store password
            user.PasswordHash = PasswordHasher.HashPassword(Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

		// ================= LOGOUT =================

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Login");
		}
	}
}