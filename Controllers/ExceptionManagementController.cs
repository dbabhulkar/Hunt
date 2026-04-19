using System;
using System.Collections.Generic;
using System.Linq;
using API_HUNT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_HUNT.Controllers
{
    public class ExceptionManagementController : Controller
    {
        private readonly IExceptionRepository _exceptionRepo;

        public ExceptionManagementController(IExceptionRepository exceptionRepo)
        {
            _exceptionRepo = exceptionRepo;
        }

        public IActionResult Index()
        {
            AddEditExceptionModel model = new AddEditExceptionModel();
            model.lstExceptionDetails = _exceptionRepo.ExceptionList();
            return View(model);
        }

        public IActionResult ListOfExceptions()
        {
            if (HttpContext.Session.GetString("Role") == "USER")
            {
                AddEditExceptionModel model = new AddEditExceptionModel();
                model.lstExceptionDetails = _exceptionRepo.ExceptionList();
                return View(model);
            }
            return RedirectToAction("Index", "ExceptionApproval");
        }

        [HttpPost]
        public IActionResult ListOfExceptions(string data) => View();

        public IActionResult AddEditException(string CaseId)
        {
            AddEditExceptionModel model = new AddEditExceptionModel();
            if (!string.IsNullOrEmpty(CaseId) && CaseId.Contains("APIGW"))
            {
                model = _exceptionRepo.GetExceptionDetail(CaseId);
                model.CaseId = CaseId;
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult SaveaddeditException(AddEditExceptionModel addeditexceptionModel)
        {
            string actionName = addeditexceptionModel.ExceptionLevel switch
            {
                "Level 1" => "ExceptionLevel1",
                "Level 2" => "ExceptionLevel2",
                "Level 3" => "ExceptionLevel3",
                _ => ""
            };
            addeditexceptionModel.currentUser = HttpContext.Session.GetString("EmpId");
            addeditexceptionModel = _exceptionRepo.SaveAddEditException(addeditexceptionModel);
            return RedirectToAction(actionName, addeditexceptionModel);
        }

        public IActionResult ExceptionLevel1(AddEditExceptionModel addeditexceptionModel)
        {
            ExceptionLevels exceptionLevels = new ExceptionLevels { CaseID = addeditexceptionModel.OriginalOnboardingGASID };
            var level1 = _exceptionRepo.BindExcpLevel1();

            ViewBag.BusinessVerticalHeadList = level1.BusinessVerticalHeadList.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = p.Val, Value = p.ID?.ToString() }).ToList();
            ViewBag.BusinessGroupHeadList = level1.BusinessGroupHeadList.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = p.Val, Value = p.ID?.ToString() }).ToList();
            ViewBag.CIOGroupList = level1.CIOGroupList.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = p.Val, Value = p.ID?.ToString() }).ToList();

            return View(exceptionLevels);
        }

        public IActionResult ExceptionLevel2(AddEditExceptionModel addeditexceptionModel)
        {
            ExceptionLevels exceptionLevels = new ExceptionLevels { CaseID = addeditexceptionModel.OriginalOnboardingGASID };
            var level2 = _exceptionRepo.BindExcpLevel2();

            ViewBag.BusinessVerticalHeadList = level2.BusinessVerticalHeadList.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = p.Val, Value = p.ID?.ToString() }).ToList();
            ViewBag.ITDRMVerticalHeadList = level2.ITDRMVerticalHeadList.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = p.Val, Value = p.ID?.ToString() }).ToList();
            ViewBag.BusinessGroupHeadList = level2.BusinessGroupHeadList.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = p.Val, Value = p.ID?.ToString() }).ToList();
            ViewBag.CIOGroupList = level2.CIOGroupList.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = p.Val, Value = p.ID?.ToString() }).ToList();
            ViewBag.ITDRMGroupHeadList = level2.ITDRMGroupHeadList.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = p.Val, Value = p.ID?.ToString() }).ToList();
            ViewBag.CISOGroupList = level2.CISOGroupList.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = p.Val, Value = p.ID?.ToString() }).ToList();

            return View(exceptionLevels);
        }

        public IActionResult ExceptionLevel3(AddEditExceptionModel addeditexceptionModel)
        {
            ExceptionLevels exceptionLevels = new ExceptionLevels { CaseID = addeditexceptionModel.OriginalOnboardingGASID };
            var level3 = _exceptionRepo.BindExcpLevel3();

            ViewBag.ITVerticalHeadList = level3.ITVerticalHeadList.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = p.Val, Value = p.ID?.ToString() }).ToList();
            ViewBag.BSGVerticalHeadList = level3.BSGVerticalHeadList.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = p.Val, Value = p.ID?.ToString() }).ToList();
            ViewBag.BusinessVerticalHeadList = level3.BusinessVerticalHeadList.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = p.Val, Value = p.ID?.ToString() }).ToList();
            ViewBag.ComplianceVerticalHeadList = level3.ComplianceVerticalHeadList.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = p.Val, Value = p.ID?.ToString() }).ToList();
            ViewBag.ITDRMVerticalHeadList = level3.ITDRMVerticalHeadList.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = p.Val, Value = p.ID?.ToString() }).ToList();
            ViewBag.ISGVerticalHeadList = level3.ISGVerticalHeadList.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = p.Val, Value = p.ID?.ToString() }).ToList();
            ViewBag.APEXSteeringCommitteeList = level3.APEXSteeringCommitteeList.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = p.Val, Value = p.ID?.ToString() }).ToList();

            return View(exceptionLevels);
        }

        [HttpPost]
        public JsonResult GetExceptionLevel(string ImpactOnBank)
        {
            AddEditExceptionModel result = new AddEditExceptionModel();
            try
            {
                result = _exceptionRepo.GetExceptionLevel(ImpactOnBank);
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return new JsonResult(result);
        }

        [HttpPost]
        public IActionResult SaveExceptionLevel1(ExceptionLevels exception)
        {
            string createdBy = HttpContext.Session.GetString("EmpId") ?? string.Empty;
            exception.lstAddExceptionlevelDetails = new List<AddExceptionlevelDetails>
            {
                new AddExceptionlevelDetails { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.BusinessVerticalHead, Status = "Created", ApproverType = "Business", ApproverLevel = "VH", CreatedBy = createdBy, ExceptionLevel = "Level1" },
                new AddExceptionlevelDetails { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.BusinessGroupHead,    Status = "Created", ApproverType = "Business", ApproverLevel = "GH", CreatedBy = createdBy, ExceptionLevel = "Level1" },
                new AddExceptionlevelDetails { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.CIOGroup,             Status = "Created", ApproverType = "CIO",      ApproverLevel = "VH", CreatedBy = createdBy, ExceptionLevel = "Level1" },
            };
            exception.lstAddExceptionlevelDetails = exception.lstAddExceptionlevelDetails.Where(x => !string.IsNullOrEmpty(x.ApproverUserID)).ToList();
            _exceptionRepo.SaveExceptionLevel(exception);
            return RedirectToAction("ListOfExceptions");
        }

        [HttpPost]
        public IActionResult SaveExceptionLevel2(ExceptionLevels exception)
        {
            string createdBy = HttpContext.Session.GetString("EmpId") ?? string.Empty;
            exception.lstAddExceptionlevelDetails = new List<AddExceptionlevelDetails>
            {
                new AddExceptionlevelDetails { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.BusinessVerticalHead, Status = "Created", ApproverType = "Business", ApproverLevel = "VH", CreatedBy = createdBy, ExceptionLevel = "Level2" },
                new AddExceptionlevelDetails { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.ITDRMVerticalHead,    Status = "Created", ApproverType = "ITDRM",    ApproverLevel = "VH", CreatedBy = createdBy, ExceptionLevel = "Level2" },
                new AddExceptionlevelDetails { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.BusinessGroupHead,    Status = "Created", ApproverType = "Business", ApproverLevel = "GH", CreatedBy = createdBy, ExceptionLevel = "Level2" },
                new AddExceptionlevelDetails { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.CIOGroup,             Status = "Created", ApproverType = "CIO",      ApproverLevel = "VH", CreatedBy = createdBy, ExceptionLevel = "Level2" },
                new AddExceptionlevelDetails { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.ITDRMGroupHead,       Status = "Created", ApproverType = "ITDRM",    ApproverLevel = "GH", CreatedBy = createdBy, ExceptionLevel = "Level2" },
                new AddExceptionlevelDetails { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.CISOGroupHead,        Status = "Created", ApproverType = "CISO",     ApproverLevel = "GH", CreatedBy = createdBy, ExceptionLevel = "Level2" },
            };
            exception.lstAddExceptionlevelDetails = exception.lstAddExceptionlevelDetails.Where(x => !string.IsNullOrEmpty(x.ApproverUserID)).ToList();
            _exceptionRepo.SaveExceptionLevel(exception);
            return RedirectToAction("ListOfExceptions");
        }

        [HttpPost]
        public IActionResult SaveExceptionLevel3(ExceptionLevels exception)
        {
            string createdBy = HttpContext.Session.GetString("EmpId") ?? string.Empty;
            exception.lstAddExceptionlevelDetails = new List<AddExceptionlevelDetails>
            {
                new AddExceptionlevelDetails { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.ITVerticalHead,         Status = "Created", ApproverType = "IT",                   ApproverLevel = "VH", CreatedBy = createdBy, ExceptionLevel = "Level3" },
                new AddExceptionlevelDetails { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.BSGVerticalHead,        Status = "Created", ApproverType = "BSG",                  ApproverLevel = "VH", CreatedBy = createdBy, ExceptionLevel = "Level3" },
                new AddExceptionlevelDetails { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.BusinessVerticalHead,   Status = "Created", ApproverType = "Business",             ApproverLevel = "VH", CreatedBy = createdBy, ExceptionLevel = "Level3" },
                new AddExceptionlevelDetails { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.ComplianceVerticalHead, Status = "Created", ApproverType = "Compliance",           ApproverLevel = "VH", CreatedBy = createdBy, ExceptionLevel = "Level3" },
                new AddExceptionlevelDetails { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.ITDRMVerticalHead,      Status = "Created", ApproverType = "ITDRM",                ApproverLevel = "VH", CreatedBy = createdBy, ExceptionLevel = "Level3" },
                new AddExceptionlevelDetails { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.ISGVerticalHead,        Status = "Created", ApproverType = "ISG",                  ApproverLevel = "VH", CreatedBy = createdBy, ExceptionLevel = "Level3" },
                new AddExceptionlevelDetails { ExceptionCaseID = exception.CaseID, ApproverUserID = exception.ApexSteeringCommittee,  Status = "Created", ApproverType = "ApexSteeringCommittee", ApproverLevel = "VH", CreatedBy = createdBy, ExceptionLevel = "Level3" },
            };
            exception.lstAddExceptionlevelDetails = exception.lstAddExceptionlevelDetails.Where(x => !string.IsNullOrEmpty(x.ApproverUserID)).ToList();
            _exceptionRepo.SaveExceptionLevel(exception);
            return RedirectToAction("ListOfExceptions");
        }
    }
}
