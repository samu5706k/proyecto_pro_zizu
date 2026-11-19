USE gimnasio_db;

CREATE TABLE IF NOT EXISTS credenciales_admin (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    AdministradorId INT NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    FechaCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_credenciales_admin
        FOREIGN KEY (AdministradorId)
        REFERENCES administradores(Id)
        ON DELETE CASCADE
);
