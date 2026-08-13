using AutoMapper;
using myshop.BLL.DTOs.Category;
using myshop.BLL.DTOs.Product;
using myshop.Entities.Models;

namespace myshop.BLL.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryListDto>().ReverseMap();
        CreateMap<CreateCategoryDto, Category>().ReverseMap();
        CreateMap<UpdateCategoryDto, Category>().ReverseMap();

        CreateMap<Product, ProductListDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ReverseMap()
            .ForMember(dest => dest.Category, opt => opt.Ignore());
        CreateMap<CreateProductDto, Product>().ReverseMap();
        CreateMap<UpdateProductDto, Product>().ReverseMap();
    }
}
