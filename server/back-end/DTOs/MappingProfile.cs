using AutoMapper;
using back_end.Enums;
using back_end.Models;

namespace back_end.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Event to EventDTO
            CreateMap<Event, EventDTO>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
                .ForMember(dest => dest.DisabilityTags, opt => opt.MapFrom(src => src.DisabilityTags.Select(t => t.Name).ToList()));

            // EventDTO to Event (for updates, not creation)
            CreateMap<EventDTO, Event>()
                .ForMember(dest => dest.Location, opt => opt.Ignore()) // Handle separately in controller
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (EventStatus)src.Status))
                .ForMember(dest => dest.DisabilityTags, opt => opt.Ignore()) // Handle separately in controller
                .ForMember(dest => dest.Host, opt => opt.Ignore())
                .ForMember(dest => dest.UsersWhoSaved, opt => opt.Ignore());
        }
    }
}
