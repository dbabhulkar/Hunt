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
    public class Partner_IntegrationController : Controller
    {
        PartnerRepository submitrepository_Obj = new PartnerRepository();
        HomeController homeController = new HomeController();
        SqlConnection sqlCon = new SqlConnection(Startup.connectionstring);
        private readonly string uploadFolderPath;
        public Partner_IntegrationController()
        {
            string solutionDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //uploadFolderPath = Path.Combine(solutionDirectory, "UploadPO");
            uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\UploadPO\\");
            Directory.CreateDirectory(uploadFolderPath); // Ensure the folder exists
        }
        public IActionResult Index()
        {
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
        public IActionResult EditPartner(string id)
        {
            return View();
        }
        public IActionResult AddEditIntegration(PartnerOnboarding PartnerOnboarding/*, string PartnerRiskClassification, 
            string APIRiskClassification, string ApproverType,string ApproverLevel*/)
        {
            PartnerRepository submitrepository_Obj = new PartnerRepository();
            PartnerOnboarding = submitrepository_Obj.partnertype();
            //PartnerOnboarding = submitrepository_Obj.PartnerCaseApprovalMetrix(PartnerRiskClassification, APIRiskClassification, ApproverType, ApproverLevel);
            return View(PartnerOnboarding);
        }
        [HttpPost]
        public IActionResult AddEditPartner(string data)
        {
            return View();
        }
        public ActionResult PartnerOnboarding_redirect()
        {
            PartnerOnboarding obj = new PartnerOnboarding();
            return View(obj);
        }
        public IActionResult sentApproval()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SaveAddPartner(PartnerOnboarding objpob, string txt_partnerName, string txt_projectdescrip, DateTime txt_TentativeGoLiveDate,
            string lstPartnerType, string txtPartnerEntityType, string txt_PartnerTPRMApplication, string PartnetRiskScore, string txtPartnerRisk, string txt_apiName,
            string txt_apiRisk, string txt_apiRiskScore)
        {
            try
            {
                PartnerRepository submitrepository_Obj = new PartnerRepository();
                //PartnerOnboarding = submitrepository_Obj.partnertype();

                PartnerOnboarding objPartnerOnboarding = new PartnerOnboarding();
                objPartnerOnboarding.PartnerName = txt_partnerName;
                objPartnerOnboarding.projectDescription = txt_projectdescrip;
                objPartnerOnboarding.TentativeGoLiveDate = txt_TentativeGoLiveDate;
                objPartnerOnboarding.PartnerType = lstPartnerType;
                objPartnerOnboarding.PartnerEntityType = txtPartnerEntityType;
                objPartnerOnboarding.PartnerTPRMAssesmetApplicability = txt_PartnerTPRMApplication;
                objPartnerOnboarding.PartnerRiskScore = PartnetRiskScore;
                objPartnerOnboarding.PartnerRisk = txtPartnerRisk;
                objPartnerOnboarding.APIName = txt_apiName;
                objPartnerOnboarding.APIRisk = txt_apiRisk;
                objPartnerOnboarding.APIRiskScore = txt_apiRiskScore;
                objPartnerOnboarding.IdentFlag = "AddPartnerboarding";
                objPartnerOnboarding.SpName = "Usp_APIA_PartnerOnBoarding";

                if (submitrepository_Obj.AddEditPartner(objPartnerOnboarding))
                {
                    ViewBag.Message = "Partner Onboarding details added successfully";
                }
                homeController.CaptureProductivityDetails(sqlCon, HttpContext.Session.GetString("EmpId").ToString(), "Partner Onboarding", "API ADDA", 1, "Partner Onboarding Add", "Partner Onboarding Add - " + HttpContext.Session.GetString("EmpId").ToString());

                return RedirectToAction("ListofPartner");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public JsonResult PartnerCaseApprovalMetrix(string PartnerRiskClassification, string APIRiskClassification, string ApproverType,
                                                           string ApproverLevel)
        {
            PartnerRepository submitrepository_Obj = new PartnerRepository();
            var objPartnerCaseApprovalMetrix = submitrepository_Obj.PartnerCaseApprovalMetrix(PartnerRiskClassification, APIRiskClassification, ApproverType, ApproverLevel);
            var lstPartnerCaseApprovalMetrix = JsonConvert.SerializeObject(objPartnerCaseApprovalMetrix.lstPartnerCaseApprovalMetrix);
            return Json(lstPartnerCaseApprovalMetrix);
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
        [HttpPost]
        public JsonResult GetApiDeatil(string APIName)
        {
            ApiDeatil apiDeatil = new ApiDeatil();
            PartnerRepository submitrepository_Obj = new PartnerRepository();
            try
            {
                apiDeatil = submitrepository_Obj.GetApiDeatil(APIName);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return new JsonResult(apiDeatil);
        }
        [HttpPost]
        public IActionResult SaveAddPartneronBoarding(string jsonPartnerOnboarding/*, IFormFile AttachedJourneyDocuments, IFormFile APIRiskAssessment, IFormFile OtherDocument*/, string next = null)
        {
            try
            {
                var CaseID = "";
                PartnerOnboarding objPartnerOnboarding = JsonConvert.DeserializeObject<PartnerOnboarding>(jsonPartnerOnboarding);
                if (next == "Request For Approval")
                {
                    objPartnerOnboarding.PartnerApproval = new List<AddPartnerCaseApprovalMetrixList>();
                    objPartnerOnboarding.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixList() { CaseId = "1", Department = "HOPP", ApproverLevel = "FH", ApproverUserID = objPartnerOnboarding.HOPP_FH, status = null, Sequence = "1" });
                    objPartnerOnboarding.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixList() { CaseId = "1", Department = "HOPP", ApproverLevel = "VH", ApproverUserID = objPartnerOnboarding.HOPP_VH, status = null, Sequence = "2" });
                    objPartnerOnboarding.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixList() { CaseId = "1", Department = "HOPP", ApproverLevel = "GH", ApproverUserID = objPartnerOnboarding.HOPP_GH, status = null, Sequence = "3" });
                    objPartnerOnboarding.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixList() { CaseId = "1", Department = "HOB", ApproverLevel = "FH", ApproverUserID = objPartnerOnboarding.HOB_FH, status = null, Sequence = "1" });
                    objPartnerOnboarding.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixList() { CaseId = "1", Department = "HOB", ApproverLevel = "VH", ApproverUserID = objPartnerOnboarding.HOB_VH, status = null, Sequence = "2" });
                    objPartnerOnboarding.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixList() { CaseId = "1", Department = "HOB", ApproverLevel = "GH", ApproverUserID = objPartnerOnboarding.HOB_GH, status = null, Sequence = "3" });
                    objPartnerOnboarding.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixList() { CaseId = "1", Department = "HODB", ApproverLevel = "FH", ApproverUserID = objPartnerOnboarding.HODB_FH, status = null, Sequence = "1" });
                    objPartnerOnboarding.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixList() { CaseId = "1", Department = "HODB", ApproverLevel = "VH", ApproverUserID = objPartnerOnboarding.HODB_VH, status = null, Sequence = "2" });
                    objPartnerOnboarding.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixList() { CaseId = "1", Department = "HODB", ApproverLevel = "GH", ApproverUserID = objPartnerOnboarding.HODB_GH, status = null, Sequence = "3" });
                    objPartnerOnboarding.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixList() { CaseId = "1", Department = "HOISG", ApproverLevel = "FH", ApproverUserID = objPartnerOnboarding.HOISG_FH, status = null, Sequence = "1" });
                    objPartnerOnboarding.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixList() { CaseId = "1", Department = "HOISG", ApproverLevel = "VH", ApproverUserID = objPartnerOnboarding.HOISG_VH, status = null, Sequence = "2" });
                    objPartnerOnboarding.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixList() { CaseId = "1", Department = "HOISG", ApproverLevel = "GH", ApproverUserID = objPartnerOnboarding.HOISG_GH, status = null, Sequence = "3" });
                    objPartnerOnboarding.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixList() { CaseId = "1", Department = "HOITDRM", ApproverLevel = "FH", ApproverUserID = objPartnerOnboarding.HOITDRM_FH, status = null, Sequence = "1" });
                    objPartnerOnboarding.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixList() { CaseId = "1", Department = "HOITDRM", ApproverLevel = "VH", ApproverUserID = objPartnerOnboarding.HOITDRM_VH, status = null, Sequence = "2" });
                    objPartnerOnboarding.PartnerApproval.Add(new AddPartnerCaseApprovalMetrixList() { CaseId = "1", Department = "HOITDRM", ApproverLevel = "GH", ApproverUserID = objPartnerOnboarding.HOITDRM_GH, status = null, Sequence = "3" });

                    objPartnerOnboarding.PartnerApproval = objPartnerOnboarding.PartnerApproval.Where(obj => obj.ApproverUserID != null && obj.ApproverUserID != "").ToList();

                    objPartnerOnboarding.createdBy = HttpContext.Session.GetString("EmpId").ToString();
                    objPartnerOnboarding.IdentFlag = "AddPartner";
                    objPartnerOnboarding.SpName = "Usp_APIA_PartnerOnBoarding";

                    //objPartnerOnboarding = submitrepository_Obj.AddPartner(objPartnerOnboarding);

                    var fileData = submitrepository_Obj.GetNewCaseId();

                    objPartnerOnboarding.AttachedJourneyDocuments = Request.Form.Files["AttachedJourneyDocuments"] != null ? fileData.CaseID + "_AttachedJourneyDocuments" : null;
                    objPartnerOnboarding.APIRiskAssessmentSheet = Request.Form.Files["APIRiskAssessment"] != null ? fileData.CaseID + "_APIRiskAssessment" : null;
                    objPartnerOnboarding.OtherDocument = Request.Form.Files["OtherDocument"] != null ? fileData.CaseID + "_OtherDocument" : null;

                    objPartnerOnboarding = submitrepository_Obj.AddPartner(objPartnerOnboarding);

                    if (fileData.CaseID != null)
                    {
                        StoreFileLocally(Request.Form.Files["AttachedJourneyDocuments"], fileData.CaseID, "AttachedJourneyDocuments");
                        StoreFileLocally(Request.Form.Files["APIRiskAssessment"], fileData.CaseID, "APIRiskAssessment");
                        StoreFileLocally(Request.Form.Files["OtherDocument"], fileData.CaseID, "OtherDocument");
                    }
                    CaseID = fileData.CaseID;
                    //if (1 == 1)
                    if (objPartnerOnboarding.Action == "Draft")
                    {
                        TempData["Result"] = "Draft Saved Successfully!!!";
                        TempData["IsSuccess"] = "True";
                    }
                    else
                    {
                        TempData["Result"] = "Submitted For Approval!!!";
                        TempData["IsSuccess"] = "True";
                    }
                }
                //return RedirectToAction("ListofPartner", "PartnerOnboarding");
                homeController.CaptureProductivityDetails(sqlCon, HttpContext.Session.GetString("EmpId").ToString(), "Partner Onboarding", "API ADDA", 1, "Partner Onboarding Add", "Partner Onboarding Add - " + HttpContext.Session.GetString("EmpId").ToString());
                if (objPartnerOnboarding.Action != "Draft")
                {
                    PartnerRepository repository = new PartnerRepository();
                    repository.GetPartnerSendMailDeatil(CaseID);
                }
                return Json(objPartnerOnboarding);
            }
            catch (Exception ex)
            {
                throw;
            }
            //return RedirectToAction("ListofPartner");
        }
        private void StoreFileLocally(IFormFile file, string CaseId = null, string fileName = null)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    string uniqueFileName = GetUniqueFileName(file.FileName);
                    string fileExtension = Path.GetExtension(uniqueFileName);

                    string FileNameNew = string.IsNullOrEmpty(fileName) ? GetUniqueFileName(file.FileName) : fileName;
                    FileNameNew += fileExtension;
                    string filePath = Path.Combine(uploadFolderPath, CaseId + "_" + FileNameNew);

                    //string filePath = Path.Combine(uploadFolderPath, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
            }
            catch (Exception)
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
        public IActionResult EditPartnerIntegration(string id, string status)
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
        [HttpPost]
        public IActionResult SaveEditPartner(PartnerOnboarding data)
        {
            try
            {
                data.createdBy = HttpContext.Session.GetString("EmpId").ToString();
                if (data.lstPOFeedbackHistory != null && data.lstPOFeedbackHistory.Count > 0)
                {
                    data.lstPOFeedbackHistory = data.lstPOFeedbackHistory.Where(X => X.FeedbackReply != null && X.FeedbackReply != "").ToList();
                }

                var partnerEdit = submitrepository_Obj.EditPartner(data);

                TempData["Result"] = "Data Edited Successfully";
                TempData["IsSuccess"] = "True";

                homeController.CaptureProductivityDetails(sqlCon, HttpContext.Session.GetString("EmpId").ToString(), "Partner Onboarding", "API ADDA", 1, "Partner Onboarding Add", "Partner Onboarding Add - " + HttpContext.Session.GetString("EmpId").ToString());

                return RedirectToAction("ListofPartnerIntegration", partnerEdit);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IActionResult ViewPartnerIntegration(string id)
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
        public JsonResult GetMstPartnerType(string PartnerType, string PartnerSubType, string PartnerEntityType, string IdentFlag)
        {
            PartnerRepository submitrepository_Obj = new PartnerRepository();
            var objMstPartnerType = submitrepository_Obj.GetMstPartnerType(PartnerType, PartnerSubType, PartnerEntityType, IdentFlag);
            var lstMstPartnerType = JsonConvert.SerializeObject(objMstPartnerType.lstMstPartnerType);
            return Json(lstMstPartnerType);
        }
        [HttpPost]
        public JsonResult GetlstApiDeatil(string prefix)
        {
            PartnerOnboarding partner = new PartnerOnboarding();
            partner = submitrepository_Obj.GetlstApiDeatil();
            var a = partner.lstApiDeatil.Where(x => x.APIName.ToLower().Contains(prefix.ToLower()) || x.APIName.ToLower().Contains(prefix.ToLower())).ToList();
            return Json(a);
        }
        [HttpPost]
        public ActionResult FeedbackPartial(string oFeedbackHistory)
        {
            try
            {
                POFeedbackHistory obj = JsonConvert.DeserializeObject<POFeedbackHistory>(oFeedbackHistory);
                return PartialView("_FeedbackPartial", obj);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ListofPartnerIntegration");
            }
        }
        public IActionResult ShowpopUp()
        {
            POFeedbackHistory obj = new POFeedbackHistory();
            return PartialView("_FeedbackPartial", obj);
        }
        [HttpPost]
        public IActionResult SaveFeedbackReply(string reqData, string FeedbackReply)
        {
            try
            {
                var createdBy = HttpContext.Session.GetString("EmpId").ToString();
                POFeedbackHistory obj = JsonConvert.DeserializeObject<POFeedbackHistory>(reqData);
                obj.FeedbackReply = FeedbackReply;
                obj.createdBy = createdBy;

                obj = submitrepository_Obj.SaveFeedbackReply(obj);

                TempData["Result"] = "Data Edited Successfully";
                TempData["IsSuccess"] = "True";

                homeController.CaptureProductivityDetails(sqlCon, HttpContext.Session.GetString("EmpId").ToString(), "Partner Onboarding", "API ADDA", 1, "Partner Onboarding Save Feedback Reply", "Partner Onboarding Save Feedback Reply - " + HttpContext.Session.GetString("EmpId").ToString());

                return Json(obj);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult SaveFeedback(POFeedbackHistory model)
        {
            return Json(new { success = true });
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
        public IActionResult EditPartnerIntegrationNew(string id, string status)
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
        [HttpPost]
        public IActionResult SaveEditPartnerNew(string jsonPartnerOnboarding)
        {
            try
            {
                PartnerOnboarding data = JsonConvert.DeserializeObject<PartnerOnboarding>(jsonPartnerOnboarding);
                data.createdBy = HttpContext.Session.GetString("EmpId").ToString();
                if (data.lstPOFeedbackHistory != null && data.lstPOFeedbackHistory.Count > 0)
                {
                    data.lstPOFeedbackHistory = data.lstPOFeedbackHistory.Where(X => X.FeedbackReply != null && X.FeedbackReply != "").ToList();
                }


                data.AttachedJourneyDocuments = Request.Form.Files["AttachedJourneyDocuments"] != null ? data.CaseID + "_AttachedJourneyDocuments" : null;
                data.APIRiskAssessmentSheet = Request.Form.Files["APIRiskAssessment"] != null ? data.CaseID + "_APIRiskAssessment" : null;
                data.OtherDocument = Request.Form.Files["OtherDocument"] != null ? data.CaseID + "_OtherDocument" : null;

                var partnerEdit = submitrepository_Obj.EditPartnerDraft(data);

                var AttachedJourneyDocuments = Request.Form.Files["AttachedJourneyDocuments"];
                //var APIRiskAssessment = Request.Form.Files["APIRiskAssessment"];
                var OtherDocument = Request.Form.Files["OtherDocument"];

                if (AttachedJourneyDocuments != null && AttachedJourneyDocuments.Length > 0)
                {
                    DeleteFile(data.CaseID + "_AttachedJourneyDocuments");
                    StoreFileLocally(Request.Form.Files["AttachedJourneyDocuments"], data.CaseID, "AttachedJourneyDocuments");
                }
                //if (APIRiskAssessment != null && APIRiskAssessment.Length > 0)
                //{
                //    DeleteFile(data.CaseID + "_APIRiskAssessment");
                //    StoreFileLocally(Request.Form.Files["APIRiskAssessment"], data.CaseID, "APIRiskAssessment");
                //}
                if (OtherDocument != null && OtherDocument.Length > 0)
                {
                    DeleteFile(data.CaseID + "_OtherDocument");
                    StoreFileLocally(Request.Form.Files["OtherDocument"], data.CaseID, "OtherDocument");
                }

                homeController.CaptureProductivityDetails(sqlCon, HttpContext.Session.GetString("EmpId").ToString(), "Partner Onboarding", "API ADDA", 1, "Partner Onboarding Add", "Partner Onboarding Add - " + HttpContext.Session.GetString("EmpId").ToString());

                TempData["Result"] = "Data Edited Successfully";
                TempData["IsSuccess"] = "True";

                if (data.Action != "Draft")
                {
                    PartnerRepository repository = new PartnerRepository();
                    repository.GetPartnerSendMailDeatil(data.CaseID);
                }

                return RedirectToAction("ListofPartnerIntegration", partnerEdit);
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