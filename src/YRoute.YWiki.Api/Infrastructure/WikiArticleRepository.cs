using MongoDB.Driver;
using YRoute.YWiki.Api.Domain;

namespace YRoute.YWiki.Api.Infrastructure;

public interface IWikiArticleRepository
{
    Task<IReadOnlyList<WikiArticle>> GetAllAsync(string? search, string? categoryId, string? tag, bool? isPublished, int skip, int take, CancellationToken ct = default);
    Task<long> CountAsync(string? search, string? categoryId, string? tag, bool? isPublished, CancellationToken ct = default);
    Task<WikiArticle?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<WikiArticle?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task CreateAsync(WikiArticle article, CancellationToken ct = default);
    Task<bool> UpdateAsync(string id, WikiArticle article, CancellationToken ct = default);
    Task<bool> DeleteAsync(string id, CancellationToken ct = default);
}

public class WikiArticleRepository : IWikiArticleRepository
{
    private readonly IMongoCollection<WikiArticle> _collection;
    private const int MaxRevisionsKept = 10;

    public WikiArticleRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<WikiArticle>("wiki_articles");
        var slugIndex = Builders<WikiArticle>.IndexKeys.Ascending(x => x.Slug);
        _collection.Indexes.CreateOne(new CreateIndexModel<WikiArticle>(slugIndex, new CreateIndexOptions { Unique = true }));
    }

    public async Task<IReadOnlyList<WikiArticle>> GetAllAsync(string? search, string? categoryId, string? tag, bool? isPublished, int skip, int take, CancellationToken ct = default)
    {
        var filter = BuildFilter(search, categoryId, tag, isPublished);
        return await _collection.Find(filter).Skip(skip).Limit(take).SortBy(x => x.Title).ToListAsync(ct);
    }

    public async Task<long> CountAsync(string? search, string? categoryId, string? tag, bool? isPublished, CancellationToken ct = default)
    {
        var filter = BuildFilter(search, categoryId, tag, isPublished);
        return await _collection.CountDocumentsAsync(filter, cancellationToken: ct);
    }

    public async Task<WikiArticle?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task<WikiArticle?> GetBySlugAsync(string slug, CancellationToken ct = default)
        => await _collection.Find(x => x.Slug == slug).FirstOrDefaultAsync(ct);

    public async Task CreateAsync(WikiArticle article, CancellationToken ct = default)
        => await _collection.InsertOneAsync(article, cancellationToken: ct);

    public async Task<bool> UpdateAsync(string id, WikiArticle article, CancellationToken ct = default)
    {
        // Keep only last N revisions
        if (article.Revisions.Count > MaxRevisionsKept)
            article.Revisions = article.Revisions.OrderByDescending(r => r.RevisionNumber).Take(MaxRevisionsKept).ToList();

        var result = await _collection.ReplaceOneAsync(x => x.Id == id, article, cancellationToken: ct);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == id, ct);
        return result.DeletedCount > 0;
    }

    private static FilterDefinition<WikiArticle> BuildFilter(string? search, string? categoryId, string? tag, bool? isPublished)
    {
        var builder = Builders<WikiArticle>.Filter;
        var filters = new List<FilterDefinition<WikiArticle>>();

        if (!string.IsNullOrWhiteSpace(search))
            filters.Add(builder.Or(
                builder.Regex(x => x.Title, new MongoDB.Bson.BsonRegularExpression(search, "i")),
                builder.Regex(x => x.Summary, new MongoDB.Bson.BsonRegularExpression(search, "i")),
                builder.Regex(x => x.Content, new MongoDB.Bson.BsonRegularExpression(search, "i"))
            ));

        if (!string.IsNullOrWhiteSpace(categoryId))
            filters.Add(builder.Eq(x => x.CategoryId, categoryId));

        if (!string.IsNullOrWhiteSpace(tag))
            filters.Add(builder.AnyEq(x => x.Tags, tag));

        if (isPublished.HasValue)
            filters.Add(builder.Eq(x => x.IsPublished, isPublished.Value));

        return filters.Count > 0 ? builder.And(filters) : builder.Empty;
    }
}
