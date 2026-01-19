-- ============================================================================
-- SCRIPT 4: CREAR TABLA PERMISOS
-- ============================================================================
-- Descripción: Crea la tabla para almacenar los permisos por rol
-- Define qué acciones puede realizar cada rol en cada módulo/formulario
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.Permisos', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla Permisos ya existe. Eliminándola...';
    DROP TABLE dbo.Permisos;
    PRINT '✓ Tabla Permisos eliminada';
END
GO

-- Crear tabla Permisos
CREATE TABLE dbo.Permisos
(
    PermisoID INT IDENTITY(1,1) NOT NULL,
    RolID INT NOT NULL,
    Categoria NVARCHAR(50) NOT NULL,           -- Ej: SISTEMA, CONTABILIDAD, GERENCIA FINANCIERA
    Modulo NVARCHAR(50) NOT NULL,              -- Ej: Cuentas por Pagar, Usuarios, Roles
    Formulario NVARCHAR(100) NULL,             -- Ej: FormSolicitudPago, FormUsuarios (opcional)
    Accion NVARCHAR(50) NOT NULL,              -- Ej: Ver, Crear, Editar, Eliminar, Imprimir
    Permitido BIT NOT NULL DEFAULT 0,          -- 1 = Permitido, 0 = Denegado
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    CreadoPorUsuarioID INT NULL,
    
    -- Constraints
    CONSTRAINT PK_Permisos PRIMARY KEY CLUSTERED (PermisoID),
    CONSTRAINT FK_Permisos_Roles FOREIGN KEY (RolID) 
        REFERENCES dbo.Roles(RolID),
    CONSTRAINT FK_Permisos_CreadoPor FOREIGN KEY (CreadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    
    -- Constraint para evitar duplicados
    CONSTRAINT UQ_Permisos_Unico UNIQUE (RolID, Categoria, Modulo, Formulario, Accion)
);
GO

-- Crear índices para consultas rápidas
CREATE NONCLUSTERED INDEX IX_Permisos_RolID 
ON dbo.Permisos(RolID);
GO

CREATE NONCLUSTERED INDEX IX_Permisos_Categoria 
ON dbo.Permisos(Categoria);
GO

CREATE NONCLUSTERED INDEX IX_Permisos_Modulo 
ON dbo.Permisos(Modulo);
GO

CREATE NONCLUSTERED INDEX IX_Permisos_Accion 
ON dbo.Permisos(Accion);
GO

-- Índice compuesto para búsquedas de permisos
CREATE NONCLUSTERED INDEX IX_Permisos_Busqueda 
ON dbo.Permisos(RolID, Categoria, Modulo, Accion) 
INCLUDE (Permitido);
GO

PRINT '✓ Tabla Permisos creada exitosamente';
PRINT '✓ Relaciones (Foreign Keys) establecidas';
PRINT '✓ Índices creados para optimizar consultas';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: Permisos';
PRINT 'Columnas principales:';
PRINT '  - PermisoID (PK)';
PRINT '  - RolID (FK → Roles)';
PRINT '  - Categoria (SISTEMA, CONTABILIDAD, etc.)';
PRINT '  - Modulo (Cuentas por Pagar, Usuarios, etc.)';
PRINT '  - Formulario (Opcional)';
PRINT '  - Accion (Ver, Crear, Editar, Eliminar, etc.)';
PRINT '  - Permitido (BIT)';
PRINT 'Estado: CREADA Y LISTA';
PRINT 'Siguiente paso: Ejecutar Script 5 - Crear tabla Auditoria';
PRINT '=======================================================';
GO