namespace API_HUNT.Models
{
    public interface IPartnerOffboardingRepository
    {
        PartnerOffboarding GetPartnername();
        PartnerOffboarding GetApiname(string partnerName);
        object UpdateAPIStatus(string apiName, bool isActive);
        object DeleteAPI(string apiName);
    }
}
