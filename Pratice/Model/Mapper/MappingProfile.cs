using AutoMapper;
using Model.Models;
using Model.Models.DTO.Product;

namespace Model.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product Mappings
            CreateMap<Products, ProductDTO>().ReverseMap();
            CreateMap<CreateProductDTO, Products>();
            CreateMap<UpdateProductDTO, Products>();
        }
    }
}
