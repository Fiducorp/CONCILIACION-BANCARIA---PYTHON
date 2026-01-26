-- ============================================================================
-- SCRIPT 2: CREAR TABLA ROLES
-- ============================================================================
-- Descripción: Crea la tabla para almacenar los roles del sistema
-- Roles del sistema: ROOT, ADMIN, CONTADOR
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla Roles ya existe. Eliminándola...';
    DROP TABLE dbo.Roles;
    PRINT '✓ Tabla Roles eliminada';
END
GO

-- Crear tabla Roles
CREATE TABLE dbo.Roles
(
    RolID INT IDENTITY(1,1) NOT NULL,
    NombreRol NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(200) NULL,
    Categoria NVARCHAR(100) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    
    -- Constraints
    CONSTRAINT PK_Roles PRIMARY KEY CLUSTERED (RolID),
    CONSTRAINT UQ_Roles_NombreRol UNIQUE (NombreRol)
);
GO

-- Crear índices
CREATE NONCLUSTERED INDEX IX_Roles_Activo 
ON dbo.Roles(Activo);
GO

CREATE NONCLUSTERED INDEX IX_Roles_Categoria 
ON dbo.Roles(Categoria);
GO

PRINT '✓ Tabla Roles creada exitosamente';
PRINT '✓ Índices creados';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: Roles';
PRINT 'Columnas: RolID, NombreRol, Descripcion, Categoria, Activo, FechaCreacion';
PRINT 'Estado: CREADA Y LISTA';
PRINT 'Siguiente paso: Ejecutar Script 3 - Crear tabla Usuarios';
PRINT '=======================================================';
GO