using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.Model.Service;
using GongSolutions.Wpf.DragDrop;
using Ninject;
using Prism.Commands;
using Prism.Mvvm;
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
                if (value != null)
                    Tasks = service.ReadAllTasksByCategory(Category);
                else
                    Tasks = null;
            }
        }

        private ITaskClientService service;

        [Inject]
        public TaskListModel(ITaskClientService service)
        {
            this.service = service;
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

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            Task sourceItem = dropInfo.Data as Task;
            Task targetItem = dropInfo.TargetItem as Task;

            service.MoveTaskToCategory(sourceItem, Category);
        }

        private void OnCreateTask()
        {
            service.CreateTaskInCategory(this.Category);
        }
    }
}
