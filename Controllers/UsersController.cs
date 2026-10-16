using Microsoft.AspNetCore.Mvc;
using UsersApiDemo.DTOs;
using UsersApiDemo.Repositories;
using UsersApiDemo.Services;

namespace UsersApiDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repository;
    private readonly IAuth0ManagementService _auth0ManagementService;

    public UsersController(
        IUserRepository repository,
        IAuth0ManagementService auth0ManagementService)
    {
        _repository = repository;
        _auth0ManagementService = auth0ManagementService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _repository.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("by-auth0/{auth0UserId}")]
    public async Task<IActionResult> GetByAuth0UserId(string auth0UserId)
    {
        var user = await _repository.GetByAuth0UserIdAsync(auth0UserId);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        if (_auth0ManagementService.IsEnabled && string.IsNullOrWhiteSpace(dto.Password))
        {
            ModelState.AddModelError(nameof(dto.Password),
                "Password is required when Auth0 Management integration is enabled.");
            return ValidationProblem(ModelState);
        }

        var auth0UserId = await _auth0ManagementService.CreateUserAsync(dto);
        var id = await _repository.CreateAsync(dto, auth0UserId);
        var user = await _repository.GetByIdAsync(id);

        return CreatedAtAction(nameof(GetById), new { id }, user);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateUserDto dto)
    {
        var updated = await _repository.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
