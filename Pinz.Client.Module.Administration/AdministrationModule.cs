using Com.Pinz.Client.Commons;
using Ninject;
using Prism.Modularity;
using Prism.Regions;

namespace Com.Pinz.Client.Module.Administration
{
    public class AdministrationModule : IModule
    {
        private readonly IRegionViewRegistry regionViewRegistry;

        [Inject]
        public AdministrationModule(IRegionViewRegistry registry)
        {
            this.regionViewRegistry = registry;
        }

        public void Initialize()
        {
            regionViewRegistry.RegisterViewWithRegion("UserSelfAdministrationRegion", typeof(View.UserSelfAdministrationView));
            regionViewRegistry.RegisterViewWithRegion("ProjectAdministrationRegion", typeof(View.ProjectAdministrationView));
            regionViewRegistry.RegisterViewWithRegion("CompanyAdministrationRegion", typeof(View.CompanyAdministrationView));

        }
    }
}
