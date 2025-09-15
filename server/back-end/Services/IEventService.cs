using back_end.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace back_end.Services
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event> GetEventByIdAsync(int id);
        Task AddEventAsync(Event eventItem);
        Task UpdateEventAsync(Event eventItem, string currentUserId);
        Task DeleteEventAsync(int id, string currentUserId);
        Task<IEnumerable<Event>> SearchEventsAsync(string? query = null, DateTime? from = null, DateTime? to = null, decimal? minPrice = null, decimal? maxPrice = null);
    }
}
