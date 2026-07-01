using BarSystem.Core.Application.DTOs.Tenants;
using BarSystem.Core.Application.Validators.Tenants;
using BarSystem.Core.Domain.Entities;
using BarSystem.Core.Infra.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BarSystem.Core.Application.Services;

public class TenantAppService
{
    private readonly CoreDbContext _context;
    private readonly ILogger<TenantAppService> _logger;

    public TenantAppService(CoreDbContext context, ILogger<TenantAppService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<TenantResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var tenants = await _context.Tenants
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync(ct);

        return tenants.Select(t => new TenantResponseDto
        {
            Id = t.Id,
            Name = t.Name,
            Cnpj = t.Cnpj,
            IsActive = t.IsActive,
            GroupId = t.GroupId,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        }).ToList();
    }

    public async Task<TenantResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var tenant = await _context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (tenant == null)
            return null;

        return new TenantResponseDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Cnpj = tenant.Cnpj,
            IsActive = tenant.IsActive,
            GroupId = tenant.GroupId,
            CreatedAt = tenant.CreatedAt,
            UpdatedAt = tenant.UpdatedAt
        };
    }

    public async Task<TenantResponseDto> CreateAsync(CreateTenantDto dto, Guid groupId, CancellationToken ct = default)
    {
        var validator = new CreateTenantValidator();
        var validationResult = await validator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
            throw new ValidationException("Erro de validação", validationResult.Errors);

        var tenant = new Tenant(dto.Name, dto.Cnpj);
        tenant.SetGroupId(groupId);

        try
        {
            await _context.Tenants.AddAsync(tenant, ct);
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Erro ao salvar tenant no banco");
            throw new Exception($"Erro ao salvar no banco: {dbEx.InnerException?.Message ?? dbEx.Message}");
        }

        return new TenantResponseDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Cnpj = tenant.Cnpj,
            IsActive = tenant.IsActive,
            GroupId = tenant.GroupId,
            CreatedAt = tenant.CreatedAt,
            UpdatedAt = tenant.UpdatedAt
        };
    }

    public async Task<TenantResponseDto?> UpdateAsync(Guid id, UpdateTenantDto dto, CancellationToken ct = default)
    {
        var validator = new UpdateTenantValidator();
        var validationResult = await validator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
            throw new ValidationException("Erro de validação", validationResult.Errors);

        var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (tenant == null)
            return null;

        tenant.Update(dto.Name, dto.Cnpj);

        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Erro ao atualizar tenant no banco");
            throw new Exception($"Erro ao salvar no banco: {dbEx.InnerException?.Message ?? dbEx.Message}");
        }

        return new TenantResponseDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Cnpj = tenant.Cnpj,
            IsActive = tenant.IsActive,
            GroupId = tenant.GroupId,
            CreatedAt = tenant.CreatedAt,
            UpdatedAt = tenant.UpdatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (tenant == null)
            return false;

        _context.Tenants.Remove(tenant);
        await _context.SaveChangesAsync(ct);

        return true;
    }
}
