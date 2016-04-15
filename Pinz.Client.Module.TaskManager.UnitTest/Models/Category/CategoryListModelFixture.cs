using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Pinz.Client.Model.Service;
using System.Collections.ObjectModel;

namespace Com.Pinz.Client.Module.TaskManager.Models.Category
{
    [TestClass]
    public class CategoryListModelFixture
    {
        private CategoryListModel model;

        private Mock<ITaskClientService> taskService;

        [TestInitialize]
        public void SetUpFixture()
        {
            ObservableCollection<DomainModel.Category> categories = new ObservableCollection<DomainModel.Category>() {
                new DomainModel.Category { Name = "category 1" },
                new DomainModel.Category { Name = "category 2" }
            };
            taskService = new Mock<ITaskClientService>();
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
