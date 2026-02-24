using System.Net.Http.Json;
using YRoute.Shared.Contracts;

namespace YRoute.Web.Services;

public abstract class ApiClientBase
{
    protected readonly HttpClient Http;

    protected ApiClientBase(HttpClient http)
    {
        Http = http;
    }

    protected async Task<T?> GetAsync<T>(string url, CancellationToken ct = default)
    {
        try
        {
            return await Http.GetFromJsonAsync<T>(url, ct);
        }
        catch (HttpRequestException)
        {
            return default;
        }
    }

    protected async Task<PagedResult<T>?> GetPagedAsync<T>(string url, CancellationToken ct = default)
        => await GetAsync<PagedResult<T>>(url, ct);
}
