using System;
using System.Collections.Generic;
using AutoMapper;
using Ninject;
using System.ServiceModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Properties;
using Com.Pinz.DomainModel;

namespace Com.Pinz.Client.RemoteServiceConsumer.ServiceImpl
{
    internal class TaskService : ServiceBase, ITaskRemoteService
    {
        private IMapper mapper;

        private ChannelFactory<TaskServiceReference.ITaskService> clientFactory;
        private TaskServiceReference.ITaskService channel;
        private UserNameClientCredentials clientCredentials;

        [Inject]
        public TaskService([Named("ServiceConsumerMapper")] IMapper mapper, ChannelFactory<TaskServiceReference.ITaskService> clientFactory, UserNameClientCredentials clientCredentials)
        {
            this.mapper = mapper;
            this.clientFactory = clientFactory;
            this.clientCredentials = clientCredentials;
        }

        public List<Project> ReadAllProjectsForCurrentUser()
        {
            List<TaskServiceReference.ProjectDO> rProjects = channel.ReadAllProjectsForUserEmail(clientCredentials.UserName);
            List<Project> projectList = mapper.Map<List<TaskServiceReference.ProjectDO>, List<Project>>(rProjects);
            return projectList;
        }

        public List<Category> ReadAllCategoriesByProject(Project project)
        {
            List<TaskServiceReference.CategoryDO> rCategories = channel.ReadAllCategoriesByProjectId(project.ProjectId);
            List<Category> categoryList = mapper.Map<List<TaskServiceReference.CategoryDO>, List<Category>>(rCategories);
            return categoryList;
        }

        public List<Task> ReadAllTasksByCategory(Category category)
        {
            List<TaskServiceReference.TaskDO> rTasks = channel.ReadAllTasksByCategoryId(category.CategoryId);
            List<Task> taskList = mapper.Map<List<TaskServiceReference.TaskDO>, List<Task>>(rTasks);
            return taskList;
        }

        public void MoveTaskToCategory(Task task, Category category)
        {
            task.CategoryId = category.CategoryId;
            UpdateTask(task);
        }

        public void ChangeTaskStatus(Task task, TaskStatus newStatus)
        {
            switch (newStatus)
            {
                case TaskStatus.TaskInProgress:
                    task.Status = TaskStatus.TaskInProgress;
                    task.StartDate = DateTime.Today;
                    task.DueDate = DateTime.Today;
                    task.DateCompleted = null;
                    break;
                case TaskStatus.TaskComplete:
                    task.Status = TaskStatus.TaskComplete;
                    task.DateCompleted = DateTime.Today;
                    break;
                case TaskStatus.TaskNotStarted:
                    task.Status = TaskStatus.TaskNotStarted;
                    task.StartDate = null;
                    task.DueDate = null;
                    task.DateCompleted = null;
                    break;
                default:
                    task.Status = newStatus;
                    break;
            }
            UpdateTask(task);
        }

       

        #region Category CUD
        public Category CreateCategoryInProject(Project project)
        {
            Category category = new Category()
            {
                ProjectId = project.ProjectId,
                Name = Resources.TaskService_NewCategoryName
            };
            TaskServiceReference.CategoryDO rCategory = channel.CreateCategory(mapper.Map<TaskServiceReference.CategoryDO>(category));
            mapper.Map(rCategory, category);
            return category;
        }

        public void DeleteCategory(Category category)
        {
            channel.DeleteCategory(mapper.Map<TaskServiceReference.CategoryDO>(category));
        }

        public void UpdateCategory(Category category)
        {
            TaskServiceReference.CategoryDO rCategory = channel.UpdateCategory(mapper.Map<TaskServiceReference.CategoryDO>(category));
            mapper.Map(rCategory, category);
        }
        #endregion

        #region Task CUD
        public Task CreateTaskInCategory(Category category)
        {
            Task task = new Task()
            {
                CategoryId = category.CategoryId,
                TaskName = Resources.TaskService_NewTaskName,
                IsComplete = false,
                CreationTime = DateTime.Now,
                ActualWork = 0,
                Status = TaskStatus.TaskNotStarted
            };
            TaskServiceReference.TaskDO rTask = channel.CreateTask(mapper.Map<TaskServiceReference.TaskDO>(task), clientCredentials.UserName);
            mapper.Map(rTask, task);
            return task;
        }

        public void DeleteTask(Task task)
        {
            channel.DeleteTask(mapper.Map<TaskServiceReference.TaskDO>(task));
        }

        public void UpdateTask(Task task)
        {
            TaskServiceReference.TaskDO rTask = channel.UpdateTask(mapper.Map<TaskServiceReference.TaskDO>(task));
            mapper.Map(rTask, task);
        }


        #endregion

        public override void OpenChannel()
        {
            channel = clientFactory.CreateChannel();
        }

        public override void CloseChannel()
        {
            CloseOrAbortServiceChannel(channel as ICommunicationObject);
        }
    }
}
