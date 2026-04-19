using System.Data;
using Microsoft.AspNetCore.Http;

namespace API_HUNT.Models
{
    public interface ISubmitRepository
    {
        DataTable DiagnoseUrl(string radioValue);
        SearchAPI searchAPIList(SearchAPI searchModel);
        NewIntegration GetNewIntegrationDetails(NewIntegration newIntegration);
        NewIntegration GetFeedbackDetails(NewIntegration newIntegration, string assignFrom);
        NewIntegration GetNewIntegrationById(int integrationId);
        NewIntegration GetNewIntegrationByIdEditPopwhendbcreate(int integrationId, NewIntegration model);
        NewIntegration GetAllDropDownListData(NewIntegration newIntegration);
        bool IsConsumerAppValidation(string? consumerApplication);
        NewIntegration SaveDetails(NewIntegration newIntegration);
        NewIntegration SaveServiceDetails(NewIntegration newIntegration);
        NewIntegration UpdateNewIntegration(NewIntegration newIntegration);
        NewIntegration DarftUpdateServiceDetails(NewIntegration newIntegration);
        NewIntegration DraftSaveDetails(NewIntegration newIntegration);
        NewIntegration DraftSaveServiceDetails(NewIntegration newIntegration);
        NewIntegration UpdateServiceDetails(NewIntegration newIntegration);
        DataSet DonwloadIntegrationDoc(int integrationId);
        void UploadFile(string fileName, IFormFile file);
        int DeleteDraft(string integrationId, string serviceId);
        DataTable GetAPIDetails(SearchAPI searchModel);
        NewIntegration GetBTGProjectmgr();
        NewIntegration GetBTGUserDropDownListData(NewIntegration newIntegration);
        NewIntegration GetCosumerName();
        NewIntegration GetExectingServiceName();
        NewIntegration GetExtenalServiceName();
        FTPCredential GetFTPCredential();
        NewIntegration GetITUserDropDownListData(NewIntegration newIntegration);
        NewIntegration GetProducerName();
        NewIntegration GetQuestionList(NewIntegration newIntegration, int serviceId);
        NewIntegration GetQuestionListInNewchange(NewIntegration newIntegration);
        TestAPI GetTestApiurlAuto();
        NewIntegration InsertQuestion(NewIntegration newIntegration);
        NewIntegration AddFeedback(NewIntegration newIntegration);
        NewIntegration ExistingServiceRecordRepository(int id);
        NewIntegration ExtenalServiceRecordRepository(string externalServiceText);
        NewIntegration ProducerRecordRepository(int id);
        NewIntegration ProducerRecordTextRepository(string fullName);
        NewIntegration CosumerRecordRepository(int id);
        NewIntegration CosumerRecordTextRepositoryT(string consumerAppText);
        PartnerOnboarding GetlstApiDetail();
        PartnerOnboarding EditPartnerDraft(PartnerOnboarding data);
        POFeedbackHistory SaveFeedbackReply(POFeedbackHistory feedback);
        System.Collections.Generic.List<POFeedbackHistory> GetFeedbackDetail(string id);
        System.Collections.Generic.List<POFeedbackHistory> GetFeedbackHistory(string id);
    }
}
