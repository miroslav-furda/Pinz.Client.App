using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using System.Collections.Generic;
using Com.Pinz.Client.Module.TaskManager.Models.Category;

namespace Com.Pinz.Client.Module.TaskManager.Models.Task
{
    [TestClass]
    public class TaskListModelFixture
    {
        private TaskListModel model;
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

            model = new TaskListModel(taskService.Object);
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
