using System.Collections.Generic;

namespace API_HUNT.Models
{
    public interface IExceptionRepository
    {
        List<ExceptionDetails> ExceptionList();
        AddEditExceptionModel GetExceptionDetail(string caseId);
        AddEditExceptionModel SaveAddEditException(AddEditExceptionModel model);
        AddEditExceptionModel GetExceptionLevel(string impactOnBank);
        ExceptionBindModel BindExcpLevel1();
        ExceptionBindModel BindExcpLevel2();
        ExceptionBindModel BindExcpLevel3();
        object SaveExceptionLevel(ExceptionLevels exception);
        List<ExceptionDetails> GetExceptionApproval(string approverUserID);
    }
}
