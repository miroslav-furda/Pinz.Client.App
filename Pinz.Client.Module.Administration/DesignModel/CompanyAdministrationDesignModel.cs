using System.Collections.ObjectModel;
using Com.Pinz.Client.Commons.Wpf.Extensions;
using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.Module.Administration.Properties;
using Prism.Commands;

namespace Com.Pinz.Client.Module.Administration.DesignModel
{
    public class CompanyAdministrationDesignModel
    {
        public CompanyAdministrationDesignModel()
        {
            TabModel = new TabModel
            {
                Title = Resources.AdministrationTab_Title_Project,
                CanClose = false,
                IsModified = false
            };
            Projects = new ObservableCollection<Project>
            {
                new Project {Name = "Project1", Description = "Project 1 Description"},
                new Project {Name = "Project2", Description = "Project 2 Description"}
            };

            SelectedProject = Projects[0];

            Users = new ObservableCollection<User>
            {
                new User {EMail = "user1@pinzonline.com", FirstName = "John1", FamilyName = "Smith1", IsCompanyAdmin = false},
                new User {EMail = "user2@pinzonline.com", FirstName = "John2", FamilyName = "Smith2", IsCompanyAdmin = false},
                new User {EMail = "user3@pinzonline.com", FirstName = "John3", FamilyName = "Smith3", IsCompanyAdmin = false},
                new User {EMail = "user4@pinzonline.com", FirstName = "John4", FamilyName = "Smith4", IsCompanyAdmin = false},
                new User {EMail = "user5@pinzonline.com", FirstName = "John5", FamilyName = "Smith5", IsCompanyAdmin = false},
                new User {EMail = "user6@pinzonline.com", FirstName = "John6", FamilyName = "Smith6", IsCompanyAdmin = false},
                new User {EMail = "user7@pinzonline.com", FirstName = "John7", FamilyName = "Smith7", IsCompanyAdmin = false},
                new User {EMail = "user8@pinzonline.com", FirstName = "John8", FamilyName = "Smith8", IsCompanyAdmin = false}
            };

            SelectedUser = Users[0];

            //Company = new Company
            //{
            //    Name = "Pinz company"
            //};
            IsCompanyEditorVisible = true;

            IsCompanyEditorEnabled = false;
            IsProjectEditorEnabled = true;
            IsUserEditorEnabled = true;

            IsNewProjectEnabled = true;
            IsNewUserEnabled = true;
        }

        public TabModel TabModel { get; private set; }
        public bool IsCompanyEditorVisible { get; set; }

        public bool IsProjectEditorVisible { get; set; }

        public bool IsUserEditorVisible { get; set; }

        public bool IsCompanyEditorEnabled { get; set; }

        public bool IsProjectEditorEnabled { get; set; }

        public bool IsUserEditorEnabled { get; set; }

        public bool IsNewUserEnabled { get; set; }

        public bool IsNewProjectEnabled { get; set; }

        public Company Company { get; set; }

        public Project SelectedProject { get; set; }

        public User SelectedUser { get; set; }

        public ObservableCollection<Project> Projects { get; set; }
        public ObservableCollection<User> Users { get; set; }

        public DelegateCommand StartEditCompany { get; private set; }
        public DelegateCommand CancelEditCompany { get; private set; }
        public DelegateCommand UpdateCompany { get; private set; }

        public DelegateCommand NewProject { get; private set; }
        public DelegateCommand StartEditProject { get; private set; }
        public DelegateCommand CancelEditProject { get; private set; }
        public DelegateCommand UpdateProject { get; private set; }
        public DelegateCommand DeleteProject { get; private set; }

        public DelegateCommand StartEditUser { get; private set; }
        public DelegateCommand CancelEditUser { get; private set; }
        public DelegateCommand UpdateUser { get; private set; }
        public DelegateCommand DeleteUser { get; private set; }
    }
}