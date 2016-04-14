using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System;
using System.ServiceModel;

namespace Com.Pinz.Client.RemoteServiceConsumer.PinzAdmin
{

    [TestClass]
    public class CompanyServiceFixture
    {
        private IPinzAdminRemoteService pinzService;
        private IAdministrationRemoteService adminService;
        private IKernel kernel;
        private Company company;

        [TestInitialize]
        public void InitializeKernel()
        {
            kernel = new StandardKernel();
            kernel.Load(new ServiceConsumerNinjectModule());

            pinzService = kernel.Get<IPinzAdminRemoteService>();
            adminService = kernel.Get<IAdministrationRemoteService>();

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
        public void CreateCompany()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException<ExceptionDetail>), "Validation failed.")]
        public void CreateCompany_ValidationFailed()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            Company company2 = new Company();

            pinzService.CreateCompany(company2);
        }

        [TestMethod]
        public void UpdateCompany()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            company.Name = "New name";
            pinzService.UpdateCompany(company);

            Company company2 = adminService.ReadCompanyById(company.CompanyId);
            Assert.AreEqual(company.Name, company2.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException<ExceptionDetail>), "Validation failed.")]
        public void UpdateCompany_ValidationFailed()
        {
            Assert.AreNotEqual(Guid.Empty, company.CompanyId);

            company.Name = null;

            pinzService.UpdateCompany(company);
        }
    }
}
