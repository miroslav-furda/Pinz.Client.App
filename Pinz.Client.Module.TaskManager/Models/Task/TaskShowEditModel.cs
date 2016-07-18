using Com.Pinz.Client.Commons.Event;
using Com.Pinz.Client.Commons.Prism;
using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.Module.TaskManager.Events;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.DomainModel;
using Ninject;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;

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


        public AwaitableDelegateCommand StartCommand { get; private set; }
        public AwaitableDelegateCommand<bool?> CompleteCommand { get; private set; }
        public DelegateCommand EditCommand { get; private set; }

        private readonly ITaskRemoteService _service;
        private readonly IEventAggregator _eventAggregator;


        [Inject]
        public TaskShowEditModel(ITaskRemoteService service, IEventAggregator eventAggregator)
        {
            this._service = service;
            this._eventAggregator = eventAggregator;
            EditMode = false;

            this.StartCommand = new AwaitableDelegateCommand(OnStart, CanStart);
            this.CompleteCommand = new AwaitableDelegateCommand<bool?>(this.OnComplete);
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
            _eventAggregator.GetEvent<TaskEditStartedEvent>().Publish(Task);
            EditCommand.RaiseCanExecuteChanged();
        }

        private async System.Threading.Tasks.Task OnComplete(bool? selected)
        {
            try
            {
                if (selected == true)
                {
                    await _service.ChangeTaskStatusAsync(Task, TaskStatus.TaskComplete);
                }
                else if (selected == false)
                {
                    await _service.ChangeTaskStatusAsync(Task, TaskStatus.TaskNotStarted);
                }
            }
            catch (TimeoutException timeoutEx)
            {
                _eventAggregator.GetEvent<TimeoutErrorEvent>().Publish(timeoutEx);
            }
            CompleteCommand.RaiseCanExecuteChanged();
            StartCommand.RaiseCanExecuteChanged();
        }

        private async System.Threading.Tasks.Task OnStart()
        {
            try
            {
                await _service.ChangeTaskStatusAsync(Task, TaskStatus.TaskInProgress);
            }
            catch (TimeoutException timeoutEx)
            {
                _eventAggregator.GetEvent<TimeoutErrorEvent>().Publish(timeoutEx);
            }
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
