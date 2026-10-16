using UsersApiDemo.DTOs;

namespace UsersApiDemo.Services;

public interface IAuth0ManagementService
{
    bool IsEnabled { get; }
    Task<Auth0CreateUserResponse?> GetUserByEmailAsync(string email);
    Task<string?> CreateUserAsync(CreateUserDto dto);
}
