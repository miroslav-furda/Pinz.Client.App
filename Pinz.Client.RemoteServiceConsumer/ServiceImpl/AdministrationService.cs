using System;
using System.Collections.Generic;
using Ninject;
using AutoMapper;
using System.ServiceModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.Client.DomainModel;

namespace Com.Pinz.Client.RemoteServiceConsumer.ServiceImpl
{
    internal class AdministrationService : ServiceBase, IAdministrationRemoteService
    {
        private IMapper mapper;
        private ChannelFactory<AdministrationServiceReference.IAdministrationService> clientFactory;
        private AdministrationServiceReference.IAdministrationService adminChannel;

        [Inject]
        public AdministrationService([Named("ServiceConsumerMapper")] IMapper mapper, ChannelFactory<AdministrationServiceReference.IAdministrationService> clientFactory)
        {
            this.mapper = mapper;
            this.clientFactory = clientFactory;
        }

        public bool ChangeUserPassword(User user, string oldPassword, string newPassword, string newPassword2)
        {
            return adminChannel.ChangeUserPassword(user.UserId, oldPassword, newPassword, newPassword2);
        }

        public void AddUserToProject(User user, Project project, bool isProjectAdmin)
        {
            adminChannel.AddUserToProject(user.UserId, project.ProjectId, isProjectAdmin);
        }

        public void RemoveUserFromProject(User user, Project project)
        {
            adminChannel.RemoveUserFromProject(user.UserId, project.ProjectId);
        }

        public List<User> ReadAllUsersForCompany(Company company)
        {
            List<AdministrationServiceReference.UserDO> rUsers = adminChannel.ReadAllUsersForCompanyId(company.CompanyId);
            List<User> users = mapper.Map<List<AdministrationServiceReference.UserDO>, List<User>>(rUsers);
            return users;
        }

        public Company ReadCompanyById(Guid id)
        {
            AdministrationServiceReference.CompanyDO rCompany = adminChannel.ReadCompanyById(id);
            return mapper.Map<Company>(rCompany);
        }

        public List<Project> ReadProjectsForCompany(Company company)
        {
            List<AdministrationServiceReference.ProjectDO> rProjects = adminChannel.ReadProjectsForCompanyId(company.CompanyId);
            List<Project> projectList = mapper.Map<List<AdministrationServiceReference.ProjectDO>, List<Project>>(rProjects);
            return projectList;
        }


        #region Project CUD
        public Project CreateProject(Project project)
        {
            AdministrationServiceReference.ProjectDO rProject = adminChannel.CreateProject(mapper.Map<AdministrationServiceReference.ProjectDO>(project));
            mapper.Map(rProject, project);
            return project;
        }

        public void UpdateProject(Project project)
        {
            AdministrationServiceReference.ProjectDO rProject = adminChannel.UpdateProject(mapper.Map<AdministrationServiceReference.ProjectDO>(project));
            mapper.Map(rProject, project);
        }

        public void DeleteProject(Project project)
        {
            adminChannel.DeleteProject(mapper.Map<AdministrationServiceReference.ProjectDO>(project));
        }
        #endregion

        #region User CUD
        public void DeleteUser(User user)
        {
            adminChannel.DeleteUser(mapper.Map<AdministrationServiceReference.UserDO>(user));
        }

        public User CreateUser(User user)
        {
            AdministrationServiceReference.UserDO rUser = mapper.Map<AdministrationServiceReference.UserDO>(user);
            rUser = adminChannel.CreateUser(rUser);
            mapper.Map(rUser, user);
            return user;
        }

        public void UpdateUser(User user)
        {
            AdministrationServiceReference.UserDO rUser = adminChannel.UpdateUser(mapper.Map<AdministrationServiceReference.UserDO>(user));
            mapper.Map(rUser, user);
        }
        #endregion 

        public override void OpenChannel()
        {
            adminChannel = clientFactory.CreateChannel();
        }

        public override void CloseChannel()
        {
            CloseOrAbortServiceChannel(adminChannel as ICommunicationObject);
        }
    }
}
