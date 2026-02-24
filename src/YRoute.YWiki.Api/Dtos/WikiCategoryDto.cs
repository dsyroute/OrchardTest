namespace YRoute.YWiki.Api.Dtos;

public class WikiCategoryDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ParentId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateWikiCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ParentId { get; set; }
}
