using Com.Pinz.Client.Commons;
using Ninject;
using Prism.Modularity;
using Prism.Regions;

namespace Com.Pinz.Client.Module.Login
{
    public class LoginModule : IModule
    {
        private readonly IRegionViewRegistry regionViewRegistry;

        [Inject]
        public LoginModule(IRegionViewRegistry registry)
        {
            this.regionViewRegistry = registry;
        }

        public void Initialize()
        {
        }
    }
}
