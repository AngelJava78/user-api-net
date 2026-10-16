using Dapper;
using Microsoft.Data.SqlClient;
using UsersApiDemo.DTOs;
using UsersApiDemo.Models;

namespace UsersApiDemo.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("Connection string 'SqlServer' was not found.");
    }

    private SqlConnection CreateConnection() => new(_connectionString);

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        const string sql = """
            SELECT Id, Auth0UserId, Name, Email, IsActive, CreatedAt
            FROM dbo.Users
            ORDER BY Id;
            """;

        await using var connection = CreateConnection();
        return await connection.QueryAsync<User>(sql);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT Id, Auth0UserId, Name, Email, IsActive, CreatedAt
            FROM dbo.Users
            WHERE Id = @Id;
            """;

        await using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
    }

    public async Task<User?> GetByAuth0UserIdAsync(string auth0UserId)
    {
        const string sql = """
            SELECT Id, Auth0UserId, Name, Email, IsActive, CreatedAt
            FROM dbo.Users
            WHERE Auth0UserId = @Auth0UserId;
            """;

        await using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Auth0UserId = auth0UserId });
    }

    public async Task<int> CreateAsync(CreateUserDto dto)
    {
        const string sql = """
            INSERT INTO dbo.Users (Auth0UserId, Name, Email, IsActive)
            OUTPUT INSERTED.Id
            VALUES (@Auth0UserId, @Name, @Email, @IsActive);
            """;

        await using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, dto);
    }

    public async Task<bool> UpdateAsync(int id, UpdateUserDto dto)
    {
        const string sql = """
            UPDATE dbo.Users
            SET Auth0UserId = @Auth0UserId,
                Name = @Name,
                Email = @Email,
                IsActive = @IsActive
            WHERE Id = @Id;
            """;

        await using var connection = CreateConnection();
        var affectedRows = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            dto.Auth0UserId,
            dto.Name,
            dto.Email,
            dto.IsActive
        });

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM dbo.Users WHERE Id = @Id;";

        await using var connection = CreateConnection();
        var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
        return affectedRows > 0;
    }
}
