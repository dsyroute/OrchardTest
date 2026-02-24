using MongoDB.Driver;
using YRoute.YWiki.Api.Domain;

namespace YRoute.YWiki.Api.Infrastructure;

public interface IWikiCategoryRepository
{
    Task<IReadOnlyList<WikiCategory>> GetAllAsync(CancellationToken ct = default);
    Task<WikiCategory?> GetByIdAsync(string id, CancellationToken ct = default);
    Task CreateAsync(WikiCategory category, CancellationToken ct = default);
    Task<bool> UpdateAsync(string id, WikiCategory category, CancellationToken ct = default);
    Task<bool> DeleteAsync(string id, CancellationToken ct = default);
}

public class WikiCategoryRepository : IWikiCategoryRepository
{
    private readonly IMongoCollection<WikiCategory> _collection;

    public WikiCategoryRepository(IMongoDatabase database)
        => _collection = database.GetCollection<WikiCategory>("wiki_categories");

    public async Task<IReadOnlyList<WikiCategory>> GetAllAsync(CancellationToken ct = default)
        => await _collection.Find(Builders<WikiCategory>.Filter.Empty).SortBy(x => x.Name).ToListAsync(ct);

    public async Task<WikiCategory?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task CreateAsync(WikiCategory category, CancellationToken ct = default)
        => await _collection.InsertOneAsync(category, cancellationToken: ct);

    public async Task<bool> UpdateAsync(string id, WikiCategory category, CancellationToken ct = default)
    {
        var result = await _collection.ReplaceOneAsync(x => x.Id == id, category, cancellationToken: ct);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == id, ct);
        return result.DeletedCount > 0;
    }
}
