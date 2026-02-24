namespace YRoute.YWiki.Api.Dtos;

public class WikiArticleDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
    public List<string> RelatedSlugs { get; set; } = [];
    public int CurrentRevisionNumber { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateWikiArticleDto
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
    public List<string> RelatedSlugs { get; set; } = [];
    public bool IsPublished { get; set; }
    public string EditedBy { get; set; } = "system";
    public string? ChangeNote { get; set; }
}

public class UpdateWikiArticleDto : CreateWikiArticleDto { }

public class WikiArticleFilter
{
    public string? CategoryId { get; set; }
    public string? Tag { get; set; }
    public string? Search { get; set; }
    public bool? IsPublished { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class WikiRevisionDto
{
    public int RevisionNumber { get; set; }
    public string Content { get; set; } = string.Empty;
    public string EditedBy { get; set; } = string.Empty;
    public string? ChangeNote { get; set; }
    public DateTime EditedAt { get; set; }
}
