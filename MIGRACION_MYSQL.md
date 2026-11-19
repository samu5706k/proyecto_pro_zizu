# Sistema de Gestión de Gimnasio — Proyecto Sizu

Sistema web desarrollado en **ASP.NET Core 9.0 (C#)** y conectado a una base de datos **MariaDB/MySQL**.

Permite administrar clientes, entrenadores, administradores, productos, membresías y pagos desde una interfaz web.

## 1. Funcionalidades del sistema

- **Login:** inicio y cierre de sesión para administradores.
- **Clientes / Socios:** registrar, consultar, buscar, editar y eliminar clientes.
- **Entrenadores:** administración de entrenadores del gimnasio.
- **Administradores:** gestión de administradores y credenciales de acceso.
- **Productos:** registro, búsqueda y control de productos e inventario.
- **Membresías:** asignación de planes a clientes, control de fechas y vencimientos.
- **Pagos:** registro de pagos de membresías, métodos de pago y consulta de saldos pendientes.

## 2. Requisitos

Para ejecutar el proyecto necesitas:

- [.NET SDK 9](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio Code](https://code.visualstudio.com/)
- [XAMPP](https://www.apachefriends.org/) con MariaDB/MySQL
- Git para clonar el repositorio

## 3. Instalación del proyecto

### Paso 1. Clonar el repositorio

Abrir una terminal y ejecutar:

`git clone https://github.com/samu5706k/proyecto_pro_zizu.git`

Entrar a la carpeta:

`cd proyecto_pro_zizu`

Abrir el proyecto en Visual Studio Code:

`code .`

### Paso 2. Iniciar MariaDB/MySQL

1. Abrir XAMPP Control Panel.
2. Iniciar el servicio MySQL.
3. Verificar que esté funcionando, normalmente en el puerto `3306`.

No es necesario iniciar Apache para ejecutar la aplicación ASP.NET Core.

### Paso 3. Configurar la base de datos

La base de datos del proyecto se llama:

**`gimnasio_db`**

Para una instalación nueva:

1. Importar el archivo `base.sql` incluido en el proyecto.
2. Ejecutar los archivos SQL adicionales de la carpeta `Migraciones`, en este orden:
   - `001_credenciales_admin.sql`
   - `002_productos.sql`
   - `003_membresias_pagos.sql`
3. Comprobar que se hayan creado las tablas.

**Tablas del sistema:**

| Tabla | Descripción |
|---|---|
| `administradores` | Datos de los administradores |
| `credenciales_admin` | Contraseñas protegidas de administradores |
| `clientes` | Información de clientes y socios |
| `entrenadores` | Información de entrenadores |
| `productos` | Inventario de productos |
| `membresias` | Planes asignados a los socios |
| `pagos` | Pagos registrados de membresías |

**Importante:** si la base de datos ya contiene información, no se debe importar nuevamente `base.sql` sin revisar antes su contenido. Las migraciones deben aplicarse únicamente cuando corresponda.

### Paso 4. Configurar la conexión

Abrir una terminal dentro de la carpeta que contiene `gimnasio.csproj`.

Si el proyecto todavía no tiene configurado User Secrets, ejecutar:

`dotnet user-secrets init`

Después configurar la conexión:

`dotnet user-secrets set "ConnectionStrings:Gimnasio" "Server=127.0.0.1;Port=3306;Database=gimnasio_db;User ID=root;Password=;SslMode=None;"`

Esta configuración corresponde a XAMPP local con usuario `root`, sin contraseña y puerto `3306`.

Si la instalación utiliza otra contraseña o puerto, será necesario ajustar esos datos.

La contraseña de la base de datos no debe almacenarse directamente en los archivos compartidos en GitHub.

### Paso 5. Restaurar dependencias

Ejecutar:

`dotnet restore`

### Paso 6. Compilar el proyecto

Ejecutar:

`dotnet build`

### Paso 7. Ejecutar la aplicación

Ejecutar:

`dotnet run`

Abrir la dirección que aparezca en la terminal. En la configuración actual del proyecto:

**http://localhost:5211**

El puerto puede variar según la configuración de cada computadora.

## 4. Inicio de sesión

El sistema cuenta con autenticación de administradores.

Para ingresar se necesita:

- Correo electrónico de un administrador registrado y activo.
- Contraseña previamente configurada para ese administrador.

Las contraseñas se almacenan mediante hashes, no en texto plano.

**Primera instalación:** se debe configurar de forma segura la contraseña de un administrador inicial antes de poder iniciar sesión. Crear un registro en la tabla `administradores` no es suficiente: también necesita sus credenciales correspondientes.

Una vez iniciado el sistema, los administradores autorizados podrán registrar nuevos administradores con contraseña desde el formulario correspondiente.

## 5. Módulos del sistema

**Clientes / Socios:** administración de personas registradas en el gimnasio.

**Entrenadores:** registro y gestión de los entrenadores.

**Administradores:** administración de usuarios, accesos y credenciales.

**Productos:** gestión de inventario, precios y cantidades disponibles.

**Membresías:** asignación de planes a clientes, fechas de inicio, vencimiento, precios y estados.

**Pagos:** registro de pagos completos o parciales, historial de pagos y consulta de saldos.

## 6. Tecnologías utilizadas

- C#
- ASP.NET Core MVC
- .NET 9
- MariaDB/MySQL
- MySqlConnector
- HTML, CSS y Bootstrap
- Visual Studio Code
- Git y GitHub

## 7. Consideraciones importantes

Cada integrante del equipo debe configurar su propia conexión a MariaDB/MySQL.

La base de datos local de una computadora no se comparte automáticamente con las demás.

Para compartir registros entre varias computadoras, se necesita un servidor de base de datos compartido y configurado de forma segura.

Los archivos SQL de migración permiten reproducir la estructura de las tablas, pero no contienen los registros personales.

La versión `MySqlConnector 2.4.0` tiene una advertencia de seguridad `NU1902` conocida. El proyecto puede compilar, pero esta dependencia debe actualizarse antes de utilizar el sistema en producción.

## 8. Repositorio

Código fuente:

https://github.com/samu5706k/proyecto_pro_zizu

**Proyecto académico desarrollado para la gestión de un gimnasio.**
