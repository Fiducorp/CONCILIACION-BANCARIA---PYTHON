# 🔍 MANUAL DE USUARIO: BÚSQUEDA AVANZADA DE AUDITORÍA
## MOFIS-ERP | Versión 1.0

---

## 📋 TABLA DE CONTENIDOS

1. [Introducción](#introducción)
2. [Acceso al Módulo](#acceso-al-módulo)
3. [Interfaz General](#interfaz-general)
4. [Modo 1: Rastrear Usuario](#modo-1-rastrear-usuario)
5. [Modo 2: Rastrear Categoría](#modo-2-rastrear-categoría)
6. [Modo 3: Rastrear Módulo](#modo-3-rastrear-módulo)
7. [Modo 4: Rastrear Acción](#modo-4-rastrear-acción)
8. [Modo 5: Rastrear Rango Horario](#modo-5-rastrear-rango-horario)
9. [Modo 6: Rastrear IP/Máquina](#modo-6-rastrear-ipmáquina)
10. [Modo 7: Rastrear Registro](#modo-7-rastrear-registro)
11. [Exportación de Datos](#exportación-de-datos)
12. [Casos de Uso Comunes](#casos-de-uso-comunes)
13. [Preguntas Frecuentes](#preguntas-frecuentes)

---

## 📖 INTRODUCCIÓN

El módulo de **Búsqueda Avanzada de Auditoría** le permite realizar investigaciones profundas y especializadas sobre la actividad registrada en el sistema ERP. Con 7 modos de búsqueda diferentes, puede rastrear cualquier actividad desde múltiples perspectivas.

### ¿Para qué sirve?

- ✅ Auditar actividad de usuarios específicos
- ✅ Investigar cambios en módulos o categorías
- ✅ Detectar patrones anómalos de uso
- ✅ Rastrear modificaciones a registros importantes
- ✅ Generar reportes detallados para gerencia
- ✅ Cumplir con requisitos de compliance
- ✅ Análisis forense de seguridad

---

## 🚪 ACCESO AL MÓDULO

### Desde el Formulario de Auditoría General

1. Navegue a **Sistema → Auditoría**
2. Haga clic en el botón **🔍 Búsqueda Avanzada** (color naranja)
3. Se abrirá el formulario de búsqueda avanzada

### Permisos Requeridos

- **Permiso:** `CONSULTAR_AUDITORIA`
- **Roles autorizados:** Administrador, Auditor, Gerencia

---

## 🖥️ INTERFAZ GENERAL

### Componentes Principales

```
┌────────────────────────────────────────────────────┐
│ 🔍 BÚSQUEDA AVANZADA DE AUDITORÍA       [← Volver] │
├────────────────────────────────────────────────────┤
│ [👤 Por Usuario] [📂 Por Categoría] [🗂️ Por Módu...│
│                                                     │
│ ┌──────────────────────────────────────────────┐  │
│ │ FILTROS DE BÚSQUEDA                          │  │
│ └──────────────────────────────────────────────┘  │
│                                                     │
│ ┌──────────────────────────────────────────────┐  │
│ │ RESULTADOS (DataGridView)                    │  │
│ │                                               │  │
│ └──────────────────────────────────────────────┘  │
│                                                     │
├────────────────────────────────────────────────────┤
│ Total: X registros        [📊 Excel] [📄 PDF] [🗑️]│
└────────────────────────────────────────────────────┘
```

### Botones Globales

| Botón | Función |
|-------|---------|
| **← Volver** | Regresa al formulario de auditoría general |
| **📊 Exportar Excel** | Exporta los resultados a archivo Excel |
| **📄 Exportar PDF** | Exporta los resultados a archivo PDF (próximamente) |
| **🗑️ Limpiar** | Limpia filtros y resultados del modo actual |

---

## 👤 MODO 1: RASTREAR USUARIO

### Objetivo
Ver cronológicamente **TODAS las acciones** de un usuario específico.

### Filtros Disponibles

| Campo | Tipo | Descripción | Requerido |
|-------|------|-------------|-----------|
| Usuario | ComboBox | Usuario a rastrear | ✅ Sí |
| Búsqueda rápida | TextBox | Filtrar usuarios por nombre | ❌ No |
| Desde | Fecha | Fecha inicio del rango | ❌ No |
| Hasta | Fecha | Fecha fin del rango | ❌ No |
| Acción | ComboBox | Filtrar por acción específica | ❌ No |
| Categoría | ComboBox | Filtrar por categoría | ❌ No |
| Módulo | ComboBox | Filtrar por módulo | ❌ No |
| Formulario | ComboBox | Filtrar por formulario | ❌ No |
| Agrupar sesiones | Checkbox | Agrupar por LOGIN/LOGOUT | ❌ No |

### Cómo usar

1. Seleccione el **usuario** del ComboBox o use la búsqueda rápida
2. (Opcional) Configure el rango de fechas
3. (Opcional) Aplique filtros adicionales
4. Haga clic en **🔍 BUSCAR**

### Resultados

**Vista de Lista:**
- Timeline cronológico de todas las acciones
- Ordenado por fecha/hora ascendente
- Incluye IP y máquina de origen

**Panel de Estadísticas (derecha):**
```
═══════════════════════════════════════
  📊 ESTADÍSTICAS DEL USUARIO
═══════════════════════════════════════

Usuario: Juan Pérez
Username: jperez
Total de acciones: 1,247
Total de sesiones: 45
Duración promedio: 3h 25m

Primera acción: 01/12/2025 08:15
Última acción: 10/01/2026 17:30

─────────────────────────────────────
📌 ACCIONES MÁS FRECUENTES:
─────────────────────────────────────
1. CONSULTAR_AUDITORIA
   324 veces (26.0%)
   ████████████████████░

2. MODIFICAR_USUARIO
   198 veces (15.9%)
   ████████████░░░░░░░░

...
```

### Casos de uso

✅ "¿Qué hizo Juan Pérez el 15 de diciembre?"
✅ "Mostrar todas las modificaciones de usuarios realizadas por el administrador"
✅ "Ver el historial completo de actividad de un usuario específico"

---

## 📂 MODO 2: RASTREAR CATEGORÍA

### Objetivo
Analizar **toda la actividad** dentro de una categoría del sistema.

### Filtros Disponibles

| Campo | Tipo | Descripción | Requerido |
|-------|------|-------------|-----------|
| Categoría | ComboBox | Categoría a analizar | ✅ Sí |
| Desde | Fecha | Fecha inicio | ❌ No |
| Hasta | Fecha | Fecha fin | ❌ No |
| Usuario | ComboBox | Filtrar por usuario | ❌ No |
| Módulo | ComboBox | Módulo dentro de categoría | ❌ No |
| Acción | ComboBox | Tipo de acción | ❌ No |

### Cómo usar

1. Seleccione la **categoría** (ej: SISTEMA, CONTABILIDAD, etc.)
2. Configure fechas y filtros adicionales
3. Haga clic en **🔍 BUSCAR**

### Resultados

**Panel de Estadísticas:**
```
═══════════════════════════════════════
  📂 ANÁLISIS: SISTEMA
═══════════════════════════════════════

Total de acciones: 3,456
Total de módulos: 8
Usuarios participantes: 12

─────────────────────────────────────
🏆 MÓDULOS MÁS ACTIVOS:
─────────────────────────────────────
1. Gestión de Usuarios
   1,562 acciones (45.2%)
   5 usuarios
   ████████████████████
   Última: 10/01 14:30

2. Gestión de Roles
   1,109 acciones (32.1%)
   4 usuarios
   ██████████████░░░░░░
   Última: 10/01 12:15

...
```

### Casos de uso

✅ "Generar reporte de actividad en la categoría CONTABILIDAD"
✅ "¿Qué módulos se usan más en GERENCIA FINANCIERA?"
✅ "Auditar toda la actividad en SISTEMA durante diciembre"

---

## 🗂️ MODO 3: RASTREAR MÓDULO

### Objetivo
Análisis **detallado** de un módulo específico.

### Filtros Disponibles

| Campo | Tipo | Descripción | Requerido |
|-------|------|-------------|-----------|
| Categoría | ComboBox | Categoría (cascada) | ❌ No |
| Módulo | ComboBox | Módulo a analizar | ✅ Sí |
| Formulario | ComboBox | Formulario específico | ❌ No |
| Desde/Hasta | Fechas | Rango temporal | ❌ No |
| Usuario | ComboBox | Usuario específico | ❌ No |
| Acción | ComboBox | Tipo de acción | ❌ No |

### Cómo usar

1. (Opcional) Seleccione la categoría
2. Seleccione el **módulo**
3. Configure filtros adicionales
4. Haga clic en **🔍 BUSCAR**

### Resultados

**Panel de Estadísticas:**
```
═══════════════════════════════════════
  🗂️ ANÁLISIS: Gestión de Usuarios
═══════════════════════════════════════

Total de acciones: 856
Usuarios activos: 5

─────────────────────────────────────
📊 ACCIONES POR TIPO:
─────────────────────────────────────
1. MODIFICAR_USUARIO
   360 veces (42.1%)
   ████████████████████

2. CREAR_USUARIO
   299 veces (34.9%)
   █████████████████░░░

3. CONSULTAR_USUARIO
   128 veces (15.0%)
   ███████░░░░░░░░░░░░░

...
```

### Casos de uso

✅ "Auditar el módulo de Cuentas por Pagar"
✅ "¿Cuántas veces se modificó la configuración de nómina?"
✅ "Ver actividad en Solicitudes de Pago durante el último mes"

---

## 🎯 MODO 4: RASTREAR ACCIÓN

### Objetivo
Análisis **forense** de una acción particular.

### Filtros Disponibles

| Campo | Tipo | Descripción | Requerido |
|-------|------|-------------|-----------|
| Acción | ComboBox | Acción a rastrear | ✅ Sí |
| Categoría | ComboBox | Categoría | ❌ No |
| Módulo | ComboBox | Módulo | ❌ No |
| Formulario | ComboBox | Formulario | ❌ No |
| Desde/Hasta | Fechas | Rango temporal | ❌ No |
| Usuario | ComboBox | Usuario específico | ❌ No |

### Cómo usar

1. Seleccione la **acción** (ej: MODIFICAR_USUARIO, CREAR_FACTURA, etc.)
2. Configure filtros contextuales
3. Haga clic en **🔍 BUSCAR**

### Resultados

**Panel de Estadísticas:**
```
═══════════════════════════════════════
  🎯 ANÁLISIS: MODIFICAR_USUARIO
═══════════════════════════════════════

Total ejecuciones: 234
Periodo: 01/12/2025 - 10/01/2026

─────────────────────────────────────
📈 TENDENCIA:
─────────────────────────────────────
Periodo anterior: 198
Cambio: 18.2%
Dirección: 📈 Aumentando

─────────────────────────────────────
👥 USUARIOS QUE LA EJECUTAN:
─────────────────────────────────────
1. admin
   145 veces (62.0%)
   ████████████████████

2. supervisor1
   52 veces (22.2%)
   ███████░░░░░░░░░░░░░

...

─────────────────────────────────────
📅 DÍA DE LA SEMANA:
─────────────────────────────────────
Lunes: 75 (32.1%)
Martes: 56 (23.9%)
Miércoles: 47 (20.1%)
...
```

### Casos de uso

✅ "¿Quién está ejecutando ELIMINAR_FACTURA y cuántas veces?"
✅ "Analizar tendencia de CREAR_PROVEEDOR (¿aumenta o disminuye?)"
✅ "¿En qué días se ejecuta más APROBAR_SOLICITUD?"

---

## 🕐 MODO 5: RASTREAR RANGO HORARIO

### Objetivo
Detectar actividad en **horarios específicos** (útil para detectar accesos fuera de horario).

### Filtros Disponibles

| Campo | Tipo | Descripción | Requerido |
|-------|------|-------------|-----------|
| Fecha | Fecha | Día específico | ✅ Sí |
| Hora Inicio | Hora | Hora de inicio (HH:mm) | ✅ Sí |
| Hora Fin | Hora | Hora de fin (HH:mm) | ✅ Sí |
| Usuario | ComboBox | Usuario específico | ❌ No |
| Acción | ComboBox | Tipo de acción | ❌ No |
| Módulo | ComboBox | Módulo | ❌ No |

### Cómo usar

1. Seleccione la **fecha**
2. Configure **hora inicio** y **hora fin** (ej: 22:00 - 23:59)
3. (Opcional) Filtros adicionales
4. Haga clic en **🔍 BUSCAR**

### Resultados

**Panel de Estadísticas:**
```
═══════════════════════════════════════
  🕐 ANÁLISIS POR HORARIO
═══════════════════════════════════════

Fecha: 15/12/2025
Rango: 22:00 - 23:59
Total acciones: 12
Tipo: ⚠️ Fuera de horario

─────────────────────────────────────
👥 USUARIOS ACTIVOS:
─────────────────────────────────────
1. supervisor1: 8 acciones
2. admin: 4 acciones

─────────────────────────────────────
📊 ACCIONES MÁS COMUNES:
─────────────────────────────────────
1. CONSULTAR_AUDITORIA
   5 veces (41.7%)

2. MODIFICAR_USUARIO
   4 veces (33.3%)

═══════════════════════════════════════
⚠️  ALERTA DE SEGURIDAD
═══════════════════════════════════════
Se detectó actividad fuera del horario
laboral estándar (08:00 - 18:00).
Revisar si esta actividad es autorizada.
```

### Casos de uso

✅ "¿Qué pasó entre 18:00 y 22:00 del 15 de diciembre?"
✅ "Detectar accesos fuera de horario laboral"
✅ "Auditar actividad nocturna (00:00 - 06:00)"

---

## 🌐 MODO 6: RASTREAR IP/MÁQUINA

### Objetivo
Rastrear **origen** de las acciones (seguridad).

### Filtros Disponibles

| Campo | Tipo | Descripción | Requerido |
|-------|------|-------------|-----------|
| Modo | Radio | IP o Máquina | ✅ Sí |
| IP | ComboBox | Dirección IP | Condicional |
| Máquina | ComboBox | Nombre de PC | Condicional |
| Desde/Hasta | Fechas | Rango temporal | ❌ No |
| Usuario | ComboBox | Usuario específico | ❌ No |
| Acción | ComboBox | Tipo de acción | ❌ No |

### Cómo usar

1. Seleccione **IP** o **Máquina**
2. Elija la IP o máquina específica
3. Configure filtros adicionales
4. Haga clic en **🔍 BUSCAR**

### Resultados

**Panel de Estadísticas (IP):**
```
═══════════════════════════════════════
  🌐 ANÁLISIS DE IP
═══════════════════════════════════════

IP: 192.168.1.100
Máquina: PC-ADMIN-01
Total acciones: 1,520
Total usuarios: 3

Primera actividad: 01/12 08:15
Última actividad: 10/01 17:30

─────────────────────────────────────
👥 USUARIOS DESDE ESTA IP:
─────────────────────────────────────
  • admin
  • supervisor1
  • usuario_temp

═══════════════════════════════════════
⚠️  ALERTAS DE SEGURIDAD
═══════════════════════════════════════

🟠 [Media] MultipleUsuarios
   3 usuarios diferentes han usado esta IP
   Usuarios: admin, supervisor1, usuario_temp

🟡 [Baja] ActividadFinSemana
   15 acciones realizadas en fin de semana
```

### Casos de uso

✅ "¿Qué usuarios acceden desde la IP 192.168.1.100?"
✅ "Rastrear actividad desde PC-GERENCIA-02"
✅ "Detectar IPs sospechosas con múltiples usuarios"

---

## 📋 MODO 7: RASTREAR REGISTRO

### Objetivo
Ver **historial completo** de un registro particular (timeline de cambios).

### Filtros Disponibles

| Campo | Tipo | Descripción | Requerido |
|-------|------|-------------|-----------|
| RegistroID | TextBox | ID del registro | ✅ Sí |
| Módulo | ComboBox | Módulo/Tabla | ❌ No |
| Desde/Hasta | Fechas | Rango temporal | ❌ No |

### Cómo usar

1. Ingrese el **RegistroID** (número)
2. (Opcional) Seleccione el módulo
3. Haga clic en **🔍 BUSCAR**

### Resultados

**Panel Timeline:**
```
═══════════════════════════════════════
  📋 TIMELINE DEL REGISTRO
═══════════════════════════════════════

RegistroID: 245
Módulo: Usuarios
Total de cambios: 4

Creado: 15/12/2025 09:30:15
Por: admin

Último cambio: 05/01/2026 16:45:12
Por: rrhh_manager

═══════════════════════════════════════
  📅 HISTORIAL CRONOLÓGICO
═══════════════════════════════════════

🆕 15/12/2025 09:30:15
   [CREACIÓN] CREAR_USUARIO
   Usuario: admin
   Detalle: Usuario creado con rol "Empleado"
   IP: 192.168.1.100 | Máquina: PC-ADMIN-01
   ───────────────────────────────────

✏️ 18/12/2025 14:22:40
   [MODIFICACIÓN] MODIFICAR_USUARIO
   Usuario: admin
   Detalle: Rol cambiado: Empleado → Supervisor
   IP: 192.168.1.100 | Máquina: PC-ADMIN-01
   ───────────────────────────────────

✏️ 20/12/2025 10:15:33
   [MODIFICACIÓN] MODIFICAR_PERMISOS_USUARIO
   Usuario: admin
   Detalle: Agregados permisos especiales
   IP: 192.168.1.100 | Máquina: PC-ADMIN-01
   ───────────────────────────────────

❌ 05/01/2026 16:45:12
   [DESACTIVACIÓN] DESACTIVAR_USUARIO
   Usuario: rrhh_manager
   Detalle: Usuario desactivado (fin de contrato)
   IP: 192.168.1.50 | Máquina: PC-RRHH-01
   ───────────────────────────────────

═══════════════════════════════════════
  📊 RESUMEN
═══════════════════════════════════════

CAMBIOS POR USUARIO:
  • admin: 3 cambios
  • rrhh_manager: 1 cambio

CAMBIOS POR TIPO:
  • CREACIÓN: 1
  • MODIFICACIÓN: 2
  • DESACTIVACIÓN: 1
```

### Casos de uso

✅ "¿Quién modificó el usuario ID 245 y cuándo?"
✅ "Ver historial completo de la factura #1234"
✅ "Rastrear todos los cambios en el proveedor ID 567"

---

## 📊 EXPORTACIÓN DE DATOS

### Exportar a Excel

1. Realice una búsqueda en cualquier modo
2. Haga clic en **📊 Exportar Excel**
3. Seleccione ubicación y nombre de archivo
4. Haga clic en **Guardar**

**El archivo incluirá:**
- Todos los registros de la búsqueda
- Formato profesional con filtros automáticos
- Columnas ajustadas automáticamente

### Exportar a PDF

*(En desarrollo - próximamente)*

---

## 💡 CASOS DE USO COMUNES

### Auditoría de Usuario

**Escenario:** Necesita auditar a un empleado que está por salir de la empresa.

**Solución:**
1. Use **Modo 1: Por Usuario**
2. Seleccione el usuario
3. Configure rango: último mes
4. Exportar a Excel para archivo

---

### Detección de Accesos No Autorizados

**Escenario:** Detectar si alguien accedió fuera de horario.

**Solución:**
1. Use **Modo 5: Por Rango Horario**
2. Configure horario: 22:00 - 06:00
3. Revisar fechas sospechosas
4. Revisar alertas de seguridad

---

### Investigación de Cambios

**Escenario:** Un registro importante fue modificado y necesita saber quién y cuándo.

**Solución:**
1. Use **Modo 7: Por Registro**
2. Ingrese el RegistroID
3. Revisar timeline completo
4. Identificar usuario y cambios

---

### Análisis de Tendencias

**Escenario:** ¿Está aumentando el uso de una funcionalidad?

**Solución:**
1. Use **Modo 4: Por Acción**
2. Seleccione la acción
3. Compare periodos
4. Revisar gráfico de tendencias

---

### Auditoría por Departamento

**Escenario:** Generar reporte de actividad del departamento de Contabilidad.

**Solución:**
1. Use **Modo 2: Por Categoría**
2. Seleccione "CONTABILIDAD"
3. Configure mes actual
4. Exportar a Excel
5. Revisar módulos más activos

---

## ❓ PREGUNTAS FRECUENTES

### ¿Cuántos resultados puedo obtener?

No hay límite técnico, pero se recomienda usar filtros para mantener los resultados manejables (< 10,000 registros).

### ¿Los datos se actualizan en tiempo real?

Sí, cada búsqueda consulta la base de datos en tiempo real.

### ¿Puedo guardar mis búsquedas favoritas?

En la versión actual no, pero puede exportar los resultados a Excel para referencia futura.

### ¿Qué diferencia hay entre "Modo 1" y "Modo 4"?

- **Modo 1:** Rastrea TODO lo que hizo UN usuario
- **Modo 4:** Rastrea UNA acción ejecutada por TODOS los usuarios

### ¿Puedo ver acciones eliminadas?

No, el sistema de auditoría registra acciones pero no recupera datos eliminados del sistema.

### ¿Los administradores pueden ver mi actividad?

Sí, todos los usuarios con permiso de auditoría pueden ver la actividad de todos los usuarios del sistema.

### ¿Se puede modificar o eliminar el historial de auditoría?

No, el historial de auditoría es **inmutable** y solo puede ser consultado, nunca modificado.

---

## 📞 SOPORTE

Para dudas o problemas con el módulo de búsqueda avanzada:

- **Soporte Técnico:** soporte@mofiserp.com
- **Documentación:** /MOFIS ERP/Docs/
- **Capacitación:** Solicitar a RRHH

---

## 📝 HISTORIAL DE VERSIONES

| Versión | Fecha | Cambios |
|---------|-------|---------|
| 1.0 | 2026-01-10 | Lanzamiento inicial con 7 modos |

---

**FIN DEL MANUAL DE USUARIO**

*MOFIS ERP - Sistema de Gestión Empresarial*
*© 2026 - Todos los derechos reservados*
