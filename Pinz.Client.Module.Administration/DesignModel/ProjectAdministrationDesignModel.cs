using Com.Pinz.Client.Commons.Wpf.Extensions;
using Com.Pinz.Client.DomainModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;

namespace Com.Pinz.Client.Module.Administration.DesignModel
{
    public class ProjectAdministrationDesignModel
    {
        public TabModel TabModel { get; private set; }
        public List<Project> Projects { get; private set; }
        public Project SelectedProject { get; set; }
        public ObservableCollection<User> AllCompanyUsers { get; private set; }
        public ObservableCollection<ProjectUser> ProjectUsers { get; private set; }

        public bool IsProjectSelected { get; private set; }

        public ProjectAdministrationDesignModel()
        {
            TabModel = new TabModel()
            {
                Title = Properties.Resources.AdministrationTab_Title_Project,
                CanClose = false,
                IsModified = false
            };
            Projects = new List<Project>()
            {
                new Project() { Name = "Project1", Description="Project 1 Description" },
                new Project() { Name = "Project2", Description="Project 2 Description" }
            };

            SelectedProject = Projects[0];

            AllCompanyUsers = new ObservableCollection<User>()
            {
                new User() {EMail = "user1@pinzonline.com", FirstName="John1", FamilyName="Smith1", IsCompanyAdmin= false },
                new User() {EMail = "user2@pinzonline.com", FirstName="John2", FamilyName="Smith2", IsCompanyAdmin= false },
                new User() {EMail = "user3@pinzonline.com", FirstName="John3", FamilyName="Smith3", IsCompanyAdmin= false },
                new User() {EMail = "user4@pinzonline.com", FirstName="John4", FamilyName="Smith4", IsCompanyAdmin= false },
                new User() {EMail = "user5@pinzonline.com", FirstName="John5", FamilyName="Smith5", IsCompanyAdmin= false },
                new User() {EMail = "user6@pinzonline.com", FirstName="John6", FamilyName="Smith6", IsCompanyAdmin= false },
                new User() {EMail = "user7@pinzonline.com", FirstName="John7", FamilyName="Smith7", IsCompanyAdmin= false },
                new User() {EMail = "user8@pinzonline.com", FirstName="John8", FamilyName="Smith8", IsCompanyAdmin= false }
            };

            ProjectUsers = new ObservableCollection<ProjectUser>()
            {
                new ProjectUser() {EMail = "userX@pinzonline.com", FirstName="John1", FamilyName="Smith1", IsCompanyAdmin= false, IsProjectAdmin = true},
                new ProjectUser() {EMail = "userA@pinzonline.com", FirstName="John2", FamilyName="Smith2", IsCompanyAdmin= true },
                new ProjectUser() {EMail = "userS@pinzonline.com", FirstName="John3", FamilyName="Smith3", IsCompanyAdmin= true, IsProjectAdmin = true}
            };

            IsProjectSelected = true;
        }

        public InteractionRequest<INotification> ChangeNotification { get; private set; }

        public string NewUserEmail { get; set; }

        public bool IsInviteEnabled { get; set; }

        public bool ProjectSetAsAdminEnabled { get; set; }

        public User AllCompanySelectedUser { get; set; }

        public ProjectUser ProjectSelectedUser { get; set; }

        public DelegateCommand InviteUserCommand { get; private set; }
        public DelegateCommand AddUserToProjectCommand { get; private set; }
        public DelegateCommand RemoveUserFromProjectCommand { get; private set; }
        public DelegateCommand CompanyAdminCheckCommand { get; private set; }
        public DelegateCommand ProjectSetAsAdminCommand { get; private set; }

    }
}
