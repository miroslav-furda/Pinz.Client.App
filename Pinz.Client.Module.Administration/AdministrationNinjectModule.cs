using Com.Pinz.Client.Module.Administration.View;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
