using Com.Pinz.Client.DomainModel;

namespace Com.Pinz.Client.RemoteServiceConsumer.Service
{
    public interface IPinzAdminRemoteService
    {
        Company CreateCompany(Company company);

        void UpdateCompany(Company company);

        void DeleteCompany(Company company);
    }
}
