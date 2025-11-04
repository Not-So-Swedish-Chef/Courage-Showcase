using System.Collections.Generic;
using System.Threading.Tasks;
using back_end.Models;
using back_end.Enums;
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
                    .Include(u => u.SavedEvents.Where(e => e.Status != back_end.Enums.EventStatus.Canceled))
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
                if (eventItem == null || eventItem.Status == back_end.Enums.EventStatus.Canceled)
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

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return new List<User>();
            }
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            try
            {
                return await _context.Users.FindAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user {userId}");
                return null;
            }
        }

        public async Task<bool> SuspendUserAsync(int userId, int days)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return false;

                user.Status = UserStatus.Suspended;
                user.SuspensionEndDate = DateTime.UtcNow.AddDays(days);
                
                await _context.SaveChangesAsync();
                _logger.LogInformation($"User {userId} suspended for {days} days until {user.SuspensionEndDate}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error suspending user {userId}");
                return false;
            }
        }

        public async Task<bool> BanUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return false;

                user.Status = UserStatus.Banned;
                user.SuspensionEndDate = null;

                // If user is a host, cancel all their active events
                if (user.UserType == UserType.Host)
                {
                    var host = await _context.Hosts
                        .Include(h => h.Events)
                        .FirstOrDefaultAsync(h => h.Id == userId);
                    
                    if (host != null)
                    {
                        var activeEvents = host.Events.Where(e => e.Status == EventStatus.Active).ToList();
                        foreach (var eventItem in activeEvents)
                        {
                            eventItem.Status = EventStatus.Canceled;
                        }
                        _logger.LogInformation($"Canceled {activeEvents.Count} active events for banned host {userId}");
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"User {userId} has been permanently banned");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error banning user {userId}");
                return false;
            }
        }

        public async Task<bool> UnsuspendUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null || user.Status != UserStatus.Suspended)
                    return false;

                user.Status = UserStatus.Active;
                user.SuspensionEndDate = null;
                
                await _context.SaveChangesAsync();
                _logger.LogInformation($"User {userId} has been unsuspended");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error unsuspending user {userId}");
                return false;
            }
        }

        public async Task<bool> UnbanUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null || user.Status != UserStatus.Banned)
                    return false;

                user.Status = UserStatus.Active;
                user.SuspensionEndDate = null;
                
                await _context.SaveChangesAsync();
                _logger.LogInformation($"User {userId} has been unbanned");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error unbanning user {userId}");
                return false;
            }
        }
    }
}