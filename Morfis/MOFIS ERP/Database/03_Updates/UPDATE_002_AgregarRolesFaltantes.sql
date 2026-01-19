USE FiducorpERP;
GO

-- Verificar e insertar roles faltantes
IF NOT EXISTS (SELECT 1 FROM Roles WHERE NombreRol = 'GERENTE')
BEGIN
    INSERT INTO Roles (NombreRol, Descripcion, Categoria, Activo)
    VALUES ('GERENTE', 'Gerente del área. Permisos de supervisión y aprobación.', 'GERENCIA FINANCIERA,GERENCIA LEGAL', 1);
    PRINT '✓ Rol GERENTE insertado';
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE NombreRol = 'ANALISTA')
BEGIN
    INSERT INTO Roles (NombreRol, Descripcion, Categoria, Activo)
    VALUES ('ANALISTA', 'Analista operativo. Permisos de consulta y análisis.', 'CONTABILIDAD,GERENCIA FINANCIERA', 1);
    PRINT '✓ Rol ANALISTA insertado';
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE NombreRol = 'PROBADOR')
BEGIN
    INSERT INTO Roles (NombreRol, Descripcion, Categoria, Activo)
    VALUES ('PROBADOR', 'Usuario de pruebas. Acceso limitado para testing.', 'DESARROLLO', 1);
    PRINT '✓ Rol PROBADOR insertado';
END

-- Verificar
SELECT RolID, NombreRol, Descripcion, Categoria FROM Roles ORDER BY RolID;
GO