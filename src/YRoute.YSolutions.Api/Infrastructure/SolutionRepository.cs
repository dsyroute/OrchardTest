using MongoDB.Driver;
using YRoute.YSolutions.Api.Domain;

namespace YRoute.YSolutions.Api.Infrastructure;

public interface ISolutionRepository
{
    Task<IReadOnlyList<SolutionEntry>> GetAllAsync(string? search, string? categoryId, string? tag, bool? isPublished, int skip, int take, CancellationToken ct = default);
    Task<long> CountAsync(string? search, string? categoryId, string? tag, bool? isPublished, CancellationToken ct = default);
    Task<SolutionEntry?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<SolutionEntry?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task CreateAsync(SolutionEntry entry, CancellationToken ct = default);
    Task<bool> UpdateAsync(string id, SolutionEntry entry, CancellationToken ct = default);
    Task<bool> DeleteAsync(string id, CancellationToken ct = default);
}

public class SolutionRepository : ISolutionRepository
{
    private readonly IMongoCollection<SolutionEntry> _collection;

    public SolutionRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<SolutionEntry>("solutions");
        EnsureIndexes();
    }

    private void EnsureIndexes()
    {
        var slugIndex = Builders<SolutionEntry>.IndexKeys.Ascending(x => x.Slug);
        _collection.Indexes.CreateOne(new CreateIndexModel<SolutionEntry>(slugIndex, new CreateIndexOptions { Unique = true }));
    }

    public async Task<IReadOnlyList<SolutionEntry>> GetAllAsync(string? search, string? categoryId, string? tag, bool? isPublished, int skip, int take, CancellationToken ct = default)
    {
        var filter = BuildFilter(search, categoryId, tag, isPublished);
        return await _collection.Find(filter).Skip(skip).Limit(take).SortByDescending(x => x.CreatedAt).ToListAsync(ct);
    }

    public async Task<long> CountAsync(string? search, string? categoryId, string? tag, bool? isPublished, CancellationToken ct = default)
    {
        var filter = BuildFilter(search, categoryId, tag, isPublished);
        return await _collection.CountDocumentsAsync(filter, cancellationToken: ct);
    }

    public async Task<SolutionEntry?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task<SolutionEntry?> GetBySlugAsync(string slug, CancellationToken ct = default)
        => await _collection.Find(x => x.Slug == slug).FirstOrDefaultAsync(ct);

    public async Task CreateAsync(SolutionEntry entry, CancellationToken ct = default)
        => await _collection.InsertOneAsync(entry, cancellationToken: ct);

    public async Task<bool> UpdateAsync(string id, SolutionEntry entry, CancellationToken ct = default)
    {
        var result = await _collection.ReplaceOneAsync(x => x.Id == id, entry, cancellationToken: ct);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == id, ct);
        return result.DeletedCount > 0;
    }

    private static FilterDefinition<SolutionEntry> BuildFilter(string? search, string? categoryId, string? tag, bool? isPublished)
    {
        var builder = Builders<SolutionEntry>.Filter;
        var filters = new List<FilterDefinition<SolutionEntry>>();

        if (!string.IsNullOrWhiteSpace(search))
            filters.Add(builder.Or(
                builder.Regex(x => x.Title, new MongoDB.Bson.BsonRegularExpression(search, "i")),
                builder.Regex(x => x.Summary, new MongoDB.Bson.BsonRegularExpression(search, "i"))
            ));

        if (!string.IsNullOrWhiteSpace(categoryId))
            filters.Add(builder.AnyEq(x => x.CategoryIds, categoryId));

        if (!string.IsNullOrWhiteSpace(tag))
            filters.Add(builder.AnyEq(x => x.Tags, tag));

        if (isPublished.HasValue)
            filters.Add(builder.Eq(x => x.IsPublished, isPublished.Value));

        return filters.Count > 0 ? builder.And(filters) : builder.Empty;
    }
}
