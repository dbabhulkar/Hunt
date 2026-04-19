namespace API_HUNT.Models
{
    public interface IJiraRepository
    {
        object PayloadForAPIINT(NewIntegration newIntegration, string? producerApplication, int count, string prodApp);
        object PayloadForAPIGATE(NewIntegration newIntegration, string? producerApplication, int count, string prodApp);
        object PayloadForSOAENHN(NewIntegration newIntegration, string? producerApplication, int serviceID, int count, string prodApp);
        object PayloadForOBPINT(NewIntegration newIntegration, string? producerApplication, int serviceID, int count, string prodApp);
        object PayloadForOBPENH(NewIntegration newIntegration, string? producerApplication, int serviceID, int count, string prodApp);
        void InsertJIRAId(int serviceID, string key);
    }
}
