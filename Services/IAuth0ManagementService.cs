using UsersApiDemo.DTOs;

namespace UsersApiDemo.Services;

public interface IAuth0ManagementService
{
    bool IsEnabled { get; }
    Task<string?> CreateUserAsync(CreateUserDto dto);
}
