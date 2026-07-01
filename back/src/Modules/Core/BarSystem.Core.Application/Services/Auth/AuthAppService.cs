using BarSystem.Core.Application.DTOs.Auth;
using BarSystem.Core.Application.Validators.Auth;
using BarSystem.Core.Domain.Entities;
using BarSystem.Core.Infra.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BarSystem.Core.Application.Services.Auth;

public class AuthAppService
{
    private readonly CoreDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthAppService> _logger;

    public AuthAppService(
        CoreDbContext context, 
        IJwtService jwtService, 
        IConfiguration configuration,
        ILogger<AuthAppService> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        var validator = new LoginValidator();
        var validationResult = await validator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
            throw new ValidationException("Erro de validação", validationResult.Errors);

        var user = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == dto.Email, ct);

        if (user == null || !user.IsActive)
        {
            throw new ValidationException("Credenciais inválidas", new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("Email", "Email ou senha inválidos")
            });
        }

        var passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!passwordValid)
        {
            throw new ValidationException("Credenciais inválidas", new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("Password", "Email ou senha inválidos")
            });
        }

        var group = await _context.Groups
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(g => g.Id == user.GroupId && g.IsActive, ct);

        if (group == null)
        {
            throw new ValidationException("Grupo não encontrado ou inativo");
        }

        Tenant? tenant = null;
        if (user.TenantId.HasValue)
        {
            tenant = await _context.Tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == user.TenantId.Value && t.IsActive, ct);

            if (tenant == null)
            {
                throw new ValidationException("Bar não encontrado ou inativo");
            }
        }

        var token = _jwtService.GenerateToken(user, group, tenant);
        var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationInMinutes"]!);

        return new LoginResponseDto
        {
            Token = token,
            UserId = user.Id,
            GroupId = group.Id,
            GroupName = group.Name,
            TenantId = tenant?.Id,
            TenantName = tenant?.Name,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
        };
    }

    public async Task<LoginResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
    {
        var validator = new RegisterValidator();
        var validationResult = await validator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
            throw new ValidationException("Erro de validação", validationResult.Errors);

        var existingUser = await _context.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Email == dto.AdminEmail, ct);

        if (existingUser)
            throw new ValidationException("Email já cadastrado", new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("AdminEmail", "Este email já está em uso")
            });

        try
        {
            var group = new Group(
                dto.GroupName, 
                dto.GroupDescription, 
                dto.GroupContactEmail, 
                dto.GroupContactPhone
            );
            await _context.Groups.AddAsync(group, ct);
            await _context.SaveChangesAsync(ct);

            var firstBar = new Tenant(dto.FirstBarName, dto.FirstBarCnpj);
            firstBar.SetGroupId(group.Id);
            await _context.Tenants.AddAsync(firstBar, ct);
            await _context.SaveChangesAsync(ct);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.AdminPassword);
            var adminUser = new User(dto.AdminName, dto.AdminEmail, passwordHash, "GroupAdmin");
            adminUser.SetGroupId(group.Id);
            
            await _context.Users.AddAsync(adminUser, ct);
            await _context.SaveChangesAsync(ct);

            var token = _jwtService.GenerateToken(adminUser, group, null);
            var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationInMinutes"]!);

            return new LoginResponseDto
            {
                Token = token,
                UserId = adminUser.Id,
                GroupId = group.Id,
                GroupName = group.Name,
                TenantId = null,
                TenantName = null,
                Name = adminUser.Name,
                Email = adminUser.Email,
                Role = adminUser.Role,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
            };
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Erro ao salvar no banco");
            throw new Exception($"Erro ao salvar no banco: {dbEx.InnerException?.Message ?? dbEx.Message}");
        }
    }

    public async Task<LoginResponseDto> SwitchTenantAsync(Guid userId, SwitchTenantDto dto, CancellationToken ct = default)
    {
        var user = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null)
            throw new ValidationException("Usuário não encontrado");

        if (!user.HasAccessToTenant(dto.TenantId))
            throw new ValidationException("Você não tem acesso a este bar");

        var group = await _context.Groups
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(g => g.Id == user.GroupId, ct);

        var tenant = await _context.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == dto.TenantId && t.GroupId == user.GroupId, ct);

        if (group == null || tenant == null)
            throw new ValidationException("Grupo ou bar não encontrado");

        var token = _jwtService.GenerateToken(user, group, tenant);
        var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationInMinutes"]!);

        return new LoginResponseDto
        {
            Token = token,
            UserId = user.Id,
            GroupId = group.Id,
            GroupName = group.Name,
            TenantId = tenant.Id,
            TenantName = tenant.Name,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
        };
    }
}
