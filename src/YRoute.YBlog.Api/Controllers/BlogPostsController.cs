using Microsoft.AspNetCore.Mvc;
using YRoute.Shared.Contracts;
using YRoute.YBlog.Api.Domain;
using YRoute.YBlog.Api.Dtos;
using YRoute.YBlog.Api.Infrastructure;

namespace YRoute.YBlog.Api.Controllers;

[ApiController]
[Route("api/blog-posts")]
[Produces("application/json")]
public class BlogPostsController : ControllerBase
{
    private readonly IBlogPostRepository _repo;
    private readonly IAuthorRepository _authorRepo;
    private readonly ILogger<BlogPostsController> _logger;

    public BlogPostsController(IBlogPostRepository repo, IAuthorRepository authorRepo, ILogger<BlogPostsController> logger)
    {
        _repo = repo;
        _authorRepo = authorRepo;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<BlogPostDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] BlogPostFilter filter, CancellationToken ct)
    {
        var skip = (filter.Page - 1) * filter.PageSize;
        var items = await _repo.GetAllAsync(filter.Search, filter.CategoryId, filter.Tag, filter.AuthorId, filter.IsPublished, skip, filter.PageSize, ct);
        var total = await _repo.CountAsync(filter.Search, filter.CategoryId, filter.Tag, filter.AuthorId, filter.IsPublished, ct);
        return Ok(new PagedResult<BlogPostDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = (int)total,
            Page = filter.Page,
            PageSize = filter.PageSize
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BlogPostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, CancellationToken ct)
    {
        var post = await _repo.GetByIdAsync(id, ct);
        return post is null ? NotFound() : Ok(MapToDto(post));
    }

    [HttpGet("by-slug/{slug}")]
    [ProducesResponseType(typeof(BlogPostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
    {
        var post = await _repo.GetBySlugAsync(slug, ct);
        return post is null ? NotFound() : Ok(MapToDto(post));
    }

    [HttpPost]
    [ProducesResponseType(typeof(BlogPostDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateBlogPostDto dto, CancellationToken ct)
    {
        var author = await _authorRepo.GetByIdAsync(dto.AuthorId, ct);
        var post = new BlogPost
        {
            Title = dto.Title,
            Slug = dto.Slug,
            Excerpt = dto.Excerpt,
            Content = dto.Content,
            AuthorId = dto.AuthorId,
            AuthorName = author?.DisplayName ?? string.Empty,
            CategoryIds = dto.CategoryIds,
            Tags = dto.Tags,
            CoverImageUrl = dto.CoverImageUrl,
            SeoTitle = dto.SeoTitle,
            SeoDescription = dto.SeoDescription,
            IsPublished = dto.IsPublished,
            PublishedAt = dto.IsPublished ? (dto.PublishedAt ?? DateTime.UtcNow) : null
        };
        await _repo.CreateAsync(post, ct);
        _logger.LogInformation("Created blog post {Id} with slug {Slug}", post.Id, post.Slug);
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, MapToDto(post));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateBlogPostDto dto, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        var author = await _authorRepo.GetByIdAsync(dto.AuthorId, ct);
        existing.Title = dto.Title;
        existing.Slug = dto.Slug;
        existing.Excerpt = dto.Excerpt;
        existing.Content = dto.Content;
        existing.AuthorId = dto.AuthorId;
        existing.AuthorName = author?.DisplayName ?? string.Empty;
        existing.CategoryIds = dto.CategoryIds;
        existing.Tags = dto.Tags;
        existing.CoverImageUrl = dto.CoverImageUrl;
        existing.SeoTitle = dto.SeoTitle;
        existing.SeoDescription = dto.SeoDescription;
        existing.IsPublished = dto.IsPublished;
        existing.PublishedAt = dto.IsPublished ? (dto.PublishedAt ?? existing.PublishedAt ?? DateTime.UtcNow) : null;
        existing.UpdatedAt = DateTime.UtcNow;

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

    private static BlogPostDto MapToDto(BlogPost p) => new()
    {
        Id = p.Id,
        Title = p.Title,
        Slug = p.Slug,
        Excerpt = p.Excerpt,
        Content = p.Content,
        AuthorId = p.AuthorId,
        AuthorName = p.AuthorName,
        CategoryIds = p.CategoryIds,
        Tags = p.Tags,
        CoverImageUrl = p.CoverImageUrl,
        SeoTitle = p.SeoTitle,
        SeoDescription = p.SeoDescription,
        IsPublished = p.IsPublished,
        PublishedAt = p.PublishedAt,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };
}
