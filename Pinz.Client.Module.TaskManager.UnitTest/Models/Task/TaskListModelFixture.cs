using Moq;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Pinz.Client.Model.Service;

namespace Com.Pinz.Client.Module.TaskManager.Models.Task
{
    [TestClass]
    public class TaskListModelFixture
    {
        private TaskListModel model;
        private Mock<ITaskClientService> taskService;

        [TestInitialize]
        public void SetUpFixture()
        {
            ObservableCollection<DomainModel.Task> tasks = new ObservableCollection<DomainModel.Task>() {
                new DomainModel.Task { TaskName = "test" },
                new DomainModel.Task { TaskName = "test2" }
            };
            taskService = new Mock<ITaskClientService>();
            taskService.Setup(x => x.ReadAllTasksByCategory(It.IsAny<DomainModel.Category>())).Returns(tasks);

            model = new TaskListModel(taskService.Object);
        }

        [TestMethod]
        public void InitializationSetsValues()
        {
            model.Category = new DomainModel.Category() { Name = "Test" };
            Assert.AreEqual(model.Tasks.Count, 2);
            taskService.Verify(m => m.ReadAllTasksByCategory(It.IsAny<DomainModel.Category>()));
        }

        [TestMethod]
        public void TasksNullOnNullCategory()
        {
            model.Category = new DomainModel.Category() { Name = "Test" };

            Assert.AreEqual(model.Tasks.Count, 2);

            model.Category = null;
            Assert.IsNull(model.Tasks);
        }
    }
}
