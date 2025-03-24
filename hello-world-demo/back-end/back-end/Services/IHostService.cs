using System.Threading.Tasks;
using back_end.Controllers;
using back_end.Models;
using Host = back_end.Models.Host;

namespace back_end.Services
{
    public interface IHostService
    {
        Task CreateHostAsync(User user);
        Task<Host> GetHostByUserIdAsync(int userId);
        Task<bool> UpdateHostInfoAsync(int userId, UpdateHostModel model);
    }
}