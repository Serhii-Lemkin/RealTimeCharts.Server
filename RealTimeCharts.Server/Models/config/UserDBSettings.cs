using TalkBack.Models;

namespace RealTimeCharts.Server.Models.config
{
    public class UsersDBSettings : IUsersDBSettings
    {
        public string DataBaseName { get; set; } = "";
        public string ConnectionString { get; set; } = "";
        public string CollectionName { get; set; } = "";
    }
}
