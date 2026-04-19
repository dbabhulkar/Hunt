using System;

namespace API_HUNT.Models
{
    public class LoginUserRecord
    {
        public string? EmpName { get; set; }
        public string? UserRole { get; set; }
        public string? BranchCode { get; set; }
        public string? ProfileDescription { get; set; }
        public int? ID { get; set; }
        public bool Active { get; set; }
        public DateTime? LastLogoutDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string? Status { get; set; }
    }
}
