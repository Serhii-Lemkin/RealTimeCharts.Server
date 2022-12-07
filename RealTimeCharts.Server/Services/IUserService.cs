using RealTimeCharts.Server.Models;

namespace RealTimeCharts.Server.Services
{
    public interface IUserService
    {
        List<User> Get();
        User GetUser(string id);
        User GetByUserName(string username);
        User AddNew(User user);
        void UpdateUser(string id, User user);
        void DeleteUser(string id);
        bool CheckIfAvailable(string username);
    }
}
