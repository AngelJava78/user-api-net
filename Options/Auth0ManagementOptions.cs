namespace UsersApiDemo.Options;

public class Auth0ManagementOptions
{
    public const string SectionName = "Auth0Management";

    public bool Enabled { get; set; }
    public string Domain { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Connection { get; set; } = "Username-Password-Authentication";
}
