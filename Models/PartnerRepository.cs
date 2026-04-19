using System.Collections.Generic;
using System.Data;

namespace API_HUNT.Models
{
    public class PartnerRepository : IPartnerRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PartnerRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public List<PartnerDetail> GetPartnerList() => new List<PartnerDetail>();

        public PartnerOnboarding partnertype() => null!;

        public PartnerOnboarding PartnerCaseApprovalMetrix(string partnerRiskClassification, string apiRiskClassification, string approverType, string approverLevel) => null!;

        public List<PartnerTypeItem> GetPartnerTypeList() => new List<PartnerTypeItem>();

        public ApiDetail GetApiDetail(string apiName) => null!;

        public bool AddEditPartner(PartnerOnboarding partnerOnboarding) => false;

        public PartnerOnboarding GetPartnerApprovalDetail(string id) => null!;

        public List<POFeedbackHistory> GetFeedbackDetail(string id) => new List<POFeedbackHistory>();

        public List<POFeedbackHistory> GetFeedbackHistory(string id) => new List<POFeedbackHistory>();

        public List<ApiDetailItem> GetPartnerApprovalAPIDetail(string id) => new List<ApiDetailItem>();

        public List<AddPartnerCaseApprovalMetrixList> GetApprovalTrailDetail(string id) => new List<AddPartnerCaseApprovalMetrixList>();

        public CaseIdData GetNewCaseId() => null!;

        public PartnerOnboarding AddPartner(PartnerOnboarding partnerOnboarding) => null!;

        public PartnerOnboarding EditPartner(PartnerOnboarding partnerOnboarding) => null!;

        public bool DeletePartner(string id) => false;

        public PartnerOnboarding GetMstPartnerType(string partnerType, string partnerSubType, string partnerEntityType, string identFlag) => null!;

        public void GetPartnerSendMailDetail(string caseId) { }

        public PartnerOnboarding EditPartnerDraft(PartnerOnboarding data) => null!;

        public PartnerOnboarding GetlstApiDetail() => null!;

        public POFeedbackHistory SaveFeedbackReply(POFeedbackHistory feedback) => null!;

        public PartnerIntegrationNew GetIntegrationDetail(string id) => null!;

        public PartnerIntegrationNew EditPartnerIntegration(PartnerIntegrationNew data) => null!;

        public PartnerIntegrationNew AddPartnerIntegration(PartnerIntegrationNew data) => null!;

        public CaseIdNewData GetNewCaseID() => null!;
    }
}
