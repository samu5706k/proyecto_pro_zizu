USE gimnasio_db;

-- MEMBRESÍAS ASIGNADAS A CLIENTES
CREATE TABLE IF NOT EXISTS membresias (
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
CREATE TABLE IF NOT EXISTS pagos (
    Id INT NOT NULL AUTO_INCREMENT,
    MembresiaId INT NOT NULL,
    FechaPago DATE NOT NULL,
    Monto DECIMAL(10,2) NOT NULL,
    Metodo VARCHAR(30) NOT NULL,
    Referencia VARCHAR(80) NOT NULL DEFAULT '',

    PRIMARY KEY (Id),

    INDEX idx_pagos_membresia (MembresiaId),

    CONSTRAINT fk_pagos_membresias
        FOREIGN KEY (MembresiaId)
        REFERENCES membresias(Id)
        ON DELETE RESTRICT,

    CONSTRAINT chk_pagos_monto
        CHECK (Monto > 0)
) ENGINE=InnoDB;
