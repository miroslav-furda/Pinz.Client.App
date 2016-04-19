using Com.Pinz.Client.Commons.Wpf.Extensions;

namespace Com.Pinz.Client.Module.Administration.Model
{
    public class CompanyAdministrationModel
    {
        public TabModel TabModel { get; private set; }

        public CompanyAdministrationModel()
        {
            TabModel = new TabModel()
            {
                Title = Properties.Resources.AdministrationTab_Title_Company,
                CanClose = false,
                IsModified = false
            };
        }
    }
}
