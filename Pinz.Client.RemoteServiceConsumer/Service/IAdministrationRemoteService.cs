using Com.Pinz.Client.DomainModel;
using System;
using System.Collections.Generic;

namespace Com.Pinz.Client.RemoteServiceConsumer.Service
{
    public interface IAdministrationRemoteService
    {
        User InviteNewUser(string newUserEmail, Project project, User invitingUser);

        void SetProjectAdminFlag(Guid userId, Guid projectId, bool isProjectAdmin);

        List<User> ReadAllUsersByProject(Project project);

        List<Project> ReadAdminProjectsForUser(User user);

        bool ChangeUserPassword(User user, string oldPassword, string newPassword, string newPassword2);

        List<Project> ReadProjectsForCompany(Company company);

        List<User> ReadAllUsersForCompany(Guid companyId);

        Company ReadCompanyById(Guid id);

        void AddUserToProject(User user, Project project, bool isProjectAdmin);

        void RemoveUserFromProject(User user, Project project);

        Project CreateProject(Project project);

        void UpdateProject(Project project);

        void DeleteProject(Project project);

        User CreateUser(User user);

        void UpdateUser(User user);

        void DeleteUser(User user);
    }
}
