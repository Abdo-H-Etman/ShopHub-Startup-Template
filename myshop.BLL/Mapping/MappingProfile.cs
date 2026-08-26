using AutoMapper;
using myshop.BLL.DTOs.Category;
using myshop.BLL.DTOs.Order;
using myshop.BLL.DTOs.Product;
using myshop.BLL.DTOs.Review;
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
            .ForMember(dest => dest.AverageRating,
                opt => opt.MapFrom(src => src.Reviews != null && src.Reviews.Any(r => !r.IsDeleted)
                    ? Math.Round(src.Reviews.Where(r => !r.IsDeleted).Average(r => r.Rating), 1)
                    : 0))
            .ForMember(dest => dest.ReviewCount,
                opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count(r => !r.IsDeleted) : 0))
            .ReverseMap()
            .ForMember(dest => dest.Category, opt => opt.Ignore());

        CreateMap<CreateProductDto, Product>().ReverseMap();
        CreateMap<UpdateProductDto, Product>().ReverseMap();

        // Order Mappings
        CreateMap<OrderHeader, OrderHeaderDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.ApplicationUser != null ? src.ApplicationUser.Name : src.Name))
            .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.ApplicationUser != null ? src.ApplicationUser.Email : string.Empty))
            .ReverseMap();

        CreateMap<OrderDetail, OrderDetailDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
            .ForMember(dest => dest.ProductImg, opt => opt.MapFrom(src => src.Product != null ? src.Product.Img : string.Empty))
            .ReverseMap();

        // Review Mappings
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : string.Empty))
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : string.Empty))
            .ReverseMap();
    }
}
