using back_end.Models;
using back_end.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;

namespace back_end.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserService _userService;
        private readonly ILogger<EventService> _logger;
        public EventService(IEventRepository eventRepository, IUserService userService, ILogger<EventService> logger)
        {
            _eventRepository = eventRepository;
            _userService = userService;
            _logger = logger;
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            try
            {
                return await _eventRepository.GetAllEventsAsync();
            }
            catch (Exception ex)
            {
                throw new DataException("An error occurred while retrieving all events.", ex);
            }
        }

        public async Task<Event?> GetEventByIdAsync(int id)
        {
            try
            {
                var eventItem = await _eventRepository.GetEventByIdAsync(id);
                return eventItem;
            }
            catch (Exception ex)
            {
                throw new DataException("An error occurred while retrieving the event.", ex);
            }
        }

        public async Task AddEventAsync(Event eventItem)
        {
            try
            {
                await _eventRepository.AddEventAsync(eventItem);
            }
            catch (Exception ex)
            {
                throw new DataException("An error occurred while adding the event.", ex);
            }
        }

        public async Task UpdateEventAsync(Event eventItem, string currentUserId)
        {
            try
            {
                var existingEvent = await _eventRepository.GetEventByIdIncludingCanceledAsync(eventItem.Id);
                if (existingEvent == null)
                {
                    throw new DataException("Event not found.");
                }

                // Check if current user is the host or an admin
                bool isAuthorized = existingEvent.HostId.ToString() == currentUserId;
                
                if (!isAuthorized && int.TryParse(currentUserId, out int userId))
                {
                    var currentUser = await _userService.GetUserByIdAsync(userId);
                    isAuthorized = currentUser?.UserType == UserType.Admin;
                }

                if (!isAuthorized)
                {
                    throw new UnauthorizedAccessException("You are not authorized to update this event.");
                }

                await _eventRepository.UpdateEventAsync(eventItem);
            }
            catch (Exception ex)
            {
                throw new DataException("An error occurred while updating the event.", ex);
            }
        }

        public async Task DeleteEventAsync(int id, string currentUserId)
        {
            try
            {
                var existingEvent = await _eventRepository.GetEventByIdIncludingCanceledAsync(id);
                if (existingEvent == null)
                {
                    throw new DataException("Event not found.");
                }

                // Check if current user is the host or an admin
                bool isAuthorized = existingEvent.HostId.ToString() == currentUserId;
                
                if (!isAuthorized && int.TryParse(currentUserId, out int userId))
                {
                    var currentUser = await _userService.GetUserByIdAsync(userId);
                    isAuthorized = currentUser?.UserType == UserType.Admin;
                }

                if (!isAuthorized)
                {
                    throw new UnauthorizedAccessException("You are not authorized to delete this event.");
                }

                await _eventRepository.DeleteEventAsync(id);
            }
            catch (Exception ex)
            {
                throw new DataException("An error occurred while deleting the event.", ex);
            }
        }

        public async Task<IEnumerable<Event>> SearchEventsAsync(string? query = null, DateTime? from = null, DateTime? to = null, decimal? minPrice = null, decimal? maxPrice = null, List<string>? disabilityTags = null, List<string>? cities = null, int? age = null)
        {
            try
            {
                return await _eventRepository.SearchEventsAsync(query, from, to, minPrice, maxPrice, disabilityTags, cities, age);
            }
            catch (Exception ex)
            {
                throw new DataException("An error occurred while searching events.", ex);
            }
        }
    }
}
