using System.Collections.Generic;

namespace API_HUNT.Models
{
    public interface IPartnerApprovalRepository
    {
        List<ApprovalTrailListItem> GetApprovalTrailList(string approverUserID);
        PartnerApprovalModel GetPartnerApprovalDetail(string id);
        List<ApiDetailItem> GetPartnerApprovalAPIDetail(string id);
        List<DepartmentStatus> GetDepartmentWiseStatus(string id);
        List<POFeedbackHistory> GetFeedbackHistory(string id);
        List<ApprovalTrailListItem> GetApprovalTrailDetail(string id, string approverUserID);
        List<PartnerTypeItem> GetPartnerTypeList();
        FeedbackReplyData GetFeedbackReply(string id, string? approvalTrialID);
        PartnerApprovalModel SaveAddPartnerApproval(PartnerApprovalModel partnerApproval);
    }
}
