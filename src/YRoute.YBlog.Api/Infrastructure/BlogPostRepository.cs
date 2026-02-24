using MongoDB.Driver;
using YRoute.YBlog.Api.Domain;

namespace YRoute.YBlog.Api.Infrastructure;

public interface IBlogPostRepository
{
    Task<IReadOnlyList<BlogPost>> GetAllAsync(string? search, string? categoryId, string? tag, string? authorId, bool? isPublished, int skip, int take, CancellationToken ct = default);
    Task<long> CountAsync(string? search, string? categoryId, string? tag, string? authorId, bool? isPublished, CancellationToken ct = default);
    Task<BlogPost?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<BlogPost?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task CreateAsync(BlogPost post, CancellationToken ct = default);
    Task<bool> UpdateAsync(string id, BlogPost post, CancellationToken ct = default);
    Task<bool> DeleteAsync(string id, CancellationToken ct = default);
}

public class BlogPostRepository : IBlogPostRepository
{
    private readonly IMongoCollection<BlogPost> _collection;

    public BlogPostRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<BlogPost>("blog_posts");
        var slugIndex = Builders<BlogPost>.IndexKeys.Ascending(x => x.Slug);
        _collection.Indexes.CreateOne(new CreateIndexModel<BlogPost>(slugIndex, new CreateIndexOptions { Unique = true }));
    }

    public async Task<IReadOnlyList<BlogPost>> GetAllAsync(string? search, string? categoryId, string? tag, string? authorId, bool? isPublished, int skip, int take, CancellationToken ct = default)
    {
        var filter = BuildFilter(search, categoryId, tag, authorId, isPublished);
        return await _collection.Find(filter).Skip(skip).Limit(take).SortByDescending(x => x.CreatedAt).ToListAsync(ct);
    }

    public async Task<long> CountAsync(string? search, string? categoryId, string? tag, string? authorId, bool? isPublished, CancellationToken ct = default)
    {
        var filter = BuildFilter(search, categoryId, tag, authorId, isPublished);
        return await _collection.CountDocumentsAsync(filter, cancellationToken: ct);
    }

    public async Task<BlogPost?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task<BlogPost?> GetBySlugAsync(string slug, CancellationToken ct = default)
        => await _collection.Find(x => x.Slug == slug).FirstOrDefaultAsync(ct);

    public async Task CreateAsync(BlogPost post, CancellationToken ct = default)
        => await _collection.InsertOneAsync(post, cancellationToken: ct);

    public async Task<bool> UpdateAsync(string id, BlogPost post, CancellationToken ct = default)
    {
        var result = await _collection.ReplaceOneAsync(x => x.Id == id, post, cancellationToken: ct);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == id, ct);
        return result.DeletedCount > 0;
    }

    private static FilterDefinition<BlogPost> BuildFilter(string? search, string? categoryId, string? tag, string? authorId, bool? isPublished)
    {
        var builder = Builders<BlogPost>.Filter;
        var filters = new List<FilterDefinition<BlogPost>>();

        if (!string.IsNullOrWhiteSpace(search))
            filters.Add(builder.Or(
                builder.Regex(x => x.Title, new MongoDB.Bson.BsonRegularExpression(search, "i")),
                builder.Regex(x => x.Excerpt, new MongoDB.Bson.BsonRegularExpression(search, "i"))
            ));

        if (!string.IsNullOrWhiteSpace(categoryId))
            filters.Add(builder.AnyEq(x => x.CategoryIds, categoryId));

        if (!string.IsNullOrWhiteSpace(tag))
            filters.Add(builder.AnyEq(x => x.Tags, tag));

        if (!string.IsNullOrWhiteSpace(authorId))
            filters.Add(builder.Eq(x => x.AuthorId, authorId));

        if (isPublished.HasValue)
            filters.Add(builder.Eq(x => x.IsPublished, isPublished.Value));

        return filters.Count > 0 ? builder.And(filters) : builder.Empty;
    }
}
