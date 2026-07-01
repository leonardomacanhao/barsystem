using BarSystem.Core.Application.DTOs.Tables;
using BarSystem.Core.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BarSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TablesController : ControllerBase
{
    private readonly TableAppService _appService;

    public TablesController(TableAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TableResponseDto>>> GetAll(CancellationToken ct)
    {
        var tables = await _appService.GetAllAsync(ct);
        return Ok(tables);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TableResponseDto>> GetById(Guid id, CancellationToken ct)
    {
        var table = await _appService.GetByIdAsync(id, ct);
        if (table == null)
            return NotFound();

        return Ok(table);
    }

    [HttpPost]
    public async Task<ActionResult<TableResponseDto>> Create([FromBody] CreateTableDto dto, CancellationToken ct)
    {
        try
        {
            var table = await _appService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = table.Id }, table);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }).ToList();
            return BadRequest(new { message = ex.Message, errors });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TableResponseDto>> Update(Guid id, [FromBody] UpdateTableDto dto, CancellationToken ct)
    {
        try
        {
            var table = await _appService.UpdateAsync(id, dto, ct);
            if (table == null)
                return NotFound();

            return Ok(table);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }).ToList();
            return BadRequest(new { message = ex.Message, errors });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<TableResponseDto>> UpdateStatus(Guid id, [FromBody] UpdateTableStatusDto dto, CancellationToken ct)
    {
        try
        {
            var table = await _appService.UpdateStatusAsync(id, dto, ct);
            if (table == null)
                return NotFound();

            return Ok(table);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }).ToList();
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
