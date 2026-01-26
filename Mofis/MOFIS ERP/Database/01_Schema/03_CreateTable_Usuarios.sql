-- ============================================================================
-- SCRIPT 3: CREAR TABLA USUARIOS
-- ============================================================================
-- Descripción: Crea la tabla para almacenar los usuarios del sistema
-- Incluye control de contraseñas, roles y auditoría básica
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.Usuarios', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla Usuarios ya existe. Eliminándola...';
    DROP TABLE dbo.Usuarios;
    PRINT '✓ Tabla Usuarios eliminada';
END
GO

-- Crear tabla Usuarios
CREATE TABLE dbo.Usuarios
(
    UsuarioID INT IDENTITY(1,1) NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    NombreCompleto NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NULL,
    RolID INT NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    DebeCambiarPassword BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    UltimoAcceso DATETIME NULL,
    CreadoPorUsuarioID INT NULL,
    FechaModificacion DATETIME NULL,
    ModificadoPorUsuarioID INT NULL,
    
    -- Constraints
    CONSTRAINT PK_Usuarios PRIMARY KEY CLUSTERED (UsuarioID),
    CONSTRAINT UQ_Usuarios_Username UNIQUE (Username),
    CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (RolID) 
        REFERENCES dbo.Roles(RolID),
    CONSTRAINT FK_Usuarios_CreadoPor FOREIGN KEY (CreadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT FK_Usuarios_ModificadoPor FOREIGN KEY (ModificadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID)
);
GO

-- Crear índices
CREATE NONCLUSTERED INDEX IX_Usuarios_Username 
ON dbo.Usuarios(Username);
GO

CREATE NONCLUSTERED INDEX IX_Usuarios_RolID 
ON dbo.Usuarios(RolID);
GO

CREATE NONCLUSTERED INDEX IX_Usuarios_Activo 
ON dbo.Usuarios(Activo);
GO

CREATE NONCLUSTERED INDEX IX_Usuarios_Email 
ON dbo.Usuarios(Email);
GO

PRINT '✓ Tabla Usuarios creada exitosamente';
PRINT '✓ Relaciones (Foreign Keys) establecidas con tabla Roles';
PRINT '✓ Índices creados';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: Usuarios';
PRINT 'Columnas principales:';
PRINT '  - UsuarioID (PK)';
PRINT '  - Username (Unique)';
PRINT '  - PasswordHash (BCrypt)';
PRINT '  - NombreCompleto';
PRINT '  - RolID (FK → Roles)';
PRINT '  - DebeCambiarPassword (BIT)';
PRINT '  - Activo (BIT)';
PRINT 'Estado: CREADA Y LISTA';
PRINT 'Siguiente paso: Ejecutar Script 4 - Crear tabla Permisos';
PRINT '=======================================================';
GO