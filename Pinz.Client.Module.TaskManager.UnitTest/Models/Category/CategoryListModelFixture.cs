using System;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using System.Collections.Generic;
using Com.Pinz.Client.DomainModel;

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
            taskService.Setup(x => x.ReadAllCategoriesByProject(It.IsAny<DomainModel.Project>())).Returns(categories);

            adminService = new Mock<IAdministrationRemoteService>();
            adminService.Setup(x => x.ReadAllUsersByProject(It.IsAny<DomainModel.Project>())).Returns(users);

            model = new CategoryListModel(taskService.Object, adminService.Object);
        }

        [TestMethod]
        public void InitializationSetsValues()
        {
            model.Project = new ProjectModel { Name = "project" };

            Assert.AreEqual(model.Categories.Count, 2);
            taskService.Verify(m => m.ReadAllCategoriesByProject(model.Project));
        }

        [TestMethod]
        public void CallServiceOnCreateCategory()
        {
            model.Project = new ProjectModel { Name = "project" };

            model.CreateCategory.Execute();

            taskService.Verify(m => m.CreateCategoryInProject(model.Project));
        }
    }
}
