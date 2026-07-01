using AutoMapper;
using BarSystem.Core.Application.DTOs.Products;
using BarSystem.Core.Domain.Entities;

namespace BarSystem.Core.Application.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.Ignore());

        CreateMap<CreateProductDto, Product>()
            .ConstructUsing(dto => new Product(dto.Name, dto.Description, dto.Price, dto.CategoryId))
            .AfterMap((dto, product) =>
            {
                if (!string.IsNullOrEmpty(dto.ImageUrl))
                    product.SetImage(dto.ImageUrl);
            });

        CreateMap<UpdateProductDto, Product>()
            .AfterMap((dto, product) =>
            {
                if (!string.IsNullOrEmpty(dto.ImageUrl))
                    product.SetImage(dto.ImageUrl);
            });
    }
}
