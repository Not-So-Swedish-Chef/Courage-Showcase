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

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _eventRepository.GetAllEventsAsync();
        }

        public async Task<Event> GetEventByIdAsync(int id)
        {
            return await _eventRepository.GetEventByIdAsync(id);
        }

        public async Task AddEventAsync(Event eventItem)
        {
            await _eventRepository.AddEventAsync(eventItem);
        }

        public async Task UpdateEventAsync(Event eventItem)
        {
            await _eventRepository.UpdateEventAsync(eventItem);
        }

        public async Task DeleteEventAsync(int id)
        {
            await _eventRepository.DeleteEventAsync(id);
        }
    }
}
