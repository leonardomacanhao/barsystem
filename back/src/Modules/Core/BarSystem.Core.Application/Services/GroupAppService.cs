using BarSystem.Core.Application.DTOs.Groups;
using BarSystem.Core.Application.Validators.Groups;
using BarSystem.Core.Domain.Entities;
using BarSystem.Core.Infra.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BarSystem.Core.Application.Services;

public class GroupAppService
{
    private readonly CoreDbContext _context;
    private readonly ILogger<GroupAppService> _logger;

    public GroupAppService(CoreDbContext context, ILogger<GroupAppService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<GroupResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var groups = await _context.Groups
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .ToListAsync(ct);

        var result = new List<GroupResponseDto>();
        foreach (var g in groups)
        {
            var tenantsCount = await _context.Tenants.IgnoreQueryFilters().CountAsync(t => t.GroupId == g.Id, ct);
            var usersCount = await _context.Users.IgnoreQueryFilters().CountAsync(u => u.GroupId == g.Id, ct);

            result.Add(new GroupResponseDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                ContactEmail = g.ContactEmail,
                ContactPhone = g.ContactPhone,
                IsActive = g.IsActive,
                TenantsCount = tenantsCount,
                UsersCount = usersCount,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt
            });
        }

        return result;
    }

    public async Task<GroupResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var group = await _context.Groups
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id, ct);

        if (group == null)
            return null;

        var tenantsCount = await _context.Tenants.IgnoreQueryFilters().CountAsync(t => t.GroupId == group.Id, ct);
        var usersCount = await _context.Users.IgnoreQueryFilters().CountAsync(u => u.GroupId == group.Id, ct);

        return new GroupResponseDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            ContactEmail = group.ContactEmail,
            ContactPhone = group.ContactPhone,
            IsActive = group.IsActive,
            TenantsCount = tenantsCount,
            UsersCount = usersCount,
            CreatedAt = group.CreatedAt,
            UpdatedAt = group.UpdatedAt
        };
    }

    public async Task<GroupResponseDto> CreateAsync(CreateGroupDto dto, CancellationToken ct = default)
    {
        var validator = new CreateGroupValidator();
        var validationResult = await validator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
            throw new ValidationException("Erro de validação", validationResult.Errors);

        var group = new Group(dto.Name, dto.Description, dto.ContactEmail, dto.ContactPhone);

        try
        {
            await _context.Groups.AddAsync(group, ct);
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Erro ao salvar grupo no banco");
            throw new Exception($"Erro ao salvar no banco: {dbEx.InnerException?.Message ?? dbEx.Message}");
        }

        return new GroupResponseDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            ContactEmail = group.ContactEmail,
            ContactPhone = group.ContactPhone,
            IsActive = group.IsActive,
            TenantsCount = 0,
            UsersCount = 0,
            CreatedAt = group.CreatedAt,
            UpdatedAt = group.UpdatedAt
        };
    }

    public async Task<GroupResponseDto?> UpdateAsync(Guid id, UpdateGroupDto dto, CancellationToken ct = default)
    {
        var validator = new UpdateGroupValidator();
        var validationResult = await validator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
            throw new ValidationException("Erro de validação", validationResult.Errors);

        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == id, ct);
        if (group == null)
            return null;

        group.Update(dto.Name, dto.Description, dto.ContactEmail, dto.ContactPhone);

        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Erro ao atualizar grupo no banco");
            throw new Exception($"Erro ao salvar no banco: {dbEx.InnerException?.Message ?? dbEx.Message}");
        }

        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == id, ct);
        if (group == null)
            return false;

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync(ct);

        return true;
    }
}
