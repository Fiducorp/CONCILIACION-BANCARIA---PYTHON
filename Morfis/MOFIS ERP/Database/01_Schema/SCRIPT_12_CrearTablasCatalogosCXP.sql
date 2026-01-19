-- ============================================================================
-- SCRIPT 12: CREAR TABLAS DE CATÁLOGOS PARA CUENTAS POR PAGAR
-- ============================================================================
-- Descripción: Crea las tablas de catálogos necesarias para el módulo CXP
--              - TiposNCF (Comprobantes fiscales DGII)
--              - Monedas (Catálogo de monedas ISO 4217)
--              - TiposPago (Formas de pago)
--              - TiposComprobante (NCF, Cubicación, Cotización, etc.)
--              - TiposFideicomiso (Clasificación de fideicomisos)
-- Fecha: 2026-01-17
-- Módulo: CONTABILIDAD > CUENTAS POR PAGAR
-- ============================================================================

USE FiducorpERP;
GO

PRINT '=======================================================';
PRINT 'SCRIPT 12: CREAR TABLAS DE CATÁLOGOS CXP';
PRINT '=======================================================';
PRINT '';

-- ============================================================================
-- TABLA 1: TiposNCF (Comprobantes Fiscales DGII)
-- ============================================================================
IF OBJECT_ID('dbo.TiposNCF', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla TiposNCF ya existe. Eliminándola...';
    DROP TABLE dbo.TiposNCF;
    PRINT '✓ Tabla TiposNCF eliminada';
END
GO

CREATE TABLE dbo.TiposNCF
(
    TipoNCFID INT IDENTITY(1,1) NOT NULL,
    Codigo NVARCHAR(3) NOT NULL,                -- B01, E31, etc.
    CodigoNumerico NVARCHAR(2) NOT NULL,        -- 01, 31, etc.
    Serie CHAR(1) NOT NULL,                     -- B o E
    Nombre NVARCHAR(100) NOT NULL,              -- Factura de Crédito Fiscal
    NombreCorto NVARCHAR(50) NOT NULL,          -- Crédito Fiscal
    Descripcion NVARCHAR(500) NULL,
    EsElectronico BIT NOT NULL DEFAULT 0,       -- 1 = e-NCF (Serie E)
    LongitudSecuencia INT NOT NULL DEFAULT 8,   -- 8 para Serie B, 10 para Serie E
    LongitudTotal INT NOT NULL DEFAULT 11,      -- 11 para Serie B, 13 para Serie E
    RequiereRNC BIT NOT NULL DEFAULT 1,         -- Si requiere RNC del receptor
    PermiteCredito BIT NOT NULL DEFAULT 0,      -- Si permite crédito fiscal
    OrdenVisualizacion INT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT PK_TiposNCF PRIMARY KEY CLUSTERED (TipoNCFID),
    CONSTRAINT UQ_TiposNCF_Codigo UNIQUE (Codigo)
);
GO

CREATE NONCLUSTERED INDEX IX_TiposNCF_Serie ON dbo.TiposNCF(Serie);
CREATE NONCLUSTERED INDEX IX_TiposNCF_Activo ON dbo.TiposNCF(Activo);
CREATE NONCLUSTERED INDEX IX_TiposNCF_Orden ON dbo.TiposNCF(OrdenVisualizacion);
GO

PRINT '✓ Tabla TiposNCF creada exitosamente';
GO

-- ============================================================================
-- TABLA 2: Monedas (Catálogo ISO 4217)
-- ============================================================================
IF OBJECT_ID('dbo.Monedas', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla Monedas ya existe. Eliminándola...';
    DROP TABLE dbo.Monedas;
    PRINT '✓ Tabla Monedas eliminada';
END
GO

CREATE TABLE dbo.Monedas
(
    MonedaID INT IDENTITY(1,1) NOT NULL,
    CodigoISO CHAR(3) NOT NULL,                 -- DOP, USD, EUR
    Simbolo NVARCHAR(5) NOT NULL,               -- RD$, US$, €
    Nombre NVARCHAR(50) NOT NULL,               -- Peso Dominicano
    NombreIngles NVARCHAR(50) NULL,             -- Dominican Peso
    Pais NVARCHAR(50) NULL,                     -- República Dominicana
    DecimalesMoneda INT NOT NULL DEFAULT 2,     -- Decimales estándar
    FormatoDisplay NVARCHAR(20) NOT NULL DEFAULT '#,##0.00',
    EsLocal BIT NOT NULL DEFAULT 0,             -- 1 = DOP (moneda local)
    OrdenVisualizacion INT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT PK_Monedas PRIMARY KEY CLUSTERED (MonedaID),
    CONSTRAINT UQ_Monedas_CodigoISO UNIQUE (CodigoISO)
);
GO

CREATE NONCLUSTERED INDEX IX_Monedas_Activo ON dbo.Monedas(Activo);
CREATE NONCLUSTERED INDEX IX_Monedas_Orden ON dbo.Monedas(OrdenVisualizacion);
CREATE NONCLUSTERED INDEX IX_Monedas_EsLocal ON dbo.Monedas(EsLocal);
GO

PRINT '✓ Tabla Monedas creada exitosamente';
GO

-- ============================================================================
-- TABLA 3: TiposPago (Formas de Pago)
-- ============================================================================
IF OBJECT_ID('dbo.TiposPago', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla TiposPago ya existe. Eliminándola...';
    DROP TABLE dbo.TiposPago;
    PRINT '✓ Tabla TiposPago eliminada';
END
GO

CREATE TABLE dbo.TiposPago
(
    TipoPagoID INT IDENTITY(1,1) NOT NULL,
    Codigo NVARCHAR(5) NOT NULL,                -- CHQ, TRF, EFE
    Nombre NVARCHAR(50) NOT NULL,               -- Cheque, Transferencia
    Descripcion NVARCHAR(200) NULL,
    RequiereCuenta BIT NOT NULL DEFAULT 0,      -- Si requiere cuenta bancaria
    RequiereReferencia BIT NOT NULL DEFAULT 0,  -- Si requiere número de referencia
    OrdenVisualizacion INT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT PK_TiposPago PRIMARY KEY CLUSTERED (TipoPagoID),
    CONSTRAINT UQ_TiposPago_Codigo UNIQUE (Codigo)
);
GO

CREATE NONCLUSTERED INDEX IX_TiposPago_Activo ON dbo.TiposPago(Activo);
CREATE NONCLUSTERED INDEX IX_TiposPago_Orden ON dbo.TiposPago(OrdenVisualizacion);
GO

PRINT '✓ Tabla TiposPago creada exitosamente';
GO

-- ============================================================================
-- TABLA 4: TiposComprobante (Tipo de documento: NCF, Cubicación, etc.)
-- ============================================================================
IF OBJECT_ID('dbo.TiposComprobante', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla TiposComprobante ya existe. Eliminándola...';
    DROP TABLE dbo.TiposComprobante;
    PRINT '✓ Tabla TiposComprobante eliminada';
END
GO

CREATE TABLE dbo.TiposComprobante
(
    TipoComprobanteID INT IDENTITY(1,1) NOT NULL,
    Codigo NVARCHAR(5) NOT NULL,                -- NCF, CUB, COT
    Nombre NVARCHAR(50) NOT NULL,               -- NCF, Cubicación, Cotización
    Descripcion NVARCHAR(200) NULL,
    RequiereNCF BIT NOT NULL DEFAULT 0,         -- 1 = Debe ingresar NCF
    OrdenVisualizacion INT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT PK_TiposComprobante PRIMARY KEY CLUSTERED (TipoComprobanteID),
    CONSTRAINT UQ_TiposComprobante_Codigo UNIQUE (Codigo)
);
GO

CREATE NONCLUSTERED INDEX IX_TiposComprobante_Activo ON dbo.TiposComprobante(Activo);
CREATE NONCLUSTERED INDEX IX_TiposComprobante_Orden ON dbo.TiposComprobante(OrdenVisualizacion);
GO

PRINT '✓ Tabla TiposComprobante creada exitosamente';
GO

-- ============================================================================
-- TABLA 5: TiposFideicomiso (Clasificación de Fideicomisos)
-- ============================================================================
IF OBJECT_ID('dbo.TiposFideicomiso', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla TiposFideicomiso ya existe. Eliminándola...';
    DROP TABLE dbo.TiposFideicomiso;
    PRINT '✓ Tabla TiposFideicomiso eliminada';
END
GO

CREATE TABLE dbo.TiposFideicomiso
(
    TipoFideicomisoID INT IDENTITY(1,1) NOT NULL,
    Codigo NVARCHAR(10) NOT NULL,               -- INMOB, ADMIN, BAJO, etc.
    Nombre NVARCHAR(100) NOT NULL,              -- Inmobiliario y Garantía
    Descripcion NVARCHAR(500) NULL,
    OrdenVisualizacion INT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT PK_TiposFideicomiso PRIMARY KEY CLUSTERED (TipoFideicomisoID),
    CONSTRAINT UQ_TiposFideicomiso_Codigo UNIQUE (Codigo)
);
GO

CREATE NONCLUSTERED INDEX IX_TiposFideicomiso_Activo ON dbo.TiposFideicomiso(Activo);
CREATE NONCLUSTERED INDEX IX_TiposFideicomiso_Orden ON dbo.TiposFideicomiso(OrdenVisualizacion);
GO

PRINT '✓ Tabla TiposFideicomiso creada exitosamente';
GO

-- ============================================================================
-- TABLA 6: MetodosConversion (Métodos de conversión de moneda)
-- ============================================================================
IF OBJECT_ID('dbo.MetodosConversion', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla MetodosConversion ya existe. Eliminándola...';
    DROP TABLE dbo.MetodosConversion;
    PRINT '✓ Tabla MetodosConversion eliminada';
END
GO

CREATE TABLE dbo.MetodosConversion
(
    MetodoConversionID INT IDENTITY(1,1) NOT NULL,
    Codigo NVARCHAR(10) NOT NULL,               -- DIRECTO, BASE, SELECT, INDIV, MANUAL
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(500) NULL,
    OrdenVisualizacion INT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT PK_MetodosConversion PRIMARY KEY CLUSTERED (MetodoConversionID),
    CONSTRAINT UQ_MetodosConversion_Codigo UNIQUE (Codigo)
);
GO

CREATE NONCLUSTERED INDEX IX_MetodosConversion_Activo ON dbo.MetodosConversion(Activo);
GO

PRINT '✓ Tabla MetodosConversion creada exitosamente';
GO

-- ============================================================================
-- RESUMEN
-- ============================================================================
PRINT '';
PRINT '=======================================================';
PRINT '✓ SCRIPT 12 COMPLETADO EXITOSAMENTE';
PRINT '=======================================================';
PRINT 'Tablas de catálogos creadas:';
PRINT '  1. TiposNCF           - Comprobantes fiscales DGII';
PRINT '  2. Monedas            - Catálogo ISO 4217';
PRINT '  3. TiposPago          - Formas de pago';
PRINT '  4. TiposComprobante   - Tipos de documento';
PRINT '  5. TiposFideicomiso   - Clasificación fideicomisos';
PRINT '  6. MetodosConversion  - Métodos conversión moneda';
PRINT '';
PRINT 'Siguiente paso: Ejecutar Script 13 - Crear tablas principales';
PRINT '=======================================================';
GO
