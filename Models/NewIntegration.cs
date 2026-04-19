using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace API_HUNT.Models
{
    public class IntegrationList
    {
        public int integrationId { get; set; }
        public int workflowstatus { get; set; }
        public string? AssignTo { get; set; }
        public string? AssignFrom { get; set; }
        public string? Status { get; set; }
        public string? UserId { get; set; }
        public string? HUNTID { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectId { get; set; }
        public string? ProjectManagerBTG { get; set; }
        public string? ProjectManagerIT { get; set; }
        public string? Platform { get; set; }
        public string? JIRAId { get; set; }
        public string? ConsumerApplicationId { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class ServiceDetails
    {
        public int ServiceID { get; set; }
        public string? InternalServiceName { get; set; }
        public string? ExternalServiceName { get; set; }
        public string? ExternalServiceNameList { get; set; }
        public string? Purpose { get; set; }
        public string? ProducerApplication { get; set; }
        public string? Transformation { get; set; }
        public string? Volume { get; set; }
        public int Existing_New_Id { get; set; }
        public int Rest_SOAP_Id { get; set; }
        public int ServiceType_Id { get; set; }
        public int APIType_Id { get; set; }
        public int APICategory_Id { get; set; }
        public string? RistClassify { get; set; }
        public string? TotalScore { get; set; }
        public int DomainName_Id { get; set; }
        public string? ConsumerDC { get; set; }
        public string? ProducerDC { get; set; }
        public string? Platform { get; set; }
        public string? SfileName { get; set; }
        public string? modifyExpectedAPISpecificationFileName { get; set; }
        public string? AddServiceStatus { get; set; }
        public IFormFile? ExpectedAPISpecificationFiles { get; set; }
        public int MiddlewareNameId { get; set; }
        public int APIRiskScore_Id { get; set; }
        public int PartnerRiskScore_Id { get; set; }
        public bool IsAPIGW { get; set; }
        public string? ExternalServiceName_text { get; set; }
    }

    public class Questinonselected
    {
        public int? QID { get; set; }
        public int? QuesOptionsID { get; set; }
        public string? Weightage { get; set; }
        public string? Val { get; set; }
        public int QusServiceID { get; set; }
    }

    public class QuestionServiceDetails
    {
        public int? QID { get; set; }
        public int? QuesOptionsID { get; set; }
        public string? Weightage { get; set; }
        public string? Val { get; set; }
        public int QusServiceID { get; set; }
    }

    public class QuestionItem
    {
        public int? ID { get; set; }
        public string? Question { get; set; }
        public string? QType { get; set; }
        public int? APICategoryId { get; set; }
    }

    public class QDropItem
    {
        public int? ID { get; set; }
        public int? QID { get; set; }
        public string? Weightage { get; set; }
        public string? Val { get; set; }
    }

    public class ProducerAppItem
    {
        public string? ProducerName { get; set; }
        public string? ITGRCCodeProducer { get; set; }
    }

    public class ConsumerAppItem
    {
        public string? ConsumerName { get; set; }
        public string? ITGRCCodeConsumer { get; set; }
    }

    public class ExistingServiceItem
    {
        public string? ExServiceName { get; set; }
        public string? ExCodServiceId { get; set; }
    }

    public class ExternalServiceItem
    {
        public string? ExternalServicName { get; set; }
        public string? ExternalCodServiceId { get; set; }
    }

    public class BTGProjectMgrItem
    {
        public string? ProjectMgrBTG { get; set; }
    }

    public class UserItem
    {
        public string? UserName { get; set; }
        public string? UserID { get; set; }
    }

    public class FTPCredential
    {
        public string? HostName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }

    public class NewIntegration
    {
        public int IntegrationId { get; set; }
        public int? ParentIntegrationId { get; set; }
        public string? IN_Platform { get; set; }
        public string? AssignFrom { get; set; }
        public string? AssignTo { get; set; }
        public string? UserId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime GoLiveDate { get; set; }
        public int workflowstatus { get; set; }
        public string? fileName { get; set; }
        public string? modifyFileName { get; set; }
        public string? RfileName { get; set; }
        public string? SqfileName { get; set; }
        public string? modifyRDConceptNoteFileName { get; set; }
        public string? modifySequenceDiagFileName { get; set; }
        public string? UserJourneyFiles { get; set; }
        public string? RDConceptNoteFiles { get; set; }
        public string? RDConceptNoteDoc { get; set; }
        public string? SequenceDiagFiles { get; set; }
        public string? SpName { get; set; }
        public string? MethodFlag { get; set; }
        public string? ConsumerApplication { get; set; }
        public string? ConsumerApplicationId { get; set; }
        public string? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectMgrBTG { get; set; }
        public string? ProjectMgrIT { get; set; }
        public string? BusinessSponsor { get; set; }
        public string? ExecutiveSponsor { get; set; }
        public string? CostCenterCode { get; set; }
        public string? BusinesssJustification { get; set; }
        public string? BTGUSER { get; set; }
        public string? ITUSER { get; set; }
        public string? ITARCHITECH { get; set; }
        public string? Role { get; set; }
        public string? FullName { get; set; }
        public string? APPShortName { get; set; }
        public string? ITGRCCode { get; set; }
        public string? ITGRCName { get; set; }
        public string? Department { get; set; }
        public string? HostingDC { get; set; }
        public string? ChannelId { get; set; }
        public string? ContainerName { get; set; }
        public List<SelectItem>? MiddlewareName { get; set; }
        public string? status { get; set; }
        public string? SpocLevel { get; set; }
        public string? Feedback { get; set; }
        public int FeedbackId { get; set; }
        public int FeedbackCount { get; set; }
        public string? Purpose { get; set; }

        public IFormFile? UserJourneyDocument { get; set; }
        public IFormFile? RDConceptNoteDocument { get; set; }
        public IFormFile? SequenceDiagDocument { get; set; }

        // Dropdown lists
        public List<SelectItem>? ExistingNewList { get; set; }
        public List<SelectItem>? RestSoapList { get; set; }
        public List<SelectItem>? ServiceTypeList { get; set; }
        public List<SelectItem>? APITypeList { get; set; }
        public List<SelectItem>? APICategoryList { get; set; }
        public List<SelectItem>? DomainNameList { get; set; }
        public List<SelectItem>? ConsumerDCList { get; set; }
        public List<SelectItem>? AllddDeparment { get; set; }
        public List<SelectItem>? AllddStatus { get; set; }
        public List<SelectItem>? AllddHostingDC { get; set; }
        public List<SelectItem>? SpocLevels { get; set; }

        // User lists
        public List<UserItem>? ITUserNameFillList { get; set; }
        public List<UserItem>? BTGUserNameFillList { get; set; }
        public List<BTGProjectMgrItem>? lstBTGProjectmngr { get; set; }

        // App lists
        public List<ProducerAppItem>? ProducerAppList { get; set; }
        public List<ConsumerAppItem>? ConsumerAppList { get; set; }
        public List<ExistingServiceItem>? ExistingServiceNameList { get; set; }
        public List<ExternalServiceItem>? ExternalServiceNameList { get; set; }

        // Question lists
        public List<QuestionItem>? QuestionList { get; set; }
        public List<QDropItem>? QDropValuesList { get; set; }
        public List<Questinonselected>? QuestinonselectedList { get; set; }
        public List<QuestionServiceDetails>? QuestionServiceDetailsList { get; set; }

        // Feedback
        public List<POFeedbackHistory>? FeedbackHistoryList { get; set; }

        // Integration lists
        public List<IntegrationList>? integrationLists { get; set; }
        public List<ServiceDetails>? serviceDetails { get; set; }
        public List<ServiceDetails>? serviceDetailsOld { get; set; }
        public ServiceDetails? serviceDetails1 { get; set; }
        public List<ServiceDetails>? lstServiceDetails { get; set; }

        /// <summary>Creates a NewIntegration with all list properties initialized to empty lists.</summary>
        public static NewIntegration CreateEmpty()
        {
            return new NewIntegration
            {
                MiddlewareName = new List<SelectItem>(),
                ExistingNewList = new List<SelectItem>(),
                RestSoapList = new List<SelectItem>(),
                ServiceTypeList = new List<SelectItem>(),
                APITypeList = new List<SelectItem>(),
                APICategoryList = new List<SelectItem>(),
                DomainNameList = new List<SelectItem>(),
                ConsumerDCList = new List<SelectItem>(),
                AllddDeparment = new List<SelectItem>(),
                AllddStatus = new List<SelectItem>(),
                AllddHostingDC = new List<SelectItem>(),
                SpocLevels = new List<SelectItem>(),
                ITUserNameFillList = new List<UserItem>(),
                BTGUserNameFillList = new List<UserItem>(),
                lstBTGProjectmngr = new List<BTGProjectMgrItem>(),
                ProducerAppList = new List<ProducerAppItem>(),
                ConsumerAppList = new List<ConsumerAppItem>(),
                ExistingServiceNameList = new List<ExistingServiceItem>(),
                ExternalServiceNameList = new List<ExternalServiceItem>(),
                QuestionList = new List<QuestionItem>(),
                QDropValuesList = new List<QDropItem>(),
                QuestinonselectedList = new List<Questinonselected>(),
                QuestionServiceDetailsList = new List<QuestionServiceDetails>(),
                FeedbackHistoryList = new List<POFeedbackHistory>(),
                integrationLists = new List<IntegrationList>(),
                serviceDetails = new List<ServiceDetails>(),
                serviceDetailsOld = new List<ServiceDetails>(),
                lstServiceDetails = new List<ServiceDetails>(),
            };
        }
    }
}
