using Com.Pinz.Client.Model.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.ObjectModel;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    [TestClass]
    public class PinzProjectsTabModelFixture
    {
        private PinzProjectsTabModel model;
        private Mock<ITaskClientService> taskService;

        [TestInitialize]
        public void SetUpFixture()
        {
            ObservableCollection<DomainModel.Project> projects = new ObservableCollection<DomainModel.Project>() {
                new DomainModel.Project { Name = "test" },
                new DomainModel.Project { Name = "test2" }
            };
            taskService = new Mock<ITaskClientService>();
            taskService.Setup(x => x.ReadAllProjectsForCurrentUser()).Returns(projects);
            taskService.Setup(x => x.Projects).Returns(projects);

            model = new PinzProjectsTabModel(taskService.Object);
        }

        [TestMethod]
        public void InitializationSetsValues()
        {
            Assert.AreEqual(model.Projects.Count, 2);
            taskService.Verify(m => m.Projects);
        }
    }
}
