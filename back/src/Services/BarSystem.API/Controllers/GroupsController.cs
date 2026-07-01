using BarSystem.Core.Application.DTOs.Groups;
using BarSystem.Core.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly GroupAppService _appService;

    public GroupsController(GroupAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GroupResponseDto>>> GetAll(CancellationToken ct)
    {
        var groups = await _appService.GetAllAsync(ct);
        return Ok(groups);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GroupResponseDto>> GetById(Guid id, CancellationToken ct)
    {
        var group = await _appService.GetByIdAsync(id, ct);
        if (group == null)
            return NotFound();

        return Ok(group);
    }

    [HttpPost]
    [Authorize(Roles = "GroupAdmin")]
    public async Task<ActionResult<GroupResponseDto>> Create([FromBody] CreateGroupDto dto, CancellationToken ct)
    {
        try
        {
            var group = await _appService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
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
    public async Task<ActionResult<GroupResponseDto>> Update(Guid id, [FromBody] UpdateGroupDto dto, CancellationToken ct)
    {
        try
        {
            var group = await _appService.UpdateAsync(id, dto, ct);
            if (group == null)
                return NotFound();

            return Ok(group);
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
