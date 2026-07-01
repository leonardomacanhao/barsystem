using AutoMapper;
using BarSystem.Core.Application.DTOs.Categories;
using BarSystem.Core.Domain.Entities;

namespace BarSystem.Core.Application.Mappings;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryResponseDto>();
        CreateMap<CreateCategoryDto, Category>()
            .ConstructUsing(dto => new Category(dto.Name, dto.Description));
        CreateMap<UpdateCategoryDto, Category>();
    }
}
