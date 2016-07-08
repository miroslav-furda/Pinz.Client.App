
using Com.Pinz.Client.DomainModel;

namespace Com.Pinz.Client.Module.Administration.DesignModel
{
    internal class UserSelfAdministrationDesignModel
    {
        public User CurrentUser { get; private set; }
        public bool IsUserInEditMode { get; private set; }
        public bool IsPasswordInEditMode { get; private set; }

        public UserSelfAdministrationDesignModel()
        {
            IsUserInEditMode = false;
            IsPasswordInEditMode = true;

            CurrentUser = new User()
            {
                EMail = "test@pinzonline.com",
                FirstName = "John",
                FamilyName = "Smith"
            };
        }
    }
}
