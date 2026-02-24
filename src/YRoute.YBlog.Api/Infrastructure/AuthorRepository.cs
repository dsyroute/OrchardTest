using MongoDB.Driver;
using YRoute.YBlog.Api.Domain;

namespace YRoute.YBlog.Api.Infrastructure;

public interface IAuthorRepository
{
    Task<IReadOnlyList<Author>> GetAllAsync(CancellationToken ct = default);
    Task<Author?> GetByIdAsync(string id, CancellationToken ct = default);
    Task CreateAsync(Author author, CancellationToken ct = default);
    Task<bool> UpdateAsync(string id, Author author, CancellationToken ct = default);
    Task<bool> DeleteAsync(string id, CancellationToken ct = default);
}

public class AuthorRepository : IAuthorRepository
{
    private readonly IMongoCollection<Author> _collection;

    public AuthorRepository(IMongoDatabase database)
        => _collection = database.GetCollection<Author>("authors");

    public async Task<IReadOnlyList<Author>> GetAllAsync(CancellationToken ct = default)
        => await _collection.Find(Builders<Author>.Filter.Empty).SortBy(x => x.DisplayName).ToListAsync(ct);

    public async Task<Author?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task CreateAsync(Author author, CancellationToken ct = default)
        => await _collection.InsertOneAsync(author, cancellationToken: ct);

    public async Task<bool> UpdateAsync(string id, Author author, CancellationToken ct = default)
    {
        var result = await _collection.ReplaceOneAsync(x => x.Id == id, author, cancellationToken: ct);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == id, ct);
        return result.DeletedCount > 0;
    }
}
