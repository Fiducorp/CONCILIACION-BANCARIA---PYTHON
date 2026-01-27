USE FiducorpERP;
GO

PRINT '=======================================================';
PRINT 'SCRIPT UPDATE: AGREGAR COLUMNAS DE CONVERSIÓN A SOLICITUDES PAGO';
PRINT '=======================================================';

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SolicitudesPago')
BEGIN
    -- Agregar columnas de montos convertidos si no existen
    
    -- Exento
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SolicitudesPago') AND name = 'ExentoConvertido')
    BEGIN
        ALTER TABLE dbo.SolicitudesPago ADD ExentoConvertido DECIMAL(18,2) NULL;
        PRINT '✓ Columna ExentoConvertido agregada';
    END

    -- Dirección Técnica
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SolicitudesPago') AND name = 'DireccionTecnicaConvertido')
    BEGIN
        ALTER TABLE dbo.SolicitudesPago ADD DireccionTecnicaConvertido DECIMAL(18,2) NULL;
        PRINT '✓ Columna DireccionTecnicaConvertido agregada';
    END

    -- Descuento
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SolicitudesPago') AND name = 'DescuentoConvertido')
    BEGIN
        ALTER TABLE dbo.SolicitudesPago ADD DescuentoConvertido DECIMAL(18,2) NULL;
        PRINT '✓ Columna DescuentoConvertido agregada';
    END

    -- Horas Extras
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SolicitudesPago') AND name = 'HorasExtrasConvertido')
    BEGIN
        ALTER TABLE dbo.SolicitudesPago ADD HorasExtrasConvertido DECIMAL(18,2) NULL;
        PRINT '✓ Columna HorasExtrasConvertido agregada';
    END

    -- Otros Impuestos
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SolicitudesPago') AND name = 'OtrosImpuestosConvertido')
    BEGIN
        ALTER TABLE dbo.SolicitudesPago ADD OtrosImpuestosConvertido DECIMAL(18,2) NULL;
        PRINT '✓ Columna OtrosImpuestosConvertido agregada';
    END

    -- Notas Crédito
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SolicitudesPago') AND name = 'NotaCreditoMontoConvertido')
    BEGIN
        ALTER TABLE dbo.SolicitudesPago ADD NotaCreditoMontoConvertido DECIMAL(18,2) NULL;
        PRINT '✓ Columna NotaCreditoMontoConvertido agregada';
    END

    -- Notas Débito
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SolicitudesPago') AND name = 'NotaDebitoMontoConvertido')
    BEGIN
        ALTER TABLE dbo.SolicitudesPago ADD NotaDebitoMontoConvertido DECIMAL(18,2) NULL;
        PRINT '✓ Columna NotaDebitoMontoConvertido agregada';
    END

    -- Retenciones
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SolicitudesPago') AND name = 'RetencionITBISMontoConvertido')
    BEGIN
        ALTER TABLE dbo.SolicitudesPago ADD RetencionITBISMontoConvertido DECIMAL(18,2) NULL;
        PRINT '✓ Columna RetencionITBISMontoConvertido agregada';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SolicitudesPago') AND name = 'RetencionISRMontoConvertido')
    BEGIN
        ALTER TABLE dbo.SolicitudesPago ADD RetencionISRMontoConvertido DECIMAL(18,2) NULL;
        PRINT '✓ Columna RetencionISRMontoConvertido agregada';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SolicitudesPago') AND name = 'RetencionSFSMontoConvertido')
    BEGIN
        ALTER TABLE dbo.SolicitudesPago ADD RetencionSFSMontoConvertido DECIMAL(18,2) NULL;
        PRINT '✓ Columna RetencionSFSMontoConvertido agregada';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SolicitudesPago') AND name = 'RetencionAFPMontoConvertido')
    BEGIN
        ALTER TABLE dbo.SolicitudesPago ADD RetencionAFPMontoConvertido DECIMAL(18,2) NULL;
        PRINT '✓ Columna RetencionAFPMontoConvertido agregada';
    END

    -- Total Retención
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SolicitudesPago') AND name = 'TotalRetencionConvertido')
    BEGIN
        ALTER TABLE dbo.SolicitudesPago ADD TotalRetencionConvertido DECIMAL(18,2) NULL;
        PRINT '✓ Columna TotalRetencionConvertido agregada';
    END

    PRINT '✓ ACTUALIZACIÓN COMPLETADA EXITOSAMENTE';
END
ELSE
BEGIN
    PRINT '❌ Error: La tabla SolicitudesPago no existe';
END
GO
