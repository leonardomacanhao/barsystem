using BarSystem.Core.Application.DTOs.Tables;
using BarSystem.Core.Application.Validators.Tables;
using BarSystem.Core.Domain.Entities;
using BarSystem.Core.Infra.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BarSystem.Core.Application.Services;

public class TableAppService
{
    private readonly CoreDbContext _context;
    private readonly ILogger<TableAppService> _logger;

    public TableAppService(CoreDbContext context, ILogger<TableAppService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<TableResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var tables = await _context.Tables
            .AsNoTracking()
            .OrderBy(t => t.Number)
            .ToListAsync(ct);

        return tables.Select(t => new TableResponseDto
        {
            Id = t.Id,
            Number = t.Number,
            Capacity = t.Capacity,
            Status = t.Status,
            Location = t.Location,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        }).ToList();
    }

    public async Task<TableResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var table = await _context.Tables
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (table == null)
            return null;

        return new TableResponseDto
        {
            Id = table.Id,
            Number = table.Number,
            Capacity = table.Capacity,
            Status = table.Status,
            Location = table.Location,
            CreatedAt = table.CreatedAt,
            UpdatedAt = table.UpdatedAt
        };
    }

    public async Task<TableResponseDto> CreateAsync(CreateTableDto dto, CancellationToken ct = default)
    {
        var validator = new CreateTableValidator();
        var validationResult = await validator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
            throw new ValidationException("Erro de validação", validationResult.Errors);

        // Verifica se já existe mesa com esse número
        var exists = await _context.Tables.AnyAsync(t => t.Number == dto.Number, ct);
        if (exists)
        {
            throw new ValidationException("Número da mesa já existe", new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("Number", $"Já existe uma mesa com o número {dto.Number}")
            });
        }

        var table = new Table(dto.Number, dto.Capacity, dto.Location);

        try
        {
            await _context.Tables.AddAsync(table, ct);
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Erro ao salvar mesa no banco");
            throw new Exception($"Erro ao salvar no banco: {dbEx.InnerException?.Message ?? dbEx.Message}");
        }

        return new TableResponseDto
        {
            Id = table.Id,
            Number = table.Number,
            Capacity = table.Capacity,
            Status = table.Status,
            Location = table.Location,
            CreatedAt = table.CreatedAt,
            UpdatedAt = table.UpdatedAt
        };
    }

    public async Task<TableResponseDto?> UpdateAsync(Guid id, UpdateTableDto dto, CancellationToken ct = default)
    {
        var validator = new UpdateTableValidator();
        var validationResult = await validator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
            throw new ValidationException("Erro de validação", validationResult.Errors);

        var table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (table == null)
            return null;

        // Verifica se já existe outra mesa com esse número
        var exists = await _context.Tables.AnyAsync(t => t.Number == dto.Number && t.Id != id, ct);
        if (exists)
        {
            throw new ValidationException("Número da mesa já existe", new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("Number", $"Já existe uma mesa com o número {dto.Number}")
            });
        }

        table.Update(dto.Number, dto.Capacity, dto.Location);

        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Erro ao atualizar mesa no banco");
            throw new Exception($"Erro ao salvar no banco: {dbEx.InnerException?.Message ?? dbEx.Message}");
        }

        return new TableResponseDto
        {
            Id = table.Id,
            Number = table.Number,
            Capacity = table.Capacity,
            Status = table.Status,
            Location = table.Location,
            CreatedAt = table.CreatedAt,
            UpdatedAt = table.UpdatedAt
        };
    }

    public async Task<TableResponseDto?> UpdateStatusAsync(Guid id, UpdateTableStatusDto dto, CancellationToken ct = default)
    {
        var validator = new UpdateTableStatusValidator();
        var validationResult = await validator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
            throw new ValidationException("Erro de validação", validationResult.Errors);

        var table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (table == null)
            return null;

        table.SetStatus(dto.Status);

        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Erro ao atualizar status da mesa");
            throw new Exception($"Erro ao salvar no banco: {dbEx.InnerException?.Message ?? dbEx.Message}");
        }

        return new TableResponseDto
        {
            Id = table.Id,
            Number = table.Number,
            Capacity = table.Capacity,
            Status = table.Status,
            Location = table.Location,
            CreatedAt = table.CreatedAt,
            UpdatedAt = table.UpdatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (table == null)
            return false;

        _context.Tables.Remove(table);
        await _context.SaveChangesAsync(ct);

        return true;
    }
}
