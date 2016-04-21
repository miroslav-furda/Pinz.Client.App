using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.Model.Service;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.DomainModel;
using Ninject;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System;

namespace Com.Pinz.Client.Model.Remote
{
    public class TaskClientServiceRemote : ITaskClientService
    {
        public ObservableCollection<Project> Projects { get; private set; }

        private object _projectsLock = new object();
        private Dictionary<Project, ObservableCollection<Category>> categoriesMap;
        private Dictionary<Category, ObservableCollection<Task>> tasksMap;

        private ITaskRemoteService taskRemoteService;
        private TaskFilter filter;

        [Inject]
        public TaskClientServiceRemote(ITaskRemoteService taskRemoteService, TaskFilter filter)
        {
            this.taskRemoteService = taskRemoteService;
            this.filter = filter;

            Projects = new ObservableCollection<Project>();
            BindingOperations.EnableCollectionSynchronization(Projects, _projectsLock);
            categoriesMap = new Dictionary<Project, ObservableCollection<Category>>();
            tasksMap = new Dictionary<Category, ObservableCollection<Task>>();

            filter.PropertyChanged += TaskFilter_PropertyChanged;
        }


        #region Read

        public ObservableCollection<Project> ReadAllProjectsForCurrentUser()
        {
            List<Project> projectList = taskRemoteService.ReadAllProjectsForCurrentUser();
            Projects.Clear();
            projectList.ForEach(Projects.Add);
            return Projects;
        }

        public ObservableCollection<Category> ReadAllCategoriesByProject(Project project)
        {
            ObservableCollection<Category> categories = loadObservableCollection(project, categoriesMap);
            categories.Clear();
            List<Category> catList = taskRemoteService.ReadAllCategoriesByProject(project);
            catList.ForEach(categories.Add);

            return categories;
        }

        public ObservableCollection<Task> ReadAllTasksByCategory(Category category)
        {
            ObservableCollection<Task> tasks = loadObservableCollection(category, tasksMap);
            tasks.Clear();
            List<Task> taskList = taskRemoteService.ReadAllTasksByCategory(category);

            taskList.ForEach(item =>
            {
                if (FilterTasks(item))
                    tasks.Add(item);
            });

            return tasks;
        }
        #endregion

        public void MoveTaskToCategory(Task task, Category category)
        {
            Category originalCategory = tasksMap.Keys.Where(c => c.CategoryId == task.CategoryId).Single();
            taskRemoteService.MoveTaskToCategory(task, category);
            loadObservableCollection(originalCategory, tasksMap).Remove(task);
            loadObservableCollection(category, tasksMap).Add(task);
        }

        public void ChangeTaskStatus(Task task, TaskStatus newStatus)
        {
            taskRemoteService.ChangeTaskStatus(task, newStatus);
        }

        #region Task CUD
        public Task CreateTaskInCategory(Category category)
        {
            Task createdTask;
            createdTask = taskRemoteService.CreateTaskInCategory(category);
            loadObservableCollection(category, tasksMap).Add(createdTask);
            return createdTask;
        }

        public void UpdateTask(Task task)
        {
            taskRemoteService.UpdateTask(task);
        }

        public void DeleteTask(Task task)
        {
            taskRemoteService.DeleteTask(task);
            Category category = tasksMap.Keys.Where(c => c.CategoryId == task.CategoryId).Single();
            loadObservableCollection(category, tasksMap).Remove(task);
        }
        #endregion

        #region Category CUD
        public Category CreateCategoryInProject(Project project)
        {
            Category createdCategory;
            createdCategory = taskRemoteService.CreateCategoryInProject(project);
            loadObservableCollection(project, categoriesMap).Add(createdCategory);
            return createdCategory;
        }

        public void UpdateCategory(Category category)
        {
            taskRemoteService.UpdateCategory(category);
        }

        public void DeleteCategory(Category category)
        {
            taskRemoteService.DeleteCategory(category);
            Project project = categoriesMap.Keys.Where(p => p.ProjectId == category.ProjectId).Single();
            loadObservableCollection(project, categoriesMap).Remove(category);
        }
        #endregion

        #region Listeners
        private void TaskOutlookService_TaskRemove(Task task, Category category)
        {
            ObservableCollection<Task> tasks = loadObservableCollection(category, tasksMap);
            if (tasks.Contains(task))
                tasks.Remove(task);
        }

        private void TaskOutlookService_TaskAdd(Task task, Category category)
        {
            ObservableCollection<Task> tasks = loadObservableCollection(category, tasksMap);
            if (!tasks.Contains(task))
                tasks.Add(task);
        }
        #endregion


        #region private Filter

        private void TaskFilter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            foreach (Category category in tasksMap.Keys)
            {
                ReadAllTasksByCategory(category);
            }
        }

        private bool FilterTasks(Task taskitem)
        {
            bool retval = true;

            if (!filter.Complete)
            {
                retval = taskitem.IsComplete.Equals(false);
            }

            if (retval && filter.DueToday)
            {
                System.DateTime today = System.DateTime.Today;
                retval = taskitem.DueDate.Equals(today);
            }

            if (retval && filter.InProgress)
            {
                retval = taskitem.Status.Equals(TaskStatus.TaskInProgress);
                if (filter.NotStarted)
                {
                    retval = taskitem.Status.Equals(TaskStatus.TaskNotStarted);
                }
            }
            return retval;
        }
        #endregion



        private ObservableCollection<T> loadObservableCollection<K, T>(K key, Dictionary<K, ObservableCollection<T>> dictionary)
               where T : class
               where K : class
        {
            ObservableCollection<T> observableCollection;
            if (dictionary.ContainsKey(key))
            {
                observableCollection = dictionary[key];
            }
            else
            {
                observableCollection = new ObservableCollection<T>();
                dictionary.Add(key, observableCollection);
            }

            return observableCollection;
        }

  
    }
}
