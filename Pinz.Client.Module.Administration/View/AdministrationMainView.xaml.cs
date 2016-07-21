using Common.Logging;
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

namespace Com.Pinz.Client.Module.Administration.View
{
    /// <summary>
    /// Interaction logic for AdministrationMainView.xaml
    /// </summary>
    public partial class AdministrationMainView : UserControl, INavigationAware
    {
        private readonly ILog Log = LogManager.GetLogger<AdministrationMainView>();

        public AdministrationMainView()
        {
            InitializeComponent();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Log.Debug("On navigation from ...");
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Log.Debug("On navigation to ...");
        }
    }
}
