-- ============================================================================
-- SCRIPT 6: CREAR TABLA CATALOGO CATEGORIAS
-- ============================================================================
-- Descripción: Crea la tabla maestra de categorías del sistema
-- Define las grandes áreas funcionales (SISTEMA, CONTABILIDAD, etc.)
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.CatalogoCategorias', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla CatalogoCategorias ya existe. Eliminándola...';
    DROP TABLE dbo.CatalogoCategorias;
    PRINT '✓ Tabla CatalogoCategorias eliminada';
END
GO

-- Crear tabla CatalogoCategorias
CREATE TABLE dbo.CatalogoCategorias
(
    CategoriaID INT IDENTITY(1,1) NOT NULL,
    CodigoCategoria NVARCHAR(20) NOT NULL,     -- SYS, CONT, GERFIN, GERLEG, DEV
    NombreCategoria NVARCHAR(100) NOT NULL,    -- SISTEMA, CONTABILIDAD, etc.
    Descripcion NVARCHAR(500) NULL,
    Icono NVARCHAR(10) NULL,                   -- Emoji para UI
    OrdenVisualizacion INT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    
    -- Constraints
    CONSTRAINT PK_CatalogoCategorias PRIMARY KEY CLUSTERED (CategoriaID),
    CONSTRAINT UQ_CatalogoCategorias_Codigo UNIQUE (CodigoCategoria),
    CONSTRAINT UQ_CatalogoCategorias_Nombre UNIQUE (NombreCategoria)
);
GO

-- Crear índices
CREATE NONCLUSTERED INDEX IX_CatalogoCategorias_Activo 
ON dbo.CatalogoCategorias(Activo);
GO

CREATE NONCLUSTERED INDEX IX_CatalogoCategorias_Orden 
ON dbo.CatalogoCategorias(OrdenVisualizacion);
GO

PRINT '✓ Tabla CatalogoCategorias creada exitosamente';
PRINT '✓ Índices creados';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: CatalogoCategorias';
PRINT 'Columnas: CategoriaID, CodigoCategoria, NombreCategoria, Icono, Orden';
PRINT 'Estado: CREADA Y LISTA';
PRINT '=======================================================';
GO