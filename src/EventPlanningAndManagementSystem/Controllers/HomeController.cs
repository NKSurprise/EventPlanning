using System.Diagnostics;
using EventPlanningAndManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanningAndManagementSystem.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Events");
        }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Error403()
        {
            return View("Error403");
        }
        public IActionResult Error404()
        {
            return View("Error404");
        }

        public IActionResult Error500()
        {
            return View("Error500");
        }

    }
}
