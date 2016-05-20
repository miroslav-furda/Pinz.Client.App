using Com.Pinz.Client.Module.Administration.View;
using Ninject.Modules;

namespace Com.Pinz.Client.Module.Administration
{
    public class AdministrationNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<object>().To<AdministrationMainView>().Named("AdministrationMainView");
        }
    }
}
