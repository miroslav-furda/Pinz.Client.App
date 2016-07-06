using System;
using System.Collections.ObjectModel;
using Com.Pinz.Client.Commons.Prism;
using Com.Pinz.Client.Commons.Wpf.Extensions;
using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.Model;
using Com.Pinz.Client.Module.Administration.Properties;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Ninject;
using Prism.Commands;
using Task = System.Threading.Tasks.Task;

namespace Com.Pinz.Client.Module.Administration.Model
{
    public class CompanyAdministrationModel : BindableValidationBase
    {
        private readonly IAdministrationRemoteService adminService;
        private readonly IPinzAdminRemoteService pinzAdminService;
        private readonly ApplicationGlobalModel globalModel;

        private Company company;

        private bool isCompanyEditorEnabled;

        private bool isCompanyEditorVisible;

        private bool isNewProjectEnabled = true;
        private bool isProjectEditorEnabled;
        private bool isProjectEditorVisible;
        private bool isUserEditorEnabled;
        private bool isUserEditorVisible;

        private string originalCompanyName;
        private Project selectedProject;
        private User selectedUser;

        [Inject]
        public CompanyAdministrationModel(IAdministrationRemoteService adminService, IPinzAdminRemoteService pinzAdminService, ApplicationGlobalModel globalModel)
        {
            this.globalModel = globalModel;
            TabModel = new TabModel
            {
                Title = Resources.AdministrationTab_Title_Company,
                CanClose = false,
                IsModified = false
            };
            this.adminService = adminService;
            this.pinzAdminService = pinzAdminService;

            StartEditCompany = new DelegateCommand(OnStartEditCompany);
            CancelEditCompany = new DelegateCommand(OnCancelEditCompany);
            UpdateCompany = new AwaitableDelegateCommand(OnUpdateCompany);

            NewProject = new DelegateCommand(OnNewProject);
            StartEditProject = new DelegateCommand(OnEditProject);
            DeleteProject = new AwaitableDelegateCommand(OnDeleteProject);
            UpdateProject = new AwaitableDelegateCommand(OnSaveProject);
            CancelEditProject = new DelegateCommand(OnCancelEditProject);

            StartEditUser = new DelegateCommand(OnEditUser);
            DeleteUser = new AwaitableDelegateCommand(OnDeleteUser);
            UpdateUser = new AwaitableDelegateCommand(OnUpdateUser);
            CancelEditUser = new DelegateCommand(OnCancelEditUser);

            Projects = new ObservableCollection<Project>();
            Users = new ObservableCollection<User>();

            LoadCompany();
        }

        public TabModel TabModel { get; private set; }


        public DelegateCommand StartEditCompany { get; private set; }
        public DelegateCommand CancelEditCompany { get; private set; }
        public AwaitableDelegateCommand UpdateCompany { get; private set; }

        public DelegateCommand NewProject { get; private set; }
        public DelegateCommand StartEditProject { get; private set; }
        public DelegateCommand CancelEditProject { get; private set; }
        public AwaitableDelegateCommand UpdateProject { get; private set; }
        public AwaitableDelegateCommand DeleteProject { get; private set; }

        public DelegateCommand StartEditUser { get; private set; }
        public DelegateCommand CancelEditUser { get; private set; }
        public AwaitableDelegateCommand UpdateUser { get; private set; }
        public AwaitableDelegateCommand DeleteUser { get; private set; }

        public bool IsCompanyEditorVisible
        {
            get { return isCompanyEditorVisible; }
            set { SetProperty(ref isCompanyEditorVisible, value); }
        }

        public bool IsProjectEditorVisible
        {
            get { return isProjectEditorVisible; }
            set
            {
                SetProperty(ref isProjectEditorVisible, value);
                IsNewProjectEnabled = !IsNewProjectEnabled;
            }
        }

        public bool IsUserEditorVisible
        {
            get { return isUserEditorVisible; }
            set { SetProperty(ref isUserEditorVisible, value); }
        }

        public bool IsCompanyEditorEnabled
        {
            get { return isCompanyEditorEnabled; }
            set { SetProperty(ref isCompanyEditorEnabled, value); }
        }

        public bool IsProjectEditorEnabled
        {
            get { return isProjectEditorEnabled; }
            set { SetProperty(ref isProjectEditorEnabled, value); }
        }

        public bool IsUserEditorEnabled
        {
            get { return isUserEditorEnabled; }
            set { SetProperty(ref isUserEditorEnabled, value); }
        }

        public bool IsNewProjectEnabled
        {
            get { return isNewProjectEnabled; }
            set { SetProperty(ref isNewProjectEnabled, value); }
        }

        public Company Company
        {
            get { return company; }
            set
            {
                SetProperty(ref company, value);
                IsCompanyEditorEnabled = value != null;
            }
        }

        public Project SelectedProject
        {
            get { return selectedProject; }
            set
            {
                SetProperty(ref selectedProject, value);
                IsProjectEditorEnabled = value != null;
            }
        }

        public User SelectedUser
        {
            get { return selectedUser; }
            set
            {
                SetProperty(ref selectedUser, value);
                IsUserEditorEnabled = value != null;
            }
        }

        public ObservableCollection<Project> Projects { get; set; }
        public ObservableCollection<User> Users { get; set; }

        private async Task LoadCompany()
        {
            Company = await adminService.ReadCompanyByIdAsync(globalModel.CurrentUser.CompanyId);

            var projects = await adminService.ReadProjectsForCompanyAsync(Company);
            Projects.Clear();
            Projects.AddRange(projects);

            var users = await adminService.ReadAllUsersForCompanyAsync(Company.CompanyId);
            Users.Clear();
            Users.AddRange(users);
        }

        #region Company

        private void OnStartEditCompany()
        {
            originalCompanyName = Company.Name;
            IsCompanyEditorVisible = true;
        }

        private async Task OnUpdateCompany()
        {
            await pinzAdminService.UpdateCompanyAsync(Company);
            IsCompanyEditorVisible = false;
        }

        private void OnCancelEditCompany()
        {
            Company.Name = originalCompanyName;
            IsCompanyEditorVisible = false;
        }

        #endregion

        #region Project

        private string originalProjectTitle;

        private void OnNewProject()
        {
            var newProject = new Project
            {
                CompanyId = Company.CompanyId
            };
            SelectedProject = newProject;
            originalProjectTitle = SelectedProject.Name;
            IsProjectEditorVisible = true;
        }

        private void OnEditProject()
        {
            originalProjectTitle = SelectedProject.Name;
            IsProjectEditorVisible = true;
        }

        private async Task OnSaveProject()
        {
            if (SelectedProject.ProjectId == Guid.Empty)
                SelectedProject = await adminService.CreateProjectAsync(selectedProject);
            else
                await adminService.UpdateProjectAsync(selectedProject);
            if (!Projects.Contains(SelectedProject))
                Projects.Add(SelectedProject);
            IsProjectEditorVisible = false;
        }

        private async Task OnDeleteProject()
        {
            if (SelectedProject.ProjectId != Guid.Empty)
                await adminService.DeleteProjectAsync(SelectedProject);
            Projects.Remove(SelectedProject);
            SelectedProject = null;
            IsProjectEditorVisible = false;
        }

        private void OnCancelEditProject()
        {
            if (Projects.Contains(SelectedProject))
            {
                SelectedProject.Name = originalProjectTitle;
            }
            else
            {
                SelectedProject = null;
            }
            IsProjectEditorVisible = false;
        }

        #endregion

        #region User

        private string originalUserEmail;
        private string originalUserFirstName;
        private string originalUserFamilyName;
        private bool originalUserAdmin;

        private void OnEditUser()
        {
            originalUserEmail = SelectedUser.EMail;
            originalUserFirstName = SelectedUser.FirstName;
            originalUserFamilyName = SelectedUser.FamilyName;
            originalUserAdmin = SelectedUser.IsCompanyAdmin;
            IsUserEditorVisible = true;
        }

        private async Task OnUpdateUser()
        {
            await adminService.UpdateUserAsync(SelectedUser);            
            IsUserEditorVisible = false;
        }

        private async Task OnDeleteUser()
        {
            await adminService.DeleteUserAsync(SelectedUser);
            Users.Remove(SelectedUser);
            SelectedUser = null;
            IsUserEditorVisible = false;
        }

        private void OnCancelEditUser()
        {
            SelectedUser.EMail = originalUserEmail;
            SelectedUser.FirstName = originalUserFirstName;
            SelectedUser.FamilyName = originalUserFamilyName;
            SelectedUser.IsCompanyAdmin = originalUserAdmin;
            IsUserEditorVisible = false;
        }

        #endregion
    }
}