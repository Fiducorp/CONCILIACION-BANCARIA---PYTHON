-- ============================================================================
-- SCRIPT 08: INSERTAR DATOS DE CATÁLOGOS CXP
-- ============================================================================
-- Descripción: Inserta los datos iniciales de los catálogos del módulo CXP
--              - TiposNCF (Comprobantes fiscales DGII)
--              - Monedas (ISO 4217)
--              - TiposPago
--              - TiposComprobante
--              - TiposFideicomiso
--              - MetodosConversion
-- Fecha: 2026-01-17
-- Módulo: CONTABILIDAD > CUENTAS POR PAGAR
-- ============================================================================

USE FiducorpERP;
GO

PRINT '=======================================================';
PRINT 'SCRIPT 08: INSERTAR DATOS DE CATÁLOGOS CXP';
PRINT '=======================================================';
PRINT '';

-- ============================================================================
-- 1. TIPOS NCF (Comprobantes Fiscales DGII República Dominicana)
-- ============================================================================
PRINT 'Insertando TiposNCF...';

-- Limpiar tabla si tiene datos
IF EXISTS (SELECT 1 FROM dbo.TiposNCF)
BEGIN
    DELETE FROM dbo.TiposNCF;
    DBCC CHECKIDENT ('dbo.TiposNCF', RESEED, 0);
END

-- NCF Tradicionales (Serie B) - 11 caracteres
INSERT INTO dbo.TiposNCF (Codigo, CodigoNumerico, Serie, Nombre, NombreCorto, Descripcion, EsElectronico, LongitudSecuencia, LongitudTotal, RequiereRNC, PermiteCredito, OrdenVisualizacion, Activo)
VALUES 
    ('B01', '01', 'B', 'Factura de Crédito Fiscal', 'Crédito Fiscal', 
     'Transacciones entre contribuyentes, permite sustentar gastos/costos y crédito fiscal', 
     0, 8, 11, 1, 1, 1, 1),
    
    ('B02', '02', 'B', 'Factura de Consumo', 'Consumo Final', 
     'Ventas a consumidores finales, no sustenta gastos', 
     0, 8, 11, 0, 0, 2, 1),
    
    ('B03', '03', 'B', 'Nota de Débito', 'Nota Débito', 
     'Aumentar valor de factura (intereses mora, fletes, etc.)', 
     0, 8, 11, 1, 1, 3, 1),
    
    ('B04', '04', 'B', 'Nota de Crédito', 'Nota Crédito', 
     'Reducir valor de factura (descuentos, devoluciones, anulaciones)', 
     0, 8, 11, 1, 1, 4, 1),
    
    ('B11', '11', 'B', 'Comprobante de Compras', 'Compras', 
     'Compras a personas no registradas como contribuyentes (proveedores informales)', 
     0, 8, 11, 0, 1, 5, 1),
    
    ('B12', '12', 'B', 'Registro Único de Ingresos', 'Único Ingresos', 
     'Resumen de ventas diarias (colmados, salones, etc.)', 
     0, 8, 11, 0, 0, 6, 1),
    
    ('B13', '13', 'B', 'Comprobante para Gastos Menores', 'Gastos Menores', 
     'Peajes, estacionamientos, consumibles, transporte', 
     0, 8, 11, 0, 1, 7, 1),
    
    ('B14', '14', 'B', 'Comprobante para Regímenes Especiales', 'Régimen Especial', 
     'Ventas exentas ITBIS/ISC a zonas francas, etc.', 
     0, 8, 11, 1, 0, 8, 1),
    
    ('B15', '15', 'B', 'Comprobante Gubernamental', 'Gubernamental', 
     'Ventas al Gobierno, instituciones estatales', 
     0, 8, 11, 1, 1, 9, 1),
    
    ('B16', '16', 'B', 'Comprobante para Exportaciones', 'Exportaciones', 
     'Ventas de bienes al exterior', 
     0, 8, 11, 0, 0, 10, 1),
    
    ('B17', '17', 'B', 'Comprobante para Pagos al Exterior', 'Pagos Exterior', 
     'Pagos a no residentes fiscales (retención 27% ISR)', 
     0, 8, 11, 0, 0, 11, 1);

-- NCF Electrónicos (Serie E) - 13 caracteres
INSERT INTO dbo.TiposNCF (Codigo, CodigoNumerico, Serie, Nombre, NombreCorto, Descripcion, EsElectronico, LongitudSecuencia, LongitudTotal, RequiereRNC, PermiteCredito, OrdenVisualizacion, Activo)
VALUES 
    ('E31', '31', 'E', 'Factura de Crédito Fiscal Electrónica', 'e-Crédito Fiscal', 
     'Versión electrónica de B01', 
     1, 10, 13, 1, 1, 12, 1),
    
    ('E32', '32', 'E', 'Factura de Consumo Electrónica', 'e-Consumo Final', 
     'Versión electrónica de B02', 
     1, 10, 13, 0, 0, 13, 1),
    
    ('E33', '33', 'E', 'Nota de Débito Electrónica', 'e-Nota Débito', 
     'Versión electrónica de B03', 
     1, 10, 13, 1, 1, 14, 1),
    
    ('E34', '34', 'E', 'Nota de Crédito Electrónica', 'e-Nota Crédito', 
     'Versión electrónica de B04', 
     1, 10, 13, 1, 1, 15, 1),
    
    ('E41', '41', 'E', 'Comprobante Electrónico de Compras', 'e-Compras', 
     'Versión electrónica de B11', 
     1, 10, 13, 0, 1, 16, 1),
    
    ('E42', '42', 'E', 'Comprobante Electrónico de Registro Único de Ingresos', 'e-Único Ingresos', 
     'Versión electrónica de B12', 
     1, 10, 13, 0, 0, 17, 1),
    
    ('E43', '43', 'E', 'Comprobante Electrónico para Gastos Menores', 'e-Gastos Menores', 
     'Versión electrónica de B13', 
     1, 10, 13, 0, 1, 18, 1),
    
    ('E44', '44', 'E', 'Comprobante Electrónico para Regímenes Especiales', 'e-Régimen Especial', 
     'Versión electrónica de B14', 
     1, 10, 13, 1, 0, 19, 1),
    
    ('E45', '45', 'E', 'Comprobante Electrónico Gubernamental', 'e-Gubernamental', 
     'Versión electrónica de B15', 
     1, 10, 13, 1, 1, 20, 1),
    
    ('E46', '46', 'E', 'Comprobante Electrónico para Exportaciones', 'e-Exportaciones', 
     'Versión electrónica de B16', 
     1, 10, 13, 0, 0, 21, 1),
    
    ('E47', '47', 'E', 'Comprobante Electrónico para Pagos al Exterior', 'e-Pagos Exterior', 
     'Versión electrónica de B17', 
     1, 10, 13, 0, 0, 22, 1);

PRINT '✓ TiposNCF insertados: ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' registros';
GO

-- ============================================================================
-- 2. MONEDAS (ISO 4217)
-- ============================================================================
PRINT 'Insertando Monedas...';

IF EXISTS (SELECT 1 FROM dbo.Monedas)
BEGIN
    DELETE FROM dbo.Monedas;
    DBCC CHECKIDENT ('dbo.Monedas', RESEED, 0);
END

INSERT INTO dbo.Monedas (CodigoISO, Simbolo, Nombre, NombreIngles, Pais, DecimalesMoneda, FormatoDisplay, EsLocal, OrdenVisualizacion, Activo)
VALUES 
    ('DOP', 'RD$', 'Peso Dominicano', 'Dominican Peso', 'República Dominicana', 2, '#,##0.00', 1, 1, 1),
    ('USD', 'US$', 'Dólar Estadounidense', 'US Dollar', 'Estados Unidos', 2, '#,##0.00', 0, 2, 1),
    ('EUR', '€', 'Euro', 'Euro', 'Unión Europea', 2, '#,##0.00', 0, 3, 1),
    ('GBP', '£', 'Libra Esterlina', 'British Pound', 'Reino Unido', 2, '#,##0.00', 0, 4, 1),
    ('CAD', 'CA$', 'Dólar Canadiense', 'Canadian Dollar', 'Canadá', 2, '#,##0.00', 0, 5, 1),
    ('MXN', 'MX$', 'Peso Mexicano', 'Mexican Peso', 'México', 2, '#,##0.00', 0, 6, 1),
    ('COP', 'CO$', 'Peso Colombiano', 'Colombian Peso', 'Colombia', 2, '#,##0.00', 0, 7, 1),
    ('BRL', 'R$', 'Real Brasileño', 'Brazilian Real', 'Brasil', 2, '#,##0.00', 0, 8, 1),
    ('CHF', 'CHF', 'Franco Suizo', 'Swiss Franc', 'Suiza', 2, '#,##0.00', 0, 9, 1),
    ('JPY', '¥', 'Yen Japonés', 'Japanese Yen', 'Japón', 0, '#,##0', 0, 10, 1),
    ('CNY', '¥', 'Yuan Chino', 'Chinese Yuan', 'China', 2, '#,##0.00', 0, 11, 1),
    ('ARS', 'AR$', 'Peso Argentino', 'Argentine Peso', 'Argentina', 2, '#,##0.00', 0, 12, 1);

PRINT '✓ Monedas insertadas: ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' registros';
GO

-- ============================================================================
-- 3. TIPOS DE PAGO
-- ============================================================================
PRINT 'Insertando TiposPago...';

IF EXISTS (SELECT 1 FROM dbo.TiposPago)
BEGIN
    DELETE FROM dbo.TiposPago;
    DBCC CHECKIDENT ('dbo.TiposPago', RESEED, 0);
END

INSERT INTO dbo.TiposPago (Codigo, Nombre, Descripcion, RequiereCuenta, RequiereReferencia, OrdenVisualizacion, Activo)
VALUES 
    ('TRF', 'Transferencia', 'Transferencia bancaria', 1, 1, 1, 1),
    ('CHQ', 'Cheque', 'Pago mediante cheque bancario', 1, 1, 2, 1),
    ('EFE', 'Efectivo', 'Pago en efectivo', 0, 0, 3, 1),
    ('TDC', 'Tarjeta de Crédito', 'Pago con tarjeta de crédito', 0, 1, 4, 1),
    ('TDD', 'Tarjeta de Débito', 'Pago con tarjeta de débito', 0, 1, 5, 1),
    ('OTR', 'Otro', 'Otro método de pago', 0, 0, 6, 1);

PRINT '✓ TiposPago insertados: ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' registros';
GO

-- ============================================================================
-- 4. TIPOS DE COMPROBANTE (No NCF)
-- ============================================================================
PRINT 'Insertando TiposComprobante...';

IF EXISTS (SELECT 1 FROM dbo.TiposComprobante)
BEGIN
    DELETE FROM dbo.TiposComprobante;
    DBCC CHECKIDENT ('dbo.TiposComprobante', RESEED, 0);
END

INSERT INTO dbo.TiposComprobante (Codigo, Nombre, Descripcion, RequiereNCF, OrdenVisualizacion, Activo)
VALUES 
    ('NCF', 'NCF', 'Número de Comprobante Fiscal (DGII)', 1, 1, 1),
    ('CUB', 'Cubicación', 'Documento de cubicación de obra', 0, 2, 1),
    ('COT', 'Cotización', 'Cotización de servicios o productos', 0, 3, 1),
    ('FAC', 'Factura Simple', 'Factura sin NCF', 0, 4, 1),
    ('REC', 'Recibo', 'Recibo de pago', 0, 5, 1),
    ('CON', 'Contrato', 'Pago según contrato', 0, 6, 1),
    ('OTR', 'Otro', 'Otro tipo de documento', 0, 7, 1);

PRINT '✓ TiposComprobante insertados: ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' registros';
GO

-- ============================================================================
-- 5. TIPOS DE FIDEICOMISO
-- ============================================================================
PRINT 'Insertando TiposFideicomiso...';

IF EXISTS (SELECT 1 FROM dbo.TiposFideicomiso)
BEGIN
    DELETE FROM dbo.TiposFideicomiso;
    DBCC CHECKIDENT ('dbo.TiposFideicomiso', RESEED, 0);
END

INSERT INTO dbo.TiposFideicomiso (Codigo, Nombre, Descripcion, OrdenVisualizacion, Activo)
VALUES 
    ('INMOB', 'Inmobiliario y Garantía', 'Fideicomiso de desarrollo inmobiliario y garantía', 1, 1),
    ('ADMIN', 'De Administración y Pago', 'Fideicomiso de administración de recursos y pagos', 2, 1),
    ('BAJO', 'De Bajo Costo', 'Fideicomiso de viviendas de bajo costo', 3, 1),
    ('RESID', 'Residencial', 'Fideicomiso de proyectos residenciales', 4, 1),
    ('PLAZA', 'Plaza Comercial', 'Fideicomiso de plazas y centros comerciales', 5, 1),
    ('OTRO', 'Otro', 'Otro tipo de fideicomiso', 6, 1);

PRINT '✓ TiposFideicomiso insertados: ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' registros';
GO

-- ============================================================================
-- 6. MÉTODOS DE CONVERSIÓN
-- ============================================================================
PRINT 'Insertando MetodosConversion...';

IF EXISTS (SELECT 1 FROM dbo.MetodosConversion)
BEGIN
    DELETE FROM dbo.MetodosConversion;
    DBCC CHECKIDENT ('dbo.MetodosConversion', RESEED, 0);
END

INSERT INTO dbo.MetodosConversion (Codigo, Nombre, Descripcion, OrdenVisualizacion, Activo)
VALUES 
    ('DIRECTO', 'Conversión Directa Total', 
     'Todos los montos se multiplican directamente por la tasa de cambio', 1, 1),
    
    ('BASE', 'Conversión Base + Recálculo', 
     'Solo el subtotal se convierte, ITBIS y retenciones se recalculan del subtotal convertido', 2, 1),
    
    ('SELECT', 'Conversión Selectiva (Personalizada)', 
     'El usuario elige qué campos convertir directamente y cuáles recalcular', 3, 1),
    
    ('INDIV', 'Conversión Individual de Subtotales', 
     'Cada subtotal se convierte individualmente antes de sumar (o suma y luego convierte)', 4, 1),
    
    ('MANUAL', 'Conversión Manual/Mixta', 
     'El usuario ingresa valores convertidos manualmente, sistema detecta diferencias', 5, 1);

PRINT '✓ MetodosConversion insertados: ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' registros';
GO

-- ============================================================================
-- VERIFICACIÓN FINAL
-- ============================================================================
PRINT '';
PRINT '=======================================================';
PRINT '✓ SCRIPT 08 COMPLETADO EXITOSAMENTE';
PRINT '=======================================================';
PRINT 'Resumen de datos insertados:';
PRINT '';

SELECT 'TiposNCF' AS Tabla, COUNT(*) AS Registros FROM dbo.TiposNCF
UNION ALL
SELECT 'Monedas', COUNT(*) FROM dbo.Monedas
UNION ALL
SELECT 'TiposPago', COUNT(*) FROM dbo.TiposPago
UNION ALL
SELECT 'TiposComprobante', COUNT(*) FROM dbo.TiposComprobante
UNION ALL
SELECT 'TiposFideicomiso', COUNT(*) FROM dbo.TiposFideicomiso
UNION ALL
SELECT 'MetodosConversion', COUNT(*) FROM dbo.MetodosConversion;

PRINT '';
PRINT 'Siguiente paso: Ejecutar Script 09 - Insertar configuración CXP';
PRINT '=======================================================';
GO
