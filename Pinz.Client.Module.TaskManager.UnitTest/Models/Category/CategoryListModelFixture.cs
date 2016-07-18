using System;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.Client.DomainModel;
using Prism.Events;

namespace Com.Pinz.Client.Module.TaskManager.Models.Category
{
    [TestClass]
    public class CategoryListModelFixture
    {
        private CategoryListModel model;

        private Mock<ITaskRemoteService> taskService;
        private Mock<IAdministrationRemoteService> adminService;

        [TestInitialize]
        public void SetUpFixture()
        {
            List<DomainModel.Category> categories = new List<DomainModel.Category>() {
                new DomainModel.Category { Name = "category 1" },
                new DomainModel.Category { Name = "category 2" }
            };

            List<DomainModel.User> users = new List<User>
            {
                new User {UserId = Guid.Empty, EMail = "test@test.sk"}
            };
                
            taskService = new Mock<ITaskRemoteService>();
            taskService.Setup(x => x.ReadAllCategoriesByProjectAsync(It.IsAny<DomainModel.Project>())).Returns(
                System.Threading.Tasks.Task.FromResult(categories));

            adminService = new Mock<IAdministrationRemoteService>();
            adminService.Setup(x => x.ReadAllUsersByProjectAsync(It.IsAny<DomainModel.Project>())).Returns(
                System.Threading.Tasks.Task.FromResult(users));

            model = new CategoryListModel(taskService.Object, adminService.Object, new Mock<EventAggregator>().Object);
        }

        [TestMethod]
        public void InitializationSetsValues()
        {
            model.Project = new ProjectModel { Name = "project" };

            Assert.AreEqual(model.Categories.Count, 2);
            taskService.Verify(m => m.ReadAllCategoriesByProjectAsync(model.Project));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task CallServiceOnCreateCategory()
        {
            model.Project = new ProjectModel { Name = "project" };

            await model.CreateCategory.ExecuteAsync(this);

            taskService.Verify(m => m.CreateCategoryInProjectAsync(model.Project));
        }
    }
}
