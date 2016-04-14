using AutoMapper;
using Com.Pinz.Client.DomainModel;
using Ninject.Activation;
using Ninject.Modules;

namespace Com.Pinz.Client.Module.TaskManager
{
    public class TaskManagerNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IMapper>().ToMethod(StartAutoMapper).InSingletonScope().Named("WpfClientMapper");
        }

        private IMapper StartAutoMapper(IContext arg)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Task,Task>();
            });

            return config.CreateMapper();
        }
    }
}
