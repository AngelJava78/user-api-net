# UsersApiDemo

API REST didactica en .NET 10, C#, Dapper y SQL Server.

## Objetivo

Administrar usuarios propios de la aplicacion sin implementar autenticacion ni almacenar contrasenas.
Auth0 se encargara posteriormente de la autenticacion.

## Separacion de responsabilidades

SQL Server almacena los datos propios de la aplicacion:

- Id
- Auth0UserId (opcional hasta que exista una vinculacion con Auth0)
- Name
- Email
- IsActive
- CreatedAt

Auth0 sera responsable de identidad y autenticacion. La contrasena nunca se almacena en esta API.

## Cambio de esta version

`Auth0UserId` ya no forma parte de `CreateUserDto` ni de `UpdateUserDto`.
Un usuario puede existir en SQL Server sin estar vinculado todavia a Auth0.

Cuando posteriormente vinculemos ambas identidades, el valor de `Auth0UserId` correspondera normalmente al claim `sub` entregado por Auth0.

## Base de datos existente

Si ya ejecutaste la primera version del proyecto, ejecuta:

`Sql/02-make-auth0-user-id-optional.sql`

Esto hace nullable `Auth0UserId` y sustituye la restriccion UNIQUE anterior por un indice UNIQUE filtrado, permitiendo multiples registros con `NULL` pero evitando repetir un Auth0 User ID real.

Para una instalacion nueva usa:

`Sql/01-create-database.sql`

## Endpoints existentes

- GET /api/users
- GET /api/users/{id}
- GET /api/users/by-auth0/{auth0UserId}
- POST /api/users
- PUT /api/users/{id}
- DELETE /api/users/{id}

Las rutas existentes se conservan.

## Auth0 Management API integration (prepared, disabled by default)

This version adds the code required for `POST /api/users` to create the identity in Auth0 first and then store the returned `user_id` in `dbo.Users.Auth0UserId`.

The integration is disabled by default:

```json
"Auth0Management": {
  "Enabled": false,
  "Domain": "",
  "ClientId": "",
  "ClientSecret": "",
  "Connection": "Username-Password-Authentication"
}
```

With `Enabled=false`, the API keeps the previous behavior and creates the SQL Server user without calling Auth0.

When Auth0 is configured later and `Enabled=true`, the request will include `password`. The password is sent only to Auth0 and is never stored in SQL Server.
