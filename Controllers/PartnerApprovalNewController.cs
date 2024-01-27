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
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using API_Adda.Models;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace API_Adda.Controllers
{
    public class PartnerApprovalNewController : Controller
    {
        HomeController homeController = new HomeController();
        SqlConnection sqlCon = new SqlConnection(Startup.connectionstring);
        private readonly string uploadFolderPath;
        public PartnerApprovalNewController()
        {
            uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\UploadPINew\\");
            Directory.CreateDirectory(uploadFolderPath); // Ensure the folder exists
        }
        public IActionResult ListofPartner()
        {
            ApprovalListNewModel approvalTrail = new ApprovalListNewModel();
            PartnerOnboardingNewRepository partnerOnboardingNewRepository = new PartnerOnboardingNewRepository();
            string ApproverUserID = HttpContext.Session.GetString("EmpId").ToString();
            approvalTrail.lstApprovalTrailList = partnerOnboardingNewRepository.ListofApproval(ApproverUserID);
            approvalTrail.lstApprovalTrailList = approvalTrail.lstApprovalTrailList.Where(x => x.status == "In Progress" || x.status == "Created").ToList();
            return View(approvalTrail);
        }
        public IActionResult ApprovedPartner(string id, string status)
        {

            PartnerOnboardingNewRepository PartnerRepository = new PartnerOnboardingNewRepository();
            var partnerApproval = PartnerRepository.GetApprovalDetail(id);
            partnerApproval.lstApiDeatil = PartnerRepository.GetPartnerApprovalAPIDeatil(id);
            partnerApproval.PartnerApproval = PartnerRepository.GetApprovalTrailDeatil(id);
            partnerApproval.lstPartnerOnBoardFeedbackHistory = PartnerRepository.GetFeedbackHistory(id);
            partnerApproval.Action = status;
            string ApproverUserID = HttpContext.Session.GetString("EmpId").ToString();
            partnerApproval.lstApprovalTrailDeatil = PartnerRepository.GetApprovalTrailDeatil(id, ApproverUserID);

            partnerApproval.HOPP_FH = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "FH" && x.Department == "HOPP").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOPP_VH = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "VH" && x.Department == "HOPP").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOPP_GH = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "GH" && x.Department == "HOPP").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOB_FH = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "FH" && x.Department == "HOB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOB_VH = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "VH" && x.Department == "HOB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOB_GH = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "GH" && x.Department == "HOB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HODB_FH = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "FH" && x.Department == "HODB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HODB_VH = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "VH" && x.Department == "HODB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HODB_GH = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "GH" && x.Department == "HODB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOISG_FH = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "FH" && x.Department == "HOISG").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOISG_VH = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "VH" && x.Department == "HOISG").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOISG_GH = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "GH" && x.Department == "HOISG").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOITDRM_FH = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "FH" && x.Department == "HOITDRM").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOITDRM_VH = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "VH" && x.Department == "HOITDRM").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOITDRM_GH = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "GH" && x.Department == "HOITDRM").Select(x => x.ApproverUserID).FirstOrDefault();

            partnerApproval.HOPP_FH_Name = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "FH" && x.Department == "HOPP").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOPP_VH_Name = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "VH" && x.Department == "HOPP").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOPP_GH_Name = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "GH" && x.Department == "HOPP").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOB_FH_Name = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "FH" && x.Department == "HOB").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOB_VH_Name = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "VH" && x.Department == "HOB").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOB_GH_Name = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "GH" && x.Department == "HOB").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HODB_FH_Name = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "FH" && x.Department == "HODB").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HODB_VH_Name = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "VH" && x.Department == "HODB").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HODB_GH_Name = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "GH" && x.Department == "HODB").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOISG_FH_Name = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "FH" && x.Department == "HOISG").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOISG_VH_Name = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "VH" && x.Department == "HOISG").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOISG_GH_Name = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "GH" && x.Department == "HOISG").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOITDRM_FH_Name = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "FH" && x.Department == "HOITDRM").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOITDRM_VH_Name = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "VH" && x.Department == "HOITDRM").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOITDRM_GH_Name = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverLevel == "GH" && x.Department == "HOITDRM").Select(x => x.ApproverName).FirstOrDefault();

            partnerApproval.CurrentApproverLevel = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverUserID.ToUpper() == ApproverUserID.ToUpper()).Select(x => x.ApproverLevel).FirstOrDefault();
            partnerApproval.CurrentApproverUserID = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverUserID.ToUpper() == ApproverUserID.ToUpper()).Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.CurrentSequence = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverUserID.ToUpper() == ApproverUserID.ToUpper()).Select(x => x.Sequence).FirstOrDefault();
            partnerApproval.CurrentDepartment = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverUserID.ToUpper() == ApproverUserID.ToUpper()).Select(x => x.Department).FirstOrDefault();
            partnerApproval.CurrentApprovalTrialID = partnerApproval.lstApprovalTrailDeatil.Where(x => x.ApproverUserID.ToUpper() == ApproverUserID.ToUpper()).Select(x => x.ApprovalTrialID).FirstOrDefault();

            var POFeedbackReply = PartnerRepository.GetFeedbackReply(id, partnerApproval.CurrentApprovalTrialID);
            if (POFeedbackReply.FeedbackReply != null)
            {
                partnerApproval.FeedbackReply = POFeedbackReply.FeedbackReply;
            }

            return View(partnerApproval);
        }
        [HttpPost]
        public IActionResult SaveAddPartnerApproval(PartnerApprovalNew partnerApproval)
        {
            try
            {
                var partnerApprovalRepository = new PartnerOnboardingNewRepository();
                partnerApproval.CreatedBy = HttpContext.Session.GetString("EmpId").ToString();
                partnerApproval.Action = (partnerApproval.Action == "Feedback") ? "Awaiting For Reply" : partnerApproval.Action;

                partnerApproval = partnerApprovalRepository.SaveAddPartnerApproval(partnerApproval);

                homeController.CaptureProductivityDetails(sqlCon, HttpContext.Session.GetString("EmpId").ToString(), "Partner Onboarding", "API ADDA", 1, "Partner Onboarding Save Partner " + partnerApproval.status, "Partner Onboarding Save Partner " + partnerApproval.status + " - " + HttpContext.Session.GetString("EmpId").ToString());

                return Json(partnerApproval);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}