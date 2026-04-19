using System.Collections.Generic;

namespace API_HUNT.Models
{
    public class JsonModel
    {
        public string? RequestBody { get; set; }
        public string? ResponseBody { get; set; }
        public string? Endpoint { get; set; }
        public string? Method { get; set; }
        public string? Status { get; set; }
    }

    public class RequestUrl
    {
        public string? raw { get; set; }
    }

    public class RequestBody
    {
        public string? raw { get; set; }
    }

    public class RequestDetail
    {
        public RequestUrl? url { get; set; }
        public RequestBody? body { get; set; }
    }

    public class ApplicationItem
    {
        public string? name { get; set; }
        public RequestDetail? request { get; set; }
        public List<object>? response { get; set; }
    }

    public class Application
    {
        public string? info { get; set; }
        public List<ApplicationItem>? item { get; set; }
    }
}
