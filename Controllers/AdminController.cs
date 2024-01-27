using Hunt.Controllers;
using Hunt.Models;
using Microsoft.AspNetCore.Hosting;
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
using API_Adda.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
//test
namespace API_Adda.Controllers
{
    public class AdminController : Controller
    {
        AdminRepository Adminrepository = new AdminRepository();
        AdminMaster adminMaster = new AdminMaster();
        displayUsermaster disUsermaster = new displayUsermaster();
        displayAppmaster disAppmaster = new displayAppmaster();
        HomeController homeController = new HomeController();
        SqlConnection sqlCon = new SqlConnection(Startup.connectionstring);
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Admins()
        {
            ModelState.Clear();
            try
            {
                adminMaster = Adminrepository.AllAdminMaster();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return View(adminMaster);
        }
        [HttpPost]
        public async Task<IActionResult> Admins(AdminMaster adminMaster, string button = null)
        {
            ModelState.Clear();
            return RedirectToAction("Admins", "Admin");
            //return View(adminMaster);
        }
        [HttpPost]
        public JsonResult GetUserDetail(string data)
        {
            try
            {
                disUsermaster = Adminrepository.UserMasterEdit(data);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return new JsonResult(disUsermaster);
        }
        [HttpPost]
        public JsonResult GetAppDetail(string data)
        {

            try
            {
                disAppmaster = Adminrepository.AppMasterEdit(data);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return new JsonResult(disAppmaster);
        }

        public JsonResult GetAppDetailpoc(string appId, string appName)
        {

            try
            {
                disAppmaster = Adminrepository.AppMasterEditpoc(appId, appName);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return new JsonResult(disAppmaster);
        }
        [HttpPost]
        public JsonResult AddUpdateAppMaster(displayAppmaster disAppmaster)
        {
            string result = "";
            try
            {
                result = Adminrepository.AddUpdateAPPsMaster(disAppmaster, HttpContext.Session.GetString("EmpId"));
                if (disAppmaster.btnValue == "Submit")
                {
                    homeController.CaptureProductivityDetails(sqlCon, HttpContext.Session.GetString("EmpId").ToString(), "Admin", "API ADDA", 1, "Admin Insert AppMaster", "Admin Insert AppMaster - " + HttpContext.Session.GetString("EmpId").ToString());
                }
                else if (disAppmaster.btnValue == "Update")
                {
                    homeController.CaptureProductivityDetails(sqlCon, HttpContext.Session.GetString("EmpId").ToString(), "Admin", "API ADDA", 1, "Admin Update AppMaster", "Admin Update AppMaster - " + HttpContext.Session.GetString("EmpId").ToString());
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return new JsonResult(result);
        }
        [HttpPost]
        public JsonResult AddUpdateUserMaster(displayUsermaster disUsermaster)
        {
            string result = "";
            try
            {
                result = Adminrepository.AddUpdateUsersMaster(disUsermaster, HttpContext.Session.GetString("EmpId"));
                if (disUsermaster.btnValue == "Submit")
                {
                    homeController.CaptureProductivityDetails(sqlCon, HttpContext.Session.GetString("EmpId").ToString(), "Admin", "API ADDA", 1, "Admin Insert UserMaster", "Admin Insert UserMaster - " + HttpContext.Session.GetString("EmpId").ToString());
                }
                else if (disUsermaster.btnValue == "Update")
                {
                    homeController.CaptureProductivityDetails(sqlCon, HttpContext.Session.GetString("EmpId").ToString(), "Admin", "API ADDA", 1, "Admin Update UserMaster", "Admin Update UserMaster - " + HttpContext.Session.GetString("EmpId").ToString());
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return new JsonResult(result);
        }
        [HttpPost]
        public JsonResult GetUserName(string UserID)
        {
            GetUserDetail UserDetail = new GetUserDetail();
            try
            {
                UserDetail = Adminrepository.GetUserNames(UserID);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return new JsonResult(UserDetail);
        }
    }
}
