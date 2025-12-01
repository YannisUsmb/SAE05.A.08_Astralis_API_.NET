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
                .ForMember(dest => dest.AuthorUsername, opt => opt.MapFrom(src => src.UserNavigation.Username))
                .ForMember(dest => dest.AuthorAvatarUrl, opt => opt.MapFrom(src => src.UserNavigation.AvatarUrl))
                // Computed Properties.
                .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.ArticleInterests.Count())) // Computed Property: LikesCount.
                .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.Comments.Count())); // Computed Property: CommentsCount.
            CreateMap<Article, ArticleDetailDto>()
                // .Include(a => a.UserNavigation)
                .ForMember(dest => dest.AuthorUsername, opt => opt.MapFrom(src => src.UserNavigation.Username))
                .ForMember(dest => dest.AuthorAvatarUrl, opt => opt.MapFrom(src => src.UserNavigation.AvatarUrl))
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

            ///// CartItem.
            // Entity to DTO (Read).
            CreateMap<CartItem, CartItemDto>()
                // .Include(ci => ci.ProductNavigation)
                .ForMember(dest => dest.ProductLabel, opt => opt.MapFrom(src => src.ProductNavigation.Label))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.ProductNavigation.Price));
            // DTO to Entity (Write).
            CreateMap<CartItemCreateDto, CartItem>();
            CreateMap<CartItemUpdateDto, CartItem>();

            ///// CelestialBody.
            // Entity to DTO (Read).
            CreateMap<CelestialBody, CelestialBodyListDto>()
                .ForMember(dest => dest.CelestialBodyTypeName, opt => opt.MapFrom(src => src.CelestialBodyTypeNavigation.Label));
            // DTO to Entity (Write).
            CreateMap<CelestialBodyCreateDto, CelestialBody>();
            CreateMap<CelestialBodyUpdateDto, CelestialBody>();

            ///// CelestialBodyType.
            // Entity to DTO (Read).
            CreateMap<CelestialBodyType, CelestialBodyTypeDto>();

            ///// City.
            // Entity to DTO (Read).
            CreateMap<City, CityDto>()
                // .Include(c => c.CountryNavigation)
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.CountryNavigation.Name));

            ///// Comet.
            // Entity to DTO (Read).
            CreateMap<Comet, CometDto>()
                // .Include(c => c.CelestialBodyNavigation)
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CelestialBodyNavigation.Name))
                .ForMember(dest => dest.Alias, opt => opt.MapFrom(src => src.CelestialBodyNavigation.Alias));
            // DTO to Entity (Write).
            CreateMap<CometUpdateDto, Comet>();
            CreateMap<CometCreateDto, CelestialBody>(); // DTO to Parent Entity.
            CreateMap<CometCreateDto, Comet>() // DTO to Child Entity.
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CelestialBodyId, opt => opt.Ignore());

            ///// Command.
            // Entity to DTO (Read).
            CreateMap<Command, CommandListDto>()
                // .Include(c => c.CommandStatusNavigation)
                .ForMember(dest => dest.CommandStatusLabel, opt => opt.MapFrom(src => src.CommandStatusNavigation.Label))
                // Computed Property.
                .ForMember(dest => dest.CountItems, opt => opt.MapFrom(src => 
                    src.OrderDetails.Sum(od => od.Quantity)));
            CreateMap<Command, CommandDetailDto>()
                // .Include(c => c.CommandStatusNavigation)
                .ForMember(dest => dest.CommandStatusLabel, opt => opt.MapFrom(src => src.CommandStatusNavigation.Label));
            // DTO to Entity (Write).
            CreateMap<CommandUpdateDto, Command>();

            ///// CommandStatus.
            // Entity to DTO (Read).
            CreateMap<CommandStatus, CommandStatusDto>();

            ///// Comment.
            // Entity to DTO (Read).
            CreateMap<Comment, CommentDto>()
                // .Include(c => c.UserNavigation)
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserNavigation.Username))
                .ForMember(dest => dest.UserAvatarUrl, opt => opt.MapFrom(src => src.UserNavigation.AvatarUrl));
            // DTO to Entity (Write).
            CreateMap<CommentCreateDto, Comment>();
            CreateMap<CommentUpdateDto, Comment>();

            ///// Country.
            // Entity to DTO (Read).
            CreateMap<Country, CountryDto>();

            ///// CreditCard.
            // Entity to DTO (Read).
            CreateMap<CreditCard, CreditCardDto>();

            ///// DetectionMethod.
            // Entity to DTO (Read).
            CreateMap<DetectionMethod, DetectionMethodDto>();

            ///// Discovery.
            // Entity to DTO (Read).
            CreateMap<Discovery, DiscoveryDto>()
                // .Include(d => d.CelestialBodyNavigation)
                .ForMember(dest => dest.CelestialBodyName, opt => opt.MapFrom(src => src.CelestialBodyNavigation.Name))
                // .Include(d => d.UserNavigation)
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserNavigation.Username))
                // .Include(d => d.DiscoveryStatusNavigation)
                .ForMember(dest => dest.DiscoveryStatusLabel, opt => opt.MapFrom(src => src.DiscoveryStatusNavigation.Label))
                // .Include(d => d.AliasStatusNavigation)
                .ForMember(dest => dest.AliasStatusLabel, opt => opt.MapFrom(src => src.AliasStatusNavigation != null ? src.AliasStatusNavigation.Label : null));
            // DTO to Entity (Write).
            CreateMap<DiscoveryCreateDto, Discovery>();
            CreateMap<DiscoveryUpdateDto, Discovery>();
            CreateMap<DiscoveryModerationDto, Discovery>();

            ///// DiscoveryStatus.
            // Entity to DTO (Read).
            CreateMap<DiscoveryStatus, DiscoveryStatusDto>();

            ///// Event.
            // Entity to DTO (Read).
            CreateMap<Event, EventDto>()
                // .Include(e => e.EventTypeNavigation)
                .ForMember(dest => dest.EventTypeLabel, opt => opt.MapFrom(src => src.EventTypeNavigation.Label))
                // Computed Property.
                .ForMember(dest => dest.InterestCount, opt => opt.MapFrom(src => 
                    src.EventInterests.Count()));
            // DTO to Entity (Write).
            CreateMap<EventCreateDto, Event>();
            CreateMap<EventUpdateDto, Event>();

            ///// EventInterest.
            // Entity to DTO (Read).
            CreateMap<EventInterest, EventInterestDto>();

            ///// EventType.
            // Entity to DTO (Read).
            CreateMap<EventType, EventTypeDto>();

            ///// GalaxyQuasar.
            // Entity to DTO (Read).
            CreateMap<GalaxyQuasar, GalaxyQuasarDto>()
                // .Include(gq => gq.CelestialBodyNavigation)
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CelestialBodyNavigation.Name))
                .ForMember(dest => dest.Alias, opt => opt.MapFrom(src => src.CelestialBodyNavigation.Alias))
                // .Include(gq => gq.GalaxyQuasarClassNavigation)
                .ForMember(dest => dest.GalaxyQuasarClassName, opt => opt.MapFrom(src => src.GalaxyQuasarClassNavigation.Label));
            // DTO to Entity (Write).
            CreateMap<GalaxyQuasarUpdateDto, GalaxyQuasar>();
            CreateMap<GalaxyQuasarCreateDto, CelestialBody>(); // DTO to Parent Entity.
            CreateMap<GalaxyQuasarCreateDto, GalaxyQuasar>() // DTO to Child Entity.
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CelestialBodyId, opt => opt.Ignore());

            ///// GalaxyQuasarClass.
            // Entity to DTO (Read).
            CreateMap<GalaxyQuasarClass, GalaxyQuasarClassDto>();

            ///// Notification.
            // Entity to DTO (Read).
            //CreateMap<Notification, MyNotificationDto>()
            //    // .Include(n => n.NotificationTypeNavigation)
            //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.NotificationNavigation.Id))
            //    .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.NotificationNavigation.Label))
            //    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.NotificationNavigation.Description))
            // A RETRAVAILLER CAR MODELS MAL FAIT !!!

            ///// OrderDetail.
            // Entity to DTO (Read).
            CreateMap<OrderDetail, OrderDetailDto>()
                // .Include(od => od.ProductNavigation)
                .ForMember(dest => dest.ProductLabel, opt => opt.MapFrom(src => src.ProductNavigation.Label))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.ProductNavigation.Price));

            ///// PhonePrefix.
            // Entity to DTO (Read).
            CreateMap<PhonePrefix, PhonePrefixDto>();
        }
    }
}
