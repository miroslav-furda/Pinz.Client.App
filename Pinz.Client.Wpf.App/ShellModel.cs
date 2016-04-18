using Com.Pinz.Client.Model.Service;
using Ninject;
using Prism.Mvvm;

namespace Com.Pinz.Client.Wpf.App
{
    public class ShellModel : BindableBase
    {
        private bool _isAdminEnabled;
        public bool IsAdminEnabled
        {
            get
            {
                return _isAdminEnabled;
            }
            private set
            {
                SetProperty(ref this._isAdminEnabled, value);
            }
        }

        [Inject]
        public ShellModel(IAdminClientService adminService)
        {
            IsAdminEnabled = adminService.IsUserLoggedIn;
        }
    }
}
