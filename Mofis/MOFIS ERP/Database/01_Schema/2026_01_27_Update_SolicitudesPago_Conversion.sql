-- ============================================================================
-- SCRIPT DE ACTUALIZACIÓN: NORMALIZACIÓN DE CONVERSIÓN DE MONEDA
-- ============================================================================
-- Descripción: 
-- 1. Crea la tabla SolicitudesPagoMonedaLocal para montos convertidos.
-- 2. Agrega campos faltantes en SolicitudesPago (OtrosImpuestosSumar, ITBISDiferencia).
-- 3. Limpia columnas convertidas redundantes de la tabla principal.
-- ============================================================================

USE FiducorpERP;
GO

PRINT 'Iniciando actualización de esquema para Conversión de Moneda...';

-- 1. AGREGAR CAMPOS FALTANTES A SolicitudesPago
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SolicitudesPago') AND name = 'OtrosImpuestosSumar')
BEGIN
    ALTER TABLE dbo.SolicitudesPago ADD OtrosImpuestosSumar BIT NOT NULL DEFAULT 1;
    PRINT '✓ Columna OtrosImpuestosSumar agregada.';
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SolicitudesPago') AND name = 'ITBISDiferencia')
BEGIN
    ALTER TABLE dbo.SolicitudesPago ADD ITBISDiferencia DECIMAL(18,2) NOT NULL DEFAULT 0;
    PRINT '✓ Columna ITBISDiferencia agregada.';
END
GO

-- 2. CREAR TABLA SATÉLITE PARA MONTOS CONVERTIDOS
IF OBJECT_ID('dbo.SolicitudesPagoMonedaLocal', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SolicitudesPagoMonedaLocal
    (
        SolicitudPagoID INT NOT NULL,
        
        -- Montos Principales Convertidos (DOP o Moneda Local)
        SubtotalL DECIMAL(18,2) NOT NULL DEFAULT 0,
        ITBISL DECIMAL(18,2) NOT NULL DEFAULT 0,
        ExentoL DECIMAL(18,2) NOT NULL DEFAULT 0,
        DireccionTecnicaL DECIMAL(18,2) NOT NULL DEFAULT 0,
        DescuentoL DECIMAL(18,2) NOT NULL DEFAULT 0,
        HorasExtrasL DECIMAL(18,2) NOT NULL DEFAULT 0,
        OtrosImpuestosL DECIMAL(18,2) NOT NULL DEFAULT 0,
        
        -- Notas y Ajustes Convertidos
        NotaCreditoL DECIMAL(18,2) NOT NULL DEFAULT 0,
        NotaDebitoL DECIMAL(18,2) NOT NULL DEFAULT 0,
        AnticipoL DECIMAL(18,2) NOT NULL DEFAULT 0,
        AvanceParaPagarL DECIMAL(18,2) NOT NULL DEFAULT 0,
        
        -- Retenciones Convertidas
        RetencionITBISL DECIMAL(18,2) NOT NULL DEFAULT 0,
        RetencionISRL DECIMAL(18,2) NOT NULL DEFAULT 0,
        RetencionSFSL DECIMAL(18,2) NOT NULL DEFAULT 0,
        RetencionAFPL DECIMAL(18,2) NOT NULL DEFAULT 0,
        TotalRetencionL DECIMAL(18,2) NOT NULL DEFAULT 0,
        
        -- Totales Finales Convertidos
        TotalFacturaL DECIMAL(18,2) NOT NULL DEFAULT 0,
        TotalAPagarL DECIMAL(18,2) NOT NULL DEFAULT 0,
        
        CONSTRAINT PK_SolicitudesPagoMonedaLocal PRIMARY KEY CLUSTERED (SolicitudPagoID),
        CONSTRAINT FK_SolicitudesPagoML_Solicitud FOREIGN KEY (SolicitudPagoID) 
            REFERENCES dbo.SolicitudesPago(SolicitudPagoID) ON DELETE CASCADE
    );
    PRINT '✓ Tabla SolicitudesPagoMonedaLocal creada.';
END
GO

-- 3. MIGRAR DATOS EXISTENTES (Si los hay) Y ELIMINAR COLUMNAS ANTIGUAS
-- Solo se ejecuta si existen las columnas antiguas
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SolicitudesPago') AND name = 'SubtotalConvertido')
BEGIN
    -- Migración básica preventiva
    INSERT INTO dbo.SolicitudesPagoMonedaLocal (SolicitudPagoID, SubtotalL, ITBISL, TotalFacturaL, TotalAPagarL)
    SELECT SolicitudPagoID, SubtotalConvertido, ITBISConvertido, TotalFacturaConvertido, TotalAPagarConvertido
    FROM dbo.SolicitudesPago
    WHERE SubtotalConvertido IS NOT NULL OR TotalAPagarConvertido IS NOT NULL;

    -- Eliminar columnas antiguas
    ALTER TABLE dbo.SolicitudesPago DROP COLUMN SubtotalConvertido;
    ALTER TABLE dbo.SolicitudesPago DROP COLUMN ITBISConvertido;
    ALTER TABLE dbo.SolicitudesPago DROP COLUMN TotalFacturaConvertido;
    ALTER TABLE dbo.SolicitudesPago DROP COLUMN TotalAPagarConvertido;
    
    PRINT '✓ Columnas convertidas antiguas migradas y eliminadas de SolicitudesPago.';
END
GO

PRINT 'Actualización completada exitosamente.';
GO
