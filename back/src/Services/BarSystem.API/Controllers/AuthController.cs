using BarSystem.Core.Application.DTOs.Auth;
using BarSystem.Core.Application.Services.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BarSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthAppService _authService;

    public AuthController(AuthAppService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto, CancellationToken ct)
    {
        try
        {
            var response = await _authService.LoginAsync(dto, ct);
            return Ok(response);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }).ToList();
            return Unauthorized(new { message = ex.Message, errors });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponseDto>> Register([FromBody] RegisterDto dto, CancellationToken ct)
    {
        try
        {
            var response = await _authService.RegisterAsync(dto, ct);
            return CreatedAtAction(nameof(Login), response);
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

    [HttpPost("switch-tenant")]
    [Authorize]
    public async Task<ActionResult<LoginResponseDto>> SwitchTenant([FromBody] SwitchTenantDto dto, CancellationToken ct)
    {
        try
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Usuário não autenticado" });

            var response = await _authService.SwitchTenantAsync(userId, dto, ct);
            return Ok(response);
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
}
