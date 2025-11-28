using AutoMapper;
using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            // Address.

            // Entity to DTO (Read).
            CreateMap<Address, AddressDto>()
                // .Include(a => a.CityNavigation)
                .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.CityNavigation.Id))
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.CityNavigation.Name))
                .ForMember(dest => dest.PostCode, opt => opt.MapFrom(src => src.CityNavigation.PostCode))
                // .ThenInclude(c => c.CountryNavigation)
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CityNavigation.CountryNavigation.Id))
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.CityNavigation.CountryNavigation.Name));

            // DTO to Entity (Write).
            CreateMap<AddressCreateDto, Address>();
            CreateMap<AddressUpdateDto, Address>();
        }
    }
}
