using Com.Pinz.Client.Module.TaskManager.Models;
using Common.Logging;
using Ninject;
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

namespace Com.Pinz.Client.Module.TaskManager.Views
{
    /// <summary>
    /// Interaction logic for PinzProjectsTabView.xaml
    /// </summary>
    public partial class PinzProjectsTabView : UserControl
    {
        private static readonly ILog Log = LogManager.GetLogger<PinzProjectsTabModel>();

        [Inject]
        public PinzProjectsTabView(PinzProjectsTabModel model)
        {
            Log.DebugFormat("Constructor with model {0}", model);
            InitializeComponent();
            DataContext = model;
        }
    }
}
