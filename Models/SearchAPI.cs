using System.Collections.Generic;

namespace API_HUNT.Models
{
    public class ConsumerItem
    {
        public string? ConsumerName { get; set; }
        public string? ConsumerID { get; set; }
        public string? ConsumerId { get; set; }
        public bool IsCheck { get; set; }
        public int RecordCount { get; set; }
        public int InternalPCount { get; set; }
        public int ExternalPCount { get; set; }
    }

    public class Consumer
    {
        public string? ConsumerName { get; set; }
        public string? ConsumerId { get; set; }
        public bool IsCheck { get; set; }
        public int RecordCount { get; set; }
        public int InternalPCount { get; set; }
        public int ExternalPCount { get; set; }
    }

    public class MiddlewareItem
    {
        public string? MiddlewareName { get; set; }
        public string? MiddlewareID { get; set; }
        public string? middlewareId { get; set; }
        public bool IsCheck { get; set; }
        public int RecordCount { get; set; }
    }

    public class SearchAPIItem
    {
        public string? APIName { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Owner { get; set; }
        public string? APIMainId { get; set; }
        public string? ServiceDesc { get; set; }
        public string? ServiceType { get; set; }
        public string? ServiceUrl { get; set; }
    }

    public class ServiceProviderItem
    {
        public string? ProviderName { get; set; }
        public string? ProviderID { get; set; }
        public string? ServiceProvideName { get; set; }
        public string? ServiceProviderId { get; set; }
        public bool IsCheck { get; set; }
        public int RecordCount { get; set; }
    }

    public class ProducerItem
    {
        public string? ProducerName { get; set; }
        public string? ProducerID { get; set; }
        public string? ProducerId { get; set; }
        public bool IsCheck { get; set; }
        public int RecordCount { get; set; }
        public int InternalPCount { get; set; }
        public int ExternalPCount { get; set; }
    }

    public class Producer
    {
        public string? ProducerName { get; set; }
        public string? ProducerId { get; set; }
        public bool IsCheck { get; set; }
        public int RecordCount { get; set; }
        public int InternalPCount { get; set; }
        public int ExternalPCount { get; set; }
    }

    public class SearchAPI
    {
        public string? SearchTerm { get; set; }
        public string? ServiceUrl { get; set; }
        public string? SearchAPIURL { get; set; }
        public string? APIMainId { get; set; }
        public string? SpName { get; set; }
        public string? IdentFlag { get; set; }
        public string? MethodFlag { get; set; }
        public string? Name { get; set; }
        public string? Raw { get; set; }
        public string? response { get; set; }
        public string? RequestSample { get; set; }
        public string? ResponseSample { get; set; }
        public string? ServiceDesc { get; set; }
        public string? ServiceType { get; set; }
        public string? apiCategory { get; set; }
        public string? strProvider { get; set; }
        public string? strMiddleware { get; set; }
        public string? UserJourneyFiles { get; set; }
        public string? ServiceDocumentFile { get; set; }

        public List<SearchAPIItem>? lstSearchResults { get; set; }
        public List<SearchAPIItem>? searchAPILists { get; set; }
        public List<Consumer>? lstConsumer { get; set; }
        public List<MiddlewareItem>? lstMiddlewares { get; set; }
        public List<Producer>? lstProducer { get; set; }
        public List<ServiceProviderItem>? lstServiceProviders { get; set; }
        public List<ConsumerItem>? lstInternalConsumer { get; set; }
        public List<ConsumerItem>? lstExternalConsumer { get; set; }
        public List<ProducerItem>? lstInternalProducer { get; set; }
        public List<ProducerItem>? lstExternalProducer { get; set; }
    }
}
