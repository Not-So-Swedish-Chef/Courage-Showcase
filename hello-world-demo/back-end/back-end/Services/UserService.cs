using System.Collections.Generic;
using System.Threading.Tasks;
using back_end.Models;
using Microsoft.EntityFrameworkCore;

namespace back_end.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Event>> GetSavedEventsAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.SavedEvents)
                .FirstOrDefaultAsync(u => u.Id == userId);
            return user?.SavedEvents;
        }

        public async Task<bool> SaveEventAsync(int userId, int eventId)
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

        public async Task<bool> RemoveSavedEventAsync(int userId, int eventId)
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
    }
}