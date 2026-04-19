using System.Collections.Generic;
using System.Data;

namespace API_HUNT.Models
{
    public interface IPartnerRepository
    {
        List<PartnerDetail> GetPartnerList();
        PartnerOnboarding partnertype();
        PartnerOnboarding PartnerCaseApprovalMetrix(string partnerRiskClassification, string apiRiskClassification, string approverType, string approverLevel);
        List<PartnerTypeItem> GetPartnerTypeList();
        ApiDetail GetApiDetail(string apiName);
        bool AddEditPartner(PartnerOnboarding partnerOnboarding);
        PartnerOnboarding GetPartnerApprovalDetail(string id);
        List<POFeedbackHistory> GetFeedbackDetail(string id);
        List<POFeedbackHistory> GetFeedbackHistory(string id);
        List<ApiDetailItem> GetPartnerApprovalAPIDetail(string id);
        List<AddPartnerCaseApprovalMetrixList> GetApprovalTrailDetail(string id);
        CaseIdData GetNewCaseId();
        PartnerOnboarding AddPartner(PartnerOnboarding partnerOnboarding);
        PartnerOnboarding EditPartner(PartnerOnboarding partnerOnboarding);
        bool DeletePartner(string id);
        PartnerOnboarding GetMstPartnerType(string partnerType, string partnerSubType, string partnerEntityType, string identFlag);
        void GetPartnerSendMailDetail(string caseId);
        PartnerOnboarding EditPartnerDraft(PartnerOnboarding data);
        PartnerOnboarding GetlstApiDetail();
        POFeedbackHistory SaveFeedbackReply(POFeedbackHistory feedback);
        PartnerIntegrationNew GetIntegrationDetail(string id);
        PartnerIntegrationNew EditPartnerIntegration(PartnerIntegrationNew data);
        PartnerIntegrationNew AddPartnerIntegration(PartnerIntegrationNew data);
        CaseIdNewData GetNewCaseID();
    }
}
