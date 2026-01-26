-- ============================================================================
-- SCRIPT 1: INSERTAR ROLES INICIALES
-- ============================================================================
-- Descripción: Inserta los 3 roles fundamentales del sistema
-- ROOT, ADMIN, CONTADOR
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si ya existen roles
IF EXISTS (SELECT 1 FROM dbo.Roles)
BEGIN
    PRINT 'La tabla Roles ya contiene datos. Eliminando roles existentes...';
    DELETE FROM dbo.Roles;
    DBCC CHECKIDENT ('dbo.Roles', RESEED, 0);
    PRINT '✓ Roles anteriores eliminados';
END
GO

-- Insertar los 3 roles del sistema
INSERT INTO dbo.Roles (NombreRol, Descripcion, Categoria, Activo)
VALUES 
    ('ROOT', 
     'Administrador técnico del sistema. Acceso total y gestión completa de usuarios, roles, permisos y configuración.', 
     'SISTEMA', 
     1),
    
    ('ADMIN', 
     'Administrador funcional. Acceso completo a todas las categorías operativas (Contabilidad, Gerencia Financiera, Gerencia Legal). Gestión limitada de usuarios.', 
     'SISTEMA,CONTABILIDAD,GERENCIA FINANCIERA,GERENCIA LEGAL', 
     1),
    
    ('CONTADOR', 
     'Usuario operativo del área contable. Acceso exclusivo a módulos de Contabilidad. CRUD completo solo sobre sus propios registros.', 
     'CONTABILIDAD', 
     1);
GO

-- Verificar inserción
SELECT 
    RolID,
    NombreRol,
    Descripcion,
    Categoria,
    Activo,
    FechaCreacion
FROM dbo.Roles
ORDER BY RolID;
GO

PRINT '';
PRINT '=======================================================';
PRINT '✓ Roles insertados exitosamente';
PRINT '=======================================================';
PRINT 'Roles creados:';
PRINT '  1. ROOT     - Acceso total al sistema';
PRINT '  2. ADMIN    - Acceso funcional completo';
PRINT '  3. CONTADOR - Acceso solo a Contabilidad';
PRINT '';
PRINT 'Estado: COMPLETADO';
PRINT 'Siguiente paso: Script 7 - Crear usuario ROOT inicial';
PRINT '=======================================================';
GO