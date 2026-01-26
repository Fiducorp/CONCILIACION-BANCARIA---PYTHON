USE FiducorpERP;
GO

-- Insertar permisos básicos para GERENTE (similar a ADMIN pero más limitado)
INSERT INTO PermisosRol (RolID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID)
SELECT 
    (SELECT RolID FROM Roles WHERE NombreRol = 'GERENTE'),
    F.FormularioID,
    A.AccionID,
    CASE 
        WHEN A.CodigoAccion IN ('VIEW', 'PRINT', 'EXPORT') THEN 1
        ELSE 0
    END AS Permitido,
    1
FROM CatalogoFormularios F
CROSS JOIN CatalogoAcciones A
WHERE F.Activo = 1 AND A.Activo = 1
    AND A.CodigoAccion IN ('VIEW', 'CREATE', 'EDIT', 'DELETE', 'PRINT', 'EXPORT')
    AND NOT EXISTS (
        SELECT 1 FROM PermisosRol PR 
        WHERE PR.RolID = (SELECT RolID FROM Roles WHERE NombreRol = 'GERENTE')
        AND PR.FormularioID = F.FormularioID
        AND PR.AccionID = A.AccionID
    );

-- Insertar permisos para ANALISTA (solo consulta)
INSERT INTO PermisosRol (RolID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID)
SELECT 
    (SELECT RolID FROM Roles WHERE NombreRol = 'ANALISTA'),
    F.FormularioID,
    A.AccionID,
    CASE 
        WHEN A.CodigoAccion = 'VIEW' THEN 1
        ELSE 0
    END AS Permitido,
    1
FROM CatalogoFormularios F
CROSS JOIN CatalogoAcciones A
WHERE F.Activo = 1 AND A.Activo = 1
    AND A.CodigoAccion IN ('VIEW', 'CREATE', 'EDIT', 'DELETE', 'PRINT', 'EXPORT')
    AND NOT EXISTS (
        SELECT 1 FROM PermisosRol PR 
        WHERE PR.RolID = (SELECT RolID FROM Roles WHERE NombreRol = 'ANALISTA')
        AND PR.FormularioID = F.FormularioID
        AND PR.AccionID = A.AccionID
    );

-- Insertar permisos para PROBADOR (acceso de testing)
INSERT INTO PermisosRol (RolID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID)
SELECT 
    (SELECT RolID FROM Roles WHERE NombreRol = 'PROBADOR'),
    F.FormularioID,
    A.AccionID,
    1 AS Permitido,
    1
FROM CatalogoFormularios F
CROSS JOIN CatalogoAcciones A
WHERE F.Activo = 1 AND A.Activo = 1
    AND A.CodigoAccion IN ('VIEW', 'CREATE', 'EDIT', 'DELETE', 'PRINT', 'EXPORT')
    AND NOT EXISTS (
        SELECT 1 FROM PermisosRol PR 
        WHERE PR.RolID = (SELECT RolID FROM Roles WHERE NombreRol = 'PROBADOR')
        AND PR.FormularioID = F.FormularioID
        AND PR.AccionID = A.AccionID
    );

PRINT '✓ Permisos insertados para todos los roles';
GO