IF DB_ID('UsersDemo') IS NULL
BEGIN
    CREATE DATABASE UsersDemo;
END
GO

USE UsersDemo;
GO

IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
        Auth0UserId NVARCHAR(200) NOT NULL,
        Name NVARCHAR(150) NOT NULL,
        Email NVARCHAR(320) NOT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT UQ_Users_Auth0UserId UNIQUE (Auth0UserId),
        CONSTRAINT UQ_Users_Email UNIQUE (Email)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Users)
BEGIN
    INSERT INTO dbo.Users (Auth0UserId, Name, Email, IsActive)
    VALUES
    ('auth0|demo-user-001', 'Ana Lopez', 'ana.lopez@example.com', 1),
    ('auth0|demo-user-002', 'Carlos Ramirez', 'carlos.ramirez@example.com', 1),
    ('auth0|demo-user-003', 'Maria Torres', 'maria.torres@example.com', 1);
END
GO
