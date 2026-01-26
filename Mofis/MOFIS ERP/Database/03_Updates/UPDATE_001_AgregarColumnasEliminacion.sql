-- ============================================================================
-- UPDATE 001: AGREGAR COLUMNAS PARA ELIMINACIÓN LÓGICA DE USUARIOS
-- ============================================================================
-- Descripción: Agrega columnas necesarias para implementar eliminación lógica
--              en lugar de eliminación física de usuarios
-- Fecha: 2024-12-25
-- Autor: [Tu nombre]
-- ============================================================================

USE FiducorpERP;
GO

PRINT '=======================================================';
PRINT 'UPDATE 001: Agregar columnas para eliminación lógica';
PRINT '=======================================================';
PRINT '';

-- Verificar si las columnas ya existen antes de agregarlas
IF NOT EXISTS (SELECT 1 FROM sys.columns 
               WHERE object_id = OBJECT_ID('dbo.Usuarios') 
               AND name = 'EsEliminado')
BEGIN
    PRINT 'Agregando columna EsEliminado...';
    ALTER TABLE dbo.Usuarios
    ADD EsEliminado BIT NOT NULL DEFAULT 0;
    PRINT '✓ Columna EsEliminado agregada';
END
ELSE
BEGIN
    PRINT '⚠ Columna EsEliminado ya existe, saltando...';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns 
               WHERE object_id = OBJECT_ID('dbo.Usuarios') 
               AND name = 'FechaEliminacion')
BEGIN
    PRINT 'Agregando columna FechaEliminacion...';
    ALTER TABLE dbo.Usuarios
    ADD FechaEliminacion DATETIME NULL;
    PRINT '✓ Columna FechaEliminacion agregada';
END
ELSE
BEGIN
    PRINT '⚠ Columna FechaEliminacion ya existe, saltando...';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns 
               WHERE object_id = OBJECT_ID('dbo.Usuarios') 
               AND name = 'EliminadoPorUsuarioID')
BEGIN
    PRINT 'Agregando columna EliminadoPorUsuarioID...';
    ALTER TABLE dbo.Usuarios
    ADD EliminadoPorUsuarioID INT NULL;
    PRINT '✓ Columna EliminadoPorUsuarioID agregada';
END
ELSE
BEGIN
    PRINT '⚠ Columna EliminadoPorUsuarioID ya existe, saltando...';
END
GO

-- Crear Foreign Key para EliminadoPorUsuarioID
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys 
               WHERE name = 'FK_Usuarios_EliminadoPor')
BEGIN
    PRINT 'Creando Foreign Key FK_Usuarios_EliminadoPor...';
    ALTER TABLE dbo.Usuarios
    ADD CONSTRAINT FK_Usuarios_EliminadoPor 
        FOREIGN KEY (EliminadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID);
    PRINT '✓ Foreign Key FK_Usuarios_EliminadoPor creada';
END
ELSE
BEGIN
    PRINT '⚠ Foreign Key FK_Usuarios_EliminadoPor ya existe, saltando...';
END
GO

-- Crear índice para mejorar consultas de usuarios no eliminados
IF NOT EXISTS (SELECT 1 FROM sys.indexes 
               WHERE name = 'IX_Usuarios_EsEliminado' 
               AND object_id = OBJECT_ID('dbo.Usuarios'))
BEGIN
    PRINT 'Creando índice IX_Usuarios_EsEliminado...';
    CREATE NONCLUSTERED INDEX IX_Usuarios_EsEliminado 
    ON dbo.Usuarios(EsEliminado);
    PRINT '✓ Índice IX_Usuarios_EsEliminado creado';
END
ELSE
BEGIN
    PRINT '⚠ Índice IX_Usuarios_EsEliminado ya existe, saltando...';
END
GO

PRINT '';
PRINT '=======================================================';
PRINT 'UPDATE 001 - COMPLETADO EXITOSAMENTE';
PRINT '=======================================================';
PRINT 'Columnas agregadas:';
PRINT '  ✓ EsEliminado (BIT, DEFAULT 0)';
PRINT '  ✓ FechaEliminacion (DATETIME NULL)';
PRINT '  ✓ EliminadoPorUsuarioID (INT NULL)';
PRINT '';
PRINT 'Relaciones:';
PRINT '  ✓ FK_Usuarios_EliminadoPor → Usuarios(UsuarioID)';
PRINT '';
PRINT 'Índices:';
PRINT '  ✓ IX_Usuarios_EsEliminado';
PRINT '';
PRINT 'Estado: LISTO PARA USAR';
PRINT 'Funcionalidad habilitada: Eliminación lógica de usuarios';
PRINT '=======================================================';
GO