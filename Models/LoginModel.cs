namespace API_HUNT.Models
{
    public class LoginModel
    {
        public string? UserId { get; set; }
        public string? Password { get; set; }
        public string? UserName { get; set; }
        public string? BranchCode { get; set; }
        public string? GlobalOrgIdRole { get; set; }
        public int RefID { get; set; }
        public string? Role { get; set; }
    }
}
