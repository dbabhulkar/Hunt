namespace API_HUNT.Models
{
    public class AdminRepository : IAdminRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AdminRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public AdminMaster AllAdminMaster() => null!;

        public displayUsermaster UserMasterEdit(string data) => null!;

        public displayAppmaster AppMasterEdit(string data) => null!;

        public displayAppmaster AppMasterEditpoc(string appId, string appName) => null!;

        public string AddUpdateAPPsMaster(displayAppmaster disAppmaster, string? empId) => string.Empty;

        public string AddUpdateUsersMaster(displayUsermaster disUsermaster, string? empId) => string.Empty;

        public GetUserDetail GetUserNames(string userId) => null!;
    }
}
