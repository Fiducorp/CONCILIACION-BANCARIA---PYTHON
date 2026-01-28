-- ============================================================================
-- MIGRACIÓN: AÑADIR COLUMNAS FALTANTES A SolicitudesPago
-- ============================================================================
-- Fecha: 2026-01-28
-- Descripción: Añade columnas necesarias para la nueva lógica de impuestos
--              y detalles de notas si no existen.
-- ============================================================================

USE FiducorpERP;
GO

-- 1. OtrosImpuestosSumar
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SolicitudesPago') AND name = 'OtrosImpuestosSumar')
BEGIN
    ALTER TABLE dbo.SolicitudesPago ADD OtrosImpuestosSumar BIT NOT NULL DEFAULT 1;
    PRINT '✓ Columna OtrosImpuestosSumar añadida';
END
GO

-- 2. ITBISDiferencia
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SolicitudesPago') AND name = 'ITBISDiferencia')
BEGIN
    ALTER TABLE dbo.SolicitudesPago ADD ITBISDiferencia DECIMAL(18,2) NOT NULL DEFAULT 0;
    PRINT '✓ Columna ITBISDiferencia añadida';
END
GO

-- 3. NotaCreditoMostrarDetalle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SolicitudesPago') AND name = 'NotaCreditoMostrarDetalle')
BEGIN
    ALTER TABLE dbo.SolicitudesPago ADD NotaCreditoMostrarDetalle BIT NOT NULL DEFAULT 0;
    PRINT '✓ Columna NotaCreditoMostrarDetalle añadida';
END
GO

-- 4. NotaDebitoMostrarDetalle
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SolicitudesPago') AND name = 'NotaDebitoMostrarDetalle')
BEGIN
    ALTER TABLE dbo.SolicitudesPago ADD NotaDebitoMostrarDetalle BIT NOT NULL DEFAULT 0;
    PRINT '✓ Columna NotaDebitoMostrarDetalle añadida';
END
GO

-- 5. Verificar existencia de tabla SolicitudesPagoMonedaLocal
IF OBJECT_ID('dbo.SolicitudesPagoMonedaLocal', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SolicitudesPagoMonedaLocal
    (
        SolicitudPagoID INT NOT NULL,
        SubtotalL DECIMAL(18,2) NOT NULL,
        ITBISL DECIMAL(18,2) NOT NULL,
        ExentoL DECIMAL(18,2) NOT NULL,
        DireccionTecnicaL DECIMAL(18,2) NOT NULL,
        DescuentoL DECIMAL(18,2) NOT NULL,
        HorasExtrasL DECIMAL(18,2) NOT NULL,
        OtrosImpuestosL DECIMAL(18,2) NOT NULL,
        NotaCreditoL DECIMAL(18,2) NOT NULL,
        NotaDebitoL DECIMAL(18,2) NOT NULL,
        AnticipoL DECIMAL(18,2) NOT NULL,
        AvanceParaPagarL DECIMAL(18,2) NOT NULL,
        RetencionITBISL DECIMAL(18,2) NOT NULL,
        RetencionISRL DECIMAL(18,2) NOT NULL,
        RetencionSFSL DECIMAL(18,2) NOT NULL,
        RetencionAFPL DECIMAL(18,2) NOT NULL,
        TotalRetencionL DECIMAL(18,2) NOT NULL,
        TotalFacturaL DECIMAL(18,2) NOT NULL,
        TotalAPagarL DECIMAL(18,2) NOT NULL,
        CONSTRAINT PK_SolicitudesPagoMonedaLocal PRIMARY KEY (SolicitudPagoID),
        CONSTRAINT FK_SolicitudesPagoMonedaLocal_Main FOREIGN KEY (SolicitudPagoID) 
            REFERENCES dbo.SolicitudesPago(SolicitudPagoID) ON DELETE CASCADE
    );
    PRINT '✓ Tabla SolicitudesPagoMonedaLocal creada';
END
GO
