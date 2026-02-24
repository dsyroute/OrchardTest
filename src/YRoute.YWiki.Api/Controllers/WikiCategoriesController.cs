using Microsoft.AspNetCore.Mvc;
using YRoute.YWiki.Api.Domain;
using YRoute.YWiki.Api.Dtos;
using YRoute.YWiki.Api.Infrastructure;

namespace YRoute.YWiki.Api.Controllers;

[ApiController]
[Route("api/wiki-categories")]
[Produces("application/json")]
public class WikiCategoriesController : ControllerBase
{
    private readonly IWikiCategoryRepository _repo;

    public WikiCategoriesController(IWikiCategoryRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok((await _repo.GetAllAsync(ct)).Select(MapToDto));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct)
    {
        var cat = await _repo.GetByIdAsync(id, ct);
        return cat is null ? NotFound() : Ok(MapToDto(cat));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWikiCategoryDto dto, CancellationToken ct)
    {
        var category = new WikiCategory { Name = dto.Name, Slug = dto.Slug, Description = dto.Description, ParentId = dto.ParentId };
        await _repo.CreateAsync(category, ct);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, MapToDto(category));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateWikiCategoryDto dto, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();
        existing.Name = dto.Name;
        existing.Slug = dto.Slug;
        existing.Description = dto.Description;
        existing.ParentId = dto.ParentId;
        await _repo.UpdateAsync(id, existing, ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var deleted = await _repo.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }

    private static WikiCategoryDto MapToDto(WikiCategory c) => new()
    {
        Id = c.Id, Name = c.Name, Slug = c.Slug, Description = c.Description, ParentId = c.ParentId, CreatedAt = c.CreatedAt
    };
}
