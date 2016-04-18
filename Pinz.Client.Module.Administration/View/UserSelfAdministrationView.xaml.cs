using Com.Pinz.Client.Module.Administration.Model;
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

namespace Com.Pinz.Client.Module.Administration.View
{
    /// <summary>
    /// Interaction logic for UserSelfAdministrationView.xaml
    /// </summary>
    public partial class UserSelfAdministrationView : UserControl
    {
        [Inject]
        public UserSelfAdministrationView(UserSelfAdministrationModel model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
