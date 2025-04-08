using AutoMapper;
using back_end.DTOs;
using back_end.Models;
using Xunit;

namespace back_end_tests.DTOs
{
    public class EventDTOTests
    {
        private readonly IMapper _mapper;

        public EventDTOTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public void EventToEventDTO_MapsCorrectly()
        {
            // Arrange
            var ev = new Event
            {
                Id = 1,
                Title = "Tech Conference",
                Location = "Toronto",
                ImageUrl = "image.jpg",
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now.AddHours(2),
                Price = 49.99m,
                Url = "https://event.com"
            };

            // Act
            var dto = _mapper.Map<EventDTO>(ev);

            // Assert
            Assert.Equal(ev.Id, dto.Id);
            Assert.Equal(ev.Title, dto.Title);
            Assert.Equal(ev.Location, dto.Location);
            Assert.Equal(ev.ImageUrl, dto.ImageUrl);
            Assert.Equal(ev.StartDateTime, dto.StartDateTime);
            Assert.Equal(ev.EndDateTime, dto.EndDateTime);
            Assert.Equal(ev.Price, dto.Price);
            Assert.Equal(ev.Url, dto.Url);
        }

        [Fact]
        public void EventDTOToEvent_MapsCorrectly()
        {
            // Arrange
            var dto = new EventDTO
            {
                Id = 2,
                Title = "Hackathon",
                Location = "Waterloo",
                ImageUrl = "hack.jpg",
                StartDateTime = DateTime.Today,
                EndDateTime = DateTime.Today.AddDays(1),
                Price = 0,
                Url = "https://hack.com"
            };

            // Act
            var ev = _mapper.Map<Event>(dto);

            // Assert
            Assert.Equal(dto.Id, ev.Id);
            Assert.Equal(dto.Title, ev.Title);
            Assert.Equal(dto.Location, ev.Location);
            Assert.Equal(dto.ImageUrl, ev.ImageUrl);
            Assert.Equal(dto.StartDateTime, ev.StartDateTime);
            Assert.Equal(dto.EndDateTime, ev.EndDateTime);
            Assert.Equal(dto.Price, ev.Price);
            Assert.Equal(dto.Url, ev.Url);
        }
    }
}
