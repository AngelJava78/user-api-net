using UsersApiDemo.DTOs;
using UsersApiDemo.Models;

namespace UsersApiDemo.Services;

public interface IUserService
{
    Task<User> CreateAsync(CreateUserDto dto);
}
