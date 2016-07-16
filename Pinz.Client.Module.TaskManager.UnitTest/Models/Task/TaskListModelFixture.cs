using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using System.Collections.Generic;
using Com.Pinz.Client.Module.TaskManager.Models.Category;
using Com.Pinz.Client.Commons.Model;
using Com.Pinz.Client.Model;
using Com.Pinz.Client.Module.TaskManager.Events;
using Prism.Events;

namespace Com.Pinz.Client.Module.TaskManager.Models.Task
{
    [TestClass]
    public class TaskListModelFixture
    {
        private TaskListModel model;
        private TaskFilter taskFilter;
        private Mock<ITaskRemoteService> taskService;

        [TestInitialize]
        public void SetUpFixture()
        {
            List<DomainModel.Task> tasks = new List<DomainModel.Task>() {
                new DomainModel.Task { TaskName = "test" },
                new DomainModel.Task { TaskName = "test2" }
            };
            taskService = new Mock<ITaskRemoteService>();
            taskService.Setup(x => x.ReadAllTasksByCategoryAsync(It.IsAny<DomainModel.Category>())).Returns(
                System.Threading.Tasks.Task.FromResult(tasks));
            taskFilter = new TaskFilter();
            Mock<ApplicationGlobalModel> applicationGlobalModel = new Mock<ApplicationGlobalModel>();

            var eventAgregator = new Mock<IEventAggregator>();
            var taskDeltedEvent = new Mock<TaskDeletedEvent>();
            eventAgregator.Setup(x => x.GetEvent<TaskDeletedEvent>()).Returns(taskDeltedEvent.Object);

            model = new TaskListModel(taskService.Object, taskFilter, applicationGlobalModel.Object, eventAgregator.Object);
        }

        [TestMethod]
        public void InitializationSetsValues()
        {
            model.Category = new CategoryModel { Name = "Test" };
            Assert.AreEqual(model.Tasks.Count, 2);
            taskService.Verify(m => m.ReadAllTasksByCategoryAsync(It.IsAny<DomainModel.Category>()));
        }

        [TestMethod]
        [Ignore]
        public void TasksNullOnNullCategory()
        {
            model.Category = new CategoryModel{ Name = "Test" };

            Assert.AreEqual(model.Tasks.Count, 2);

            model.Category = null;
            Assert.IsNull(model.Tasks);
        }
    }
}
