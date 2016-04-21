using Com.Pinz.Client.Module.Administration.Model;
using Ninject;
using System.Windows.Controls;

namespace Com.Pinz.Client.Module.Administration.View
{
    /// <summary>
    /// Interaction logic for ProjectAdministrationView.xaml
    /// </summary>
    public partial class ProjectAdministrationView : UserControl
    {
        [Inject]
        public ProjectAdministrationView(ProjectAdministrationModel model)
        {
            InitializeComponent();
            DataContext = model;
        }

        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
