using Com.Pinz.Client.Module.Administration.Model;
using Ninject;
using System.Windows.Controls;

namespace Com.Pinz.Client.Module.Administration.View
{
    /// <summary>
    /// Interaction logic for CompanyAdministrationView.xaml
    /// </summary>
    public partial class CompanyAdministrationView : UserControl
    {
        [Inject]
        public CompanyAdministrationView(CompanyAdministrationModel model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
