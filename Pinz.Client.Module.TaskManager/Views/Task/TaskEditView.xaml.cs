using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.Module.TaskManager.Models;
using Ninject;
using Prism.Regions;
using System.Windows.Controls;

namespace Com.Pinz.Client.Module.TaskManager.Views
{
    /// <summary>
    /// Interaction logic for TaskEditView.xaml
    /// </summary>
    public partial class TaskEditView : UserControl
    {
        [Inject]
        public TaskEditView(TaskEditModel model)
        {
            InitializeComponent();
            this.DataContext = model;

            RegionContext.GetObservableContext(this).PropertyChanged += (s, e) =>
            {
                if (RegionContext.GetObservableContext(this).Value != null)
                    model.Task = RegionContext.GetObservableContext(this).Value as Task;
            };
        }
    }
}
