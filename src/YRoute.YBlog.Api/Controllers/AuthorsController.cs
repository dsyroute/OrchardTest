using Microsoft.AspNetCore.Mvc;
using YRoute.YBlog.Api.Domain;
using YRoute.YBlog.Api.Dtos;
using YRoute.YBlog.Api.Infrastructure;

namespace YRoute.YBlog.Api.Controllers;

[ApiController]
[Route("api/authors")]
[Produces("application/json")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorRepository _repo;

    public AuthorsController(IAuthorRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok((await _repo.GetAllAsync(ct)).Select(MapToDto));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct)
    {
        var author = await _repo.GetByIdAsync(id, ct);
        return author is null ? NotFound() : Ok(MapToDto(author));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAuthorDto dto, CancellationToken ct)
    {
        var author = new Author { DisplayName = dto.DisplayName, Email = dto.Email, Bio = dto.Bio, AvatarUrl = dto.AvatarUrl };
        await _repo.CreateAsync(author, ct);
        return CreatedAtAction(nameof(GetById), new { id = author.Id }, MapToDto(author));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateAuthorDto dto, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();
        existing.DisplayName = dto.DisplayName;
        existing.Email = dto.Email;
        existing.Bio = dto.Bio;
        existing.AvatarUrl = dto.AvatarUrl;
        await _repo.UpdateAsync(id, existing, ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var deleted = await _repo.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }

    private static AuthorDto MapToDto(Author a) => new()
    {
        Id = a.Id, DisplayName = a.DisplayName, Email = a.Email, Bio = a.Bio, AvatarUrl = a.AvatarUrl, CreatedAt = a.CreatedAt
    };
}
