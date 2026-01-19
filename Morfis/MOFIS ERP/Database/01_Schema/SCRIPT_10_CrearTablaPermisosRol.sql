-- ============================================================================
-- SCRIPT 10: CREAR TABLA PERMISOS ROL
-- ============================================================================
-- Descripción: Nueva tabla de permisos basada en IDs (Foreign Keys)
-- Reemplaza la tabla Permisos antigua que usaba texto
-- Define qué acciones puede realizar cada ROL en cada FORMULARIO
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.PermisosRol', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla PermisosRol ya existe. Eliminándola...';
    DROP TABLE dbo.PermisosRol;
    PRINT '✓ Tabla PermisosRol eliminada';
END
GO

-- Crear tabla PermisosRol
CREATE TABLE dbo.PermisosRol
(
    PermisoRolID INT IDENTITY(1,1) NOT NULL,
    RolID INT NOT NULL,
    FormularioID INT NOT NULL,
    AccionID INT NOT NULL,
    Permitido BIT NOT NULL DEFAULT 0,          -- 1 = Permitido, 0 = Denegado
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    CreadoPorUsuarioID INT NULL,
    FechaModificacion DATETIME NULL,
    ModificadoPorUsuarioID INT NULL,
    
    -- Constraints
    CONSTRAINT PK_PermisosRol PRIMARY KEY CLUSTERED (PermisoRolID),
    CONSTRAINT FK_PermisosRol_Rol FOREIGN KEY (RolID) 
        REFERENCES dbo.Roles(RolID),
    CONSTRAINT FK_PermisosRol_Formulario FOREIGN KEY (FormularioID) 
        REFERENCES dbo.CatalogoFormularios(FormularioID),
    CONSTRAINT FK_PermisosRol_Accion FOREIGN KEY (AccionID) 
        REFERENCES dbo.CatalogoAcciones(AccionID),
    CONSTRAINT FK_PermisosRol_CreadoPor FOREIGN KEY (CreadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT FK_PermisosRol_ModificadoPor FOREIGN KEY (ModificadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    
    -- Constraint para evitar duplicados: un rol no puede tener dos permisos iguales
    CONSTRAINT UQ_PermisosRol_Unico UNIQUE (RolID, FormularioID, AccionID)
);
GO

-- Crear índices para consultas rápidas
CREATE NONCLUSTERED INDEX IX_PermisosRol_RolID 
ON dbo.PermisosRol(RolID);
GO

CREATE NONCLUSTERED INDEX IX_PermisosRol_FormularioID 
ON dbo.PermisosRol(FormularioID);
GO

CREATE NONCLUSTERED INDEX IX_PermisosRol_AccionID 
ON dbo.PermisosRol(AccionID);
GO

-- Índice compuesto para búsquedas de permisos (el más usado)
CREATE NONCLUSTERED INDEX IX_PermisosRol_Busqueda 
ON dbo.PermisosRol(RolID, FormularioID, AccionID) 
INCLUDE (Permitido);
GO

PRINT '✓ Tabla PermisosRol creada exitosamente';
PRINT '✓ Relaciones (Foreign Keys) establecidas';
PRINT '✓ Índices creados para optimizar consultas';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: PermisosRol';
PRINT 'Estructura: RolID + FormularioID + AccionID = Permiso';
PRINT 'Ventaja: Usa IDs en lugar de texto (sin errores de typo)';
PRINT 'Estado: CREADA Y LISTA';
PRINT '=======================================================';
GO