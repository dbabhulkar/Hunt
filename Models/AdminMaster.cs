using System.Collections.Generic;

namespace API_HUNT.Models
{
    public class AppMasterListItem
    {
        public string? Id { get; set; }
        public string? APPShortName { get; set; }
        public string? FullName { get; set; }
        public string? Purpose { get; set; }
        public string? HostingDC { get; set; }
        public string? ITGRCCode { get; set; }
        public string? ITGRCName { get; set; }
        public string? status { get; set; }
        public string? Department { get; set; }
        public string? SpocLevel { get; set; }
    }

    public class UserMasterListItem
    {
        public string? UserID { get; set; }
        public string? UserName { get; set; }
        public string? Status { get; set; }
        public string? DateCreated { get; set; }
        public string? UserRole { get; set; }
        public string? ApproverDept { get; set; }
        public string? Emailid { get; set; }
        public string? Userstatus { get; set; }
        public string? LastLoginDate { get; set; }
        public string? UpdatedDate { get; set; }
    }

    public class GetUserDetail
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }

    public class AdminMaster
    {
        // List properties
        public List<AppMasterListItem>? AppMasterLists { get; set; }
        public List<UserMasterListItem>? UserMasterLists { get; set; }
        public List<displayUsermaster>? lstUserMaster { get; set; }
        public List<displayAppmaster>? lstAppMaster { get; set; }

        // Form fields for App Master
        public string? APPShortName { get; set; }
        public string? FullName { get; set; }
        public string? Purpose { get; set; }
        public string? HostingDC { get; set; }
        public string? ITGRCCode { get; set; }
        public string? ITGRCName { get; set; }
        public string? status { get; set; }
        public string? Department { get; set; }
        public string? SpocLevel { get; set; }

        // Form fields for User Master
        public string? UserID { get; set; }
        public string? UserRole { get; set; }
        public string? ApproverDept { get; set; }
        public string? UserName { get; set; }
        public string? Emailid { get; set; }
        public string? Userstatus { get; set; }
        public string? LastLoginDate { get; set; }
        public string? CreatedDate { get; set; }
        public string? UpdatedDate { get; set; }

        // Dropdown lists
        public List<SelectItem>? AllddHostingDC { get; set; }
        public List<SelectItem>? AllddStatus { get; set; }
        public List<SelectItem>? AllddDeparment { get; set; }
        public List<SelectItem>? AllddUserRole { get; set; }
        public List<SelectItem>? AllddApproverDept { get; set; }
        public List<SelectItem>? SpocLevels { get; set; }
    }
}
