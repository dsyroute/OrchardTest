using YRoute.Shared.Contracts;

namespace YRoute.Web.Services;

public class SolutionSummary
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
    public List<string> ImageUrls { get; set; } = [];
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SolutionDetail : SolutionSummary
{
    public string Content { get; set; } = string.Empty;
    public List<string> CategoryIds { get; set; } = [];
    public string SeoTitle { get; set; } = string.Empty;
    public string SeoDescription { get; set; } = string.Empty;
}

public class SolutionsApiClient : ApiClientBase
{
    public SolutionsApiClient(HttpClient http) : base(http) { }

    public Task<PagedResult<SolutionSummary>?> GetSolutionsAsync(int page = 1, int pageSize = 20, string? search = null, string? categoryId = null, CancellationToken ct = default)
    {
        var url = $"api/solutions?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search)) url += $"&search={Uri.EscapeDataString(search)}";
        if (!string.IsNullOrWhiteSpace(categoryId)) url += $"&categoryId={Uri.EscapeDataString(categoryId)}";
        url += "&isPublished=true";
        return GetPagedAsync<SolutionSummary>(url, ct);
    }

    public Task<SolutionDetail?> GetSolutionBySlugAsync(string slug, CancellationToken ct = default)
        => GetAsync<SolutionDetail>($"api/solutions/by-slug/{Uri.EscapeDataString(slug)}", ct);

    public Task<SolutionDetail?> GetSolutionByIdAsync(string id, CancellationToken ct = default)
        => GetAsync<SolutionDetail>($"api/solutions/{id}", ct);
}
