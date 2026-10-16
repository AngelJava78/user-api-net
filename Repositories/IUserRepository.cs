using UsersApiDemo.DTOs;
using UsersApiDemo.Models;

namespace UsersApiDemo.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByAuth0UserIdAsync(string auth0UserId);
    Task<int> CreateAsync(CreateUserDto dto);
    Task<bool> UpdateAuth0UserIdAsync(int id, string auth0UserId);
    Task<bool> UpdateAsync(int id, UpdateUserDto dto);
    Task<bool> DeleteAsync(int id);
}
