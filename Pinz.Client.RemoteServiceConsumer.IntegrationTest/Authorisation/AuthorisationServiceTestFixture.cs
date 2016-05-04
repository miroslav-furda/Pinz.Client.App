using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System;

namespace Com.Pinz.Client.RemoteServiceConsumer.Authorisation
{
    [TestClass]
    class AuthorisationServiceTestFixture
    {
        private IPinzAdminRemoteService pinzService;
        private IAdministrationRemoteService service;
        private IAuthorisationRemoteService authorisationService;
        private IKernel kernel;
        private Company company;
        private Project project;
        private User user;

        [TestInitialize]
        public void InitializeKernel()
        {
            kernel = new StandardKernel();
            kernel.Load(new ServiceConsumerNinjectModule());

            service = kernel.Get<IAdministrationRemoteService>();
            pinzService = kernel.Get<IPinzAdminRemoteService>();
            authorisationService = kernel.Get<IAuthorisationRemoteService>();

            UserNameClientCredentials credentials = kernel.Get<UserNameClientCredentials>();
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
        }

        [TestCleanup]
        public void UnloadKernel()
        {
            pinzService.DeleteCompany(company);

            kernel.Dispose();
        }

        [TestMethod]
        public void IsUserCompanyAdmin()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            bool isCompanyAdmin = authorisationService.IsUserComapnyAdmin(user);

            Assert.IsTrue(isCompanyAdmin);
        }

        [TestMethod]
        public async void ReadUserByEmail()
        {
            User rUser = await authorisationService.ReadUserByEmail(user.EMail);

            Assert.AreEqual(user.UserId, rUser.UserId);
        }

        [TestMethod]
        public void IsUserProjectAdmin_False()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            bool isCompanyAdmin = authorisationService.IsUserProjectAdmin(user, project);

            Assert.IsFalse(isCompanyAdmin);
        }

        [TestMethod]
        public void IsUserProjectAdmin_True()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);
            service.AddUserToProject(user, project, true);

            bool isCompanyAdmin = authorisationService.IsUserProjectAdmin(user, project);

            Assert.IsTrue(isCompanyAdmin);
        }
    }
}
