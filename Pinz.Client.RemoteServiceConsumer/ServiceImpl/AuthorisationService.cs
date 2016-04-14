using AutoMapper;
using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Ninject;
using System.ServiceModel;

namespace Com.Pinz.Client.RemoteServiceConsumer.ServiceImpl
{
    internal class AuthorisationService : ServiceBase, IAuthorisationRemoteService
    {
        private IMapper mapper;

        private ChannelFactory<AuthorisationServiceReference.IAuthorisationService> clientFactory;
        private AuthorisationServiceReference.IAuthorisationService channel;

        [Inject]
        public AuthorisationService([Named("ServiceConsumerMapper")] IMapper mapper, ChannelFactory<AuthorisationServiceReference.IAuthorisationService> clientFactory)
        {
            this.mapper = mapper;
            this.clientFactory = clientFactory;

        }
        
        public bool IsUserComapnyAdmin(User user)
        {
            return channel.IsUserComapnyAdmin(user.UserId);
        }

        public bool IsUserProjectAdmin(User user, Project project)
        {
            return channel.IsUserProjectAdmin(user.UserId, project.ProjectId);
        }

        public User ReadUserByEmail(string email)
        {
            return mapper.Map<User>(channel.ReadUserByEmail(email));
        }

        public override void OpenChannel()
        {
            channel = clientFactory.CreateChannel();
        }

        public override void CloseChannel()
        {
            CloseOrAbortServiceChannel(channel as ICommunicationObject);
        }

    }
}
