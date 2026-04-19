namespace API_HUNT.Models
{
    public class PartnerOffboardingRepository : IPartnerOffboardingRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PartnerOffboardingRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public PartnerOffboarding GetPartnername() => null!;

        public PartnerOffboarding GetApiname(string partnerName) => null!;

        public object UpdateAPIStatus(string apiName, bool isActive) => null!;

        public object DeleteAPI(string apiName) => null!;
    }
}
