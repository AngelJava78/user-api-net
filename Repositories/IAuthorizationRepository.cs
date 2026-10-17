namespace UsersApiDemo.Repositories;

public interface IAuthorizationRepository
{
    Task<bool> HasPermissionAsync(string auth0UserId, string permission);
}
