using Hunt.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Hunt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API_Adda.Controllers
{
    public class PartnerDashboardController : Controller
    {
        public IActionResult PartnerDashboard()
        {
            return View();
        }
        public IActionResult PartnerDashboardNew()
        {
            return View();
        }
    }
}