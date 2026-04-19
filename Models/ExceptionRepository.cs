using System.Collections.Generic;

namespace API_HUNT.Models
{
    public class ExceptionRepository : IExceptionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ExceptionRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public List<ExceptionDetails> ExceptionList() => new List<ExceptionDetails>();

        public AddEditExceptionModel GetExceptionDetail(string caseId) => null!;

        public AddEditExceptionModel SaveAddEditException(AddEditExceptionModel model) => null!;

        public AddEditExceptionModel GetExceptionLevel(string impactOnBank) => null!;

        public ExceptionBindModel BindExcpLevel1() => new ExceptionBindModel();

        public ExceptionBindModel BindExcpLevel2() => new ExceptionBindModel();

        public ExceptionBindModel BindExcpLevel3() => new ExceptionBindModel();

        public object SaveExceptionLevel(ExceptionLevels exception) => null!;

        public List<ExceptionDetails> GetExceptionApproval(string approverUserID) => new List<ExceptionDetails>();
    }
}
