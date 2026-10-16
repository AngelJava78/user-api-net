using System.ComponentModel.DataAnnotations;

namespace UsersApiDemo.DTOs;

public class CreateUserDto
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(320)]
    public string Email { get; set; } = string.Empty;

    // Only used to create the identity in Auth0. It is never stored in SQL Server.
    public string? Password { get; set; }

    public bool IsActive { get; set; } = true;
}
