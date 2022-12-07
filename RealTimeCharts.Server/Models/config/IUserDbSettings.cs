namespace TalkBack.Models
{
    public interface IUsersDBSettings
    {
        string DataBaseName { get; set; }
        string CollectionName { get; set; }
        string ConnectionString { get; set; }
    }
}
