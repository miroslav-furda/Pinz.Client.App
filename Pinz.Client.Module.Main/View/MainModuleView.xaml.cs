﻿using Com.Pinz.Client.Commons;
using Com.Pinz.Client.Module.Main.Model;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Com.Pinz.Client.Module.Main.View
{
    /// <summary>
    /// Interaction logic for MainModuleView.xaml
    /// </summary>
    public partial class MainModuleView : UserControl
    {
        private const string LoginModuleName = "LoginModule";
        private static Uri LoginViewUri = new Uri("/LoginView", UriKind.Relative);

        private IModuleManager ModuleManager;
        private IRegionManager RegionManager;

        [Inject]
        public MainModuleView(MainModuleModel model,IModuleManager ModuleManager, IRegionManager RegionManager)
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
    }
}
