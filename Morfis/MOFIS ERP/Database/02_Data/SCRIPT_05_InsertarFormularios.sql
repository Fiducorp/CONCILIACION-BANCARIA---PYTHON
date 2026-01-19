-- ============================================================================
-- SCRIPT 5: INSERTAR FORMULARIOS INICIALES
-- ============================================================================
-- Descripción: Inserta SOLO los formularios actualmente implementados
-- Se actualizará cada vez que se cree un nuevo formulario
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si ya existen formularios
IF EXISTS (SELECT 1 FROM dbo.CatalogoFormularios)
BEGIN
    PRINT 'La tabla CatalogoFormularios ya contiene datos.';
    PRINT 'Eliminando formularios existentes...';
    DELETE FROM dbo.CatalogoFormularios;
    DBCC CHECKIDENT ('dbo.CatalogoFormularios', RESEED, 0);
    PRINT '✓ Formularios anteriores eliminados';
END
GO

-- Insertar formularios ACTUALES del sistema
INSERT INTO dbo.CatalogoFormularios 
    (ModuloID, CodigoFormulario, NombreFormulario, NombreClase, RutaCompleta, Descripcion, OrdenVisualizacion, EsReporte, Activo)
VALUES 
    -- ============================================
    -- SISTEMA > GESTIÓN DE USUARIOS (ModuloID = 1)
    -- ============================================
    (1, 'FGEUSR', 'Gestión de Usuarios', 'FormGestionUsuarios', 
     'Sistema > Gestión de Usuarios', 
     'CRUD completo de usuarios del sistema', 1, 0, 1),
    
    (1, 'FUSR', 'Usuario (Alta/Edición)', 'FormUsuario', 
     'Sistema > Gestión de Usuarios > Usuario', 
     'Formulario para crear y editar usuarios', 2, 0, 1),
    
    (1, 'FRESETPWD', 'Reset de Contraseña', 'FormResetPassword', 
     'Sistema > Gestión de Usuarios > Reset Password', 
     'Resetear contraseña de usuario', 3, 0, 1),
    
    (1, 'FCONFIRMDEL', 'Confirmar Eliminación', 'FormConfirmarEliminacion', 
     'Sistema > Gestión de Usuarios > Confirmar Eliminación', 
     'Confirmación de eliminación de usuario', 4, 0, 1);
GO

-- Verificar inserción
SELECT 
    F.FormularioID,
    C.NombreCategoria AS Categoria,
    M.NombreModulo AS Modulo,
    F.CodigoFormulario,
    F.NombreFormulario,
    F.NombreClase,
    F.EsReporte,
    F.Activo
FROM dbo.CatalogoFormularios F
INNER JOIN dbo.CatalogoModulos M ON F.ModuloID = M.ModuloID
INNER JOIN dbo.CatalogoCategorias C ON M.CategoriaID = C.CategoriaID
ORDER BY C.OrdenVisualizacion, M.OrdenVisualizacion, F.OrdenVisualizacion;
GO

PRINT '';
PRINT '=======================================================';
PRINT '✓ Formularios insertados exitosamente';
PRINT '=======================================================';
PRINT 'Formularios implementados actualmente:';
PRINT '  SISTEMA > Gestión de Usuarios:';
PRINT '    - FGEUSR: Gestión de Usuarios';
PRINT '    - FUSR: Usuario (Alta/Edición)';
PRINT '    - FRESETPWD: Reset de Contraseña';
PRINT '    - FCONFIRMDEL: Confirmar Eliminación';
PRINT '';
PRINT 'Total formularios registrados: 4';
PRINT 'Nota: Se actualizará al crear nuevos formularios';
PRINT 'Estado: COMPLETADO';
PRINT '=======================================================';
GO