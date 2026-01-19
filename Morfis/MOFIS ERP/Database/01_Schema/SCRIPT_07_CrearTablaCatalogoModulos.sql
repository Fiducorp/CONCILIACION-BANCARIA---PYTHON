-- ============================================================================
-- SCRIPT 7: CREAR TABLA CATALOGO MODULOS
-- ============================================================================
-- Descripción: Crea la tabla maestra de módulos del sistema
-- Define los módulos dentro de cada categoría
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.CatalogoModulos', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla CatalogoModulos ya existe. Eliminándola...';
    DROP TABLE dbo.CatalogoModulos;
    PRINT '✓ Tabla CatalogoModulos eliminada';
END
GO

-- Crear tabla CatalogoModulos
CREATE TABLE dbo.CatalogoModulos
(
    ModuloID INT IDENTITY(1,1) NOT NULL,
    CategoriaID INT NOT NULL,
    CodigoModulo NVARCHAR(20) NOT NULL,        -- GEUSR, CXP, IMP, REC
    NombreModulo NVARCHAR(100) NOT NULL,       -- Gestión de Usuarios, Cuentas por Pagar
    Descripcion NVARCHAR(500) NULL,
    Icono NVARCHAR(10) NULL,                   -- Emoji para UI
    OrdenVisualizacion INT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    
    -- Constraints
    CONSTRAINT PK_CatalogoModulos PRIMARY KEY CLUSTERED (ModuloID),
    CONSTRAINT UQ_CatalogoModulos_Codigo UNIQUE (CodigoModulo),
    CONSTRAINT FK_CatalogoModulos_Categoria FOREIGN KEY (CategoriaID) 
        REFERENCES dbo.CatalogoCategorias(CategoriaID)
);
GO

-- Crear índices
CREATE NONCLUSTERED INDEX IX_CatalogoModulos_CategoriaID 
ON dbo.CatalogoModulos(CategoriaID);
GO

CREATE NONCLUSTERED INDEX IX_CatalogoModulos_Activo 
ON dbo.CatalogoModulos(Activo);
GO

CREATE NONCLUSTERED INDEX IX_CatalogoModulos_Orden 
ON dbo.CatalogoModulos(OrdenVisualizacion);
GO

PRINT '✓ Tabla CatalogoModulos creada exitosamente';
PRINT '✓ Relación con CatalogoCategorias establecida';
PRINT '✓ Índices creados';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: CatalogoModulos';
PRINT 'Columnas: ModuloID, CategoriaID, CodigoModulo, NombreModulo';
PRINT 'Estado: CREADA Y LISTA';
PRINT '=======================================================';
GO