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

        public HostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateHostAsync(User user)
        {
            // Create a Host record only if one doesn't already exist.
            var existingHost = await _context.Hosts.FindAsync(user.Id);
            if (existingHost == null)
            {
                var host = new Host
                {
                    Id = user.Id,  // Assuming the Host primary key is the same as the User Id
                    User = user
                };
                _context.Hosts.Add(host);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Host> GetHostByUserIdAsync(int userId)
        {
            return await _context.Hosts
                .Include(h => h.Events)
                .FirstOrDefaultAsync(h => h.Id == userId);
        }

        public async Task<bool> UpdateHostInfoAsync(int userId, UpdateHostModel model)
        {
            var host = await _context.Hosts.FirstOrDefaultAsync(h => h.Id == userId);
            if (host == null)
                return false;

            host.AgencyName = model.AgencyName;
            host.Bio = model.Bio;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}