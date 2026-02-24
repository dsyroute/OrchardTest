using Microsoft.AspNetCore.Mvc;
using YRoute.Shared.Contracts;
using YRoute.YSolutions.Api.Domain;
using YRoute.YSolutions.Api.Dtos;
using YRoute.YSolutions.Api.Infrastructure;

namespace YRoute.YSolutions.Api.Controllers;

[ApiController]
[Route("api/solutions")]
[Produces("application/json")]
public class SolutionsController : ControllerBase
{
    private readonly ISolutionRepository _repo;
    private readonly ILogger<SolutionsController> _logger;

    public SolutionsController(ISolutionRepository repo, ILogger<SolutionsController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SolutionEntryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] SolutionEntryFilter filter, CancellationToken ct)
    {
        var skip = (filter.Page - 1) * filter.PageSize;
        var items = await _repo.GetAllAsync(filter.Search, filter.CategoryId, filter.Tag, filter.IsPublished, skip, filter.PageSize, ct);
        var total = await _repo.CountAsync(filter.Search, filter.CategoryId, filter.Tag, filter.IsPublished, ct);

        return Ok(new PagedResult<SolutionEntryDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = (int)total,
            Page = filter.Page,
            PageSize = filter.PageSize
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SolutionEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, CancellationToken ct)
    {
        var entry = await _repo.GetByIdAsync(id, ct);
        return entry is null ? NotFound() : Ok(MapToDto(entry));
    }

    [HttpGet("by-slug/{slug}")]
    [ProducesResponseType(typeof(SolutionEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
    {
        var entry = await _repo.GetBySlugAsync(slug, ct);
        return entry is null ? NotFound() : Ok(MapToDto(entry));
    }

    [HttpPost]
    [ProducesResponseType(typeof(SolutionEntryDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateSolutionEntryDto dto, CancellationToken ct)
    {
        var entry = MapFromCreateDto(dto);
        await _repo.CreateAsync(entry, ct);
        _logger.LogInformation("Created solution entry {Id} with slug {Slug}", entry.Id, entry.Slug);
        return CreatedAtAction(nameof(GetById), new { id = entry.Id }, MapToDto(entry));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateSolutionEntryDto dto, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        var updated = MapFromCreateDto(dto);
        updated.Id = id;
        updated.CreatedAt = existing.CreatedAt;
        updated.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(id, updated, ct);
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

    private static SolutionEntryDto MapToDto(SolutionEntry e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Slug = e.Slug,
        Summary = e.Summary,
        Content = e.Content,
        CategoryIds = e.CategoryIds,
        Tags = e.Tags,
        ImageUrls = e.ImageUrls,
        SeoTitle = e.SeoTitle,
        SeoDescription = e.SeoDescription,
        IsPublished = e.IsPublished,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };

    private static SolutionEntry MapFromCreateDto(CreateSolutionEntryDto dto) => new()
    {
        Title = dto.Title,
        Slug = dto.Slug,
        Summary = dto.Summary,
        Content = dto.Content,
        CategoryIds = dto.CategoryIds,
        Tags = dto.Tags,
        ImageUrls = dto.ImageUrls,
        SeoTitle = dto.SeoTitle,
        SeoDescription = dto.SeoDescription,
        IsPublished = dto.IsPublished
    };
}
