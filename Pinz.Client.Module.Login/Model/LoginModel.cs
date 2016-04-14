using Com.Pinz.Client.Commons;
using Com.Pinz.Client.Model.Service;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Ninject;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Threading.Tasks;

namespace Com.Pinz.Client.Module.Login.Model
{
    public class LoginModel : BindableBase
    {
        private bool _isNotWorking;
        public bool IsNotWorking
        {
            get
            {
                return _isNotWorking;
            }
            private set
            {
                SetProperty(ref this._isNotWorking, value);
            }
        }

        public DelegateCommand LoginCommand { get; private set; }
        public UserNameClientCredentials Credentials { get; private set; }

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
        private ITaskClientService taskService;

        [Inject]
        public LoginModel(UserNameClientCredentials credentials, IRegionManager regionManager, ITaskClientService taskService)
        {
            this.Credentials = credentials;
            this.regionManager = regionManager;
            this.taskService = taskService;
            IsNotWorking = true;

            LoginCommand = new DelegateCommand(login);

            Credentials.UserName = "test@test.com";
            Credentials.Password = "test";
        }

        private async void login()
        {
            try
            {
                IsNotWorking = false;
                Credentials.UpdateCredentialsForAllFactories();
                await AsyncLogin();
                regionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri("PinzProjectsTabView", UriKind.Relative));
            }
            catch
            {
                ErrorMessage = Properties.Resources.BadLogin;
            }
            finally
            {
                IsNotWorking = true;
            }
        }

        private Task AsyncLogin()
        {
            return Task.Run(() =>
            {
                taskService.ReadAllProjectsForCurrentUser();
            });
        }
    }
}
