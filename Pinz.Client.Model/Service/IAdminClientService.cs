using Com.Pinz.Client.DomainModel;

namespace Com.Pinz.Client.Model.Service
{
    public interface IAdminClientService
    {
        User CurrentUser { get; }
        bool IsUserLoggedIn { get; }

        bool loginUser(string email, string password);
        void ChangePasswordForUser(User currentUser, string oldPasword, string newPasword, string newPasword2);
    }
}
