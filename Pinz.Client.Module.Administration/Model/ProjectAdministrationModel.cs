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

namespace Com.Pinz.Client.Module.Administration.Model
{
    public class ProjectAdministrationModel : BindableValidationBase
    {
        public TabModel TabModel { get; private set; }
        public List<Project> Projects { get; private set; }
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


        private IAdminClientService adminService;

        [Inject]
        public ProjectAdministrationModel(IAdminClientService adminService)
        {
            this.adminService = adminService;

            TabModel = new TabModel()
            {
                Title = Properties.Resources.AdministrationTab_Title_Project,
                CanClose = false,
                IsModified = false
            };

            Projects = adminService.ReadAdminProjectsForCurrentUser();
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

        private void InviteUser()
        {
            if (!HasErrors)
            {
                User newUser = adminService.InviteNewUser(NewUserEmail, SelectedProject);
                ProjectUsers.Add(newUser);
            }
        }

        private void RemoveUserFromProject()
        {
            adminService.RemoveUserFromProject(ProjectSelectedUser, SelectedProject);
            AllCompanyUsers.Add(ProjectSelectedUser);
            ProjectUsers.Remove(ProjectSelectedUser);
        }

        private void AddUserToProject()
        {
            adminService.AddUserToProject(AllCompanySelectedUser, SelectedProject);
            ProjectUsers.Add(AllCompanySelectedUser);
            AllCompanyUsers.Remove(AllCompanySelectedUser);
        }

        private void SelectProjectRefs()
        {
            IsProjectSelected = true;
            ProjectUsers.Clear();
            List<User> projectUserList = adminService.ReadAllUsersByProject(SelectedProject);
            projectUserList.ForEach(ProjectUsers.Add);

            AllCompanyUsers.Clear();
            adminService.ReadAllUsersForCompany().ForEach(u =>
            {
                if (!projectUserList.Any(pu => pu.UserId == u.UserId))
                    AllCompanyUsers.Add(u);
            });
        }

    }
}
