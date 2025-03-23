using back_end.Models;
using back_end.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace back_end.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Retrieves all users for admin views or user listings
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        // Fetch a specific user by ID for profile or management purposes
        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        // Lookup by email for login, validation, or password reset
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        // Add a new user during registration or admin creation
        public async Task AddUserAsync(User user)
        {
            await _userRepository.AddUserAsync(user);
        }

        // Save changes to user details like name, email, or account type
        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateUserAsync(user);
        }

        // Remove a user by ID, typically for admin deletion or user-initiated removal
        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);
        }
    }
}
