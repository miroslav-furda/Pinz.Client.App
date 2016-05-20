using Ninject.Modules;

namespace Com.Pinz.Client.Module.Login
{
    public class LoginNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<object>().To<View.LoginView>().Named("LoginView");
        }
    }
}
