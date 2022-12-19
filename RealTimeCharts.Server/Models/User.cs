using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace RealTimeCharts.Server.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; }
        [BsonElement("username")]
        public string UserName { get; set; }

        [BsonElement("lastRequest")]
        public DateTime LastRequest { get; set; }
        [BsonElement("personalCode")]
        public string PersonalCode { get; set; }
        [BsonElement("passwordHash")]
        public byte[] PasswordHash { get; set; }
        [BsonElement("passwordSalt")]
        public byte[] PasswordSalt { get; set; }
        
    }
}
