using Microsoft.AspNetCore.Mvc;
using EventEase_POE.Data;
using System.Linq;

namespace EventEase_POE.Controllers
{
	public class UsersController : Controller
	{
		private readonly ApplicationDbContext _context;

		public UsersController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			if (HttpContext.Session.GetString("UserRole") != "Admin")
			{
				return RedirectToAction("Login", "Account");
			}

			var users = _context.Users.ToList();
			return View(users);
		}
	}
}