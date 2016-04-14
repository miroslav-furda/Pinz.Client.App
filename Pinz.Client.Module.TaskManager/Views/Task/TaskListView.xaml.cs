using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.Module.TaskManager.Models;
using Prism.Regions;
using System.Windows.Controls;

namespace Com.Pinz.Client.Module.TaskManager.Views
{
    /// <summary>
    /// Interaction logic for TaskListView.xaml
    /// </summary>
    public partial class TaskListView : UserControl
    {
        public TaskListView(TaskListModel model)
        {
            InitializeComponent();
            this.DataContext = model;

            RegionContext.GetObservableContext(this).PropertyChanged += (s, e) =>
            {
                if (RegionContext.GetObservableContext(this).Value != null)
                    model.Category = RegionContext.GetObservableContext(this).Value as Category;
            };
        }
    }
}
