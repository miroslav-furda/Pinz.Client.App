using Com.Pinz.Client.Commons;
using Com.Pinz.Client.Commons.Prism;
using Com.Pinz.Client.Model.Service;
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
        private IAdminClientService adminClientService;

        [Inject]
        public LoginModel(IAdminClientService adminClientService, IRegionManager regionManager)
        {
            this.adminClientService = adminClientService;
            this.regionManager = regionManager;
            LoginCommand = new DelegateCommand(login);
        }

        private async void login()
        {
            if (!ValidateModel())
            {
                try
                {
                    bool success = await adminClientService.loginUser(UserName, Password);
                    if (success)
                        regionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri("PinzProjectsTabView", UriKind.Relative));
                    else
                        ErrorMessage = Properties.Resources.BadLogin;
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

        private Task<bool> AsyncLogin()
        {
            return Task.Run(() =>
            {
                return adminClientService.loginUser(UserName, Password);
            });
        }
    }
}
