using Microsoft.AspNetCore.Authentication.JwtBearer;
using UsersApiDemo.Options;
using UsersApiDemo.Repositories;
using UsersApiDemo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthorizationRepository, AuthorizationRepository>();
builder.Services.AddScoped<IUserService, UserService>();

var auth0Domain = builder.Configuration["Auth0:Domain"]
    ?? throw new InvalidOperationException("Auth0:Domain no está configurado.");

var auth0Audience = builder.Configuration["Auth0:Audience"]
    ?? throw new InvalidOperationException("Auth0:Audience no está configurado.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{auth0Domain.TrimEnd('/')}";
        options.Audience = auth0Audience;
    });

builder.Services.AddAuthorization();

builder.Services.Configure<Auth0ManagementOptions>(
    builder.Configuration.GetSection(Auth0ManagementOptions.SectionName));

builder.Services.AddHttpClient<IAuth0ManagementService, Auth0ManagementService>((serviceProvider, client) =>
{
    var options = serviceProvider
        .GetRequiredService<Microsoft.Extensions.Options.IOptions<Auth0ManagementOptions>>()
        .Value;

    if (!string.IsNullOrWhiteSpace(options.Domain))
    {
        var domain = options.Domain
            .Replace("https://", string.Empty, StringComparison.OrdinalIgnoreCase)
            .TrimEnd('/');

        client.BaseAddress = new Uri($"https://{domain}/");
    }
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
