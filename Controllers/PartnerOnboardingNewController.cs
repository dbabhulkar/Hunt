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
    public class PartnerOnboardingNewController : Controller
    {
        HomeController homeController = new HomeController();
        SqlConnection sqlCon = new SqlConnection(Startup.connectionstring);
        private readonly string uploadFolderPath;
        public PartnerOnboardingNewController()
        {
            uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\UploadPONew\\");
            Directory.CreateDirectory(uploadFolderPath); // Ensure the folder exists
        }
        public IActionResult ListofPartner()
        {
            if (HttpContext.Session.GetString("Role").ToString() == "USER")
            {
                PartnerOnboardingNew partnerOnboarding = new PartnerOnboardingNew();
                PartnerOnboardingNewRepository partnerOnboardingNewRepository = new PartnerOnboardingNewRepository();
                partnerOnboarding.lstPartnerDetail = partnerOnboardingNewRepository.GetPartnerList();
                partnerOnboarding.lstPartnerDetail = partnerOnboarding.lstPartnerDetail.OrderByDescending(x => x.PartnerName).ToList();
                return View(partnerOnboarding);
            }
            else
            {
                return RedirectToAction("Index", "PartnerApproval");
            }
        }
        public IActionResult AddPartnerOnBoarding()
        {
            PartnerOnboardingNew partnerOnboarding = new PartnerOnboardingNew();
            PartnerOnboardingNewRepository partnerOnboardingNewRepository = new PartnerOnboardingNewRepository();

            var PartnerTypeList = (from product in partnerOnboardingNewRepository.GetPartnerTypeList()
                                   select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                   { Text = product.PartnerType, Value = product.PartnerType }).ToList();
            ViewBag.Listofproducts = PartnerTypeList;

            partnerOnboarding.PartnerLastModifiedDate = DateTime.Now;

            return View(partnerOnboarding);
        }
        public JsonResult GetMstPartnerType(string PartnerType, string PartnerSubType, string PartnerEntityType, string IdentFlag)
        {
            PartnerOnboardingNewRepository onboardingNewRepository = new PartnerOnboardingNewRepository();
            var objMstPartnerType = onboardingNewRepository.GetMstPartnerType(PartnerType, PartnerSubType, PartnerEntityType, IdentFlag);
            var lstMstPartnerType = JsonConvert.SerializeObject(objMstPartnerType.lstMstPartnerType);
            return Json(lstMstPartnerType);
        }
        public IActionResult SaveAddPartneronBoarding(string jsonPartnerOnboarding, string next = null)
        {
            try
            {
                var PartnerID = "";
                PartnerOnboardingNew objPartnerOnboarding = JsonConvert.DeserializeObject<PartnerOnboardingNew>(jsonPartnerOnboarding);
                PartnerOnboardingNewRepository onboardingNewRepository = new PartnerOnboardingNewRepository();
                objPartnerOnboarding.CreatedBy = HttpContext.Session.GetString("EmpId").ToString();
                var fileData = onboardingNewRepository.GetNewPartnerID();
                objPartnerOnboarding.ClientProfileSheet = Request.Form.Files["ClientProfileSheet"] != null ? fileData.PartnerID + "_ClientProfileSheet" : null;
                objPartnerOnboarding.PartnerRiskAssessmentSheet = Request.Form.Files["PartnerRiskAssessmentSheet"] != null ? fileData.PartnerID + "_PartnerRiskAssessmentSheet" : null;
                objPartnerOnboarding = onboardingNewRepository.AddPartner(objPartnerOnboarding);
                PartnerID = fileData.PartnerID;
                if (fileData.PartnerID != null)
                {
                    StoreFileLocally(Request.Form.Files["ClientProfileSheet"], fileData.PartnerID, "ClientProfileSheet");
                    StoreFileLocally(Request.Form.Files["PartnerRiskAssessmentSheet"], fileData.PartnerID, "PartnerRiskAssessmentSheet");
                }

                homeController.CaptureProductivityDetails(sqlCon, HttpContext.Session.GetString("EmpId").ToString(), "Partner Onboarding", "API ADDA", 1, "Partner Onboarding Add", "Partner Onboarding Add - " + HttpContext.Session.GetString("EmpId").ToString());
                return Json(objPartnerOnboarding);
            }
            catch (Exception ex)
            {
                throw;
            }
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
        public IActionResult ViewPartneronBoarding(string id)
        {
            PartnerOnboardingNewRepository partnerOnboardingNewRepository = new PartnerOnboardingNewRepository();

            var PartnerTypeList = (from product in partnerOnboardingNewRepository.GetPartnerTypeList()
                                   select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                   { Text = product.PartnerType, Value = product.PartnerType }).ToList();

            ViewBag.Listofproducts = PartnerTypeList;
            var partnerApproval = partnerOnboardingNewRepository.GetPartnerDeatil(id);
            return View(partnerApproval);
        }
        public IActionResult EditPartneronBoarding(string id)
        {
            PartnerOnboardingNewRepository partnerOnboardingNewRepository = new PartnerOnboardingNewRepository();

            var PartnerTypeList = (from product in partnerOnboardingNewRepository.GetPartnerTypeList()
                                   select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                   { Text = product.PartnerType, Value = product.PartnerType }).ToList();

            ViewBag.Listofproducts = PartnerTypeList;
            var partnerApproval = partnerOnboardingNewRepository.GetPartnerDeatil(id);
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
        public IActionResult SaveEditPartneronBoarding(string jsonPartnerOnboarding, string next = null)
        {
            try
            {
                var PartnerID = "";
                PartnerOnboardingNew objPartnerOnboarding = JsonConvert.DeserializeObject<PartnerOnboardingNew>(jsonPartnerOnboarding);
                PartnerOnboardingNewRepository onboardingNewRepository = new PartnerOnboardingNewRepository();
                objPartnerOnboarding.CreatedBy = HttpContext.Session.GetString("EmpId").ToString();
                objPartnerOnboarding.ClientProfileSheet = Request.Form.Files["ClientProfileSheet"] != null ? objPartnerOnboarding.PartnerID + "_ClientProfileSheet" : null;
                objPartnerOnboarding.PartnerRiskAssessmentSheet = Request.Form.Files["PartnerRiskAssessmentSheet"] != null ? objPartnerOnboarding.PartnerID + "_PartnerRiskAssessmentSheet" : null;
                var ClientProfileSheet = Request.Form.Files["ClientProfileSheet"];
                var PartnerRiskAssessmentSheet = Request.Form.Files["PartnerRiskAssessmentSheet"];
                objPartnerOnboarding = onboardingNewRepository.EditPartner(objPartnerOnboarding);
                if (ClientProfileSheet != null && ClientProfileSheet.Length > 0)
                {
                    DeleteFile(objPartnerOnboarding.PartnerID + "_ClientProfileSheet");
                    StoreFileLocally(Request.Form.Files["ClientProfileSheet"], objPartnerOnboarding.PartnerID, "ClientProfileSheet");
                }
                if (PartnerRiskAssessmentSheet != null && PartnerRiskAssessmentSheet.Length > 0)
                {
                    DeleteFile(objPartnerOnboarding.PartnerID + "_PartnerRiskAssessmentSheet");
                    StoreFileLocally(Request.Form.Files["PartnerRiskAssessmentSheet"], objPartnerOnboarding.PartnerID, "PartnerRiskAssessmentSheet");
                }

                homeController.CaptureProductivityDetails(sqlCon, HttpContext.Session.GetString("EmpId").ToString(), "Partner Onboarding", "API ADDA", 1, "Partner Onboarding Add", "Partner Onboarding Add - " + HttpContext.Session.GetString("EmpId").ToString());
                return Json(objPartnerOnboarding);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}