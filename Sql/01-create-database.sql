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
        Auth0UserId NVARCHAR(200) NULL,
        Name NVARCHAR(150) NOT NULL,
        Email NVARCHAR(320) NOT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT UQ_Users_Email UNIQUE (Email)
    );

    CREATE UNIQUE INDEX UX_Users_Auth0UserId
        ON dbo.Users (Auth0UserId)
        WHERE Auth0UserId IS NOT NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Users)
BEGIN
    INSERT INTO dbo.Users (Name, Email, IsActive)
    VALUES
    ('Ana Lopez', 'ana.lopez@example.com', 1),
    ('Carlos Ramirez', 'carlos.ramirez@example.com', 1),
    ('Maria Torres', 'maria.torres@example.com', 1);
END
GO
