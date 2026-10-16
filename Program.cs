using UsersApiDemo.Options;
using UsersApiDemo.Repositories;
using UsersApiDemo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IUserRepository, UserRepository>();

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
app.MapControllers();

app.Run();
