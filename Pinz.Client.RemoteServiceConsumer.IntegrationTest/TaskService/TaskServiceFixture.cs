using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Com.Pinz.Client.RemoteServiceConsumer.TaskService
{
    [TestClass]
    public class TaskServiceFixture
    {
        private IPinzAdminRemoteService pinzService;
        private IAdministrationRemoteService service;
        private ITaskRemoteService taskService;
        private IKernel kernel;
        private Category category;
        private Company company;
        private IAuthorisationRemoteService authorisationService;

        [TestInitialize]
        public void InitializeKernel()
        {
            kernel = new StandardKernel();
            kernel.Load(new ServiceConsumerNinjectModule());

            service = kernel.Get<IAdministrationRemoteService>();
            pinzService = kernel.Get<IPinzAdminRemoteService>();
            taskService = kernel.Get<ITaskRemoteService>();
            authorisationService = kernel.Get<IAuthorisationRemoteService>();

            UserNameClientCredentials credentials = kernel.Get<UserNameClientCredentials>();
            credentials.UserName = "test@test.com";
            credentials.Password = "test";
            credentials.UpdateCredentialsForAllFactories();

            company = new Company()
            {
                Name = "Pinz Online"
            };
            company = pinzService.CreateCompany(company);

            Project project = new Project()
            {
                CompanyId = company.CompanyId,
                Name = "My test project",
                Description = "Descirption"
            };
            service.CreateProject(project);

            User user = new User()
            {
                EMail = "me@gmail.com",
                IsCompanyAdmin = true,
                CompanyId = company.CompanyId
            };
            user = service.CreateUser(user);

            service.AddUserToProject(user, project, true);

            category = taskService.CreateCategoryInProject(project);
        }

        [TestCleanup]
        public void UnloadKernel()
        {
            pinzService.DeleteCompany(company);

            kernel.Dispose();
        }

        [TestMethod]
        public void CreateTask()
        {
            Assert.AreNotEqual(Guid.Empty, category.CategoryId);

            Task task = taskService.CreateTaskInCategory(category);

            Assert.IsNotNull(task.TaskId);
        }

        [TestMethod]
        public void UpdateTask()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);
            Task task = taskService.CreateTaskInCategory(category);
            Assert.IsNotNull(task.TaskId);

            task.TaskName = "New name";
            taskService.UpdateTask(task);

            List<Task> tasks = taskService.ReadAllTasksByCategory(category);
            Assert.AreEqual(1, tasks.Count());
            Assert.AreEqual(task.TaskName, tasks[0].TaskName);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException<ExceptionDetail>), "Validation failed.")]
        public void UpdateTask_ValidationFailed()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);
            Task task = taskService.CreateTaskInCategory(category);
            Assert.IsNotNull(task.TaskId);

            task.TaskName = null;
            taskService.UpdateTask(task);
        }

        [TestMethod]
        public void DeleteTask()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);
            Task task = taskService.CreateTaskInCategory(category);
            Assert.IsNotNull(task.TaskId);

            taskService.DeleteTask(task);

            List<Task> tasks = taskService.ReadAllTasksByCategory(category);
            Assert.AreEqual(0, tasks.Count());
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
