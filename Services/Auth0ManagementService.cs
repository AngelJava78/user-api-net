using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using UsersApiDemo.DTOs;
using UsersApiDemo.Options;

namespace UsersApiDemo.Services;

public class Auth0ManagementService : IAuth0ManagementService
{
    private readonly HttpClient _httpClient;
    private readonly Auth0ManagementOptions _options;

    public Auth0ManagementService(
        HttpClient httpClient,
        IOptions<Auth0ManagementOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public bool IsEnabled => _options.Enabled;

    public async Task<string?> CreateUserAsync(CreateUserDto dto)
    {
        if (!IsEnabled)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            throw new ArgumentException(
                "Password is required when Auth0 Management integration is enabled.",
                nameof(dto));
        }

        ValidateConfiguration();

        var accessToken = await GetManagementAccessTokenAsync();

        using var request = new HttpRequestMessage(HttpMethod.Post, "api/v2/users");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = JsonContent.Create(new
        {
            connection = _options.Connection,
            email = dto.Email,
            password = dto.Password,
            name = dto.Name
        });

        using var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Auth0 could not create the user. Status: {(int)response.StatusCode}. Response: {responseBody}");
        }

        var auth0User = await response.Content.ReadFromJsonAsync<Auth0CreateUserResponse>();

        if (auth0User is null || string.IsNullOrWhiteSpace(auth0User.UserId))
        {
            throw new InvalidOperationException("Auth0 did not return user_id when creating the user.");
        }

        return auth0User.UserId;
    }

    private async Task<string> GetManagementAccessTokenAsync()
    {
        using var response = await _httpClient.PostAsJsonAsync("oauth/token", new
        {
            client_id = _options.ClientId,
            client_secret = _options.ClientSecret,
            audience = $"https://{NormalizeDomain(_options.Domain)}/api/v2/",
            grant_type = "client_credentials"
        });

        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Auth0 could not issue a Management API token. Status: {(int)response.StatusCode}. Response: {responseBody}");
        }

        var token = await response.Content.ReadFromJsonAsync<Auth0TokenResponse>();

        if (token is null || string.IsNullOrWhiteSpace(token.AccessToken))
        {
            throw new InvalidOperationException("Auth0 did not return an access_token.");
        }

        return token.AccessToken;
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_options.Domain) ||
            string.IsNullOrWhiteSpace(_options.ClientId) ||
            string.IsNullOrWhiteSpace(_options.ClientSecret) ||
            string.IsNullOrWhiteSpace(_options.Connection))
        {
            throw new InvalidOperationException(
                "Auth0Management configuration is incomplete.");
        }
    }

    private static string NormalizeDomain(string domain) =>
        domain.Replace("https://", string.Empty, StringComparison.OrdinalIgnoreCase)
              .TrimEnd('/');
}
