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
using System.Collections.Generic;
using System.Linq;
using API_Adda.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Adda.Controllers
{
    public class ExceptionApprovalController : Controller
    {

        public IActionResult Index()
        {
            List<ExceptionDetails> lstApprovalTrailDeatil = new List<ExceptionDetails>();
            var exceptionrepository = new ExceptionRepository();
            string ApproverUserID = HttpContext.Session.GetString("EmpId").ToString();
            lstApprovalTrailDeatil = exceptionrepository.GetExceptionApproval(ApproverUserID);

            // Create an instance of AddEditExceptionModel and set lstExceptionDetails
            var model = new AddEditExceptionModel
            {
                lstExceptionDetails = lstApprovalTrailDeatil
            };

            return View(model);
        }



    }
}