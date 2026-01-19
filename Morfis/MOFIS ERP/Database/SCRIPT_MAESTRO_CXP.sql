-- ============================================================================
-- SCRIPT MAESTRO: INSTALACIÓN MÓDULO CUENTAS POR PAGAR
-- ============================================================================
-- Descripción: Ejecuta todos los scripts necesarios para instalar el módulo
--              Cuentas por Pagar en la base de datos FiducorpERP
-- 
-- IMPORTANTE: Ejecutar este script completo en SQL Server Management Studio
--             Asegúrese de que la base de datos FiducorpERP ya existe
--
-- Fecha: 2026-01-17
-- Módulo: CONTABILIDAD > CUENTAS POR PAGAR
-- ============================================================================

USE FiducorpERP;
GO

PRINT '╔═══════════════════════════════════════════════════════════════════════╗';
PRINT '║                                                                       ║';
PRINT '║     MOFIS-ERP: INSTALACIÓN MÓDULO CUENTAS POR PAGAR                  ║';
PRINT '║                                                                       ║';
PRINT '╚═══════════════════════════════════════════════════════════════════════╝';
PRINT '';
PRINT 'Fecha de ejecución: ' + CONVERT(VARCHAR(20), GETDATE(), 120);
PRINT 'Base de datos: FiducorpERP';
PRINT '';
PRINT '=======================================================';
PRINT 'ORDEN DE EJECUCIÓN DE SCRIPTS:';
PRINT '=======================================================';
PRINT '';
PRINT '  ESQUEMA (01_Schema):';
PRINT '    12. SCRIPT_12_CrearTablasCatalogosCXP.sql';
PRINT '    13. SCRIPT_13_CrearTablasFideicomisosProveedores.sql';
PRINT '    14. SCRIPT_14_CrearTablaSolicitudesPago.sql';
PRINT '    15. SCRIPT_15_CrearTablasFirmasConfiguracion.sql';
PRINT '';
PRINT '  DATOS (03_Data):';
PRINT '    08. SCRIPT_08_InsertarDatosCatalogosCXP.sql';
PRINT '    09. SCRIPT_09_InsertarConfiguracionCXP.sql';
PRINT '    10. SCRIPT_10_ActualizarCatalogosCXP.sql';
PRINT '';
PRINT '=======================================================';
PRINT '';

-- ============================================================================
-- VERIFICAR PREREQUISITOS
-- ============================================================================
PRINT '🔍 Verificando prerequisitos...';
PRINT '';

-- Verificar tablas base
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Usuarios')
BEGIN
    RAISERROR('❌ ERROR: La tabla Usuarios no existe. Ejecute primero los scripts base del sistema.', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Roles')
BEGIN
    RAISERROR('❌ ERROR: La tabla Roles no existe. Ejecute primero los scripts base del sistema.', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'CatalogoModulos')
BEGIN
    RAISERROR('❌ ERROR: La tabla CatalogoModulos no existe. Ejecute primero los scripts base del sistema.', 16, 1);
    RETURN;
END

PRINT '✓ Prerequisitos verificados correctamente';
PRINT '';

-- ============================================================================
-- NOTA IMPORTANTE
-- ============================================================================
PRINT '╔═══════════════════════════════════════════════════════════════════════╗';
PRINT '║  ⚠️  INSTRUCCIONES DE EJECUCIÓN                                       ║';
PRINT '╠═══════════════════════════════════════════════════════════════════════╣';
PRINT '║                                                                       ║';
PRINT '║  Este script es solo una GUÍA. Debe ejecutar cada script              ║';
PRINT '║  individualmente en el siguiente orden:                               ║';
PRINT '║                                                                       ║';
PRINT '║  1. Copie cada archivo .sql a SQL Server Management Studio            ║';
PRINT '║  2. Ejecute en el orden indicado arriba                               ║';
PRINT '║  3. Verifique que cada script termine con "COMPLETADO EXITOSAMENTE"   ║';
PRINT '║                                                                       ║';
PRINT '║  Si algún script falla, NO continúe con el siguiente.                 ║';
PRINT '║  Corrija el error primero.                                            ║';
PRINT '║                                                                       ║';
PRINT '╚═══════════════════════════════════════════════════════════════════════╝';
PRINT '';

-- ============================================================================
-- VERIFICACIÓN POST-INSTALACIÓN (ejecutar después de todos los scripts)
-- ============================================================================
/*
-- Descomente este bloque después de ejecutar todos los scripts para verificar:

PRINT '=======================================================';
PRINT 'VERIFICACIÓN POST-INSTALACIÓN';
PRINT '=======================================================';

-- Verificar tablas creadas
SELECT 
    'Tablas CXP' AS Categoria,
    name AS Tabla,
    create_date AS FechaCreacion
FROM sys.tables
WHERE name IN (
    'TiposNCF', 'Monedas', 'TiposPago', 'TiposComprobante', 
    'TiposFideicomiso', 'MetodosConversion', 'Fideicomisos', 
    'Proveedores', 'SolicitudesPago', 'SolicitudesPagoSubtotales',
    'SolicitudesPagoComprobantes', 'SolicitudesPagoAvances',
    'FirmasUsuarios', 'ConfiguracionModuloCXP', 'MemoriaTemporalFormularios'
)
ORDER BY name;

-- Verificar datos insertados
SELECT 'TiposNCF' AS Tabla, COUNT(*) AS Registros FROM TiposNCF
UNION ALL SELECT 'Monedas', COUNT(*) FROM Monedas
UNION ALL SELECT 'TiposPago', COUNT(*) FROM TiposPago
UNION ALL SELECT 'TiposComprobante', COUNT(*) FROM TiposComprobante
UNION ALL SELECT 'TiposFideicomiso', COUNT(*) FROM TiposFideicomiso
UNION ALL SELECT 'MetodosConversion', COUNT(*) FROM MetodosConversion
UNION ALL SELECT 'ConfiguracionModuloCXP', COUNT(*) FROM ConfiguracionModuloCXP;

-- Verificar formularios registrados
SELECT 
    CodigoFormulario,
    NombreFormulario,
    NombreClase
FROM CatalogoFormularios
WHERE ModuloID = (SELECT ModuloID FROM CatalogoModulos WHERE CodigoModulo = 'CXP')
ORDER BY OrdenVisualizacion;

PRINT '';
PRINT '✅ Si todas las verificaciones son correctas, el módulo está listo.';
*/

PRINT '';
PRINT '=======================================================';
PRINT 'FIN DEL SCRIPT MAESTRO';
PRINT '=======================================================';
GO
