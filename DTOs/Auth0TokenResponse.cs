using System.Text.Json.Serialization;

namespace UsersApiDemo.DTOs;

public class Auth0TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
}
