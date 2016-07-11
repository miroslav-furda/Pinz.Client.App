using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using Com.Pinz.Client.Module.TaskManager.Models.Category;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using GongSolutions.Wpf.DragDrop;
using Ninject;
using Prism.Commands;
using Prism.Mvvm;
using Com.Pinz.Client.Commons.Prism;
using Com.Pinz.Client.Commons.Model;
using Com.Pinz.DomainModel;
using System.Collections.Generic;
using Com.Pinz.Client.Module.TaskManager.Models.Task;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class TaskListModel : BindableBase, IDropTarget
    {
        public AwaitableDelegateCommand CreateTask { get; private set; }

        private ObservableCollection<TaskModel> _tasks;
        public ObservableCollection<TaskModel> Tasks
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

        private readonly ITaskRemoteService service;
        private readonly TaskFilter taskFilter;
        private List<DomainModel.Task> allTasksFromServer;

        [Inject]
        public TaskListModel(ITaskRemoteService service, TaskFilter filter)
        {
            this.service = service;
            this.taskFilter = filter;
            this.Tasks = new ObservableCollection<TaskModel>();
            CreateTask = new AwaitableDelegateCommand(OnCreateTask);
            this.taskFilter.PropertyChanged += Filter_PropertyChanged;
        }


        #region private Filter

        private void Filter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            updateTaskObservableCollection();
        }

        private bool filterTasks(DomainModel.Task taskitem)
        {
            bool retval = true;

            if (!taskFilter.Complete)
            {
                retval = taskitem.IsComplete.Equals(false);
            }

            if (retval && taskFilter.DueToday)
            {
                System.DateTime today = System.DateTime.Today;
                retval = taskitem.DueDate.Equals(today);
            }

            if (retval && taskFilter.InProgress)
            {
                retval = taskitem.Status.Equals(TaskStatus.TaskInProgress);
                if (taskFilter.NotStarted)
                {
                    retval = taskitem.Status.Equals(TaskStatus.TaskNotStarted);
                }
            }
            return retval;
        }
        #endregion


        #region Drag and Drop

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as TaskModel;
            var targetItem = dropInfo.TargetItem as TaskModel;


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
            var sourceItem = dropInfo.Data as TaskModel;
            var targetItem = dropInfo.TargetItem as TaskModel;

            await service.MoveTaskToCategoryAsync(sourceItem, Category);
        }

        #endregion

        private async System.Threading.Tasks.Task OnCreateTask()
        {
            await service.CreateTaskInCategoryAsync(Category);
        }

        private async System.Threading.Tasks.Task LoadTasks()
        {
            if (Category != null)
            {
                allTasksFromServer = await service.ReadAllTasksByCategoryAsync(Category);
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
            foreach (var task in allTasksFromServer)
            {
                if (filterTasks(task))
                    Tasks.Add(new TaskModel(task, _category));
            }
        }
    }
}