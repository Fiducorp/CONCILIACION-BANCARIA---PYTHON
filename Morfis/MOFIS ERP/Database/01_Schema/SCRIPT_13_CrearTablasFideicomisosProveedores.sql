-- ============================================================================
-- SCRIPT 13: CREAR TABLAS FIDEICOMISOS Y PROVEEDORES
-- ============================================================================
-- Descripción: Crea las tablas maestras de Fideicomisos y Proveedores
--              Estas son entidades compartidas por todo el módulo CXP
-- Fecha: 2026-01-17
-- Módulo: CONTABILIDAD > CUENTAS POR PAGAR
-- ============================================================================

USE FiducorpERP;
GO

PRINT '=======================================================';
PRINT 'SCRIPT 13: CREAR TABLAS FIDEICOMISOS Y PROVEEDORES';
PRINT '=======================================================';
PRINT '';

-- ============================================================================
-- TABLA 1: Fideicomisos
-- ============================================================================
IF OBJECT_ID('dbo.Fideicomisos', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla Fideicomisos ya existe. Eliminándola...';
    DROP TABLE dbo.Fideicomisos;
    PRINT '✓ Tabla Fideicomisos eliminada';
END
GO

CREATE TABLE dbo.Fideicomisos
(
    FideicomisoID INT IDENTITY(1,1) NOT NULL,
    Codigo NVARCHAR(20) NOT NULL,               -- Código ingresado por usuario
    Nombre NVARCHAR(200) NOT NULL,              -- Nombre del fideicomiso
    RNC NVARCHAR(15) NOT NULL,                  -- RNC del fideicomiso (formato: 000-00000-0)
    TipoFideicomisoID INT NULL,                 -- FK a TiposFideicomiso (opcional)
    
    -- Auditoría
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    CreadoPorUsuarioID INT NULL,
    FechaModificacion DATETIME NULL,
    ModificadoPorUsuarioID INT NULL,
    
    -- Eliminación lógica
    EsEliminado BIT NOT NULL DEFAULT 0,
    FechaEliminacion DATETIME NULL,
    EliminadoPorUsuarioID INT NULL,
    
    -- Constraints
    CONSTRAINT PK_Fideicomisos PRIMARY KEY CLUSTERED (FideicomisoID),
    CONSTRAINT UQ_Fideicomisos_Codigo UNIQUE (Codigo),
    CONSTRAINT UQ_Fideicomisos_RNC UNIQUE (RNC),
    CONSTRAINT FK_Fideicomisos_TipoFideicomiso FOREIGN KEY (TipoFideicomisoID) 
        REFERENCES dbo.TiposFideicomiso(TipoFideicomisoID),
    CONSTRAINT FK_Fideicomisos_CreadoPor FOREIGN KEY (CreadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT FK_Fideicomisos_ModificadoPor FOREIGN KEY (ModificadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT FK_Fideicomisos_EliminadoPor FOREIGN KEY (EliminadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID)
);
GO

-- Índices para búsquedas frecuentes
CREATE NONCLUSTERED INDEX IX_Fideicomisos_Codigo ON dbo.Fideicomisos(Codigo);
CREATE NONCLUSTERED INDEX IX_Fideicomisos_Nombre ON dbo.Fideicomisos(Nombre);
CREATE NONCLUSTERED INDEX IX_Fideicomisos_RNC ON dbo.Fideicomisos(RNC);
CREATE NONCLUSTERED INDEX IX_Fideicomisos_Activo ON dbo.Fideicomisos(Activo) WHERE EsEliminado = 0;
CREATE NONCLUSTERED INDEX IX_Fideicomisos_TipoFideicomiso ON dbo.Fideicomisos(TipoFideicomisoID);

-- Índice compuesto para búsquedas de autocompletado
CREATE NONCLUSTERED INDEX IX_Fideicomisos_Busqueda 
ON dbo.Fideicomisos(Activo, EsEliminado) 
INCLUDE (Codigo, Nombre, RNC);
GO

PRINT '✓ Tabla Fideicomisos creada exitosamente';
GO

-- ============================================================================
-- TABLA 2: Proveedores
-- ============================================================================
IF OBJECT_ID('dbo.Proveedores', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla Proveedores ya existe. Eliminándola...';
    DROP TABLE dbo.Proveedores;
    PRINT '✓ Tabla Proveedores eliminada';
END
GO

CREATE TABLE dbo.Proveedores
(
    ProveedorID INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(200) NOT NULL,              -- Nombre o Razón Social
    TipoDocumento CHAR(1) NOT NULL DEFAULT 'R', -- R = RNC, C = Cédula
    NumeroDocumento NVARCHAR(15) NOT NULL,      -- RNC o Cédula (con formato)
    Telefono NVARCHAR(20) NULL,                 -- Opcional
    Email NVARCHAR(100) NULL,                   -- Opcional
    
    -- Auditoría
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    CreadoPorUsuarioID INT NULL,
    FechaModificacion DATETIME NULL,
    ModificadoPorUsuarioID INT NULL,
    
    -- Eliminación lógica
    EsEliminado BIT NOT NULL DEFAULT 0,
    FechaEliminacion DATETIME NULL,
    EliminadoPorUsuarioID INT NULL,
    
    -- Constraints
    CONSTRAINT PK_Proveedores PRIMARY KEY CLUSTERED (ProveedorID),
    CONSTRAINT UQ_Proveedores_NumeroDocumento UNIQUE (NumeroDocumento),
    CONSTRAINT CK_Proveedores_TipoDocumento CHECK (TipoDocumento IN ('R', 'C')),
    CONSTRAINT FK_Proveedores_CreadoPor FOREIGN KEY (CreadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT FK_Proveedores_ModificadoPor FOREIGN KEY (ModificadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT FK_Proveedores_EliminadoPor FOREIGN KEY (EliminadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID)
);
GO

-- Índices para búsquedas frecuentes
CREATE NONCLUSTERED INDEX IX_Proveedores_Nombre ON dbo.Proveedores(Nombre);
CREATE NONCLUSTERED INDEX IX_Proveedores_NumeroDocumento ON dbo.Proveedores(NumeroDocumento);
CREATE NONCLUSTERED INDEX IX_Proveedores_Activo ON dbo.Proveedores(Activo) WHERE EsEliminado = 0;
CREATE NONCLUSTERED INDEX IX_Proveedores_TipoDocumento ON dbo.Proveedores(TipoDocumento);

-- Índice compuesto para búsquedas de autocompletado
CREATE NONCLUSTERED INDEX IX_Proveedores_Busqueda 
ON dbo.Proveedores(Activo, EsEliminado) 
INCLUDE (Nombre, NumeroDocumento, TipoDocumento, Telefono, Email);
GO

PRINT '✓ Tabla Proveedores creada exitosamente';
GO

-- ============================================================================
-- RESUMEN
-- ============================================================================
PRINT '';
PRINT '=======================================================';
PRINT '✓ SCRIPT 13 COMPLETADO EXITOSAMENTE';
PRINT '=======================================================';
PRINT 'Tablas principales creadas:';
PRINT '';
PRINT '  1. Fideicomisos';
PRINT '     - Codigo (único, ingresado por usuario)';
PRINT '     - Nombre';
PRINT '     - RNC (único, con formato)';
PRINT '     - TipoFideicomiso (FK, opcional)';
PRINT '     - Auditoría completa';
PRINT '     - Eliminación lógica';
PRINT '';
PRINT '  2. Proveedores';
PRINT '     - ID auto-generado';
PRINT '     - Nombre/Razón Social';
PRINT '     - TipoDocumento (R=RNC, C=Cédula)';
PRINT '     - NumeroDocumento (único, con formato)';
PRINT '     - Telefono, Email (opcionales)';
PRINT '     - Auditoría completa';
PRINT '     - Eliminación lógica';
PRINT '';
PRINT 'Siguiente paso: Ejecutar Script 14 - Crear tabla SolicitudesPago';
PRINT '=======================================================';
GO
