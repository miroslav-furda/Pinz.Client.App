using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.Module.TaskManager.Events;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.DomainModel;
using Ninject;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class TaskShowEditModel : BindableBase
    {
        private Task _task;
        public Task Task
        {
            get
            {
                return _task;
            }
            set
            {
                SetProperty(ref this._task, value);
                value.PropertyChanged += Task_PropertyChanged;
            }
        }

        private bool _editMode;
        public bool EditMode
        {
            get
            {
                return _editMode;
            }
            set
            {
                SetProperty(ref this._editMode, value);
            }
        }


        public DelegateCommand StartCommand { get; private set; }
        public DelegateCommand<bool?> CompleteCommand { get; private set; }
        public DelegateCommand EditCommand { get; private set; }

        private ITaskRemoteService service;
        private IEventAggregator eventAggregator;


        [Inject]
        public TaskShowEditModel(ITaskRemoteService service, IEventAggregator eventAggregator)
        {
            this.service = service;
            this.eventAggregator = eventAggregator;
            EditMode = false;

            this.StartCommand = new DelegateCommand(OnStart, CanStart);
            this.CompleteCommand = new DelegateCommand<bool?>(this.OnComplete);
            this.EditCommand = new DelegateCommand(OnEdit, CanEdit);

            TaskEditFinishedEvent taskEditFinishedEvent = eventAggregator.GetEvent<TaskEditFinishedEvent>();
            taskEditFinishedEvent.Subscribe(StopEdit, ThreadOption.UIThread, false, t => t == Task);

        }

        private bool CanEdit()
        {
            return !EditMode;
        }

        private void StopEdit(Task obj)
        {
            EditMode = false;
            EditCommand.RaiseCanExecuteChanged();
        }

        private void OnEdit()
        {
            EditMode = true;
            eventAggregator.GetEvent<TaskEditStartedEvent>().Publish(Task);
            EditCommand.RaiseCanExecuteChanged();
        }

        private async void OnComplete(bool? selected)
        {
            if (selected == true)
            {
                await System.Threading.Tasks.Task.Run(() => service.ChangeTaskStatus(Task, TaskStatus.TaskComplete));
            }
            else if (selected == false)
            {
                await System.Threading.Tasks.Task.Run(() => service.ChangeTaskStatus(Task, TaskStatus.TaskNotStarted));
            }
            CompleteCommand.RaiseCanExecuteChanged();
            StartCommand.RaiseCanExecuteChanged();
        }

        private async void OnStart()
        {
            await System.Threading.Tasks.Task.Run(() => service.ChangeTaskStatus(Task, TaskStatus.TaskInProgress));
            StartCommand.RaiseCanExecuteChanged();
        }

        private bool CanStart()
        {
            return TaskStatus.TaskNotStarted.Equals(this.Task.Status);
        }

        private void Task_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ("Status".Equals(e.PropertyName))
                StartCommand.RaiseCanExecuteChanged();
        }
    }
}
