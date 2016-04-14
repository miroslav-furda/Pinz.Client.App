using Com.Pinz.Client.Model.Remote;
using Com.Pinz.Client.Model.Service;
using Com.Pinz.Client.Module.Login;
using Com.Pinz.Client.Module.TaskManager;
using Com.Pinz.Client.RemoteServiceConsumer;
using Com.Pinz.WpfClient.Module.Login;
using Ninject;
using Prism.Modularity;
using Prism.Ninject;
using System.Windows;

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
            Application.Current.MainWindow = (Window)this.Shell;
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
            moduleCatalog.AddModule(typeof(LoginModule));
            moduleCatalog.AddModule(typeof(TaskManagerModule));
        }

        protected override void ConfigureKernel()
        {
            base.ConfigureKernel();
            Kernel.Load(new TaskManagerNinjectModule());
            Kernel.Load(new ServiceConsumerNinjectModule());
            Kernel.Load(new LoginNinjectModule());

            Kernel.Bind<ITaskClientService>().To<TaskClientServiceRemote>().InSingletonScope();
        }
    }
}
