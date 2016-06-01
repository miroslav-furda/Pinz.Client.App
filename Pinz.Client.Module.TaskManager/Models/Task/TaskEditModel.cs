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

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class TaskEditModel : BindableBase
    {
        private TaskModel task;
        public TaskModel Task
        {
            get
            {
                return task;
            }
            set
            {
                if (SetProperty(ref this.task, value))
                    Users = Task.Category.Project.ProjectUsers;
            }
        }

        private bool editMode;
        public bool EditMode
        {
            get
            {
                return editMode;
            }
            set
            {
                SetProperty(ref this.editMode, value);
            }
        }

        private ObservableCollection<UserModel> users;
        public ObservableCollection<UserModel> Users
        {
            get
            {
                return users;
            }
            set
            {
                SetProperty(ref this.users, value);
            }
        }

        public DelegateCommand OkCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }
        public InteractionRequest<IConfirmation> DeleteConfirmation { get; private set; }

        private IEventAggregator eventAggregator;
        private ITaskRemoteService service;        
        private TaskModel originalTask;
        private IMapper mapper;

        [Inject]
        public TaskEditModel(ITaskRemoteService service, IEventAggregator eventAggregator, [Named("WpfClientMapper")] IMapper mapper)
        {
            this.service = service;            
            this.eventAggregator = eventAggregator;
            this.mapper = mapper;
            this.EditMode = false;
            this.originalTask = null;
            this.users = new ObservableCollection<UserModel>();

            TaskEditStartedEvent taskEditStartEvent = eventAggregator.GetEvent<TaskEditStartedEvent>();
            taskEditStartEvent.Subscribe(StartEdit, ThreadOption.UIThread, false, t => t == Task);
            taskEditStartEvent.Subscribe(OnCancelExecute, ThreadOption.UIThread, false, t => t != Task);
            CategoryEditStartedEvent categoryEditEvent = eventAggregator.GetEvent<CategoryEditStartedEvent>();
            categoryEditEvent.Subscribe(OnCancelExecute);

            OkCommand = new DelegateCommand(OnOkExecute);
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
                    eventAggregator.GetEvent<TaskEditFinishedEvent>().Publish(Task);
                    await System.Threading.Tasks.Task.Run(() => service.DeleteTask(this.Task));
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
            mapper.Map(originalTask, Task);
            EditMode = false;
            eventAggregator.GetEvent<TaskEditFinishedEvent>().Publish(Task);
        }

        private async void OnOkExecute()
        {
            await System.Threading.Tasks.Task.Run(() => service.UpdateTask(this.Task));
            EditMode = false;
            eventAggregator.GetEvent<TaskEditFinishedEvent>().Publish(Task);
        }

        private void StartEdit(TaskModel obj)
        {
            originalTask = mapper.Map<TaskModel>(Task);
            EditMode = true;
        }       
    }
}
