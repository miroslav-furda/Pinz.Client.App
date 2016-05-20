using Ninject;
using Prism.Modularity;
using Prism.Regions;

namespace Com.Pinz.Client.Module.TaskManager
{
    public class TaskManagerModule : IModule
    {
        private readonly IRegionViewRegistry regionViewRegistry;

        [Inject]
        public TaskManagerModule(IRegionViewRegistry registry)
        {
            this.regionViewRegistry = registry;
        }

        public void Initialize()
        {
            regionViewRegistry.RegisterViewWithRegion("CategoryListRegion", typeof(Views.CategoryListView));
            regionViewRegistry.RegisterViewWithRegion("CategoryShowEditRegion", typeof(Views.CategoryShowEditView));
            regionViewRegistry.RegisterViewWithRegion("TaskListRegion", typeof(Views.TaskListView));
            regionViewRegistry.RegisterViewWithRegion("TaskShowEditRegion", typeof(Views.TaskShowEditView));
            regionViewRegistry.RegisterViewWithRegion("TaskEditRegion", typeof(Views.TaskEditView));
        }
    }
}
