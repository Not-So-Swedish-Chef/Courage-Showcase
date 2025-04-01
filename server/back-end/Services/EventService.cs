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
        private readonly ILogger<EventService> _logger;
        public EventService(IEventRepository eventRepository, ILogger<EventService> logger)
        {
            _eventRepository = eventRepository;
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
                return eventItem ?? throw new DataException("Event not found.");
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
                var existingEvent = await _eventRepository.GetEventByIdAsync(eventItem.Id);
                if (existingEvent == null)
                {
                    throw new DataException("Event not found.");
                }

                if (existingEvent.HostId.ToString() != currentUserId)
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
                var existingEvent = await _eventRepository.GetEventByIdAsync(id);
                if (existingEvent == null)
                {
                    throw new DataException("Event not found.");
                }

                if (existingEvent.HostId.ToString() != currentUserId)
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
    }
}
