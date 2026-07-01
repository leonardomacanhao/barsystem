using BarSystem.Core.Application.DTOs.Tenants;
using BarSystem.Core.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TenantsController : ControllerBase
{
    private readonly TenantAppService _appService;

    public TenantsController(TenantAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TenantResponseDto>>> GetAll(CancellationToken ct)
    {
        var tenants = await _appService.GetAllAsync(ct);
        return Ok(tenants);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TenantResponseDto>> GetById(Guid id, CancellationToken ct)
    {
        var tenant = await _appService.GetByIdAsync(id, ct);
        if (tenant == null)
            return NotFound();

        return Ok(tenant);
    }

    [HttpPost]
    [Authorize(Roles = "GroupAdmin")]
    public async Task<ActionResult<TenantResponseDto>> Create([FromBody] CreateTenantDto dto, CancellationToken ct)
    {
        try
        {
            var groupId = (Guid)HttpContext.Items["GroupId"]!;
            var tenant = await _appService.CreateAsync(dto, groupId, ct);
            return CreatedAtAction(nameof(GetById), new { id = tenant.Id }, tenant);
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
    [Authorize(Roles = "GroupAdmin")]
    public async Task<ActionResult<TenantResponseDto>> Update(Guid id, [FromBody] UpdateTenantDto dto, CancellationToken ct)
    {
        try
        {
            var tenant = await _appService.UpdateAsync(id, dto, ct);
            if (tenant == null)
                return NotFound();

            return Ok(tenant);
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
    [Authorize(Roles = "GroupAdmin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var success = await _appService.DeleteAsync(id, ct);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
