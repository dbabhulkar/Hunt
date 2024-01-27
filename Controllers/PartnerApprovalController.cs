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
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API_Adda.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Adda.Controllers
{
    public class PartnerApprovalController : Controller
    {
        private readonly string uploadFolderPath;
        HomeController homeController = new HomeController();
        SqlConnection sqlCon = new SqlConnection(Startup.connectionstring);
        public PartnerApprovalController()
        {
            string solutionDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //uploadFolderPath = Path.Combine(solutionDirectory, "UploadPO");
            uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\UploadPO\\");
            Directory.CreateDirectory(uploadFolderPath); // Ensure the folder exists
        }
        public IActionResult Index()
        {
            ApprovalListModel approvalTrail = new ApprovalListModel();
            var partnerApprovalRepository = new PartnerApprovalRepository();
            string ApproverUserID = HttpContext.Session.GetString("EmpId").ToString();
            //ApproverUserID = "M3330";
            approvalTrail.lstApprovalTrailList = partnerApprovalRepository.GetApprovalTrailList(ApproverUserID);
            approvalTrail.lstApprovalTrailList = approvalTrail.lstApprovalTrailList.Where(x => x.StatusDescription == "In Progress").ToList();
            return View(approvalTrail);
        }
        public IActionResult ApprovedPartner(string id)
        {
            var partnerApprovalRepository = new PartnerApprovalRepository();
            //id = "54";
            var partnerApproval = partnerApprovalRepository.GetPartnerApprovalDeatil(id);
            partnerApproval.lstAPIDeatil = partnerApprovalRepository.GetPartnerApprovalAPIDeatil(id);
            partnerApproval.lstDepartmentStatus = partnerApprovalRepository.GetDepartmentWiseStatus(id);
            partnerApproval.lstPartnerOnBoardFeedbackHistory = partnerApprovalRepository.GetFeedbackHistory(id);

            partnerApproval.CaseID = id;
            var PartnerTypeList = (from product in partnerApprovalRepository.GetPartnerTypeList()
                                   select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                   { Text = product.PartnerType, Value = product.PartnerType }).ToList();

            ViewBag.Listofproducts = PartnerTypeList;

            string ApproverUserID = HttpContext.Session.GetString("EmpId").ToString();
            //ApproverUserID = "M3330";
            partnerApproval.lstApprovalTrailDeatil = partnerApprovalRepository.GetApprovalTrailDeatil(id, ApproverUserID);

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

            var POFeedbackReply = partnerApprovalRepository.GetFeedbackReply(id, partnerApproval.CurrentApprovalTrialID);
            if (POFeedbackReply.FeedbackReply != null)
            {
                partnerApproval.FeedbackReply = POFeedbackReply.FeedbackReply;
            }

            return View(partnerApproval);
        }
        [HttpPost]
        public IActionResult SaveAddPartnerApproval(PartnerApprovalModel partnerApproval)
        {
            try
            {
                var partnerApprovalRepository = new PartnerApprovalRepository();
                partnerApproval.createdBy = HttpContext.Session.GetString("EmpId").ToString();
                partnerApproval = partnerApprovalRepository.SaveAddPartnerApproval(partnerApproval);

                homeController.CaptureProductivityDetails(sqlCon, HttpContext.Session.GetString("EmpId").ToString(), "Partner Onboarding", "API ADDA", 1, "Partner Onboarding Save Partner " + partnerApproval.status, "Partner Onboarding Save Partner " + partnerApproval.status + " - " + HttpContext.Session.GetString("EmpId").ToString());

                //PartnerRepository repository = new PartnerRepository();
                //repository.GetPartnerSendMailDeatil(partnerApproval.CaseID);

                return Json(partnerApproval);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IActionResult ShowImage(string fileName)
        {
            var fileNamePng = fileName + ".png";
            var fileNameJpg = fileName + ".jpg";
            var fileNameJpeg = fileName + ".jpeg";
            var fileNameGif = fileName + ".gif";

            string filePathPng = Path.Combine(uploadFolderPath, fileNamePng);
            string filePathJpg = Path.Combine(uploadFolderPath, fileNameJpg);
            string filePathJpeg = Path.Combine(uploadFolderPath, fileNameJpeg);
            string filePathGif = Path.Combine(uploadFolderPath, fileNameGif);

            // Ensure the requested file exists
            //string filePath = Path.Combine(uploadFolderPath, fileName);
            if (System.IO.File.Exists(filePathPng))
            {
                FileStream fileStream = new FileStream(filePathPng, FileMode.Open, FileAccess.Read);
                return File(fileStream, "image/jpeg"); // Adjust content type based on your file type
            }
            else if (System.IO.File.Exists(filePathJpg))
            {
                FileStream fileStream = new FileStream(filePathJpg, FileMode.Open, FileAccess.Read);
                return File(fileStream, "image/jpeg");
            }
            else if (System.IO.File.Exists(filePathJpeg))
            {
                FileStream fileStream = new FileStream(filePathJpeg, FileMode.Open, FileAccess.Read);
                return File(fileStream, "image/jpeg");
            }
            else if (System.IO.File.Exists(filePathGif))
            {
                FileStream fileStream = new FileStream(filePathGif, FileMode.Open, FileAccess.Read);
                return File(fileStream, "image/jpeg");
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult DownloadFile(string fileName)
        {
            //string modifileName = fileName.Replace("APIGW0000000000", "");
            //string modifileName = fileName.Length <= 5 ? "APIGW0000000000" + fileName.PadLeft(5, '0') : fileName.Replace("APIGW0000000000", "");
            //string modifileName = fileName.Replace("APIGW0000000000", "").TrimStart('0');
            //string modifileName = fileName.Substring(13, fileName.Length - 13);

            string modifileName = "";
            string part = fileName.Substring(0, fileName.IndexOf('_'));
            if (fileName.Contains("APIGW"))
            {
                var tcount = Convert.ToInt32(part.Substring(part.Length - 4));

                modifileName = tcount > 999 ? part.Substring(12, part.Length - 12) :
                            tcount > 99 ? part.Substring(13, part.Length - 13) :
                            tcount > 9 ? part.Substring(14, part.Length - 14) :
                            part.Substring(15, part.Length - 15);
            }

            int lastIndex = fileName.LastIndexOf("_");

            if (lastIndex != -1 && lastIndex < fileName.Length - 1)
            {
                modifileName = modifileName + "_" + fileName.Substring(lastIndex + 1);
            }

            string[] supportedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".pdf", ".docx", ".xlsx", ".txt", ".xls", ".zip", ".7z", ".doc" }; // Add more extensions as needed

            foreach (string extension in supportedExtensions)
            {
                string filePath = Path.Combine(uploadFolderPath, modifileName + extension);

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
            //string modifileName = fileName.Replace("APIGW0000000000", "");
            //string modifileName = fileName.Length <= 5 ? "APIGW0000000000" + fileName.PadLeft(5, '0') : fileName.Replace("APIGW0000000000", "");
            //string modifileName = fileName.Replace("APIGW0000000000", "").TrimStart('0');
            //string modifileName = fileName.Substring(13, fileName.Length - 13);

            string modifileName = "";
            string part = fileName.Substring(0, fileName.IndexOf('_'));
            if (fileName.Contains("APIGW"))
            {
                var tcount = Convert.ToInt32(part.Substring(part.Length - 4));

                modifileName = tcount > 999 ? part.Substring(12, part.Length - 12) :
                            tcount > 99 ? part.Substring(13, part.Length - 13) :
                            tcount > 9 ? part.Substring(14, part.Length - 14) :
                            part.Substring(15, part.Length - 15);
            }

            int lastIndex = fileName.LastIndexOf("_");

            if (lastIndex != -1 && lastIndex < fileName.Length - 1)
            {
                modifileName = modifileName + "_" + fileName.Substring(lastIndex + 1);
            }

            string[] supportedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".pdf", ".docx", ".xlsx", ".txt", ".xls", ".zip", ".7z", ".doc" };

            foreach (string extension in supportedExtensions)
            {
                string filePath = Path.Combine(uploadFolderPath, modifileName + extension);

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
    }
}