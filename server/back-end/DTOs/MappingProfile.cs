using AutoMapper;
using back_end.Enums;
using back_end.Models;
using Host = back_end.Models.Host;

namespace back_end.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Event to EventDTO
            CreateMap<Event, EventDTO>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
                .ForMember(dest => dest.DisabilityTags, opt => opt.MapFrom(src => src.DisabilityTags.Select(t => t.Name).ToList()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            // EventDTO to Event (for updates, not creation)
            CreateMap<EventDTO, Event>()
                .ForMember(dest => dest.Location, opt => opt.Ignore())
                .ForMember(dest => dest.City, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (EventStatus)src.Status))
                .ForMember(dest => dest.DisabilityTags, opt => opt.Ignore())
                .ForMember(dest => dest.Host, opt => opt.Ignore())
                .ForMember(dest => dest.UsersWhoSaved, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            // Host to HostDTO
            CreateMap<Host, HostDTO>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Events, opt => opt.MapFrom(src => src.Events));

            // User to UserDTO
            CreateMap<User, UserDTO>();
        }
    }
}
