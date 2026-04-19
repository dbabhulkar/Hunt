using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace API_HUNT.Models
{
    public class PartnerNameItem2
    {
        public string? PARTNER_NAME { get; set; }
    }

    public class APINameItem
    {
        public string? APIName { get; set; }
    }

    public class PartnerOffboarding
    {
        public IFormFile? Uploadfile { get; set; }
        public IFormFile? OtherUploadfile { get; set; }
        public string? CaseId { get; set; }
        public string? PartnerName { get; set; }
        public string? Status { get; set; }
        public string? ExitScenario { get; set; }
        public string? APIName { get; set; }
        public string? Remark { get; set; }
        public List<SelectListItem>? DropdownList { get; set; }
        public List<PartnerNameItem2>? lstPARTNER_NAME { get; set; }
        public List<APINameItem>? lstAPIName { get; set; }
    }
}
