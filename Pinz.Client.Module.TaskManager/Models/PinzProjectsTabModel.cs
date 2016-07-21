using Com.Pinz.Client.DomainModel;
using Ninject;
using Prism.Regions;
using System.Collections.ObjectModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.Client.Model;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Prism.Mvvm;
using Prism.Events;
using System;
using Com.Pinz.Client.Commons.Event;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class PinzProjectsTabModel : BindableBase, INavigationAware
    {
        private static readonly ILog Log = LogManager.GetLogger<PinzProjectsTabModel>();

        public ObservableCollection<ProjectModel> Projects { get; private set; }

        private ProjectModel _selectedProject;
        public ProjectModel SelectedProject
        {
            get { return _selectedProject; }
            set { SetProperty(ref _selectedProject, value); }
        }

        private ITaskRemoteService _taskService;
        private readonly IEventAggregator _eventAggregator;

        [Inject]
        public PinzProjectsTabModel(ITaskRemoteService taskService, ApplicationGlobalModel globalModel, IEventAggregator eventAggregator)
        {
            Log.Debug("Constructor");
            this._taskService = taskService;
            this._eventAggregator = eventAggregator;
            Projects = new ObservableCollection<ProjectModel>();
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            try
            {
                Log.Debug("OnNavigatedTo called ...");
                var projects = await _taskService.ReadAllProjectsForCurrentUserAsync();
                Log.DebugFormat("OnNavigatedTo projects loaded from remote. Count: {0}", projects.Count);
                Projects.Clear();
                Log.Debug("OnNavigatedTo Projects cleared");
                foreach (var project in projects)
                {
                    Projects.Add(new ProjectModel(project));
                }
                SelectedProject = Projects.FirstOrDefault();
                Log.DebugFormat("OnNavigatedTo called Projects populated. Count: {0}", Projects.Count);
            }
            catch (TimeoutException timeoutEx)
            {
                _eventAggregator.GetEvent<TimeoutErrorEvent>().Publish(timeoutEx);
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            Log.Debug("IsNavigationTarget executed ... returning false");
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //do nothing
        }
    }
}
