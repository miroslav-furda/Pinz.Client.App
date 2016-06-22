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

        public DelegateCommand InviteUserCommand { get; private set; }
        public DelegateCommand AddUserToProjectCommand { get; private set; }
        public DelegateCommand RemoveUserFromProjectCommand { get; private set; }
        public DelegateCommand CompanyAdminCheckCommand { get; private set; }
        public DelegateCommand ProjectSetAsAdminCommand { get; private set; }

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

            AddUserToProjectCommand = new DelegateCommand(AddUserToProject, CanExecuteAddUserToProject);
            RemoveUserFromProjectCommand = new DelegateCommand(RemoveUserFromProject, CanExecuteRemoveUserFromProject);
            InviteUserCommand = new DelegateCommand(InviteUser, CanExecuteInviteUser);
            CompanyAdminCheckCommand = new DelegateCommand(CompanyAdminCheck);
            ProjectSetAsAdminCommand = new DelegateCommand(SetAsAdmin, CanSetAsAdmin);

            ChangeNotification = new InteractionRequest<INotification>();

            LoadProjects();
        }

        private async void CompanyAdminCheck()
        {
            await System.Threading.Tasks.Task.Run(() => adminService.SetProjectAdminFlag(ProjectSelectedUser.UserId, SelectedProject.ProjectId, ProjectSelectedUser.IsProjectAdmin));
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
            return ProjectSelectedUser != null && !ProjectSelectedUser.IsProjectAdmin;
        }
        #endregion

        private async void InviteUser()
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
                    User newUser = await System.Threading.Tasks.Task.Run(() => adminService.InviteNewUser(NewUserEmail, SelectedProject, globalModel.CurrentUser));
                    ProjectUsers.Add(mapper.Map<ProjectUser>(newUser));
                }
            }
        }

        private async void RemoveUserFromProject()
        {
            await System.Threading.Tasks.Task.Run(() => adminService.RemoveUserFromProject(ProjectSelectedUser, SelectedProject));
            AllCompanyUsers.Add(ProjectSelectedUser);
            ProjectUsers.Remove(ProjectSelectedUser);
        }

        private async void AddUserToProject()
        {
            await System.Threading.Tasks.Task.Run(() => adminService.AddUserToProject(AllCompanySelectedUser, SelectedProject, false));
            ProjectUsers.Add(mapper.Map<ProjectUser>(AllCompanySelectedUser));
            AllCompanyUsers.Remove(AllCompanySelectedUser);
        }

        private async void SetAsAdmin()
        {
            ProjectSelectedUser.IsProjectAdmin = !ProjectSelectedUser.IsProjectAdmin;
            await System.Threading.Tasks.Task.Run(() => adminService.SetProjectAdminFlag(ProjectSelectedUser.UserId, SelectedProject.ProjectId, ProjectSelectedUser.IsProjectAdmin));
        }

        private async void SelectProjectRefs()
        {
            ProjectUsers.Clear();
            List<ProjectUser> projectUserList = await System.Threading.Tasks.Task.Run(() => adminService.ReadAllProjectUsersInProject(SelectedProject));
            projectUserList.ForEach(ProjectUsers.Add);

            AllCompanyUsers.Clear();
            List<User> users = await System.Threading.Tasks.Task.Run(() => adminService.ReadAllUsersForCompany(globalModel.CurrentUser.CompanyId));
            users.ForEach(u =>
            {
                if (projectUserList.All(pu => pu.UserId != u.UserId))
                    AllCompanyUsers.Add(u);
            });

            IsProjectSelected = true;
        }

        public async void LoadProjects()
        {
            Projects = await System.Threading.Tasks.Task.Run(() => adminService.ReadAdminProjectsForUser(globalModel.CurrentUser));
        }
    }
}
