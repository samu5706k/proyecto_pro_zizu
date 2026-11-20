CREATE DATABASE IF NOT EXISTS gimnasio_db
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE gimnasio_db;

CREATE TABLE gimnasio_db.clientes (
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

CREATE TABLE gimnasio_db.entrenadores (
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

CREATE TABLE gimnasio_db.administradores (
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

CREATE TABLE IF NOT EXISTS gimnasio_db.credenciales_admin (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    AdministradorId INT NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    FechaCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_credenciales_admin
        FOREIGN KEY (AdministradorId)
        REFERENCES administradores(Id)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS gimnasio_db.productos (
    Id INT NOT NULL AUTO_INCREMENT,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(500) NOT NULL DEFAULT '',
    Categoria VARCHAR(60) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL DEFAULT 0,
    Stock INT NOT NULL DEFAULT 0,
    Activo TINYINT(1) NOT NULL DEFAULT 1,

    PRIMARY KEY (Id),

    INDEX idx_productos_nombre (Nombre),
    INDEX idx_productos_categoria (Categoria),

    CONSTRAINT chk_productos_precio
        CHECK (Precio >= 0),

    CONSTRAINT chk_productos_stock
        CHECK (Stock >= 0)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS gimnasio_db.membresias (
    Id INT NOT NULL AUTO_INCREMENT,
    ClienteId INT NOT NULL,
    Plan VARCHAR(40) NOT NULL,
    FechaInicio DATE NOT NULL,
    FechaVencimiento DATE NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Activa TINYINT(1) NOT NULL DEFAULT 1,

    PRIMARY KEY (Id),

    INDEX idx_membresias_cliente (ClienteId),

    CONSTRAINT fk_membresias_clientes
        FOREIGN KEY (ClienteId)
        REFERENCES clientes(Id)
        ON DELETE RESTRICT,

    CONSTRAINT chk_membresias_precio
        CHECK (Precio > 0),

    CONSTRAINT chk_membresias_fechas
        CHECK (FechaVencimiento >= FechaInicio)
) ENGINE=InnoDB;


-- PAGOS DE MEMBRESÍAS
CREATE TABLE IF NOT EXISTS gimnasio_db.pagos (
    Id INT NOT NULL AUTO_INCREMENT,
    MembresiaId INT NOT NULL,
    FechaPago DATE NOT NULL,
    Monto DECIMAL(10,2) NOT NULL,
    Metodo VARCHAR(30) NOT NULL,
    Referencia VARCHAR(80) NOT NULL,

    PRIMARY KEY (Id),

    INDEX idx_pagos_membresia (MembresiaId),

    CONSTRAINT fk_pagos_membresias
        FOREIGN KEY (MembresiaId)
        REFERENCES membresias(Id)
        ON DELETE RESTRICT,

    CONSTRAINT chk_pagos_monto
        CHECK (Monto > 0),

    CONSTRAINT chk_pagos_referencia
        CHECK (CHAR_LENGTH(TRIM(Referencia)) > 0)

) ENGINE=InnoDB;


INSERT INTO gimnasio_db.administradores (Nombre, Apellido, Ci, Telefono, Email, Cargo, NivelAcceso, Activo)
VALUES ('Limber', 'Jhon', '62591006', '77777777', 'limber@gmail.com', 'Administrador', 'Completo', 1);

INSERT INTO gimnasio_db.credenciales_admin (AdministradorId, PasswordHash, FechaCreacion)
VALUES (1, 'AQAAAAIAAYagAAAAEBuHNdM23PGmtKWZemsVPI/yVlpffMZFdwMoQANaq0lfUv/eM5Mbrv6S6oG95+nznA==', current_timestamp());
