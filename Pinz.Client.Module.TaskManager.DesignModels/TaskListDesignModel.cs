
using Com.Pinz.Client.DomainModel;

namespace Com.Pinz.Client.Module.TaskManager.DesignModels
{
    public class TaskListDesignModel
    {
        public Category Category { get; set; }


        public TaskListDesignModel()
        {
            Category = new Category()
            {
                Name = "Category"
            };
        }
    }
}
