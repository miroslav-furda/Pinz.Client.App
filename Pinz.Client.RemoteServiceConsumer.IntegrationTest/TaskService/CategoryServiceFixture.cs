using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Com.Pinz.Client.RemoteServiceConsumer.TaskService
{
    [TestClass]
    public class CategoryServiceFixture
    {
        private IPinzAdminRemoteService pinzService;
        private IAdministrationRemoteService service;
        private IAuthorisationRemoteService authorisationService;
        private ITaskRemoteService taskService;
        private IKernel kernel;
        private Company company;
        private Project project;

        [TestInitialize]
        public void InitializeKernel()
        {
            kernel = new StandardKernel();
            kernel.Load(new ServiceConsumerNinjectModule());

            service = kernel.Get<IAdministrationRemoteService>();
            pinzService = kernel.Get<IPinzAdminRemoteService>();
            taskService = kernel.Get<ITaskRemoteService>();
            authorisationService = kernel.Get<IAuthorisationRemoteService>();

            pinzService = kernel.Get<IPinzAdminRemoteService>();
            UserNameClientCredentials credentials = kernel.Get<UserNameClientCredentials>();
            credentials.UserName = "test@test.com";
            credentials.Password = "test";
            credentials.UpdateCredentialsForAllFactories();

            company = new Company()
            {
                Name = "Pinz Online"
            };
            company = pinzService.CreateCompany(company);

            project = new Project()
            {
                CompanyId = company.CompanyId,
                Name = "My test project",
                Description = "Descirption"
            };
            project = service.CreateProject(project);

            User user = new User()
            {
                EMail = "me@gmail.com",
                FirstName = "Blaha",
                FamilyName = "Boo",
                IsCompanyAdmin = true,
                CompanyId = company.CompanyId
            };
            user = service.CreateUser(user);

            service.AddUserToProject(user, project, true);
        }

        [TestCleanup]
        public void UnloadKernel()
        {
            pinzService.DeleteCompany(company);

            kernel.Dispose();
        }

        [TestMethod]
        public void CreateCategory()
        {
            Assert.AreNotEqual(Guid.Empty, project.ProjectId);

            Category category = taskService.CreateCategoryInProject(project);

            Assert.IsNotNull(category.CategoryId);
            Assert.IsNotNull(category.ProjectId);
            Assert.IsNotNull(category.Name);
        }

        [TestMethod]
        public void UpdateCategory()
        {
            Assert.AreNotEqual(Guid.Empty, project.ProjectId);

            Category category =  taskService.CreateCategoryInProject(project);
            Assert.IsNotNull(category.CategoryId);


            category.Name = "New name";
            taskService.UpdateCategory(category);

            List<Category> categories = taskService.ReadAllCategoriesByProject(project);
            Assert.AreEqual(1, categories.Count());
            Assert.AreEqual(category.Name, categories[0].Name);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException<ExceptionDetail>), "Validation failed.")]
        public void UpdateCategory_ValidationFailed()
        {
            Assert.AreNotEqual(Guid.Empty, project.ProjectId);

            Category category = taskService.CreateCategoryInProject(project);
            Assert.IsNotNull(category.CategoryId);

            category.Name = null;
            taskService.UpdateCategory(category);
        }

        [TestMethod]
        public void DeleteCategory()
        {
            Assert.AreNotEqual(Guid.Empty, project.ProjectId);

            Category category =  taskService.CreateCategoryInProject(project);
            Assert.IsNotNull(category.CategoryId);

            taskService.DeleteCategory(category);

            List<Category> categories = taskService.ReadAllCategoriesByProject(project);
            Assert.AreEqual(0, categories.Count());
        }
    }
}
