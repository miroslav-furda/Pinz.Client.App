﻿using Com.Pinz.Client.DomainModel;
using Ninject;
using Prism.Regions;
using System.Collections.ObjectModel;
using Com.Pinz.Client.RemoteServiceConsumer.Service;
using Com.Pinz.Client.Model;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class PinzProjectsTabModel : INavigationAware
    {
        private static readonly ILog Log = LogManager.GetLogger<PinzProjectsTabModel>();

        public ObservableCollection<ProjectModel> Projects { get; private set; }
        private ITaskRemoteService taskService;

        [Inject]
        public PinzProjectsTabModel(ITaskRemoteService taskService, ApplicationGlobalModel globalModel)
        {
            Log.Debug("Constructor");
            this.taskService = taskService;
            Projects = new ObservableCollection<ProjectModel>();
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            Log.Debug("OnNavigatedTo called ...");
            var projects = await taskService.ReadAllProjectsForCurrentUserAsync();
            Log.DebugFormat("OnNavigatedTo projects loaded from remote. Count: {0}", projects.Count);
            Projects.Clear();
            Log.Debug("OnNavigatedTo Projects cleared");            
            foreach (var project in projects)
            {
                Projects.Add(new ProjectModel(project));
            }            
            Log.DebugFormat("OnNavigatedTo called Projects populated. Count: {0}", Projects.Count);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            Log.Debug("IsNavigationTarget executed ... returning true");
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //do nothing
        }
    }
}
