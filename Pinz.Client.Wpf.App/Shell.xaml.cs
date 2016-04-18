using Com.Pinz.Client.Commons;
using Com.Pinz.Client.Model.Service;
using Ninject;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Windows;

namespace Com.Pinz.Client.Wpf.App
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window
    {
        private const string LoginModuleName = "LoginModule";
        private static Uri LoginViewUri = new Uri("/LoginView", UriKind.Relative);
        private static Uri AdministrationViewUri = new Uri("/AdministrationMainView", UriKind.Relative);

        private IModuleManager ModuleManager;
        private IRegionManager RegionManager;

        [Inject]
        public Shell(IAdminClientService model, IModuleManager ModuleManager, IRegionManager RegionManager)
        {
            this.ModuleManager = ModuleManager;
            this.RegionManager = RegionManager;
            InitializeComponent();
            DataContext = model;
        }


        public void OnImportsSatisfied()
        {
            this.ModuleManager.LoadModuleCompleted +=
                (s, e) =>
                {
                    // todo: 01 - Navigation on when modules are loaded.
                    // When using region navigation, be sure to use it consistently
                    // to ensure you get proper journal behavior.  If we mixed
                    // usage of adding views directly to regions, such as through
                    // RegionManager.AddToRegion, and then use RegionManager.RequestNavigate,
                    // we may not be able to navigate back correctly.
                    // 
                    // Here, we wait until the module we want to start with is
                    // loaded and then navigate to the view we want to display
                    // initially.
                    //     
                    if (e.ModuleInfo.ModuleName == LoginModuleName)
                    {
                        this.RegionManager.RequestNavigate(
                            RegionNames.MainContentRegion,
                            LoginViewUri);
                    }
                };
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            this.RegionManager.RequestNavigate( RegionNames.MainContentRegion, AdministrationViewUri);
        }
    }
}
