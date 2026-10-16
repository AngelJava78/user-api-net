# UsersApiDemo

API REST didactica para CRUD de usuarios propios de la aplicacion usando .NET 10, C#, Dapper y SQL Server.

## Importante

Esta API NO implementa autenticacion y NO almacena contrasenas.

`Auth0UserId` sirve para relacionar un registro de negocio de SQL Server con la identidad autenticada por Auth0. La autenticacion seguira perteneciendo a Auth0.

## Flujo

Cliente HTTP -> UsersController -> IUserRepository -> UserRepository -> Dapper -> SQL Server

## Endpoints

- GET /api/users
- GET /api/users/{id}
- GET /api/users/by-auth0/{auth0UserId}
- POST /api/users
- PUT /api/users/{id}
- DELETE /api/users/{id}

## Base de datos

Ejecuta `Sql/01-create-database.sql` en SQL Server.

Por defecto la cadena es:

`Server=localhost;Database=UsersDemo;Trusted_Connection=True;TrustServerCertificate=True;`

Ajustala en `appsettings.json` si tu instancia es diferente.

## Ejecutar

```bash
dotnet restore
dotnet run
```

El perfil HTTP usa `http://localhost:5195`.

## Paquetes

- Dapper 2.1.89
- Microsoft.Data.SqlClient 7.1.1
