using Ninject;
using System.Windows;

namespace Com.Pinz.Client.Wpf.App
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window
    {
        [Inject]
        public Shell(ShellModel model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
