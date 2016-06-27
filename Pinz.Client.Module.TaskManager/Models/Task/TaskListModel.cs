using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using Com.Pinz.Client.Module.TaskManager.Models.Category;
using Com.Pinz.Client.Module.TaskManager.Models.Task;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using GongSolutions.Wpf.DragDrop;
using Ninject;
using Prism.Commands;
using Prism.Mvvm;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class TaskListModel : BindableBase, IDropTarget
    {
        private ObservableCollection<TaskModel> tasks;


        private CategoryModel category;

        private readonly ITaskRemoteService service;

        [Inject]
        public TaskListModel(ITaskRemoteService service)
        {
            this.service = service;
            this.tasks = new ObservableCollection<TaskModel>();
            CreateTask = new DelegateCommand(OnCreateTask);
        }

        public DelegateCommand CreateTask { get; private set; }

        public ObservableCollection<TaskModel> Tasks
        {
            get { return tasks; }
            set { SetProperty(ref tasks, value); }
        }

        public CategoryModel Category
        {
            get { return category; }
            set
            {
                SetProperty(ref category, value);
                LoadTasks();
            }
        }

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

            await System.Threading.Tasks.Task.Run(() => service.MoveTaskToCategory(sourceItem, Category));
        }

        private async void OnCreateTask()
        {
            await System.Threading.Tasks.Task.Run(() => service.CreateTaskInCategory(Category));
        }

        private async void LoadTasks()
        {
            if (Category != null)
            {                
                Tasks.Clear();                
                var tasks = await System.Threading.Tasks.Task.Run(() => service.ReadAllTasksByCategory(Category));
                foreach (var task in tasks)
                {
                    Tasks.Add(new TaskModel(task, category));
                }
                Category.Tasks = Tasks;
            }
            else
            {
                Tasks.Clear();
            }
        }
    }
}