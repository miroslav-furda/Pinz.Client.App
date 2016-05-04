using Com.Pinz.Client.Module.Main.Model;
using Com.Pinz.Client.RemoteServiceConsumer.Callback;
using Ninject.Modules;

namespace Com.Pinz.Client.Module.Main
{
    public class MainNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IServiceRunningIndicator>().To<MainModuleModel>().InSingletonScope();
        }
    }
}
