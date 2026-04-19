using System.Collections.Generic;

namespace API_HUNT.Models
{
    public class TestAPIItem
    {
        public string? ServiceURL { get; set; }
        public string? APIName { get; set; }
        public string? Method { get; set; }
    }

    public class TestAPI
    {
        public string? APIName { get; set; }
        public string? Endpoint { get; set; }
        public string? Method { get; set; }
        public string? RequestBody { get; set; }
        public string? ResponseBody { get; set; }
        public string? Status { get; set; }
        public List<TestAPIItem>? TestAPIAutoList { get; set; }
    }
}
