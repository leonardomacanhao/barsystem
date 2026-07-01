using BarSystem.Core.Application.DTOs.Products;
using BarSystem.Core.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BarSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductAppService _appService;

    public ProductsController(ProductAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAll(CancellationToken ct)
    {
        var products = await _appService.GetAllAsync(ct);
        return Ok(products);
    }

    [HttpGet("by-category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetByCategory(Guid categoryId, CancellationToken ct)
    {
        var products = await _appService.GetByCategoryAsync(categoryId, ct);
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponseDto>> GetById(Guid id, CancellationToken ct)
    {
        var product = await _appService.GetByIdAsync(id, ct);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> Create([FromBody] CreateProductDto dto, CancellationToken ct)
    {
        try
        {
            var product = await _appService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new 
            { 
                field = e.PropertyName, 
                message = e.ErrorMessage 
            }).ToList();
            
            return BadRequest(new { message = ex.Message, errors });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductResponseDto>> Update(Guid id, [FromBody] UpdateProductDto dto, CancellationToken ct)
    {
        try
        {
            var product = await _appService.UpdateAsync(id, dto, ct);
            if (product == null)
                return NotFound();

            return Ok(product);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new 
            { 
                field = e.PropertyName, 
                message = e.ErrorMessage 
            }).ToList();
            
            return BadRequest(new { message = ex.Message, errors });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
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
