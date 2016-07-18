using System;
using Com.Pinz.Client.Commons.Event;
using Com.Pinz.Client.RemoteServiceConsumer.Callback;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
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

        public InteractionRequest<INotification> TimeoutNotification { get; private set; }

        public MainModuleModel(IEventAggregator eventAggregator)
        {
            IsServiceRunning = false;

            TimeoutNotification = new InteractionRequest<INotification>();
            var timeoutErrorEvent = eventAggregator.GetEvent<TimeoutErrorEvent>();
            timeoutErrorEvent.Subscribe(TimeoutEventHandler);
        }

        private void TimeoutEventHandler(TimeoutException obj)
        {
            TimeoutNotification.Raise(new Notification()
            {
                Title = Properties.Resources.Error_Timeout_Title,
                Content = Properties.Resources.Error_Timeout_Content
                
            });
        }

    }
}
