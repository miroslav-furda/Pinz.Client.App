using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using GongSolutions.Wpf.DragDrop;
using Ninject;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Com.Pinz.Client.Module.TaskManager.Models.Category;
using Com.Pinz.Client.Module.TaskManager.Models.Task;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class TaskListModel : BindableBase, IDropTarget
    {
        public DelegateCommand CreateTask { get; private set; }

        private ObservableCollection<TaskModel> _tasks;
        public ObservableCollection<TaskModel> Tasks
        {
            get
            {
                return _tasks;
            }
            set
            {
                SetProperty(ref this._tasks, value);
            }
        }


        private CategoryModel category;
        public CategoryModel Category
        {
            get
            {
                return category;
            }
            set
            {
                SetProperty(ref this.category, value);
                LoadTasks();
            }
        }

        private ITaskRemoteService service;

        [Inject]
        public TaskListModel(ITaskRemoteService service)
        {
            this.service = service;
            Tasks = new ObservableCollection<TaskModel>();
            CreateTask = new DelegateCommand(OnCreateTask);
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            TaskModel sourceItem = dropInfo.Data as TaskModel;
            TaskModel targetItem = dropInfo.TargetItem as TaskModel;


            if (sourceItem != null && ((targetItem != null && sourceItem.CategoryId != targetItem.CategoryId) || targetItem == null))
            {
                System.Diagnostics.Debug.WriteLine("DragOver called with source:{0} and target:{1}", sourceItem, targetItem);
                //dropInfo.DestinationText = sourceItem.TaskName;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("DragOver None");
                dropInfo.Effects = DragDropEffects.None;
            }
        }

        async void IDropTarget.Drop(IDropInfo dropInfo)
        {
            TaskModel sourceItem = dropInfo.Data as TaskModel;
            TaskModel targetItem = dropInfo.TargetItem as TaskModel;

            await System.Threading.Tasks.Task.Run(() => service.MoveTaskToCategory(sourceItem, Category));
        }

        private async void OnCreateTask()
        {
            await System.Threading.Tasks.Task.Run(() => service.CreateTaskInCategory(this.Category));
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
            }
            else
            {
                Tasks.Clear();
            }
        }
    }
}
