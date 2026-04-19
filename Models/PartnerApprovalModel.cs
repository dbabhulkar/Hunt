using System.Collections.Generic;

namespace API_HUNT.Models
{
    public class ApprovalTrailListItem
    {
        public string? PartnetOnboading_ID { get; set; }
        public string? Partner_Name { get; set; }
        public string? Project_Description { get; set; }
        public string? API_risk { get; set; }
        public string? Partnerrisk { get; set; }
        public string? StatusDescription { get; set; }
        public string? ApproverUserID { get; set; }
        public string? ApproverName { get; set; }
        public string? ApproverLevel { get; set; }
        public string? Department { get; set; }
        public string? Sequence { get; set; }
        public string? Status { get; set; }
        public string? ApprovalTrialID { get; set; }
    }

    public class DepartmentStatus
    {
        public string? Department { get; set; }
        public string? Status { get; set; }
        public string? ApprovalStatus { get; set; }
    }

    public class FeedbackReplyData
    {
        public string? FeedbackReply { get; set; }
    }

    public class PartnerApprovalModel
    {
        public string? CaseID { get; set; }
        public string? createdBy { get; set; }
        public string? status { get; set; }
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

        // Current approver
        public string? CurrentApproverLevel { get; set; }
        public string? CurrentApproverUserID { get; set; }
        public string? CurrentSequence { get; set; }
        public string? CurrentDepartment { get; set; }
        public string? CurrentApprovalTrialID { get; set; }

        // Partner info fields
        public string? PartnerName { get; set; }
        public string? PartnerType { get; set; }
        public string? PartnerSubType { get; set; }
        public string? PartnerEntityType { get; set; }
        public string? PartnerRisk { get; set; }
        public string? PartnerRiskScore { get; set; }
        public string? PartnerTPRMAssesmetApplicability { get; set; }
        public string? APIRisk { get; set; }
        public string? projectDescription { get; set; }
        public System.DateTime TentativeGoLiveDate { get; set; }
        public string? Feedback { get; set; }
        public string? AttachedClientProfileSheet { get; set; }
        public string? APIRiskAssessmentSheet { get; set; }
        public string? AttachedJourneyDocuments { get; set; }
        public string? OtherDocument { get; set; }

        // Lists
        public List<ApprovalTrailListItem>? lstApprovalTrailDetail { get; set; }
        public List<ApiDetailItem>? lstAPIDetail { get; set; }
        public List<DepartmentStatus>? lstDepartmentStatus { get; set; }
        public List<POFeedbackHistory>? lstPartnerOnBoardFeedbackHistory { get; set; }
    }
}
