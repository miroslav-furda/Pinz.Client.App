using Com.Pinz.Client.DomainModel;
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

        public ApplicationGlobalModel()
        {
            IsUserLoggedIn = false;
        }

    }
}
