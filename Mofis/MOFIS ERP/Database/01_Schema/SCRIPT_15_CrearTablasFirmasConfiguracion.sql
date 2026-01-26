-- ============================================================================
-- SCRIPT 15: CREAR TABLAS FIRMAS Y CONFIGURACIÓN CXP
-- ============================================================================
-- Descripción: Crea las tablas para gestión de firmas digitales y
--              configuración del módulo Cuentas por Pagar
-- Fecha: 2026-01-17
-- Módulo: CONTABILIDAD > CUENTAS POR PAGAR
-- ============================================================================

USE FiducorpERP;
GO

PRINT '=======================================================';
PRINT 'SCRIPT 15: CREAR TABLAS FIRMAS Y CONFIGURACIÓN';
PRINT '=======================================================';
PRINT '';

-- ============================================================================
-- TABLA 1: FirmasUsuarios (Firmas digitales de usuarios)
-- ============================================================================
IF OBJECT_ID('dbo.FirmasUsuarios', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla FirmasUsuarios ya existe. Eliminándola...';
    DROP TABLE dbo.FirmasUsuarios;
    PRINT '✓ Tabla FirmasUsuarios eliminada';
END
GO

CREATE TABLE dbo.FirmasUsuarios
(
    FirmaID INT IDENTITY(1,1) NOT NULL,
    UsuarioID INT NOT NULL,                     -- FK a Usuarios
    NombreFirma NVARCHAR(100) NOT NULL,         -- Nombre descriptivo (ej: "Firma Principal")
    ImagenFirma VARBINARY(MAX) NOT NULL,        -- Imagen PNG con transparencia
    TipoImagen NVARCHAR(10) NOT NULL DEFAULT 'PNG',  -- PNG recomendado
    AnchoPixeles INT NULL,                      -- Ancho de la imagen
    AltoPixeles INT NULL,                       -- Alto de la imagen
    
    -- Estado
    EsPrincipal BIT NOT NULL DEFAULT 0,         -- Si es la firma principal del usuario
    Activa BIT NOT NULL DEFAULT 1,
    
    -- Auditoría
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    CreadoPorUsuarioID INT NOT NULL,
    FechaModificacion DATETIME NULL,
    ModificadoPorUsuarioID INT NULL,
    
    -- Eliminación lógica
    EsEliminado BIT NOT NULL DEFAULT 0,
    FechaEliminacion DATETIME NULL,
    EliminadoPorUsuarioID INT NULL,
    
    CONSTRAINT PK_FirmasUsuarios PRIMARY KEY CLUSTERED (FirmaID),
    CONSTRAINT FK_FirmasUsuarios_Usuario FOREIGN KEY (UsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT FK_FirmasUsuarios_CreadoPor FOREIGN KEY (CreadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT FK_FirmasUsuarios_ModificadoPor FOREIGN KEY (ModificadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT FK_FirmasUsuarios_EliminadoPor FOREIGN KEY (EliminadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT CK_FirmasUsuarios_TipoImagen CHECK (TipoImagen IN ('PNG', 'JPG', 'JPEG', 'GIF'))
);
GO

-- Solo una firma principal por usuario
CREATE UNIQUE NONCLUSTERED INDEX IX_FirmasUsuarios_Principal 
ON dbo.FirmasUsuarios(UsuarioID) 
WHERE EsPrincipal = 1 AND Activa = 1 AND EsEliminado = 0;

CREATE NONCLUSTERED INDEX IX_FirmasUsuarios_Usuario 
ON dbo.FirmasUsuarios(UsuarioID, Activa) 
WHERE EsEliminado = 0;
GO

PRINT '✓ Tabla FirmasUsuarios creada exitosamente';
GO

-- ============================================================================
-- TABLA 2: ConfiguracionModuloCXP (Configuración del módulo)
-- ============================================================================
IF OBJECT_ID('dbo.ConfiguracionModuloCXP', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla ConfiguracionModuloCXP ya existe. Eliminándola...';
    DROP TABLE dbo.ConfiguracionModuloCXP;
    PRINT '✓ Tabla ConfiguracionModuloCXP eliminada';
END
GO

CREATE TABLE dbo.ConfiguracionModuloCXP
(
    ConfiguracionID INT IDENTITY(1,1) NOT NULL,
    Clave NVARCHAR(50) NOT NULL,                -- Nombre de la configuración
    Valor NVARCHAR(500) NOT NULL,               -- Valor de la configuración
    TipoDato NVARCHAR(20) NOT NULL DEFAULT 'STRING',  -- STRING, INT, DECIMAL, BOOL
    Descripcion NVARCHAR(500) NULL,             -- Descripción para documentación
    Categoria NVARCHAR(50) NULL,                -- Categoría para agrupar
    EsSistema BIT NOT NULL DEFAULT 0,           -- Si es configuración del sistema (no editable)
    
    -- Auditoría
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaModificacion DATETIME NULL,
    ModificadoPorUsuarioID INT NULL,
    
    CONSTRAINT PK_ConfiguracionModuloCXP PRIMARY KEY CLUSTERED (ConfiguracionID),
    CONSTRAINT UQ_ConfiguracionModuloCXP_Clave UNIQUE (Clave),
    CONSTRAINT FK_ConfiguracionModuloCXP_ModificadoPor FOREIGN KEY (ModificadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT CK_ConfiguracionModuloCXP_TipoDato CHECK (TipoDato IN ('STRING', 'INT', 'DECIMAL', 'BOOL'))
);
GO

CREATE NONCLUSTERED INDEX IX_ConfiguracionModuloCXP_Categoria 
ON dbo.ConfiguracionModuloCXP(Categoria);
GO

PRINT '✓ Tabla ConfiguracionModuloCXP creada exitosamente';
GO

-- ============================================================================
-- TABLA 3: MemoriaTemporalFormularios (Para guardado en memoria)
-- ============================================================================
IF OBJECT_ID('dbo.MemoriaTemporalFormularios', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla MemoriaTemporalFormularios ya existe. Eliminándola...';
    DROP TABLE dbo.MemoriaTemporalFormularios;
    PRINT '✓ Tabla MemoriaTemporalFormularios eliminada';
END
GO

CREATE TABLE dbo.MemoriaTemporalFormularios
(
    MemoriaID INT IDENTITY(1,1) NOT NULL,
    UsuarioID INT NOT NULL,                     -- FK a Usuarios
    NombreFormulario NVARCHAR(100) NOT NULL,    -- FormSolicitudPago, etc.
    DatosJSON NVARCHAR(MAX) NOT NULL,           -- Datos serializados en JSON
    FechaGuardado DATETIME NOT NULL DEFAULT GETDATE(),
    SesionID NVARCHAR(100) NULL,                -- Para identificar la sesión
    
    CONSTRAINT PK_MemoriaTemporalFormularios PRIMARY KEY CLUSTERED (MemoriaID),
    CONSTRAINT FK_MemoriaTemporalFormularios_Usuario FOREIGN KEY (UsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID)
);
GO

-- Un usuario solo puede tener una memoria guardada por formulario
CREATE UNIQUE NONCLUSTERED INDEX IX_MemoriaTemporalFormularios_Unico 
ON dbo.MemoriaTemporalFormularios(UsuarioID, NombreFormulario);
GO

PRINT '✓ Tabla MemoriaTemporalFormularios creada exitosamente';
GO

-- ============================================================================
-- AGREGAR FK DE FIRMA A SOLICITUDES DE PAGO
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SolicitudesPago_Firma')
BEGIN
    ALTER TABLE dbo.SolicitudesPago
    ADD CONSTRAINT FK_SolicitudesPago_Firma FOREIGN KEY (FirmaUsuarioID) 
        REFERENCES dbo.FirmasUsuarios(FirmaID);
    
    PRINT '✓ FK_SolicitudesPago_Firma agregada';
END
GO

-- ============================================================================
-- RESUMEN
-- ============================================================================
PRINT '';
PRINT '=======================================================';
PRINT '✓ SCRIPT 15 COMPLETADO EXITOSAMENTE';
PRINT '=======================================================';
PRINT 'Tablas creadas:';
PRINT '';
PRINT '  1. FirmasUsuarios';
PRINT '     - Almacena imágenes de firmas (PNG con transparencia)';
PRINT '     - Una firma principal por usuario';
PRINT '     - Solo el dueño puede usar su firma';
PRINT '';
PRINT '  2. ConfiguracionModuloCXP';
PRINT '     - Configuraciones del módulo CXP';
PRINT '     - Límites, formatos, valores por defecto';
PRINT '';
PRINT '  3. MemoriaTemporalFormularios';
PRINT '     - Guardado en memoria para formularios sin guardar';
PRINT '     - Se limpia al cerrar sesión';
PRINT '';
PRINT 'Siguiente paso: Ejecutar Script 16 - Insertar datos iniciales';
PRINT '=======================================================';
GO
