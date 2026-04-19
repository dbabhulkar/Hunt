using System;
using API_HUNT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace API_HUNT.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminRepository _adminRepo;
        private readonly IActivityLogRepository _activityLog;

        public AdminController(IAdminRepository adminRepo, IActivityLogRepository activityLog)
        {
            _adminRepo = adminRepo;
            _activityLog = activityLog;
        }

        public IActionResult Index() => View();

        public IActionResult Admins()
        {
            ModelState.Clear();
            AdminMaster adminMaster = new AdminMaster();
            try
            {
                adminMaster = _adminRepo.AllAdminMaster();
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
        }

        [HttpPost]
        public JsonResult GetUserDetail(string data)
        {
            displayUsermaster disUsermaster = new displayUsermaster();
            try
            {
                disUsermaster = _adminRepo.UserMasterEdit(data);
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
            displayAppmaster disAppmaster = new displayAppmaster();
            try
            {
                disAppmaster = _adminRepo.AppMasterEdit(data);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return new JsonResult(disAppmaster);
        }

        public JsonResult GetAppDetailpoc(string appId, string appName)
        {
            displayAppmaster disAppmaster = new displayAppmaster();
            try
            {
                disAppmaster = _adminRepo.AppMasterEditpoc(appId, appName);
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
                string empId = HttpContext.Session.GetString("EmpId") ?? string.Empty;
                result = _adminRepo.AddUpdateAPPsMaster(disAppmaster, empId);
                if (disAppmaster.btnValue == "Submit")
                    _activityLog.LogActivity(empId, "Admin", "API HUNT", 1, "Admin Insert AppMaster", "Admin Insert AppMaster - " + empId);
                else if (disAppmaster.btnValue == "Update")
                    _activityLog.LogActivity(empId, "Admin", "API HUNT", 1, "Admin Update AppMaster", "Admin Update AppMaster - " + empId);
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
                string empId = HttpContext.Session.GetString("EmpId") ?? string.Empty;
                result = _adminRepo.AddUpdateUsersMaster(disUsermaster, empId);
                if (disUsermaster.btnValue == "Submit")
                    _activityLog.LogActivity(empId, "Admin", "API HUNT", 1, "Admin Insert UserMaster", "Admin Insert UserMaster - " + empId);
                else if (disUsermaster.btnValue == "Update")
                    _activityLog.LogActivity(empId, "Admin", "API HUNT", 1, "Admin Update UserMaster", "Admin Update UserMaster - " + empId);
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
                UserDetail = _adminRepo.GetUserNames(UserID);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return new JsonResult(UserDetail);
        }
    }
}
