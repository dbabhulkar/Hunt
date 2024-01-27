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
    public class PartnerIntegrationNewController : Controller
    {
        HomeController homeController = new HomeController();
        SqlConnection sqlCon = new SqlConnection(Startup.connectionstring);
        private readonly string uploadFolderPath;
        public PartnerIntegrationNewController()
        {
            uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\UploadPINew\\");
            Directory.CreateDirectory(uploadFolderPath); // Ensure the folder exists
        }
        public IActionResult ListofPartner(string Status)
        {
            if (HttpContext.Session.GetString("Role").ToString() == "USER")
            {
                PartnerIntegrationNew partnerOnboarding = new PartnerIntegrationNew();
                PartnerOnboardingNewRepository partnerOnboardingNewRepository = new PartnerOnboardingNewRepository();
                partnerOnboarding.lstPartnerIntegrationDetail = partnerOnboardingNewRepository.ListofIntegration();
                partnerOnboarding.lstPartnerIntegrationDetail = partnerOnboarding.lstPartnerIntegrationDetail.OrderByDescending(x => x.PartnerName).ToList();

                ViewBag.Created = partnerOnboarding.lstPartnerIntegrationDetail.Where(x => x.status == "Created" || x.status == "Submit").Count();
                ViewBag.Drafted = partnerOnboarding.lstPartnerIntegrationDetail.Where(x => x.status == "Drafted").Count();
                ViewBag.InProgress = partnerOnboarding.lstPartnerIntegrationDetail.Where(x => x.status == "In Progress").Count();
                ViewBag.ApprovedReject = partnerOnboarding.lstPartnerIntegrationDetail.Where(x => x.status == "Approved" || x.status == "Reject").Count();
                ViewBag.FeedbackReply = partnerOnboarding.lstPartnerIntegrationDetail.Where(x => x.status == "Awaiting For Reply").Count();
                ViewBag.Status = "False";
                if (Status == "1")
                {
                    partnerOnboarding.lstPartnerIntegrationDetail = partnerOnboarding.lstPartnerIntegrationDetail.Where(x => x.status == "Created" || x.status == "Submit").OrderByDescending(x => x.CaseID).ToList();
                }
                else if (Status == "2")
                {
                    partnerOnboarding.lstPartnerIntegrationDetail = partnerOnboarding.lstPartnerIntegrationDetail.Where(x => x.status == "Drafted").OrderByDescending(x => x.CaseID).ToList();
                }
                else if (Status == "3")
                {
                    partnerOnboarding.lstPartnerIntegrationDetail = partnerOnboarding.lstPartnerIntegrationDetail.Where(x => x.status == "In Progress").OrderByDescending(x => x.CaseID).ToList();
                }
                else if (Status == "4")
                {
                    partnerOnboarding.lstPartnerIntegrationDetail = partnerOnboarding.lstPartnerIntegrationDetail.Where(x => x.status == "Awaiting For Reply").OrderByDescending(x => x.CaseID).ToList();
                }
                else if (Status == "5")
                {
                    partnerOnboarding.lstPartnerIntegrationDetail = partnerOnboarding.lstPartnerIntegrationDetail.Where(x => x.status == "Approved" || x.status == "Reject").OrderByDescending(x => x.CaseID).ToList();
                }
                else
                {
                    ViewBag.Status = "True";
                }

                return View(partnerOnboarding);
            }
            else
            {
                return RedirectToAction("ListofPartner", "PartnerApprovalNew");
            }
        }
        public IActionResult AddPartnerIntegration()
        {
            PartnerIntegrationNew partnerOnboarding = new PartnerIntegrationNew();
            PartnerOnboardingNewRepository partnerOnboardingNewRepository = new PartnerOnboardingNewRepository();
            var PartnerNameList = (from Partner in partnerOnboardingNewRepository.GetPartnerNameList()
                                   select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                   { Text = Partner.PartnerName, Value = Partner.PartnerID }).ToList();
            ViewBag.ListPartnerName = PartnerNameList;
            return View(partnerOnboarding);
        }
        [HttpGet]
        public ActionResult GetPartnerDetails(string partnerID)
        {
            try
            {
                PartnerOnboardingNewRepository partnerOnboardingNewRepository = new PartnerOnboardingNewRepository();
                var partnerDetails = partnerOnboardingNewRepository.GetPartnerDeatilIntegration(partnerID);

                if (partnerDetails != null)
                {
                    return Json(new { partnerDetails = partnerDetails });
                }
                else
                {
                    return Json(new { error = "Partner details not found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = "An error occurred while fetching partner details: " + ex.Message });
            }
        }
        public JsonResult PartnerCaseApprovalMetrix(string PartnerRiskClassification, string APIRiskClassification, string ApproverType,
                                                           string ApproverLevel)
        {
            PartnerOnboardingNewRepository partnerOnboarding = new PartnerOnboardingNewRepository();
            var objPartnerCaseApprovalMetrix = partnerOnboarding.PartnerCaseApprovalMetrix(PartnerRiskClassification, APIRiskClassification, ApproverType, ApproverLevel);
            var lstPartnerCaseApprovalMetrix = JsonConvert.SerializeObject(objPartnerCaseApprovalMetrix.lstPartnerCaseApprovalMetrix);
            return Json(lstPartnerCaseApprovalMetrix);
        }
        [HttpPost]
        public JsonResult GetlstApiDeatil(string prefix)
        {
            PartnerOnboardingNewRepository partnerOnboarding = new PartnerOnboardingNewRepository();
            PartnerIntegrationNew partner = new PartnerIntegrationNew();
            partner = partnerOnboarding.GetlstApiDeatil();
            var a = partner.lstApiDeatil.Where(x => x.APIName.ToLower().Contains(prefix.ToLower()) || x.APIName.ToLower().Contains(prefix.ToLower())).ToList();
            return Json(a);
        }
        [HttpPost]
        public JsonResult GetApiDeatil(string APIName)
        {
            ApiDeatilNew apiDeatil = new ApiDeatilNew();
            PartnerOnboardingNewRepository PartnerRepository = new PartnerOnboardingNewRepository();
            try
            {
                apiDeatil = PartnerRepository.GetApiDeatil(APIName);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return new JsonResult(apiDeatil);
        }
        [HttpPost]
        public IActionResult SavePartnerIntegration(string jsonPartnerOnboarding)
        {
            PartnerIntegrationNew data = JsonConvert.DeserializeObject<PartnerIntegrationNew>(jsonPartnerOnboarding);
            PartnerOnboardingNewRepository PartnerRepository = new PartnerOnboardingNewRepository();
            data.CreatedBy = HttpContext.Session.GetString("EmpId").ToString();
            data.Action = data.Action == "Draft" ? "Draft" : "Created";
            var CaseID = "";

            data.PartnerApproval = new List<AddPartnerCaseApprovalMetrixListNew>();
            data.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixListNew() { CaseId = "1", Department = "HOPP", ApproverLevel = "FH", ApproverUserID = data.HOPP_FH, Status = null, Sequence = "1" });
            data.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixListNew() { CaseId = "1", Department = "HOPP", ApproverLevel = "VH", ApproverUserID = data.HOPP_VH, Status = null, Sequence = "2" });
            data.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixListNew() { CaseId = "1", Department = "HOPP", ApproverLevel = "GH", ApproverUserID = data.HOPP_GH, Status = null, Sequence = "3" });
            data.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixListNew() { CaseId = "1", Department = "HOB", ApproverLevel = "FH", ApproverUserID = data.HOB_FH, Status = null, Sequence = "1" });
            data.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixListNew() { CaseId = "1", Department = "HOB", ApproverLevel = "VH", ApproverUserID = data.HOB_VH, Status = null, Sequence = "2" });
            data.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixListNew() { CaseId = "1", Department = "HOB", ApproverLevel = "GH", ApproverUserID = data.HOB_GH, Status = null, Sequence = "3" });
            data.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixListNew() { CaseId = "1", Department = "HODB", ApproverLevel = "FH", ApproverUserID = data.HODB_FH, Status = null, Sequence = "1" });
            data.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixListNew() { CaseId = "1", Department = "HODB", ApproverLevel = "VH", ApproverUserID = data.HODB_VH, Status = null, Sequence = "2" });
            data.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixListNew() { CaseId = "1", Department = "HODB", ApproverLevel = "GH", ApproverUserID = data.HODB_GH, Status = null, Sequence = "3" });
            data.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixListNew() { CaseId = "1", Department = "HOISG", ApproverLevel = "FH", ApproverUserID = data.HOISG_FH, Status = null, Sequence = "1" });
            data.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixListNew() { CaseId = "1", Department = "HOISG", ApproverLevel = "VH", ApproverUserID = data.HOISG_VH, Status = null, Sequence = "2" });
            data.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixListNew() { CaseId = "1", Department = "HOISG", ApproverLevel = "GH", ApproverUserID = data.HOISG_GH, Status = null, Sequence = "3" });
            data.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixListNew() { CaseId = "1", Department = "HOITDRM", ApproverLevel = "FH", ApproverUserID = data.HOITDRM_FH, Status = null, Sequence = "1" });
            data.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixListNew() { CaseId = "1", Department = "HOITDRM", ApproverLevel = "VH", ApproverUserID = data.HOITDRM_VH, Status = null, Sequence = "2" });
            data.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixListNew() { CaseId = "1", Department = "HOITDRM", ApproverLevel = "GH", ApproverUserID = data.HOITDRM_GH, Status = null, Sequence = "3" });
            data.PartnerApproval = data.PartnerApproval.Where(obj => obj.ApproverUserID != null && obj.ApproverUserID != "").ToList();

            var fileData = PartnerRepository.GetNewCaseID();
            data.AttachedJourneyDocuments = Request.Form.Files["AttachedJourneyDocuments"] != null ? fileData.CaseID + "_AttachedJourneyDocuments" : null;
            data.OtherDocument = Request.Form.Files["OtherDocument"] != null ? fileData.CaseID + "_OtherDocument" : null;
            data = PartnerRepository.AddPartnerIntegration(data);
            if (fileData.CaseID != null)
            {
                StoreFileLocally(Request.Form.Files["AttachedJourneyDocuments"], fileData.CaseID, "AttachedJourneyDocuments");
                StoreFileLocally(Request.Form.Files["OtherDocument"], fileData.CaseID, "OtherDocument");
            }
            CaseID = fileData.CaseID;
            //if (data.Action != "Draft")
            //{
            //    PartnerOnboardingNewRepository repository = new PartnerOnboardingNewRepository();
            //    repository.GetPartnerSendMailDeatil(data.CaseID);
            //}
            if (data.Action == "Draft")
            {
                TempData["Result"] = "Draft Saved Successfully!!!";
                TempData["IsSuccess"] = "True";
            }
            else
            {
                TempData["Result"] = "Submitted For Approval!!!";
                TempData["IsSuccess"] = "True";
            }

            return View();
        }
        private void StoreFileLocally(IFormFile file, string PartnerID = null, string fileName = null)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    string uniqueFileName = GetUniqueFileName(file.FileName);
                    string fileExtension = Path.GetExtension(uniqueFileName);

                    string FileNameNew = string.IsNullOrEmpty(fileName) ? GetUniqueFileName(file.FileName) : fileName;
                    FileNameNew += fileExtension;
                    string filePath = Path.Combine(uploadFolderPath, PartnerID + "_" + FileNameNew);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private string GetUniqueFileName(string fileName)
        {
            string baseFileName = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            string uniqueFileName = baseFileName + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;

            // Check if the file already exists
            int count = 1;
            while (System.IO.File.Exists(Path.Combine(uploadFolderPath, uniqueFileName)))
            {
                uniqueFileName = baseFileName + DateTime.Now.ToString("yyyyMMddHHmmssfff") + count + extension;
                count++;
            }

            return uniqueFileName;
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
        public IActionResult EditPartnerIntegration(string id, string status)
        {
            PartnerOnboardingNewRepository PartnerRepository = new PartnerOnboardingNewRepository();
            var partnerApproval = PartnerRepository.GetIntegrationDetail(id);
            partnerApproval.lstApiDeatil = PartnerRepository.GetPartnerApprovalAPIDeatil(id);
            partnerApproval.PartnerApproval = PartnerRepository.GetApprovalTrailDeatil(id);
            partnerApproval.lstPOFeedbackHistory = PartnerRepository.GetFeedbackDeatil(id);
            partnerApproval.Action = status;

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
        public IActionResult ViewPartnerIntegration(string id)
        {
            PartnerOnboardingNewRepository PartnerRepository = new PartnerOnboardingNewRepository();
            var partnerApproval = PartnerRepository.GetIntegrationDetail(id);
            partnerApproval.lstApiDeatil = PartnerRepository.GetPartnerApprovalAPIDeatil(id);
            partnerApproval.PartnerApproval = PartnerRepository.GetApprovalTrailDeatil(id);

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
        public IActionResult DownloadFile(string fileName)
        {
            string[] supportedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".pdf", ".docx", ".xlsx", ".txt", ".xls", ".zip", ".7z", ".doc" }; // Add more extensions as needed

            foreach (string extension in supportedExtensions)
            {
                string filePath = Path.Combine(uploadFolderPath, fileName + extension);

                if (System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return File(fileBytes, "application/octet-stream", fileName + extension);
                }
            }

            return NotFound();
        }
        public IActionResult DisplayFile(string fileName)
        {
            string[] supportedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".pdf", ".docx", ".xlsx", ".txt", ".xls", ".zip", ".7z", ".doc" };

            foreach (string extension in supportedExtensions)
            {
                string filePath = Path.Combine(uploadFolderPath, fileName + extension);

                if (System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    if (extension == ".docx" || extension == ".xlsx"/* || extension == ".txt" */|| extension == ".xls" || extension == ".zip" || extension == ".7z" || extension == ".doc")
                    {
                        return File(fileBytes, "application/octet-stream", fileName + extension);
                    }
                    else
                    {
                        string contentType = GetContentType(extension);
                        return File(fileBytes, contentType);
                    }
                }
            }

            return NotFound();
        }
        private string GetContentType(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".pdf":
                    return "application/pdf";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".txt":
                    return "text/plain";
                case ".zip":
                    return "application/x-zip-compressed";
                case ".7z":
                    return "application/x-7z-compressed";
                case ".doc":
                    return "application/msword";
                case ".xls":
                    return "application/vnd.ms-excel";
                default:
                    return "application/octet-stream"; // Default to binary data for other file types
            }
        }
        [HttpPost]
        public IActionResult SaveEditPartnerIntegration(string jsonPartnerOnboarding)
        {
            try
            {
                PartnerIntegrationNew data = JsonConvert.DeserializeObject<PartnerIntegrationNew>(jsonPartnerOnboarding);
                data.CreatedBy = HttpContext.Session.GetString("EmpId").ToString();
                PartnerOnboardingNewRepository PartnerRepository = new PartnerOnboardingNewRepository();
                data.AttachedJourneyDocuments = Request.Form.Files["AttachedJourneyDocuments"] != null ? data.CaseID + "_AttachedJourneyDocuments" : null;
                data.OtherDocument = Request.Form.Files["OtherDocument"] != null ? data.CaseID + "_OtherDocument" : null;
                var partnerEdit = PartnerRepository.EditPartnerIntegration(data);
                var AttachedJourneyDocuments = Request.Form.Files["AttachedJourneyDocuments"];
                var OtherDocument = Request.Form.Files["OtherDocument"];
                if (AttachedJourneyDocuments != null && AttachedJourneyDocuments.Length > 0)
                {
                    DeleteFile(data.CaseID + "_AttachedJourneyDocuments");
                    StoreFileLocally(Request.Form.Files["AttachedJourneyDocuments"], data.CaseID, "AttachedJourneyDocuments");
                }
                if (OtherDocument != null && OtherDocument.Length > 0)
                {
                    DeleteFile(data.CaseID + "_OtherDocument");
                    StoreFileLocally(Request.Form.Files["OtherDocument"], data.CaseID, "OtherDocument");
                }

                homeController.CaptureProductivityDetails(sqlCon, HttpContext.Session.GetString("EmpId").ToString(), "Partner Integration", "API ADDA", 1, "Partner Integration Add", "Partner Integration Add - " + HttpContext.Session.GetString("EmpId").ToString());

                TempData["Result"] = "Data Edited Successfully";
                TempData["IsSuccess"] = "True";

                //if (data.Action != "Draft")
                //{
                //    PartnerOnboardingNewRepository repository = new PartnerOnboardingNewRepository();
                //    repository.GetPartnerSendMailDeatil(data.CaseID);
                //}

                return RedirectToAction("ListofPartnerIntegration", partnerEdit);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}