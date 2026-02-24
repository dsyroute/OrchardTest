using MongoDB.Driver;
using YRoute.YSolutions.Api.Domain;

namespace YRoute.YSolutions.Api.Infrastructure;

public interface ISolutionCategoryRepository
{
    Task<IReadOnlyList<SolutionCategory>> GetAllAsync(CancellationToken ct = default);
    Task<SolutionCategory?> GetByIdAsync(string id, CancellationToken ct = default);
    Task CreateAsync(SolutionCategory category, CancellationToken ct = default);
    Task<bool> UpdateAsync(string id, SolutionCategory category, CancellationToken ct = default);
    Task<bool> DeleteAsync(string id, CancellationToken ct = default);
}

public class SolutionCategoryRepository : ISolutionCategoryRepository
{
    private readonly IMongoCollection<SolutionCategory> _collection;

    public SolutionCategoryRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<SolutionCategory>("solution_categories");
    }

    public async Task<IReadOnlyList<SolutionCategory>> GetAllAsync(CancellationToken ct = default)
        => await _collection.Find(Builders<SolutionCategory>.Filter.Empty).SortBy(x => x.Name).ToListAsync(ct);

    public async Task<SolutionCategory?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task CreateAsync(SolutionCategory category, CancellationToken ct = default)
        => await _collection.InsertOneAsync(category, cancellationToken: ct);

    public async Task<bool> UpdateAsync(string id, SolutionCategory category, CancellationToken ct = default)
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
