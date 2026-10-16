USE UsersDemo;
GO

-- Ejecuta este script solamente si la base UsersDemo ya fue creada
-- con la primera version de la API.

IF EXISTS
(
    SELECT 1
    FROM sys.key_constraints
    WHERE [name] = 'UQ_Users_Auth0UserId'
      AND parent_object_id = OBJECT_ID('dbo.Users')
)
BEGIN
    ALTER TABLE dbo.Users
        DROP CONSTRAINT UQ_Users_Auth0UserId;
END
GO

ALTER TABLE dbo.Users
    ALTER COLUMN Auth0UserId NVARCHAR(200) NULL;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE [name] = 'UX_Users_Auth0UserId'
      AND object_id = OBJECT_ID('dbo.Users')
)
BEGIN
    CREATE UNIQUE INDEX UX_Users_Auth0UserId
        ON dbo.Users (Auth0UserId)
        WHERE Auth0UserId IS NOT NULL;
END
GO
