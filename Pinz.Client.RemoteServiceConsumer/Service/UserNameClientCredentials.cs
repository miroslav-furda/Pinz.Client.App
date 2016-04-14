using System.ServiceModel;
using System.ServiceModel.Description;

namespace Com.Pinz.Client.RemoteServiceConsumer.Service
{
    public class UserNameClientCredentials
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        private ChannelFactory<AdministrationServiceReference.IAdministrationService> _channelFactoryAdmin;
        private ChannelFactory<AuthorisationServiceReference.IAuthorisationService> _channelFactoryAuthorisation;
        private ChannelFactory<PinzAdminServiceReference.IPinzAdminService> _channelFactoryPinzAdmin;
        private ChannelFactory<TaskServiceReference.ITaskService> _channelFactoryTask;

        public UserNameClientCredentials(ChannelFactory<AdministrationServiceReference.IAdministrationService> channelFactoryAdmin,
            ChannelFactory<AuthorisationServiceReference.IAuthorisationService> channelFactoryAuthorisation,
            ChannelFactory<PinzAdminServiceReference.IPinzAdminService> channelFactoryPinzAdmin,
            ChannelFactory<TaskServiceReference.ITaskService> channelFactoryTask)
        {
            this._channelFactoryAdmin = channelFactoryAdmin;
            this._channelFactoryAuthorisation = channelFactoryAuthorisation;
            this._channelFactoryPinzAdmin = channelFactoryPinzAdmin;
            this._channelFactoryTask = channelFactoryTask;
        }

        public void UpdateCredentialsForAllFactories()
        {
            UpdateCredentials(_channelFactoryAdmin);
            UpdateCredentials(_channelFactoryAuthorisation);
            UpdateCredentials(_channelFactoryPinzAdmin);
            UpdateCredentials(_channelFactoryTask);
        }

        private void UpdateCredentials(ChannelFactory channelFactory)
        {
            var defaultCredentials = channelFactory.Endpoint.Behaviors.Find<ClientCredentials>();
            defaultCredentials.UserName.UserName = UserName;
            defaultCredentials.UserName.Password = Password;
        }
    }
}