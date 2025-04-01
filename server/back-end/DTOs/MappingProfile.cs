using AutoMapper;

namespace back_end.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Event, EventDTO>();
            CreateMap<EventDTO, Event>();
        }
    }
}
