using Com.Pinz.Client.Commons;
using Com.Pinz.Client.Model;
using Ninject;
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
        private static Uri AdministrationViewUri = new Uri("/AdministrationMainView", UriKind.Relative);
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
            this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, AdministrationViewUri);
        }
    }
}
