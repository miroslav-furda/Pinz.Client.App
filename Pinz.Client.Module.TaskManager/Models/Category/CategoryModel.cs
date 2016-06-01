namespace Com.Pinz.Client.Module.TaskManager.Models.Category
{
    public class CategoryModel : DomainModel.Category
    {
        private ProjectModel projectModel;

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
    }
}