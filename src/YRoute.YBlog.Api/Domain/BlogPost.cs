using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace YRoute.YBlog.Api.Domain;

public class BlogPost
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public string AuthorId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;

    public List<string> CategoryIds { get; set; } = [];
    public List<string> Tags { get; set; } = [];

    public string? CoverImageUrl { get; set; }

    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;

    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
