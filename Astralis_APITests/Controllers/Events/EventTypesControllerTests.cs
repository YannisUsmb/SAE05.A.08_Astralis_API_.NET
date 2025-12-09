using Astralis.Shared.DTOs;
using Astralis_API.Controllers;
using Astralis_API.Models.EntityFramework;
using Astralis_API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Astralis_API.Tests.Controllers
{
    [TestClass]
    public class EventTypesControllerTests : ReadableControllerTests<EventType, EventTypesController, EventTypeDto, EventTypeDto, int>
    {
        protected override List<EventType> GetSampleEntities()
        {
            return new List<EventType>
            {
                new EventType { Id = 100, Label = "Test1" },
                new EventType { Id = 200, Label = "Test2" }
            };
        }
        protected override int GetDtoId(EventTypeDto dto) => dto.Id;
        protected override int GetNonExistentId()
        {
            return 0;
        }
        protected override int GetEntityId(EventType entity) => entity.Id;
        protected override Task<ActionResult<IEnumerable<EventTypeDto>>> ActionGetAll() => _controller.GetAll();
        protected override Task<ActionResult<EventTypeDto>> ActionGetById(int id) => _controller.GetById(id);
        protected override EventTypesController CreateController(AstralisDbContext context, IMapper mapper)
        {
            return new EventTypesController(new TestRepo(context), mapper);
        }

        private class TestRepo : IEventTypeRepository
        {
            private readonly AstralisDbContext _db;
            public TestRepo(AstralisDbContext db) { _db = db; }

            public async Task<IEnumerable<EventType>> GetAllAsync() => await _db.EventTypes.ToListAsync();
            public async Task<EventType?> GetByIdAsync(int id) => await _db.EventTypes.FindAsync(id);
        }

        [TestCleanup]
        public override void Cleanup()
        {
            var samples = GetSampleEntities();
            var sampleIds = samples.Select(s => s.Id).ToList();

            foreach (var sample in samples)
            {
                var entityInDb = _context.EventTypes.Find(sample.Id);
                if (entityInDb != null)
                {
                    _context.EventTypes.Remove(entityInDb);
                }
            }
            _context.SaveChanges();
        }
    }
}