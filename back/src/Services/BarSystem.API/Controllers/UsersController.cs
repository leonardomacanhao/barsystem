using BarSystem.Core.Application.DTOs.Users;
using BarSystem.Core.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserAppService _appService;

    public UsersController(UserAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll(CancellationToken ct)
    {
        var users = await _appService.GetAllAsync(ct);
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetById(Guid id, CancellationToken ct)
    {
        var user = await _appService.GetByIdAsync(id, ct);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    [Authorize(Roles = "GroupAdmin")]
    public async Task<ActionResult<UserResponseDto>> Create([FromBody] CreateUserDto dto, CancellationToken ct)
    {
        try
        {
            var groupId = (Guid)HttpContext.Items["GroupId"]!;
            var user = await _appService.CreateAsync(dto, groupId, ct);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
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
    public async Task<ActionResult<UserResponseDto>> Update(Guid id, [FromBody] UpdateUserDto dto, CancellationToken ct)
    {
        try
        {
            var user = await _appService.UpdateAsync(id, dto, ct);
            if (user == null)
                return NotFound();

            return Ok(user);
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
