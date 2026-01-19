-- ============================================================================
-- SCRIPT 1: CREAR BASE DE DATOS FiducorpERP
-- ============================================================================
-- Descripción: Crea la base de datos principal del ERP
-- Compatible con cualquier instalación de SQL Server Express
-- Las rutas se detectan automáticamente según la configuración del servidor
-- ============================================================================

USE master;
GO

-- Verificar si la base de datos ya existe
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'FiducorpERP')
BEGIN
    PRINT 'La base de datos FiducorpERP ya existe. Eliminándola...';
    
    -- Cerrar todas las conexiones activas
    ALTER DATABASE FiducorpERP SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    
    -- Eliminar la base de datos
    DROP DATABASE FiducorpERP;
    
    PRINT 'Base de datos eliminada exitosamente.';
END
GO

-- Obtener la ruta predeterminada de datos de SQL Server
DECLARE @DataPath NVARCHAR(512);
DECLARE @LogPath NVARCHAR(512);

-- Obtener la ruta de la base de datos master como referencia
SELECT @DataPath = SUBSTRING(physical_name, 1, CHARINDEX(N'master.mdf', LOWER(physical_name)) - 1)
FROM sys.master_files
WHERE database_id = 1 AND file_id = 1;

SET @LogPath = @DataPath;

-- Mostrar las rutas que se usarán
PRINT '=======================================================';
PRINT 'Rutas detectadas automáticamente:';
PRINT 'Data: ' + @DataPath + 'FiducorpERP.mdf';
PRINT 'Log:  ' + @LogPath + 'FiducorpERP_log.ldf';
PRINT '=======================================================';
PRINT '';

-- Crear la base de datos usando rutas dinámicas
DECLARE @SQL NVARCHAR(MAX);
SET @SQL = N'
CREATE DATABASE FiducorpERP
ON PRIMARY
(
    NAME = ''FiducorpERP_Data'',
    FILENAME = ''' + @DataPath + N'FiducorpERP.mdf'',
    SIZE = 50MB,
    MAXSIZE = UNLIMITED,
    FILEGROWTH = 10MB
)
LOG ON
(
    NAME = ''FiducorpERP_Log'',
    FILENAME = ''' + @LogPath + N'FiducorpERP_log.ldf'',
    SIZE = 10MB,
    MAXSIZE = 500MB,
    FILEGROWTH = 5MB
);';

EXEC sp_executesql @SQL;
GO

PRINT '✓ Base de datos FiducorpERP creada exitosamente';
GO

-- Establecer opciones de la base de datos
USE FiducorpERP;
GO

-- Configurar opciones recomendadas
ALTER DATABASE FiducorpERP SET RECOVERY SIMPLE;
ALTER DATABASE FiducorpERP SET AUTO_CLOSE OFF;
ALTER DATABASE FiducorpERP SET AUTO_SHRINK OFF;
GO

PRINT '✓ Configuración de base de datos completada';
PRINT '';
PRINT '=======================================================';
PRINT 'Base de datos: FiducorpERP';
PRINT 'Estado: CREADA Y LISTA';
PRINT 'Siguiente paso: Ejecutar Script 2 - Crear tabla Roles';
PRINT '=======================================================';
GO