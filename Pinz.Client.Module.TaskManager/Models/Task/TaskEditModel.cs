using System.Collections.ObjectModel;
using AutoMapper;
using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.Module.TaskManager.Events;
using Com.Pinz.Client.Module.TaskManager.Models.Task;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Ninject;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Com.Pinz.Client.Commons.Prism;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class TaskEditModel : BindableBase
    {
        private TaskModel _task;
        public TaskModel Task
        {
            get
            {
                return _task;
            }
            set
            {
                if (SetProperty(ref this._task, value))
                    Users = Task.Category.Project.ProjectUsers;
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

        private ObservableCollection<UserModel> _users;
        public ObservableCollection<UserModel> Users
        {
            get
            {
                return _users;
            }
            set
            {
                SetProperty(ref this._users, value);
            }
        }

        public AwaitableDelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }
        public InteractionRequest<IConfirmation> DeleteConfirmation { get; private set; }

        private IEventAggregator _eventAggregator;
        private ITaskRemoteService _service;        
        private TaskModel _originalTask;
        private IMapper _mapper;

        [Inject]
        public TaskEditModel(ITaskRemoteService service, IEventAggregator eventAggregator, [Named("WpfClientMapper")] IMapper mapper)
        {
            this._service = service;            
            this._eventAggregator = eventAggregator;
            this._mapper = mapper;
            this.EditMode = false;
            this._originalTask = null;
            this._users = new ObservableCollection<UserModel>();

            TaskEditStartedEvent taskEditStartEvent = eventAggregator.GetEvent<TaskEditStartedEvent>();
            taskEditStartEvent.Subscribe(StartEdit, ThreadOption.UIThread, false, t => t == Task);
            taskEditStartEvent.Subscribe(OnCancelExecute, ThreadOption.UIThread, false, t => t != Task);
            CategoryEditStartedEvent categoryEditEvent = eventAggregator.GetEvent<CategoryEditStartedEvent>();
            categoryEditEvent.Subscribe(OnCancelExecute);

            OkCommand = new AwaitableDelegateCommand(OnOkExecute);
            CancelCommand = new DelegateCommand(OnCancelExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
            this.DeleteConfirmation = new InteractionRequest<IConfirmation>();
        }

        private void OnDeleteExecute()
        {
            this.DeleteConfirmation.Raise(new Confirmation
            {
                Title = Properties.Resources.DeleteConfirmation_Title,
                Content = Properties.Resources.DeleteConfirmation_Content
            }, async (dialog) =>
            {
                if (dialog.Confirmed)
                {
                    EditMode = false;
                    _eventAggregator.GetEvent<TaskEditFinishedEvent>().Publish(Task);
                    await _service.DeleteTaskAsync(this.Task);
                }
            });
        }

        private void OnCancelExecute(object obj)
        {
            if (EditMode)
                OnCancelExecute();
        }

        private void OnCancelExecute()
        {
            _mapper.Map(_originalTask, Task);
            EditMode = false;
            _eventAggregator.GetEvent<TaskEditFinishedEvent>().Publish(Task);
        }

        private async System.Threading.Tasks.Task OnOkExecute()
        {
            if (Task.ValidateModel())
            {
                await _service.UpdateTaskAsync(Task);
                EditMode = false;
                _eventAggregator.GetEvent<TaskEditFinishedEvent>().Publish(Task);
            }
        }

        private void StartEdit(TaskModel obj)
        {
            _originalTask = _mapper.Map<TaskModel>(Task);
            EditMode = true;
        }       
    }
}
