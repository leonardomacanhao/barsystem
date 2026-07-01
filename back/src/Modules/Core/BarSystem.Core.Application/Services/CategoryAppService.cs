using AutoMapper;
using BarSystem.Core.Application.DTOs.Categories;
using BarSystem.Core.Application.Validators.Categories;
using BarSystem.Core.Domain.Entities;
using BarSystem.Core.Infra.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BarSystem.Core.Application.Services;

public class CategoryAppService
{
    private readonly CoreDbContext _context;
    private readonly IMapper _mapper;

    public CategoryAppService(CoreDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(ct);

        return _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
    }

    public async Task<CategoryResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        return category == null ? null : _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default)
    {
        var validator = new CreateCategoryValidator();
        var validationResult = await validator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var category = _mapper.Map<Category>(dto);
        
        await _context.Categories.AddAsync(category, ct);
        await _context.SaveChangesAsync(ct);

        return _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task<CategoryResponseDto?> UpdateAsync(Guid id, UpdateCategoryDto dto, CancellationToken ct = default)
    {
        var validator = new UpdateCategoryValidator();
        var validationResult = await validator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category == null)
            return null;

        category.Update(dto.Name, dto.Description);
        
        await _context.SaveChangesAsync(ct);

        return _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category == null)
            return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(ct);

        return true;
    }
}
