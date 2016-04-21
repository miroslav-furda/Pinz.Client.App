using System;
using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.Model.Service;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Prism.Mvvm;
using System.Collections.Generic;

namespace Com.Pinz.Client.Model.Remote
{
    public class AdminClientServiceRemote : BindableBase, IAdminClientService
    {
        private UserNameClientCredentials userCredentials;
        private IAuthorisationRemoteService authorisationService;
        private IAdministrationRemoteService administrationService;

        public User CurrentUser { get; private set; }

        private bool _isUserLoggedIn;
        public bool IsUserLoggedIn
        {
            get
            {
                return _isUserLoggedIn;
            }
            private set
            {
                SetProperty(ref this._isUserLoggedIn, value);
            }
        }

        public AdminClientServiceRemote(UserNameClientCredentials userCredentials, IAuthorisationRemoteService authorisationService, IAdministrationRemoteService administrationService)
        {
            this.userCredentials = userCredentials;
            this.authorisationService = authorisationService;
            this.administrationService = administrationService;
        }

        public User InviteNewUser(string newUserEmail, Project project)
        {
            return administrationService.InviteNewUser(newUserEmail, project, CurrentUser);
        }

        public void SetProjectAdminFlag(Guid userId, Guid projectId, bool isProjectAdmin)
        {
            administrationService.SetProjectAdminFlag(userId, projectId, isProjectAdmin);
        }

        public List<User> ReadAllUsersByProject(Project project)
        {
            return administrationService.ReadAllUsersByProject(project);
        }

        public void AddUserToProject(User user, Project project)
        {
            administrationService.AddUserToProject(user, project, false);
        }

        public void RemoveUserFromProject(User user, Project project)
        {
            administrationService.RemoveUserFromProject(user, project);
        }

        public bool loginUser(string email, string password)
        {
            userCredentials.UserName = email;
            userCredentials.Password = password;
            userCredentials.UpdateCredentialsForAllFactories();

            CurrentUser = authorisationService.ReadUserByEmail(email);
            IsUserLoggedIn = true;

            return true;
        }

        public void UpdateUser(User user)
        {
            administrationService.UpdateUser(user);
        }

        public bool ChangePasswordForUser(User currentUser, string oldPassword, string newPassword, string newPassword2)
        {
            return administrationService.ChangeUserPassword(currentUser, oldPassword, newPassword, newPassword2);
        }

        public List<Project> ReadAdminProjectsForCurrentUser()
        {
            return administrationService.ReadAdminProjectsForUser(CurrentUser);
        }

        public List<User> ReadAllUsersForCompany()
        {
            return administrationService.ReadAllUsersForCompany(CurrentUser.CompanyId);
        }
    }
}
