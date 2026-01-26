-- ============================================================================
-- SCRIPT 2: CREAR USUARIO ROOT INICIAL
-- ============================================================================
-- Descripción: Crea el primer usuario ROOT del sistema
-- Este usuario tendrá acceso total y podrá crear otros usuarios
-- IMPORTANTE: La contraseña está hasheada con BCrypt
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si ya existe el usuario ROOT
IF EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Username = 'Cysero19')
BEGIN
    PRINT 'El usuario ROOT (Cysero19) ya existe. Eliminándolo...';
    DELETE FROM dbo.Usuarios WHERE Username = 'Cysero19';
    PRINT '✓ Usuario ROOT anterior eliminado';
END
GO

-- Insertar usuario ROOT
-- Contraseña temporal: 198237645M
-- Hash generado con BCrypt (Cost Factor: 10)
INSERT INTO dbo.Usuarios 
(
    Username,
    PasswordHash,
    NombreCompleto,
    Email,
    RolID,
    Activo,
    DebeCambiarPassword,
    FechaCreacion,
    CreadoPorUsuarioID
)
VALUES 
(
    'Cysero19',
    '$2a$11$ZzqQefZT4dQByDgBQWAD9.o4Z.hQtqT6qo.h.JyCgKEhGE7DWCGlm',  -- Hash de: 198237645M
    'Melvin Ortiz',
    'melvinortiz1919@gmail.com',
    1,  -- RolID = 1 (ROOT)
    1,  -- Activo = 1
    1,  -- DebeCambiarPassword = 1 (forzar cambio en primer login)
    GETDATE(),
    NULL  -- El primer usuario no tiene creador
);
GO

-- Verificar la inserción
SELECT 
    UsuarioID,
    Username,
    NombreCompleto,
    Email,
    RolID,
    (SELECT NombreRol FROM dbo.Roles WHERE RolID = U.RolID) AS NombreRol,
    Activo,
    DebeCambiarPassword,
    FechaCreacion
FROM dbo.Usuarios U
WHERE Username = 'Cysero19';
GO

PRINT '';
PRINT '=======================================================';
PRINT '✓ Usuario ROOT creado exitosamente';
PRINT '=======================================================';
PRINT 'Credenciales de acceso:';
PRINT '  Username: Cysero19';
PRINT '  Password: 198237645M';
PRINT '  Nombre:   Melvin Ortiz';
PRINT '  Email:    melvinortiz1919@gmail.com';
PRINT '  Rol:      ROOT (Acceso Total)';
PRINT '';
PRINT '⚠️  IMPORTANTE:';
PRINT '  - Deberás cambiar la contraseña en el primer login';
PRINT '  - La contraseña está hasheada con BCrypt';
PRINT '  - Guarda estas credenciales en un lugar seguro';
PRINT '';
PRINT '=======================================================';
PRINT '🎉 BASE DE DATOS COMPLETAMENTE CONFIGURADA';
PRINT '=======================================================';
PRINT 'Estado del sistema:';
PRINT '  ✓ Base de datos: FiducorpERP creada';
PRINT '  ✓ Tablas: Roles, Usuarios, Permisos, Auditoria';
PRINT '  ✓ Roles: ROOT, ADMIN, CONTADOR insertados';
PRINT '  ✓ Usuario ROOT: Cysero19 creado';
PRINT '';
PRINT 'Siguiente paso: Desarrollar clase de conexión en C#';
PRINT '=======================================================';
GO