using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace API_HUNT.Models
{
    public class PartnerDetailNew
    {
        public string? PartnerID { get; set; }
        public string? PartnerName { get; set; }
        public string? status { get; set; }
        public string? CaseID { get; set; }
        public string? PartnerType { get; set; }
        public string? PartnerSubType { get; set; }
        public string? PartnerEntityType { get; set; }
        public string? PartnerTPRMAssessmentApplicability { get; set; }
        public string? Ageing { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedDate { get; set; }
        public string? LastmodifiedDate { get; set; }
        public string? ViewCaseId { get; set; }
    }

    public class ApiDetailNew
    {
        public string? APIName { get; set; }
        public string? APIRisk { get; set; }
        public string? APIRiskScore { get; set; }
    }

    public class ApiDetailNewItem
    {
        public string? APIName { get; set; }
        public string? APIRisk { get; set; }
        public string? APIRiskScore { get; set; }
        public string? APIId { get; set; }
    }

    public class MstPartnerTypeItem
    {
        public string? PartnerType { get; set; }
        public string? PartnerSubType { get; set; }
        public string? PartnerEntityType { get; set; }
    }

    public class PartnerNameItem
    {
        public string? PartnerID { get; set; }
        public string? PartnerName { get; set; }
    }

    public class AddPartnerCaseApprovalMetrixListNew
    {
        public string? CaseId { get; set; }
        public string? Department { get; set; }
        public string? ApproverLevel { get; set; }
        public string? ApproverUserID { get; set; }
        public string? Status { get; set; }
        public string? Sequence { get; set; }
    }

    public class ApprovalTrailListNewItem
    {
        public string? CaseID { get; set; }
        public string? PartnerName { get; set; }
        public string? status { get; set; }
        public string? ApproverUserID { get; set; }
        public string? ApproverName { get; set; }
        public string? ApproverLevel { get; set; }
        public string? Department { get; set; }
        public string? Sequence { get; set; }
        public string? ApprovalTrialID { get; set; }
        public string? Ageing { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedDate { get; set; }
        public string? LastmodifiedDate { get; set; }
        public string? ViewCaseId { get; set; }
    }

    public class PartnerIDData
    {
        public string? PartnerID { get; set; }
    }

    public class PartnerOnboardingNew
    {
        public string? PartnerID { get; set; }
        public string? PartnerName { get; set; }
        public string? PartnerType { get; set; }
        public string? PartnerSubType { get; set; }
        public string? PartnerEntityType { get; set; }
        public DateTime PartnerLastModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ClientProfileSheet { get; set; }
        public string? PartnerRiskAssessmentSheet { get; set; }
        public string? Action { get; set; }
        public string? status { get; set; }

        public string? Remark { get; set; }
        public string? PartnerTPRMAssessmentApplicability { get; set; }
        public string? PartnerRiskScore { get; set; }
        public string? PartnerRisk { get; set; }
        public string? PartnerDescription { get; set; }
        public string? CostCentre { get; set; }

        public List<PartnerDetailNew>? lstPartnerDetail { get; set; }
        public List<MstPartnerTypeItem>? lstMstPartnerType { get; set; }
    }
}
