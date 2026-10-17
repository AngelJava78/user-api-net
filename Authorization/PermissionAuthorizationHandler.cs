using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using UsersApiDemo.Repositories;

namespace UsersApiDemo.Authorization;

public sealed class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly IAuthorizationRepository _authorizationRepository;

    public PermissionAuthorizationHandler(
        IAuthorizationRepository authorizationRepository)
    {
        _authorizationRepository = authorizationRepository;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var auth0UserId = context.User.FindFirstValue("sub")
            ?? context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(auth0UserId))
        {
            return;
        }

        var hasPermission = await _authorizationRepository.HasPermissionAsync(
            auth0UserId,
            requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}
