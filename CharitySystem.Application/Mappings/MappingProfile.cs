using AutoMapper;
using CharitySystem.Application.DTOs.Auth;
using CharitySystem.Application.DTOs.Clothes;
using CharitySystem.Domain.Entities;

namespace CharitySystem.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, AuthResponseDto>();
            CreateMap<Cloth, ClothDto>()
                .ForMember(dest => dest.Category, 
                opt => opt.MapFrom(src => 
                src.Category.ToString())); 

            CreateMap<CreateClothDto, Cloth>();

        }
    }
}
