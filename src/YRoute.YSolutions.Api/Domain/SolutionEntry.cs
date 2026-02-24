using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace YRoute.YSolutions.Api.Domain;

public class SolutionEntry
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    /// <summary>Category references (embedded slug + name for read performance).</summary>
    public List<string> CategoryIds { get; set; } = [];

    public List<string> Tags { get; set; } = [];

    /// <summary>Orchard Core media paths or external URLs.</summary>
    public List<string> ImageUrls { get; set; } = [];

    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;

    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
