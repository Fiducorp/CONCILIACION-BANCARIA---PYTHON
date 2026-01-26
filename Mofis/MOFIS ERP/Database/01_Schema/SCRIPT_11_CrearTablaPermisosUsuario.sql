-- ============================================================================
-- SCRIPT 11: CREAR TABLA PERMISOS USUARIO
-- ============================================================================
-- Descripción: Tabla de excepciones de permisos por USUARIO
-- Permite sobrescribir permisos del ROL para usuarios específicos
-- Ejemplo: Un Contador puede tener permiso especial para aprobar pagos
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.PermisosUsuario', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla PermisosUsuario ya existe. Eliminándola...';
    DROP TABLE dbo.PermisosUsuario;
    PRINT '✓ Tabla PermisosUsuario eliminada';
END
GO

-- Crear tabla PermisosUsuario
CREATE TABLE dbo.PermisosUsuario
(
    PermisoUsuarioID INT IDENTITY(1,1) NOT NULL,
    UsuarioID INT NOT NULL,
    FormularioID INT NOT NULL,
    AccionID INT NOT NULL,
    Permitido BIT NOT NULL DEFAULT 0,          -- 1 = Permitido, 0 = Denegado
    Motivo NVARCHAR(500) NULL,                 -- Justificación del permiso especial
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    CreadoPorUsuarioID INT NULL,
    FechaModificacion DATETIME NULL,
    ModificadoPorUsuarioID INT NULL,
    FechaExpiracion DATETIME NULL,             -- Opcional: permisos temporales
    
    -- Constraints
    CONSTRAINT PK_PermisosUsuario PRIMARY KEY CLUSTERED (PermisoUsuarioID),
    CONSTRAINT FK_PermisosUsuario_Usuario FOREIGN KEY (UsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT FK_PermisosUsuario_Formulario FOREIGN KEY (FormularioID) 
        REFERENCES dbo.CatalogoFormularios(FormularioID),
    CONSTRAINT FK_PermisosUsuario_Accion FOREIGN KEY (AccionID) 
        REFERENCES dbo.CatalogoAcciones(AccionID),
    CONSTRAINT FK_PermisosUsuario_CreadoPor FOREIGN KEY (CreadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT FK_PermisosUsuario_ModificadoPor FOREIGN KEY (ModificadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    
    -- Constraint para evitar duplicados
    CONSTRAINT UQ_PermisosUsuario_Unico UNIQUE (UsuarioID, FormularioID, AccionID)
);
GO

-- Crear índices para consultas rápidas
CREATE NONCLUSTERED INDEX IX_PermisosUsuario_UsuarioID 
ON dbo.PermisosUsuario(UsuarioID);
GO

CREATE NONCLUSTERED INDEX IX_PermisosUsuario_FormularioID 
ON dbo.PermisosUsuario(FormularioID);
GO

CREATE NONCLUSTERED INDEX IX_PermisosUsuario_AccionID 
ON dbo.PermisosUsuario(AccionID);
GO

-- Índice compuesto para búsquedas de permisos
CREATE NONCLUSTERED INDEX IX_PermisosUsuario_Busqueda 
ON dbo.PermisosUsuario(UsuarioID, FormularioID, AccionID) 
INCLUDE (Permitido);
GO

-- Índice para permisos expirados
CREATE NONCLUSTERED INDEX IX_PermisosUsuario_Expiracion 
ON dbo.PermisosUsuario(FechaExpiracion) 
WHERE FechaExpiracion IS NOT NULL;
GO

PRINT '✓ Tabla PermisosUsuario creada exitosamente';
PRINT '✓ Relaciones (Foreign Keys) establecidas';
PRINT '✓ Índices creados para optimizar consultas';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: PermisosUsuario';
PRINT 'Propósito: Excepciones de permisos para usuarios específicos';
PRINT 'Prioridad: Los permisos de usuario SOBRESCRIBEN los del rol';
PRINT 'Estado: CREADA Y LISTA';
PRINT '=======================================================';
GO