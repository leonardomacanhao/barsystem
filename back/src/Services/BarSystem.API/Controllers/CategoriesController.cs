using BarSystem.Core.Application.DTOs.Categories;
using BarSystem.Core.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BarSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly CategoryAppService _appService;

    public CategoriesController(CategoryAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetAll(CancellationToken ct)
    {
        var categories = await _appService.GetAllAsync(ct);
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponseDto>> GetById(Guid id, CancellationToken ct)
    {
        var category = await _appService.GetByIdAsync(id, ct);
        if (category == null)
            return NotFound();

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponseDto>> Create([FromBody] CreateCategoryDto dto, CancellationToken ct)
    {
        try
        {
            var category = await _appService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryResponseDto>> Update(Guid id, [FromBody] UpdateCategoryDto dto, CancellationToken ct)
    {
        try
        {
            var category = await _appService.UpdateAsync(id, dto, ct);
            if (category == null)
                return NotFound();

            return Ok(category);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var success = await _appService.DeleteAsync(id, ct);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
