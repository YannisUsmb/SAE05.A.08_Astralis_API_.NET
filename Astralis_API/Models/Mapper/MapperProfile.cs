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
        }
    }
}
