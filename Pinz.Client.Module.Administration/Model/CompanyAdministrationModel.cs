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
using Prism.Interactivity.InteractionRequest;
using Prism.Events;
using Com.Pinz.Client.Commons.Event;

namespace Com.Pinz.Client.Module.Administration.Model
{
    public class CompanyAdministrationModel : BindableValidationBase
    {
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
        public AwaitableDelegateCommand InitializeCommand { get; private set; }

        public InteractionRequest<IConfirmation> DeleteConfirmation { get; private set; }

        private bool _isCompanyEditorVisible;
        public bool IsCompanyEditorVisible
        {
            get { return _isCompanyEditorVisible; }
            set { SetProperty(ref _isCompanyEditorVisible, value); }
        }

        private bool _isProjectEditorVisible;
        public bool IsProjectEditorVisible
        {
            get { return _isProjectEditorVisible; }
            set
            {
                SetProperty(ref _isProjectEditorVisible, value);
                IsNewProjectEnabled = !IsNewProjectEnabled;
            }
        }

        private bool _isUserEditorVisible;
        public bool IsUserEditorVisible
        {
            get { return _isUserEditorVisible; }
            set { SetProperty(ref _isUserEditorVisible, value); }
        }

        private bool _isCompanyEditorEnabled;
        public bool IsCompanyEditorEnabled
        {
            get { return _isCompanyEditorEnabled; }
            set { SetProperty(ref _isCompanyEditorEnabled, value); }
        }

        private bool _isProjectEditorEnabled;
        public bool IsProjectEditorEnabled
        {
            get { return _isProjectEditorEnabled; }
            set { SetProperty(ref _isProjectEditorEnabled, value); }
        }

        private bool _isUserEditorEnabled;
        public bool IsUserEditorEnabled
        {
            get { return _isUserEditorEnabled; }
            set { SetProperty(ref _isUserEditorEnabled, value); }
        }

        private bool _isNewProjectEnabled = true;
        public bool IsNewProjectEnabled
        {
            get { return _isNewProjectEnabled; }
            set { SetProperty(ref _isNewProjectEnabled, value); }
        }

        private Company _company;
        public Company Company
        {
            get { return _company; }
            set
            {
                SetProperty(ref _company, value);
                IsCompanyEditorEnabled = value != null;
            }
        }

        private Project _selectedProject;
        public Project SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                SetProperty(ref _selectedProject, value);
                IsProjectEditorEnabled = value != null;
            }
        }

        private User _selectedUser;
        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                SetProperty(ref _selectedUser, value);
                IsUserEditorEnabled = value != null;
            }
        }

        public ObservableCollection<Project> Projects { get; set; }
        public ObservableCollection<User> Users { get; set; }

        private string _originalCompanyName;

        private readonly IAdministrationRemoteService _adminService;
        private readonly IPinzAdminRemoteService _pinzAdminService;
        private readonly ApplicationGlobalModel _globalModel;
        private readonly IEventAggregator _eventAggregator;

        [Inject]
        public CompanyAdministrationModel(IAdministrationRemoteService adminService, IPinzAdminRemoteService pinzAdminService,
            ApplicationGlobalModel globalModel, IEventAggregator eventAggregator)
        {
            this._globalModel = globalModel;
            TabModel = new TabModel
            {
                Title = Resources.AdministrationTab_Title_Company,
                CanClose = false,
                IsModified = false
            };
            this._adminService = adminService;
            this._pinzAdminService = pinzAdminService;
            this._eventAggregator = eventAggregator;

            StartEditCompany = new DelegateCommand(OnStartEditCompany, IsCompanyAdmin);
            CancelEditCompany = new DelegateCommand(OnCancelEditCompany, IsCompanyAdmin);
            UpdateCompany = new AwaitableDelegateCommand(OnUpdateCompany, IsCompanyAdmin);

            NewProject = new DelegateCommand(OnNewProject, IsCompanyAdmin);
            StartEditProject = new DelegateCommand(OnEditProject, IsCompanyAdmin);
            DeleteProject = new AwaitableDelegateCommand(OnDeleteProject, IsCompanyAdmin);
            UpdateProject = new AwaitableDelegateCommand(OnSaveProject, IsCompanyAdmin);
            CancelEditProject = new DelegateCommand(OnCancelEditProject, IsCompanyAdmin);

            StartEditUser = new DelegateCommand(OnEditUser, IsCompanyAdmin);
            DeleteUser = new AwaitableDelegateCommand(OnDeleteUser, IsCompanyAdmin);
            UpdateUser = new AwaitableDelegateCommand(OnUpdateUser, IsCompanyAdmin);
            CancelEditUser = new DelegateCommand(OnCancelEditUser, IsCompanyAdmin);

            Projects = new ObservableCollection<Project>();
            Users = new ObservableCollection<User>();

            InitializeCommand = new AwaitableDelegateCommand(LoadCompany);

            this.DeleteConfirmation = new InteractionRequest<IConfirmation>();
        }

        private async Task LoadCompany()
        {
            try
            {
                Company = await _adminService.ReadCompanyByIdAsync(_globalModel.CurrentUser.CompanyId);

                var projects = await _adminService.ReadProjectsForCompanyAsync(Company);
                Projects.Clear();
                Projects.AddRange(projects);

                var users = await _adminService.ReadAllUsersForCompanyAsync(Company.CompanyId);
                Users.Clear();
                Users.AddRange(users);
            }
            catch (TimeoutException timeoutEx)
            {
                _eventAggregator.GetEvent<TimeoutErrorEvent>().Publish(timeoutEx);
            }

            IsCompanyEditorVisible = false;
            IsProjectEditorVisible = false;
            IsUserEditorVisible = false;
        }

        private bool IsCompanyAdmin()
        {
            return _globalModel.CurrentUser.IsCompanyAdmin;
        }

        #region Company

        private void OnStartEditCompany()
        {
            _originalCompanyName = Company.Name;
            IsCompanyEditorVisible = true;
        }

        private async Task OnUpdateCompany()
        {
            if (Company.ValidateModel())
            {
                try
                {
                    await _pinzAdminService.UpdateCompanyAsync(Company);
                    IsCompanyEditorVisible = false;
                }
                catch (TimeoutException timeoutEx)
                {
                    _eventAggregator.GetEvent<TimeoutErrorEvent>().Publish(timeoutEx);
                }
            }
        }

        private void OnCancelEditCompany()
        {
            Company.Name = _originalCompanyName;
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
            if (_selectedProject.ValidateModel())
            {
                try
                {
                    if (SelectedProject.ProjectId == Guid.Empty)
                        SelectedProject = await _adminService.CreateProjectAsync(_selectedProject);
                    else
                        await _adminService.UpdateProjectAsync(_selectedProject);
                    if (!Projects.Contains(SelectedProject))
                        Projects.Add(SelectedProject);
                    IsProjectEditorVisible = false;
                }
                catch (TimeoutException timeoutEx)
                {
                    _eventAggregator.GetEvent<TimeoutErrorEvent>().Publish(timeoutEx);
                }
            }
        }

        private async Task OnDeleteProject()
        {
            this.DeleteConfirmation.Raise(new Confirmation
            {
                Title = Resources.DeleteProjectConfirmation_Title,
                Content = Resources.DeleteProjectConfirmation_Content
            }, async (dialog) =>
            {
                if (dialog.Confirmed)
                {
                    try
                    {
                        if (SelectedProject.ProjectId != Guid.Empty)
                            await _adminService.DeleteProjectAsync(SelectedProject);
                        Projects.Remove(SelectedProject);
                        SelectedProject = null;
                        IsProjectEditorVisible = false;
                    }
                    catch (TimeoutException timeoutEx)
                    {
                        _eventAggregator.GetEvent<TimeoutErrorEvent>().Publish(timeoutEx);
                    }
                }
            });
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
            if (SelectedUser.ValidateModel())
            {
                try
                {
                    await _adminService.UpdateUserAsync(SelectedUser);
                    IsUserEditorVisible = false;
                }
                catch (TimeoutException timeoutEx)
                {
                    _eventAggregator.GetEvent<TimeoutErrorEvent>().Publish(timeoutEx);
                }
            }
        }

        private async Task OnDeleteUser()
        {
            this.DeleteConfirmation.Raise(new Confirmation
            {
                Title = Resources.DeleteUserConfirmation_Title,
                Content = Resources.DeleteUserConfirmation_Content
            }, async (dialog) =>
            {
                if (dialog.Confirmed)
                {
                    try
                    {
                        await _adminService.DeleteUserAsync(SelectedUser);
                        Users.Remove(SelectedUser);
                        SelectedUser = null;
                        IsUserEditorVisible = false;
                    }
                    catch (TimeoutException timeoutEx)
                    {
                        _eventAggregator.GetEvent<TimeoutErrorEvent>().Publish(timeoutEx);
                    }
                }
            });
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