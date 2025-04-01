using System.Collections.Generic;
using System.Threading.Tasks;
using back_end.Models;

namespace back_end.Services
{
    public interface IUserService
    {
        Task<IEnumerable<Event>> GetSavedEventsAsync(int userId);
        Task<bool> SaveEventAsync(int userId, int eventId);
        Task<bool> RemoveSavedEventAsync(int userId, int eventId);
    }
}