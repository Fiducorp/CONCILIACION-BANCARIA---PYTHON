-- ============================================================================
-- UPDATE 004: AGREGAR COLUMNA EsSistema A TABLA ROLES
-- ============================================================================
-- Descripción: Permite diferenciar roles del sistema (fijos) de roles personalizados
-- Fecha: 2025-12-27
-- Autor: Sistema MOFIS-ERP
-- ============================================================================

USE FiducorpERP;
GO

PRINT '=======================================================';
PRINT 'INICIO: UPDATE_002 - Agregar columna EsSistema';
PRINT '=======================================================';
PRINT '';

-- Verificar si la columna ya existe
IF NOT EXISTS (SELECT 1 FROM sys.columns 
               WHERE object_id = OBJECT_ID('dbo.Roles') 
               AND name = 'EsSistema')
BEGIN
    PRINT 'Agregando columna EsSistema a tabla Roles...';
    
    -- Agregar columna EsSistema
    ALTER TABLE dbo.Roles
    ADD EsSistema BIT NOT NULL DEFAULT 0;
    
    PRINT '✓ Columna EsSistema agregada exitosamente';
    PRINT '';
END
ELSE
BEGIN
    PRINT '⚠ La columna EsSistema ya existe. Omitiendo creación.';
    PRINT '';
END
GO

-- Marcar los roles actuales como "roles de sistema"
PRINT 'Marcando roles del sistema...';

UPDATE dbo.Roles
SET EsSistema = 1
WHERE NombreRol IN ('ROOT', 'ADMIN', 'GERENTE', 'CONTADOR', 'ANALISTA', 'PROBADOR');

DECLARE @RolesSistema INT = @@ROWCOUNT;
PRINT CONCAT('✓ ', @RolesSistema, ' roles marcados como roles del sistema');
PRINT '';
GO

-- Verificar resultado
PRINT 'Verificando resultado:';
PRINT '';

SELECT 
    RolID,
    NombreRol,
    CASE WHEN EsSistema = 1 THEN 'Sistema' ELSE 'Personalizado' END AS TipoRol,
    Activo,
    FechaCreacion
FROM dbo.Roles
ORDER BY EsSistema DESC, RolID;
GO

PRINT '';
PRINT '=======================================================';
PRINT '✓ UPDATE_002 completado exitosamente';
PRINT '=======================================================';
PRINT 'Resumen:';
PRINT '  - Columna EsSistema agregada';
PRINT '  - Roles del sistema marcados (ROOT, ADMIN, etc.)';
PRINT '  - Roles personalizados tendrán EsSistema = 0';
PRINT '';
PRINT 'Siguiente paso: Implementar gestión de roles dinámicos en UI';
PRINT '=======================================================';
GO