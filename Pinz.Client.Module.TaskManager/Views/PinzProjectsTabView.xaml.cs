using Com.Pinz.Client.Module.TaskManager.Models;
using Common.Logging;
using Ninject;
using System.Windows.Controls;

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
