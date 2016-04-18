using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.Model.Service;
using Ninject;
using Prism.Regions;
using System.Collections.ObjectModel;
using System;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class PinzProjectsTabModel : INavigationAware
    {
        public ObservableCollection<Project> Projects { get; private set; }
        private ITaskClientService taskService;

        [Inject]
        public PinzProjectsTabModel(ITaskClientService taskService)
        {
            this.taskService = taskService;
            Projects = taskService.Projects;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            taskService.ReadAllProjectsForCurrentUser();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}
