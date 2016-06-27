using Com.Pinz.Client.Commons;
using Com.Pinz.Client.Model;
using Ninject;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Controls.Ribbon;

namespace Com.Pinz.Client.Wpf.App
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window
    {
        private static Uri AdministrationViewUri = new Uri("AdministrationMainView", UriKind.Relative);
        private static Uri PinzProjectsTabViewUri = new Uri("PinzProjectsTabView", UriKind.Relative);
        private IRegionManager RegionManager;

        [Inject]
        public Shell(ApplicationGlobalModel model, IRegionManager RegionManager)
        {
            this.RegionManager = RegionManager;
            InitializeComponent();
            DataContext = model;
        }


        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            RibbonToggleButton button = sender as RibbonToggleButton;
            if (button.IsChecked == true)
                this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, AdministrationViewUri);
            else
                this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, PinzProjectsTabViewUri);
        }
    }
}
