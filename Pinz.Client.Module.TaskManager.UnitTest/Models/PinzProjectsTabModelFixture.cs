using Com.Pinz.Client.Model;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using System.Collections.Generic;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    [TestClass]
    public class PinzProjectsTabModelFixture
    {
        private PinzProjectsTabModel model;
        private Mock<ITaskRemoteService> taskService;

        [TestInitialize]
        public void SetUpFixture()
        {
            List<DomainModel.Project> projects = new List<DomainModel.Project>() {
                new DomainModel.Project { Name = "test" },
                new DomainModel.Project { Name = "test2" }
            };
            taskService = new Mock<ITaskRemoteService>();
            taskService.Setup(x => x.ReadAllProjectsForCurrentUserAsync()).Returns(
                System.Threading.Tasks.Task.FromResult(projects));
            Mock<ApplicationGlobalModel> globalModel = new Mock<ApplicationGlobalModel>();

            model = new PinzProjectsTabModel(taskService.Object, globalModel.Object, new Mock<EventAggregator>().Object);
        }

        [TestMethod]
        [Ignore]
        public void InitializationSetsValues()
        {
            Assert.AreEqual(model.Projects.Count, 2);
            taskService.Verify(m => m.ReadAllProjectsForCurrentUserAsync());
        }
    }
}
