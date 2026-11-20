# Sistema de Gestión de Gimnasio — Pro Zizu

Aplicación web desarrollada con **C#**, **ASP.NET Core MVC (.NET 9)** y **MySQL/MariaDB** para administrar clientes, entrenadores, administradores, productos, membresías y pagos.

## Requisitos

- Visual Studio Code
- SDK de .NET 9
- XAMPP (solo para iniciar el servicio **MySQL**; no se necesita PHP ni Apache)

## Ejecutar en Visual Studio Code

1. Abrir la carpeta del proyecto, donde está `gimnasio.csproj`.
2. Iniciar **MySQL** desde XAMPP (normalmente puerto `3306`).
3. **Solo para una instalación nueva:** desde MySQL, ejecutar el archivo `base.sql` una vez, después de revisar/eliminar cualquier administrador de ejemplo y su hash de contraseña. No volver a importarlo sobre una base existente.
4. En la terminal de Visual Studio Code, configurar la conexión (ajustar usuario, contraseña y puerto según su instalación):

   ```powershell
   dotnet user-secrets set "ConnectionStrings:Gimnasio" "Server=127.0.0.1;Port=3306;Database=gimnasio_db;User ID=root;Password=;SslMode=None;"
   ```

   `gimnasio.csproj` ya incluye `UserSecretsId`; no es necesario ejecutar `dotnet user-secrets init`.

5. Restaurar, compilar y ejecutar:

   ```powershell
   dotnet restore
   dotnet build
   dotnet run --launch-profile http
   ```

6. Abrir la dirección mostrada después de `Now listening on:` (con el perfil `http`, normalmente `http://localhost:5211`). Mantener la terminal abierta.

## Inicio de sesión

Se requiere un administrador activo con una credencial guardada en `credenciales_admin`. El sistema almacena **hashes** de contraseñas, no contraseñas en texto plano. Las rutas privadas requieren autenticación y el rol autorizado.

**Seguridad:** no publicar credenciales reales, contraseñas de conexión ni hashes de usuarios en GitHub. El archivo `base.sql` del ZIP original contiene un registro de demostración con un hash; antes de compartir o reutilizar el script, retirar esos dos `INSERT` y crear el administrador inicial por un procedimiento seguro.

## Tablas y relaciones

`base.sql` crea las tablas: `administradores`, `credenciales_admin`, `clientes`, `entrenadores`, `productos`, `membresias` y `pagos`.

- `administradores` **1 : 0..1** `credenciales_admin` (FK `AdministradorId`, única).
- `clientes` **1 : 0..N** `membresias` (FK `ClienteId`).
- `membresias` **1 : 0..N** `pagos` (FK `MembresiaId`).
- `entrenadores` y `productos` no tienen relaciones FK en el diseño actual.

El diagrama actualizado está en `pro_zizu_diagrama_ER_corregido.png` y su versión editable en `pro_zizu_diagrama_ER_corregido.mmd`.

## Observaciones y comprobaciones

- Verificar acceso a páginas privadas sin sesión desde una ventana incógnita.
- Verificar que el formulario de pagos rechaza referencias vacías y montos inválidos.
- Revisar que MySQL y el código ASP.NET usen el mismo esquema y que los datos persistan al reiniciar.
- No incluir en las instrucciones la carpeta `Migraciones`: **no está incluida en el ZIP revisado** y `base.sql` ya contiene las siete tablas.
- Resolver la advertencia de seguridad de `MySqlConnector 2.4.0`, verificando y actualizando a una versión segura y compatible antes de desplegar a producción.

## Tecnologías

ASP.NET Core MVC · C# · .NET 9 · MySQL/MariaDB · MySqlConnector · Bootstrap · Visual Studio Code
