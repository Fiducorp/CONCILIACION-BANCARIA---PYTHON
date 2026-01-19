# 📦 MÓDULO CUENTAS POR PAGAR - SCRIPTS DE BASE DE DATOS

## 📋 Descripción

Este directorio contiene todos los scripts SQL necesarios para implementar el módulo **Cuentas por Pagar** del sistema MOFIS-ERP.

---

## 🗂️ Estructura de Archivos

```
Database/
├── 01_Schema/                              # Scripts de estructura (tablas)
│   ├── SCRIPT_12_CrearTablasCatalogosCXP.sql
│   ├── SCRIPT_13_CrearTablasFideicomisosProveedores.sql
│   ├── SCRIPT_14_CrearTablaSolicitudesPago.sql
│   └── SCRIPT_15_CrearTablasFirmasConfiguracion.sql
│
├── 03_Data/                                # Scripts de datos iniciales
│   ├── SCRIPT_08_InsertarDatosCatalogosCXP.sql
│   ├── SCRIPT_09_InsertarConfiguracionCXP.sql
│   └── SCRIPT_10_ActualizarCatalogosCXP.sql
│
├── SCRIPT_MAESTRO_CXP.sql                  # Guía de ejecución
└── README_CXP.md                           # Este archivo
```

---

## 🚀 Orden de Ejecución

### ⚠️ IMPORTANTE: Ejecutar en este orden exacto

| Paso | Script | Descripción |
|------|--------|-------------|
| 1 | `SCRIPT_12_CrearTablasCatalogosCXP.sql` | Crea tablas de catálogos (TiposNCF, Monedas, etc.) |
| 2 | `SCRIPT_13_CrearTablasFideicomisosProveedores.sql` | Crea tablas Fideicomisos y Proveedores |
| 3 | `SCRIPT_14_CrearTablaSolicitudesPago.sql` | Crea tabla principal SolicitudesPago y relacionadas |
| 4 | `SCRIPT_15_CrearTablasFirmasConfiguracion.sql` | Crea tablas de firmas y configuración |
| 5 | `SCRIPT_08_InsertarDatosCatalogosCXP.sql` | Inserta datos de catálogos (NCF, monedas, etc.) |
| 6 | `SCRIPT_09_InsertarConfiguracionCXP.sql` | Inserta configuración del módulo |
| 7 | `SCRIPT_10_ActualizarCatalogosCXP.sql` | Registra formularios y permisos |

---

## 📊 Tablas Creadas

### Catálogos
| Tabla | Descripción | Registros |
|-------|-------------|-----------|
| `TiposNCF` | Tipos de comprobantes fiscales DGII | 22 |
| `Monedas` | Catálogo ISO 4217 | 12 |
| `TiposPago` | Formas de pago | 6 |
| `TiposComprobante` | Tipos de documento | 7 |
| `TiposFideicomiso` | Clasificación de fideicomisos | 6 |
| `MetodosConversion` | Métodos de conversión de moneda | 5 |

### Tablas Principales
| Tabla | Descripción |
|-------|-------------|
| `Fideicomisos` | Maestro de fideicomisos |
| `Proveedores` | Maestro de proveedores |
| `SolicitudesPago` | Solicitudes de pago (70+ columnas) |
| `SolicitudesPagoSubtotales` | Múltiples subtotales por solicitud |
| `SolicitudesPagoComprobantes` | Múltiples NCF por solicitud |
| `SolicitudesPagoAvances` | Historial de avances |

### Configuración
| Tabla | Descripción |
|-------|-------------|
| `FirmasUsuarios` | Firmas digitales de usuarios |
| `ConfiguracionModuloCXP` | Parámetros del módulo |
| `MemoriaTemporalFormularios` | Guardado temporal sin guardar |

---

## 🔐 Permisos por Rol

| Rol | Permisos en CXP |
|-----|-----------------|
| **ROOT** | Acceso total |
| **ADMIN** | Acceso total |
| **CONTADOR** | Ver todo, CRUD en propios, imprimir, exportar |
| **GERENTE** | Solo consulta e impresión |
| **ANALISTA** | Solo consulta |

---

## ✅ Verificación Post-Instalación

Ejecute estas consultas para verificar la instalación:

```sql
-- Verificar tablas creadas
SELECT name FROM sys.tables 
WHERE name LIKE '%NCF%' OR name LIKE '%Solicitud%' 
   OR name LIKE '%Fideicomiso%' OR name LIKE '%Proveedor%'
ORDER BY name;

-- Verificar datos de catálogos
SELECT 'TiposNCF' AS Tabla, COUNT(*) AS Registros FROM TiposNCF
UNION ALL SELECT 'Monedas', COUNT(*) FROM Monedas
UNION ALL SELECT 'ConfiguracionModuloCXP', COUNT(*) FROM ConfiguracionModuloCXP;

-- Verificar formularios registrados
SELECT CodigoFormulario, NombreFormulario 
FROM CatalogoFormularios 
WHERE ModuloID = (SELECT ModuloID FROM CatalogoModulos WHERE CodigoModulo = 'CXP');
```

---

## 📝 Notas Importantes

1. **Prerequisitos**: La base de datos `FiducorpERP` debe existir con las tablas base (Usuarios, Roles, CatalogoModulos, etc.)

2. **Backup**: Se recomienda hacer backup antes de ejecutar los scripts

3. **Errores**: Si un script falla, NO continúe con el siguiente. Corrija el error primero.

4. **Re-ejecución**: Los scripts están diseñados para ser idempotentes (se pueden ejecutar múltiples veces sin duplicar datos)

---

## 📅 Historial de Cambios

| Fecha | Versión | Descripción |
|-------|---------|-------------|
| 2026-01-17 | 1.0 | Versión inicial del módulo CXP |

---

**MOFIS-ERP** - Sistema de Gestión Empresarial para Fiducorp
