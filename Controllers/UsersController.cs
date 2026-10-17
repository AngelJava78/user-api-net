using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersApiDemo.DTOs;
using UsersApiDemo.Repositories;
using UsersApiDemo.Services;

namespace UsersApiDemo.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repository;
    private readonly IUserService _userService;

    public UsersController(
        IUserRepository repository,
        IUserService userService)
    {
        _repository = repository;
        _userService = userService;
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
        try
        {
            var user = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (UserAlreadyExistsException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(nameof(dto.Password), ex.Message);
            return ValidationProblem(ModelState);
        }
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
