using Project.Areas.Identity.Data;
using Project.Models;
using AutoMapper;

namespace Project.Mapper
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Product, ProductModel>()
                 .ForMember(dest => dest.Picture, opt => opt.Ignore());

        }
    }
}
