using BarSystem.Core.Application.DTOs.Products;
using BarSystem.Core.Application.Validators.Products;
using BarSystem.Core.Domain.Entities;
using BarSystem.Core.Infra.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BarSystem.Core.Application.Services;

public class ProductAppService
{
    private readonly CoreDbContext _context;
    private readonly ILogger<ProductAppService> _logger;

    public ProductAppService(CoreDbContext context, ILogger<ProductAppService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var products = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync(ct);

        return products.Select(p => new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name,
            ImageUrl = p.ImageUrl,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();
    }

    public async Task<IEnumerable<ProductResponseDto>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default)
    {
        var products = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .OrderBy(p => p.Name)
            .ToListAsync(ct);

        return products.Select(p => new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name,
            ImageUrl = p.ImageUrl,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();
    }

    public async Task<ProductResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (product == null)
            return null;

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
    {
        var validator = new CreateProductValidator();
        var validationResult = await validator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }).ToList();
            throw new ValidationException("Erro de validação", validationResult.Errors);
        }

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.CategoryId, ct);
        if (category == null)
        {
            throw new ValidationException("Categoria não encontrada", new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("CategoryId", $"Categoria com ID {dto.CategoryId} não existe")
            });
        }

        var product = new Product(dto.Name, dto.Description, dto.Price, dto.CategoryId);
        
        if (!string.IsNullOrEmpty(dto.ImageUrl))
            product.SetImage(dto.ImageUrl);

        try
        {
            await _context.Products.AddAsync(product, ct);
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Erro ao salvar produto no banco. Inner exception: {InnerEx}", dbEx.InnerException?.Message);
            throw new Exception($"Erro ao salvar no banco: {dbEx.InnerException?.Message ?? dbEx.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar produto");
            throw;
        }

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            CategoryName = category.Name,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    public async Task<ProductResponseDto?> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken ct = default)
    {
        var validator = new UpdateProductValidator();
        var validationResult = await validator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
            throw new ValidationException("Erro de validação", validationResult.Errors);

        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (product == null)
            return null;

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.CategoryId, ct);
        if (category == null)
            throw new ValidationException("Categoria não encontrada", new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("CategoryId", $"Categoria com ID {dto.CategoryId} não existe")
            });

        product.Update(dto.Name, dto.Description, dto.Price, dto.CategoryId);
        
        if (!string.IsNullOrEmpty(dto.ImageUrl))
            product.SetImage(dto.ImageUrl);

        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Erro ao atualizar produto no banco. Inner exception: {InnerEx}", dbEx.InnerException?.Message);
            throw new Exception($"Erro ao salvar no banco: {dbEx.InnerException?.Message ?? dbEx.Message}");
        }

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            CategoryName = category.Name,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (product == null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(ct);

        return true;
    }
}
