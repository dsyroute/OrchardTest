namespace YRoute.YSolutions.Api.Dtos;

public class SolutionEntryDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<string> CategoryIds { get; set; } = [];
    public List<string> Tags { get; set; } = [];
    public List<string> ImageUrls { get; set; } = [];
    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateSolutionEntryDto
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<string> CategoryIds { get; set; } = [];
    public List<string> Tags { get; set; } = [];
    public List<string> ImageUrls { get; set; } = [];
    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
}

public class UpdateSolutionEntryDto : CreateSolutionEntryDto { }

public class SolutionEntryFilter
{
    public string? CategoryId { get; set; }
    public string? Tag { get; set; }
    public string? Search { get; set; }
    public bool? IsPublished { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
