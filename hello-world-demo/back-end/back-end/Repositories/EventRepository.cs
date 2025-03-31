using back_end.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace back_end.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EventRepository> _logger;

        public EventRepository(ApplicationDbContext context, ILogger<EventRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            try
            {
                return await _context.Events.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error retrieving events.");
                throw;
            }
        }


        public async Task<Event> GetEventByIdAsync(int id)
        {
            try
            {
                return await _context.Events.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error retrieving event.");
                throw;
            }
        }

        public async Task AddEventAsync(Event eventItem)
        {
            try
            {
                await _context.Events.AddAsync(eventItem);
                await _context.SaveChangesAsync();
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error updating event.");
                throw;
            }
        }

        public async Task UpdateEventAsync(Event eventItem)
        {
            try
            {
                var existingEvent = await _context.Events.FindAsync(eventItem.Id);
                if (existingEvent != null)
                {
                    _context.Entry(existingEvent).CurrentValues.SetValues(eventItem);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error saving event.");
                throw;
            }
        }

        public async Task DeleteEventAsync(int id)
        {
            try
            {
                var eventItem = await _context.Events.FindAsync(id);
                if (eventItem != null)
                {
                    _context.Events.Remove(eventItem);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error saving event.");
                throw;
            }
        }
    }
}
