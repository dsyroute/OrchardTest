using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace YRoute.YWiki.Api.Domain;

public class WikiArticle
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;

    public string CategoryId { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];

    /// <summary>Internal links to other wiki articles (by slug).</summary>
    public List<string> RelatedSlugs { get; set; } = [];

    /// <summary>Revision history (last N revisions kept). Uses MongoDB document model.</summary>
    public List<WikiRevision> Revisions { get; set; } = [];

    public int CurrentRevisionNumber { get; set; }

    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
