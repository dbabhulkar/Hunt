namespace API_HUNT.Models
{
    public class JIRARepository : IJiraRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public JIRARepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public object PayloadForAPIINT(NewIntegration newIntegration, string? producerApplication, int count, string prodApp) => null!;

        public object PayloadForAPIGATE(NewIntegration newIntegration, string? producerApplication, int count, string prodApp) => null!;

        public object PayloadForSOAENHN(NewIntegration newIntegration, string? producerApplication, int serviceID, int count, string prodApp) => null!;

        public object PayloadForOBPINT(NewIntegration newIntegration, string? producerApplication, int serviceID, int count, string prodApp) => null!;

        public object PayloadForOBPENH(NewIntegration newIntegration, string? producerApplication, int serviceID, int count, string prodApp) => null!;

        public void InsertJIRAId(int serviceID, string key) { }
    }
}
