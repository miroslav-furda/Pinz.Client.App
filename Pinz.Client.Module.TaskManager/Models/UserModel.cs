using Com.Pinz.Client.DomainModel;
using Com.Pinz.Client.Module.TaskManager.Components.AutoCompleteCombo;

namespace Com.Pinz.Client.Module.TaskManager.Models
{
    public class UserModel : User, IComboBoxProperties
    {
        public UserModel()
        {
        }

        public UserModel(User user)
        {
            EMail = user.EMail;
            FirstName = user.FirstName;
            FamilyName = user.FamilyName;
            IsCompanyAdmin = user.IsCompanyAdmin;
            CompanyId = user.CompanyId;
            UserId = user.UserId;
        }

        public object Id => UserId;

        public string Name => EMail;
    }
}