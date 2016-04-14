using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Com.Pinz.Client.RemoteServiceConsumer.Administration
{
    [TestClass]
    public class ProjectServiceFixture
    {
        private IPinzAdminRemoteService pinzService;
        private IAdministrationRemoteService service;
        private IKernel kernel;
        private Company company;

        [TestInitialize]
        public void InitializeKernel()
        {
            kernel = new StandardKernel();
            kernel.Load(new ServiceConsumerNinjectModule());

            service = kernel.Get<IAdministrationRemoteService>();
            pinzService = kernel.Get<IPinzAdminRemoteService>();

            UserNameClientCredentials credentials = kernel.Get<UserNameClientCredentials>();
            credentials.UserName = "test@test.com";
            credentials.Password = "test";
            credentials.UpdateCredentialsForAllFactories();

            Company company1 = new Company()
            {
                Name = "Pinz Online"
            };
            company = pinzService.CreateCompany(company1);
        }

        [TestCleanup]
        public void UnloadKernel()
        {
            pinzService.DeleteCompany(company);

            kernel.Dispose();
        }

        [TestMethod]
        public void CreateProject()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            Project project = new Project();
            project.CompanyId = company.CompanyId;
            project.Name = "My test project";
            project.Description = "Descirption";

            service.CreateProject(project);

            Assert.IsNotNull(project.ProjectId);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException<ExceptionDetail>), "Validation failed.")]
        public void CreateProject_ValidationFailed()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            Project project = new Project();
            project.CompanyId = company.CompanyId;
            project.Description = "Descirption";

            service.CreateProject(project);
        }

        [TestMethod]
        public void UpdateProject()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            Project project = new Project();
            project.CompanyId = company.CompanyId;
            project.Name = "My test project";
            project.Description = "Description";

            service.CreateProject(project);
            Assert.IsNotNull(project.ProjectId);

            project.Name = "New name";
            service.UpdateProject(project);

            List<Project> projects = service.ReadProjectsForCompany(company);
            Assert.AreEqual(1, projects.Count());
            Assert.AreEqual(project.Name, projects[0].Name);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException<ExceptionDetail>), "Validation failed.")]
        public void UpdateProject_ValidationFailed()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            Project project = new Project();
            project.CompanyId = company.CompanyId;
            project.Name = "My test project";
            project.Description = "Description";

            service.CreateProject(project);
            Assert.IsNotNull(project.ProjectId);

            project.Name = null;
            service.UpdateProject(project);
        }

        [TestMethod]
        public void DeleteProject()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            Project project = new Project();
            project.CompanyId = company.CompanyId;
            project.Name = "My test project";
            project.Description = "Description";

            service.CreateProject(project);
            Assert.IsNotNull(project.ProjectId);

            service.DeleteProject(project);

            List<Project> projects = service.ReadProjectsForCompany(company);
            Assert.AreEqual(0, projects.Count());
        }
    }
}
