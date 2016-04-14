using AutoMapper;
using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Ninject;
using System.ServiceModel;

namespace Com.Pinz.Client.RemoteServiceConsumer.ServiceImpl
{
    internal class PinzAdminService : ServiceBase, IPinzAdminRemoteService
    {
        private IMapper mapper;

        private ChannelFactory<PinzAdminServiceReference.IPinzAdminService> clientFactory;
        private PinzAdminServiceReference.IPinzAdminService channel;

        [Inject]
        public PinzAdminService([Named("ServiceConsumerMapper")] IMapper mapper, ChannelFactory<PinzAdminServiceReference.IPinzAdminService> clientFactory)
        {
            this.mapper = mapper;
            this.clientFactory = clientFactory;
        }

        public override void OpenChannel()
        {
            channel = clientFactory.CreateChannel();
        }

        public override void CloseChannel()
        {
            CloseOrAbortServiceChannel(channel as ICommunicationObject);
        }

        public Company CreateCompany(Company company)
        {
            PinzAdminServiceReference.CompanyDO rCompIn = mapper.Map<PinzAdminServiceReference.CompanyDO>(company);
            PinzAdminServiceReference.CompanyDO rCompany = channel.CreateCompany(rCompIn);
            mapper.Map(rCompany, company);
            return company;
        }

        public void UpdateCompany(Company company)
        {
            channel.UpdateCompany(mapper.Map<PinzAdminServiceReference.CompanyDO>(company));
        }

        public void DeleteCompany(Company company)
        {
            channel.DeleteCompany(mapper.Map<PinzAdminServiceReference.CompanyDO>(company));
        }
    }
}
