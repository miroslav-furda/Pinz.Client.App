using Com.Pinz.Client.DomainModel;
using System;
using System.Collections.Generic;

namespace Com.Pinz.Client.Model.Service
{
    public interface IAdminClientService
    {
        void SetProjectAdminFlag(Guid userId, Guid projectId, bool isProjectAdmin);

        void AddUserToProject(User user, Project project);

        void RemoveUserFromProject(User user, Project project);

        User CurrentUser { get; }

        bool IsUserLoggedIn { get; }

        System.Threading.Tasks.Task<bool> loginUser(string email, string password);

        bool ChangePasswordForUser(User currentUser, string oldPasword, string newPasword, string newPasword2);

        void UpdateUser(User user);

        List<Project> ReadAdminProjectsForCurrentUser();

        List<User> ReadAllUsersForCompany();

        List<User> ReadAllUsersByProject(Project project);

        User InviteNewUser(string newUserEmail, Project project);
    }
}
