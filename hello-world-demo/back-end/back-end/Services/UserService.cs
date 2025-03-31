using System.Collections.Generic;
using System.Threading.Tasks;
using back_end.Models;
using Microsoft.EntityFrameworkCore;

namespace back_end.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Event>> GetSavedEventsAsync(int userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.SavedEvents)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                return user?.SavedEvents ?? new List<Event>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving saved events for user {userId}");
                return new List<Event>();
            }
        }

        public async Task<bool> SaveEventAsync(int userId, int eventId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.SavedEvents)
                    .FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return false;

                var eventItem = await _context.Events.FindAsync(eventId);
                if (eventItem == null)
                    return false;

                if (!user.SavedEvents.Contains(eventItem))
                {
                    user.SavedEvents.Add(eventItem);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving event {eventId} for user {userId}");
                return false;
            }
        }

        public async Task<bool> RemoveSavedEventAsync(int userId, int eventId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.SavedEvents)
                    .FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return false;

                var eventItem = await _context.Events.FindAsync(eventId);
                if (eventItem == null)
                    return false;

                if (user.SavedEvents.Contains(eventItem))
                {
                    user.SavedEvents.Remove(eventItem);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing event {eventId} for user {userId}");
                return false;
            }
        }
    }
}