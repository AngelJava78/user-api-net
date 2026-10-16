using UsersApiDemo.DTOs;
using UsersApiDemo.Models;
using UsersApiDemo.Repositories;

namespace UsersApiDemo.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IAuth0ManagementService _auth0ManagementService;

    public UserService(
        IUserRepository repository,
        IAuth0ManagementService auth0ManagementService)
    {
        _repository = repository;
        _auth0ManagementService = auth0ManagementService;
    }

    public async Task<User> CreateAsync(CreateUserDto dto)
    {
        if (!_auth0ManagementService.IsEnabled)
        {
            throw new InvalidOperationException(
                "Auth0 Management integration must be enabled to create users.");
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            throw new ArgumentException(
                "Password is required to create the user in Auth0.",
                nameof(dto.Password));
        }

        // 1. Obtener el usuario de SQL Server.
        //    Si existe, terminar con error. Si no existe, continuar.
        var databaseUser = await _repository.GetByEmailAsync(dto.Email);
        if (databaseUser is not null)
        {
            throw new UserAlreadyExistsException(
                $"The user with email '{dto.Email}' already exists in SQL Server.");
        }

        // 2. Obtener el usuario de Auth0.
        //    Si existe, terminar con error. Si no existe, continuar.
        var auth0ExistingUser = await _auth0ManagementService.GetUserByEmailAsync(dto.Email);
        if (auth0ExistingUser is not null)
        {
            throw new UserAlreadyExistsException(
                $"The user with email '{dto.Email}' already exists in Auth0.");
        }

        // 3. Registrar el usuario en SQL Server.
        //    Auth0UserId queda NULL en este momento.
        var id = await _repository.CreateAsync(dto);

        // 4. Registrar el usuario en Auth0.
        //    Auth0 devuelve el user_id generado.
        var auth0UserId = await _auth0ManagementService.CreateUserAsync(dto);
        if (string.IsNullOrWhiteSpace(auth0UserId))
        {
            throw new InvalidOperationException(
                "Auth0 did not return user_id when creating the user.");
        }

        // 5. Asignar el Auth0UserId al usuario previamente creado en SQL Server.
        var updated = await _repository.UpdateAuth0UserIdAsync(id, auth0UserId);
        if (!updated)
        {
            throw new InvalidOperationException(
                "The user was created in Auth0, but Auth0UserId could not be saved in SQL Server.");
        }

        var user = await _repository.GetByIdAsync(id);
        if (user is null)
        {
            throw new InvalidOperationException(
                "The user was created, but could not be retrieved from SQL Server.");
        }

        return user;
    }
}
