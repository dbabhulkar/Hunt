using System.Collections.Generic;

namespace API_HUNT.Models
{
    public class PartnerIntegrationNew
    {
        public string? CaseId { get; set; }
        public string? CaseID { get; set; }
        public string? PartnerID { get; set; }
        public string? PartnerName { get; set; }
        public string? IntegrationType { get; set; }
        public string? Status { get; set; }
        public string? status { get; set; }
        public string? CreatedBy { get; set; }
        public string? Action { get; set; }
        public List<PartnerIntegrationNew>? IntegrationList { get; set; }

        // Approver fields
        public string? HOPP_FH { get; set; }
        public string? HOPP_VH { get; set; }
        public string? HOPP_GH { get; set; }
        public string? HOB_FH { get; set; }
        public string? HOB_VH { get; set; }
        public string? HOB_GH { get; set; }
        public string? HODB_FH { get; set; }
        public string? HODB_VH { get; set; }
        public string? HODB_GH { get; set; }
        public string? HOISG_FH { get; set; }
        public string? HOISG_VH { get; set; }
        public string? HOISG_GH { get; set; }
        public string? HOITDRM_FH { get; set; }
        public string? HOITDRM_VH { get; set; }
        public string? HOITDRM_GH { get; set; }

        public string? AttachedJourneyDocuments { get; set; }
        public string? OtherDocument { get; set; }

        // Additional fields
        public string? PartnerType { get; set; }
        public string? PartnerSubType { get; set; }
        public string? PartnerEntityType { get; set; }
        public string? PartnerRisk { get; set; }
        public string? PartnerRiskScore { get; set; }
        public string? PartnerTPRMAssessmentApplicability { get; set; }
        public string? PartnerDescription { get; set; }
        public string? APIRisk { get; set; }
        public string? APIRiskScore { get; set; }
        public string? APIName { get; set; }
        public string? APIId { get; set; }
        public string? Remark { get; set; }
        public string? CostCentre { get; set; }
        public string? statusDesc { get; set; }
        public string? ViewCaseId { get; set; }
        public string? Feedback { get; set; }
        public string? ClientProfileSheet { get; set; }
        public string? PartnerRiskAssessmentSheet { get; set; }
        public DateTime PartnerLastModifiedDate { get; set; }

        // Approver status fields
        public string? HOPP_FH_Status { get; set; }
        public string? HOPP_VH_Status { get; set; }
        public string? HOPP_GH_Status { get; set; }
        public string? HOB_FH_Status { get; set; }
        public string? HOB_VH_Status { get; set; }
        public string? HOB_GH_Status { get; set; }
        public string? HODB_FH_Status { get; set; }
        public string? HODB_VH_Status { get; set; }
        public string? HODB_GH_Status { get; set; }
        public string? HOISG_FH_Status { get; set; }
        public string? HOISG_VH_Status { get; set; }
        public string? HOISG_GH_Status { get; set; }
        public string? HOITDRM_FH_Status { get; set; }
        public string? HOITDRM_VH_Status { get; set; }
        public string? HOITDRM_GH_Status { get; set; }

        // Approver name fields
        public string? HOPP_FH_Name { get; set; }
        public string? HOPP_VH_Name { get; set; }
        public string? HOPP_GH_Name { get; set; }
        public string? HOB_FH_Name { get; set; }
        public string? HOB_VH_Name { get; set; }
        public string? HOB_GH_Name { get; set; }
        public string? HODB_FH_Name { get; set; }
        public string? HODB_VH_Name { get; set; }
        public string? HODB_GH_Name { get; set; }
        public string? HOISG_FH_Name { get; set; }
        public string? HOISG_VH_Name { get; set; }
        public string? HOISG_GH_Name { get; set; }
        public string? HOITDRM_FH_Name { get; set; }
        public string? HOITDRM_VH_Name { get; set; }
        public string? HOITDRM_GH_Name { get; set; }

        // Current approver
        public string? CurrentApproverLevel { get; set; }
        public string? CurrentApproverUserID { get; set; }
        public string? CurrentSequence { get; set; }
        public string? CurrentDepartment { get; set; }
        public string? CurrentApprovalTrialID { get; set; }

        public List<AddPartnerCaseApprovalMetrixListNew>? PartnerApproval { get; set; }
        public List<PartnerDetailNew>? lstPartnerIntegrationDetail { get; set; }
        public List<ApiDetailNewItem>? lstApiDetail { get; set; }
        public List<PartnerCaseApprovalMetrixItem>? lstPartnerCaseApprovalMetrix { get; set; }
        public List<POFeedbackHistory>? lstPOFeedbackHistory { get; set; }
        public List<ApprovalTrailListNewItem>? lstApprovalTrailDetail { get; set; }
    }
}
