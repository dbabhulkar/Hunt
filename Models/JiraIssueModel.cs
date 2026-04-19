namespace API_HUNT.Models
{
    public class JiraIssueModel
    {
        public string? ProjectKey { get; set; }
        public string? Summary { get; set; }
        public string? Description { get; set; }
        public string? IssueType { get; set; }
        public string? Priority { get; set; }
        public string? Assignee { get; set; }
        public string? Reporter { get; set; }
    }
}
