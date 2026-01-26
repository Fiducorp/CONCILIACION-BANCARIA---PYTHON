-- ============================================================================
-- SCRIPT 4: INSERTAR MÓDULOS INICIALES
-- ============================================================================
-- Descripción: Inserta los módulos del sistema organizados por categoría
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si ya existen módulos
IF EXISTS (SELECT 1 FROM dbo.CatalogoModulos)
BEGIN
    PRINT 'La tabla CatalogoModulos ya contiene datos.';
    PRINT 'Eliminando módulos existentes...';
    DELETE FROM dbo.CatalogoModulos;
    DBCC CHECKIDENT ('dbo.CatalogoModulos', RESEED, 0);
    PRINT '✓ Módulos anteriores eliminados';
END
GO

-- Insertar módulos del sistema
INSERT INTO dbo.CatalogoModulos 
    (CategoriaID, CodigoModulo, NombreModulo, Descripcion, Icono, OrdenVisualizacion, Activo)
VALUES 
    -- ============================================
    -- SISTEMA (CategoriaID = 1)
    -- ============================================
    (1, 'GEUSR', 'GESTIÓN DE USUARIOS', 
     'Administración de usuarios del sistema', '👥', 1, 1),
    
    (1, 'GEROL', 'GESTIÓN DE ROLES', 
     'Configuración de roles y permisos', '🔐', 2, 1),
    
    (1, 'AUD', 'AUDITORÍA', 
     'Registro de actividades del sistema', '📋', 3, 1),
    
    -- ============================================
    -- CONTABILIDAD (CategoriaID = 2)
    -- ============================================
    (2, 'CXP', 'CUENTAS POR PAGAR', 
     'Gestión de solicitudes de pago y proveedores', '💳', 1, 1),
    
    (2, 'IMP', 'IMPUESTOS', 
     'Gestión de impuestos y retenciones', '📊', 2, 1),
    
    (2, 'REC', 'RECAUDOS', 
     'Gestión de recaudos y cobranzas', '💵', 3, 1),
    
    (2, 'FUT', 'FUTURA', 
     'Módulo futuro de contabilidad', '🔮', 4, 1),
    
    -- ============================================
    -- GERENCIA FINANCIERA (CategoriaID = 3)
    -- ============================================
    (3, 'REPFIN', 'REPORTES FINANCIEROS', 
     'Reportes y análisis financiero', '📈', 1, 1),
    
    -- ============================================
    -- GERENCIA LEGAL (CategoriaID = 4)
    -- ============================================
    (4, 'CONTR', 'CONTRATOS', 
     'Gestión de contratos', '📄', 1, 1),
    
    -- ============================================
    -- DESARROLLO (CategoriaID = 5)
    -- ============================================
    (5, 'HERR', 'HERRAMIENTAS', 
     'Herramientas de desarrollo', '🛠️', 1, 1);
GO

-- Verificar inserción
SELECT 
    M.ModuloID,
    C.NombreCategoria AS Categoria,
    M.CodigoModulo,
    M.NombreModulo,
    M.Icono,
    M.OrdenVisualizacion,
    M.Activo
FROM dbo.CatalogoModulos M
INNER JOIN dbo.CatalogoCategorias C ON M.CategoriaID = C.CategoriaID
ORDER BY C.OrdenVisualizacion, M.OrdenVisualizacion;
GO

PRINT '';
PRINT '=======================================================';
PRINT '✓ Módulos insertados exitosamente';
PRINT '=======================================================';
PRINT 'Módulos por categoría:';
PRINT '  SISTEMA:';
PRINT '    - Gestión de Usuarios';
PRINT '    - Gestión de Roles';
PRINT '    - Auditoría';
PRINT '  CONTABILIDAD:';
PRINT '    - Cuentas por Pagar';
PRINT '    - Impuestos';
PRINT '    - Recaudos';
PRINT '    - Futura';
PRINT '  GERENCIA FINANCIERA:';
PRINT '    - Reportes Financieros';
PRINT '  GERENCIA LEGAL:';
PRINT '    - Contratos';
PRINT '  DESARROLLO:';
PRINT '    - Herramientas';
PRINT '';
PRINT 'Estado: COMPLETADO';
PRINT '=======================================================';
GO