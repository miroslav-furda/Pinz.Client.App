using Com.Pinz.Client.DomainModel;

namespace Com.Pinz.Client.Model.Service
{
    public interface IAdminClientService
    {
        User CurrentUser { get; }
        bool IsUserLoggedIn { get; }

        bool loginUser(string email, string password);
        bool ChangePasswordForUser(User currentUser, string oldPasword, string newPasword, string newPasword2);

        void UpdateUser(User user);
    }
}
