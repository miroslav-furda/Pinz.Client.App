using Com.Pinz.Client.Commons;
using Ninject;
using Prism.Modularity;
using Prism.Regions;

namespace Com.Pinz.Client.Module.Administration
{
    public class AdministrationModule : IModule
    {
        private readonly IRegionViewRegistry regionViewRegistry;
        private readonly static string USER_SELF_ADMINISTRATION_REGION = "UserSelfAdministrationRegion";

        [Inject]
        public AdministrationModule(IRegionViewRegistry registry)
        {
            this.regionViewRegistry = registry;
        }

        public void Initialize()
        {
            regionViewRegistry.RegisterViewWithRegion(USER_SELF_ADMINISTRATION_REGION, typeof(View.UserSelfAdministrationView));
            regionViewRegistry.RegisterViewWithRegion(USER_SELF_ADMINISTRATION_REGION, typeof(View.ProjectAdministrationView));
            regionViewRegistry.RegisterViewWithRegion(USER_SELF_ADMINISTRATION_REGION, typeof(View.CompanyAdministrationView));

        }
    }
}
