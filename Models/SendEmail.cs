namespace API_HUNT.Models
{
    public class SendEmail
    {
        public string? To { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? From { get; set; }
        public string? CC { get; set; }

        public bool Send() => false;

        public bool SendMail(string to, string subject, string body, string from = "") => false;

        public void SendMailAlert(NewIntegration newIntegration, string logUserId, string status, int integrationId) { }

        public void SendMailAlert(NewIntegration newIntegration, string logUserId, string status, int integrationId, string consumerApplicationId) { }
    }
}
