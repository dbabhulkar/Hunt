using System.Collections.Generic;

namespace API_HUNT.Models
{
    public class ApprovalListModel
    {
        public List<ApprovalTrailListItem>? lstApprovalTrailList { get; set; }
        public List<PartnerApprovalModel>? ApprovalList { get; set; }
    }

    public class ApprovalListNewModel
    {
        public List<ApprovalTrailListNewItem>? lstApprovalTrailList { get; set; }
        public List<PartnerApprovalNewModel>? ApprovalList { get; set; }
    }
}
