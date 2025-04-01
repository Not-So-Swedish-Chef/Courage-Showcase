using System.Threading.Tasks;
using back_end.Controllers;
using back_end.Models;
using Microsoft.EntityFrameworkCore;
using Host = back_end.Models.Host;

namespace back_end.Services
{
    public class HostService : IHostService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HostService> _logger;

        public HostService(ApplicationDbContext context, ILogger<HostService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CreateHostAsync(User user)
        {
            try
            {
                var existingHost = await _context.Hosts.FindAsync(user.Id);
                if (existingHost == null)
                {
                    var host = new Host
                    {
                        Id = user.Id,
                        User = user
                    };
                    _context.Hosts.Add(host);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a host for user ID {UserId}", user.Id);
                throw;
            }
        }

        public async Task<Host?> GetHostByUserIdAsync(int userId)
        {
            try
            {
                var host = await _context.Hosts
                    .Include(h => h.Events)
                    .FirstOrDefaultAsync(h => h.Id == userId);

                if (host == null)
                {
                    _logger.LogWarning("No host found for user ID {UserId}", userId);
                    return null;
                }

                return host;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving host for user ID {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UpdateHostInfoAsync(int userId, UpdateHostModel model)
        {
            try
            {
                var host = await _context.Hosts.FirstOrDefaultAsync(h => h.Id == userId);
                if (host == null)
                {
                    _logger.LogWarning("Attempted to update non-existent host for user ID {UserId}", userId);
                    return false;
                }

                host.AgencyName = model.AgencyName;
                host.Bio = model.Bio;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating host info for user ID {UserId}", userId);
                throw;
            }
        }
    }
}