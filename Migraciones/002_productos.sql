
USE gimnasio_db;

CREATE TABLE IF NOT EXISTS productos (
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
