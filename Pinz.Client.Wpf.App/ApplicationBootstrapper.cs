﻿using AutoMapper;
using Com.Pinz.Client.Commons.Model;
using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.Model;
using Com.Pinz.Client.Module.Administration;
using Com.Pinz.Client.Module.Login;
using Com.Pinz.Client.Module.Main;
using Com.Pinz.Client.Module.TaskManager;
using Com.Pinz.Client.RemoteServiceConsumer;
using Ninject;
using Ninject.Activation;
using Prism.Modularity;
using Prism.Ninject;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace Com.Pinz.Client.Wpf.App
{
    public class ApplicationBootstrapper : NinjectBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Kernel.Get<Shell>();
        }

        protected override void InitializeShell()
        {
            //set current culture
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            Application.Current.MainWindow = (Window)this.Shell;
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
            moduleCatalog.AddModule(typeof(MainModule));
            moduleCatalog.AddModule(typeof(LoginModule));
            moduleCatalog.AddModule(typeof(TaskManagerModule));
            moduleCatalog.AddModule(typeof(AdministrationModule));
        }

        protected override void ConfigureKernel()
        {
            base.ConfigureKernel();

            Kernel.Load(new MainNinjectModule());
            Kernel.Load(new AdministrationNinjectModule());
            Kernel.Load(new TaskManagerNinjectModule());
            Kernel.Load(new ServiceConsumerNinjectModule());
            Kernel.Load(new LoginNinjectModule());

            Kernel.Bind<TaskFilter>().ToSelf().InSingletonScope();
            Kernel.Bind<ApplicationGlobalModel>().ToSelf().InSingletonScope();

            Kernel.Bind<IMapper>().ToMethod(StartAutoMapper).InSingletonScope().Named("WpfClientMapper");
        }

        private IMapper StartAutoMapper(IContext arg)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Task, Task>();
                cfg.CreateMap<User, User>();
                cfg.CreateMap<User, ProjectUser>();
            });

            return config.CreateMapper();
        }
    }
}
