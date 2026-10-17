using Dapper;
using Microsoft.Data.SqlClient;

namespace UsersApiDemo.Repositories;

public class AuthorizationRepository : IAuthorizationRepository
{
    private readonly string _connectionString;

    public AuthorizationRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("Connection string 'SqlServer' was not found.");
    }

    private SqlConnection CreateConnection() => new(_connectionString);

    public async Task<bool> HasPermissionAsync(string auth0UserId, string permission)
    {
        const string sql = """
            SELECT CAST(
                CASE WHEN EXISTS
                (
                    SELECT 1
                    FROM dbo.Users u
                    INNER JOIN dbo.UserRoles ur
                        ON ur.UserId = u.Id
                    INNER JOIN dbo.RolePermissions rp
                        ON rp.RoleId = ur.RoleId
                    INNER JOIN dbo.Permissions p
                        ON p.Id = rp.PermissionId
                    WHERE u.Auth0UserId = @Auth0UserId
                      AND u.IsActive = 1
                      AND p.Name = @Permission
                )
                THEN 1
                ELSE 0
                END
            AS bit);
            """;

        await using var connection = CreateConnection();

        return await connection.ExecuteScalarAsync<bool>(sql, new
        {
            Auth0UserId = auth0UserId,
            Permission = permission
        });
    }
}
