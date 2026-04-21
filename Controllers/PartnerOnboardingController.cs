using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API_HUNT.Models;
using Newtonsoft.Json;

namespace API_HUNT.Controllers
{
    public class PartnerOnboardingController : Controller
    {
        private readonly IPartnerOnboardingNewRepository _partnerRepo;
        private readonly IActivityLogRepository _activityLog;
        private readonly string uploadFolderPath;

        public PartnerOnboardingController(IPartnerOnboardingNewRepository partnerRepo, IActivityLogRepository activityLog)
        {
            _partnerRepo = partnerRepo;
            _activityLog = activityLog;
            uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\UploadPONew\\");
            Directory.CreateDirectory(uploadFolderPath);
        }

        public IActionResult ListofPartner()
        {
            if (HttpContext.Session.GetString("Role") == "USER")
            {
                PartnerOnboardingNew partnerOnboarding = new PartnerOnboardingNew();
                partnerOnboarding.lstPartnerDetail = _partnerRepo.GetPartnerList();
                partnerOnboarding.lstPartnerDetail = partnerOnboarding.lstPartnerDetail.OrderByDescending(x => x.PartnerName).ToList();
                return View(partnerOnboarding);
            }
            return RedirectToAction("Index", "PartnerApproval");
        }

        public IActionResult AddPartnerOnBoarding()
        {
            PartnerOnboardingNew partnerOnboarding = new PartnerOnboardingNew();
            ViewBag.Listofproducts = _partnerRepo.GetPartnerTypeList()
                .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = p.PartnerType, Value = p.PartnerType })
                .ToList();
            partnerOnboarding.PartnerLastModifiedDate = DateTime.Now;
            return View(partnerOnboarding);
        }

        public JsonResult GetMstPartnerType(string PartnerType, string PartnerSubType, string PartnerEntityType, string IdentFlag)
        {
            var objMstPartnerType = _partnerRepo.GetMstPartnerType(PartnerType, PartnerSubType, PartnerEntityType, IdentFlag);
            var lstMstPartnerType = JsonConvert.SerializeObject(objMstPartnerType?.lstMstPartnerType);
            return Json(lstMstPartnerType);
        }

        public IActionResult SaveAddPartneronBoarding(string jsonPartnerOnboarding, string next = null)
        {
            try
            {
                string empId = HttpContext.Session.GetString("EmpId") ?? string.Empty;
                PartnerOnboardingNew objPartnerOnboarding = JsonConvert.DeserializeObject<PartnerOnboardingNew>(jsonPartnerOnboarding);

                if (objPartnerOnboarding == null)
                {
                    return BadRequest("Invalid partner onboarding data.");
                    // or: throw new ArgumentException("Failed to deserialize partner onboarding data.");
                }
                objPartnerOnboarding.CreatedBy = empId;

                var fileData = _partnerRepo.GetNewPartnerID();
                objPartnerOnboarding.ClientProfileSheet = Request.Form.Files["ClientProfileSheet"] != null ? fileData.PartnerID + "_ClientProfileSheet" : null;
                objPartnerOnboarding.PartnerRiskAssessmentSheet = Request.Form.Files["PartnerRiskAssessmentSheet"] != null ? fileData.PartnerID + "_PartnerRiskAssessmentSheet" : null;
                objPartnerOnboarding = _partnerRepo.AddPartner(objPartnerOnboarding);

                if (fileData.PartnerID != null)
                {
                    StoreFileLocally(Request.Form.Files["ClientProfileSheet"], fileData.PartnerID, "ClientProfileSheet");
                    StoreFileLocally(Request.Form.Files["PartnerRiskAssessmentSheet"], fileData.PartnerID, "PartnerRiskAssessmentSheet");
                }

                _activityLog.LogActivity(empId, "Partner Onboarding", "API HUNT", 1, "Partner Onboarding Add", "Partner Onboarding Add - " + empId);
                return Json(objPartnerOnboarding);
            }
            catch
            {
                throw;
            }
        }

        public IActionResult ViewPartneronBoarding(string id)
        {
            ViewBag.Listofproducts = _partnerRepo.GetPartnerTypeList()
                .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = p.PartnerType, Value = p.PartnerType })
                .ToList();
            return View(_partnerRepo.GetPartnerDetail(id));
        }

        public IActionResult EditPartneronBoarding(string id)
        {
            ViewBag.Listofproducts = _partnerRepo.GetPartnerTypeList()
                .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = p.PartnerType, Value = p.PartnerType })
                .ToList();
            return View(_partnerRepo.GetPartnerDetail(id));
        }

        public IActionResult SaveEditPartneronBoarding(string jsonPartnerOnboarding, string next = null)
        {
            try
            {
                string empId = HttpContext.Session.GetString("EmpId") ?? string.Empty;
                PartnerOnboardingNew objPartnerOnboarding = JsonConvert.DeserializeObject<PartnerOnboardingNew>(jsonPartnerOnboarding);
                objPartnerOnboarding.CreatedBy = empId;
                objPartnerOnboarding.ClientProfileSheet = Request.Form.Files["ClientProfileSheet"] != null ? objPartnerOnboarding.PartnerID + "_ClientProfileSheet" : null;
                objPartnerOnboarding.PartnerRiskAssessmentSheet = Request.Form.Files["PartnerRiskAssessmentSheet"] != null ? objPartnerOnboarding.PartnerID + "_PartnerRiskAssessmentSheet" : null;

                var clientProfileSheet = Request.Form.Files["ClientProfileSheet"];
                var partnerRiskSheet = Request.Form.Files["PartnerRiskAssessmentSheet"];
                objPartnerOnboarding = _partnerRepo.EditPartner(objPartnerOnboarding);

                if (clientProfileSheet != null && clientProfileSheet.Length > 0)
                {
                    DeleteFile(objPartnerOnboarding.PartnerID + "_ClientProfileSheet");
                    StoreFileLocally(clientProfileSheet, objPartnerOnboarding.PartnerID, "ClientProfileSheet");
                }
                if (partnerRiskSheet != null && partnerRiskSheet.Length > 0)
                {
                    DeleteFile(objPartnerOnboarding.PartnerID + "_PartnerRiskAssessmentSheet");
                    StoreFileLocally(partnerRiskSheet, objPartnerOnboarding.PartnerID, "PartnerRiskAssessmentSheet");
                }

                _activityLog.LogActivity(empId, "Partner Onboarding", "API HUNT", 1, "Partner Onboarding Edit", "Partner Onboarding Edit - " + empId);
                return Json(objPartnerOnboarding);
            }
            catch
            {
                throw;
            }
        }

        public IActionResult DownloadFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return BadRequest("File name is required.");
            fileName = Path.GetFileName(fileName);

            string[] extensions = { ".png", ".jpg", ".jpeg", ".gif", ".pdf", ".docx", ".xlsx", ".txt", ".xls", ".zip", ".7z", ".doc" };
            foreach (var ext in extensions)
            {
                string filePath = FileSecurityHelper.GetSafePath(fileName + ext, uploadFolderPath);
                if (System.IO.File.Exists(filePath))
                    return File(System.IO.File.ReadAllBytes(filePath), "application/octet-stream", fileName + ext);
            }
            return NotFound();
        }

        public IActionResult DisplayFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return BadRequest("File name is required.");
            fileName = Path.GetFileName(fileName);

            string[] extensions = { ".png", ".jpg", ".jpeg", ".gif", ".pdf", ".docx", ".xlsx", ".txt", ".xls", ".zip", ".7z", ".doc" };
            foreach (var ext in extensions)
            {
                string filePath = FileSecurityHelper.GetSafePath(fileName + ext, uploadFolderPath);
                if (System.IO.File.Exists(filePath))
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(filePath);
                    if (ext is ".docx" or ".xlsx" or ".xls" or ".zip" or ".7z" or ".doc")
                        return File(bytes, "application/octet-stream", fileName + ext);
                    return File(bytes, GetContentType(ext));
                }
            }
            return NotFound();
        }

        private void StoreFileLocally(IFormFile? file, string? partnerID, string? fileName)
        {
            if (file == null || file.Length == 0) return;
            string ext = Path.GetExtension(GetUniqueFileName(file.FileName));
            string finalName = (string.IsNullOrEmpty(fileName) ? GetUniqueFileName(file.FileName) : fileName) + ext;
            string filePath = Path.Combine(uploadFolderPath, partnerID + "_" + finalName);
            using var stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);
        }

        private string GetUniqueFileName(string fileName)
        {
            string baseName = Path.GetFileNameWithoutExtension(fileName);
            string ext = Path.GetExtension(fileName);
            string unique = baseName + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
            int count = 1;
            while (System.IO.File.Exists(Path.Combine(uploadFolderPath, unique)))
                unique = baseName + DateTime.Now.ToString("yyyyMMddHHmmssfff") + count++ + ext;
            return unique;
        }

        private void DeleteFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;
            fileName = Path.GetFileName(fileName);

            string[] extensions = { ".png", ".jpg", ".jpeg", ".gif", ".pdf", ".docx", ".xlsx", ".txt", ".xls", ".zip", ".7z", ".doc" };
            foreach (var ext in extensions)
            {
                string filePath = FileSecurityHelper.GetSafePath(fileName + ext, uploadFolderPath);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
        }

        private static string GetContentType(string ext) => ext.ToLower() switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };
    }
}
