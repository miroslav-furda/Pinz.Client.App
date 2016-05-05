using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using System.Collections.Generic;

namespace Com.Pinz.Client.Module.TaskManager.Models.Category
{
    [TestClass]
    public class CategoryListModelFixture
    {
        private CategoryListModel model;

        private Mock<ITaskRemoteService> taskService;

        [TestInitialize]
        public void SetUpFixture()
        {
            List<DomainModel.Category> categories = new List<DomainModel.Category>() {
                new DomainModel.Category { Name = "category 1" },
                new DomainModel.Category { Name = "category 2" }
            };
            taskService = new Mock<ITaskRemoteService>();
            taskService.Setup(x => x.ReadAllCategoriesByProject(It.IsAny<DomainModel.Project>())).Returns(categories);

            model = new CategoryListModel(taskService.Object);
        }

        [TestMethod]
        public void InitializationSetsValues()
        {
            model.Project = new DomainModel.Project() { Name = "project" };

            Assert.AreEqual(model.Categories.Count, 2);
            taskService.Verify(m => m.ReadAllCategoriesByProject(model.Project));
        }

        [TestMethod]
        public void CallServiceOnCreateCategory()
        {
            model.Project = new DomainModel.Project() { Name = "project" };

            model.CreateCategory.Execute();

            taskService.Verify(m => m.CreateCategoryInProject(model.Project));
        }
    }
}
