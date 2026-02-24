namespace YRoute.YBlog.Api.Dtos;

public class BlogPostDto
{
    public string Id { get; set; } = string.Empty;
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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateBlogPostDto
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public List<string> CategoryIds { get; set; } = [];
    public List<string> Tags { get; set; } = [];
    public string? CoverImageUrl { get; set; }
    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
}

public class UpdateBlogPostDto : CreateBlogPostDto { }

public class BlogPostFilter
{
    public string? CategoryId { get; set; }
    public string? Tag { get; set; }
    public string? AuthorId { get; set; }
    public string? Search { get; set; }
    public bool? IsPublished { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
