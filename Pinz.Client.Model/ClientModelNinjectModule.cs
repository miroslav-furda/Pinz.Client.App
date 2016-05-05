using Ninject.Modules;
using Com.Pinz.Client.Model.Service;
using Com.Pinz.Client.Model.Remote;
using AutoMapper;
using Ninject.Activation;
using Com.Pinz.Client.DomainModel;

namespace Com.Pinz.Client.Model
{
    public class ClientModelNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<TaskFilter>().ToSelf().InSingletonScope();
            Kernel.Bind<ApplicationGlobalModel>().ToSelf().InSingletonScope();
            Kernel.Bind<ITaskClientService>().To<TaskClientServiceRemote>().InSingletonScope();
            Kernel.Bind<IAdminClientService>().To<AdminClientServiceRemote>().InSingletonScope();

            Kernel.Bind<IMapper>().ToMethod(StartAutoMapper).InSingletonScope().Named("WpfClientMapper");
        }

        private IMapper StartAutoMapper(IContext arg)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Task, Task>();
                cfg.CreateMap<User, User>();
            });

            return config.CreateMapper();
        }
    }
}
