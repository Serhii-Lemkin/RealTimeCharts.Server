using MongoDB.Driver;
using RealTimeCharts.Server.Models;
using TalkBack.Models;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealTimeCharts.Server.Services
{

    public class UserService : IUserService
    {
        private readonly MongoClient _client;
        readonly IMongoCollection<User> _users;

        public UserService(IUsersDBSettings settings, IMongoClient mongoClient)
        {
            _client = new MongoClient(settings.ConnectionString);
            _users = _client
                .GetDatabase(settings.DataBaseName)
                .GetCollection<User>(settings.CollectionName);
        }
        public User AddNew(User user)
        {
            _users.InsertOneAsync(user);
            return user;
        }

        public void DeleteUser(string id) =>
            _users.DeleteOne(user => user.Id == id);

        public List<User> Get() =>
            _users.Find(user => true).ToList();

        public User GetUser(string id) =>
            _users.Find(user => user.Id == id).FirstOrDefault();

        public User GetByUserName(string username) =>
            _users.Find(user => user.UserName == username).FirstOrDefault();

        public void UpdateUser(string id, User user)
        => _users.ReplaceOne(user => user.Id == id, user);

        public bool CheckIfAvailable(string username)
         {
            var tmpUser = _users.Find(user => user.UserName == username).FirstOrDefault();
            if (tmpUser == null) return true; 
            if ((DateTime.Now - tmpUser.LastRequest).Minutes > 15)
            {
                _users.DeleteOne(user => user.Id == tmpUser.Id);
                return true;
            }
            return false;
        }
        public void UpdateLastRequest(User user)
        {
            user.LastRequest = DateTime.Now;
            _users.ReplaceOne(u => u.Id == user.Id, user);
        }

        public User GetUserByCode(string code)
        {
            return _users.Find(user => user.PersonalCode == code).FirstOrDefault();
        }
    }
}

