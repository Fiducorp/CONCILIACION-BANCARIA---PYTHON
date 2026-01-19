-- ============================================================================
-- SCRIPT 10: ACTUALIZAR CATÁLOGOS PARA MÓDULO CXP
-- ============================================================================
-- Descripción: Actualiza los catálogos existentes para incluir:
--              - Nuevos formularios del módulo CXP
--              - Nuevas acciones específicas de CXP
--              - Permisos para los roles existentes
-- Fecha: 2026-01-17
-- Módulo: CONTABILIDAD > CUENTAS POR PAGAR
-- ============================================================================

USE FiducorpERP;
GO

PRINT '=======================================================';
PRINT 'SCRIPT 10: ACTUALIZAR CATÁLOGOS PARA CXP';
PRINT '=======================================================';
PRINT '';

-- ============================================================================
-- 1. OBTENER ID DEL MÓDULO CXP
-- ============================================================================
DECLARE @ModuloIDCXP INT;
SELECT @ModuloIDCXP = ModuloID FROM dbo.CatalogoModulos WHERE CodigoModulo = 'CXP';

IF @ModuloIDCXP IS NULL
BEGIN
    PRINT '⚠ ERROR: Módulo CXP no encontrado.';
    PRINT '  Ejecute primero los scripts de datos iniciales.';
    RETURN;
END

PRINT '✓ Módulo CXP encontrado: ModuloID = ' + CAST(@ModuloIDCXP AS VARCHAR(10));
PRINT '';

-- ============================================================================
-- 2. INSERTAR FORMULARIOS DEL MÓDULO CXP
-- ============================================================================
PRINT 'Insertando formularios de Cuentas por Pagar...';

-- Dashboard de Cuentas por Pagar
IF NOT EXISTS (SELECT 1 FROM dbo.CatalogoFormularios WHERE CodigoFormulario = 'FDASHCXP')
BEGIN
    INSERT INTO dbo.CatalogoFormularios 
        (ModuloID, CodigoFormulario, NombreFormulario, NombreClase, RutaCompleta, Descripcion, OrdenVisualizacion, EsReporte, Activo)
    VALUES 
        (@ModuloIDCXP, 'FDASHCXP', 'Dashboard Cuentas por Pagar', 'FormDashboardCuentasPorPagar', 
         'Contabilidad > Cuentas por Pagar', 
         'Dashboard principal del módulo con cards de acceso', 1, 0, 1);
    PRINT '  ✓ FDASHCXP - Dashboard Cuentas por Pagar';
END
ELSE
    PRINT '  - FDASHCXP ya existe';

-- Menú Cartas y Solicitudes
IF NOT EXISTS (SELECT 1 FROM dbo.CatalogoFormularios WHERE CodigoFormulario = 'FMENUCARSOL')
BEGIN
    INSERT INTO dbo.CatalogoFormularios 
        (ModuloID, CodigoFormulario, NombreFormulario, NombreClase, RutaCompleta, Descripcion, OrdenVisualizacion, EsReporte, Activo)
    VALUES 
        (@ModuloIDCXP, 'FMENUCARSOL', 'Menú Cartas y Solicitudes', 'FormMenuCartasSolicitudes', 
         'Contabilidad > Cuentas por Pagar > Cartas y Solicitudes', 
         'Menú lateral con acceso a todas las formas', 2, 0, 1);
    PRINT '  ✓ FMENUCARSOL - Menú Cartas y Solicitudes';
END
ELSE
    PRINT '  - FMENUCARSOL ya existe';

-- Solicitud de Pago
IF NOT EXISTS (SELECT 1 FROM dbo.CatalogoFormularios WHERE CodigoFormulario = 'FSOLPAGO')
BEGIN
    INSERT INTO dbo.CatalogoFormularios 
        (ModuloID, CodigoFormulario, NombreFormulario, NombreClase, RutaCompleta, Descripcion, OrdenVisualizacion, EsReporte, Activo)
    VALUES 
        (@ModuloIDCXP, 'FSOLPAGO', 'Solicitud de Pago', 'FormSolicitudPago', 
         'Contabilidad > Cuentas por Pagar > Solicitud de Pago', 
         'Registro de solicitudes de pago a proveedores', 3, 0, 1);
    PRINT '  ✓ FSOLPAGO - Solicitud de Pago';
END
ELSE
    PRINT '  - FSOLPAGO ya existe';

-- Consulta de Solicitudes
IF NOT EXISTS (SELECT 1 FROM dbo.CatalogoFormularios WHERE CodigoFormulario = 'FCONSOLPAGO')
BEGIN
    INSERT INTO dbo.CatalogoFormularios 
        (ModuloID, CodigoFormulario, NombreFormulario, NombreClase, RutaCompleta, Descripcion, OrdenVisualizacion, EsReporte, Activo)
    VALUES 
        (@ModuloIDCXP, 'FCONSOLPAGO', 'Consulta de Solicitudes', 'FormConsultaSolicitudes', 
         'Contabilidad > Cuentas por Pagar > Consulta de Solicitudes', 
         'Búsqueda y gestión de solicitudes existentes', 4, 0, 1);
    PRINT '  ✓ FCONSOLPAGO - Consulta de Solicitudes';
END
ELSE
    PRINT '  - FCONSOLPAGO ya existe';

-- Certificado de Retención
IF NOT EXISTS (SELECT 1 FROM dbo.CatalogoFormularios WHERE CodigoFormulario = 'FCERTRET')
BEGIN
    INSERT INTO dbo.CatalogoFormularios 
        (ModuloID, CodigoFormulario, NombreFormulario, NombreClase, RutaCompleta, Descripcion, OrdenVisualizacion, EsReporte, Activo)
    VALUES 
        (@ModuloIDCXP, 'FCERTRET', 'Certificado de Retención', 'FormCertificadoRetencion', 
         'Contabilidad > Cuentas por Pagar > Certificado de Retención', 
         'Generación de certificados de retención', 5, 0, 1);
    PRINT '  ✓ FCERTRET - Certificado de Retención';
END
ELSE
    PRINT '  - FCERTRET ya existe';

-- Relación de Pago
IF NOT EXISTS (SELECT 1 FROM dbo.CatalogoFormularios WHERE CodigoFormulario = 'FRELPAGO')
BEGIN
    INSERT INTO dbo.CatalogoFormularios 
        (ModuloID, CodigoFormulario, NombreFormulario, NombreClase, RutaCompleta, Descripcion, OrdenVisualizacion, EsReporte, Activo)
    VALUES 
        (@ModuloIDCXP, 'FRELPAGO', 'Relación de Pago', 'FormRelacionPago', 
         'Contabilidad > Cuentas por Pagar > Relación de Pago', 
         'Generación de relaciones de pago', 6, 0, 1);
    PRINT '  ✓ FRELPAGO - Relación de Pago';
END
ELSE
    PRINT '  - FRELPAGO ya existe';

-- Relación de Anticipos
IF NOT EXISTS (SELECT 1 FROM dbo.CatalogoFormularios WHERE CodigoFormulario = 'FRELANTICIPO')
BEGIN
    INSERT INTO dbo.CatalogoFormularios 
        (ModuloID, CodigoFormulario, NombreFormulario, NombreClase, RutaCompleta, Descripcion, OrdenVisualizacion, EsReporte, Activo)
    VALUES 
        (@ModuloIDCXP, 'FRELANTICIPO', 'Relación de Anticipos', 'FormRelacionAnticipos', 
         'Contabilidad > Cuentas por Pagar > Relación de Anticipos', 
         'Gestión de anticipos y avances', 7, 0, 1);
    PRINT '  ✓ FRELANTICIPO - Relación de Anticipos';
END
ELSE
    PRINT '  - FRELANTICIPO ya existe';

-- Carta de Desistimiento
IF NOT EXISTS (SELECT 1 FROM dbo.CatalogoFormularios WHERE CodigoFormulario = 'FCARTDESIST')
BEGIN
    INSERT INTO dbo.CatalogoFormularios 
        (ModuloID, CodigoFormulario, NombreFormulario, NombreClase, RutaCompleta, Descripcion, OrdenVisualizacion, EsReporte, Activo)
    VALUES 
        (@ModuloIDCXP, 'FCARTDESIST', 'Carta de Desistimiento', 'FormCartaDesistimiento', 
         'Contabilidad > Cuentas por Pagar > Carta de Desistimiento', 
         'Generación de cartas de desistimiento', 8, 0, 1);
    PRINT '  ✓ FCARTDESIST - Carta de Desistimiento';
END
ELSE
    PRINT '  - FCARTDESIST ya existe';

-- Configuración CXP
IF NOT EXISTS (SELECT 1 FROM dbo.CatalogoFormularios WHERE CodigoFormulario = 'FCONFCXP')
BEGIN
    INSERT INTO dbo.CatalogoFormularios 
        (ModuloID, CodigoFormulario, NombreFormulario, NombreClase, RutaCompleta, Descripcion, OrdenVisualizacion, EsReporte, Activo)
    VALUES 
        (@ModuloIDCXP, 'FCONFCXP', 'Configuración CXP', 'FormConfiguracionCXP', 
         'Contabilidad > Cuentas por Pagar > Configuración', 
         'Configuración del módulo y firmas digitales', 9, 0, 1);
    PRINT '  ✓ FCONFCXP - Configuración CXP';
END
ELSE
    PRINT '  - FCONFCXP ya existe';

-- Gestión de Fideicomisos
IF NOT EXISTS (SELECT 1 FROM dbo.CatalogoFormularios WHERE CodigoFormulario = 'FGESFIDEI')
BEGIN
    INSERT INTO dbo.CatalogoFormularios 
        (ModuloID, CodigoFormulario, NombreFormulario, NombreClase, RutaCompleta, Descripcion, OrdenVisualizacion, EsReporte, Activo)
    VALUES 
        (@ModuloIDCXP, 'FGESFIDEI', 'Gestión de Fideicomisos', 'FormGestionFideicomisos', 
         'Contabilidad > Cuentas por Pagar > Fideicomisos', 
         'Alta, edición y consulta de fideicomisos', 10, 0, 1);
    PRINT '  ✓ FGESFIDEI - Gestión de Fideicomisos';
END
ELSE
    PRINT '  - FGESFIDEI ya existe';

-- Gestión de Proveedores
IF NOT EXISTS (SELECT 1 FROM dbo.CatalogoFormularios WHERE CodigoFormulario = 'FGESPROV')
BEGIN
    INSERT INTO dbo.CatalogoFormularios 
        (ModuloID, CodigoFormulario, NombreFormulario, NombreClase, RutaCompleta, Descripcion, OrdenVisualizacion, EsReporte, Activo)
    VALUES 
        (@ModuloIDCXP, 'FGESPROV', 'Gestión de Proveedores', 'FormGestionProveedores', 
         'Contabilidad > Cuentas por Pagar > Proveedores', 
         'Alta, edición y consulta de proveedores', 11, 0, 1);
    PRINT '  ✓ FGESPROV - Gestión de Proveedores';
END
ELSE
    PRINT '  - FGESPROV ya existe';

-- Gestión de Firmas
IF NOT EXISTS (SELECT 1 FROM dbo.CatalogoFormularios WHERE CodigoFormulario = 'FGESFIRMAS')
BEGIN
    INSERT INTO dbo.CatalogoFormularios 
        (ModuloID, CodigoFormulario, NombreFormulario, NombreClase, RutaCompleta, Descripcion, OrdenVisualizacion, EsReporte, Activo)
    VALUES 
        (@ModuloIDCXP, 'FGESFIRMAS', 'Gestión de Firmas', 'FormGestionFirmas', 
         'Contabilidad > Cuentas por Pagar > Configuración > Firmas', 
         'Gestión de firmas digitales de usuario', 12, 0, 1);
    PRINT '  ✓ FGESFIRMAS - Gestión de Firmas';
END
ELSE
    PRINT '  - FGESFIRMAS ya existe';

PRINT '';

-- ============================================================================
-- 3. INSERTAR NUEVAS ACCIONES ESPECÍFICAS DE CXP
-- ============================================================================
PRINT 'Verificando acciones específicas de CXP...';

-- Clonar solicitud
IF NOT EXISTS (SELECT 1 FROM dbo.CatalogoAcciones WHERE CodigoAccion = 'CLONE')
BEGIN
    INSERT INTO dbo.CatalogoAcciones 
        (CodigoAccion, NombreAccion, Descripcion, GrupoAccion, OrdenVisualizacion, Activo)
    VALUES 
        ('CLONE', 'Clonar / Copiar', 'Crear copia de un registro existente', 'CRUD', 16, 1);
    PRINT '  ✓ CLONE - Clonar / Copiar';
END

-- Firmar documento
IF NOT EXISTS (SELECT 1 FROM dbo.CatalogoAcciones WHERE CodigoAccion = 'SIGN')
BEGIN
    INSERT INTO dbo.CatalogoAcciones 
        (CodigoAccion, NombreAccion, Descripcion, GrupoAccion, OrdenVisualizacion, Activo)
    VALUES 
        ('SIGN', 'Firmar', 'Aplicar firma digital a documento', 'ADMINISTRACION', 17, 1);
    PRINT '  ✓ SIGN - Firmar';
END

-- Editar propias
IF NOT EXISTS (SELECT 1 FROM dbo.CatalogoAcciones WHERE CodigoAccion = 'EDITOWN')
BEGIN
    INSERT INTO dbo.CatalogoAcciones 
        (CodigoAccion, NombreAccion, Descripcion, GrupoAccion, OrdenVisualizacion, Activo)
    VALUES 
        ('EDITOWN', 'Editar Propias', 'Editar solo registros propios', 'CRUD', 18, 1);
    PRINT '  ✓ EDITOWN - Editar Propias';
END

-- Eliminar propias
IF NOT EXISTS (SELECT 1 FROM dbo.CatalogoAcciones WHERE CodigoAccion = 'DELETEOWN')
BEGIN
    INSERT INTO dbo.CatalogoAcciones 
        (CodigoAccion, NombreAccion, Descripcion, GrupoAccion, OrdenVisualizacion, Activo)
    VALUES 
        ('DELETEOWN', 'Eliminar Propias', 'Eliminar solo registros propios', 'CRUD', 19, 1);
    PRINT '  ✓ DELETEOWN - Eliminar Propias';
END

-- Exportar PDF
IF NOT EXISTS (SELECT 1 FROM dbo.CatalogoAcciones WHERE CodigoAccion = 'EXPORTPDF')
BEGIN
    INSERT INTO dbo.CatalogoAcciones 
        (CodigoAccion, NombreAccion, Descripcion, GrupoAccion, OrdenVisualizacion, Activo)
    VALUES 
        ('EXPORTPDF', 'Exportar PDF', 'Exportar documento a formato PDF', 'EXPORTACION', 20, 1);
    PRINT '  ✓ EXPORTPDF - Exportar PDF';
END

-- Exportar Excel
IF NOT EXISTS (SELECT 1 FROM dbo.CatalogoAcciones WHERE CodigoAccion = 'EXPORTXLS')
BEGIN
    INSERT INTO dbo.CatalogoAcciones 
        (CodigoAccion, NombreAccion, Descripcion, GrupoAccion, OrdenVisualizacion, Activo)
    VALUES 
        ('EXPORTXLS', 'Exportar Excel', 'Exportar documento a formato Excel', 'EXPORTACION', 21, 1);
    PRINT '  ✓ EXPORTXLS - Exportar Excel';
END

PRINT '';

-- ============================================================================
-- 4. INSERTAR PERMISOS PARA LOS NUEVOS FORMULARIOS
-- ============================================================================
PRINT 'Insertando permisos para formularios CXP...';

-- Obtener IDs de los formularios recién creados
DECLARE @FormulariosCXP TABLE (FormularioID INT, CodigoFormulario NVARCHAR(20));

INSERT INTO @FormulariosCXP
SELECT FormularioID, CodigoFormulario 
FROM dbo.CatalogoFormularios 
WHERE CodigoFormulario IN (
    'FDASHCXP', 'FMENUCARSOL', 'FSOLPAGO', 'FCONSOLPAGO', 
    'FCERTRET', 'FRELPAGO', 'FRELANTICIPO', 'FCARTDESIST',
    'FCONFCXP', 'FGESFIDEI', 'FGESPROV', 'FGESFIRMAS'
);

-- Obtener acciones relevantes
DECLARE @AccionesRelevantes TABLE (AccionID INT, CodigoAccion NVARCHAR(20));

INSERT INTO @AccionesRelevantes
SELECT AccionID, CodigoAccion 
FROM dbo.CatalogoAcciones 
WHERE CodigoAccion IN (
    'VIEW', 'CREATE', 'EDIT', 'DELETE', 'PRINT', 'REPRINT', 
    'EXPORT', 'EXPORTPDF', 'EXPORTXLS', 'CLONE', 'SIGN',
    'EDITOWN', 'DELETEOWN'
)
AND Activo = 1;

-- ========================================
-- PERMISOS PARA ROOT (RolID = 1) - Todo permitido
-- ========================================
INSERT INTO dbo.PermisosRol (RolID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID)
SELECT 
    1 AS RolID,
    f.FormularioID,
    a.AccionID,
    1 AS Permitido,
    1 AS CreadoPorUsuarioID
FROM @FormulariosCXP f
CROSS JOIN @AccionesRelevantes a
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.PermisosRol pr 
    WHERE pr.RolID = 1 
    AND pr.FormularioID = f.FormularioID 
    AND pr.AccionID = a.AccionID
);

PRINT '  ✓ Permisos ROOT insertados';

-- ========================================
-- PERMISOS PARA ADMIN (RolID = 2) - Todo permitido
-- ========================================
INSERT INTO dbo.PermisosRol (RolID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID)
SELECT 
    2 AS RolID,
    f.FormularioID,
    a.AccionID,
    1 AS Permitido,
    1 AS CreadoPorUsuarioID
FROM @FormulariosCXP f
CROSS JOIN @AccionesRelevantes a
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.PermisosRol pr 
    WHERE pr.RolID = 2 
    AND pr.FormularioID = f.FormularioID 
    AND pr.AccionID = a.AccionID
);

PRINT '  ✓ Permisos ADMIN insertados';

-- ========================================
-- PERMISOS PARA CONTADOR (RolID = 3)
-- CRUD en sus propios registros, consulta de todos
-- ========================================
INSERT INTO dbo.PermisosRol (RolID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID)
SELECT 
    3 AS RolID,
    f.FormularioID,
    a.AccionID,
    CASE 
        -- Permitir ver todo
        WHEN a.CodigoAccion = 'VIEW' THEN 1
        -- Permitir crear
        WHEN a.CodigoAccion = 'CREATE' THEN 1
        -- Permitir editar/eliminar propias
        WHEN a.CodigoAccion IN ('EDITOWN', 'DELETEOWN') THEN 1
        -- NO permitir editar/eliminar de otros
        WHEN a.CodigoAccion IN ('EDIT', 'DELETE') THEN 0
        -- Permitir impresión y exportación
        WHEN a.CodigoAccion IN ('PRINT', 'REPRINT', 'EXPORT', 'EXPORTPDF', 'EXPORTXLS') THEN 1
        -- Permitir clonar
        WHEN a.CodigoAccion = 'CLONE' THEN 1
        -- Permitir firmar (solo su firma)
        WHEN a.CodigoAccion = 'SIGN' THEN 1
        -- Denegar configuración
        WHEN f.CodigoFormulario = 'FCONFCXP' AND a.CodigoAccion NOT IN ('VIEW') THEN 0
        ELSE 0
    END AS Permitido,
    1 AS CreadoPorUsuarioID
FROM @FormulariosCXP f
CROSS JOIN @AccionesRelevantes a
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.PermisosRol pr 
    WHERE pr.RolID = 3 
    AND pr.FormularioID = f.FormularioID 
    AND pr.AccionID = a.AccionID
);

PRINT '  ✓ Permisos CONTADOR insertados';

-- ========================================
-- PERMISOS PARA GERENTE (RolID = 4) - Si existe
-- ========================================
IF EXISTS (SELECT 1 FROM dbo.Roles WHERE RolID = 4)
BEGIN
    INSERT INTO dbo.PermisosRol (RolID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID)
    SELECT 
        4 AS RolID,
        f.FormularioID,
        a.AccionID,
        CASE 
            WHEN a.CodigoAccion IN ('VIEW', 'PRINT', 'EXPORT', 'EXPORTPDF', 'EXPORTXLS') THEN 1
            ELSE 0
        END AS Permitido,
        1 AS CreadoPorUsuarioID
    FROM @FormulariosCXP f
    CROSS JOIN @AccionesRelevantes a
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.PermisosRol pr 
        WHERE pr.RolID = 4 
        AND pr.FormularioID = f.FormularioID 
        AND pr.AccionID = a.AccionID
    );
    
    PRINT '  ✓ Permisos GERENTE insertados';
END

-- ========================================
-- PERMISOS PARA ANALISTA (RolID = 5) - Si existe
-- ========================================
IF EXISTS (SELECT 1 FROM dbo.Roles WHERE RolID = 5)
BEGIN
    INSERT INTO dbo.PermisosRol (RolID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID)
    SELECT 
        5 AS RolID,
        f.FormularioID,
        a.AccionID,
        CASE 
            WHEN a.CodigoAccion = 'VIEW' THEN 1
            ELSE 0
        END AS Permitido,
        1 AS CreadoPorUsuarioID
    FROM @FormulariosCXP f
    CROSS JOIN @AccionesRelevantes a
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.PermisosRol pr 
        WHERE pr.RolID = 5 
        AND pr.FormularioID = f.FormularioID 
        AND pr.AccionID = a.AccionID
    );
    
    PRINT '  ✓ Permisos ANALISTA insertados';
END

PRINT '';

-- ============================================================================
-- 5. VERIFICACIÓN FINAL
-- ============================================================================
PRINT '=======================================================';
PRINT 'VERIFICACIÓN FINAL';
PRINT '=======================================================';
PRINT '';

PRINT 'Formularios CXP registrados:';
SELECT 
    CodigoFormulario AS Codigo,
    NombreFormulario AS Nombre,
    Activo
FROM dbo.CatalogoFormularios
WHERE ModuloID = @ModuloIDCXP
ORDER BY OrdenVisualizacion;

PRINT '';
PRINT 'Permisos por rol para formularios CXP:';
SELECT 
    R.NombreRol,
    COUNT(*) AS TotalPermisos,
    SUM(CASE WHEN PR.Permitido = 1 THEN 1 ELSE 0 END) AS Permitidos,
    SUM(CASE WHEN PR.Permitido = 0 THEN 1 ELSE 0 END) AS Denegados
FROM dbo.PermisosRol PR
INNER JOIN dbo.Roles R ON PR.RolID = R.RolID
INNER JOIN dbo.CatalogoFormularios F ON PR.FormularioID = F.FormularioID
WHERE F.ModuloID = @ModuloIDCXP
GROUP BY R.RolID, R.NombreRol
ORDER BY R.RolID;

PRINT '';
PRINT '=======================================================';
PRINT '✓ SCRIPT 10 COMPLETADO EXITOSAMENTE';
PRINT '=======================================================';
PRINT 'Se han registrado:';
PRINT '  - 12 formularios para el módulo CXP';
PRINT '  - 6 nuevas acciones específicas';
PRINT '  - Permisos para todos los roles existentes';
PRINT '';
PRINT 'Siguiente paso: Ejecutar scripts en SQL Server';
PRINT '=======================================================';
GO
