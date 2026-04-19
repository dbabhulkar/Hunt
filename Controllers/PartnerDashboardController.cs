using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API_HUNT.Controllers
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