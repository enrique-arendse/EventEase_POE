using Microsoft.AspNetCore.Mvc;

namespace EventEase_POE.Controllers
{
	public class DashboardController : Controller
	{
		public IActionResult Admin()
		{
			return View();
		}
		public IActionResult BookingSpecialist()
		{
			return View();
		}
	}
}
