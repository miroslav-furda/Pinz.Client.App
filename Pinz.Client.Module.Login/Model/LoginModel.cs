using Com.Pinz.Client.Commons;
using Com.Pinz.Client.Commons.Prism;
using Com.Pinz.Client.Model;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Common.Logging;
using Ninject;
using Prism.Commands;
using Prism.Regions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Com.Pinz.Client.Module.Login.Model
{
    public class LoginModel : BindableValidationBase
    {
        private static readonly ILog Log = LogManager.GetLogger<LoginModel>();

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

        private TaskScheduler scheduler;

        [Inject]
        public LoginModel(ApplicationGlobalModel applicationGlobalModel, UserNameClientCredentials userCredentials,
            IAuthorisationRemoteService authorisationService, IRegionManager regionManager)
        {
            this.applicationGlobalModel = applicationGlobalModel;
            this.regionManager = regionManager;
            this.userCredentials = userCredentials;
            this.authorisationService = authorisationService;

            LoginCommand = new DelegateCommand(login);

            scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        private async void login()
        {
            if (!ValidateModel())
            {
                try
                {
                    ErrorMessage = null;
                    await Task.Run(() => loginUser(UserName, Password)).ContinueWith(c =>
                    {
                        Log.Debug("login succesfull, navigate to PinzProjectsTabView");
                        regionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri("PinzProjectsTabView", UriKind.Relative), (r) =>
                        {
                            if (false == r.Result)
                                Log.ErrorFormat("Error navigating to PinzProjectsTabView, URI:{0}", r.Error, r.Context.Uri);
                        });
                    }, scheduler);
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("Error logging in with user {0}", ex, UserName);
                    ErrorMessage = Properties.Resources.BadLogin;
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
