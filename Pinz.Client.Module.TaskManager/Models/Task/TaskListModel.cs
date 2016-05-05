using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using GongSolutions.Wpf.DragDrop;
using Ninject;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class TaskListModel : BindableBase, IDropTarget
    {
        public DelegateCommand CreateTask { get; private set; }

        private ObservableCollection<Task> _tasks;
        public ObservableCollection<Task> Tasks
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


        private Category _category;
        public Category Category
        {
            get
            {
                return _category;
            }
            set
            {
                SetProperty(ref this._category, value);
                LoadTasks();
            }
        }

        private ITaskRemoteService service;

        [Inject]
        public TaskListModel(ITaskRemoteService service)
        {
            this.service = service;
            Tasks = new ObservableCollection<Task>();
            CreateTask = new DelegateCommand(OnCreateTask);
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            Task sourceItem = dropInfo.Data as Task;
            Task targetItem = dropInfo.TargetItem as Task;


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
            Task sourceItem = dropInfo.Data as Task;
            Task targetItem = dropInfo.TargetItem as Task;

            await System.Threading.Tasks.Task.Run(() => service.MoveTaskToCategory(sourceItem, Category));
        }

        private async void OnCreateTask()
        {
            await System.Threading.Tasks.Task.Run(() => service.CreateTaskInCategory(this.Category));
        }

        private async void LoadTasks()
        {
            Tasks.Clear();
            if (Category != null)
            {
                List<Task> tasks = await System.Threading.Tasks.Task.Run(() => service.ReadAllTasksByCategory(Category));
                tasks.ForEach(Tasks.Add);
            }
        }
    }
}
