using System.Collections.Generic;
using API_HUNT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_HUNT.Controllers
{
    public class ExceptionApprovalController : Controller
    {
        private readonly IExceptionRepository _exceptionRepo;

        public ExceptionApprovalController(IExceptionRepository exceptionRepo)
        {
            _exceptionRepo = exceptionRepo;
        }

        public IActionResult Index()
        {
            string approverUserID = HttpContext.Session.GetString("EmpId") ?? string.Empty;
            List<ExceptionDetails> lstApprovalTrailDetail = _exceptionRepo.GetExceptionApproval(approverUserID);

            var model = new AddEditExceptionModel { lstExceptionDetails = lstApprovalTrailDetail };
            return View(model);
        }
    }
}
