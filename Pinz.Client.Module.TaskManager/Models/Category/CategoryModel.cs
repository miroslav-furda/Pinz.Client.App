using System.Collections.ObjectModel;
using Com.Pinz.Client.Module.TaskManager.Models.Task;

namespace Com.Pinz.Client.Module.TaskManager.Models.Category
{
    public class CategoryModel : DomainModel.Category
    {
        private ProjectModel projectModel;
        private ObservableCollection<TaskModel> tasks;

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

        public ObservableCollection<TaskModel> Tasks
        {
            get { return tasks; }
            set { SetProperty(ref tasks, value); }
        }
    }
}