using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace YRoute.YSolutions.Api.Domain;

public class SolutionTag
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}
