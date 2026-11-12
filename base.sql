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
    FechaNacimiento DATE         NOT NULL,
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

INSERT INTO clientes
    (Id, Nombre, Apellido, Ci, Telefono, Email, Direccion, FechaNacimiento, FechaRegistro, Activo)
VALUES
    (1, 'zizu', 'pana', '2348719', '76349087', 'pepe@gmail.com', 'sopocachi', '2006-04-22', '2026-10-26', 1),
    (2, 'samu', 'Gutierrez', '1223376', '76398782', 'samu@gmail.com', 'puebla', '2004-09-11', '2026-09-12', 1);

INSERT INTO entrenadores
    (Id, Nombre, Apellido, Ci, Telefono, Email, Especialidad, FechaContratacion, Salario, Activo)
VALUES
    (1, 'samu', 'guti', '1038375865', '79505385', 'saihdug@gmail.com', 'yoga', '2026-10-29', 2000.00, 1);

INSERT INTO administradores
    (Id, Nombre, Apellido, Ci, Telefono, Email, Cargo, NivelAcceso, Activo)
VALUES
    (1, 'laura', 'chavez', '1234567', '76543210', 'laura123@gmail.com', 'Administrador', 'Completo', 1);

-- Para que el próximo AUTO_INCREMENT continúe después de los datos cargados
ALTER TABLE clientes        AUTO_INCREMENT = 3;
ALTER TABLE entrenadores    AUTO_INCREMENT = 2;
ALTER TABLE administradores AUTO_INCREMENT = 2;
