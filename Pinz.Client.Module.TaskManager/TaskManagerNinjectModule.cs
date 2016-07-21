using Com.Pinz.Client.Module.TaskManager.Models;
using Com.Pinz.Client.Module.TaskManager.Views;
using Common.Logging;
using Ninject.Modules;

namespace Com.Pinz.Client.Module.TaskManager
{
    public class TaskManagerNinjectModule : NinjectModule
    {
        private static readonly ILog Log = LogManager.GetLogger<TaskManagerNinjectModule>();

        public override void Load()
        {
            Log.Info("Loading TaskManagerNinjectModule ...");

            Kernel.Bind<object>().To<PinzProjectsTabView>().Named("PinzProjectsTabView");
            //Kernel.Bind<PinzProjectsTabModel>().ToSelf().InSingletonScope();
        }
    }
}
