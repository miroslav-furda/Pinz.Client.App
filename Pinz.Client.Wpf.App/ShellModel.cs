using Com.Pinz.Client.Commons;
using Com.Pinz.Client.Commons.Model;
using Com.Pinz.Client.Model;
using Ninject;
using Prism.Commands;
using Prism.Regions;
using System;

namespace Com.Pinz.Client.Wpf.App
{
    public class ShellModel
    {
        private static Uri AdministrationViewUri = new Uri("AdministrationMainView", UriKind.Relative);
        private static Uri PinzProjectsTabViewUri = new Uri("PinzProjectsTabView", UriKind.Relative);

        public ApplicationGlobalModel GlobalModel { get; private set; }
        public DelegateCommand<bool?> AdminButtonClick { get; private set; }
        public TaskFilter Filter { get; private set; }

        private IRegionManager RegionManager;

        [Inject]
        public ShellModel(ApplicationGlobalModel globalModel, IRegionManager regionManager, TaskFilter filter)
        {
            this.GlobalModel = globalModel;
            this.RegionManager = regionManager;
            this.Filter = filter;

            AdminButtonClick = new DelegateCommand<bool?>(OnAdminButtonClick);

        }

        private void OnAdminButtonClick(bool? isChecked)
        {
            if (true == isChecked)
                this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, AdministrationViewUri);
            else
                this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, PinzProjectsTabViewUri);
        }
    }
}
