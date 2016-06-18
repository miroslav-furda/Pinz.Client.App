using AutoMapper;
using Com.Pinz.Client.Commons.Prism;
using Com.Pinz.Client.Commons.Wpf.Extensions;
using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.Model;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Ninject;
using Prism.Commands;
using Prism.Events;

namespace Com.Pinz.Client.Module.Administration.Model
{
    public class CompanyAdministrationModel : BindableValidationBase
    {
        public TabModel TabModel { get; private set; }
        private bool isEditorEnabled;
        private Company company;        
        private IAdministrationRemoteService adminService;        
        private IMapper mapper;
        private string originalCompanyName;

        public DelegateCommand StartEditCategory { get; private set; }
        public DelegateCommand CancelEditCategory { get; private set; }
        public DelegateCommand UpdateCategory { get; private set; }

        public bool IsEditorEnabled
        {
            get { return isEditorEnabled; }
            set { SetProperty(ref isEditorEnabled, value); }
        }

        public Company Company
        {
            get { return company; }
            set { SetProperty(ref company, value); }
        }

        [Inject]
        public CompanyAdministrationModel(IAdministrationRemoteService adminService, [Named("WpfClientMapper")] IMapper mapper)
        {
            IsEditorEnabled = false;
            TabModel = new TabModel()
            {
                Title = Properties.Resources.AdministrationTab_Title_Company,
                CanClose = false,
                IsModified = false
            };
            this.adminService = adminService;            
            this.mapper = mapper;

            StartEditCategory = new DelegateCommand(OnStartEditCategory);
            CancelEditCategory = new DelegateCommand(OnCancelEditCompany);
            UpdateCategory = new DelegateCommand(OnUpdateCompany);
        }

        private void OnUpdateCompany()
        {
            //adminService.ReadAllUsersForCompany()
            IsEditorEnabled = false;
        }

        private void OnCancelEditCompany()
        {
            Company.Name = originalCompanyName;
            IsEditorEnabled = false;
        }        

        private void OnStartEditCategory()
        {            
            originalCompanyName = Company.Name;
            IsEditorEnabled = true;
        }
    }
}
