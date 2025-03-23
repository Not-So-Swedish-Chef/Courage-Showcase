using back_end.Models;
using back_end.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace back_end.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        // Return all events from the data source for use in listings or displays
        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _eventRepository.GetAllEventsAsync();
        }

        // Fetch a single event by its ID for detail view or editing
        public async Task<Event> GetEventByIdAsync(int id)
        {
            return await _eventRepository.GetEventByIdAsync(id);
        }

        // Pass the event to repository for creation
        public async Task AddEventAsync(Event eventItem)
        {
            await _eventRepository.AddEventAsync(eventItem);
        }

        // Send the updated event to the repository for persistence
        public async Task UpdateEventAsync(Event eventItem)
        {
            await _eventRepository.UpdateEventAsync(eventItem);
        }

        // Forward the delete request to the repository by event ID
        public async Task DeleteEventAsync(int id)
        {
            await _eventRepository.DeleteEventAsync(id);
        }
    }
}
