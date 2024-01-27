using Hunt.Models;
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
using System.Linq;
using System.Threading.Tasks;
using API_Adda.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Adda.Controllers
{
    public class ExceptionManagementController : Controller
    {
        ExceptionRepository exceptionRepository = new ExceptionRepository();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ListOfExceptions()
        {
            if (HttpContext.Session.GetString("Role").ToString() == "USER")
            {
                AddEditExceptionModel addEditExceptionModel = new AddEditExceptionModel();
                addEditExceptionModel.lstExceptionDetails = exceptionRepository.ExceptionList();
                return View(addEditExceptionModel);
            }
            else
            {
                return RedirectToAction("Index", "ExceptionApproval");
            }
        }
        [HttpPost]
        public IActionResult ListOfExceptions(string data)
        {
            return View();
        }
        public IActionResult AddEditException(string CaseId)
        {
            AddEditExceptionModel addeditexceptionModel = new AddEditExceptionModel();
            if (CaseId != null && CaseId != "" && CaseId.Contains("APIGW") == true)
            {
                addeditexceptionModel = exceptionRepository.GetExceptionDeatil(CaseId);
                addeditexceptionModel.CaseId = CaseId;
            }
            return View(addeditexceptionModel);
        }
        [HttpPost]
        public IActionResult SaveaddeditException(AddEditExceptionModel addeditexceptionModel)
        {
            var actionName = "";
            if (addeditexceptionModel.ExceptionLevel == "Level 1")
            {
                actionName = "ExceptionLevel1";
            }
            else if (addeditexceptionModel.ExceptionLevel == "Level 2")
            {
                actionName = "ExceptionLevel2";
            }
            else if (addeditexceptionModel.ExceptionLevel == "Level 3")
            {
                actionName = "ExceptionLevel3";
            }
            addeditexceptionModel.currentUser = HttpContext.Session.GetString("EmpId").ToString();
            ExceptionRepository exceptionRepository = new ExceptionRepository();
            addeditexceptionModel = exceptionRepository.SaveAddEditException(addeditexceptionModel);
            //exceptionRepository.SaveAddEditException(addeditexceptionModel);

            return RedirectToAction(actionName: actionName, addeditexceptionModel);

        }
        public IActionResult ExceptionLevel1(AddEditExceptionModel addeditexceptionModel)
        {
            ExceptionLevels exceptionLevels = new ExceptionLevels();
            exceptionLevels.CaseID = addeditexceptionModel.OriginalOnboardingGASID;

            var BusinessVerticalHeadList = (from product in exceptionRepository.BindExcpLevel1().BusinessVerticalHeadList
                                            select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                            { Text = product.Val, Value = product.ID }).ToList();
            ViewBag.BusinessVerticalHeadList = BusinessVerticalHeadList;

            var BusinessGroupHeadList = (from product in exceptionRepository.BindExcpLevel1().BusinessGroupHeadList
                                         select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                         { Text = product.Val, Value = product.ID }).ToList();
            ViewBag.BusinessGroupHeadList = BusinessGroupHeadList;

            var CIOGroupList = (from product in exceptionRepository.BindExcpLevel1().CIOGroupList
                                select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                { Text = product.Val, Value = product.ID }).ToList();
            ViewBag.CIOGroupList = CIOGroupList;

            return View(exceptionLevels);
            //return RedirectToPage("/ExceptionLevels");
        }
        public IActionResult ExceptionLevel2(AddEditExceptionModel addeditexceptionModel)
        {
            ExceptionLevels exceptionLevels = new ExceptionLevels();
            exceptionLevels.CaseID = addeditexceptionModel.OriginalOnboardingGASID;

            var BusinessVerticalHeadList = (from product in exceptionRepository.BindExcpLevel2().BusinessVerticalHeadList
                                            select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                            { Text = product.Val, Value = product.ID }).ToList();
            ViewBag.BusinessVerticalHeadList = BusinessVerticalHeadList;

            var ITDRMVerticalHeadList = (from product in exceptionRepository.BindExcpLevel2().ITDRMVerticalHeadList
                                         select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                         { Text = product.Val, Value = product.ID }).ToList();
            ViewBag.ITDRMVerticalHeadList = ITDRMVerticalHeadList;

            var BusinessGroupHeadList = (from product in exceptionRepository.BindExcpLevel2().BusinessGroupHeadList
                                         select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                         { Text = product.Val, Value = product.ID }).ToList();
            ViewBag.BusinessGroupHeadList = BusinessGroupHeadList;

            var CIOGroupList = (from product in exceptionRepository.BindExcpLevel2().CIOGroupList
                                select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                { Text = product.Val, Value = product.ID }).ToList();
            ViewBag.CIOGroupList = CIOGroupList;

            var ITDRMGroupHeadList = (from product in exceptionRepository.BindExcpLevel2().ITDRMGroupHeadList
                                      select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                      { Text = product.Val, Value = product.ID }).ToList();
            ViewBag.ITDRMGroupHeadList = ITDRMGroupHeadList;

            var CISOGroupList = (from product in exceptionRepository.BindExcpLevel2().CISOGroupList
                                 select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                 { Text = product.Val, Value = product.ID }).ToList();
            ViewBag.CISOGroupList = CISOGroupList;

            return View(exceptionLevels);
            //return RedirectToPage("/ExceptionLevels");
        }
        public IActionResult ExceptionLevel3(AddEditExceptionModel addeditexceptionModel)
        {
            ExceptionLevels exceptionLevels = new ExceptionLevels();
            exceptionLevels.CaseID = addeditexceptionModel.OriginalOnboardingGASID;

            var ITVerticalHeadList = (from product in exceptionRepository.BindExcpLevel3().ITVerticalHeadList
                                      select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                      { Text = product.Val, Value = product.ID }).ToList();
            ViewBag.ITVerticalHeadList = ITVerticalHeadList;

            var BSGVerticalHeadList = (from product in exceptionRepository.BindExcpLevel3().BSGVerticalHeadList
                                       select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                       { Text = product.Val, Value = product.ID }).ToList();
            ViewBag.BSGVerticalHeadList = BSGVerticalHeadList;

            var BusinessVerticalHeadList = (from product in exceptionRepository.BindExcpLevel3().BusinessVerticalHeadList
                                            select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                            { Text = product.Val, Value = product.ID }).ToList();
            ViewBag.BusinessVerticalHeadList = BusinessVerticalHeadList;

            var ComplianceVerticalHeadList = (from product in exceptionRepository.BindExcpLevel3().ComplianceVerticalHeadList
                                              select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                              { Text = product.Val, Value = product.ID }).ToList();
            ViewBag.ComplianceVerticalHeadList = ComplianceVerticalHeadList;

            var ITDRMVerticalHeadList = (from product in exceptionRepository.BindExcpLevel3().ITDRMVerticalHeadList
                                         select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                         { Text = product.Val, Value = product.ID }).ToList();
            ViewBag.ITDRMVerticalHeadList = ITDRMVerticalHeadList;

            var ISGVerticalHeadList = (from product in exceptionRepository.BindExcpLevel3().ISGVerticalHeadList
                                       select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                       { Text = product.Val, Value = product.ID }).ToList();
            ViewBag.ISGVerticalHeadList = ISGVerticalHeadList;

            var APEXSteeringCommitteeList = (from product in exceptionRepository.BindExcpLevel3().APEXSteeringCommitteeList
                                             select new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                                             { Text = product.Val, Value = product.ID }).ToList();
            ViewBag.APEXSteeringCommitteeList = APEXSteeringCommitteeList;

            return View(exceptionLevels);
            //return RedirectToPage("/ExceptionLevels");
        }
        [HttpPost]
        public JsonResult GetExceptionLevel(string ImpactOnBank)
        {
            AddEditExceptionModel getexceptionlevel = new AddEditExceptionModel();
            ExceptionRepository exceptionrepository = new ExceptionRepository();
            try
            {
                getexceptionlevel = exceptionrepository.GetExceptionLevel(ImpactOnBank);
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return new JsonResult(getexceptionlevel);
        }
        [HttpPost]
        public IActionResult SaveExceptionLevel1(ExceptionLevels exception)
        {
            try
            {
                var CreatedBy = HttpContext.Session.GetString("EmpId").ToString();
                exception.lstAddExceptionlevelDetails = new List<AddExceptionlevelDetails>();
                exception.lstAddExceptionlevelDetails.Add(new AddExceptionlevelDetails() { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.BusinessVerticalHead, Status = "Created", ApproverType = "Business", ApproverLevel = "VH", CreatedBy = CreatedBy, ExceptionLevel = "Level1" });
                exception.lstAddExceptionlevelDetails.Add(new AddExceptionlevelDetails() { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.BusinessGroupHead, Status = "Created", ApproverType = "Business", ApproverLevel = "GH", CreatedBy = CreatedBy, ExceptionLevel = "Level1" });
                exception.lstAddExceptionlevelDetails.Add(new AddExceptionlevelDetails() { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.CIOGroup, Status = "Created", ApproverType = "CIO", ApproverLevel = "VH", CreatedBy = CreatedBy, ExceptionLevel = "Level1" });

                exception.lstAddExceptionlevelDetails = exception.lstAddExceptionlevelDetails.Where(X => X.ApproverUserID != null && X.ApproverUserID != "").ToList();
                var objPartnerOnboarding = exceptionRepository.SaveExceptionLevel(exception);
                return RedirectToAction("ListOfExceptions");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        public IActionResult SaveExceptionLevel2(ExceptionLevels exception)
        {
            try
            {
                var CreatedBy = HttpContext.Session.GetString("EmpId").ToString();
                exception.lstAddExceptionlevelDetails = new List<AddExceptionlevelDetails>();
                exception.lstAddExceptionlevelDetails.Add(new AddExceptionlevelDetails() { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.BusinessVerticalHead, Status = "Created", ApproverType = "Business", ApproverLevel = "VH", CreatedBy = CreatedBy, ExceptionLevel = "Level2" });
                exception.lstAddExceptionlevelDetails.Add(new AddExceptionlevelDetails() { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.ITDRMVerticalHead, Status = "Created", ApproverType = "ITDRM", ApproverLevel = "VH", CreatedBy = CreatedBy, ExceptionLevel = "Level2" });
                exception.lstAddExceptionlevelDetails.Add(new AddExceptionlevelDetails() { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.BusinessGroupHead, Status = "Created", ApproverType = "Business", ApproverLevel = "GH", CreatedBy = CreatedBy, ExceptionLevel = "Level2" });
                exception.lstAddExceptionlevelDetails.Add(new AddExceptionlevelDetails() { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.CIOGroup, Status = "Created", ApproverType = "CIO", ApproverLevel = "VH", CreatedBy = CreatedBy, ExceptionLevel = "Level2" });
                exception.lstAddExceptionlevelDetails.Add(new AddExceptionlevelDetails() { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.ITDRMGroupHead, Status = "Created", ApproverType = "ITDRM", ApproverLevel = "GH", CreatedBy = CreatedBy, ExceptionLevel = "Level2" });
                exception.lstAddExceptionlevelDetails.Add(new AddExceptionlevelDetails() { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.CISOGroupHead, Status = "Created", ApproverType = "CISO", ApproverLevel = "GH", CreatedBy = CreatedBy, ExceptionLevel = "Level2" });

                exception.lstAddExceptionlevelDetails = exception.lstAddExceptionlevelDetails.Where(X => X.ApproverUserID != null && X.ApproverUserID != "").ToList();
                var objPartnerOnboarding = exceptionRepository.SaveExceptionLevel(exception);
                return RedirectToAction("ListOfExceptions");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        public IActionResult SaveExceptionLevel3(ExceptionLevels exception)
        {
            try
            {
                var CreatedBy = HttpContext.Session.GetString("EmpId").ToString();
                exception.lstAddExceptionlevelDetails = new List<AddExceptionlevelDetails>();
                exception.lstAddExceptionlevelDetails.Add(new AddExceptionlevelDetails() { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.ITVerticalHead, Status = "Created", ApproverType = "IT", ApproverLevel = "VH", CreatedBy = CreatedBy, ExceptionLevel = "Level3" });
                exception.lstAddExceptionlevelDetails.Add(new AddExceptionlevelDetails() { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.BSGVerticalHead, Status = "Created", ApproverType = "BSG", ApproverLevel = "VH", CreatedBy = CreatedBy, ExceptionLevel = "Level3" });
                exception.lstAddExceptionlevelDetails.Add(new AddExceptionlevelDetails() { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.BusinessVerticalHead, Status = "Created", ApproverType = "Business", ApproverLevel = "VH", CreatedBy = CreatedBy, ExceptionLevel = "Level3" });
                exception.lstAddExceptionlevelDetails.Add(new AddExceptionlevelDetails() { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.ComplianceVerticalHead, Status = "Created", ApproverType = "Compliance", ApproverLevel = "VH", CreatedBy = CreatedBy, ExceptionLevel = "Level3" });
                exception.lstAddExceptionlevelDetails.Add(new AddExceptionlevelDetails() { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.ITDRMVerticalHead, Status = "Created", ApproverType = "ITDRM", ApproverLevel = "VH", CreatedBy = CreatedBy, ExceptionLevel = "Level3" });
                exception.lstAddExceptionlevelDetails.Add(new AddExceptionlevelDetails() { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.ISGVerticalHead, Status = "Created", ApproverType = "ISG", ApproverLevel = "VH", CreatedBy = CreatedBy, ExceptionLevel = "Level3" });
                exception.lstAddExceptionlevelDetails.Add(new AddExceptionlevelDetails() { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.ApexSteeringCommittee, Status = "Created", ApproverType = "ApexSteeringCommittee", ApproverLevel = "VH", CreatedBy = CreatedBy, ExceptionLevel = "Level3" });

                exception.lstAddExceptionlevelDetails = exception.lstAddExceptionlevelDetails.Where(X => X.ApproverUserID != null && X.ApproverUserID != "").ToList();
                var objPartnerOnboarding = exceptionRepository.SaveExceptionLevel(exception);
                return RedirectToAction("ListOfExceptions");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}