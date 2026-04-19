using System.Data;
using Microsoft.AspNetCore.Http;

namespace API_HUNT.Models
{
    public class SubmitRepository : ISubmitRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SubmitRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public DataTable DiagnoseUrl(string radioValue)
        {
            DataTable dt = new DataTable();
            using (var conn = _connectionFactory.CreateConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT URL FROM TBL_API_URLMaster WHERE URLName = @URLName AND Status = 1", conn))
                {
                    cmd.Parameters.AddWithValue("@URLName", radioValue);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public SearchAPI searchAPIList(SearchAPI searchModel)
        {
            searchModel.lstMiddlewares ??= new List<MiddlewareItem>();
            searchModel.lstServiceProviders ??= new List<ServiceProviderItem>();
            searchModel.lstProducer ??= new List<Producer>();
            searchModel.lstConsumer ??= new List<Consumer>();
            searchModel.lstSearchResults ??= new List<SearchAPIItem>();
            searchModel.searchAPILists ??= new List<SearchAPIItem>();
            searchModel.lstInternalConsumer ??= new List<ConsumerItem>();
            searchModel.lstExternalConsumer ??= new List<ConsumerItem>();
            searchModel.lstInternalProducer ??= new List<ProducerItem>();
            searchModel.lstExternalProducer ??= new List<ProducerItem>();
            return searchModel;
        }

        public NewIntegration GetNewIntegrationDetails(NewIntegration newIntegration) => NewIntegration.CreateEmpty();

        public NewIntegration GetFeedbackDetails(NewIntegration newIntegration, string assignFrom) => NewIntegration.CreateEmpty();

        public NewIntegration GetNewIntegrationById(int integrationId) => NewIntegration.CreateEmpty();

        public NewIntegration GetNewIntegrationByIdEditPopwhendbcreate(int integrationId, NewIntegration model) => NewIntegration.CreateEmpty();

        public NewIntegration GetAllDropDownListData(NewIntegration newIntegration)
        {
            newIntegration.ExistingNewList = GetMisccdList("Existing_New");
            newIntegration.RestSoapList = GetMisccdList("Rest_Soap");
            newIntegration.ServiceTypeList = GetMisccdList("Service Type");
            newIntegration.APITypeList = GetMisccdList("API Type");
            newIntegration.APICategoryList = GetMisccdList("API Category");
            newIntegration.DomainNameList = GetMisccdList("Domain Name");
            newIntegration.MiddlewareName = GetMisccdList("Middleware Name");
            newIntegration.ConsumerDCList = GetMisccdList("Consumer DC");
            return newIntegration;
        }

        private List<SelectItem> GetMisccdList(string cdtp)
        {
            var list = new List<SelectItem>();
            using var conn = _connectionFactory.CreateConnection();
            conn.Open();
            using var cmd = new SqlCommand("SELECT MisccdId AS ID, CDValDesc AS Val FROM tbl_API_HUNT_Misccd WHERE CDTP = @CDTP AND Status = 1 ORDER BY Seq", conn);
            cmd.Parameters.AddWithValue("@CDTP", cdtp);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new SelectItem
                {
                    ID = reader.GetInt32(0),
                    Val = reader.GetString(1)
                });
            }
            return list;
        }

        public bool IsConsumerAppValidation(string? consumerApplication) => false;

        public NewIntegration SaveDetails(NewIntegration newIntegration) => NewIntegration.CreateEmpty();

        public NewIntegration SaveServiceDetails(NewIntegration newIntegration) => NewIntegration.CreateEmpty();

        public NewIntegration UpdateNewIntegration(NewIntegration newIntegration) => NewIntegration.CreateEmpty();

        public NewIntegration DarftUpdateServiceDetails(NewIntegration newIntegration) => NewIntegration.CreateEmpty();

        public NewIntegration DraftSaveDetails(NewIntegration newIntegration) => NewIntegration.CreateEmpty();

        public NewIntegration DraftSaveServiceDetails(NewIntegration newIntegration) => NewIntegration.CreateEmpty();

        public NewIntegration UpdateServiceDetails(NewIntegration newIntegration) => NewIntegration.CreateEmpty();

        public DataSet DonwloadIntegrationDoc(int integrationId) => new DataSet();

        public void UploadFile(string fileName, IFormFile file) { }

        public int DeleteDraft(string integrationId, string serviceId) => 0;

        public DataTable GetAPIDetails(SearchAPI searchModel) => new DataTable();

        public NewIntegration GetBTGProjectmgr() => NewIntegration.CreateEmpty();

        public NewIntegration GetBTGUserDropDownListData(NewIntegration newIntegration) => NewIntegration.CreateEmpty();

        public NewIntegration GetCosumerName() => NewIntegration.CreateEmpty();

        public NewIntegration GetExectingServiceName() => NewIntegration.CreateEmpty();

        public NewIntegration GetExtenalServiceName() => NewIntegration.CreateEmpty();

        public FTPCredential GetFTPCredential() => null!;

        public NewIntegration GetITUserDropDownListData(NewIntegration newIntegration) => NewIntegration.CreateEmpty();

        public NewIntegration GetProducerName() => NewIntegration.CreateEmpty();

        public NewIntegration GetQuestionList(NewIntegration newIntegration, int serviceId) => NewIntegration.CreateEmpty();

        public NewIntegration GetQuestionListInNewchange(NewIntegration newIntegration) => NewIntegration.CreateEmpty();

        public TestAPI GetTestApiurlAuto() => null!;

        public NewIntegration InsertQuestion(NewIntegration newIntegration) => NewIntegration.CreateEmpty();

        public NewIntegration AddFeedback(NewIntegration newIntegration) => NewIntegration.CreateEmpty();

        public NewIntegration ExistingServiceRecordRepository(int id) => NewIntegration.CreateEmpty();

        public NewIntegration ExtenalServiceRecordRepository(string externalServiceText) => NewIntegration.CreateEmpty();

        public NewIntegration ProducerRecordRepository(int id) => NewIntegration.CreateEmpty();

        public NewIntegration ProducerRecordTextRepository(string fullName) => NewIntegration.CreateEmpty();

        public NewIntegration CosumerRecordRepository(int id) => NewIntegration.CreateEmpty();

        public NewIntegration CosumerRecordTextRepositoryT(string consumerAppText) => NewIntegration.CreateEmpty();

        public PartnerOnboarding GetlstApiDetail() => null!;

        public PartnerOnboarding EditPartnerDraft(PartnerOnboarding data) => null!;

        public POFeedbackHistory SaveFeedbackReply(POFeedbackHistory feedback) => null!;

        public System.Collections.Generic.List<POFeedbackHistory> GetFeedbackDetail(string id) => new System.Collections.Generic.List<POFeedbackHistory>();

        public System.Collections.Generic.List<POFeedbackHistory> GetFeedbackHistory(string id) => new System.Collections.Generic.List<POFeedbackHistory>();
    }
}
