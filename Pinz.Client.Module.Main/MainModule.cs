using Com.Pinz.Client.Commons;
using Prism.Modularity;
using Prism.Regions;

namespace Com.Pinz.Client.Module.Main
{
    public class MainModule : IModule
    {
        private readonly IRegionViewRegistry regionViewRegistry;

        [Inject]
        public MainModule(IRegionViewRegistry registry)
        {
            this.regionViewRegistry = registry;
        }

        public void Initialize()
        {
            regionViewRegistry.RegisterViewWithRegion(RegionNames.PinzMainRegion, typeof(View.MainModuleView));
        }
    }
}
