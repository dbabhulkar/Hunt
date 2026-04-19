using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace API_HUNT.Models
{
    public class ExceptionDetails
    {
        public string? CASEID { get; set; }
        public string? PartnerName { get; set; }
        public string? Status { get; set; }
        public string? CreatedBy { get; set; }
        public string? DateCreated { get; set; }
        public string? DateUpdated { get; set; }
    }

    public class AddEditExceptionModel
    {
        public string? CaseId { get; set; }
        public string? OriginalOnboardingGASID { get; set; }
        public string? ExceptionRequestor { get; set; }
        public string? APIProjectName { get; set; }
        public string? APIProjectDescription { get; set; }
        public string? PartnersToBeIntegrated { get; set; }
        public string? ProductAPIToBeConsumed { get; set; }
        public string? RequestedException { get; set; }
        public string? ReasonForException { get; set; }
        public string? HowExceptionToBeImplemented { get; set; }
        public string? DurationofApproval { get; set; }
        public string? ImpactOnBank { get; set; }
        public string? ExceptionLevel { get; set; }
        public string? RelatedDocumentstoUpload { get; set; }
        public string? currentUser { get; set; }
        public List<ExceptionDetails>? lstExceptionDetails { get; set; }
    }
}
