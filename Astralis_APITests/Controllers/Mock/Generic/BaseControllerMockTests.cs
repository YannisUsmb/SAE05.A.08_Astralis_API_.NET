using Astralis_API.Models.Mapper;
using AutoMapper;

namespace Astralis_APITests.Controllers
{
    public abstract class BaseControllerMockTests
    {
        protected IMapper _mapper;

        [TestInitialize]
        public virtual void BaseInitialize()
        {
            MapperConfiguration configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
            });
            _mapper = configuration.CreateMapper();
        }
    }
}