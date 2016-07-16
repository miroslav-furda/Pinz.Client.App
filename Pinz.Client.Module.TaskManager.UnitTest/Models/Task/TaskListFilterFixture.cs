using Com.Pinz.Client.Commons.Model;
using Com.Pinz.Client.Module.TaskManager.Models;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using DomainModel = Com.Pinz.Client.DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Com.Pinz.Client.Model;
using System;
using System.Linq;
using Com.Pinz.DomainModel;
using Com.Pinz.Client.Module.TaskManager.Models.Category;
using Prism.Events;
using Com.Pinz.Client.Module.TaskManager.Events;

namespace Pinz.Client.Module.TaskManager.UnitTest.Models.Task
{
    [TestClass]
    public class TaskListFilterFixture
    {
        private TaskListModel model;
        private TaskFilter taskFilter;
        private Mock<ITaskRemoteService> taskService;
        private DomainModel.User currentUser;


        [TestInitialize]
        public void SetUpFixture()
        {
            currentUser = new DomainModel.User()
            {
                UserId = Guid.NewGuid(),
                EMail = "current@pinz.com"
            };

            List<DomainModel.Task> tasks = new List<DomainModel.Task>() {
                new DomainModel.Task { TaskId = Guid.NewGuid(), TaskName = "Not started task", Status = TaskStatus.TaskNotStarted, UserId = currentUser.UserId },
                new DomainModel.Task { TaskId = Guid.NewGuid(), TaskName = "Task completed", Status = TaskStatus.TaskComplete, DueDate = DateTime.Today, UserId = currentUser.UserId },
                new DomainModel.Task { TaskId = Guid.NewGuid(), TaskName = "Task due today", Status = TaskStatus.TaskNotStarted, DueDate = DateTime.Today, UserId = Guid.NewGuid() },
                new DomainModel.Task { TaskId = Guid.NewGuid(), TaskName = "In progress task" , Status = TaskStatus.TaskInProgress}
            };
            taskService = new Mock<ITaskRemoteService>();
            taskService.Setup(x => x.ReadAllTasksByCategoryAsync(It.IsAny<DomainModel.Category>())).Returns(
                System.Threading.Tasks.Task.FromResult(tasks));
            taskFilter = new TaskFilter();
            taskFilter.Complete = false;
            var applicationGlobalModel = new ApplicationGlobalModel()
            {
                CurrentUser = currentUser
            };

            var eventAgregator = new Mock<IEventAggregator>();
            var taskDeltedEvent = new Mock<TaskDeletedEvent>();
            eventAgregator.Setup(x => x.GetEvent<TaskDeletedEvent>()).Returns(taskDeltedEvent.Object);

            model = new TaskListModel(taskService.Object, taskFilter, applicationGlobalModel, eventAgregator.Object);
        }

        [TestMethod]
        public void ShowOnlyTasksOfCurrentUser()
        {
            taskFilter.MyTasks = true;
            model.Category = new CategoryModel() { Name = "Test" };
            Assert.IsTrue(model.Tasks.All(t => t.UserId == currentUser.UserId));
            Assert.IsFalse(model.Tasks.Any(t => t.Status == TaskStatus.TaskComplete));
            taskService.Verify(m => m.ReadAllTasksByCategoryAsync(It.IsAny<DomainModel.Category>()), Times.Once);
        }


        [TestMethod]
        public void ShowOnlyTasksInProgressAndNotStarted()
        {
            taskFilter.InProgress = true;
            taskFilter.NotStarted = true;
            model.Category = new CategoryModel() { Name = "Test" };
            Assert.IsTrue(model.Tasks.All(t => t.Status == TaskStatus.TaskInProgress || t.Status == TaskStatus.TaskNotStarted));
            taskService.Verify(m => m.ReadAllTasksByCategoryAsync(It.IsAny<DomainModel.Category>()), Times.Once);
        }

        [TestMethod]
        public void ShowOnlyTasksInProgress()
        {
            taskFilter.InProgress = true;
            model.Category = new CategoryModel() { Name = "Test" };
            Assert.IsTrue(model.Tasks.All(t => t.Status == TaskStatus.TaskInProgress));
            taskService.Verify(m => m.ReadAllTasksByCategoryAsync(It.IsAny<DomainModel.Category>()), Times.Once);
        }

        [TestMethod]
        public void ShowOnlyTasksNotStarted()
        {
            taskFilter.NotStarted = true;
            model.Category = new CategoryModel() { Name = "Test" };
            Assert.IsTrue(model.Tasks.All(t => t.Status == TaskStatus.TaskNotStarted));
            taskService.Verify(m => m.ReadAllTasksByCategoryAsync(It.IsAny<DomainModel.Category>()), Times.Once);
        }


        [TestMethod]
        public void ShowOnlyDueToday()
        {
            model.Category = new CategoryModel() { Name = "Test" };
            Assert.AreEqual(3, model.Tasks.Count);
            Assert.IsFalse(model.Tasks.Any(t => t.Status == TaskStatus.TaskComplete));
            taskService.Verify(m => m.ReadAllTasksByCategoryAsync(It.IsAny<DomainModel.Category>()), Times.Once);

            taskFilter.DueToday = true;
            Assert.AreEqual(1, model.Tasks.Count);
            Assert.IsFalse(model.Tasks.Any(t => t.Status == TaskStatus.TaskComplete));
            Assert.IsTrue(model.Tasks.All(t => t.DueDate == DateTime.Today));
            taskService.Verify(m => m.ReadAllTasksByCategoryAsync(It.IsAny<DomainModel.Category>()), Times.Once);
        }

        [TestMethod]
        public void ShowOnlyDueTodayPlusFinished()
        {
            model.Category = new CategoryModel() { Name = "Test" };
            Assert.AreEqual(3, model.Tasks.Count);
            Assert.IsFalse(model.Tasks.Any(t => t.Status == TaskStatus.TaskComplete));
            taskService.Verify(m => m.ReadAllTasksByCategoryAsync(It.IsAny<DomainModel.Category>()), Times.Once);

            taskFilter.DueToday = true;
            taskFilter.Complete = true;
            Assert.AreEqual(2, model.Tasks.Count);
            Assert.IsTrue(model.Tasks.All(t => t.DueDate == DateTime.Today));
            taskService.Verify(m => m.ReadAllTasksByCategoryAsync(It.IsAny<DomainModel.Category>()), Times.Once);
        }


        [TestMethod]
        public void DontShowCompleteTasks()
        {
            taskFilter.Complete = false;
            model.Category = new CategoryModel() { Name = "Test" };

            Assert.AreEqual(3, model.Tasks.Count);
            Assert.IsFalse(model.Tasks.Any(t => t.Status == TaskStatus.TaskComplete));
            taskService.Verify(m => m.ReadAllTasksByCategoryAsync(It.IsAny<DomainModel.Category>()));
        }

        [TestMethod]
        public void ShowCompleteTasks()
        {
            taskFilter.Complete = true;
            model.Category = new CategoryModel() { Name = "Test" };

            Assert.AreEqual(4, model.Tasks.Count);
            taskService.Verify(m => m.ReadAllTasksByCategoryAsync(It.IsAny<DomainModel.Category>()));
        }

        [TestMethod]
        public void ShowCompleteTasks2()
        {
            model.Category = new CategoryModel() { Name = "Test" };
            Assert.AreEqual(3, model.Tasks.Count);
            taskService.Verify(m => m.ReadAllTasksByCategoryAsync(It.IsAny<DomainModel.Category>()), Times.Once);

            taskFilter.Complete = true;
            Assert.AreEqual(4, model.Tasks.Count);
            taskService.Verify(m => m.ReadAllTasksByCategoryAsync(It.IsAny<DomainModel.Category>()), Times.Once);
        }
    }
}
