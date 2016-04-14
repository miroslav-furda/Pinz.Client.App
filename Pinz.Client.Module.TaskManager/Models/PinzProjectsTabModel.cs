using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.Model.Service;
using Ninject;
using System.Collections.ObjectModel;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class PinzProjectsTabModel
    {
        public ObservableCollection<Project> Projects { get; private set; }

        [Inject]
        public PinzProjectsTabModel(ITaskClientService taskService)
        {
            Projects = taskService.Projects;
        }
    }
}
