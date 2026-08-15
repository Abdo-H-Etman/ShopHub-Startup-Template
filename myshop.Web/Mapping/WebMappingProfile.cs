using AutoMapper;
using myshop.BLL.DTOs.Account;
using myshop.Entities.ViewModels.Account;

namespace myshop.Web.Mapping;

public class WebMappingProfile : Profile
{
    public WebMappingProfile()
    {
        CreateMap<RegisterVM, RegisterDto>();
        CreateMap<LoginVM, LoginDto>();
    }
}