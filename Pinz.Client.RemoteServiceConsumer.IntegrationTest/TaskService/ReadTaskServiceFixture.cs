using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Pinz.Client.RemoteServiceConsumer.TaskService
{
    [TestClass]
    public class ReadTaskServiceFixture
    {
        private IPinzAdminRemoteService pinzService;
        private IAdministrationRemoteService service;
        private ITaskRemoteService taskService;
        private IKernel kernel;
        private Category category;
        private Company company;
        private Project project;
        private User user;
        private IAuthorisationRemoteService authorisationService;
        private UserNameClientCredentials credentials;

        [TestInitialize]
        public void InitializeKernel()
        {
            kernel = new StandardKernel();
            kernel.Load(new ServiceConsumerNinjectModule());

            service = kernel.Get<IAdministrationRemoteService>();
            pinzService = kernel.Get<IPinzAdminRemoteService>();
            taskService = kernel.Get<ITaskRemoteService>();
            authorisationService = kernel.Get<IAuthorisationRemoteService>();

            credentials = kernel.Get<UserNameClientCredentials>();
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
            service.CreateProject(project);

            user = new User()
            {
                EMail = "me@gmail.com",
                IsCompanyAdmin = true,
                CompanyId = company.CompanyId
            };
            user = service.CreateUser(user);

            category = taskService.CreateCategoryInProject(project);
        }

        [TestCleanup]
        public void UnloadKernel()
        {
            credentials.UserName = "test@test.com";
            credentials.Password = "test";
            credentials.UpdateCredentialsForAllFactories();

            pinzService.DeleteCompany(company);

            kernel.Dispose();
        }

        [TestMethod]
        public void ReadAllTasksByCategory()
        {
            Assert.AreNotEqual(Guid.Empty, category.CategoryId);

            Task task = taskService.CreateTaskInCategory(category);
            Assert.IsNotNull(task.TaskId);

            List<Task> tasks = taskService.ReadAllTasksByCategory(category);

            Assert.AreEqual(1, tasks.Count());
        }

        [TestMethod]
        public void ReadAllCategoriesByProject()
        {
            List<Category> categories = taskService.ReadAllCategoriesByProject(project);

            Assert.AreEqual(1, categories.Count());
        }

        [TestMethod]
        public void ReadAllProjectsForUser()
        {
            Assert.AreNotEqual(Guid.Empty, category.CategoryId);
            Task task = taskService.CreateTaskInCategory(category);
            Assert.IsNotNull(task.TaskId);

            service.AddUserToProject(user, project, true);

            credentials.UserName = user.EMail;
            credentials.Password = "test";
            credentials.UpdateCredentialsForAllFactories();

            List<Project> projects = taskService.ReadAllProjectsForCurrentUser();

            Assert.AreEqual(1, projects.Count());
        }

        private Task createTask()
        {
            Task task = new Task()
            {
                TaskName = "TaskName",
                CreationTime = DateTime.Now,
                IsComplete = false,
                ActualWork = 0,
                Status = TaskStatus.TaskNotStarted,
                CategoryId = category.CategoryId
            };
            return task;
        }
    }
}
