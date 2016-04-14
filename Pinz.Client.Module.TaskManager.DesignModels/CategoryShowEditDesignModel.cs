using Com.Pinz.Client.DomainModel;

namespace Com.Pinz.Client.Module.TaskManager.DesignModels
{
    public class CategoryShowEditDesignModel
    {
        public Category Category { get; set; }

        public bool IsEditorEnabled { get; set; }

        public CategoryShowEditDesignModel()
        {
            IsEditorEnabled = false;
            Category = new Category()
            {
                Name = "Category"
            };
        }
    }
}
