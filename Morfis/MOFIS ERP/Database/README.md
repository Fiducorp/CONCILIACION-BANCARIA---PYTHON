# Scripts de Base de Datos - MOFIS ERP

## Orden de ejecución inicial

1. `01_Schema/SCRIPT_01_CrearBaseDatos.sql`
2. `01_Schema/SCRIPT_02_CrearTablaRoles.sql`
3. `01_Schema/SCRIPT_03_CrearTablaUsuarios.sql`
4. `02_Data/SCRIPT_04_InsertarDatosIniciales.sql`

## Updates aplicados

| # | Archivo | Fecha | Descripción |
|---|---------|-------|-------------|
| 001 | UPDATE_001_AgregarColumnasEliminacion.sql | 2024-12-25 | Columnas para eliminación lógica de usuarios |

## Notas importantes

- Los scripts de UPDATE verifican si los cambios ya existen antes de aplicarlos
- Siempre ejecutar en orden secuencial
- Hacer backup antes de aplicar updates en producción