namespace API_HUNT.Models
{
    public interface ILoginRepository
    {
        LoginUserRecord? GetUserMasterData(string userName);
        string GetUserRole(string userId);
        string GetMofeeUrl();
    }
}
