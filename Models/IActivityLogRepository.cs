namespace API_HUNT.Models
{
    public interface IActivityLogRepository
    {
        void LogActivity(string empCode, string formName, string moduleName, int totalCount, string activity, string activityDetails);
    }
}
