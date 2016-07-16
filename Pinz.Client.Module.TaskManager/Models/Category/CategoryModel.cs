using System.Collections.ObjectModel;
using Com.Pinz.Client.DomainModel;

namespace Com.Pinz.Client.Module.TaskManager.Models.Category
{
    public class CategoryModel : DomainModel.Category
    {
        private ProjectModel projectModel;
        private ObservableCollection<Task> tasks;

        public CategoryModel()
        {
        }

        public CategoryModel(DomainModel.Category category, ProjectModel parent)
        {
            ProjectId = category.ProjectId;
            CategoryId = category.CategoryId;
            Name = category.Name;
            projectModel = parent;
        }

        public ProjectModel Project
        {
            get { return projectModel; }
            set { SetProperty(ref projectModel, value); }
        }

        public ObservableCollection<Task> Tasks
        {
            get { return tasks; }
            set { SetProperty(ref tasks, value); }
        }
    }
}