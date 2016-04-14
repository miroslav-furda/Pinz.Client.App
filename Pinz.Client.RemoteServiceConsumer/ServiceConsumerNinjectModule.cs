using AutoMapper;
using Ninject.Activation;
using Ninject.Modules;
using System.ServiceModel;
using Ninject;
using Ninject.Extensions.Interception.Infrastructure.Language;
using System.Diagnostics;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.Client.RemoteServiceConsumer.ServiceImpl;
using Com.Pinz.Client.RemoteServiceConsumer.Infrastructure;
using Com.Pinz.Client.DomainModel;

namespace Com.Pinz.Client.RemoteServiceConsumer
{
    public class ServiceConsumerNinjectModule : NinjectModule
    {
        private readonly static TraceSource Tracer = new TraceSource("Com.Pinz.Client.ServiceConsumer.Services");

        public override void Load()
        {
            Tracer.TraceInformation("ServiceConsumerNinjectModule loading ... ");
            Kernel.Bind<IMapper>().ToMethod(StartAutoMapper).InSingletonScope().Named("ServiceConsumerMapper");

            Kernel.Bind<ChannelFactory<AdministrationServiceReference.IAdministrationService>>().ToSelf().InSingletonScope().WithConstructorArgument("WSHttpBinding_IAdministrationService");
            Kernel.Bind<ChannelFactory<TaskServiceReference.ITaskService>>().ToSelf().InSingletonScope().WithConstructorArgument("WSHttpBinding_ITaskService");
            Kernel.Bind<ChannelFactory<AuthorisationServiceReference.IAuthorisationService>>().ToSelf().InSingletonScope().WithConstructorArgument("WSHttpBinding_IAuthorisationService");
            Kernel.Bind<ChannelFactory<PinzAdminServiceReference.IPinzAdminService>>().ToSelf().InSingletonScope().WithConstructorArgument("WSHttpBinding_IPinzAdminService");

            Kernel.Bind<UserNameClientCredentials>().ToSelf().InSingletonScope();

            Kernel.Bind<ITaskRemoteService>().To<TaskService>().InSingletonScope().Intercept().With<ChannelFactoryInterceptor>();
            Kernel.Bind<IAdministrationRemoteService>().To<AdministrationService>().InSingletonScope().Intercept().With<ChannelFactoryInterceptor>();
            Kernel.Bind<IAuthorisationRemoteService>().To<AuthorisationService>().InSingletonScope().Intercept().With<ChannelFactoryInterceptor>();
            Kernel.Bind<IPinzAdminRemoteService>().To<PinzAdminService>().InSingletonScope().Intercept().With<ChannelFactoryInterceptor>();

        }

        private IMapper StartAutoMapper(IContext arg)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TaskServiceReference.CategoryDO, Category>();
                cfg.CreateMap<Category, TaskServiceReference.CategoryDO>();
                cfg.CreateMap<TaskServiceReference.ProjectDO, Project>();
                cfg.CreateMap<Project, TaskServiceReference.ProjectDO>();
                cfg.CreateMap<TaskServiceReference.TaskDO, Task>();
                cfg.CreateMap<Task, TaskServiceReference.TaskDO>();

                cfg.CreateMap<AdministrationServiceReference.CompanyDO, Company>();
                cfg.CreateMap<Company, AdministrationServiceReference.CompanyDO>();
                cfg.CreateMap<AdministrationServiceReference.ProjectDO, Project>();
                cfg.CreateMap<Project, AdministrationServiceReference.ProjectDO>();

                cfg.CreateMap<AdministrationServiceReference.UserDO, User>();
                cfg.CreateMap<User, AdministrationServiceReference.UserDO>();

                cfg.CreateMap<AuthorisationServiceReference.UserDO, User>();
                cfg.CreateMap<User, AuthorisationServiceReference.UserDO>();

                cfg.CreateMap<PinzAdminServiceReference.CompanyDO, Company>();
                cfg.CreateMap<Company, PinzAdminServiceReference.CompanyDO>();
            });

            return config.CreateMapper();
        }
    }
}
