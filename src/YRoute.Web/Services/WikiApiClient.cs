using YRoute.Shared.Contracts;

namespace YRoute.Web.Services;

public class WikiArticleSummary
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
    public List<string> RelatedSlugs { get; set; } = [];
    public DateTime UpdatedAt { get; set; }
}

public class WikiArticleDetail : WikiArticleSummary
{
    public string Content { get; set; } = string.Empty;
    public int CurrentRevisionNumber { get; set; }
}

public class WikiApiClient : ApiClientBase
{
    public WikiApiClient(HttpClient http) : base(http) { }

    public Task<PagedResult<WikiArticleSummary>?> GetArticlesAsync(int page = 1, int pageSize = 20, string? search = null, string? categoryId = null, CancellationToken ct = default)
    {
        var url = $"api/wiki-articles?page={page}&pageSize={pageSize}&isPublished=true";
        if (!string.IsNullOrWhiteSpace(search)) url += $"&search={Uri.EscapeDataString(search)}";
        if (!string.IsNullOrWhiteSpace(categoryId)) url += $"&categoryId={Uri.EscapeDataString(categoryId)}";
        return GetPagedAsync<WikiArticleSummary>(url, ct);
    }

    public Task<WikiArticleDetail?> GetArticleBySlugAsync(string slug, CancellationToken ct = default)
        => GetAsync<WikiArticleDetail>($"api/wiki-articles/by-slug/{Uri.EscapeDataString(slug)}", ct);
}
