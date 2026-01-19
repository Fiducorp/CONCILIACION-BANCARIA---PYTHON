-- ============================================================================
-- SCRIPT 3: INSERTAR CATEGORÍAS INICIALES
-- ============================================================================
-- Descripción: Inserta las 5 categorías principales del sistema
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si ya existen categorías
IF EXISTS (SELECT 1 FROM dbo.CatalogoCategorias)
BEGIN
    PRINT 'La tabla CatalogoCategorias ya contiene datos.';
    PRINT 'Eliminando categorías existentes...';
    DELETE FROM dbo.CatalogoCategorias;
    DBCC CHECKIDENT ('dbo.CatalogoCategorias', RESEED, 0);
    PRINT '✓ Categorías anteriores eliminadas';
END
GO

-- Insertar categorías del sistema
INSERT INTO dbo.CatalogoCategorias 
    (CodigoCategoria, NombreCategoria, Descripcion, Icono, OrdenVisualizacion, Activo)
VALUES 
    ('SYS', 'SISTEMA', 
     'Configuración y administración del sistema', 
     '⚙️', 1, 1),
    
    ('CONT', 'CONTABILIDAD', 
     'Gestión contable y financiera', 
     '📊', 2, 1),
    
    ('GERFIN', 'GERENCIA FINANCIERA', 
     'Análisis y reportes financieros', 
     '💼', 3, 1),
    
    ('GERLEG', 'GERENCIA LEGAL', 
     'Gestión legal y contratos', 
     '⚖️', 4, 1),
    
    ('DEV', 'DESARROLLO', 
     'Herramientas de desarrollo y módulos futuros', 
     '🚀', 5, 1);
GO

-- Verificar inserción
SELECT 
    CategoriaID,
    CodigoCategoria,
    NombreCategoria,
    Icono,
    OrdenVisualizacion,
    Activo
FROM dbo.CatalogoCategorias
ORDER BY OrdenVisualizacion;
GO

PRINT '';
PRINT '=======================================================';
PRINT '✓ Categorías insertadas exitosamente';
PRINT '=======================================================';
PRINT 'Categorías creadas:';
PRINT '  1. SISTEMA';
PRINT '  2. CONTABILIDAD';
PRINT '  3. GERENCIA FINANCIERA';
PRINT '  4. GERENCIA LEGAL';
PRINT '  5. DESARROLLO';
PRINT '';
PRINT 'Estado: COMPLETADO';
PRINT '=======================================================';
GO