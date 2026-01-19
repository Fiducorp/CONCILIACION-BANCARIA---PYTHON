# 🔍 ARQUITECTURA: SISTEMA DE BÚSQUEDA AVANZADA DE AUDITORÍA
## MOFIS-ERP | Versión 1.0 | 2026-01-10

---

## 📋 ÍNDICE

1. [Visión General](#visión-general)
2. [Estructura de la Base de Datos](#estructura-de-la-base-de-datos)
3. [Arquitectura del Sistema](#arquitectura-del-sistema)
4. [Modos de Búsqueda](#modos-de-búsqueda)
5. [Componentes del Sistema](#componentes-del-sistema)
6. [Flujo de Datos](#flujo-de-datos)
7. [Optimizaciones de Rendimiento](#optimizaciones-de-rendimiento)
8. [Seguridad y Auditoría](#seguridad-y-auditoría)

---

## 🎯 VISIÓN GENERAL

### Objetivo
Proporcionar un sistema avanzado y completo de búsqueda, análisis y reporte de auditoría que permita a los administradores rastrear cualquier actividad en el sistema con precisión quirúrgica.

### Características Principales
- ✅ 7 Modos de Búsqueda Especializados
- ✅ Interfaz moderna con TabControl
- ✅ Análisis estadísticos en tiempo real
- ✅ Exportación avanzada a Excel y PDF
- ✅ Agrupaciones inteligentes y vistas múltiples
- ✅ Timeline cronológico de sesiones
- ✅ Detección de patrones y anomalías

---

## 🗄️ ESTRUCTURA DE LA BASE DE DATOS

### Tabla: Auditoria

```sql
CREATE TABLE dbo.Auditoria (
    AuditoriaID      BIGINT IDENTITY(1,1) PRIMARY KEY,
    UsuarioID        INT NOT NULL,               -- FK → Usuarios
    Accion           NVARCHAR(50) NOT NULL,      -- Acción realizada
    Categoria        NVARCHAR(50) NULL,          -- Categoría del sistema
    Modulo           NVARCHAR(50) NULL,          -- Módulo específico
    Formulario       NVARCHAR(100) NULL,         -- Formulario donde ocurrió
    RegistroID       INT NULL,                   -- ID del registro afectado
    Detalle          NVARCHAR(MAX) NULL,         -- Información detallada
    FechaHora        DATETIME NOT NULL,          -- Timestamp de la acción
    DireccionIP      NVARCHAR(50) NULL,          -- IP origen
    NombreMaquina    NVARCHAR(100) NULL          -- Nombre de PC
);
```

### Índices Optimizados
- `IX_Auditoria_UsuarioID` - Para búsquedas por usuario
- `IX_Auditoria_FechaHora` - Para búsquedas temporales (DESC)
- `IX_Auditoria_Accion` - Para búsquedas por acción
- `IX_Auditoria_Modulo` - Para búsquedas por módulo
- `IX_Auditoria_Usuario_Fecha` - Índice compuesto con INCLUDE
- `IX_Auditoria_RegistroID` - Para rastreo de cambios

**Nota:** Los índices existentes son suficientes. No se requieren modificaciones en la base de datos.

---

## 🏗️ ARQUITECTURA DEL SISTEMA

### Estructura de Archivos

```
Forms/Sistema/Auditoria/
├── FormBusquedaAvanzadaAuditoria.cs          # Formulario principal
├── FormBusquedaAvanzadaAuditoria.Designer.cs # Diseño visual
├── FormBusquedaAvanzadaAuditoria.resx        # Recursos
├── BusquedaAvanzadaHelper.cs                 # Helper con consultas SQL
├── ResultadoBusquedaDTO.cs                   # DTO de resultados
├── EstadisticasBusquedaDTO.cs                # DTO de estadísticas
└── ARQUITECTURA_BUSQUEDA_AVANZADA.md         # Este documento
```

### Diagrama de Componentes

```
┌─────────────────────────────────────────────┐
│   FormBusquedaAvanzadaAuditoria (Principal) │
└─────────────────┬───────────────────────────┘
                  │
        ┌─────────┴─────────┐
        │   TabControl      │
        │   (7 TabPages)    │
        └─────────┬─────────┘
                  │
    ┌─────────────┼─────────────┐
    │             │             │
┌───▼───┐  ┌──────▼──────┐  ┌──▼────┐
│ MODO 1│  │   MODO 2    │  │ MODO  │
│Usuario│  │  Categoría  │  │  3-7  │
└───┬───┘  └──────┬──────┘  └───┬───┘
    │             │             │
    └─────────────┼─────────────┘
                  │
    ┌─────────────▼──────────────┐
    │ BusquedaAvanzadaHelper     │
    │ (Consultas SQL optimizadas)│
    └─────────────┬──────────────┘
                  │
    ┌─────────────▼──────────────┐
    │   Procesamiento de Datos   │
    │   • Agrupaciones           │
    │   • Estadísticas           │
    │   • Análisis de tendencias │
    └─────────────┬──────────────┘
                  │
    ┌─────────────┴──────────────┐
    │                            │
┌───▼──────┐          ┌─────────▼────┐
│ Exportar │          │ Visualización│
│Excel/PDF │          │  DataGridView│
└──────────┘          └──────────────┘
```

---

## 🔍 MODOS DE BÚSQUEDA

### MODO 1: Rastrear TODO lo que hizo un usuario

**Propósito:** Ver cronológicamente todas las acciones de un usuario específico.

**Filtros Disponibles:**
- ✅ Usuario (ComboBox con búsqueda incremental)
- ✅ Rango de fechas (Desde - Hasta)
- ✅ Día específico (opcional)
- ✅ Acción (opcional - filtrar tipo de acción)
- ✅ Categoría (opcional)
- ✅ Módulo (opcional)
- ✅ Formulario (opcional)

**Resultados:**
- Timeline cronológico de acciones
- Agrupación por sesión (LOGIN → acciones → LOGOUT)
- Total de acciones en el periodo
- Duración promedio de sesiones

**Análisis Avanzado:**
```
┌─────────────────────────────────────────┐
│ RESUMEN ANALÍTICO DEL USUARIO          │
├─────────────────────────────────────────┤
│ • Acciones más frecuentes (Top 10)     │
│ • Módulos más utilizados               │
│ • Horarios de actividad (gráfico)     │
│ • Promedio de acciones por sesión      │
│ • Días más activos                     │
│ • Comparativa con otros usuarios       │
└─────────────────────────────────────────┘
```

**Vista de Sesiones:**
```
📅 2026-01-10
  ┌─ SESIÓN 1: 08:15:23 - 12:45:10 (4h 30m)
  │  ├─ LOGIN desde 192.168.1.100 (PC-ADMIN-01)
  │  ├─ CONSULTAR_AUDITORIA (3 veces)
  │  ├─ MODIFICAR_USUARIO (Juan Pérez)
  │  ├─ CREAR_ROL (Supervisor)
  │  └─ LOGOUT
  │
  └─ SESIÓN 2: 14:00:05 - 17:30:22 (3h 30m)
     ├─ LOGIN desde 192.168.1.100 (PC-ADMIN-01)
     ├─ EXPORTAR_AUDITORIA_EXCEL (2 veces)
     └─ LOGOUT
```

### MODO 2: Rastrear acciones en una CATEGORÍA

**Propósito:** Analizar toda la actividad dentro de una categoría del sistema.

**Filtros Disponibles:**
- ✅ Categoría (ComboBox)
- ✅ Rango de fechas
- ✅ Usuario (opcional)
- ✅ Módulo dentro de la categoría (cascada)
- ✅ Tipo de acción (opcional)

**Resultados:**
- Agrupación por módulo dentro de la categoría
- Vista de árbol: Categoría → Módulos → Formularios → Acciones
- Vista de lista: Flat con todas las acciones

**Análisis Avanzado:**
```
┌─────────────────────────────────────────┐
│ ANÁLISIS DE CATEGORÍA: SISTEMA         │
├─────────────────────────────────────────┤
│ Total de acciones: 1,247               │
│                                         │
│ MÓDULOS MÁS ACTIVOS:                   │
│  1. Gestión de Usuarios    (45.2%)     │
│  2. Gestión de Roles       (32.1%)     │
│  3. Auditoría General      (22.7%)     │
│                                         │
│ USUARIOS PARTICIPANTES:                │
│  • admin (520 acciones)                │
│  • supervisor1 (412 acciones)          │
│  • auditor (315 acciones)              │
│                                         │
│ TENDENCIAS:                            │
│  • Actividad mayor: Lunes 9-12h        │
│  • Pico de actividad: 10:30 AM         │
│  • Acción más común: CONSULTAR         │
└─────────────────────────────────────────┘
```

### MODO 3: Rastrear acciones en MÓDULO/SUBMÓDULO

**Propósito:** Análisis detallado de un módulo específico.

**Filtros Disponibles:**
- ✅ Categoría (cascada)
- ✅ Módulo (cascada)
- ✅ Formulario específico (opcional)
- ✅ Rango de fechas
- ✅ Usuario (opcional)
- ✅ Tipo de acción (opcional)

**Agrupaciones:**
- Por formulario
- Por usuario
- Por tipo de acción (CREATE, EDIT, DELETE, VIEW)
- Por día/hora

**Estadísticas:**
```
Acciones por Tipo:
  CREATE: ████████░░ 35%
  EDIT:   ██████████ 42%
  DELETE: ██░░░░░░░░  8%
  VIEW:   ███░░░░░░░ 15%
```

### MODO 4: Rastrear una ACCIÓN específica

**Propósito:** Análisis forense de una acción particular.

**Filtros Disponibles:**
- ✅ Acción específica (ComboBox con todas las acciones)
- ✅ Categoría (opcional)
- ✅ Módulo (opcional)
- ✅ Formulario (opcional)
- ✅ Rango de fechas
- ✅ Usuario (opcional)

**Análisis de Frecuencia:**
```
┌─────────────────────────────────────────┐
│ ANÁLISIS: MODIFICAR_USUARIO            │
├─────────────────────────────────────────┤
│ Total de ejecuciones: 234              │
│ Periodo: 01/12/2025 - 10/01/2026       │
│                                         │
│ USUARIOS QUE LA EJECUTAN:              │
│  1. admin         (145 veces - 62%)    │
│  2. supervisor1   ( 52 veces - 22%)    │
│  3. rrhh_manager  ( 37 veces - 16%)    │
│                                         │
│ HORARIOS MÁS COMUNES:                  │
│  09:00 - 12:00 ████████████░ 65%       │
│  14:00 - 17:00 ██████░░░░░░░ 30%       │
│  17:00 - 20:00 █░░░░░░░░░░░░  5%       │
│                                         │
│ TENDENCIA:                             │
│  📈 Aumentando (+12% vs mes anterior)  │
│                                         │
│ DÍAS DE LA SEMANA:                     │
│  Lunes    ████████ 32%                 │
│  Martes   ██████   24%                 │
│  Miércoles█████    20%                 │
│  Jueves   ████     16%                 │
│  Viernes  ██        8%                 │
└─────────────────────────────────────────┘
```

### MODO 5: Rastrear por RANGO HORARIO

**Propósito:** Detectar actividad en horarios específicos (especialmente fuera de horario laboral).

**Filtros Disponibles:**
- ✅ Fecha (selector de día)
- ✅ Hora inicio (HH:mm)
- ✅ Hora fin (HH:mm)
- ✅ Usuario (opcional)
- ✅ Tipo de acción (opcional)

**Casos de Uso:**
- 🔍 "¿Qué pasó entre 18:00 y 22:00 del 15 de diciembre?"
- 🔍 Detectar accesos fuera de horario laboral
- 🔍 Análisis de turnos nocturnos

**Alertas de Seguridad:**
```
⚠️  ACTIVIDAD FUERA DE HORARIO DETECTADA
─────────────────────────────────────────
Horario: 22:45:12 - 23:15:34
Usuario: supervisor1
Acciones: 12
Módulos accedidos: Gestión de Usuarios, Roles
IPs: 192.168.1.250 (no habitual)

Recomendación: Revisar actividad
```

### MODO 6: Rastrear por IP/MÁQUINA

**Propósito:** Rastrear origen de las acciones (seguridad).

**Filtros Disponibles:**
- ✅ IP específica (TextBox con validación)
- ✅ Nombre de máquina (TextBox con autocompletado)
- ✅ Rango de fechas
- ✅ Usuario (opcional)
- ✅ Tipo de acción (opcional)

**Análisis de Seguridad:**
```
┌─────────────────────────────────────────┐
│ ANÁLISIS DE IP: 192.168.1.100          │
├─────────────────────────────────────────┤
│ Máquina: PC-ADMIN-01                   │
│ Total acciones: 1,520                  │
│                                         │
│ USUARIOS DESDE ESTA IP:                │
│  • admin (1,200 acciones - normal)     │
│  • supervisor1 (250 acciones)          │
│  ⚠️ usuario_temp (70 acciones - ALERTA)│
│                                         │
│ PATRÓN DE USO:                         │
│  Lunes a Viernes: 08:00 - 18:00       │
│  ⚠️ Detección: Acceso Sábado 02:30 AM  │
│                                         │
│ ALERTAS:                               │
│  ⚠️ Múltiples usuarios desde misma IP  │
│  ℹ️  IP dentro del rango corporativo   │
└─────────────────────────────────────────┘
```

**Casos de Uso:**
- 🔍 Detectar accesos no autorizados
- 🔍 Rastrear uso de IPs sospechosas
- 🔍 Auditar uso compartido de cuentas

### MODO 7: Rastrear CAMBIOS en REGISTRO específico

**Propósito:** Ver historial completo de un registro particular.

**Filtros Disponibles:**
- ✅ RegistroID (TextBox numérico)
- ✅ Módulo/Tabla (ComboBox)
- ✅ Rango de fechas (opcional)

**Timeline de Cambios:**
```
📋 HISTORIAL DEL REGISTRO: UsuarioID = 245
─────────────────────────────────────────────

📅 15/12/2025 09:30:15 | CREAR_USUARIO
   Usuario: admin
   Detalle: Usuario creado con rol "Empleado"
   Valores iniciales:
     - Username: jperez
     - Nombre: Juan Pérez
     - Email: jperez@empresa.com
     - Estado: Activo

     ↓

📅 18/12/2025 14:22:40 | MODIFICAR_USUARIO
   Usuario: admin
   Cambios:
     - Rol: Empleado → Supervisor
     - Email: jperez@empresa.com → juan.perez@empresa.com

     ↓

📅 20/12/2025 10:15:33 | MODIFICAR_PERMISOS_USUARIO
   Usuario: admin
   Detalle: Agregados permisos especiales
     + Módulo: Reportes Gerenciales

     ↓

📅 05/01/2026 16:45:12 | DESACTIVAR_USUARIO
   Usuario: rrhh_manager
   Detalle: Usuario desactivado (fin de contrato)
     - Estado: Activo → Inactivo

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
RESUMEN:
  • Creado: 15/12/2025
  • Total de modificaciones: 3
  • Último cambio: 05/01/2026
  • Estado actual: Inactivo
  • Modificado por: admin (2), rrhh_manager (1)
```

---

## 🧩 COMPONENTES DEL SISTEMA

### 1. FormBusquedaAvanzadaAuditoria.cs

**Responsabilidades:**
- Gestionar TabControl con 7 tabs
- Coordinar filtros dinámicos
- Mostrar resultados en DataGridView
- Generar estadísticas en tiempo real
- Coordinar exportaciones

**Características:**
- ✅ Formulario MDI Child
- ✅ Diseño moderno con colores corporativos
- ✅ Responsive y optimizado
- ✅ Validación de filtros
- ✅ Caché inteligente

### 2. BusquedaAvanzadaHelper.cs

**Responsabilidades:**
- Consultas SQL optimizadas para cada modo
- Métodos de análisis estadístico
- Agrupaciones y agregaciones
- Detección de patrones

**Métodos Principales:**
```csharp
// MODO 1
public static DataTable BuscarPorUsuario(int usuarioID, DateTime? desde, DateTime? hasta, ...);
public static List<SesionUsuario> ObtenerSesionesUsuario(int usuarioID, ...);
public static EstadisticasUsuario ObtenerEstadisticasUsuario(int usuarioID, ...);

// MODO 2
public static DataTable BuscarPorCategoria(string categoria, ...);
public static List<ResumenModulo> ObtenerResumenModulos(string categoria, ...);

// MODO 3
public static DataTable BuscarPorModulo(string modulo, ...);
public static Dictionary<string, int> ObtenerEstadisticasAcciones(string modulo, ...);

// MODO 4
public static DataTable BuscarPorAccion(string accion, ...);
public static TendenciaAccion AnalizarTendencia(string accion, ...);

// MODO 5
public static DataTable BuscarPorRangoHorario(DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin, ...);

// MODO 6
public static DataTable BuscarPorIP(string ip, ...);
public static DataTable BuscarPorMaquina(string nombreMaquina, ...);
public static List<AlertaSeguridad> DetectarAnomalias(string ip, ...);

// MODO 7
public static DataTable BuscarPorRegistroID(int registroID, string modulo, ...);
public static List<CambioRegistro> ObtenerTimelineCambios(int registroID, ...);
```

### 3. DTOs (Data Transfer Objects)

**ResultadoBusquedaDTO.cs:**
```csharp
public class ResultadoBusquedaDTO
{
    public DataTable Datos { get; set; }
    public int TotalRegistros { get; set; }
    public DateTime FechaConsulta { get; set; }
    public Dictionary<string, object> Metadatos { get; set; }
}
```

**EstadisticasBusquedaDTO.cs:**
```csharp
public class EstadisticasBusquedaDTO
{
    public int TotalAcciones { get; set; }
    public int TotalUsuarios { get; set; }
    public Dictionary<string, int> AccionesPorTipo { get; set; }
    public Dictionary<string, int> AccionesPorModulo { get; set; }
    public Dictionary<string, int> AccionesPorHora { get; set; }
    public List<TopUsuario> TopUsuarios { get; set; }
    public TendenciaDTO Tendencia { get; set; }
}
```

---

## 🔄 FLUJO DE DATOS

### Flujo General de Búsqueda

```
┌─────────────────┐
│  Usuario        │
│  Selecciona Modo│
└────────┬────────┘
         │
         ▼
┌─────────────────────────┐
│ TabControl              │
│ Activa Tab específico   │
└────────┬────────────────┘
         │
         ▼
┌─────────────────────────┐
│ Usuario configura       │
│ filtros específicos     │
└────────┬────────────────┘
         │
         ▼
┌─────────────────────────┐
│ Validar filtros         │
│ ¿Son válidos?           │
└────┬────────────────┬───┘
     │ NO             │ SÍ
     ▼                ▼
  [Error]   ┌──────────────────┐
            │ BusquedaHelper   │
            │ Ejecuta consulta │
            └────────┬─────────┘
                     │
                     ▼
            ┌──────────────────┐
            │ Procesar datos   │
            │ • Agrupar        │
            │ • Calcular stats │
            └────────┬─────────┘
                     │
                     ▼
            ┌──────────────────┐
            │ Mostrar en Grid  │
            └────────┬─────────┘
                     │
                     ▼
            ┌──────────────────┐
            │ Generar análisis │
            │ • Estadísticas   │
            │ • Gráficos       │
            │ • Resúmenes      │
            └────────┬─────────┘
                     │
                     ▼
            ┌──────────────────┐
            │ Habilitar Export │
            │ • Excel          │
            │ • PDF            │
            └──────────────────┘
```

---

## ⚡ OPTIMIZACIONES DE RENDIMIENTO

### 1. Consultas SQL
- ✅ Uso de índices existentes
- ✅ WITH (NOLOCK) para lecturas
- ✅ Paginación cuando sea necesario
- ✅ INCLUDE en índices compuestos

### 2. Caché
- ✅ Caché de listas de usuarios
- ✅ Caché de catálogos (módulos, acciones, categorías)
- ✅ Caché de últimas búsquedas
- ✅ Duración: 10 minutos

### 3. UI
- ✅ Carga asíncrona con async/await
- ✅ SuspendLayout/ResumeLayout para DataGridView
- ✅ BindingSource con RaiseListChangedEvents = false
- ✅ Virtualización de datos grandes

### 4. Límites
- ✅ Advertencia si resultados > 10,000 registros
- ✅ Opción de paginación para grandes volúmenes
- ✅ Timeout de 60 segundos para consultas

---

## 🔒 SEGURIDAD Y AUDITORÍA

### Registro de Uso
Cada búsqueda se registra en auditoría:

```
BUSQUEDA_AVANZADA_MODO_1
BUSQUEDA_AVANZADA_MODO_2
...
BUSQUEDA_AVANZADA_MODO_7
```

### Permisos
- ✅ Solo usuarios con permiso "CONSULTAR_AUDITORIA"
- ✅ Restricción por roles
- ✅ No se permite modificar/eliminar desde búsqueda

### Privacidad
- ✅ No mostrar passwords en detalles
- ✅ Ofuscar datos sensibles si es necesario

---

## 📊 EXPORTACIÓN

### Excel
- ✅ Múltiples hojas según modo
- ✅ Formato profesional con ClosedXML
- ✅ Gráficos y estadísticas
- ✅ Filtros automáticos

### PDF
- ✅ Diseño profesional con iTextSharp
- ✅ Portada personalizada
- ✅ Índice de contenidos
- ✅ Gráficos vectoriales

---

## 📝 NOTAS DE IMPLEMENTACIÓN

### Prioridades
1. ✅ Funcionalidad completa de los 7 modos
2. ✅ Rendimiento optimizado
3. ✅ UX/UI moderna y profesional
4. ✅ Exportación robusta
5. ✅ Documentación completa

### Tecnologías
- C# .NET Framework 4.7.2+
- Windows Forms
- SQL Server
- ClosedXML (Excel)
- iTextSharp (PDF)

### Compatibilidad
- ✅ Compatible con FormMain MDI
- ✅ Integración con módulo de auditoría existente
- ✅ Sin cambios en base de datos
- ✅ Reutiliza clases helper existentes

---

## 🎨 DISEÑO UI

### Paleta de Colores
- **Corporativo:** RGB(0, 120, 212) - #0078D4
- **Verde:** RGB(34, 139, 34) - #228B22
- **Rojo:** RGB(220, 53, 69) - #DC3545
- **Naranja:** RGB(255, 152, 0) - #FF9800
- **Morado:** RGB(156, 39, 176) - #9C27B0
- **Gris:** RGB(108, 117, 125) - #6C757D

### Tipografía
- **Principal:** Segoe UI
- **Títulos:** Segoe UI Bold, 14pt
- **Texto:** Segoe UI Regular, 10pt
- **Código:** Consolas, 9pt

---

## ✅ CHECKLIST DE DESARROLLO

- [x] Análisis de base de datos
- [x] Diseño de arquitectura
- [ ] Implementación de BusquedaAvanzadaHelper
- [ ] Implementación de DTOs
- [ ] Diseño de FormBusquedaAvanzadaAuditoria.Designer
- [ ] Implementación de MODO 1
- [ ] Implementación de MODO 2
- [ ] Implementación de MODO 3
- [ ] Implementación de MODO 4
- [ ] Implementación de MODO 5
- [ ] Implementación de MODO 6
- [ ] Implementación de MODO 7
- [ ] Sistema de exportación Excel
- [ ] Sistema de exportación PDF
- [ ] Integración con FormAuditoria
- [ ] Pruebas exhaustivas
- [ ] Documentación de usuario

---

## 📞 SOPORTE

Para dudas o sugerencias sobre este módulo:
- **Desarrollador:** Claude Code
- **Fecha:** 2026-01-10
- **Versión:** 1.0

---

**FIN DEL DOCUMENTO DE ARQUITECTURA**
