-- ============================================================================
-- SCRIPT 6: INSERTAR ACCIONES ESTÁNDAR
-- ============================================================================
-- Descripción: Inserta las acciones estándar del sistema de permisos
-- Estas acciones son aplicables a todos los formularios
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si ya existen acciones
IF EXISTS (SELECT 1 FROM dbo.CatalogoAcciones)
BEGIN
    PRINT 'La tabla CatalogoAcciones ya contiene datos.';
    PRINT 'Eliminando acciones existentes...';
    DELETE FROM dbo.CatalogoAcciones;
    DBCC CHECKIDENT ('dbo.CatalogoAcciones', RESEED, 0);
    PRINT '✓ Acciones anteriores eliminadas';
END
GO

-- Insertar acciones estándar
INSERT INTO dbo.CatalogoAcciones 
    (CodigoAccion, NombreAccion, Descripcion, GrupoAccion, OrdenVisualizacion, Activo)
VALUES 
    -- ============================================
    -- GRUPO: CRUD (Operaciones básicas)
    -- ============================================
    ('VIEW', 'Ver / Consultar', 
     'Abrir formulario y consultar registros', 
     'CRUD', 1, 1),
    
    ('CREATE', 'Crear / Nuevo', 
     'Crear nuevos registros', 
     'CRUD', 2, 1),
    
    ('EDIT', 'Editar / Modificar', 
     'Modificar registros existentes', 
     'CRUD', 3, 1),
    
    ('DELETE', 'Eliminar', 
     'Eliminar registros (lógica o físicamente)', 
     'CRUD', 4, 1),
    
    -- ============================================
    -- GRUPO: IMPRESIÓN
    -- ============================================
    ('PRINT', 'Imprimir', 
     'Imprimir documentos por primera vez', 
     'IMPRESION', 5, 1),
    
    ('REPRINT', 'Reimprimir', 
     'Volver a imprimir documentos ya impresos', 
     'IMPRESION', 6, 1),
    
    -- ============================================
    -- GRUPO: EXPORTACIÓN
    -- ============================================
    ('EXPORT', 'Exportar', 
     'Exportar datos a Excel, PDF u otros formatos', 
     'EXPORTACION', 7, 1),
    
    -- ============================================
    -- GRUPO: APROBACIÓN (futuro)
    -- ============================================
    ('APPROVE', 'Aprobar', 
     'Aprobar solicitudes, pagos, documentos', 
     'APROBACION', 8, 1),
    
    ('REJECT', 'Rechazar', 
     'Rechazar solicitudes, pagos, documentos', 
     'APROBACION', 9, 1),
    
    -- ============================================
    -- GRUPO: ADMINISTRACIÓN
    -- ============================================
    ('ACTIVATE', 'Activar / Desactivar', 
     'Cambiar estado activo/inactivo de registros', 
     'ADMINISTRACION', 10, 1),
    
    ('RESET', 'Resetear', 
     'Resetear contraseñas, estados, configuraciones', 
     'ADMINISTRACION', 11, 1),
    
    -- ============================================
    -- GRUPO: CONTABILIDAD (específicas)
    -- ============================================
    ('POST', 'Contabilizar', 
     'Registrar en la contabilidad', 
     'CONTABILIDAD', 12, 1),
    
    ('VOID', 'Anular', 
     'Anular documentos contables', 
     'CONTABILIDAD', 13, 1),
    
    ('CLOSE', 'Cerrar Periodo', 
     'Cerrar periodo contable', 
     'CONTABILIDAD', 14, 1),
    
    ('REOPEN', 'Reabrir Periodo', 
     'Reabrir periodo contable cerrado', 
     'CONTABILIDAD', 15, 1);
GO

-- Verificar inserción
SELECT 
    AccionID,
    CodigoAccion,
    NombreAccion,
    GrupoAccion,
    OrdenVisualizacion,
    Activo
FROM dbo.CatalogoAcciones
ORDER BY OrdenVisualizacion;
GO

PRINT '';
PRINT '=======================================================';
PRINT '✓ Acciones estándar insertadas exitosamente';
PRINT '=======================================================';
PRINT 'Acciones por grupo:';
PRINT '  CRUD:';
PRINT '    - VIEW: Ver / Consultar';
PRINT '    - CREATE: Crear / Nuevo';
PRINT '    - EDIT: Editar / Modificar';
PRINT '    - DELETE: Eliminar';
PRINT '  IMPRESIÓN:';
PRINT '    - PRINT: Imprimir';
PRINT '    - REPRINT: Reimprimir';
PRINT '  EXPORTACIÓN:';
PRINT '    - EXPORT: Exportar';
PRINT '  APROBACIÓN:';
PRINT '    - APPROVE: Aprobar';
PRINT '    - REJECT: Rechazar';
PRINT '  ADMINISTRACIÓN:';
PRINT '    - ACTIVATE: Activar / Desactivar';
PRINT '    - RESET: Resetear';
PRINT '  CONTABILIDAD:';
PRINT '    - POST: Contabilizar';
PRINT '    - VOID: Anular';
PRINT '    - CLOSE: Cerrar Periodo';
PRINT '    - REOPEN: Reabrir Periodo';
PRINT '';
PRINT 'Total acciones registradas: 15';
PRINT 'Estado: COMPLETADO';
PRINT '=======================================================';
GO