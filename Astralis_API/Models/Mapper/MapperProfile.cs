using AutoMapper;
using Astralis.Shared.DTOs;
using Astralis_API.Models.EntityFramework;

namespace Astralis_API.Models.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            ///// Address.
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

            ///// Alias Status.
            // Entity to DTO (Read).
            CreateMap<AliasStatus, AliasStatusDto>();

            ///// Article.
            // Entity to DTO (Read).
            CreateMap<Article, ArticleListDto>()
                // Computed Property: Preview.
                .ForMember(dest => dest.Preview, opt => opt.MapFrom(src => 
                    (src.Content.Length > 100) ? src.Content.Substring(0, 100) + "..." : src.Content))
                // .Include(a => a.UserNavigation)
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserNavigation.Id))
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.UserNavigation.Username))
                .ForMember(dest => dest.AuthorAvatarPath, opt => opt.MapFrom(src => src.UserNavigation.AvatarUrl))
                // Computed Property: LikesCount.
                .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.ArticleInterests.Count()))
                // Computed Property: CommentsCount.
                .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.Comments.Count()));
            CreateMap<Article, ArticleDetailDto>()
                // .Include(a => a.UserNavigation)
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserNavigation.Id))
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.UserNavigation.Username))
                .ForMember(dest => dest.AuthorAvatarPath, opt => opt.MapFrom(src => src.UserNavigation.AvatarUrl))
                // .Include(a => a.TypesOfArticle)
                .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => 
                    src.TypesOfArticle.Select(toa => toa.ArticleTypeId).ToList()))
                // .ThenInclude(toa => toa.ArticleTypeNavigation)
                .ForMember(dest => dest.CategoryNames, opt => opt.MapFrom(src => 
                    src.TypesOfArticle.Select(toa => toa.ArticleTypeNavigation.Label).ToList()));
            // DTO to Entity (Write).
            CreateMap<ArticleCreateDto, Article>();
            CreateMap<ArticleUpdateDto, Article>();

            ///// ArticleInterest.
            // Entity to DTO (Read).
            CreateMap<ArticleInterest, ArticleInterestDto>();

            ///// ArticleType.
            // Entity to DTO (Read).
            CreateMap<ArticleType, ArticleTypeDto>();

            ///// Asteroid.
            // Entity to DTO (Read).
            CreateMap<Asteroid, AsteroidDto>()
                // .Include(a => a.CelestialBodyNavigation)
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CelestialBodyNavigation.Name))
                .ForMember(dest => dest.Alias, opt => opt.MapFrom(src => src.CelestialBodyNavigation.Alias))
                // .ThenInclude(cb => cb.OrbitalClassNavigation)
                .ForMember(dest => dest.OrbitalClassName, opt => opt.MapFrom(src => src.OrbitalClassNavigation.Label))
                .ForMember(dest => dest.OrbitalClassDescription, opt => opt.MapFrom(src => src.OrbitalClassNavigation.Description));
            // DTO to Entity (Write).
            CreateMap<AsteroidUpdateDto, Asteroid>();
            CreateMap<AsteroidCreateDto, CelestialBody>(); // DTO to Parent Entity.
            CreateMap<AsteroidCreateDto, Asteroid>() // DTO to Child Entity.
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CelestialBodyId, opt => opt.Ignore());

            ///// Audio.
            // Entity to DTO (Read).
            CreateMap<Audio, AudioDto>();
        }
    }
}
