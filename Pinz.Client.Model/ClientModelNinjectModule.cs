using Ninject.Modules;
using Com.Pinz.Client.Model.Service;
using Com.Pinz.Client.Model.Remote;

namespace Com.Pinz.Client.Model
{
    public class ClientModelNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<TaskFilter>().ToSelf().InSingletonScope();
            Kernel.Bind<ITaskClientService>().To<TaskClientServiceRemote>().InSingletonScope();
        }
    }
}
