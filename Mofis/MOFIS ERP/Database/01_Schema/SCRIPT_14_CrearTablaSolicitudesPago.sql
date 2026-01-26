-- ============================================================================
-- SCRIPT 14: CREAR TABLA SOLICITUDES DE PAGO Y RELACIONADAS
-- ============================================================================
-- Descripción: Crea la tabla principal de Solicitudes de Pago y sus tablas
--              relacionadas para subtotales, comprobantes y avances
-- Fecha: 2026-01-17
-- Módulo: CONTABILIDAD > CUENTAS POR PAGAR
-- ============================================================================

USE FiducorpERP;
GO

PRINT '=======================================================';
PRINT 'SCRIPT 14: CREAR TABLA SOLICITUDES DE PAGO';
PRINT '=======================================================';
PRINT '';

-- ============================================================================
-- TABLA 1: SolicitudesPago (Tabla Principal)
-- ============================================================================
IF OBJECT_ID('dbo.SolicitudesPago', 'U') IS NOT NULL
BEGIN
    -- Primero eliminar tablas dependientes
    IF OBJECT_ID('dbo.SolicitudesPagoAvances', 'U') IS NOT NULL
        DROP TABLE dbo.SolicitudesPagoAvances;
    IF OBJECT_ID('dbo.SolicitudesPagoComprobantes', 'U') IS NOT NULL
        DROP TABLE dbo.SolicitudesPagoComprobantes;
    IF OBJECT_ID('dbo.SolicitudesPagoSubtotales', 'U') IS NOT NULL
        DROP TABLE dbo.SolicitudesPagoSubtotales;
    
    PRINT 'La tabla SolicitudesPago ya existe. Eliminándola...';
    DROP TABLE dbo.SolicitudesPago;
    PRINT '✓ Tabla SolicitudesPago y dependientes eliminadas';
END
GO

CREATE TABLE dbo.SolicitudesPago
(
    SolicitudPagoID INT IDENTITY(1,1) NOT NULL,
    
    -- ========================================
    -- DATOS GENERALES
    -- ========================================
    NumeroSolicitud NVARCHAR(20) NOT NULL,      -- SP-000001 (generado automáticamente)
    FechaSolicitud DATE NOT NULL,               -- Fecha de la solicitud
    FideicomisoID INT NOT NULL,                 -- FK a Fideicomisos
    ProveedorID INT NOT NULL,                   -- FK a Proveedores
    TipoPagoID INT NOT NULL,                    -- FK a TiposPago
    TipoComprobanteID INT NULL,                 -- FK a TiposComprobante (NCF, Cubicación, etc.)
    NumeroSolicitudExterno NVARCHAR(50) NULL,   -- Número de solicitud externo (opcional)
    
    -- ========================================
    -- CONCEPTO Y OBSERVACIONES
    -- ========================================
    Concepto NVARCHAR(2000) NOT NULL,           -- Descripción de la factura
    Observaciones NVARCHAR(1000) NULL,          -- Notas adicionales
    
    -- ========================================
    -- MONTOS PRINCIPALES
    -- ========================================
    SubtotalCalculado DECIMAL(18,2) NOT NULL DEFAULT 0,  -- Suma de subtotales (calculado)
    Exento DECIMAL(18,2) NOT NULL DEFAULT 0,
    DireccionTecnica DECIMAL(18,2) NOT NULL DEFAULT 0,
    Descuento DECIMAL(18,2) NOT NULL DEFAULT 0,
    HorasExtras DECIMAL(18,2) NOT NULL DEFAULT 0,
    OtrosImpuestos DECIMAL(18,2) NOT NULL DEFAULT 0,
    OtrosImpuestosDescripcion NVARCHAR(200) NULL,
    
    -- ========================================
    -- NOTA DE CRÉDITO
    -- ========================================
    NotaCreditoMonto DECIMAL(18,2) NOT NULL DEFAULT 0,
    NotaCreditoITBIS DECIMAL(18,2) NOT NULL DEFAULT 0,
    NotaCreditoDescripcion NVARCHAR(500) NULL,
    NotaCreditoManera INT NOT NULL DEFAULT 1,   -- 1=Afecta Total, 2=Afecta Subtotal
    NotaCreditoMostrarDetalle BIT NOT NULL DEFAULT 0,
    
    -- ========================================
    -- NOTA DE DÉBITO
    -- ========================================
    NotaDebitoMonto DECIMAL(18,2) NOT NULL DEFAULT 0,
    NotaDebitoITBIS DECIMAL(18,2) NOT NULL DEFAULT 0,
    NotaDebitoDescripcion NVARCHAR(500) NULL,
    NotaDebitoManera INT NOT NULL DEFAULT 1,    -- 1=Afecta Total, 2=Afecta Subtotal
    NotaDebitoMostrarDetalle BIT NOT NULL DEFAULT 0,
    
    -- ========================================
    -- ITBIS
    -- ========================================
    ITBISPorcentaje DECIMAL(5,2) NOT NULL DEFAULT 18.00,  -- 16, 18, u otro
    ITBISBase CHAR(1) NOT NULL DEFAULT 'S',     -- S=Subtotal, D=Dirección Técnica
    ITBISCalculado DECIMAL(18,2) NOT NULL DEFAULT 0,
    ITBISIngresado DECIMAL(18,2) NULL,          -- Si se ingresó manualmente
    ITBISUsarIngresado BIT NOT NULL DEFAULT 0,  -- Si usar el ingresado en vez del calculado
    
    -- ========================================
    -- RETENCIONES
    -- ========================================
    RetencionITBISPorcentaje DECIMAL(5,2) NOT NULL DEFAULT 0,  -- 30 o 100
    RetencionITBISMonto DECIMAL(18,2) NOT NULL DEFAULT 0,
    RetencionISRPorcentaje DECIMAL(5,2) NOT NULL DEFAULT 0,    -- 2, 10 o 27
    RetencionISRMonto DECIMAL(18,2) NOT NULL DEFAULT 0,
    RetencionSFSMonto DECIMAL(18,2) NOT NULL DEFAULT 0,
    RetencionAFPMonto DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    -- ========================================
    -- ANTICIPOS Y AVANCES
    -- ========================================
    Anticipo DECIMAL(18,2) NOT NULL DEFAULT 0,              -- Monto ya pagado (resta del total)
    AvanceParaPagar DECIMAL(18,2) NOT NULL DEFAULT 0,       -- Monto que se pagará (informativo)
    TieneAvancePrevio BIT NOT NULL DEFAULT 0,               -- Si esta solicitud tiene avance previo
    SolicitudPagoOrigenID INT NULL,                         -- FK a la solicitud original (si es continuación)
    
    -- ========================================
    -- TOTALES CALCULADOS
    -- ========================================
    TotalFactura DECIMAL(18,2) NOT NULL DEFAULT 0,          -- Subtotal + ITBIS + Exento
    TotalRetencion DECIMAL(18,2) NOT NULL DEFAULT 0,        -- Suma de retenciones
    TotalDescuento DECIMAL(18,2) NOT NULL DEFAULT 0,        -- Descuento + Nota Crédito (según manera)
    TotalAPagar DECIMAL(18,2) NOT NULL DEFAULT 0,           -- Total final a pagar
    
    -- ========================================
    -- MONEDA Y CONVERSIÓN
    -- ========================================
    MonedaID INT NOT NULL,                      -- FK a Monedas (default DOP)
    TasaCambio DECIMAL(18,6) NULL,              -- Solo si moneda ≠ DOP
    MetodoConversionID INT NULL,                -- FK a MetodosConversion
    MostrarConversionEnFormulario BIT NOT NULL DEFAULT 0,
    
    -- Montos convertidos (para impresión/exportación)
    SubtotalConvertido DECIMAL(18,2) NULL,
    ITBISConvertido DECIMAL(18,2) NULL,
    TotalFacturaConvertido DECIMAL(18,2) NULL,
    TotalAPagarConvertido DECIMAL(18,2) NULL,
    
    -- ========================================
    -- FIRMA DIGITAL
    -- ========================================
    IncluirFirma BIT NOT NULL DEFAULT 0,
    FirmaUsuarioID INT NULL,                    -- FK a FirmasUsuarios
    
    -- ========================================
    -- ESTADO Y CONTROL
    -- ========================================
    Estado NVARCHAR(20) NOT NULL DEFAULT 'BORRADOR',  -- BORRADOR, GUARDADO, IMPRESO, PAGADO, ANULADO
    FechaImpresion DATETIME NULL,
    VecesImpreso INT NOT NULL DEFAULT 0,
    FechaUltimaExportacion DATETIME NULL,
    
    -- ========================================
    -- PROPIEDAD (para permisos)
    -- ========================================
    UsuarioPropietarioID INT NOT NULL,          -- FK a Usuarios (quien creó)
    
    -- ========================================
    -- AUDITORÍA
    -- ========================================
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    CreadoPorUsuarioID INT NOT NULL,
    FechaModificacion DATETIME NULL,
    ModificadoPorUsuarioID INT NULL,
    
    -- Eliminación lógica
    EsEliminado BIT NOT NULL DEFAULT 0,
    FechaEliminacion DATETIME NULL,
    EliminadoPorUsuarioID INT NULL,
    
    -- ========================================
    -- CONSTRAINTS
    -- ========================================
    CONSTRAINT PK_SolicitudesPago PRIMARY KEY CLUSTERED (SolicitudPagoID),
    CONSTRAINT UQ_SolicitudesPago_Numero UNIQUE (NumeroSolicitud),
    CONSTRAINT FK_SolicitudesPago_Fideicomiso FOREIGN KEY (FideicomisoID) 
        REFERENCES dbo.Fideicomisos(FideicomisoID),
    CONSTRAINT FK_SolicitudesPago_Proveedor FOREIGN KEY (ProveedorID) 
        REFERENCES dbo.Proveedores(ProveedorID),
    CONSTRAINT FK_SolicitudesPago_TipoPago FOREIGN KEY (TipoPagoID) 
        REFERENCES dbo.TiposPago(TipoPagoID),
    CONSTRAINT FK_SolicitudesPago_TipoComprobante FOREIGN KEY (TipoComprobanteID) 
        REFERENCES dbo.TiposComprobante(TipoComprobanteID),
    CONSTRAINT FK_SolicitudesPago_Moneda FOREIGN KEY (MonedaID) 
        REFERENCES dbo.Monedas(MonedaID),
    CONSTRAINT FK_SolicitudesPago_MetodoConversion FOREIGN KEY (MetodoConversionID) 
        REFERENCES dbo.MetodosConversion(MetodoConversionID),
    CONSTRAINT FK_SolicitudesPago_Origen FOREIGN KEY (SolicitudPagoOrigenID) 
        REFERENCES dbo.SolicitudesPago(SolicitudPagoID),
    CONSTRAINT FK_SolicitudesPago_Propietario FOREIGN KEY (UsuarioPropietarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT FK_SolicitudesPago_CreadoPor FOREIGN KEY (CreadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT FK_SolicitudesPago_ModificadoPor FOREIGN KEY (ModificadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT FK_SolicitudesPago_EliminadoPor FOREIGN KEY (EliminadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT CK_SolicitudesPago_ITBISBase CHECK (ITBISBase IN ('S', 'D')),
    CONSTRAINT CK_SolicitudesPago_NotaCreditoManera CHECK (NotaCreditoManera IN (1, 2)),
    CONSTRAINT CK_SolicitudesPago_NotaDebitoManera CHECK (NotaDebitoManera IN (1, 2)),
    CONSTRAINT CK_SolicitudesPago_Estado CHECK (Estado IN ('BORRADOR', 'GUARDADO', 'IMPRESO', 'PAGADO', 'ANULADO'))
);
GO

-- Índices para búsquedas frecuentes
CREATE NONCLUSTERED INDEX IX_SolicitudesPago_Numero ON dbo.SolicitudesPago(NumeroSolicitud);
CREATE NONCLUSTERED INDEX IX_SolicitudesPago_Fecha ON dbo.SolicitudesPago(FechaSolicitud DESC);
CREATE NONCLUSTERED INDEX IX_SolicitudesPago_Fideicomiso ON dbo.SolicitudesPago(FideicomisoID);
CREATE NONCLUSTERED INDEX IX_SolicitudesPago_Proveedor ON dbo.SolicitudesPago(ProveedorID);
CREATE NONCLUSTERED INDEX IX_SolicitudesPago_Estado ON dbo.SolicitudesPago(Estado);
CREATE NONCLUSTERED INDEX IX_SolicitudesPago_Propietario ON dbo.SolicitudesPago(UsuarioPropietarioID);
CREATE NONCLUSTERED INDEX IX_SolicitudesPago_Activo ON dbo.SolicitudesPago(EsEliminado) WHERE EsEliminado = 0;

-- Índice compuesto para consultas de listado
CREATE NONCLUSTERED INDEX IX_SolicitudesPago_Listado 
ON dbo.SolicitudesPago(EsEliminado, FechaSolicitud DESC, Estado)
INCLUDE (NumeroSolicitud, FideicomisoID, ProveedorID, TotalAPagar);
GO

PRINT '✓ Tabla SolicitudesPago creada exitosamente';
GO

-- ============================================================================
-- TABLA 2: SolicitudesPagoSubtotales (Múltiples subtotales por solicitud)
-- ============================================================================
CREATE TABLE dbo.SolicitudesPagoSubtotales
(
    SubtotalID INT IDENTITY(1,1) NOT NULL,
    SolicitudPagoID INT NOT NULL,               -- FK a SolicitudesPago
    Orden INT NOT NULL DEFAULT 1,               -- Orden de visualización
    Monto DECIMAL(18,2) NOT NULL,               -- Monto del subtotal
    Cantidad INT NOT NULL DEFAULT 1,            -- Veces que se repite
    SubtotalLinea DECIMAL(18,2) NOT NULL,       -- Monto × Cantidad (calculado)
    
    CONSTRAINT PK_SolicitudesPagoSubtotales PRIMARY KEY CLUSTERED (SubtotalID),
    CONSTRAINT FK_SolicitudesPagoSubtotales_Solicitud FOREIGN KEY (SolicitudPagoID) 
        REFERENCES dbo.SolicitudesPago(SolicitudPagoID) ON DELETE CASCADE,
    CONSTRAINT CK_SolicitudesPagoSubtotales_Monto CHECK (Monto > 0),
    CONSTRAINT CK_SolicitudesPagoSubtotales_Cantidad CHECK (Cantidad >= 1)
);
GO

CREATE NONCLUSTERED INDEX IX_SolicitudesPagoSubtotales_Solicitud 
ON dbo.SolicitudesPagoSubtotales(SolicitudPagoID, Orden);
GO

PRINT '✓ Tabla SolicitudesPagoSubtotales creada exitosamente';
GO

-- ============================================================================
-- TABLA 3: SolicitudesPagoComprobantes (Múltiples NCF por solicitud)
-- ============================================================================
CREATE TABLE dbo.SolicitudesPagoComprobantes
(
    ComprobanteID INT IDENTITY(1,1) NOT NULL,
    SolicitudPagoID INT NOT NULL,               -- FK a SolicitudesPago
    Orden INT NOT NULL DEFAULT 1,               -- Orden de visualización
    TipoNCFID INT NOT NULL,                     -- FK a TiposNCF
    NumeroComprobante NVARCHAR(13) NOT NULL,    -- Número completo (ej: B0100000306)
    
    CONSTRAINT PK_SolicitudesPagoComprobantes PRIMARY KEY CLUSTERED (ComprobanteID),
    CONSTRAINT FK_SolicitudesPagoComprobantes_Solicitud FOREIGN KEY (SolicitudPagoID) 
        REFERENCES dbo.SolicitudesPago(SolicitudPagoID) ON DELETE CASCADE,
    CONSTRAINT FK_SolicitudesPagoComprobantes_TipoNCF FOREIGN KEY (TipoNCFID) 
        REFERENCES dbo.TiposNCF(TipoNCFID),
    CONSTRAINT UQ_SolicitudesPagoComprobantes_Numero UNIQUE (NumeroComprobante)  -- NCF no puede repetirse
);
GO

CREATE NONCLUSTERED INDEX IX_SolicitudesPagoComprobantes_Solicitud 
ON dbo.SolicitudesPagoComprobantes(SolicitudPagoID, Orden);

CREATE NONCLUSTERED INDEX IX_SolicitudesPagoComprobantes_Numero 
ON dbo.SolicitudesPagoComprobantes(NumeroComprobante);
GO

PRINT '✓ Tabla SolicitudesPagoComprobantes creada exitosamente';
GO

-- ============================================================================
-- TABLA 4: SolicitudesPagoAvances (Historial de avances)
-- ============================================================================
CREATE TABLE dbo.SolicitudesPagoAvances
(
    AvanceID INT IDENTITY(1,1) NOT NULL,
    SolicitudPagoID INT NOT NULL,               -- FK a SolicitudesPago
    FechaAvance DATETIME NOT NULL DEFAULT GETDATE(),
    MontoAvance DECIMAL(18,2) NOT NULL,         -- Monto avanzado
    MontoPendiente DECIMAL(18,2) NOT NULL,      -- Lo que quedó pendiente después del avance
    Observacion NVARCHAR(500) NULL,
    
    -- Referencia a la solicitud continuación (si existe)
    SolicitudContinuacionID INT NULL,           -- FK a SolicitudesPago (la que continúa)
    
    -- Auditoría
    CreadoPorUsuarioID INT NOT NULL,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT PK_SolicitudesPagoAvances PRIMARY KEY CLUSTERED (AvanceID),
    CONSTRAINT FK_SolicitudesPagoAvances_Solicitud FOREIGN KEY (SolicitudPagoID) 
        REFERENCES dbo.SolicitudesPago(SolicitudPagoID),
    CONSTRAINT FK_SolicitudesPagoAvances_Continuacion FOREIGN KEY (SolicitudContinuacionID) 
        REFERENCES dbo.SolicitudesPago(SolicitudPagoID),
    CONSTRAINT FK_SolicitudesPagoAvances_CreadoPor FOREIGN KEY (CreadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID)
);
GO

CREATE NONCLUSTERED INDEX IX_SolicitudesPagoAvances_Solicitud 
ON dbo.SolicitudesPagoAvances(SolicitudPagoID, FechaAvance DESC);
GO

PRINT '✓ Tabla SolicitudesPagoAvances creada exitosamente';
GO

-- ============================================================================
-- SECUENCIA PARA NÚMERO DE SOLICITUD
-- ============================================================================
IF EXISTS (SELECT 1 FROM sys.sequences WHERE name = 'SEQ_SolicitudPago')
BEGIN
    DROP SEQUENCE dbo.SEQ_SolicitudPago;
END
GO

CREATE SEQUENCE dbo.SEQ_SolicitudPago
    AS INT
    START WITH 1
    INCREMENT BY 1
    MINVALUE 1
    NO MAXVALUE
    NO CYCLE
    CACHE 10;
GO

PRINT '✓ Secuencia SEQ_SolicitudPago creada exitosamente';
GO

-- ============================================================================
-- FUNCIÓN PARA GENERAR NÚMERO DE SOLICITUD
-- ============================================================================
IF OBJECT_ID('dbo.fn_GenerarNumeroSolicitudPago', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_GenerarNumeroSolicitudPago;
GO

CREATE FUNCTION dbo.fn_GenerarNumeroSolicitudPago()
RETURNS NVARCHAR(20)
AS
BEGIN
    DECLARE @Numero INT;
    DECLARE @NumeroFormateado NVARCHAR(20);
    
    -- Obtener el siguiente valor de la secuencia
    -- Nota: NEXT VALUE FOR no se puede usar en funciones, 
    -- se debe llamar antes y pasar como parámetro
    SET @Numero = (SELECT MAX(SolicitudPagoID) FROM dbo.SolicitudesPago) + 1;
    IF @Numero IS NULL SET @Numero = 1;
    
    SET @NumeroFormateado = 'SP-' + RIGHT('000000' + CAST(@Numero AS NVARCHAR(6)), 6);
    
    RETURN @NumeroFormateado;
END
GO

PRINT '✓ Función fn_GenerarNumeroSolicitudPago creada exitosamente';
GO

-- ============================================================================
-- RESUMEN
-- ============================================================================
PRINT '';
PRINT '=======================================================';
PRINT '✓ SCRIPT 14 COMPLETADO EXITOSAMENTE';
PRINT '=======================================================';
PRINT 'Tablas creadas:';
PRINT '';
PRINT '  1. SolicitudesPago (Tabla principal)';
PRINT '     - 70+ columnas para todos los campos requeridos';
PRINT '     - Soporte para Notas Crédito/Débito (2 maneras)';
PRINT '     - Conversión multi-moneda';
PRINT '     - Control de propiedad para permisos';
PRINT '     - Auditoría completa';
PRINT '';
PRINT '  2. SolicitudesPagoSubtotales';
PRINT '     - Múltiples subtotales por solicitud';
PRINT '     - Soporte para repeticiones (Monto × Cantidad)';
PRINT '';
PRINT '  3. SolicitudesPagoComprobantes';
PRINT '     - Múltiples NCF por solicitud';
PRINT '     - Validación de NCF único (no se puede repetir)';
PRINT '';
PRINT '  4. SolicitudesPagoAvances';
PRINT '     - Historial de avances realizados';
PRINT '     - Tracking de continuaciones';
PRINT '';
PRINT '  5. SEQ_SolicitudPago (Secuencia)';
PRINT '     - Generación automática de números SP-000001';
PRINT '';
PRINT 'Siguiente paso: Ejecutar Script 15 - Crear tabla FirmasUsuarios';
PRINT '=======================================================';
GO
