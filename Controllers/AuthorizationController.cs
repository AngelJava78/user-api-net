using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersApiDemo.DTOs;
using UsersApiDemo.Repositories;

namespace UsersApiDemo.Controllers;

[ApiController]
[Authorize]
[Route("api/authorization")]
public class AuthorizationController : ControllerBase
{
    private readonly IAuthorizationRepository _authorizationRepository;

    public AuthorizationController(IAuthorizationRepository authorizationRepository)
    {
        _authorizationRepository = authorizationRepository;
    }

    [HttpGet("check")]
    public async Task<IActionResult> Check([FromQuery] string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
        {
            return BadRequest(new
            {
                message = "The permission parameter is required."
            });
        }

        var auth0UserId = User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(auth0UserId))
        {
            return Unauthorized(new
            {
                message = "The authenticated user does not contain the 'sub' claim."
            });
        }

        var allowed = await _authorizationRepository.HasPermissionAsync(
            auth0UserId,
            permission);

        return Ok(new PermissionCheckResponseDto
        {
            Permission = permission,
            Allowed = allowed
        });
    }
}
