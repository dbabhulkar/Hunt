using Hunt.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Data;
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
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API_Adda.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API_Adda.Controllers
{
    public class PartnerIntegrationController : Controller
    {
        PartnerRepository submitrepository_Obj = new PartnerRepository();
        HomeController homeController = new HomeController();
        SqlConnection sqlCon = new SqlConnection(Startup.connectionstring);
        private readonly string uploadFolderPath;
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddEditPartnerIntegration()
        {
            PartnerRepository submitrepository_Obj = new PartnerRepository();

            var PartnerTypeList = (from product in submitrepository_Obj.GetPartnerTypeList()
                                   select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                   { Text = product.PartnerType, Value = product.PartnerType }).ToList();
            ViewBag.Listofproducts = PartnerTypeList;

            return View();
        }
        public IActionResult ListofPartnerIntegration(string Status = null)
        {
            if (HttpContext.Session.GetString("Role").ToString() == "USER")
            {
                PartnerOnboarding partnerOnboarding = new PartnerOnboarding();

                partnerOnboarding.lstPartnerDetail = submitrepository_Obj.GetPartnerList();
                partnerOnboarding.lstPartnerDetail = partnerOnboarding.lstPartnerDetail.OrderByDescending(x => x.CaseID).ToList();

                ViewBag.Created = partnerOnboarding.lstPartnerDetail.Where(x => x.statusDescription == "Created" || x.statusDescription == "Submit").Count();
                ViewBag.Drafted = partnerOnboarding.lstPartnerDetail.Where(x => x.statusDescription == "Drafted").Count();
                ViewBag.InProgress = partnerOnboarding.lstPartnerDetail.Where(x => x.statusDescription == "In Progress").Count();
                ViewBag.ApprovedReject = partnerOnboarding.lstPartnerDetail.Where(x => x.statusDescription == "Approved" || x.statusDescription == "Reject").Count();
                ViewBag.FeedbackReply = partnerOnboarding.lstPartnerDetail.Where(x => x.statusDescription == "Awaiting For Reply").Count();
                ViewBag.Status = "False";
                if (Status == "1")
                {
                    partnerOnboarding.lstPartnerDetail = partnerOnboarding.lstPartnerDetail.Where(x => x.statusDescription == "Created" || x.statusDescription == "Submit").OrderByDescending(x => x.CaseID).ToList();
                }
                else if (Status == "2")
                {
                    partnerOnboarding.lstPartnerDetail = partnerOnboarding.lstPartnerDetail.Where(x => x.statusDescription == "Drafted").OrderByDescending(x => x.CaseID).ToList();
                }
                else if (Status == "3")
                {
                    partnerOnboarding.lstPartnerDetail = partnerOnboarding.lstPartnerDetail.Where(x => x.statusDescription == "In Progress").OrderByDescending(x => x.CaseID).ToList();
                }
                else if (Status == "4")
                {
                    partnerOnboarding.lstPartnerDetail = partnerOnboarding.lstPartnerDetail.Where(x => x.statusDescription == "Awaiting For Reply").OrderByDescending(x => x.CaseID).ToList();
                }
                else if (Status == "5")
                {
                    partnerOnboarding.lstPartnerDetail = partnerOnboarding.lstPartnerDetail.Where(x => x.statusDescription == "Approved" || x.statusDescription == "Reject").OrderByDescending(x => x.CaseID).ToList();
                }
                else
                {
                    ViewBag.Status = "True";
                }

                return View(partnerOnboarding);
            }
            else
            {
                return RedirectToAction("Index", "PartnerApproval");
            }
        }
        public JsonResult GetMstPartnerType(string PartnerType, string PartnerSubType, string PartnerEntityType, string IdentFlag)
        {
            PartnerRepository submitrepository_Obj = new PartnerRepository();
            var objMstPartnerType = submitrepository_Obj.GetMstPartnerType(PartnerType, PartnerSubType, PartnerEntityType, IdentFlag);
            var lstMstPartnerType = JsonConvert.SerializeObject(objMstPartnerType.lstMstPartnerType);
            return Json(lstMstPartnerType);
        }
        public IActionResult ViewPartneronintegration(string id)
        {
            var PartnerTypeList = (from product in submitrepository_Obj.GetPartnerTypeList()
                                   select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                   { Text = product.PartnerType, Value = product.PartnerType }).ToList();

            ViewBag.Listofproducts = PartnerTypeList;

            var partnerApproval = submitrepository_Obj.GetPartnerApprovalDeatil(id);
            partnerApproval.lstPOFeedbackHistory = submitrepository_Obj.GetFeedbackHistory(id);
            partnerApproval.lstApiDeatil = submitrepository_Obj.GetPartnerApprovalAPIDeatil(id);
            partnerApproval.CaseID = id;

            partnerApproval.PartnerApproval = submitrepository_Obj.GetApprovalTrailDeatil(id);

            partnerApproval.HOPP_FH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOPP").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOPP_VH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOPP").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOPP_GH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOPP").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOB_FH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOB_VH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOB_GH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HODB_FH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HODB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HODB_VH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HODB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HODB_GH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HODB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOISG_FH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOISG").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOISG_VH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOISG").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOISG_GH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOISG").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOITDRM_FH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOITDRM").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOITDRM_VH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOITDRM").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOITDRM_GH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOITDRM").Select(x => x.ApproverUserID).FirstOrDefault();


            partnerApproval.HOPP_FH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOPP" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOPP_VH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOPP" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOPP_GH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOPP" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOB_FH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOB" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOB_VH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOB" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOB_GH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOB" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HODB_FH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HODB" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HODB_VH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HODB" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HODB_GH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HODB" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOISG_FH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOISG" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOISG_VH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOISG" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOISG_GH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOISG" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOITDRM_FH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOITDRM" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOITDRM_VH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOITDRM" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOITDRM_GH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOITDRM" && x.Status != null).Select(x => x.Status).FirstOrDefault();

            return View(partnerApproval);
        }
        public IActionResult EditPartneronIntegration(string id, string status)
        {
            var PartnerTypeList = (from product in submitrepository_Obj.GetPartnerTypeList()
                                   select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                   { Text = product.PartnerType, Value = product.PartnerType }).ToList();

            ViewBag.Listofproducts = PartnerTypeList;

            var partnerApproval = submitrepository_Obj.GetPartnerApprovalDeatil(id);
            partnerApproval.lstPOFeedbackHistory = submitrepository_Obj.GetFeedbackDeatil(id);
            partnerApproval.lstApiDeatil = submitrepository_Obj.GetPartnerApprovalAPIDeatil(id);
            partnerApproval.CaseID = id;

            partnerApproval.PartnerApproval = submitrepository_Obj.GetApprovalTrailDeatil(id);
            partnerApproval.statusDesc = status;

            partnerApproval.HOPP_FH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOPP").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOPP_VH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOPP").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOPP_GH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOPP").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOB_FH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOB_VH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOB_GH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HODB_FH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HODB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HODB_VH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HODB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HODB_GH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HODB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOISG_FH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOISG").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOISG_VH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOISG").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOISG_GH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOISG").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOITDRM_FH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOITDRM").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOITDRM_VH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOITDRM").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOITDRM_GH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOITDRM").Select(x => x.ApproverUserID).FirstOrDefault();

            partnerApproval.HOPP_FH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOPP" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOPP_VH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOPP" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOPP_GH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOPP" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOB_FH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOB" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOB_VH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOB" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOB_GH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOB" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HODB_FH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HODB" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HODB_VH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HODB" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HODB_GH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HODB" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOISG_FH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOISG" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOISG_VH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOISG" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOISG_GH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOISG" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOITDRM_FH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOITDRM" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOITDRM_VH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOITDRM" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOITDRM_GH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOITDRM" && x.Status != null).Select(x => x.Status).FirstOrDefault();

            return View(partnerApproval);
        }
        public IActionResult EditPartneronIntegrationNew(string id, string status)
        {
            var PartnerTypeList = (from product in submitrepository_Obj.GetPartnerTypeList()
                                   select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                   { Text = product.PartnerType, Value = product.PartnerType }).ToList();

            ViewBag.Listofproducts = PartnerTypeList;

            var partnerApproval = submitrepository_Obj.GetPartnerApprovalDeatil(id);
            partnerApproval.lstPOFeedbackHistory = submitrepository_Obj.GetFeedbackDeatil(id);
            partnerApproval.lstApiDeatil = submitrepository_Obj.GetPartnerApprovalAPIDeatil(id);
            partnerApproval.CaseID = id;

            partnerApproval.PartnerApproval = submitrepository_Obj.GetApprovalTrailDeatil(id);
            partnerApproval.statusDesc = status;

            partnerApproval.HOPP_FH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOPP").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOPP_VH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOPP").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOPP_GH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOPP").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOB_FH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOB_VH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOB_GH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HODB_FH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HODB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HODB_VH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HODB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HODB_GH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HODB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOISG_FH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOISG").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOISG_VH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOISG").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOISG_GH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOISG").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOITDRM_FH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOITDRM").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOITDRM_VH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOITDRM").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOITDRM_GH = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOITDRM").Select(x => x.ApproverUserID).FirstOrDefault();

            partnerApproval.HOPP_FH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOPP" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOPP_VH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOPP" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOPP_GH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOPP" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOB_FH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOB" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOB_VH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOB" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOB_GH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOB" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HODB_FH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HODB" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HODB_VH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HODB" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HODB_GH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HODB" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOISG_FH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOISG" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOISG_VH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOISG" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOISG_GH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOISG" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOITDRM_FH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "FH" && x.Department == "HOITDRM" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOITDRM_VH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "VH" && x.Department == "HOITDRM" && x.Status != null).Select(x => x.Status).FirstOrDefault();
            partnerApproval.HOITDRM_GH_Status = partnerApproval.PartnerApproval.Where(x => x.ApproverLevel == "GH" && x.Department == "HOITDRM" && x.Status != null).Select(x => x.Status).FirstOrDefault();

            return View(partnerApproval);
        }
        public ActionResult DeletePartneronBoarding(string id)
        {
            try
            {
                bool isDeleted = submitrepository_Obj.DeletePartner(id);
                if (isDeleted)
                {
                    TempData["Result"] = "Data Deleted Successfully";
                    TempData["IsSuccess"] = "True";
                }
                else
                {
                    TempData["Result"] = "Failed to delete data";
                    TempData["IsSuccess"] = "False";
                }
                homeController.CaptureProductivityDetails(sqlCon, HttpContext.Session.GetString("EmpId").ToString(), "Partner Onboarding", "API ADDA", 1, "Partner Onboarding Delete", "Partner Onboarding Delete - " + HttpContext.Session.GetString("EmpId").ToString());

                return RedirectToAction("ListofPartner", new { Status = "2" });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void DeleteFile(string fileName)
        {
            string[] supportedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".pdf", ".docx", ".xlsx", ".txt", ".xls", ".zip", ".7z", ".doc" };

            foreach (string extension in supportedExtensions)
            {
                string filePath = Path.Combine(uploadFolderPath, fileName + extension);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }
        public void CaptureProductivityDetails(SqlConnection Con, string Empcode, string Form_Name, string Module_Name, int Total_Count, string Activity, string Activity_Details)
        {
            SqlCommand cmd = null;
            try
            {
                if (Con.State == ConnectionState.Closed) { Con.Open(); }

                cmd = new SqlCommand("USP_Insert_Data_In_Activity_Log_Tracker_API_Adda", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Emp_Code", SqlDbType.Text).Value = Empcode;
                cmd.Parameters.Add("@Form_Name", SqlDbType.Text).Value = Form_Name;
                cmd.Parameters.Add("@Module_Name", SqlDbType.Text).Value = Module_Name;
                cmd.Parameters.Add("@Total_Count", SqlDbType.Int).Value = Total_Count;
                cmd.Parameters.Add("@Activity", SqlDbType.Text).Value = Activity;
                cmd.Parameters.Add("@Activity_Details", SqlDbType.Text).Value = Activity_Details;


                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
                cmd.Dispose();



                if (Con.State == ConnectionState.Open) { Con.Close(); }
            }
            catch (Exception)
            { throw; }
            finally
            {
                if (Con.State == ConnectionState.Open) { Con.Close(); }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
            }
        }
    }
}