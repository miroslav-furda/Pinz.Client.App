using Com.Pinz.Client.Commons.Wpf.Extensions;

namespace Com.Pinz.Client.Module.Administration.Model
{
    public class ProjectAdministrationModel
    {
        public TabModel TabModel { get; private set; }

        public ProjectAdministrationModel()
        {
            TabModel = new TabModel()
            {
                Title = Properties.Resources.AdministrationTab_Title_Project,
                CanClose = false,
                IsModified = false
            };
        }
    }
}
