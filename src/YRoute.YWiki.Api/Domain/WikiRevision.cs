namespace YRoute.YWiki.Api.Domain;

/// <summary>Lightweight revision stored as embedded document in WikiArticle.</summary>
public class WikiRevision
{
    public int RevisionNumber { get; set; }
    public string Content { get; set; } = string.Empty;
    public string EditedBy { get; set; } = string.Empty;
    public string? ChangeNote { get; set; }
    public DateTime EditedAt { get; set; } = DateTime.UtcNow;
}
