using Com.Pinz.Client.RemoteServiceConsumer.Callback;
using Prism.Mvvm;

namespace Com.Pinz.Client.Module.Main.Model
{
    public class MainModuleModel : BindableBase, IServiceRunningIndicator
    {
        private bool _isServiceRunning;
        public bool IsServiceRunning
        {
            get
            {
                return _isServiceRunning;
            }
            set
            {
                SetProperty(ref this._isServiceRunning, value);
            }
        }

        public MainModuleModel()
        {
            IsServiceRunning = false;
        }
    }
}
