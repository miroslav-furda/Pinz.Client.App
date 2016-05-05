using Com.Pinz.Client.DomainModel;
using Ninject;
using Prism.Regions;
using System.Collections.ObjectModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.Client.Model;
using System.Collections.Generic;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class PinzProjectsTabModel : INavigationAware
    {
        public ObservableCollection<Project> Projects { get; private set; }
        private ITaskRemoteService taskService;

        [Inject]
        public PinzProjectsTabModel(ITaskRemoteService taskService, ApplicationGlobalModel globalModel)
        {
            this.taskService = taskService;
            Projects = new ObservableCollection<Project>();
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            List<Project> projects = await System.Threading.Tasks.Task.Run(() => taskService.ReadAllProjectsForCurrentUser());
            Projects.Clear();
            projects.ForEach(Projects.Add);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //do nothing
        }
    }
}
