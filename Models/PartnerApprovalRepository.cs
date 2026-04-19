using System.Collections.Generic;

namespace API_HUNT.Models
{
    public class PartnerApprovalRepository : IPartnerApprovalRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PartnerApprovalRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public List<ApprovalTrailListItem> GetApprovalTrailList(string approverUserID) => new List<ApprovalTrailListItem>();

        public PartnerApprovalModel GetPartnerApprovalDetail(string id) => null!;

        public List<ApiDetailItem> GetPartnerApprovalAPIDetail(string id) => new List<ApiDetailItem>();

        public List<DepartmentStatus> GetDepartmentWiseStatus(string id) => new List<DepartmentStatus>();

        public List<POFeedbackHistory> GetFeedbackHistory(string id) => new List<POFeedbackHistory>();

        public List<ApprovalTrailListItem> GetApprovalTrailDetail(string id, string approverUserID) => new List<ApprovalTrailListItem>();

        public List<PartnerTypeItem> GetPartnerTypeList() => new List<PartnerTypeItem>();

        public FeedbackReplyData GetFeedbackReply(string id, string? approvalTrialID) => null!;

        public PartnerApprovalModel SaveAddPartnerApproval(PartnerApprovalModel partnerApproval) => null!;
    }
}
