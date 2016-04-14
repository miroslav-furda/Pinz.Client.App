using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Pinz.Client.RemoteServiceConsumer.Administration
{
    [TestClass]
    public class AddRemoveUserServiceFixture
    {
        private IPinzAdminRemoteService pinzService;
        private IAdministrationRemoteService service;
        private ITaskRemoteService taskService;
        private IKernel kernel;
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

            Company company1 = new Company()
            {
                Name = "Pinz Online"
            };
            company = pinzService.CreateCompany(company1);

            project = new Project()
            {
                CompanyId = company.CompanyId,
                Name = "My test project",
                Description = "Description"
            };
            service.CreateProject(project);

            user = new User()
            {
                EMail = "me@gmail.com",
                IsCompanyAdmin = true,
                CompanyId = company.CompanyId
            };
            user = service.CreateUser(user);
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
        public void AddUser()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            service.AddUserToProject(user, project, true);

            credentials.UserName = user.EMail;
            credentials.Password = "test";
            credentials.UpdateCredentialsForAllFactories();
            List<Project> projects = taskService.ReadAllProjectsForCurrentUser();
            Assert.AreEqual(1, projects.Count());
            Assert.AreEqual(project.ProjectId, projects[0].ProjectId);
        }

        [TestMethod]
        public void RemoveUser()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            service.AddUserToProject(user, project, true);

            credentials.UserName = user.EMail;
            credentials.Password = "test";
            credentials.UpdateCredentialsForAllFactories();

            List<Project> projects = taskService.ReadAllProjectsForCurrentUser();
            Assert.AreEqual(1, projects.Count());
            Assert.AreEqual(project.ProjectId, projects[0].ProjectId);

            service.RemoveUserFromProject(user, project);

            List<Project> projects2 = taskService.ReadAllProjectsForCurrentUser();
            Assert.AreEqual(0, projects2.Count());

        }
    }
}
