using BarSystem.Core.Application.DTOs.Users;
using BarSystem.Core.Application.Validators.Users;
using BarSystem.Core.Domain.Entities;
using BarSystem.Core.Infra.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BarSystem.Core.Application.Services;

public class UserAppService
{
    private readonly CoreDbContext _context;
    private readonly ILogger<UserAppService> _logger;

    public UserAppService(CoreDbContext context, ILogger<UserAppService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var users = await _context.Users
            .AsNoTracking()
            .Include(u => _context.Tenants) // workaround para pegar TenantName
            .OrderBy(u => u.Name)
            .ToListAsync(ct);

        // Busca nomes dos tenants
        var tenantIds = users.Where(u => u.TenantId.HasValue).Select(u => u.TenantId!.Value).Distinct().ToList();
        var tenants = await _context.Tenants
            .IgnoreQueryFilters()
            .Where(t => tenantIds.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, t => t.Name, ct);

        return users.Select(u => new UserResponseDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Role = u.Role,
            IsActive = u.IsActive,
            GroupId = u.GroupId,
            TenantId = u.TenantId,
            TenantName = u.TenantId.HasValue && tenants.ContainsKey(u.TenantId.Value) ? tenants[u.TenantId.Value] : null,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        }).ToList();
    }

    public async Task<UserResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        if (user == null)
            return null;

        string? tenantName = null;
        if (user.TenantId.HasValue)
        {
            var tenant = await _context.Tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == user.TenantId.Value, ct);
            tenantName = tenant?.Name;
        }

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            GroupId = user.GroupId,
            TenantId = user.TenantId,
            TenantName = tenantName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<UserResponseDto> CreateAsync(CreateUserDto dto, Guid groupId, CancellationToken ct = default)
    {
        var validator = new CreateUserValidator();
        var validationResult = await validator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
            throw new ValidationException("Erro de validação", validationResult.Errors);

        // Verifica se email já existe no grupo
        var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email, ct);
        if (exists)
        {
            throw new ValidationException("Email já cadastrado", new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("Email", "Este email já está em uso neste grupo")
            });
        }

        // Se TenantId foi informado, verifica se pertence ao grupo
        if (dto.TenantId.HasValue)
        {
            var tenantExists = await _context.Tenants.AnyAsync(t => t.Id == dto.TenantId.Value, ct);
            if (!tenantExists)
            {
                throw new ValidationException("Bar não encontrado", new List<FluentValidation.Results.ValidationFailure>
                {
                    new FluentValidation.Results.ValidationFailure("TenantId", "Bar não encontrado neste grupo")
                });
            }
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var user = new User(dto.Name, dto.Email, passwordHash, dto.Role, dto.TenantId);
        user.SetGroupId(groupId);

        try
        {
            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Erro ao salvar usuário no banco");
            throw new Exception($"Erro ao salvar no banco: {dbEx.InnerException?.Message ?? dbEx.Message}");
        }

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            GroupId = user.GroupId,
            TenantId = user.TenantId,
            TenantName = null,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<UserResponseDto?> UpdateAsync(Guid id, UpdateUserDto dto, CancellationToken ct = default)
    {
        var validator = new UpdateUserValidator();
        var validationResult = await validator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
            throw new ValidationException("Erro de validação", validationResult.Errors);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user == null)
            return null;

        // Verifica se email já existe em outro usuário
        var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id, ct);
        if (exists)
        {
            throw new ValidationException("Email já cadastrado", new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("Email", "Este email já está em uso")
            });
        }

        user.Update(dto.Name, dto.Email, dto.Role, dto.TenantId);

        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Erro ao atualizar usuário no banco");
            throw new Exception($"Erro ao salvar no banco: {dbEx.InnerException?.Message ?? dbEx.Message}");
        }

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            GroupId = user.GroupId,
            TenantId = user.TenantId,
            TenantName = null,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(ct);

        return true;
    }
}
