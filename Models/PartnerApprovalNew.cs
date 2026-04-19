using System.Collections.Generic;

namespace API_HUNT.Models
{
    public class PartnerApprovalNew
    {
        public string? PartnerID { get; set; }
        public string? CreatedBy { get; set; }
        public string? Action { get; set; }
        public string? status { get; set; }
        public string? CaseId { get; set; }
        public string? CaseID { get; set; }
        public string? PartnerName { get; set; }
        public string? Remarks { get; set; }
        public string? FeedbackReply { get; set; }

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

        // Partner info fields
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
        public string? ViewCaseId { get; set; }
        public string? Feedback { get; set; }
        public string? CostCentre { get; set; }
        public string? ClientProfileSheet { get; set; }
        public string? PartnerRiskAssessmentSheet { get; set; }
        public string? AttachedJourneyDocuments { get; set; }
        public string? OtherDocument { get; set; }
        public System.DateTime PartnerLastModifiedDate { get; set; }

        // Current approver
        public string? CurrentApproverLevel { get; set; }
        public string? CurrentApproverUserID { get; set; }
        public string? CurrentSequence { get; set; }
        public string? CurrentDepartment { get; set; }
        public string? CurrentApprovalTrialID { get; set; }

        // Lists
        public List<ApprovalTrailListNewItem>? lstApprovalTrailDetail { get; set; }
        public List<ApiDetailNewItem>? lstApiDetail { get; set; }
        public List<POFeedbackHistory>? lstPartnerOnBoardFeedbackHistory { get; set; }
        public List<AddPartnerCaseApprovalMetrixListNew>? PartnerApproval { get; set; }
    }
}
