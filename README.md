# UsersApiDemo v5

API REST didactica en .NET 10, C#, Dapper y SQL Server con integracion a Auth0 Management API.

## Flujo de alta de usuario

`POST /api/users` aplica obligatoriamente esta regla de negocio:

1. Buscar el usuario por email en SQL Server. Si existe, responder `409 Conflict`.
2. Buscar el usuario por email en Auth0. Si existe, responder `409 Conflict`.
3. Crear el usuario en SQL Server con `Auth0UserId = NULL`.
4. Crear el usuario en Auth0.
5. Guardar en SQL Server el `user_id` generado por Auth0.

No existe un flujo alternativo de alta solo en SQL Server. Por eso `Auth0Management:Enabled` debe estar en `true`.

## Configuracion Auth0

Completa en `appsettings.json` o mediante User Secrets:

```json
"Auth0Management": {
  "Enabled": true,
  "Domain": "TU_DOMINIO.auth0.com",
  "ClientId": "CLIENT_ID_M2M",
  "ClientSecret": "CLIENT_SECRET_M2M",
  "Connection": "Username-Password-Authentication"
}
```

La aplicacion M2M debe tener al menos `read:users` y `create:users` sobre Auth0 Management API.

## Endpoints conservados

- `GET /api/users`
- `GET /api/users/{id}`
- `GET /api/users/by-auth0/{auth0UserId}`
- `POST /api/users`
- `PUT /api/users/{id}`
- `DELETE /api/users/{id}`

La contrasena recibida en el alta se utiliza unicamente para crear la identidad en Auth0. No se almacena en SQL Server.


## Autenticación de la API con Auth0

Todos los endpoints de `UsersController` están protegidos con `[Authorize]`. La API valida Access Tokens JWT emitidos por Auth0 para el audience `https://demo-api`.

Configuración utilizada:

```json
"Auth0": {
  "Domain": "dev-angel-java.us.auth0.com",
  "Audience": "https://demo-api"
}
```

Comportamiento esperado:

- Sin `Authorization: Bearer <token>`: `401 Unauthorized`.
- Token inválido: `401 Unauthorized`.
- Token válido emitido por Auth0 para `https://demo-api`: acceso al CRUD.

La sección `Auth0Management` sigue siendo independiente: se utiliza únicamente para administrar usuarios en Auth0 mediante la Management API.
