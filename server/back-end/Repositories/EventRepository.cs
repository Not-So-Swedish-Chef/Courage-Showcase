using back_end.Models;
using back_end.Utils;
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
                return await _context.Events
                    .Include(e => e.DisabilityTags)
                    .Include(e => e.Host)
                    .Where(e => e.Status != back_end.Enums.EventStatus.Canceled)
                    .ToListAsync();
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
                return await _context.Events
                    .Include(e => e.DisabilityTags)
                    .Include(e => e.Host)
                    .Where(e => e.Status != back_end.Enums.EventStatus.Canceled)
                    .FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error retrieving event.");
                throw;
            }
        }

        public async Task<Event> GetEventByIdIncludingCanceledAsync(int id)
        {
            try
            {
                return await _context.Events
                    .Include(e => e.DisabilityTags)
                    .Include(e => e.Host)
                    .FirstOrDefaultAsync(e => e.Id == id);
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
                // Tags are already handled in the controller, just save the event
                await _context.Events.AddAsync(eventItem);
                await _context.SaveChangesAsync();
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error adding event.");
                throw;
            }
        }

        public async Task UpdateEventAsync(Event eventItem)
        {
            try
            {
                var existingEvent = await _context.Events
                    .Include(e => e.DisabilityTags)
                    .FirstOrDefaultAsync(e => e.Id == eventItem.Id);
                    
                if (existingEvent != null)
                {
                    // Update scalar properties
                    _context.Entry(existingEvent).CurrentValues.SetValues(eventItem);
                    
                    // Update DisabilityTags collection
                    existingEvent.DisabilityTags.Clear();
                    foreach (var tag in eventItem.DisabilityTags)
                    {
                        var existingTag = await _context.DisabilityTags.FindAsync(tag.Id);
                        if (existingTag != null)
                        {
                            existingEvent.DisabilityTags.Add(existingTag);
                        }
                    }
                    
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
                    eventItem.Status = back_end.Enums.EventStatus.Canceled;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error canceling event.");
                throw;
            }
        }

        public async Task<IEnumerable<Event>> SearchEventsAsync(string? query = null, DateTime? from = null, DateTime? to = null, decimal? minPrice = null, decimal? maxPrice = null, List<string>? disabilityTags = null, List<string>? locations = null, int? age = null)
        {
            try
            {
                var eventsQuery = _context.Events
                    .Include(e => e.DisabilityTags)
                    .Where(e => e.Status != back_end.Enums.EventStatus.Canceled)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(query))
                {
                    var queryLower = query.ToLowerInvariant();
                    eventsQuery = eventsQuery.Where(e => 
                        e.Title.Contains(query) || 
                        e.Location.ToString().ToLower().Contains(queryLower));
                }

                if (from.HasValue)
                {
                    eventsQuery = eventsQuery.Where(e => e.StartDateTime >= from.Value);
                }

                if (to.HasValue)
                {
                    eventsQuery = eventsQuery.Where(e => e.StartDateTime <= to.Value);
                }

                if (minPrice.HasValue)
                {
                    eventsQuery = eventsQuery.Where(e => e.Price >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    eventsQuery = eventsQuery.Where(e => e.Price <= maxPrice.Value);
                }

                if (disabilityTags != null && disabilityTags.Any())
                {
                    var normalizedInputTags = disabilityTags
                        .Select(tag => TagNormalizer.Normalize(tag))
                        .ToList();

                    var matchingTagIds = await _context.DisabilityTags
                        .Where(dt => normalizedInputTags.Contains(dt.NormalizedName))
                        .Select(dt => dt.Id)
                        .ToListAsync();

                    if (matchingTagIds.Any())
                    {
                        eventsQuery = eventsQuery.Where(e => 
                            e.DisabilityTags.Any(dt => matchingTagIds.Contains(dt.Id)));
                    }
                    else
                    {
                        return new List<Event>();
                    }
                }

                if (locations != null && locations.Any())
                {
                    var normalizedLocations = locations
                        .Select(loc => loc.ToLowerInvariant().Trim())
                        .ToList();

                    eventsQuery = eventsQuery.Where(e => 
                        normalizedLocations.Contains(e.Location.ToString().ToLower()));
                }

                if (age.HasValue)
                {
                    eventsQuery = eventsQuery.Where(e => 
                        (!e.MinAge.HasValue || e.MinAge.Value <= age.Value) &&
                        (!e.MaxAge.HasValue || e.MaxAge.Value >= age.Value));
                }

                return await eventsQuery.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error searching events.");
                throw;
            }
        }
    }
}
