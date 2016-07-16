using System.Collections.ObjectModel;
using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.Module.TaskManager.Models.Category;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class ProjectModel : Project
    {
        private ObservableCollection<CategoryModel> categories;
        private ObservableCollection<User> projectUsers;

        public ProjectModel()
        {
            ProjectUsers = new ObservableCollection<User>();
        }

        public ProjectModel(Project project) : this()
        {
            CompanyId = project.CompanyId;
            Description = project.Description;
            Name = project.Name;
            ProjectId = project.ProjectId;
        }

        public ObservableCollection<User> ProjectUsers
        {
            get { return projectUsers; }
            set { SetProperty(ref projectUsers, value); }
        }

        public ObservableCollection<CategoryModel> Categories
        {
            get { return categories; }
            set { SetProperty(ref categories, value); }
        }
    }
}