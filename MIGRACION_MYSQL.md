# Sistema de gestión de gimnasio

Aplicación web ASP.NET Core MVC desarrollada en C# para administrar clientes,
entrenadores y administradores, y consultar estadísticas. La persistencia se
realiza en MySQL; no se utilizan archivos JSON para guardar registros.

## Requisitos para ejecutar el proyecto

- .NET SDK 9.0
- MySQL Server 8.0.16 o superior
- MySQL Workbench (opcional, sirve para ejecutar y administrar el script SQL)

> GitHub almacena el código, no ejecuta ni aloja automáticamente la base de datos.
> Cada computadora que ejecute la aplicación debe tener acceso a un servidor MySQL.

## Preparar la base de datos

1. Instala e inicia MySQL Server.
2. Abre MySQL Workbench y conéctate a tu servidor local.
3. Abre y ejecuta el archivo `base.sql`. Este crea la base `gimnasio_db`
   y las tablas `clientes`, `entrenadores` y `administradores`.
4. El script crea las tablas vacías; los registros se agregan desde la aplicación.

## Configurar la conexión sin publicar contraseñas

Desde una terminal, dentro de la carpeta donde está `gimnasio.csproj`, ejecuta:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Gimnasio" "Server=localhost;Port=3306;Database=gimnasio_db;User ID=root;Password=TU_PASSWORD;SslMode=None;"
```

Reemplaza `TU_PASSWORD` por la contraseña del usuario MySQL de esa computadora.
Si el usuario no tiene contraseña, deja `Password=` vacío.

User Secrets guarda la conexión fuera del repositorio Git, así no se publica
la contraseña en GitHub. Cada persona configura sus propios datos de conexión.

También se puede usar la variable de entorno `ConnectionStrings__Gimnasio`
en un servidor o entorno de despliegue.

## Restaurar, compilar y ejecutar

```bash
dotnet restore
dotnet build
dotnet run
```

Abre en el navegador la dirección local que muestre la terminal.

## Qué incluye

- CRUD de clientes.
- CRUD de entrenadores.
- CRUD de administradores.
- Estadísticas generales basadas en los registros de MySQL.
- Reportes de entrenadores.
- Consultas SQL parametrizadas mediante el paquete `MySqlConnector`.

## Alcance actual

Esta versión no incluye inicio de sesión ni control de acceso por roles.
Las pantallas de administración están disponibles según las rutas de la aplicación.
El proyecto está preparado para conectarse a MySQL local; para que varias
computadoras compartan los mismos datos, deben conectarse a un servidor MySQL
común accesible por red, no cada una a su propia base local.

## Importante

- No subas contraseñas ni archivos de configuración locales a GitHub.
- El archivo `base.sql` contiene la estructura de la base de datos, no los datos
  que cada usuario registre después.
- Las tablas se crean al ejecutar `base.sql`; la aplicación no las crea
  automáticamente.
