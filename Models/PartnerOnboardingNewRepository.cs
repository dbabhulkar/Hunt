using System.Collections.Generic;

namespace API_HUNT.Models
{
    public class PartnerOnboardingNewRepository : IPartnerOnboardingNewRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PartnerOnboardingNewRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public List<ApprovalTrailListNewItem> ListofApproval(string approverUserID) => new List<ApprovalTrailListNewItem>();

        public PartnerApprovalNew GetApprovalDetail(string id) => null!;

        public List<ApiDetailNewItem> GetPartnerApprovalAPIDetail(string id) => new List<ApiDetailNewItem>();

        public List<AddPartnerCaseApprovalMetrixListNew> GetApprovalTrailDetail(string id) => new List<AddPartnerCaseApprovalMetrixListNew>();

        public List<POFeedbackHistory> GetFeedbackHistory(string id) => new List<POFeedbackHistory>();

        public List<ApprovalTrailListNewItem> GetApprovalTrailDetail(string id, string approverUserID) => new List<ApprovalTrailListNewItem>();

        public FeedbackReplyData GetFeedbackReply(string id, string? approvalTrialID) => null!;

        public PartnerApprovalNew SaveAddPartnerApproval(PartnerApprovalNew partnerApproval) => null!;

        public List<PartnerDetailNew> GetPartnerList() => new List<PartnerDetailNew>();

        public List<PartnerTypeItem> GetPartnerTypeList() => new List<PartnerTypeItem>();

        public PartnerOnboardingNew GetMstPartnerType(string partnerType, string partnerSubType, string partnerEntityType, string identFlag) => null!;

        public PartnerIDData GetNewPartnerID() => null!;

        public PartnerOnboardingNew AddPartner(PartnerOnboardingNew partnerOnboarding) => null!;

        public PartnerOnboardingNew EditPartner(PartnerOnboardingNew partnerOnboarding) => null!;

        public PartnerApprovalNew GetPartnerDetail(string id) => null!;

        public List<PartnerDetailNew> ListofIntegration() => new List<PartnerDetailNew>();

        public List<PartnerNameItem> GetPartnerNameList() => new List<PartnerNameItem>();

        public PartnerApprovalNew GetPartnerDetailIntegration(string partnerID) => null!;

        public PartnerIntegrationNew PartnerCaseApprovalMetrix(string partnerRiskClassification, string apiRiskClassification, string approverType, string approverLevel) => null!;

        public PartnerIntegrationNew GetlstApiDetail() => null!;

        public ApiDetailNew GetApiDetail(string apiName) => null!;

        public CaseIdNewData GetNewCaseID() => null!;

        public PartnerIntegrationNew AddPartnerIntegration(PartnerIntegrationNew data) => null!;

        public PartnerIntegrationNew EditPartnerIntegration(PartnerIntegrationNew data) => null!;

        public List<POFeedbackHistory> GetFeedbackDetail(string id) => new List<POFeedbackHistory>();

        public PartnerIntegrationNew GetIntegrationDetail(string id) => null!;
    }

    public class CaseIdNewData
    {
        public string? CaseID { get; set; }
    }
}
