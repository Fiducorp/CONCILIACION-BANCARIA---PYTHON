-- ============================================================================
-- SCRIPT 9: CREAR TABLA CATALOGO ACCIONES
-- ============================================================================
-- Descripción: Crea la tabla maestra de acciones del sistema
-- Define todas las acciones posibles sobre los formularios
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.CatalogoAcciones', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla CatalogoAcciones ya existe. Eliminándola...';
    DROP TABLE dbo.CatalogoAcciones;
    PRINT '✓ Tabla CatalogoAcciones eliminada';
END
GO

-- Crear tabla CatalogoAcciones
CREATE TABLE dbo.CatalogoAcciones
(
    AccionID INT IDENTITY(1,1) NOT NULL,
    CodigoAccion NVARCHAR(20) NOT NULL,        -- VIEW, CREATE, EDIT, DELETE, etc.
    NombreAccion NVARCHAR(50) NOT NULL,        -- Ver, Crear, Editar, Eliminar
    Descripcion NVARCHAR(200) NULL,
    GrupoAccion NVARCHAR(50) NULL,             -- CRUD, IMPRESION, APROBACION, ADMINISTRACION
    OrdenVisualizacion INT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    
    -- Constraints
    CONSTRAINT PK_CatalogoAcciones PRIMARY KEY CLUSTERED (AccionID),
    CONSTRAINT UQ_CatalogoAcciones_Codigo UNIQUE (CodigoAccion)
);
GO

-- Crear índices
CREATE NONCLUSTERED INDEX IX_CatalogoAcciones_GrupoAccion 
ON dbo.CatalogoAcciones(GrupoAccion);
GO

CREATE NONCLUSTERED INDEX IX_CatalogoAcciones_Activo 
ON dbo.CatalogoAcciones(Activo);
GO

PRINT '✓ Tabla CatalogoAcciones creada exitosamente';
PRINT '✓ Índices creados';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: CatalogoAcciones';
PRINT 'Columnas: AccionID, CodigoAccion, NombreAccion, GrupoAccion';
PRINT 'Estado: CREADA Y LISTA';
PRINT '=======================================================';
GO