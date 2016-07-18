using Com.Pinz.Client.Commons;
using Com.Pinz.Client.Commons.Prism;
using Com.Pinz.Client.Model;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Common.Logging;
using Ninject;
using Prism.Regions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Com.Pinz.Client.Module.Login.Infrastructure;
using System.Windows.Input;
using System.ServiceModel.Security;
using Prism.Events;
using Com.Pinz.Client.Commons.Event;

namespace Com.Pinz.Client.Module.Login.Model
{
    public class LoginModel : BindableValidationBase
    {
        private static readonly ILog Log = LogManager.GetLogger<LoginModel>();
        private readonly IsolatedStorageSettings settings = new IsolatedStorageSettings();
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

        private bool _autoLogin;
        public bool AutoLogin
        {
            get { return _autoLogin; }
            set { SetProperty(ref _autoLogin, value); }
        }

        public ICommand LoginCommand { get; private set; }
        public ICommand LoadedCommand { get; private set; }

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
        private readonly IEventAggregator _eventAggregator;

        private TaskScheduler scheduler;

        [Inject]
        public LoginModel(ApplicationGlobalModel applicationGlobalModel, UserNameClientCredentials userCredentials,
            IAuthorisationRemoteService authorisationService, IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.applicationGlobalModel = applicationGlobalModel;
            this.regionManager = regionManager;
            this.userCredentials = userCredentials;
            this.authorisationService = authorisationService;
            this._eventAggregator = eventAggregator;

            LoginCommand = new AwaitableDelegateCommand(Login);
            LoadedCommand = new AwaitableDelegateCommand(Loaded);

            scheduler = TaskScheduler.FromCurrentSynchronizationContext();

            LoadPreviousSettings();
        }

        private async Task Login()
        {
            if (ValidateModel())
            {
                try
                {
                    ErrorMessage = null;
                    userCredentials.UserName = UserName;
                    userCredentials.Password = Password;
                    userCredentials.UpdateCredentialsForAllFactories();

                    Task<DomainModel.User> readTask = authorisationService.ReadUserByEmailAsync(UserName);
                    applicationGlobalModel.CurrentUser = await readTask;

                    applicationGlobalModel.IsUserLoggedIn = true;
                    Log.Debug("login succesfull, navigate to PinzProjectsTabView");
                    SaveSettings();
                    regionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri("PinzProjectsTabView", UriKind.Relative), (r) =>
                    {
                        if (false == r.Result)
                            Log.ErrorFormat("Error navigating to PinzProjectsTabView, URI:{0}", r.Error, r.Context.Uri);
                    });
                }
                catch (MessageSecurityException ex)
                {
                    Log.ErrorFormat("Error logging in with user {0}", ex, UserName);
                    ErrorMessage = Properties.Resources.BadLogin;
                }
                catch(TimeoutException timeoutEx)
                {
                    _eventAggregator.GetEvent<TimeoutErrorEvent>().Publish(timeoutEx);
                }
            }
        }

        private async Task Loaded()
        {
            if (AutoLogin && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                await Login();
            }
        }

        private void LoadPreviousSettings()
        {
            AutoLogin = settings.GetValue("AutoLogin", true);
            UserName = settings.GetValue("UserName");
            Password = settings.GetValue("Password");
        }

        private void SaveSettings()
        {
            settings.SetValue("AutoLogin", AutoLogin);
            settings.SetValue("UserName", UserName);
            settings.SetValue("Password", Password);
            settings.Save();
        }
    }
}
