using YRoute.Shared.Contracts;

namespace YRoute.Web.Services;

public class BlogPostSummary
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
    public string? CoverImageUrl { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BlogPostDetail : BlogPostSummary
{
    public string Content { get; set; } = string.Empty;
    public List<string> CategoryIds { get; set; } = [];
    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
}

public class BlogApiClient : ApiClientBase
{
    public BlogApiClient(HttpClient http) : base(http) { }

    public Task<PagedResult<BlogPostSummary>?> GetPostsAsync(int page = 1, int pageSize = 20, string? search = null, string? tag = null, CancellationToken ct = default)
    {
        var url = $"api/blog-posts?page={page}&pageSize={pageSize}&isPublished=true";
        if (!string.IsNullOrWhiteSpace(search)) url += $"&search={Uri.EscapeDataString(search)}";
        if (!string.IsNullOrWhiteSpace(tag)) url += $"&tag={Uri.EscapeDataString(tag)}";
        return GetPagedAsync<BlogPostSummary>(url, ct);
    }

    public Task<BlogPostDetail?> GetPostBySlugAsync(string slug, CancellationToken ct = default)
        => GetAsync<BlogPostDetail>($"api/blog-posts/by-slug/{Uri.EscapeDataString(slug)}", ct);
}
