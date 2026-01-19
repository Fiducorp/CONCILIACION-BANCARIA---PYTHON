-- ============================================================================
-- SCRIPT 8: CREAR TABLA CATALOGO FORMULARIOS
-- ============================================================================
-- Descripción: Crea la tabla maestra de formularios del sistema
-- Define todos los formularios/pantallas con código único para permisos
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.CatalogoFormularios', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla CatalogoFormularios ya existe. Eliminándola...';
    DROP TABLE dbo.CatalogoFormularios;
    PRINT '✓ Tabla CatalogoFormularios eliminada';
END
GO

-- Crear tabla CatalogoFormularios
CREATE TABLE dbo.CatalogoFormularios
(
    FormularioID INT IDENTITY(1,1) NOT NULL,
    ModuloID INT NOT NULL,
    CodigoFormulario NVARCHAR(20) NOT NULL,    -- FGEUSR, FSOLPAG, FCARTRET
    NombreFormulario NVARCHAR(100) NOT NULL,   -- Gestión de Usuarios, Solicitud de Pago
    NombreClase NVARCHAR(100) NOT NULL,        -- FormGestionUsuarios, FormSolicitudPago
    RutaCompleta NVARCHAR(200) NULL,           -- Sistema > Gestión de Usuarios
    Descripcion NVARCHAR(500) NULL,
    OrdenVisualizacion INT NOT NULL DEFAULT 0,
    EsReporte BIT NOT NULL DEFAULT 0,          -- Distinguir formularios de reportes
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    CreadoPorUsuarioID INT NULL,
    
    -- Constraints
    CONSTRAINT PK_CatalogoFormularios PRIMARY KEY CLUSTERED (FormularioID),
    CONSTRAINT UQ_CatalogoFormularios_Codigo UNIQUE (CodigoFormulario),
    CONSTRAINT UQ_CatalogoFormularios_NombreClase UNIQUE (NombreClase),
    CONSTRAINT FK_CatalogoFormularios_Modulo FOREIGN KEY (ModuloID) 
        REFERENCES dbo.CatalogoModulos(ModuloID),
    CONSTRAINT FK_CatalogoFormularios_CreadoPor FOREIGN KEY (CreadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID)
);
GO

-- Crear índices
CREATE NONCLUSTERED INDEX IX_CatalogoFormularios_ModuloID 
ON dbo.CatalogoFormularios(ModuloID);
GO

CREATE NONCLUSTERED INDEX IX_CatalogoFormularios_Activo 
ON dbo.CatalogoFormularios(Activo);
GO

CREATE NONCLUSTERED INDEX IX_CatalogoFormularios_Codigo 
ON dbo.CatalogoFormularios(CodigoFormulario);
GO

PRINT '✓ Tabla CatalogoFormularios creada exitosamente';
PRINT '✓ Relación con CatalogoModulos establecida';
PRINT '✓ Índices creados';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: CatalogoFormularios';
PRINT 'Columnas: FormularioID, ModuloID, CodigoFormulario, NombreClase';
PRINT 'Estado: CREADA Y LISTA';
PRINT '=======================================================';
GO