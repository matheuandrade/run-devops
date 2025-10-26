using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Shopping.Api.Models;

public class Notification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; }

    [BsonElement("message")]
    public string Message { get; set; }


    [BsonElement("email")]
    public string Email { get; set; }
}
