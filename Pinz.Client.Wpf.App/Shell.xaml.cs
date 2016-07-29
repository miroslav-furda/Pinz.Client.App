using MahApps.Metro.Controls;
using Ninject;
using System.Windows;

namespace Com.Pinz.Client.Wpf.App
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : MetroWindow
    {
        [Inject]
        public Shell(ShellModel model)
        {
            InitializeComponent();
            DataContext = model;
        }
    }
}
