using System.Collections.Generic;

namespace API_HUNT.Models
{
    public class JsonSoapModel
    {
        public string? RequestBody { get; set; }
        public string? ResponseBody { get; set; }
        public string? Endpoint { get; set; }
        public string? SoapAction { get; set; }
        public string? Status { get; set; }
    }

    public class SoapRequestDetail
    {
        public RequestUrl? url { get; set; }
        public RequestBody? body { get; set; }
    }

    public class SoapSubItem
    {
        public string? name { get; set; }
        public SoapRequestDetail? request { get; set; }
    }

    public class SoapApplicationItem
    {
        public string? name { get; set; }
        public List<SoapSubItem>? Item { get; set; }
    }

    public class SoapApplication
    {
        public string? info { get; set; }
        public List<SoapApplicationItem>? item { get; set; }
    }
}
