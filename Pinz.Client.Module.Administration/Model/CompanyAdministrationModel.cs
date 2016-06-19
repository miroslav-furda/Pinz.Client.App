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
        public CompanyAdministrationModel(IAdministrationRemoteService adminService, ApplicationGlobalModel globalModel)
        {
            this.globalModel = globalModel;
            TabModel = new TabModel
            {
                Title = Resources.AdministrationTab_Title_Company,
                CanClose = false,
                IsModified = false
            };
            this.adminService = adminService;

            StartEditCompany = new DelegateCommand(OnStartEditCompany);
            CancelEditCompany = new DelegateCommand(OnCancelEditCompany);
            UpdateCompany = new DelegateCommand(OnUpdateCompany);

            NewProject = new DelegateCommand(OnNewProject);
            StartEditProject = new DelegateCommand(OnEditProject);
            DeleteProject = new DelegateCommand(OnDeleteProject);
            UpdateProject = new DelegateCommand(OnSaveProject);
            CancelEditProject = new DelegateCommand(OnCancelEditProject);

            StartEditUser = new DelegateCommand(OnEditUser);
            DeleteUser = new DelegateCommand(OnDeleteUser);
            UpdateUser = new DelegateCommand(OnUpdateUser);
            CancelEditUser = new DelegateCommand(OnCancelEditUser);

            Projects = new ObservableCollection<Project>();
            Users = new ObservableCollection<User>();

            LoadCompany();
        }

        public TabModel TabModel { get; private set; }


        public DelegateCommand StartEditCompany { get; private set; }
        public DelegateCommand CancelEditCompany { get; private set; }
        public DelegateCommand UpdateCompany { get; private set; }

        public DelegateCommand NewProject { get; private set; }
        public DelegateCommand StartEditProject { get; private set; }
        public DelegateCommand CancelEditProject { get; private set; }
        public DelegateCommand UpdateProject { get; private set; }
        public DelegateCommand DeleteProject { get; private set; }

        public DelegateCommand StartEditUser { get; private set; }
        public DelegateCommand CancelEditUser { get; private set; }
        public DelegateCommand UpdateUser { get; private set; }
        public DelegateCommand DeleteUser { get; private set; }

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

        private async void LoadCompany()
        {
            Company = await Task.Run(() => adminService.ReadCompanyById(globalModel.CurrentUser.CompanyId));

            var projects = await Task.Run(() => adminService.ReadProjectsForCompany(Company));
            Projects.Clear();
            Projects.AddRange(projects);

            var users = await Task.Run(() => adminService.ReadAllUsersForCompany(Company.CompanyId));
            Users.Clear();
            Users.AddRange(users);
        }

        #region Company

        private void OnStartEditCompany()
        {
            originalCompanyName = Company.Name;
            IsCompanyEditorVisible = true;
        }

        private void OnUpdateCompany()
        {
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

        private async void OnSaveProject()
        {
            if (SelectedProject.ProjectId == Guid.Empty)
                await Task.Run(() => adminService.CreateProject(selectedProject));
            else
                await Task.Run(() => adminService.UpdateProject(selectedProject));
            if (!Projects.Contains(SelectedProject))
                Projects.Add(SelectedProject);
            IsProjectEditorVisible = false;
        }

        private async void OnDeleteProject()
        {
            if (SelectedProject.ProjectId != Guid.Empty)
                await Task.Run(() => adminService.DeleteProject(SelectedProject));
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

        private async void OnUpdateUser()
        {
            await Task.Run(() => adminService.UpdateUser(SelectedUser));
            IsUserEditorVisible = false;
        }

        private async void OnDeleteUser()
        {
            await Task.Run(() => adminService.DeleteUser(SelectedUser));
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