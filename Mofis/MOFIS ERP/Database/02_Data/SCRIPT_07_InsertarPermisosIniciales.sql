-- ============================================================================
-- SCRIPT 7: INSERTAR PERMISOS INICIALES
-- ============================================================================
-- Descripción: Inserta los permisos iniciales para los 3 roles del sistema
-- ROOT: Acceso total
-- ADMIN: Acceso limitado
-- CONTADOR: Solo consulta en Sistema, CRUD en Contabilidad (sus registros)
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si ya existen permisos
IF EXISTS (SELECT 1 FROM dbo.PermisosRol)
BEGIN
    PRINT 'La tabla PermisosRol ya contiene datos.';
    PRINT 'Eliminando permisos existentes...';
    DELETE FROM dbo.PermisosRol;
    DBCC CHECKIDENT ('dbo.PermisosRol', RESEED, 0);
    PRINT '✓ Permisos anteriores eliminados';
END
GO

-- ============================================================================
-- PERMISOS PARA ROOT (RolID = 1)
-- ROOT tiene acceso TOTAL a TODO
-- ============================================================================
PRINT 'Insertando permisos para ROOT...';

INSERT INTO dbo.PermisosRol (RolID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID)
SELECT 
    1 AS RolID,                    -- ROOT
    F.FormularioID,
    A.AccionID,
    1 AS Permitido,                -- Todo permitido
    1 AS CreadoPorUsuarioID        -- Creado por usuario ROOT
FROM dbo.CatalogoFormularios F
CROSS JOIN dbo.CatalogoAcciones A
WHERE F.Activo = 1 AND A.Activo = 1;

PRINT '✓ Permisos de ROOT insertados (acceso total)';
GO

-- ============================================================================
-- PERMISOS PARA ADMIN (RolID = 2)
-- ADMIN: Todo excepto crear/eliminar usuarios
-- ============================================================================
PRINT 'Insertando permisos para ADMIN...';

-- ADMIN: Acceso completo a todos los formularios
INSERT INTO dbo.PermisosRol (RolID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID)
SELECT 
    2 AS RolID,                    -- ADMIN
    F.FormularioID,
    A.AccionID,
    CASE 
        -- Denegar CREATE y DELETE en FormGestionUsuarios
        WHEN F.CodigoFormulario = 'FGEUSR' AND A.CodigoAccion IN ('CREATE', 'DELETE') THEN 0
        -- Denegar acceso a FormUsuario (crear/editar usuario)
        WHEN F.CodigoFormulario = 'FUSR' THEN 0
        -- Denegar acceso a FormConfirmarEliminacion
        WHEN F.CodigoFormulario = 'FCONFIRMDEL' THEN 0
        -- Todo lo demás: permitido
        ELSE 1
    END AS Permitido,
    1 AS CreadoPorUsuarioID
FROM dbo.CatalogoFormularios F
CROSS JOIN dbo.CatalogoAcciones A
WHERE F.Activo = 1 AND A.Activo = 1;

PRINT '✓ Permisos de ADMIN insertados';
GO

-- ============================================================================
-- PERMISOS PARA CONTADOR (RolID = 3)
-- CONTADOR: Solo consulta en Sistema, CRUD en Contabilidad
-- ============================================================================
PRINT 'Insertando permisos para CONTADOR...';

INSERT INTO dbo.PermisosRol (RolID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID)
SELECT 
    3 AS RolID,                    -- CONTADOR
    F.FormularioID,
    A.AccionID,
    CASE 
        -- ========================================
        -- SISTEMA: Solo consulta (VIEW)
        -- ========================================
        WHEN M.CodigoModulo IN ('GEUSR', 'GEROL', 'AUD') THEN
            CASE 
                WHEN A.CodigoAccion = 'VIEW' THEN 1
                ELSE 0
            END
        
        -- ========================================
        -- CONTABILIDAD: CRUD completo
        -- ========================================
        WHEN M.CodigoModulo IN ('CXP', 'IMP', 'REC', 'FUT') THEN
            CASE 
                -- Permitir operaciones CRUD básicas
                WHEN A.CodigoAccion IN ('VIEW', 'CREATE', 'EDIT', 'DELETE') THEN 1
                -- Permitir impresión y exportación
                WHEN A.CodigoAccion IN ('PRINT', 'EXPORT') THEN 1
                -- Denegar todo lo demás (aprobaciones, etc.)
                ELSE 0
            END
        
        -- Todo lo demás: denegado
        ELSE 0
    END AS Permitido,
    1 AS CreadoPorUsuarioID
FROM dbo.CatalogoFormularios F
INNER JOIN dbo.CatalogoModulos M ON F.ModuloID = M.ModuloID
CROSS JOIN dbo.CatalogoAcciones A
WHERE F.Activo = 1 AND A.Activo = 1;

PRINT '✓ Permisos de CONTADOR insertados';
GO

-- ============================================================================
-- VERIFICACIÓN DE PERMISOS INSERTADOS
-- ============================================================================
PRINT '';
PRINT '=======================================================';
PRINT 'VERIFICACIÓN DE PERMISOS';
PRINT '=======================================================';

-- Contar permisos por rol
SELECT 
    R.RolID,
    R.NombreRol,
    COUNT(*) AS TotalPermisos,
    SUM(CASE WHEN P.Permitido = 1 THEN 1 ELSE 0 END) AS Permitidos,
    SUM(CASE WHEN P.Permitido = 0 THEN 1 ELSE 0 END) AS Denegados
FROM dbo.PermisosRol P
INNER JOIN dbo.Roles R ON P.RolID = R.RolID
GROUP BY R.RolID, R.NombreRol
ORDER BY R.RolID;

PRINT '';
PRINT '=======================================================';
PRINT '✓ Permisos iniciales insertados exitosamente';
PRINT '=======================================================';
PRINT 'Resumen:';
PRINT '  ROOT: Acceso total a todo';
PRINT '  ADMIN: Todo excepto crear/eliminar usuarios';
PRINT '  CONTADOR: Solo consulta en Sistema, CRUD en Contabilidad';
PRINT '';
PRINT 'Estado: COMPLETADO';
PRINT 'Siguiente paso: Crear clase PermisosHelper.cs en C#';
PRINT '=======================================================';
GO