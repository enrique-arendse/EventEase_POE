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
			return View("~/Views/Shared/Login.cshtml");
		}

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null || !PasswordHasher.VerifyPassword(user.PasswordHash, password))
            {
                ViewBag.Error = "Invalid email or password.";
                return View("~/Views/Shared/Login.cshtml");
            }

            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserName", user.Name);

            // After successful login redirect to Home Index where venues, events and bookings are displayed
            return RedirectToAction("Index", "Home");
        }

		// ================= Sign Up =================

		[HttpGet]
		public IActionResult SignUp()
		{
			// render the shared SignUp view
			return View("~/Views/Shared/SignUp.cshtml");
		}

        [HttpPost]
        public IActionResult SignUp(User user, string Password, string role)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ViewBag.Error = "Email already exists.";
                return View("~/Views/Shared/SignUp.cshtml");
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