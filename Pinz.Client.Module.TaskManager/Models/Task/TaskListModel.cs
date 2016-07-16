using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using Com.Pinz.Client.Module.TaskManager.Models.Category;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using GongSolutions.Wpf.DragDrop;
using Ninject;
using Prism.Mvvm;
using Com.Pinz.Client.Commons.Prism;
using Com.Pinz.Client.Commons.Model;
using Com.Pinz.DomainModel;
using System.Collections.Generic;
using Com.Pinz.Client.Model;
using Prism.Events;
using Com.Pinz.Client.Module.TaskManager.Events;
using System.Linq;
using Com.Pinz.Client.DomainModel;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class TaskListModel : BindableBase, IDropTarget
    {
        public AwaitableDelegateCommand CreateTask { get; private set; }

        private ObservableCollection<Task> _tasks;
        public ObservableCollection<Task> Tasks
        {
            get { return _tasks; }
            set { SetProperty(ref _tasks, value); }
        }

        private CategoryModel _category;
        public CategoryModel Category
        {
            get { return _category; }
            set
            {
                SetProperty(ref _category, value);
                LoadTasks();
            }
        }

        private readonly ITaskRemoteService _service;
        private readonly TaskFilter _taskFilter;
        private List<DomainModel.Task> _allTasksFromServer;
        private DomainModel.User _currentUser;
        private IEventAggregator _eventAggregator;


        [Inject]
        public TaskListModel(ITaskRemoteService service, TaskFilter filter, ApplicationGlobalModel applicationGlobalModel, IEventAggregator eventAggregator)
        {
            this._service = service;
            this._taskFilter = filter;
            this._eventAggregator = eventAggregator;
            this.Tasks = new ObservableCollection<Task>();
            CreateTask = new AwaitableDelegateCommand(OnCreateTask);
            this._taskFilter.PropertyChanged += Filter_PropertyChanged;
            _currentUser = applicationGlobalModel.CurrentUser;

            TaskDeletedEvent taskDeletedEvent = eventAggregator.GetEvent<TaskDeletedEvent>();
            taskDeletedEvent.Subscribe(OnDeleteTask, ThreadOption.UIThread, false, t => Category != null && t.CategoryId == Category.CategoryId);
        }

        private void OnDeleteTask(Task taskToDelete)
        {
            Tasks.Remove(taskToDelete);
            var allTaskToDelete = _allTasksFromServer.Where(t => t.TaskId == taskToDelete.TaskId).First();
            _allTasksFromServer.Remove(allTaskToDelete);
        }


        #region private Filter

        private void Filter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_allTasksFromServer != null)
                updateTaskObservableCollection();
        }

        private bool filterTasks(DomainModel.Task taskitem)
        {
            bool retval = true;

            if (!_taskFilter.Complete)
            {
                retval = taskitem.Status != TaskStatus.TaskComplete;
            }

            if (retval && _taskFilter.DueToday)
            {
                System.DateTime today = System.DateTime.Today;
                retval = taskitem.DueDate.Equals(today);
            }

            if (retval && _taskFilter.InProgress && _taskFilter.NotStarted)
            {
                retval = taskitem.Status == TaskStatus.TaskInProgress ||
                   taskitem.Status  == TaskStatus.TaskNotStarted;
            }
            else if (retval && _taskFilter.InProgress)
            {
                retval = taskitem.Status == TaskStatus.TaskInProgress;
            }
            else if (retval && _taskFilter.NotStarted)
            {
                retval = taskitem.Status == TaskStatus.TaskNotStarted;
            }

            if (retval && _taskFilter.MyTasks)
            {
                retval = taskitem.UserId == _currentUser.UserId;

            }
            return retval;
        }
        #endregion


        #region Drag and Drop

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as Task;
            var targetItem = dropInfo.TargetItem as Task;


            if (sourceItem != null && ((targetItem != null && sourceItem.CategoryId != targetItem.CategoryId) || targetItem == null))
            {
                Debug.WriteLine("DragOver called with source:{0} and target:{1}", sourceItem, targetItem);
                //dropInfo.DestinationText = sourceItem.TaskName;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
            else
            {
                Debug.WriteLine("DragOver None");
                dropInfo.Effects = DragDropEffects.None;
            }
        }

        async void IDropTarget.Drop(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as Task;
            var targetItem = dropInfo.TargetItem as Task;

            await _service.MoveTaskToCategoryAsync(sourceItem, Category);
        }

        #endregion

        private async System.Threading.Tasks.Task OnCreateTask()
        {
            var newTask = await _service.CreateTaskInCategoryAsync(Category);
            _allTasksFromServer.Add(newTask);
            Tasks.Add(newTask);
        }

        private async System.Threading.Tasks.Task LoadTasks()
        {
            if (Category != null)
            {
                _allTasksFromServer = await _service.ReadAllTasksByCategoryAsync(Category);
                updateTaskObservableCollection();
                Category.Tasks = Tasks;
            }
            else
            {
                Tasks.Clear();
            }
        }

        private void updateTaskObservableCollection()
        {
            Tasks.Clear();
            foreach (var task in _allTasksFromServer)
            {
                if (filterTasks(task))
                    Tasks.Add(task);
            }
        }
    }
}