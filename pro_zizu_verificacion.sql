-- Consultas de verificación del esquema Pro Zizu; no modifican registros.
USE gimnasio_db;

-- Deben aparecer las 7 tablas del proyecto.
SHOW TABLES;

-- Deben aparecer 3 relaciones mediante claves foráneas.
SELECT
    TABLE_NAME AS TablaHija,
    COLUMN_NAME AS ColumnaFK,
    REFERENCED_TABLE_NAME AS TablaPadre,
    REFERENCED_COLUMN_NAME AS ColumnaPK
FROM information_schema.KEY_COLUMN_USAGE
WHERE TABLE_SCHEMA = 'gimnasio_db'
  AND REFERENCED_TABLE_NAME IS NOT NULL
ORDER BY TABLE_NAME;

-- Cantidad de registros por tabla, sin exponer datos personales.
SELECT 'administradores' AS tabla, COUNT(*) AS registros FROM administradores
UNION ALL SELECT 'credenciales_admin', COUNT(*) FROM credenciales_admin
UNION ALL SELECT 'clientes', COUNT(*) FROM clientes
UNION ALL SELECT 'entrenadores', COUNT(*) FROM entrenadores
UNION ALL SELECT 'productos', COUNT(*) FROM productos
UNION ALL SELECT 'membresias', COUNT(*) FROM membresias
UNION ALL SELECT 'pagos', COUNT(*) FROM pagos;

-- Detecta pagos anteriores sin referencia.
SELECT Id, MembresiaId, FechaPago
FROM pagos
WHERE TRIM(Referencia) = '';
