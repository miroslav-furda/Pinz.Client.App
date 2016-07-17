using Com.Pinz.Client.Commons.Wpf.Extensions;
using Com.Pinz.Client.DomainModel;
using Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ninject;
using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Com.Pinz.Client.Commons.Prism;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.Client.Model;
using Prism.Interactivity.InteractionRequest;

namespace Com.Pinz.Client.Module.Administration.Model
{
    public class ProjectAdministrationModel : BindableValidationBase
    {
        public TabModel TabModel { get; private set; }

        private List<Project> _projects;
        public List<Project> Projects
        {
            get
            {
                return _projects;

            }
            set
            {
                SetProperty(ref this._projects, value);
            }
        }
        public ObservableCollection<User> AllCompanyUsers { get; private set; }
        public ObservableCollection<ProjectUser> ProjectUsers { get; private set; }

        private User _allCompanySelectedUser;
        public User AllCompanySelectedUser
        {
            get
            {
                return _allCompanySelectedUser;
            }
            set
            {
                _allCompanySelectedUser = value;
                AddUserToProjectCommand.RaiseCanExecuteChanged();
            }
        }

        private ProjectUser _projectSelectedUser;
        public ProjectUser ProjectSelectedUser
        {
            get
            {
                return _projectSelectedUser;
            }
            set
            {
                _projectSelectedUser = value;
                RemoveUserFromProjectCommand.RaiseCanExecuteChanged();
                ProjectSetAsAdminCommand.RaiseCanExecuteChanged();
            }
        }

        private string _newUserEmail;
        [EmailAddress]
        public string NewUserEmail
        {
            get
            {
                return _newUserEmail;
            }
            set
            {
                SetProperty(ref this._newUserEmail, value);
                InviteUserCommand.RaiseCanExecuteChanged();
            }
        }

        public AwaitableDelegateCommand InviteUserCommand { get; private set; }
        public AwaitableDelegateCommand AddUserToProjectCommand { get; private set; }
        public AwaitableDelegateCommand RemoveUserFromProjectCommand { get; private set; }
        public AwaitableDelegateCommand CompanyAdminCheckCommand { get; private set; }
        public AwaitableDelegateCommand ProjectSetAsAdminCommand { get; private set; }
        public AwaitableDelegateCommand InitializeCommand { get; private set; }
        

        private bool _isProjectSelected;
        public bool IsProjectSelected
        {
            get
            {
                return _isProjectSelected;
            }
            private set
            {
                SetProperty(ref this._isProjectSelected, value);
            }
        }

        private Project _selectedProject;
        public Project SelectedProject
        {
            get
            {
                return _selectedProject;
            }
            set
            {
                bool result = SetProperty(ref this._selectedProject, value);
                if (result)
                    SelectProjectRefs();
            }
        }


        private IAdministrationRemoteService adminService;
        private ApplicationGlobalModel globalModel;
        private IMapper mapper;
        public InteractionRequest<INotification> ChangeNotification { get; private set; }

        [Inject]
        public ProjectAdministrationModel(IAdministrationRemoteService adminService, ApplicationGlobalModel globalModel, [Named("WpfClientMapper")] IMapper mapper)
        {
            this.adminService = adminService;
            this.globalModel = globalModel;
            this.mapper = mapper;

            TabModel = new TabModel()
            {
                Title = Properties.Resources.AdministrationTab_Title_Project,
                CanClose = false,
                IsModified = false
            };


            IsProjectSelected = false;

            AllCompanyUsers = new ObservableCollection<User>();
            ProjectUsers = new ObservableCollection<ProjectUser>();

            AddUserToProjectCommand = new AwaitableDelegateCommand(AddUserToProject, CanExecuteAddUserToProject);
            RemoveUserFromProjectCommand = new AwaitableDelegateCommand(RemoveUserFromProject, CanExecuteRemoveUserFromProject);
            InviteUserCommand = new AwaitableDelegateCommand(InviteUser, CanExecuteInviteUser);
            CompanyAdminCheckCommand = new AwaitableDelegateCommand(CompanyAdminCheck);
            ProjectSetAsAdminCommand = new AwaitableDelegateCommand(SetAsAdmin, CanSetAsAdmin);
            InitializeCommand = new AwaitableDelegateCommand(LoadProjects);

            ChangeNotification = new InteractionRequest<INotification>();
        }

        private async System.Threading.Tasks.Task CompanyAdminCheck()
        {
            await adminService.SetProjectAdminFlagAsync(ProjectSelectedUser.UserId, SelectedProject.ProjectId, ProjectSelectedUser.IsProjectAdmin);
        }

        #region CanExecute
        private bool CanExecuteInviteUser()
        {
            return !String.IsNullOrWhiteSpace(NewUserEmail) && !HasErrors;
        }

        private bool CanExecuteRemoveUserFromProject()
        {
            return ProjectSelectedUser != null;
        }

        private bool CanExecuteAddUserToProject()
        {
            return AllCompanySelectedUser != null;
        }

        private bool CanSetAsAdmin()
        {
            return ProjectSelectedUser != null;
        }
        #endregion

        private async System.Threading.Tasks.Task InviteUser()
        {
            if (!HasErrors)
            {
                if (ProjectUsers.Any(nav => nav.EMail == NewUserEmail) || AllCompanyUsers.Any(nav => nav.EMail == NewUserEmail))
                {
                    ChangeNotification.Raise(new Notification
                    {
                        Title = Properties.Resources.User_Invitation_Exists_Title,
                        Content = string.Format(Properties.Resources.User_Invitation_Exists_Content, NewUserEmail)
                    });
                }
                else
                {
                    User newUser = await adminService.InviteNewUserAsync(NewUserEmail, SelectedProject, globalModel.CurrentUser);
                    ProjectUsers.Add(mapper.Map<ProjectUser>(newUser));
                }
            }
        }

        private async System.Threading.Tasks.Task RemoveUserFromProject()
        {
            await adminService.RemoveUserFromProjectAsync(ProjectSelectedUser, SelectedProject);
            AllCompanyUsers.Add(ProjectSelectedUser);
            ProjectUsers.Remove(ProjectSelectedUser);
        }

        private async System.Threading.Tasks.Task AddUserToProject()
        {
            await adminService.AddUserToProjectAsync(AllCompanySelectedUser, SelectedProject, false);
            ProjectUsers.Add(mapper.Map<ProjectUser>(AllCompanySelectedUser));
            AllCompanyUsers.Remove(AllCompanySelectedUser);
        }

        private async System.Threading.Tasks.Task SetAsAdmin()
        {
            ProjectSelectedUser.IsProjectAdmin = !ProjectSelectedUser.IsProjectAdmin;
            await adminService.SetProjectAdminFlagAsync(ProjectSelectedUser.UserId, SelectedProject.ProjectId, ProjectSelectedUser.IsProjectAdmin);
        }

        private async System.Threading.Tasks.Task SelectProjectRefs()
        {
            ProjectUsers.Clear();
            List<ProjectUser> projectUserList = await adminService.ReadAllProjectUsersInProjectAsync(SelectedProject);
            projectUserList.ForEach(ProjectUsers.Add);

            AllCompanyUsers.Clear();
            List<User> users = await adminService.ReadAllUsersForCompanyAsync(globalModel.CurrentUser.CompanyId);
            users.ForEach(u =>
            {
                if (projectUserList.All(pu => pu.UserId != u.UserId))
                    AllCompanyUsers.Add(u);
            });

            IsProjectSelected = true;
        }

        public async System.Threading.Tasks.Task LoadProjects()
        {
            Projects = await adminService.ReadAdminProjectsForUserAsync(globalModel.CurrentUser);
            ProjectUsers.Clear();
            AllCompanyUsers.Clear();
        }
    }
}
