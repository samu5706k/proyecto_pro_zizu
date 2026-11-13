CREATE DATABASE IF NOT EXISTS gimnasio_db
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE gimnasio_db;

CREATE TABLE clientes (
    Id              INT          NOT NULL AUTO_INCREMENT,
    Nombre          VARCHAR(50)  NOT NULL,
    Apellido        VARCHAR(50)  NOT NULL,
    Ci              VARCHAR(15)  NOT NULL,
    Telefono        VARCHAR(15)  NOT NULL,
    Email           VARCHAR(150) NOT NULL,
    Direccion       VARCHAR(200) NOT NULL,
    FechaNacimiento DATE         NULL,
    FechaRegistro   DATE         NOT NULL DEFAULT (CURRENT_DATE),
    Activo          TINYINT(1)   NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    UNIQUE KEY uq_clientes_ci (Ci),
    UNIQUE KEY uq_clientes_email (Email),
    KEY idx_clientes_nombre (Apellido, Nombre),
    CONSTRAINT chk_clientes_telefono CHECK (CHAR_LENGTH(Telefono) BETWEEN 7 AND 15)
) ENGINE=InnoDB;

CREATE TABLE entrenadores (
    Id                INT           NOT NULL AUTO_INCREMENT,
    Nombre            VARCHAR(50)   NOT NULL,
    Apellido          VARCHAR(50)   NOT NULL,
    Ci                VARCHAR(15)   NOT NULL,
    Telefono          VARCHAR(15)   NOT NULL,
    Email             VARCHAR(150)  NOT NULL,
    Especialidad      VARCHAR(50)   NOT NULL,
    FechaContratacion DATE          NOT NULL DEFAULT (CURRENT_DATE),
    Salario           DECIMAL(10,2) NOT NULL DEFAULT 0,
    Activo            TINYINT(1)    NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    UNIQUE KEY uq_entrenadores_ci (Ci),
    UNIQUE KEY uq_entrenadores_email (Email),
    KEY idx_entrenadores_especialidad (Especialidad),
    KEY idx_entrenadores_nombre (Apellido, Nombre),
    CONSTRAINT chk_entrenadores_salario CHECK (Salario BETWEEN 0 AND 1000000),
    CONSTRAINT chk_entrenadores_telefono CHECK (CHAR_LENGTH(Telefono) BETWEEN 7 AND 15)
) ENGINE=InnoDB;

CREATE TABLE administradores (
    Id          INT          NOT NULL AUTO_INCREMENT,
    Nombre      VARCHAR(50)  NOT NULL,
    Apellido    VARCHAR(50)  NOT NULL,
    Ci          VARCHAR(15)  NOT NULL,
    Telefono    VARCHAR(15)  NOT NULL,
    Email       VARCHAR(150) NOT NULL,
    Cargo       VARCHAR(20)  NOT NULL DEFAULT 'Administrador',
    NivelAcceso VARCHAR(20)  NOT NULL DEFAULT 'Limitado',
    Activo      TINYINT(1)   NOT NULL DEFAULT 1,
    PRIMARY KEY (Id),
    UNIQUE KEY uq_administradores_ci (Ci),
    UNIQUE KEY uq_administradores_email (Email),
    KEY idx_administradores_nombre (Apellido, Nombre),
    CONSTRAINT chk_admin_cargo CHECK (Cargo IN ('Administrador', 'Supervisor', 'Gerente')),
    CONSTRAINT chk_admin_nivel CHECK (NivelAcceso IN ('Limitado', 'Medio', 'Completo')),
    CONSTRAINT chk_admin_telefono CHECK (CHAR_LENGTH(Telefono) BETWEEN 7 AND 15)
) ENGINE=InnoDB;


-- Las tablas se crean vacías. Registra los datos desde la aplicación.
