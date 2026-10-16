using System.Text.Json.Serialization;

namespace UsersApiDemo.DTOs;

public class Auth0CreateUserResponse
{
    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = string.Empty;
}
