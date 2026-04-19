using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace API_HUNT.Models
{
    public class PartnerDetail
    {
        public string? CaseID { get; set; }
        public string? PartnerName { get; set; }
        public string? Partner_Name { get; set; }
        public string? statusDescription { get; set; }
        public string? ProjectDescription { get; set; }
        public string? APIRisk { get; set; }
        public string? PartnerRisk { get; set; }
        public string? Partner_Risk { get; set; }
        public string? PartnetOnboading_ID { get; set; }
        public string? Partner_Type { get; set; }
        public string? Partner_Entity_Type { get; set; }
        public string? PartnerTPRM_Application { get; set; }
        public string? Ageing { get; set; }
        public string? createdBy { get; set; }
        public string? createdDate { get; set; }
        public string? UpdatedDate { get; set; }
    }

    public class ApiDetail
    {
        public string? APIName { get; set; }
        public string? APIRisk { get; set; }
        public string? APIRiskScore { get; set; }
        public string? PartnerType { get; set; }
    }

    public class PartnerTypeItem
    {
        public string? PartnerType { get; set; }
    }

    public class PartnerCaseApprovalMetrixItem
    {
        public string? ApproverLevel { get; set; }
        public string? ApproverUserID { get; set; }
        public string? Department { get; set; }
    }

    public class AddPartnerCaseApprovalMetrixList
    {
        public string? CaseId { get; set; }
        public string? Department { get; set; }
        public string? ApproverLevel { get; set; }
        public string? ApproverUserID { get; set; }
        public string? status { get; set; }
        public string? Sequence { get; set; }
        public string? Status { get; set; }
    }

    public class ApprovalTrailDetail
    {
        public string? ApproverLevel { get; set; }
        public string? ApproverUserID { get; set; }
        public string? ApproverName { get; set; }
        public string? Department { get; set; }
        public string? Sequence { get; set; }
        public string? Status { get; set; }
        public string? ApprovalTrialID { get; set; }
    }

    public class ApiDetailItem
    {
        public string? APIName { get; set; }
        public string? APIRisk { get; set; }
        public string? APIRiskScore { get; set; }
        public string? APIId { get; set; }
    }

    public class CaseIdData
    {
        public string? CaseID { get; set; }
    }

    public class PartnerOnboarding
    {
        // Basic info
        public string? CaseID { get; set; }
        public string? PartnerName { get; set; }
        public string? projectDescription { get; set; }
        public DateTime TentativeGoLiveDate { get; set; }
        public string? PartnerType { get; set; }
        public string? PartnerEntityType { get; set; }
        public string? PartnerTPRMAssesmetApplicability { get; set; }
        public string? PartnerRiskScore { get; set; }
        public string? PartnerRisk { get; set; }
        public string? APIName { get; set; }
        public string? APIRisk { get; set; }
        public string? APIRiskScore { get; set; }
        public string? IdentFlag { get; set; }
        public string? SpName { get; set; }
        public string? Action { get; set; }
        public string? createdBy { get; set; }
        public string? statusDesc { get; set; }

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

        // Status fields
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

        // File attachments
        public string? AttachedClientProfileSheet { get; set; }
        public string? APIRiskAssessmentSheet { get; set; }
        public string? AttachedJourneyDocuments { get; set; }
        public string? OtherDocument { get; set; }
        public IFormFile? AttachedClientProfileSheetFile { get; set; }
        public IFormFile? APIRiskAssessmentFile { get; set; }

        // Additional fields
        public string? Remark { get; set; }
        public string? PartnerSubType { get; set; }
        public string? CostCentre { get; set; }
        public string? APIId { get; set; }

        // Lists
        public List<PartnerDetail>? lstPartnerDetail { get; set; }
        public List<MstPartnerTypeItem>? lstMstPartnerType { get; set; }
        public List<ApiDetailItem>? lstApiDetail { get; set; }
        public List<POFeedbackHistory>? lstPOFeedbackHistory { get; set; }
        public List<AddPartnerCaseApprovalMetrixList>? PartnerApproval { get; set; }
        public List<PartnerCaseApprovalMetrixItem>? lstPartnerCaseApprovalMetrix { get; set; }
        public List<ApprovalTrailDetail>? lstPartnerApprovalTrailDetail { get; set; }
        public List<PartnerTypeItem>? lstPartnerType { get; set; }
    }
}
