using Microsoft.AspNetCore.Mvc;
using YRoute.Shared.Contracts;
using YRoute.YWiki.Api.Domain;
using YRoute.YWiki.Api.Dtos;
using YRoute.YWiki.Api.Infrastructure;

namespace YRoute.YWiki.Api.Controllers;

[ApiController]
[Route("api/wiki-articles")]
[Produces("application/json")]
public class WikiArticlesController : ControllerBase
{
    private readonly IWikiArticleRepository _repo;
    private readonly ILogger<WikiArticlesController> _logger;

    public WikiArticlesController(IWikiArticleRepository repo, ILogger<WikiArticlesController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<WikiArticleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] WikiArticleFilter filter, CancellationToken ct)
    {
        var skip = (filter.Page - 1) * filter.PageSize;
        var items = await _repo.GetAllAsync(filter.Search, filter.CategoryId, filter.Tag, filter.IsPublished, skip, filter.PageSize, ct);
        var total = await _repo.CountAsync(filter.Search, filter.CategoryId, filter.Tag, filter.IsPublished, ct);
        return Ok(new PagedResult<WikiArticleDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = (int)total,
            Page = filter.Page,
            PageSize = filter.PageSize
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WikiArticleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, CancellationToken ct)
    {
        var article = await _repo.GetByIdAsync(id, ct);
        return article is null ? NotFound() : Ok(MapToDto(article));
    }

    [HttpGet("by-slug/{slug}")]
    [ProducesResponseType(typeof(WikiArticleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
    {
        var article = await _repo.GetBySlugAsync(slug, ct);
        return article is null ? NotFound() : Ok(MapToDto(article));
    }

    [HttpGet("{id}/revisions")]
    [ProducesResponseType(typeof(IEnumerable<WikiRevisionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRevisions(string id, CancellationToken ct)
    {
        var article = await _repo.GetByIdAsync(id, ct);
        if (article is null) return NotFound();
        return Ok(article.Revisions.OrderByDescending(r => r.RevisionNumber).Select(r => new WikiRevisionDto
        {
            RevisionNumber = r.RevisionNumber,
            Content = r.Content,
            EditedBy = r.EditedBy,
            ChangeNote = r.ChangeNote,
            EditedAt = r.EditedAt
        }));
    }

    [HttpPost]
    [ProducesResponseType(typeof(WikiArticleDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateWikiArticleDto dto, CancellationToken ct)
    {
        var revision = new WikiRevision
        {
            RevisionNumber = 1,
            Content = dto.Content,
            EditedBy = dto.EditedBy,
            ChangeNote = dto.ChangeNote ?? "Initial version"
        };
        var article = new WikiArticle
        {
            Title = dto.Title,
            Slug = dto.Slug,
            Content = dto.Content,
            Summary = dto.Summary,
            CategoryId = dto.CategoryId,
            Tags = dto.Tags,
            RelatedSlugs = dto.RelatedSlugs,
            IsPublished = dto.IsPublished,
            CurrentRevisionNumber = 1,
            Revisions = [revision]
        };
        await _repo.CreateAsync(article, ct);
        _logger.LogInformation("Created wiki article {Id} with slug {Slug}", article.Id, article.Slug);
        return CreatedAtAction(nameof(GetById), new { id = article.Id }, MapToDto(article));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateWikiArticleDto dto, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        var newRevisionNumber = existing.CurrentRevisionNumber + 1;
        var revision = new WikiRevision
        {
            RevisionNumber = newRevisionNumber,
            Content = dto.Content,
            EditedBy = dto.EditedBy,
            ChangeNote = dto.ChangeNote,
            EditedAt = DateTime.UtcNow
        };
        existing.Title = dto.Title;
        existing.Slug = dto.Slug;
        existing.Content = dto.Content;
        existing.Summary = dto.Summary;
        existing.CategoryId = dto.CategoryId;
        existing.Tags = dto.Tags;
        existing.RelatedSlugs = dto.RelatedSlugs;
        existing.IsPublished = dto.IsPublished;
        existing.CurrentRevisionNumber = newRevisionNumber;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.Revisions.Add(revision);

        await _repo.UpdateAsync(id, existing, ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var deleted = await _repo.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }

    private static WikiArticleDto MapToDto(WikiArticle a) => new()
    {
        Id = a.Id,
        Title = a.Title,
        Slug = a.Slug,
        Content = a.Content,
        Summary = a.Summary,
        CategoryId = a.CategoryId,
        Tags = a.Tags,
        RelatedSlugs = a.RelatedSlugs,
        CurrentRevisionNumber = a.CurrentRevisionNumber,
        IsPublished = a.IsPublished,
        CreatedAt = a.CreatedAt,
        UpdatedAt = a.UpdatedAt
    };
}
