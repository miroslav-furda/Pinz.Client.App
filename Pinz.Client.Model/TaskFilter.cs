using Prism.Mvvm;

namespace Com.Pinz.Client.Model
{
    public class TaskFilter : BindableBase
    {
        private bool _complete;
        private bool _dueToday;
        private bool _notStarted;
        private bool _inProgress;

        public bool Complete
        {
            get { return _complete; }
            set { SetProperty(ref this._complete, value); }
        }

        public bool DueToday
        {
            get { return _dueToday; }
            set { SetProperty(ref this._dueToday, value); }
        }

        public bool NotStarted
        {
            get { return _notStarted; }
            set { SetProperty(ref this._notStarted, value); }
        }

        public bool InProgress
        {
            get { return _inProgress; }
            set { SetProperty(ref this._inProgress, value); }
        }
    }
}
