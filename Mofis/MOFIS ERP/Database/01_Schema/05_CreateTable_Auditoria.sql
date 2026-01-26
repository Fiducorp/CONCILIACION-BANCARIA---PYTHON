-- ============================================================================
-- SCRIPT 5: CREAR TABLA AUDITORIA
-- ============================================================================
-- Descripción: Crea la tabla para registrar todas las acciones de los usuarios
-- Fundamental para trazabilidad y cumplimiento normativo
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.Auditoria', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla Auditoria ya existe. Eliminándola...';
    DROP TABLE dbo.Auditoria;
    PRINT '✓ Tabla Auditoria eliminada';
END
GO

-- Crear tabla Auditoria
CREATE TABLE dbo.Auditoria
(
    AuditoriaID BIGINT IDENTITY(1,1) NOT NULL,
    UsuarioID INT NOT NULL,
    Accion NVARCHAR(50) NOT NULL,              -- LOGIN, LOGOUT, CREAR, EDITAR, ELIMINAR, VER, IMPRIMIR
    Categoria NVARCHAR(50) NULL,               -- SISTEMA, CONTABILIDAD, GERENCIA FINANCIERA
    Modulo NVARCHAR(50) NULL,                  -- Usuarios, Roles, Cuentas por Pagar, etc.
    Formulario NVARCHAR(100) NULL,             -- FormUsuarios, FormSolicitudPago, etc.
    RegistroID INT NULL,                       -- ID del registro afectado (si aplica)
    Detalle NVARCHAR(MAX) NULL,                -- Información adicional en formato JSON o texto
    FechaHora DATETIME NOT NULL DEFAULT GETDATE(),
    DireccionIP NVARCHAR(50) NULL,             -- IP desde donde se realizó la acción
    NombreMaquina NVARCHAR(100) NULL,          -- Nombre de la PC cliente
    
    -- Constraints
    CONSTRAINT PK_Auditoria PRIMARY KEY CLUSTERED (AuditoriaID),
    CONSTRAINT FK_Auditoria_Usuarios FOREIGN KEY (UsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID)
);
GO

-- Crear índices para consultas de auditoría
CREATE NONCLUSTERED INDEX IX_Auditoria_UsuarioID 
ON dbo.Auditoria(UsuarioID);
GO

CREATE NONCLUSTERED INDEX IX_Auditoria_FechaHora 
ON dbo.Auditoria(FechaHora DESC);
GO

CREATE NONCLUSTERED INDEX IX_Auditoria_Accion 
ON dbo.Auditoria(Accion);
GO

CREATE NONCLUSTERED INDEX IX_Auditoria_Modulo 
ON dbo.Auditoria(Modulo);
GO

-- Índice compuesto para búsquedas frecuentes
CREATE NONCLUSTERED INDEX IX_Auditoria_Usuario_Fecha 
ON dbo.Auditoria(UsuarioID, FechaHora DESC) 
INCLUDE (Accion, Modulo, Formulario);
GO

-- Índice para búsqueda por registro afectado
CREATE NONCLUSTERED INDEX IX_Auditoria_RegistroID 
ON dbo.Auditoria(RegistroID, Modulo);
GO

PRINT '✓ Tabla Auditoria creada exitosamente';
PRINT '✓ Relación (Foreign Key) establecida con Usuarios';
PRINT '✓ Índices creados para optimizar consultas de auditoría';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: Auditoria';
PRINT 'Columnas principales:';
PRINT '  - AuditoriaID (PK, BIGINT para gran volumen)';
PRINT '  - UsuarioID (FK → Usuarios)';
PRINT '  - Accion (LOGIN, CREAR, EDITAR, etc.)';
PRINT '  - Categoria, Modulo, Formulario';
PRINT '  - RegistroID (ID del registro afectado)';
PRINT '  - Detalle (información adicional)';
PRINT '  - FechaHora';
PRINT '  - DireccionIP, NombreMaquina';
PRINT 'Estado: CREADA Y LISTA';
PRINT '';
PRINT '=======================================================';
PRINT '🎉 ESQUEMA COMPLETADO';
PRINT '=======================================================';
PRINT 'Todas las tablas del esquema han sido creadas:';
PRINT '  ✓ Roles';
PRINT '  ✓ Usuarios';
PRINT '  ✓ Permisos';
PRINT '  ✓ Auditoria';
PRINT '';
PRINT 'Siguiente paso: Insertar datos iniciales';
PRINT '  - Script 6: Insertar Roles (ROOT, ADMIN, CONTADOR)';
PRINT '  - Script 7: Crear usuario ROOT inicial';
PRINT '=======================================================';
GO