using System.Collections.Generic;

namespace API_HUNT.Models
{
    public class SelectItem
    {
        public int? ID { get; set; }
        public string? Val { get; set; }
    }

    public class AddExceptionlevelDetails
    {
        public string? ExceptionCaseID { get; set; }
        public string? ApproverUserID { get; set; }
        public string? Status { get; set; }
        public string? ApproverType { get; set; }
        public string? ApproverLevel { get; set; }
        public string? CreatedBy { get; set; }
        public string? ExceptionLevel { get; set; }
    }

    public class ExceptionLevels
    {
        public string? CaseID { get; set; }

        // Level 1 approvers
        public string? BusinessVerticalHead { get; set; }
        public string? BusinessGroupHead { get; set; }
        public string? CIOGroup { get; set; }

        // Level 2 additional approvers
        public string? ITDRMVerticalHead { get; set; }
        public string? ITDRMGroupHead { get; set; }
        public string? CISOGroupHead { get; set; }

        // Level 3 additional approvers
        public string? ITVerticalHead { get; set; }
        public string? BSGVerticalHead { get; set; }
        public string? ComplianceVerticalHead { get; set; }
        public string? ISGVerticalHead { get; set; }
        public string? ApexSteeringCommittee { get; set; }

        public List<AddExceptionlevelDetails>? lstAddExceptionlevelDetails { get; set; }
    }

    public class ExceptionBindModel
    {
        public List<SelectItem> BusinessVerticalHeadList { get; set; } = new List<SelectItem>();
        public List<SelectItem> BusinessGroupHeadList { get; set; } = new List<SelectItem>();
        public List<SelectItem> CIOGroupList { get; set; } = new List<SelectItem>();
        public List<SelectItem> ITDRMVerticalHeadList { get; set; } = new List<SelectItem>();
        public List<SelectItem> ITDRMGroupHeadList { get; set; } = new List<SelectItem>();
        public List<SelectItem> CISOGroupList { get; set; } = new List<SelectItem>();
        public List<SelectItem> ITVerticalHeadList { get; set; } = new List<SelectItem>();
        public List<SelectItem> BSGVerticalHeadList { get; set; } = new List<SelectItem>();
        public List<SelectItem> ComplianceVerticalHeadList { get; set; } = new List<SelectItem>();
        public List<SelectItem> ISGVerticalHeadList { get; set; } = new List<SelectItem>();
        public List<SelectItem> APEXSteeringCommitteeList { get; set; } = new List<SelectItem>();
    }
}
