using System.ComponentModel.DataAnnotations;

namespace UsersApiDemo.DTOs;

public class CreateUserDto
{
    [Required]
    [StringLength(200)]
    public string Auth0UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(320)]
    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
