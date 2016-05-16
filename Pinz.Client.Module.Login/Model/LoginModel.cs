using Com.Pinz.Client.Commons;
using Com.Pinz.Client.Commons.Prism;
using Com.Pinz.Client.Model;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Ninject;
using Prism.Commands;
using Prism.Regions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Com.Pinz.Client.Module.Login.Model
{
    public class LoginModel : BindableValidationBase
    {
        private string _userName;
        [Required]
        [EmailAddress]
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        private string _password;
        [Required]
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public DelegateCommand LoginCommand { get; private set; }

        private string _errorMessage;
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                SetProperty(ref this._errorMessage, value);
            }
        }

        private IRegionManager regionManager;
        private ApplicationGlobalModel applicationGlobalModel;
        private UserNameClientCredentials userCredentials;
        private IAuthorisationRemoteService authorisationService;

        [Inject]
        public LoginModel(ApplicationGlobalModel applicationGlobalModel, UserNameClientCredentials userCredentials,
            IAuthorisationRemoteService authorisationService, IRegionManager regionManager)
        {
            this.applicationGlobalModel = applicationGlobalModel;
            this.regionManager = regionManager;
            this.userCredentials = userCredentials;
            this.authorisationService = authorisationService;

            LoginCommand = new DelegateCommand(login);
        }

        private async void login()
        {
            if (!ValidateModel())
            {
                try
                {
                    await System.Threading.Tasks.Task.Run(() => loginUser(UserName, Password));
                    regionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri("/PinzProjectsTabView", UriKind.Relative));
                }
                catch
                {
                    ErrorMessage = Properties.Resources.BadLogin;
                }
                finally
                {
                }
            }
        }

        public void loginUser(string email, string password)
        {
            userCredentials.UserName = email;
            userCredentials.Password = password;
            userCredentials.UpdateCredentialsForAllFactories();

            applicationGlobalModel.CurrentUser = authorisationService.ReadUserByEmail(email);
            applicationGlobalModel.IsUserLoggedIn = true;
        }
    }
}
