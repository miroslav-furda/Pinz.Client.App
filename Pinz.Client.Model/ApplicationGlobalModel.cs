using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Prism.Mvvm;

namespace Com.Pinz.Client.Model
{
    public class ApplicationGlobalModel : BindableBase
    {
        public User CurrentUser { get; set; }

        private bool _isUserLoggedIn;
        public bool IsUserLoggedIn
        {
            get
            {
                return _isUserLoggedIn;
            }
            set
            {
                SetProperty(ref this._isUserLoggedIn, value);
            }
        }

        private UserNameClientCredentials userCredentials;
        private IAuthorisationRemoteService authorisationService;
        private IAdministrationRemoteService administrationService;

        public ApplicationGlobalModel(UserNameClientCredentials userCredentials, IAuthorisationRemoteService authorisationService, 
            IAdministrationRemoteService administrationService)
        {
            this.userCredentials = userCredentials;
            this.authorisationService = authorisationService;
            this.administrationService = administrationService;
        }

        public User loginUser(string email, string password)
        {
            userCredentials.UserName = email;
            userCredentials.Password = password;
            userCredentials.UpdateCredentialsForAllFactories();

            CurrentUser = authorisationService.ReadUserByEmail(email);
            IsUserLoggedIn = true;
            return CurrentUser;
        }
    }
}
