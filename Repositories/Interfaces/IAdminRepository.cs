namespace API_HUNT.Models
{
    public interface IAdminRepository
    {
        AdminMaster AllAdminMaster();
        displayUsermaster UserMasterEdit(string data);
        displayAppmaster AppMasterEdit(string data);
        displayAppmaster AppMasterEditpoc(string appId, string appName);
        string AddUpdateAPPsMaster(displayAppmaster disAppmaster, string? empId);
        string AddUpdateUsersMaster(displayUsermaster disUsermaster, string? empId);
        GetUserDetail GetUserNames(string userId);
    }
}
