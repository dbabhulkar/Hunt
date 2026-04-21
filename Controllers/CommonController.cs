using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using System.Linq;
using System.Threading.Tasks;
using API_HUNT;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace OneViewIndicator.Controllers
{
    public class CommonController : Controller
    {
        MySqlConnection sqlCon = new MySqlConnection(Startup.connectionstring);
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SessionExpiry()
        {
            return PartialView("_SessionExpiry");
        }

    }
}