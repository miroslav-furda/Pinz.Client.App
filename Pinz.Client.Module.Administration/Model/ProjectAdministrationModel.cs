using Com.Pinz.Client.Commons.Wpf.Extensions;
using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.Model.Service;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ninject;
using System;
using System.ComponentModel.DataAnnotations;
using Com.Pinz.Client.Commons.Prism;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.Client.Model;
using Prism.Regions;

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
                if(_projects == null)
                {
                    LoadProjects();
                }
                return _projects;

            }
            set
            {
                SetProperty(ref this._projects, value);
            }
        }
        public ObservableCollection<User> AllCompanyUsers { get; private set; }
        public ObservableCollection<User> ProjectUsers { get; private set; }

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

        private User _projectSelectedUser;
        public User ProjectSelectedUser
        {
            get
            {
                return _projectSelectedUser;
            }
            set
            {
                _projectSelectedUser = value;
                RemoveUserFromProjectCommand.RaiseCanExecuteChanged();
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

        [Inject]
        public ProjectAdministrationModel(IAdministrationRemoteService adminService, ApplicationGlobalModel globalModel)
        {
            this.adminService = adminService;
            this.globalModel = globalModel;

            TabModel = new TabModel()
            {
                Title = Properties.Resources.AdministrationTab_Title_Project,
                CanClose = false,
                IsModified = false
            };


            IsProjectSelected = false;

            AllCompanyUsers = new ObservableCollection<User>();
            ProjectUsers = new ObservableCollection<User>();

            AddUserToProjectCommand = new DelegateCommand(AddUserToProject, CanExecuteAddUserToProject);
            RemoveUserFromProjectCommand = new DelegateCommand(RemoveUserFromProject, CanExecuteRemoveUserFromProject);
            InviteUserCommand = new DelegateCommand(InviteUser, CanExecuteInviteUser);
            CompanyAdminCheckCommand = new DelegateCommand(CompanyAdminCheck);
        }

        private void CompanyAdminCheck()
        {
            //adminService.SetProjectAdminFlag(ProjectSelectedUser.UserId, SelectedProject.ProjectId, ProjectSelectedUser.Is);
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
        #endregion

        private async void InviteUser()
        {
            if (!HasErrors)
            {
                User newUser = await System.Threading.Tasks.Task.Run(() => adminService.InviteNewUser(NewUserEmail, SelectedProject, globalModel.CurrentUser));
                ProjectUsers.Add(newUser);
            }
        }

        private async void RemoveUserFromProject()
        {
            await System.Threading.Tasks.Task.Run(() =>  adminService.RemoveUserFromProject(ProjectSelectedUser, SelectedProject) );
            AllCompanyUsers.Add(ProjectSelectedUser);
            ProjectUsers.Remove(ProjectSelectedUser);
        }

        private async void AddUserToProject()
        {
            await System.Threading.Tasks.Task.Run(() => adminService.AddUserToProject(AllCompanySelectedUser, SelectedProject, false));
            ProjectUsers.Add(AllCompanySelectedUser);
            AllCompanyUsers.Remove(AllCompanySelectedUser);
        }

        private async void SelectProjectRefs()
        {
            IsProjectSelected = true;
            ProjectUsers.Clear();
            List<User> projectUserList = await System.Threading.Tasks.Task.Run(() => adminService.ReadAllUsersByProject(SelectedProject));
            projectUserList.ForEach(ProjectUsers.Add);

            AllCompanyUsers.Clear();
            List<User> users = await System.Threading.Tasks.Task.Run(() => adminService.ReadAllUsersForCompany(globalModel.CurrentUser.CompanyId));
            users.ForEach(u =>
            {
                if (!projectUserList.Any(pu => pu.UserId == u.UserId))
                    AllCompanyUsers.Add(u);
            });
        }

        public async void LoadProjects()
        {
            Projects =  await System.Threading.Tasks.Task.Run(() => adminService.ReadAdminProjectsForUser(globalModel.CurrentUser));
        }
    }
}
