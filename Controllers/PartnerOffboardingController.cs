using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using API_HUNT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API_HUNT.Controllers
{
    public class PartnerOffboardingController : Controller
    {
        private readonly IPartnerOffboardingRepository _offboardingRepo;
        private readonly string uploadFolderPath;

        public PartnerOffboardingController(IPartnerOffboardingRepository offboardingRepo)
        {
            _offboardingRepo = offboardingRepo;
            uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\UploadPOFB\\");
            Directory.CreateDirectory(uploadFolderPath);
        }

        public IActionResult PartnerOffboarding() => View();

        [HttpPost]
        public JsonResult PartnerOffboardingDetails(string PartnerName)
        {
            PartnerOffboarding partnerOffboarding = _offboardingRepo.GetPartnername();
            var partnerNM = partnerOffboarding.lstPARTNER_NAME.Where(x => x.PARTNER_NAME.ToLower().Contains(PartnerName.ToLower())).ToList();
            return Json(partnerNM);
        }

        [HttpPost]
        public JsonResult PartnerOffboardingapiname(string PartnerName)
        {
            PartnerOffboarding partnerOffboarding = _offboardingRepo.GetApiname(PartnerName);
            return Json(partnerOffboarding.lstAPIName);
        }

        [HttpPost]
        public ActionResult AddAPIList(string array)
        {
            try
            {
                var apiNamesObject = JsonConvert.DeserializeObject<PartnerOffboarding>(array);
                IEnumerable<string> apiNames = apiNamesObject.lstAPIName?.Select(item => item.APIName) ?? Enumerable.Empty<string>();
                foreach (var apiName in apiNames)
                    _offboardingRepo.UpdateAPIStatus(apiName, false);
                return RedirectToAction("Index", "PartnerApproval");
            }
            catch
            {
                return RedirectToAction("Index", "PartnerApproval");
            }
        }

        public IActionResult Upload(PartnerOffboarding model)
        {
            if (model.Uploadfile == null || model.Uploadfile.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a file.");
                return View("Index");
            }
            var allowed = new[] { ".pdf", ".jpg", ".jpeg" };
            var ext = Path.GetExtension(model.Uploadfile.FileName).ToLower();
            if (!allowed.Contains(ext))
            {
                ModelState.AddModelError("File", "Invalid file type. Allowed types are pdf and jpg.");
                return View("Index");
            }
            var filePath = Path.Combine("wwwroot/uploads", $"{Guid.NewGuid()}{ext}");
            using (var stream = new FileStream(filePath, FileMode.Create))
                model.Uploadfile.CopyTo(stream);
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
            var allowed = new[] { ".pdf", ".jpg", ".jpeg" };
            var ext = Path.GetExtension(model.OtherUploadfile.FileName).ToLower();
            if (!allowed.Contains(ext))
            {
                ModelState.AddModelError("File", "Invalid file type. Allowed types are pdf and jpg.");
                return View("Index");
            }
            var filePath = Path.Combine("wwwroot/uploads", $"{Guid.NewGuid()}{ext}");
            using (var stream = new FileStream(filePath, FileMode.Create))
                model.OtherUploadfile.CopyTo(stream);
            ViewBag.Message = "File uploaded successfully!";
            return View("Index");
        }

        public IActionResult clearsubmit() => View(new PartnerOffboarding());

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
