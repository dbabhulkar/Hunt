using System.Collections.Generic;

namespace API_HUNT.Models
{
    public interface IPartnerOnboardingNewRepository
    {
        List<ApprovalTrailListNewItem> ListofApproval(string approverUserID);
        PartnerApprovalNew GetApprovalDetail(string id);
        List<ApiDetailNewItem> GetPartnerApprovalAPIDetail(string id);
        List<AddPartnerCaseApprovalMetrixListNew> GetApprovalTrailDetail(string id);
        List<POFeedbackHistory> GetFeedbackHistory(string id);
        List<ApprovalTrailListNewItem> GetApprovalTrailDetail(string id, string approverUserID);
        FeedbackReplyData GetFeedbackReply(string id, string? approvalTrialID);
        PartnerApprovalNew SaveAddPartnerApproval(PartnerApprovalNew partnerApproval);
        List<PartnerDetailNew> GetPartnerList();
        List<PartnerTypeItem> GetPartnerTypeList();
        PartnerOnboardingNew GetMstPartnerType(string partnerType, string partnerSubType, string partnerEntityType, string identFlag);
        PartnerIDData GetNewPartnerID();
        PartnerOnboardingNew AddPartner(PartnerOnboardingNew partnerOnboarding);
        PartnerOnboardingNew EditPartner(PartnerOnboardingNew partnerOnboarding);
        PartnerApprovalNew GetPartnerDetail(string id);
        List<PartnerDetailNew> ListofIntegration();
        List<PartnerNameItem> GetPartnerNameList();
        PartnerApprovalNew GetPartnerDetailIntegration(string partnerID);
        PartnerIntegrationNew PartnerCaseApprovalMetrix(string partnerRiskClassification, string apiRiskClassification, string approverType, string approverLevel);
        PartnerIntegrationNew GetlstApiDetail();
        ApiDetailNew GetApiDetail(string apiName);
        CaseIdNewData GetNewCaseID();
        PartnerIntegrationNew AddPartnerIntegration(PartnerIntegrationNew data);
        PartnerIntegrationNew EditPartnerIntegration(PartnerIntegrationNew data);
        List<POFeedbackHistory> GetFeedbackDetail(string id);
        PartnerIntegrationNew GetIntegrationDetail(string id);
    }
}
