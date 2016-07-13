﻿using System.Collections.ObjectModel;
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
using Com.Pinz.Client.Model;

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

        private readonly ITaskRemoteService _service;
        private readonly TaskFilter _taskFilter;
        private List<DomainModel.Task> _allTasksFromServer;
        private DomainModel.User _currentUser;


        [Inject]
        public TaskListModel(ITaskRemoteService service, TaskFilter filter, ApplicationGlobalModel applicationGlobalModel)
        {
            this._service = service;
            this._taskFilter = filter;
            this.Tasks = new ObservableCollection<TaskModel>();
            CreateTask = new AwaitableDelegateCommand(OnCreateTask);
            this._taskFilter.PropertyChanged += Filter_PropertyChanged;
            _currentUser = applicationGlobalModel.CurrentUser;
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

            await _service.MoveTaskToCategoryAsync(sourceItem, Category);
        }

        #endregion

        private async System.Threading.Tasks.Task OnCreateTask()
        {
            await _service.CreateTaskInCategoryAsync(Category);
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
                    Tasks.Add(new TaskModel(task, _category));
            }
        }
    }
}