using Com.Pinz.Client.DomainModel;

namespace Com.Pinz.Client.RemoteServiceConsumer.Service
{
    public interface IAuthorisationRemoteService
    {
        bool IsUserProjectAdmin(User user, Project project);

        bool IsUserComapnyAdmin(User user);

        System.Threading.Tasks.Task<User> ReadUserByEmail(string email);
    }
}
