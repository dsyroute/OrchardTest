using Microsoft.AspNetCore.Mvc;
using YRoute.YSolutions.Api.Domain;
using YRoute.YSolutions.Api.Dtos;
using YRoute.YSolutions.Api.Infrastructure;

namespace YRoute.YSolutions.Api.Controllers;

[ApiController]
[Route("api/solution-categories")]
[Produces("application/json")]
public class SolutionCategoriesController : ControllerBase
{
    private readonly ISolutionCategoryRepository _repo;

    public SolutionCategoriesController(ISolutionCategoryRepository repo)
        => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var categories = await _repo.GetAllAsync(ct);
        return Ok(categories.Select(MapToDto));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct)
    {
        var cat = await _repo.GetByIdAsync(id, ct);
        return cat is null ? NotFound() : Ok(MapToDto(cat));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSolutionCategoryDto dto, CancellationToken ct)
    {
        var category = new SolutionCategory { Name = dto.Name, Slug = dto.Slug, Description = dto.Description };
        await _repo.CreateAsync(category, ct);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, MapToDto(category));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateSolutionCategoryDto dto, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();
        existing.Name = dto.Name;
        existing.Slug = dto.Slug;
        existing.Description = dto.Description;
        await _repo.UpdateAsync(id, existing, ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var deleted = await _repo.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }

    private static SolutionCategoryDto MapToDto(SolutionCategory c) => new()
    {
        Id = c.Id, Name = c.Name, Slug = c.Slug, Description = c.Description, CreatedAt = c.CreatedAt
    };
}
