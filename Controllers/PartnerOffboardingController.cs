using Hunt.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

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
    public class PartnerOffboardingController : Controller
    {
        private readonly string uploadFolderPath;
        public PartnerOffboardingController()
        {
            string solutionDirectory = AppDomain.CurrentDomain.BaseDirectory;
            uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\UploadPOFB\\");
            Directory.CreateDirectory(uploadFolderPath); // Ensure the folder exists
        }
        PartnerOffboardingRepository OffboardingRepository = new PartnerOffboardingRepository();
        public IActionResult PartnerOffboarding()
        {
            return View();
        }
        [HttpPost]
        public JsonResult PartnerOffboardingDetails(string PartnerName)
        {
            PartnerOffboarding partnerOffboarding = new PartnerOffboarding();
            partnerOffboarding = OffboardingRepository.GetPartnername();
            var partnerNM = partnerOffboarding.lstPARTNER_NAME.Where(x => x.PARTNER_NAME.ToLower().Contains(PartnerName.ToLower())).ToList();
            return Json(partnerNM);
        }
        [HttpPost]
        public JsonResult PartnerOffboardingapiname(string PartnerName)
        {
            string selectedPartnerName = PartnerName;
            PartnerOffboarding partnerOffboarding = new PartnerOffboarding();
            partnerOffboarding = OffboardingRepository.GetApiname(selectedPartnerName);
            return Json(partnerOffboarding.lstAPIName);
        }

        //[HttpPost]
        //public ActionResult addAPIList(string array)
        //{
        //    try
        //    {
        //        var partnerOffboardingRepository = new PartnerOffboardingRepository();
        //        PartnerOffboarding partnerOffboarding = new PartnerOffboarding();
        //        var apiNamesObject = JsonConvert.DeserializeObject<PartnerOffboarding>(array);
        //        IEnumerable<string> apiNames = apiNamesObject.lstAPIName?.Select(item => item.APIName);
        //        foreach (var apiName in apiNames)
        //        {
        //            var obj = partnerOffboardingRepository.DeleteAPI(apiName);
        //        }

        //        return RedirectToAction("Index", "PartnerApproval");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log or handle the exception appropriately
        //        return RedirectToAction("Index", "PartnerApproval");
        //        // return Json(new { success = false, error = ex.Message });
        //    }
        //}
        [HttpPost]

        public ActionResult AddAPIList(string array)
        {
            try
            {
                var partnerOffboardingRepository = new PartnerOffboardingRepository();
                var partnerOffboarding = new PartnerOffboarding();
                var apiNamesObject = JsonConvert.DeserializeObject<PartnerOffboarding>(array);
                IEnumerable<string> apiNames = apiNamesObject.lstAPIName?.Select(item => item.APIName);

                foreach (var apiName in apiNames)
                {
                    var obj = partnerOffboardingRepository.UpdateAPIStatus(apiName, false); // Pass false to set status as Inactive
                }

                return RedirectToAction("Index", "PartnerApproval");
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                return RedirectToAction("Index", "PartnerApproval");
            }
        }


        public class ApiNamesObject
        {
            public IEnumerable<string> APINames { get; set; }
        }

        public IActionResult Upload(PartnerOffboarding model)
        {
            if (model.Uploadfile == null || model.Uploadfile.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a file.");
                return View("Index");
            }

            // Check file extension
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg" };
            var fileExtension = Path.GetExtension(model.Uploadfile.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("File", "Invalid file type. Allowed types are pdf and jpg.");
                return View("Index");
            }

            // Save the file
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine("wwwroot/uploads", uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                model.Uploadfile.CopyTo(fileStream);
            }

            // Process the file as needed (e.g., save to database)

            ViewBag.Message = "File uploaded successfully!";
            return View("Index");
        }
        public IActionResult OtherUpload(PartnerOffboarding model)
        {
            if (model.OtherUploadfile == null || model.OtherUploadfile.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a file.");
                return View("Index");
            }

            // Check file extension
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg" };
            var fileExtension = Path.GetExtension(model.OtherUploadfile.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("File", "Invalid file type. Allowed types are pdf and jpg.");
                return View("Index");
            }

            // Save the file
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine("wwwroot/uploads", uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                model.Uploadfile.CopyTo(fileStream);
            }

            // Process the file as needed (e.g., save to database)

            ViewBag.Message = "File uploaded successfully!";
            return View("Index");
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
                    string filePath = Path.Combine(uploadFolderPath, CaseId + "_" + FileNameNew + "_" + DateTime.Now.ToString("ddMMMyyyy"));


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
        public IActionResult clearsubmit()
        {
            var model = new PartnerOffboarding();
            return View(model);
        }
        [HttpPost]
        public IActionResult clearsubmit(PartnerOffboarding model)
        {
            model.ExitScenario = string.Empty;
            model.DropdownList = null;
            model.lstPARTNER_NAME = null;
            return View(model);
        }

    }
}