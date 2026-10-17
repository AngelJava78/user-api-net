namespace UsersApiDemo.DTOs;

public class PermissionCheckResponseDto
{
    public string Permission { get; set; } = string.Empty;
    public bool Allowed { get; set; }
}
