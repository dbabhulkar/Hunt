using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API_HUNT.Models;

namespace API_HUNT.Controllers
{
    public class PartnerApprovalController : Controller
    {
        private readonly IPartnerOnboardingNewRepository _partnerRepo;
        private readonly IActivityLogRepository _activityLog;
        private readonly string uploadFolderPath;

        public PartnerApprovalController(IPartnerOnboardingNewRepository partnerRepo, IActivityLogRepository activityLog)
        {
            _partnerRepo = partnerRepo;
            _activityLog = activityLog;
            uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\UploadPINew\\");
            Directory.CreateDirectory(uploadFolderPath);
        }

        public IActionResult ListofPartner()
        {
            string approverUserID = HttpContext.Session.GetString("EmpId") ?? string.Empty;
            ApprovalListNewModel approvalTrail = new ApprovalListNewModel();
            approvalTrail.lstApprovalTrailList = _partnerRepo.ListofApproval(approverUserID);
            approvalTrail.lstApprovalTrailList = approvalTrail.lstApprovalTrailList
                .Where(x => x.status == "In Progress" || x.status == "Created").ToList();
            return View(approvalTrail);
        }

        public IActionResult ApprovedPartner(string id, string status)
        {
            string approverUserID = HttpContext.Session.GetString("EmpId") ?? string.Empty;
            var partnerApproval = _partnerRepo.GetApprovalDetail(id);
            partnerApproval.lstApiDetail = _partnerRepo.GetPartnerApprovalAPIDetail(id);
            partnerApproval.PartnerApproval = _partnerRepo.GetApprovalTrailDetail(id);
            partnerApproval.lstPartnerOnBoardFeedbackHistory = _partnerRepo.GetFeedbackHistory(id);
            partnerApproval.Action = status;
            partnerApproval.lstApprovalTrailDetail = _partnerRepo.GetApprovalTrailDetail(id, approverUserID);

            partnerApproval.HOPP_FH = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "FH" && x.Department == "HOPP").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOPP_VH = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "VH" && x.Department == "HOPP").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOPP_GH = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "GH" && x.Department == "HOPP").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOB_FH = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "FH" && x.Department == "HOB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOB_VH = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "VH" && x.Department == "HOB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOB_GH = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "GH" && x.Department == "HOB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HODB_FH = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "FH" && x.Department == "HODB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HODB_VH = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "VH" && x.Department == "HODB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HODB_GH = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "GH" && x.Department == "HODB").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOISG_FH = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "FH" && x.Department == "HOISG").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOISG_VH = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "VH" && x.Department == "HOISG").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOISG_GH = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "GH" && x.Department == "HOISG").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOITDRM_FH = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "FH" && x.Department == "HOITDRM").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOITDRM_VH = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "VH" && x.Department == "HOITDRM").Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.HOITDRM_GH = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "GH" && x.Department == "HOITDRM").Select(x => x.ApproverUserID).FirstOrDefault();

            partnerApproval.HOPP_FH_Name = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "FH" && x.Department == "HOPP").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOPP_VH_Name = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "VH" && x.Department == "HOPP").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOPP_GH_Name = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "GH" && x.Department == "HOPP").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOB_FH_Name = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "FH" && x.Department == "HOB").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOB_VH_Name = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "VH" && x.Department == "HOB").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOB_GH_Name = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "GH" && x.Department == "HOB").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HODB_FH_Name = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "FH" && x.Department == "HODB").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HODB_VH_Name = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "VH" && x.Department == "HODB").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HODB_GH_Name = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "GH" && x.Department == "HODB").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOISG_FH_Name = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "FH" && x.Department == "HOISG").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOISG_VH_Name = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "VH" && x.Department == "HOISG").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOISG_GH_Name = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "GH" && x.Department == "HOISG").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOITDRM_FH_Name = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "FH" && x.Department == "HOITDRM").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOITDRM_VH_Name = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "VH" && x.Department == "HOITDRM").Select(x => x.ApproverName).FirstOrDefault();
            partnerApproval.HOITDRM_GH_Name = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverLevel == "GH" && x.Department == "HOITDRM").Select(x => x.ApproverName).FirstOrDefault();

            partnerApproval.CurrentApproverLevel = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverUserID.ToUpper() == approverUserID.ToUpper()).Select(x => x.ApproverLevel).FirstOrDefault();
            partnerApproval.CurrentApproverUserID = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverUserID.ToUpper() == approverUserID.ToUpper()).Select(x => x.ApproverUserID).FirstOrDefault();
            partnerApproval.CurrentSequence = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverUserID.ToUpper() == approverUserID.ToUpper()).Select(x => x.Sequence).FirstOrDefault();
            partnerApproval.CurrentDepartment = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverUserID.ToUpper() == approverUserID.ToUpper()).Select(x => x.Department).FirstOrDefault();
            partnerApproval.CurrentApprovalTrialID = partnerApproval.lstApprovalTrailDetail.Where(x => x.ApproverUserID.ToUpper() == approverUserID.ToUpper()).Select(x => x.ApprovalTrialID).FirstOrDefault();

            var feedbackReply = _partnerRepo.GetFeedbackReply(id, partnerApproval.CurrentApprovalTrialID);
            if (feedbackReply?.FeedbackReply != null)
                partnerApproval.FeedbackReply = feedbackReply.FeedbackReply;

            return View(partnerApproval);
        }

        [HttpPost]
        public IActionResult SaveAddPartnerApproval(PartnerApprovalNew partnerApproval)
        {
            try
            {
                string empId = HttpContext.Session.GetString("EmpId") ?? string.Empty;
                partnerApproval.CreatedBy = empId;
                partnerApproval.Action = (partnerApproval.Action == "Feedback") ? "Awaiting For Reply" : partnerApproval.Action;
                partnerApproval = _partnerRepo.SaveAddPartnerApproval(partnerApproval);
                _activityLog.LogActivity(empId, "Partner Onboarding", "API HUNT", 1,
                    "Partner Onboarding Save Partner " + partnerApproval.status,
                    "Partner Onboarding Save Partner " + partnerApproval.status + " - " + empId);
                return Json(partnerApproval);
            }
            catch
            {
                throw;
            }
        }

        public IActionResult ShowImage(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return BadRequest("File name is required.");
            fileName = Path.GetFileName(fileName);

            var extensions = new[] { ".png", ".jpg", ".jpeg", ".gif" };
            foreach (var ext in extensions)
            {
                string filePath = FileSecurityHelper.GetSafePath(fileName + ext, uploadFolderPath);
                if (System.IO.File.Exists(filePath))
                    return File(new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read), "image/jpeg");
            }
            return NotFound();
        }

        public IActionResult DownloadFile(string fileName)
        {
            return ServeFile(fileName);
        }

        public IActionResult DisplayFile(string fileName)
        {
            return ServeFile(fileName, display: true);
        }

        private IActionResult ServeFile(string fileName, bool display = false)
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
                    if (!display || ext is ".docx" or ".xlsx" or ".xls" or ".zip" or ".7z" or ".doc")
                        return File(bytes, "application/octet-stream", fileName + ext);
                    return File(bytes, GetContentType(ext));
                }
            }
            return NotFound();
        }

        private static string GetContentType(string ext) => ext.ToLower() switch
        {
            ".png"  => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif"  => "image/gif",
            ".pdf"  => "application/pdf",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt"  => "text/plain",
            ".zip"  => "application/x-zip-compressed",
            ".7z"   => "application/x-7z-compressed",
            ".doc"  => "application/msword",
            ".xls"  => "application/vnd.ms-excel",
            _       => "application/octet-stream"
        };
    }
}
