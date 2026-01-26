-- ============================================================================
-- SCRIPT 09: INSERTAR CONFIGURACIÓN INICIAL CXP
-- ============================================================================
-- Descripción: Inserta la configuración inicial del módulo Cuentas por Pagar
--              Incluye límites, formatos, valores por defecto
-- Fecha: 2026-01-17
-- Módulo: CONTABILIDAD > CUENTAS POR PAGAR
-- ============================================================================

USE FiducorpERP;
GO

PRINT '=======================================================';
PRINT 'SCRIPT 09: INSERTAR CONFIGURACIÓN INICIAL CXP';
PRINT '=======================================================';
PRINT '';

-- ============================================================================
-- INSERTAR CONFIGURACIONES
-- ============================================================================
PRINT 'Insertando ConfiguracionModuloCXP...';

IF EXISTS (SELECT 1 FROM dbo.ConfiguracionModuloCXP)
BEGIN
    DELETE FROM dbo.ConfiguracionModuloCXP;
    DBCC CHECKIDENT ('dbo.ConfiguracionModuloCXP', RESEED, 0);
END

INSERT INTO dbo.ConfiguracionModuloCXP (Clave, Valor, TipoDato, Descripcion, Categoria, EsSistema)
VALUES 
    -- ========================================
    -- LÍMITES
    -- ========================================
    ('LIMITE_SUBTOTALES', '10', 'INT', 
     'Máximo de subtotales por solicitud de pago (1-100)', 'LIMITES', 0),
    
    ('LIMITE_COMPROBANTES', '10', 'INT', 
     'Máximo de comprobantes NCF por solicitud de pago', 'LIMITES', 0),
    
    ('LIMITE_CARACTERES_CONCEPTO', '2000', 'INT', 
     'Máximo de caracteres en el campo Concepto', 'LIMITES', 1),
    
    ('LIMITE_CARACTERES_OBSERVACIONES', '1000', 'INT', 
     'Máximo de caracteres en el campo Observaciones', 'LIMITES', 1),
    
    -- ========================================
    -- VALORES POR DEFECTO
    -- ========================================
    ('ITBIS_DEFAULT', '18', 'DECIMAL', 
     'Porcentaje ITBIS por defecto', 'DEFAULTS', 0),
    
    ('MONEDA_DEFAULT', 'DOP', 'STRING', 
     'Código ISO de moneda por defecto', 'DEFAULTS', 0),
    
    ('TIPO_PAGO_DEFAULT', 'TRF', 'STRING', 
     'Código de tipo de pago por defecto (Transferencia)', 'DEFAULTS', 0),
    
    ('METODO_CONVERSION_DEFAULT', 'DIRECTO', 'STRING', 
     'Método de conversión de moneda por defecto', 'DEFAULTS', 0),
    
    ('ITBIS_BASE_DEFAULT', 'S', 'STRING', 
     'Base para cálculo de ITBIS: S=Subtotal, D=Dirección Técnica', 'DEFAULTS', 0),
    
    -- ========================================
    -- FORMATOS
    -- ========================================
    ('FORMATO_FECHA', 'dd/MM/yyyy', 'STRING', 
     'Formato de fecha para visualización', 'FORMATOS', 0),
    
    ('DECIMALES_MONEDA', '2', 'INT', 
     'Cantidad de decimales para montos', 'FORMATOS', 1),
    
    ('DECIMALES_TASA', '6', 'INT', 
     'Cantidad de decimales para tasa de cambio', 'FORMATOS', 1),
    
    ('FORMATO_NUMERO_SOLICITUD', 'SP-{0:D6}', 'STRING', 
     'Formato para número de solicitud (SP-000001)', 'FORMATOS', 1),
    
    ('SEPARADOR_MILES', ',', 'STRING', 
     'Separador de miles para montos', 'FORMATOS', 0),
    
    ('SEPARADOR_DECIMALES', '.', 'STRING', 
     'Separador de decimales para montos', 'FORMATOS', 0),
    
    -- ========================================
    -- IMPRESIÓN
    -- ========================================
    ('IMPRIMIR_CON_LOGO', '1', 'BOOL', 
     'Incluir logo de la empresa en impresiones por defecto', 'IMPRESION', 0),
    
    ('RUTA_LOGO_EMPRESA', 'Resources\MOFIS ERP -LOGO.png', 'STRING', 
     'Ruta relativa al logo de la empresa', 'IMPRESION', 0),
    
    ('TAMANO_FUENTE_TITULO', '16', 'INT', 
     'Tamaño de fuente para títulos en impresión', 'IMPRESION', 0),
    
    ('TAMANO_FUENTE_CONTENIDO', '10', 'INT', 
     'Tamaño de fuente para contenido en impresión', 'IMPRESION', 0),
    
    ('TAMANO_FUENTE_TOTALES', '12', 'INT', 
     'Tamaño de fuente para totales en impresión', 'IMPRESION', 0),
    
    -- ========================================
    -- CONVERSIÓN DE MONEDA
    -- ========================================
    ('MOSTRAR_CONVERSION_FORMULARIO', '0', 'BOOL', 
     'Mostrar conversión en tiempo real en el formulario', 'CONVERSION', 0),
    
    ('CONVERSION_SUBTOTALES_INDIVIDUAL', '0', 'BOOL', 
     'En Método 4: Convertir subtotales individualmente (1) o sumar y convertir (0)', 'CONVERSION', 0),
    
    -- ========================================
    -- VALIDACIONES
    -- ========================================
    ('VALIDAR_NCF_DUPLICADO', '1', 'BOOL', 
     'Validar que el NCF no exista en otra solicitud', 'VALIDACIONES', 1),
    
    ('VALIDAR_RNC_DGII', '0', 'BOOL', 
     'Validar RNC contra servicio web de DGII (requiere conexión)', 'VALIDACIONES', 0),
    
    ('PERMITIR_FECHA_FUTURA', '0', 'BOOL', 
     'Permitir fechas de solicitud futuras', 'VALIDACIONES', 0),
    
    -- ========================================
    -- MEMORIA TEMPORAL
    -- ========================================
    ('HABILITAR_GUARDADO_MEMORIA', '1', 'BOOL', 
     'Habilitar guardado en memoria al salir sin guardar', 'MEMORIA', 0),
    
    ('PREGUNTAR_RESTAURAR_MEMORIA', '1', 'BOOL', 
     'Preguntar si desea restaurar datos de memoria al entrar', 'MEMORIA', 0),
    
    -- ========================================
    -- AUDITORÍA
    -- ========================================
    ('AUDITAR_CONSULTAS', '0', 'BOOL', 
     'Registrar en auditoría las consultas (solo lectura)', 'AUDITORIA', 0),
    
    ('AUDITAR_IMPRESIONES', '1', 'BOOL', 
     'Registrar en auditoría las impresiones', 'AUDITORIA', 1),
    
    ('AUDITAR_EXPORTACIONES', '1', 'BOOL', 
     'Registrar en auditoría las exportaciones', 'AUDITORIA', 1);

PRINT '✓ ConfiguracionModuloCXP insertada: ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' registros';
GO

-- ============================================================================
-- VERIFICACIÓN
-- ============================================================================
PRINT '';
PRINT '=======================================================';
PRINT 'CONFIGURACIONES POR CATEGORÍA:';
PRINT '=======================================================';

SELECT 
    Categoria,
    COUNT(*) AS Cantidad
FROM dbo.ConfiguracionModuloCXP
GROUP BY Categoria
ORDER BY Categoria;

PRINT '';
PRINT '=======================================================';
PRINT '✓ SCRIPT 09 COMPLETADO EXITOSAMENTE';
PRINT '=======================================================';
PRINT 'Configuraciones insertadas por categoría.';
PRINT '';
PRINT 'Siguiente paso: Actualizar catálogo de formularios';
PRINT '=======================================================';
GO
