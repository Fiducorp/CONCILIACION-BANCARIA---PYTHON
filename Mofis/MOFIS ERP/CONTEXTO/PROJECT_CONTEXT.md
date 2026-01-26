# 🏢 MOFIS-ERP - CONTEXTO COMPLETO DEL PROYECTO

**Versión:** 1.0
**Fecha de última actualización:** 05 Enero de 2026
**Desarrollador principal:** Cysero
**Cliente:** Fiducorp

---

## 📑 TABLA DE CONTENIDOS

1. [INFORMACIÓN GENERAL](#1-información-general-del-proyecto)
2. [ARQUITECTURA Y TECNOLOGÍAS](#2-arquitectura-y-tecnologías)
3. [ESTRUCTURA DE LA BASE DE DATOS](#3-estructura-de-la-base-de-datos)
4. [ESTRUCTURA DEL CÓDIGO](#4-estructura-del-código)
5. [MÓDULOS DEL SISTEMA](#5-módulos-del-sistema)
6. [MÓDULO ACTUAL: GESTIÓN DE ROLES Y PERMISOS](#6-módulo-gestión-de-roles-y-permisos)
7. [CONVENCIONES Y ESTÁNDARES](#7-convenciones-y-estándares)
8. [HELPERS Y CLASES AUXILIARES](#8-helpers-y-clases-auxiliares)
9. [PLAN DE DESARROLLO FUTURO](#9-plan-de-desarrollo-futuro)
10. [DECISIONES TÉCNICAS Y ARQUITECTÓNICAS](#10-decisiones-técnicas-y-arquitectónicas)

---

## 1. INFORMACIÓN GENERAL DEL PROYECTO

### 1.1. Descripción
**MOFIS-ERP** es un sistema integral de planificación de recursos empresariales (ERP) desarrollado específicamente para **Fiducorp**, una empresa fiduciaria. El sistema está diseñado para gestionar todos los procesos internos de la empresa de manera centralizada, eficiente y segura.

### 1.2. Objetivo Principal
Centralizar y automatizar los procesos de negocio de Fiducorp, incluyendo:
- Gestión de usuarios y permisos
- Contabilidad y finanzas
- Gestión gerencial
- Recursos humanos
- Reportería y auditoría

### 1.3. Cliente
**Fiducorp** - Empresa fiduciaria que requiere un sistema robusto, seguro y escalable para la gestión integral de sus operaciones.

### 1.4. Alcance Actual
El proyecto se encuentra en **fase de desarrollo activo**, con los siguientes módulos en diferentes estados:

- ✅ **Sistema de Login y Autenticación** (COMPLETADO)
- ✅ **Dashboard Principal** (COMPLETADO)
- ✅ **Gestión de Usuarios** (COMPLETADO)
- 🔄 **Gestión de Roles y Permisos** (EN DESARROLLO - 25% completado)
- ⏳ **Módulo de Contabilidad** (PENDIENTE)
- ⏳ **Módulo de Gerencia** (PENDIENTE)
- ⏳ **Módulo de Recursos Humanos** (PENDIENTE)

---

## 2. ARQUITECTURA Y TECNOLOGÍAS

### 2.1. Stack Tecnológico

#### **Frontend**
- **Plataforma:** Windows Forms (.NET Framework 4.7.2+)
- **Lenguaje:** C# 7.3+
- **IDE:** Visual Studio 2019/2022
- **Arquitectura de UI:** MDI (Multiple Document Interface)

#### **Backend**
- **Base de datos:** SQL Server Express 2019
- **ORM:** ADO.NET (SqlConnection, SqlCommand, SqlDataAdapter)
- **Seguridad:** BCrypt.NET para hashing de contraseñas

#### **Librerías y Paquetes NuGet**
- `BCrypt.Net-Next` (v4.0.3+) - Hashing de contraseñas
- `ClosedXML` (v0.104.2+) - Generación de reportes Excel

#### **Control de Versiones**
- **Sistema:** Git
- **Repositorio:** GitHub
- **Estrategia de branching:** Git Flow simplificado
  - `main` - Producción
  - `develop` - Desarrollo
  - `feature/*` - Nuevas funcionalidades

### 2.2. Arquitectura del Sistema
```
┌─────────────────────────────────────────────────────────────┐
│                      PRESENTACIÓN                           │
│  (Windows Forms - MDI Container - FormMain.cs)             │
├─────────────────────────────────────────────────────────────┤
│                      LÓGICA DE NEGOCIO                      │
│  (Classes/Helpers - Validaciones - Procesamiento)          │
├─────────────────────────────────────────────────────────────┤
│                      ACCESO A DATOS                         │
│  (DatabaseConnection - ADO.NET - Queries)                  │
├─────────────────────────────────────────────────────────────┤
│                      BASE DE DATOS                          │
│  (SQL Server Express - Stored Procedures - Triggers)       │
└─────────────────────────────────────────────────────────────┘
```

### 2.3. Patrón de Diseño Principal

**Arquitectura en Capas (Layered Architecture)**

1. **Capa de Presentación:**
   - Formularios Windows Forms
   - Controles personalizados
   - Validaciones de UI

2. **Capa de Lógica de Negocio:**
   - Helpers (AuditoriaHelper, SesionActual, etc.)
   - Validaciones de reglas de negocio
   - Procesamiento de datos

3. **Capa de Acceso a Datos:**
   - DatabaseConnection
   - Queries SQL
   - Transacciones

4. **Capa de Datos:**
   - SQL Server Express
   - Stored Procedures (futuros)
   - Triggers de auditoría

---

## 3. ESTRUCTURA DE LA BASE DE DATOS

### 3.1. Convenciones de Nomenclatura

- **Tablas:** PascalCase singular (ej: `Usuario`, `Rol`)
- **Columnas:** PascalCase (ej: `UsuarioID`, `NombreCompleto`)
- **Primary Keys:** `[NombreTabla]ID` (ej: `UsuarioID`)
- **Foreign Keys:** `[NombreTablaReferenciada]ID` (ej: `RolID`)
- **Índices:** `IX_[NombreTabla]_[Columna]`
- **Constraints:** `CK_[NombreTabla]_[Descripcion]`

### 3.2. Tablas Principales

#### **SISTEMA Y SEGURIDAD**

**Usuarios**
```sql
CREATE TABLE Usuarios (
    UsuarioID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,  -- BCrypt hash
    NombreCompleto NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    RolID INT NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    EsEliminado BIT NOT NULL DEFAULT 0,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaUltimaModificacion DATETIME,
    CreadoPorUsuarioID INT,
    ModificadoPorUsuarioID INT,
    FOREIGN KEY (RolID) REFERENCES Roles(RolID)
)
```

**Roles**
```sql
CREATE TABLE Roles (
    RolID INT PRIMARY KEY IDENTITY(1,1),
    NombreRol NVARCHAR(50) UNIQUE NOT NULL,
    Descripcion NVARCHAR(255),
    EsSistema BIT NOT NULL DEFAULT 0,  -- Protección de roles críticos
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    CreadoPorUsuarioID INT
)
```

**Roles del Sistema (EsSistema = 1):**
- ROOT (ID: 1)
- ADMIN (ID: 2)
- GERENTE (ID: 3)
- CONTADOR (ID: 4)
- ANALISTA (ID: 5)
- PROBADOR (ID: 6)

**Sesiones**
```sql
CREATE TABLE Sesiones (
    SesionID INT PRIMARY KEY IDENTITY(1,1),
    UsuarioID INT NOT NULL,
    Token NVARCHAR(255) UNIQUE NOT NULL,
    FechaInicio DATETIME NOT NULL DEFAULT GETDATE(),
    FechaExpiracion DATETIME NOT NULL,
    IPAddress NVARCHAR(50),
    Activa BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID)
)
```

#### **CATÁLOGOS DEL SISTEMA**

**CatalogoCategorias**
```sql
CREATE TABLE CatalogoCategorias (
    CategoriaID INT PRIMARY KEY IDENTITY(1,1),
    NombreCategoria NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(255),
    OrdenVisualizacion INT,
    Activo BIT NOT NULL DEFAULT 1
)
```

**Categorías actuales:**
1. SISTEMA
2. CONTABILIDAD
3. GERENCIA
4. RECURSOS HUMANOS

**CatalogoModulos**
```sql
CREATE TABLE CatalogoModulos (
    ModuloID INT PRIMARY KEY IDENTITY(1,1),
    CategoriaID INT NOT NULL,
    NombreModulo NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(255),
    OrdenVisualizacion INT,
    Activo BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (CategoriaID) REFERENCES CatalogoCategorias(CategoriaID)
)
```

**CatalogoFormularios**
```sql
CREATE TABLE CatalogoFormularios (
    FormularioID INT PRIMARY KEY IDENTITY(1,1),
    ModuloID INT NOT NULL,
    CodigoFormulario NVARCHAR(50) UNIQUE NOT NULL,  -- Ej: "SIST-USR-001"
    NombreFormulario NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255),
    RutaClase NVARCHAR(255),  -- Namespace completo de la clase
    OrdenVisualizacion INT,
    Activo BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (ModuloID) REFERENCES CatalogoModulos(ModuloID)
)
```

**CatalogoAcciones**
```sql
CREATE TABLE CatalogoAcciones (
    AccionID INT PRIMARY KEY IDENTITY(1,1),
    CodigoAccion NVARCHAR(20) UNIQUE NOT NULL,  -- VIEW, CREATE, EDIT, etc.
    NombreAccion NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(255),
    OrdenVisualizacion INT,
    Activo BIT NOT NULL DEFAULT 1
)
```

**Acciones estándar:**
- VIEW (Ver/Consultar)
- CREATE (Crear/Agregar)
- EDIT (Editar/Modificar)
- DELETE (Eliminar)
- PRINT (Imprimir)
- REPRINT (Reimprimir)
- EXPORT (Exportar)
- RESET (Resetear)
- ACTIVATE (Activar/Desactivar)

#### **PERMISOS**

**PermisosRol**
```sql
CREATE TABLE PermisosRol (
    PermisoRolID INT PRIMARY KEY IDENTITY(1,1),
    RolID INT NOT NULL,
    FormularioID INT NOT NULL,
    AccionID INT NOT NULL,
    Permitido BIT NOT NULL DEFAULT 0,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaModificacion DATETIME,
    CreadoPorUsuarioID INT,
    ModificadoPorUsuarioID INT,
    FOREIGN KEY (RolID) REFERENCES Roles(RolID),
    FOREIGN KEY (FormularioID) REFERENCES CatalogoFormularios(FormularioID),
    FOREIGN KEY (AccionID) REFERENCES CatalogoAcciones(AccionID),
    UNIQUE (RolID, FormularioID, AccionID)  -- Un permiso único por combinación
)
```

**PermisosUsuario** (Excepciones personales)
```sql
CREATE TABLE PermisosUsuario (
    PermisoUsuarioID INT PRIMARY KEY IDENTITY(1,1),
    UsuarioID INT NOT NULL,
    FormularioID INT NOT NULL,
    AccionID INT NOT NULL,
    Permitido BIT NOT NULL DEFAULT 0,
    Motivo NVARCHAR(255),  -- Justificación de la excepción
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaModificacion DATETIME,
    CreadoPorUsuarioID INT,
    ModificadoPorUsuarioID INT,
    FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID),
    FOREIGN KEY (FormularioID) REFERENCES CatalogoFormularios(FormularioID),
    FOREIGN KEY (AccionID) REFERENCES CatalogoAcciones(AccionID),
    UNIQUE (UsuarioID, FormularioID, AccionID)
)
```

**Lógica de resolución de permisos:**
```
Permiso Final = PermisoUsuario ?? PermisoRol ?? false

Si existe PermisoUsuario → Usar ese (excepción personal)
Si no existe → Usar PermisoRol (heredado del rol)
Si no existe → Denegar (false)
```

#### **AUDITORÍA**

**Auditoria**
```sql
CREATE TABLE Auditoria (
    AuditoriaID BIGINT PRIMARY KEY IDENTITY(1,1),
    UsuarioID INT NOT NULL,
    Accion NVARCHAR(100) NOT NULL,  -- LOGIN, CREAR_USUARIO, MODIFICAR_PERMISOS, etc.
    Modulo NVARCHAR(50),
    Submodulo NVARCHAR(50),
    Formulario NVARCHAR(100),
    RegistroID INT,  -- ID del registro afectado (opcional)
    Detalle NVARCHAR(MAX),  -- JSON o texto descriptivo
    FechaHora DATETIME NOT NULL DEFAULT GETDATE(),
    IPAddress NVARCHAR(50),
    FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID)
)
```

**Índice para búsquedas rápidas:**
```sql
CREATE INDEX IX_Auditoria_Usuario_Fecha ON Auditoria(UsuarioID, FechaHora DESC);
CREATE INDEX IX_Auditoria_Accion ON Auditoria(Accion);
CREATE INDEX IX_Auditoria_Modulo ON Auditoria(Modulo);
```

### 3.3. Scripts de Actualización

Los scripts SQL se organizan en:
```
Database/
├── 01_Schema/
│   └── CREATE_DATABASE.sql
├── 02_Initial_Data/
│   ├── INSERT_Roles.sql
│   ├── INSERT_Categorias.sql
│   ├── INSERT_Modulos.sql
│   └── INSERT_Formularios.sql
└── 03_Updates/
    ├── UPDATE_001_AgregarSesiones.sql
    ├── UPDATE_002_AgregarAuditoria.sql
    ├── UPDATE_003_AgregarCatalogos.sql
    └── UPDATE_004_AgregarColumnaEsSistema.sql
```

**Convención de nombres:**
- `UPDATE_XXX_DescripcionCorta.sql`
- Cada update debe ser idempotente (puede ejecutarse múltiples veces sin error)
- Usar `IF NOT EXISTS` para creación de objetos

### 3.4. Diagrama Relacional Simplificado
```
Usuarios ──────┐
    │          │
    │ RolID    │ UsuarioID
    │          │
    ▼          │
Roles          │
    │          │
    │ RolID    │
    │          │
    ▼          ▼
PermisosRol  PermisosUsuario
    │          │
    ├──────────┴─ FormularioID
    │
    ▼
CatalogoFormularios
    │
    │ ModuloID
    │
    ▼
CatalogoModulos
    │
    │ CategoriaID
    │
    ▼
CatalogoCategorias
```

---

## 4. ESTRUCTURA DEL CÓDIGO

### 4.1. Estructura de Carpetas del Proyecto
```
MOFIS-ERP/
├── 📁 Classes/
│   ├── AuditoriaHelper.cs
│   ├── DatabaseConnection.cs
│   ├── SesionActual.cs
│   └── PermissionChecker.cs (futuro)
│
├── 📁 Forms/
│   ├── 📁 Auth/
│   │   ├── FormLogin.cs
│   │   └── FormCambiarPassword.cs
│   │
│   ├── 📁 Sistema/
│   │   ├── 📁 GestionUsuarios/
│   │   │   ├── FormGestionUsuarios.cs
│   │   │   └── FormUsuario.cs
│   │   │
│   │   └── 📁 GestionRoles/
│   │       ├── FormDashboardRoles.cs              ← PANEL PRINCIPAL
│   │       ├── FormAdministrarPermisos.cs         ← COMPLETADO (2,500 líneas)
│   │       ├── FormCrearRol.cs
│   │       ├── FormCopiarPermisos.cs
│   │       ├── FormVistaPreviaCambios.cs
│   │       ├── FormConfirmarPassword.cs
│   │       ├── FormGenerarReporte.cs
│   │       ├── FormConsultarPermisos.cs           ← PENDIENTE
│   │       └── FormReportesPermisos.cs            ← PENDIENTE
│   │
│   ├── 📁 Contabilidad/
│   │   └── (módulos futuros)
│   │
│   ├── 📁 Gerencia/
│   │   └── (módulos futuros)
│   │
│   └── FormMain.cs                                ← MDI CONTAINER PRINCIPAL
│
├── 📁 Database/
│   ├── 01_Schema/
│   ├── 02_Initial_Data/
│   └── 03_Updates/
│
├── 📁 Resources/
│   ├── Icons/
│   └── Images/
│
├── 📁 Docs/
│   ├── PROJECT_CONTEXT.md                         ← ESTE ARCHIVO
│   └── ARCHITECTURE.md
│
├── App.config
├── Program.cs
└── MOFIS-ERP.csproj
```

### 4.2. Namespace Convention
```csharp
MOFIS_ERP                               // Raíz
├── MOFIS_ERP.Classes                   // Helpers y utilidades
├── MOFIS_ERP.Forms                     // Formularios base
├── MOFIS_ERP.Forms.Auth                // Autenticación
├── MOFIS_ERP.Forms.Sistema             // Sistema
│   ├── MOFIS_ERP.Forms.Sistema.GestionUsuarios
│   └── MOFIS_ERP.Forms.Sistema.GestionRoles
├── MOFIS_ERP.Forms.Contabilidad
├── MOFIS_ERP.Forms.Gerencia
└── MOFIS_ERP.Forms.RecursosHumanos
```

---

## 5. MÓDULOS DEL SISTEMA

### 5.1. Módulos Completados

#### **5.1.1. Sistema de Login y Autenticación**

**Archivos:**
- `Forms/Auth/FormLogin.cs`
- `Forms/Auth/FormCambiarPassword.cs`

**Funcionalidades:**
- ✅ Login con username/password
- ✅ Validación contra tabla Usuarios
- ✅ Verificación con BCrypt
- ✅ Generación de token de sesión
- ✅ Almacenamiento en tabla Sesiones
- ✅ Registro en auditoría (LOGIN, LOGOUT, LOGIN_FALLIDO)
- ✅ Manejo de sesiones activas
- ✅ Cambio de contraseña con validaciones

**Validaciones:**
- Longitud mínima de contraseña: 8 caracteres
- Contraseña debe contener: mayúsculas, minúsculas, números
- Bloqueo temporal después de 3 intentos fallidos
- Contraseña temporal en primer login

#### **5.1.2. Dashboard Principal (FormMain)**

**Archivo:** `Forms/FormMain.cs`

**Características:**
- ✅ MDI Container con paneles laterales
- ✅ Panel izquierdo con menú de navegación
- ✅ Panel central para cargar formularios hijos
- ✅ Barra superior con información del usuario
- ✅ Botón de logout con confirmación
- ✅ Método `CargarContenidoPanel()` para navegación

**Estructura:**
```csharp
public void CargarContenidoPanel(Form formulario)
{
    // Limpiar panel actual
    panelContenido.Controls.Clear();

    // Configurar formulario hijo
    formulario.TopLevel = false;
    formulario.FormBorderStyle = FormBorderStyle.None;
    formulario.Dock = DockStyle.Fill;

    // Agregar al panel
    panelContenido.Controls.Add(formulario);
    formulario.Show();
}

### 7.3. Colores Corporativos

```csharp
// Paleta principal
Color colorCorporativo = Color.FromArgb(0, 120, 212);      // Azul Microsoft #0078D4
Color colorVerde = Color.FromArgb(34, 139, 34);            // Verde éxito
Color colorRojo = Color.FromArgb(220, 53, 69);             // Rojo error/eliminar
Color colorNaranja = Color.FromArgb(255, 152, 0);          // Naranja advertencia
Color colorGris = Color.FromArgb(108, 117, 125);           // Gris neutro

// Colores claros (backgrounds)
Color colorVerdeClaro = Color.FromArgb(200, 255, 200);     // Permiso activado
Color colorRojoClaro = Color.FromArgb(255, 200, 200);      // Permiso denegado
Color colorNaranjaClaro = Color.FromArgb(255, 243, 205);   // Excepción usuario
Color colorGrisClaro = Color.FromArgb(240, 240, 240);      // Filas alternadas

// Colores hover (más oscuros)
Color colorCorporativoHover = Color.FromArgb(0, 100, 180);
Color colorVerdeHover = Color.FromArgb(40, 167, 69);
Color colorRojoHover = Color.FromArgb(200, 35, 51);
Color colorNaranjaHover = Color.FromArgb(255, 120, 0);

---

## 11. NOTAS FINALES

### 11.1. Contacto y Soporte

**Desarrollador Principal:** Cysero

**Para Claude Code:**
- Este documento debe estar siempre actualizado
- Consultar antes de implementar nuevas funcionalidades
- Actualizar después de cambios significativos

### 11.2. Versionado del Documento

- **v1.0** (30/12/2025): Versión inicial completa

### 11.3. Instrucciones para Claude Code

**Al iniciar una sesión:**
1. Leer este documento completo
2. Revisar el estado actual del módulo en desarrollo
3. Verificar archivos recientes en Git

**Al implementar nuevas funcionalidades:**
1. Seguir convenciones de este documento
2. Mantener coherencia con código existente
3. Actualizar este documento si haces cambios arquitectónicos

# CONTEXTO DEL PROYECTO MOFIS-ERP

## 📋 INFORMACIÓN BÁSICA

**Proyecto:** MOFIS-ERP  
**Cliente:** Fiducorp (empresa fiduciaria)  
**Tecnología:** C# Windows Forms + SQL Server Express  
**Arquitectura:** MDI (Multiple Document Interface)  
**Estado:** En desarrollo activo  

---

PROMPT 

## 🏗️ ARQUITECTURA MDI: CÓMO FUNCIONA EL SISTEMA

### **Concepto General:**

El sistema usa **MDI (Multiple Document Interface)**, que significa que hay un **contenedor principal** (FormMain) que actúa como "ventana padre" y dentro de él se cargan diferentes **formularios hijos** según la navegación del usuario.

### **Componentes Clave:**

#### **1. FormMain.cs (Contenedor MDI)**
```csharp
public partial class FormMain : Form
{
    // Panel donde se cargan los formularios hijos
    private Panel panelContenido;
    
    public FormMain()
    {
        InitializeComponent();
        this.IsMdiContainer = true;  // ← Esto lo hace un contenedor MDI
        ConfigurarInterfaz();
    }
    
    // MÉTODO CLAVE: Cargar formularios hijos
    public void CargarContenidoPanel(Form formulario)
    {
        // 1. Limpiar panel actual
        panelContenido.Controls.Clear();
        
        // 2. Configurar formulario hijo
        formulario.TopLevel = false;           // No es ventana independiente
        formulario.FormBorderStyle = FormBorderStyle.None;  // Sin borde
        formulario.Dock = DockStyle.Fill;      // Ocupar todo el espacio
        
        // 3. Agregar al panel
        panelContenido.Controls.Add(formulario);
        formulario.Show();
    }
}
```
## 👥 MÓDULO GESTIÓN DE USUARIOS (COMPLETADO)

### **Descripción:**

Sistema CRUD completo para administrar usuarios del sistema, con asignación de roles, activación/desactivación, y generación de contraseñas temporales.

**Funcionalidades:**

1. **CRUD completo:**
   - Crear nuevo usuario
   - Editar usuario existente
   - Eliminar usuario (soft delete: EsEliminado = 1)
   - Activar/Desactivar (Activo = 0/1)

2. **Filtros:**
   - Por rol (ComboBox con todos los roles)
   - Por estado (Activo/Inactivo/Todos)
   - Búsqueda en tiempo real por nombre o username

3. **Asignación de roles:**
   - ComboBox con roles disponibles
   - Al seleccionar rol, usuario hereda sus permisos

4. **Contraseñas:**
   - Generación automática de contraseña temporal
   - BCrypt para hashing
   - Usuario debe cambiar en primer login

5. **Validaciones:**
   - Username único
   - Email válido (regex)
   - Contraseña mínimo 8 caracteres
   - Rol obligatorio

6. **Auditoría:**
   - CREAR_USUARIO
   - MODIFICAR_USUARIO
   - ELIMINAR_USUARIO
   - ACTIVAR_USUARIO
   - DESACTIVAR_USUARIO

## 🔐 MÓDULO GESTIÓN DE ROLES Y PERMISOS (25% COMPLETADO)

### **Descripción:**

Sistema multinivel para administrar permisos de acceso al sistema, con gestión de roles completos y excepciones por usuario individual.

#### **Sistema de Permisos (4 Niveles):**
```
NIVEL 1: Categorías
├─ SISTEMA
├─ CONTABILIDAD
├─ GERENCIA
└─ RECURSOS HUMANOS

NIVEL 2: Módulos (dentro de cada categoría)
├─ Gestión de Usuarios
├─ Gestión de Roles
└─ ...

NIVEL 3: Formularios (dentro de cada módulo)
├─ FormGestionUsuarios
├─ FormUsuario
└─ ...

NIVEL 4: Acciones (para cada formulario)
├─ VIEW (Ver/Consultar)
├─ CREATE (Crear/Agregar)
├─ EDIT (Editar/Modificar)
├─ DELETE (Eliminar)
├─ PRINT (Imprimir)
├─ REPRINT (Reimprimir)
├─ EXPORT (Exportar)
├─ RESET (Resetear)
└─ ACTIVATE (Activar/Desactivar)
```

**Ejemplo:** Para dar permiso de "Crear Usuarios":
```
Categoría: SISTEMA
  → Módulo: Gestión de Usuarios
    → Formulario: FormGestionUsuarios
      → Acción: CREATE ← Activar este checkbox


🔍 DIFERENCIA ENTRE AUDITORÍA EN GESTION DE ROLES vs MÓDULO DE AUDITORÍA
📊 GESTIÓN DE ROLES → Reportes y Auditoría
Alcance: Solo auditoría del módulo de Roles y Permisos
Qué muestra:

✅ Cambios en permisos de roles
✅ Creación/eliminación de roles
✅ Excepciones de usuarios
✅ Copiar permisos entre roles
✅ Cambios realizados solo en este módulo

Filtros:
sqlSELECT * FROM Auditoria 
WHERE Modulo = 'SISTEMA' 
  AND Submodulo = 'Gestión de Roles'
Acciones auditadas:

MODIFICAR_PERMISOS_ROL
MODIFICAR_PERMISOS_USUARIO
CREAR_ROL
ELIMINAR_ROL
COPIAR_PERMISOS
CONFIRMAR_PASSWORD_CAMBIO_CRITICO
GENERAR_REPORTE_PERMISOS


🔍 MÓDULO DE AUDITORÍA (GLOBAL)
Alcance: Auditoría de TODO EL SISTEMA
Qué muestra:

✅ Login/Logout de usuarios
✅ CRUD de usuarios (crear, modificar, eliminar, activar)
✅ CRUD de roles y permisos
✅ Transacciones contables (futuro)
✅ Cambios en datos financieros (futuro)
✅ Acciones gerenciales (futuro)
✅ Todas las operaciones del ERP

SELECT * FROM Auditoria 
WHERE 1=1  -- TODO
  AND FechaHora BETWEEN @Desde AND @Hasta
  AND (@Modulo IS NULL OR Modulo = @Modulo)
  AND (@Usuario IS NULL OR UsuarioID = @Usuario)
  AND (@Accion IS NULL OR Accion = @Accion)
ORDER BY FechaHora DESC
```

**Acciones auditadas (ejemplos):**
- **Sistema:**
  - LOGIN
  - LOGOUT
  - LOGIN_FALLIDO
  - CREAR_USUARIO
  - MODIFICAR_USUARIO
  - ELIMINAR_USUARIO
  - CREAR_ROL
  - MODIFICAR_PERMISOS_ROL
  - ...

- **Contabilidad (futuro):**
  - CREAR_ASIENTO
  - MODIFICAR_ASIENTO
  - ELIMINAR_ASIENTO
  - CREAR_FACTURA
  - ANULAR_FACTURA
  - REGISTRAR_PAGO
  - ...

- **Gerencia (futuro):**
  - GENERAR_REPORTE_EJECUTIVO
  - APROBAR_PRESUPUESTO
  - ...

---

## 🎯 COMPARACIÓN VISUAL
```
┌─────────────────────────────────────────────────────────────────┐
│  GESTIÓN DE ROLES → Reportes y Auditoría                       │
│  (FormReportesPermisos.cs)                                     │
├─────────────────────────────────────────────────────────────────┤
│  Alcance: SOLO Gestión de Roles                               │
│                                                                 │
│  Filtros:                                                       │
│  ☑ Fecha desde/hasta                                           │
│  ☑ Usuario que hizo el cambio                                  │
│  ☑ Tipo de acción (solo de roles)                             │
│  ☑ Rol afectado                                                │
│                                                                 │
│  Acciones mostradas:                                            │
│  • MODIFICAR_PERMISOS_ROL                                      │
│  • CREAR_ROL                                                   │
│  • ELIMINAR_ROL                                                │
│  • COPIAR_PERMISOS                                             │
│  • Etc. (solo de roles)                                        │
└─────────────────────────────────────────────────────────────────┘

                            VS

┌─────────────────────────────────────────────────────────────────┐
│  MÓDULO DE AUDITORÍA (GLOBAL)                                  │
│  (FormAuditoria.cs)                                            │
├─────────────────────────────────────────────────────────────────┤
│  Alcance: TODO EL SISTEMA                                      │
│                                                                 │
│  Filtros:                                                       │
│  ☑ Fecha desde/hasta                                           │
│  ☑ Usuario que hizo la acción                                  │
│  ☑ Módulo: [TODAS] [SISTEMA] [CONTABILIDAD] [GERENCIA]       │
│  ☑ Submódulo: [Todos] [Gestión de Usuarios] [Gestión Roles]  │
│  ☑ Tipo de acción: [Todas] [LOGIN] [CREAR] [MODIFICAR]...    │
│  ☑ Búsqueda por texto en detalles                             │
│                                                                 │
│  Acciones mostradas:                                            │
│  • LOGIN, LOGOUT, LOGIN_FALLIDO                                │
│  • CREAR_USUARIO, MODIFICAR_USUARIO, ELIMINAR_USUARIO         │
│  • CREAR_ROL, MODIFICAR_PERMISOS_ROL, COPIAR_PERMISOS         │
│  • CREAR_ASIENTO, MODIFICAR_ASIENTO (futuro)                  │
│  • CREAR_FACTURA, ANULAR_FACTURA (futuro)                     │
│  • GENERAR_REPORTE_EJECUTIVO (futuro)                         │
│  • ... TODO LO QUE PASE EN EL SISTEMA                         │
└─────────────────────────────────────────────────────────────────┘
```

---

## 💡 MI RECOMENDACIÓN: **SÍ, MANTENER EL MÓDULO DE AUDITORÍA**

### **Razones:**

1. ✅ **Alcance diferente:**
   - FormReportesPermisos = Solo cambios en roles
   - FormAuditoria = Todo el sistema

2. ✅ **Propósito diferente:**
   - FormReportesPermisos = Para administradores de seguridad
   - FormAuditoria = Para auditores, compliance, gerencia

3. ✅ **Casos de uso diferentes:**
   - "¿Quién cambió los permisos del rol CONTADOR?" → FormReportesPermisos
   - "¿Quién accedió al sistema ayer?" → FormAuditoria
   - "¿Qué usuarios han hecho login esta semana?" → FormAuditoria
   - "¿Cuántos intentos fallidos de login hubo hoy?" → FormAuditoria
   - "¿Qué cambios se hicieron en finanzas?" → FormAuditoria (futuro)

4. ✅ **Requisitos legales/compliance:**
   - Muchas empresas fiduciarias requieren **auditoría completa** del sistema
   - Trazabilidad de **todas las acciones**
   - Reportes para **auditorías externas**

5. ✅ **Escalabilidad:**
   - Cuando agregues módulos de Contabilidad, Gerencia, RR.HH...
   - Necesitarás ver auditoría de **todos ellos juntos**
   - FormReportesPermisos solo mostraría roles (muy limitado)

---

## 🎨 ESTRUCTURA VISUAL DEL DASHBOARD SISTEMA
```
┌─────────────────────────────────────────────────────────────────┐
│                    DASHBOARD SISTEMA                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────────┐  ┌──────────────────┐  ┌───────────────┐│
│  │ 👥 GESTIÓN DE    │  │ 🔐 GESTIÓN DE    │  │ 🔍 AUDITORÍA  ││
│  │    USUARIOS      │  │    ROLES         │  │    GENERAL    ││
│  │                  │  │                  │  │               ││
│  │ • Crear usuarios │  │ • Administrar    │  │ • Ver TODO el ││
│  │ • Editar usuarios│  │   permisos       │  │   historial   ││
│  │ • Activar/Desact │  │ • Excepciones    │  │ • Filtros     ││
│  │ • Asignar roles  │  │   por usuario    │  │   avanzados   ││
│  │                  │  │ • Reportes roles │  │ • Exportar    ││
│  │                  │  │   (específico)   │  │               ││
│  └──────────────────┘  └──────────────────┘  └───────────────┘│
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## ✅ CONFIRMACIÓN: MANTENER 3 MÓDULOS EN CATEGORÍA SISTEMA
```
CATEGORÍA: SISTEMA
├─ 1. GESTIÓN DE USUARIOS ✅ COMPLETADO
│  └─ FormGestionUsuarios + FormUsuario
│
├─ 2. GESTIÓN DE ROLES ✅ COMPLETADO
│  ├─ FormDashboardRoles (2 cards)
│  ├─ Card #1: FormAdministrarPermisos
│  └─ Card #2: FormReportesPermisos (auditoría específica de roles)
│
└─ 3. AUDITORÍA GENERAL ⏳ PENDIENTE
   └─ FormAuditoria (auditoría de TODO el sistema)

📋 MÓDULO DE AUDITORÍA GENERAL - ESPECIFICACIÓN COMPLETA
1. OBJETIVO Y ALCANCE
Objetivo Principal:
Proporcionar un sistema centralizado de auditoría que registre y permita consultar TODAS las acciones significativas realizadas en el sistema MOFIS-ERP, garantizando trazabilidad completa, cumplimiento normativo y seguridad de la información.
Alcance:

✅ Auditoría global de todos los módulos del sistema
✅ Consulta histórica con filtros avanzados
✅ Exportación de reportes para auditorías externas
✅ Análisis de patrones de uso y actividad
✅ Detección de anomalías y accesos no autorizados
✅ Trazabilidad completa para compliance y regulaciones

✅ HACER:
- Registrar TODAS las acciones CRUD (Create, Read importante, Update, Delete)
- Registrar acciones de aprobación/rechazo
- Registrar impresiones y exportaciones importantes
- Incluir detalle claro y descriptivo
- Incluir RegistroID cuando aplique

❌ NO HACER:
- Registrar acciones triviales (abrir formulario, cambiar tab, etc.)
- Registrar queries de solo lectura simples
- Incluir información sensible en Detalle (contraseñas, etc.)
- Dejar campos vacíos sin razón

9. RESUMEN EJECUTIVO
¿Qué es el Módulo de Auditoría?
Un sistema centralizado que registra y permite consultar todas las acciones significativas realizadas en MOFIS-ERP.
¿Para qué sirve?

Trazabilidad completa de operaciones
Cumplimiento normativo y compliance
Detección de accesos no autorizados
Análisis de uso del sistema
Soporte para auditorías externas

¿Qué lo diferencia de "Reportes y Auditoría" en Gestión de Roles?

Alcance: TODO el sistema vs Solo gestión de roles
Usuarios: Auditores/Gerencia vs Admins de seguridad
Propósito: Compliance general vs Gestión específica de permisos

FormAuditoria
├─ panelSuperior (Top)
│  ├─ lblTitulo
│  ├─ lblSubtitulo
│  └─ btnVolver
│
├─ panelFiltros (Top)
│  └─ gbFiltros
│     ├─ lblFechaDesde, dtpDesde
│     ├─ lblFechaHasta, dtpHasta
│     ├─ lblModulo, cmbModulo
│     ├─ lblUsuario, cmbUsuario
│     ├─ lblAccion, cmbAccion
│     ├─ lblBuscar, txtBuscar
│     ├─ btnBuscar
│     └─ btnLimpiar
│
├─ panelEstadisticas (Top)
│  ├─ lblTotalRegistros
│  └─ lblFechaConsulta
│
├─ panelGrid (Fill)
│  └─ dgvAuditoria
│
└─ panelInferior (Bottom)
   ├─ btnActualizar
   └─ btnExportar

🔧 CONTROLES ADICIONALES A AGREGAR
PANEL BUSCADOR (Ya lo agregaste)
Perfecto, dejamos panelBuscador como está con:

txtBuscar
lblBuscar

✅ LAYOUT FINAL DE panelInferior

panelInferior (Dock: Bottom, Height: 70)
├─ btnDashboard        (20, 15)    📈 Dashboard
├─ btnBuscarDetalles   (220, 15)   🔍 Búsqueda Avanzada
├─ btnReportePDF       (420, 15)   📄 Reporte PDF
├─ btnActualizar       (920, 15)   🔄 Actualizar
└─ btnExportar         (1120, 15)  📊 Exportar a Excel

📋 RESUMEN DE FORMULARIOS QUE CREAREMOS
Basado en tu descripción, necesitaremos estos formularios:
1. FormAuditoria.cs ✅ (Actual - En proceso)

Vista principal con filtros
DataGridView con auditoría
Exportación rápida

2. FormDetalleAuditoria.cs ⏳ (Modal - Double-click en grid)

Muestra TODO el detalle de un registro específico
Información completa formateada
Opciones de navegación a registros relacionados

3. FormBusquedaAvanzadaAuditoria.cs ⏳ (Modal o MDI)

Modo 1: Rastrear todo lo que hizo UN usuario
Modo 2: Rastrear acciones en UNA categoría
Modo 3: Rastrear acciones en UN módulo/submódulo
Modo 4: Rastrear UNA acción específica (quiénes, cuándo, detalles)
Filtros de rango de fechas
Resultados en DataGridView
Exportación de resultados

4. FormDashboardAuditoria.cs ⏳ (Hijo MDI)

Gráficos analíticos dinámicos
Gráfico 1: Acciones por día/semana/mes (línea de tiempo)
Gráfico 2: Acciones por módulo (barras)
Gráfico 3: Top usuarios más activos (barras horizontales)
Gráfico 4: Distribución por tipo de acción (torta/dona)
Gráfico 5: Mapa de calor por hora del día
Gráfico 6: Comparativa de periodos
Filtros interactivos
Exportación de gráficos

5. FormReportePDFAuditoria.cs ⏳ (Modal)

Configuración de reporte PDF
Opciones de formato
Vista previa
Generación del PDF

6. FormReporteExcelAuditoria.cs ⏳ (Modal)

Configuración de reporte Excel completo
Múltiples hojas:

Resumen ejecutivo
Detalle completo
Por módulo
Por usuario
Estadísticas
Gráficos




🎯 PLAN DE DESARROLLO SUGERIDO
Fase 1: Core Funcional (Lo haremos primero)

✅ Agregar controles faltantes en FormAuditoria
✅ Implementar código de FormAuditoria.cs
✅ Testing básico

Fase 2: Detalle (Después del core)

✅ Implementar FormDetalleAuditoria.cs (modal simple)
✅ Testing de double-click

Fase 3: Reportes (Después del detalle)

✅ Implementar FormReporteExcelAuditoria.cs (completo, múltiples hojas)
✅ Implementar FormReportePDFAuditoria.cs
✅ Testing de exportaciones

Fase 4: Búsqueda Avanzada (Después de reportes)

✅ Implementar FormBusquedaAvanzadaAuditoria.cs
✅ Testing de trazabilidad

Fase 5: Dashboard Analítico (Al final)

✅ Implementar FormDashboardAuditoria.cs
✅ Integrar librería de gráficos
✅ Testing completo

🖥️ CONTROLES DEL FORMULARIO Y SU FUNCIÓN
Panel Superior (panelSuperior)

lblTitulo: "AUDITORÍA GENERAL DEL SISTEMA"
lblSubtitulo: "Historial completo de acciones en MOFIS-ERP"
btnVolver: Regresa al Dashboard de Sistema

Panel de Filtros (panelFiltros → gbFiltros)
Filtros de Fecha:

dtpDesde (DateTimePicker): Fecha desde (con checkbox)

Función: Filtra registros desde esta fecha
Comportamiento: Al cambiar valor → aplicar filtros automáticamente
Valor inicial: DateTime.Now.AddMonths(-1)


dtpHasta (DateTimePicker): Fecha hasta (con checkbox)

Función: Filtra registros hasta esta fecha
Comportamiento: Al cambiar valor → aplicar filtros automáticamente
Valor inicial: DateTime.Now

Filtros de Selección:

cmbModulo (ComboBox): Lista de módulos

Función: Filtra por módulo específico
Comportamiento: SelectedIndexChanged → aplicar filtros
Datos: Se carga DINÁMICAMENTE desde BD (DISTINCT Modulo FROM Auditoria)
Problema actual: Query lenta al cargar
Items: ["Todos los Módulos"] + módulos únicos de la BD
Actualización: Debe actualizarse automáticamente cuando se agreguen nuevos módulos al sistema


cmbUsuario (ComboBox): Lista de usuarios

Función: Filtra por usuario específico
Comportamiento: SelectedIndexChanged → aplicar filtros
Datos: Se carga desde tabla Usuarios (solo activos, no eliminados)
Problema actual: Query lenta al cargar
Items: ["Todos los Usuarios"] + usuarios activos
DisplayMember: "{NombreCompleto} ({Username})"


cmbAccion (ComboBox): Lista de acciones

Función: Filtra por tipo de acción
Comportamiento: SelectedIndexChanged → aplicar filtros
Datos: Se carga DINÁMICAMENTE desde BD (DISTINCT Accion FROM Auditoria)
Problema actual: Query lenta al cargar
Items: ["Todas las Acciones"] + acciones únicas de la BD
Actualización: Debe actualizarse automáticamente cuando se registren nuevas acciones



Buscador:

txtBuscar (TextBox): Búsqueda de texto libre

Función: Busca en TODAS las columnas (Detalle, Accion, Modulo, Formulario, Categoria, Username, DireccionIP, NombreMaquina)
Comportamiento: TextChanged con Timer de 300ms (debounce) → aplicar filtros
Placeholder: "Buscar en detalle..." (desaparece al hacer focus)
Tipo de búsqueda: LIKE '%texto%' en múltiples columnas



Botones de Control:

btnBuscar: ELIMINADO (búsqueda en tiempo real)
btnLimpiar: Resetea todos los filtros a valores por defecto

Acción: Resetear fechas (último mes), ComboBoxes a index 0, limpiar txtBuscar



Panel Buscador (panelBuscador)

Contiene solo txtBuscar y su label
Dock: Top (debajo de panelFiltros)

Panel de Estadísticas (panelEstadisticas)

lblTotalRegistros: Muestra cantidad de registros

Formato: "Total: 1,234 registros" o "Mostrando: 456 de 5,000 registros"
Actualización: Cada vez que se aplican filtros


lblFechaConsulta: Muestra última actualización

Formato: "Última actualización: 04/01/2026 14:30:00"
Actualización: Cada vez que se aplican filtros



Panel Grid (panelGrid → dgvAuditoria)
DataGridView Principal:

Columnas (10 columnas totales):

AuditoriaID (oculta)
FechaHora (130px) - Formato: "dd/MM/yyyy HH:mm:ss"
Username (120px)
Categoria (120px)
Modulo (140px)
Formulario (160px)
Accion (200px)
Detalle (Fill - resto del espacio disponible)
DireccionIP (100px)
NombreMaquina (110px)


Configuración:

ReadOnly: True
SelectionMode: FullRowSelect
MultiSelect: False
AlternatingRowsDefaultCellStyle.BackColor: #F0F0F0
ColumnHeadersDefaultCellStyle.BackColor: #0078D4 (azul corporativo)
ColumnHeadersDefaultCellStyle.ForeColor: White
Sin selección inicial: CurrentCell = null después de cargar


Eventos:

DoubleClick: Abre modal con detalles completos (FormDetalleAuditoria - pendiente)



Panel Inferior (panelInferior)
Botones de Acción:

btnDashboard (Morado #9C27B0)

Texto: "📈 Dashboard"
Función: Abre FormDashboardAuditoria con gráficos analíticos (pendiente)


btnBuscarDetalles (Naranja #FF9800)

Texto: "🔍 Búsqueda Avanzada"
Función: Abre FormBusquedaAvanzadaAuditoria con trazabilidad avanzada (pendiente)


btnReportePDF (Rojo #DC3545)

Texto: "📄 Reporte PDF"
Función: Genera reporte PDF profesional (pendiente)


btnActualizar (Azul #0078D4)

Texto: "🔄 Actualizar"
Función: Recarga datos desde BD (pregunta confirmación)
Acción: Ejecuta CargarDatosIniciales() de nuevo


btnExportar (Verde #228B22)

Texto: "📊 Exportar a Excel"
Función: Exporta datos filtrados a Excel profesional
Hojas generadas:

Resumen Ejecutivo (estadísticas, criterios, top 10)
Detalle Completo (todos los registros filtrados)
Por Módulo (agrupado)
Por Usuario (agrupado)

💬 NOTAS ADICIONALES

El sistema usa MDI (Multiple Document Interface) con FormMain
Los colores corporativos son: #0078D4 (azul), #228B22 (verde), #DC3545 (rojo)
Todas las acciones deben registrarse en auditoría usando AuditoriaHelper.RegistrarAccion()
El usuario actual está en SesionActual (UsuarioID, Username, NombreCompleto, etc.)

📊 DESCRIPCIÓN COMPLETA DE BOTONES EN panelInferior
1. btnDashboard (Morado #9C27B0)

Texto: "📈 Dashboard"
Ubicación: Location(20, 15), Size(180, 40)
Función: Abre FormDashboardAuditoria (hijo MDI)
Características del Dashboard:

Gráficos analíticos dinámicos:

Gráfico de línea: Acciones por día/semana/mes (tendencias temporales)
Gráfico de barras: Acciones por módulo (comparativa)
Gráfico de barras horizontales: Top 10 usuarios más activos
Gráfico de torta/dona: Distribución por tipo de acción
Mapa de calor: Actividad por hora del día (00:00 - 23:00)
Gráfico de comparativa: Periodo actual vs periodo anterior


Filtros interactivos:

Selector de periodo (Hoy, Última semana, Último mes, Último año, Personalizado)
Filtro por categoría/módulo
Filtro por usuario


Exportación de gráficos:

Guardar gráficos como imagen (PNG)
Exportar dashboard completo a PDF


Estadísticas en tiempo real:

Total de acciones en el periodo
Promedio de acciones por día
Usuario más activo
Módulo más usado
Hora pico de actividad




Estado actual: Pendiente de implementación (muestra MessageBox informativo)


2. btnBuscarDetalles (Naranja #FF9800)

Texto: "🔍 Búsqueda Avanzada"
Ubicación: Location(220, 15), Size(180, 40)
Función: Abre FormBusquedaAvanzadaAuditoria (modal o hijo MDI)
Características de Búsqueda Avanzada:
MODO 1: Rastrear TODO lo que hizo un usuario

Filtros:

Seleccionar usuario (ComboBox)
Rango de fechas (Desde - Hasta)
Día específico (opcional)


Resultados:

DataGridView con timeline cronológico de todas sus acciones
Agrupado por sesión (LOGIN → acciones → LOGOUT)
Total de acciones en el periodo
Resumen: acciones más frecuentes de ese usuario


Exportación:

Excel con detalle completo
PDF con reporte de actividad del usuario



MODO 2: Rastrear acciones en una CATEGORÍA

Filtros:

Seleccionar categoría (SISTEMA, CONTABILIDAD, GERENCIA, RR.HH.)
Rango de fechas
Mostrar quién hizo cada acción


Resultados:

Todas las acciones en esa categoría
Agrupado por módulo dentro de la categoría
Resumen: módulos más activos
Lista de usuarios que trabajaron en esa categoría


Visualización:

Vista de árbol: Categoría → Módulos → Acciones
Vista de lista: Flat con todas las acciones



MODO 3: Rastrear acciones en un MÓDULO/SUBMÓDULO

Filtros:

Seleccionar categoría (cascada)
Seleccionar módulo (cascada)
Seleccionar formulario específico (opcional)
Rango de fechas


Resultados:

Todas las acciones en ese módulo/formulario
Quién hizo qué y cuándo
Estadísticas: acciones por tipo (CREATE, EDIT, DELETE, etc.)


Agrupaciones:

Por formulario
Por usuario
Por tipo de acción
Por día/hora



MODO 4: Rastrear una ACCIÓN específica

Filtros:

Seleccionar categoría (opcional)
Seleccionar módulo (opcional)
Seleccionar formulario (opcional)
Seleccionar acción específica (LOGIN, CREAR_USUARIO, MODIFICAR_FACTURA, etc.)
Rango de fechas


Resultados:

Quiénes han ejecutado esa acción
Cuándo la ejecutaron
Detalles de cada ejecución


Análisis:

Frecuencia de la acción
Usuarios que más la ejecutan
Horarios más comunes
Tendencia (está aumentando o disminuyendo)



MODOS ADICIONALES SUGERIDOS:
MODO 5: Rastrear por RANGO HORARIO

Buscar todas las acciones entre horas específicas
Ejemplo: "¿Qué pasó entre 18:00 y 22:00 del 15 de diciembre?"
Útil para detectar actividad fuera de horario laboral

MODO 6: Rastrear por IP/MÁQUINA

Buscar todas las acciones desde una IP específica
Buscar todas las acciones desde una máquina específica
Útil para detectar accesos no autorizados

MODO 7: Rastrear CAMBIOS en un REGISTRO específico

Ingresar RegistroID
Ver historial completo de cambios de ese registro
Timeline: Creación → Modificaciones → Estado actual
Quién creó, quién modificó, qué cambió


Interfaz del formulario:

RadioButtons para seleccionar modo de búsqueda
Panel de filtros que cambia según el modo seleccionado
DataGridView con resultados
Panel de resumen/estadísticas
Botones: Buscar, Limpiar, Exportar, Cerrar


Estado actual: Pendiente de implementación (muestra MessageBox informativo)


3. btnReportePDF (Rojo #DC3545)

Texto: "📄 Reporte PDF"
Ubicación: Location(420, 15), Size(180, 40)
Función: Abre FormReportePDFAuditoria (modal)
Características del Reporte PDF:
Configuración del Reporte:

Tipo de reporte:

Reporte Completo (todo lo filtrado actualmente)
Reporte Ejecutivo (resumen con gráficos)
Reporte Detallado (listado completo sin gráficos)
Reporte Comparativo (dos periodos lado a lado)


Opciones de formato:

Orientación: Vertical / Horizontal
Tamaño de página: A4 / Carta / Legal
Incluir portada: Sí / No
Incluir índice: Sí / No
Incluir gráficos: Sí / No
Incluir firmas: Sí / No (espacios para firma de responsables)


Secciones a incluir:

☑ Resumen Ejecutivo
☑ Criterios de búsqueda aplicados
☑ Estadísticas generales
☑ Top 10 acciones más frecuentes
☑ Top 10 usuarios más activos
☑ Detalle de registros (tabla completa)
☑ Resumen por módulo
☑ Resumen por usuario
☑ Gráficos analíticos
☑ Conclusiones y recomendaciones (campo de texto editable)



Vista Previa:

Visor PDF embebido antes de generar
Navegación por páginas
Zoom in/out

Contenido del PDF generado:
Página 1 - Portada:

Logo de la empresa
Título: "REPORTE DE AUDITORÍA - SISTEMA MOFIS ERP"
Subtítulo: "Análisis de Actividad del Sistema"
Periodo del reporte
Generado por: {Usuario}
Fecha de generación
Nivel de confidencialidad

Página 2 - Resumen Ejecutivo:

Total de registros analizados
Periodo de análisis
Criterios de filtrado aplicados
Principales hallazgos (bullets)
Estadísticas clave en cuadros destacados

Página 3 - Gráficos Analíticos:

Gráfico de barras: Acciones por módulo
Gráfico de torta: Distribución por tipo de acción
Gráfico de línea: Tendencia temporal
Gráfico de barras: Top 10 usuarios

Página 4+ - Detalle de Registros:

Tabla con todos los registros filtrados
Headers en cada página
Colores alternados para facilitar lectura
Paginación automática

Última Página - Resumen Final:

Conclusiones
Recomendaciones
Espacio para firmas:

Generado por: _______________
Revisado por: _______________
Fecha: _______________



Opciones de destino:

Guardar como archivo
Imprimir directamente
Enviar por email (futuro)
Guardar en servidor (futuro)


Librería sugerida: iTextSharp o PdfSharp
Estado actual: Pendiente de implementación (muestra MessageBox informativo)


4. btnActualizar (Azul #0078D4)

Texto: "🔄 Actualizar"
Ubicación: Location(920, 15), Size(180, 40)
Función: Recarga datos desde la base de datos

Cuándo usar:

Cuando se sospecha que hay registros nuevos en BD
Después de realizar acciones importantes
Cuando los datos parecen desactualizados


Optimización sugerida:

Agregar timestamp de última actualización
Mostrar cuántos registros nuevos hay antes de recargar
Opción de "solo cargar nuevos" sin resetear filtros


Estado actual: ✅ IMPLEMENTADO Y FUNCIONAL

5. btnExportar (Verde #228B22)

Texto: "📊 Exportar a Excel"
Ubicación: Location(1120, 15), Size(180, 40)
Función: Exporta los datos FILTRADOS ACTUALMENTE a Excel profesional
Comportamiento:

Valida que haya datos para exportar
Genera archivo Excel con 4 hojas:

Hoja 1: "Resumen Ejecutivo"

Título profesional con logo
Información del reporte:

Fecha de generación
Generado por: {Usuario} ({Rol})
Total de registros exportados


Criterios de búsqueda aplicados:

Fecha desde/hasta (si aplica)
Módulo filtrado (si aplica)
Usuario filtrado (si aplica)
Acción filtrada (si aplica)
Texto buscado (si aplica)


Estadísticas generales:

TOP 10 ACCIONES MÁS FRECUENTES (tabla con cantidad)
TOP 10 USUARIOS MÁS ACTIVOS (tabla con cantidad)



Hoja 2: "Detalle Completo"

Encabezados con formato profesional:

Fondo azul corporativo (#0078D4)
Texto blanco en negrita
Altura de fila: 25px


Columnas:

Fecha/Hora (formato: dd/mm/yyyy hh:mm:ss)
Usuario
Categoría
Módulo
Formulario
Acción
Detalle
IP
Máquina


Filas alternadas (color gris claro #F0F0F0)
Bordes en todas las celdas
Anchos de columna ajustados

Hoja 3: "Por Módulo"

Resumen agrupado por módulo
Columnas:

Módulo
Total de Acciones
Primera Acción (fecha/hora)
Última Acción (fecha/hora)


Ordenado por cantidad (mayor a menor)

Hoja 4: "Por Usuario"

Resumen agrupado por usuario
Columnas:

Usuario
Total de Acciones
Primera Acción (fecha/hora)
Última Acción (fecha/hora)


Ordenado por cantidad (mayor a menor)


Abre SaveFileDialog:

Nombre sugerido: Auditoria_Completo_YYYYMMDD_HHmmss.xlsx
Filtro: Solo archivos .xlsx


Guarda el archivo
Registra la exportación en auditoría:

     AuditoriaHelper.RegistrarAccion(
         SesionActual.UsuarioID,
         AuditoriaAcciones.AuditoriaGeneral.EXPORTAR_AUDITORIA_EXCEL,
         AuditoriaAcciones.Categorias.SISTEMA,
         AuditoriaAcciones.Modulos.AUDITORIA_GENERAL,
         "FormAuditoria",
         detalle: "Exportación de {cantidad} registros a Excel"
     );
```
  
  6. Muestra MessageBox de confirmación:
```
     "✅ Reporte exportado exitosamente
     
     Archivo: Auditoria_Completo_20260104_143000.xlsx
     Registros: 1,234
     
     Hojas generadas:
     • Resumen Ejecutivo (estadísticas y top 10)
     • Detalle Completo (todos los registros)
     • Por Módulo (agrupado)
     • Por Usuario (agrupado)"
     
     [OK]
```
  
  7. Pregunta si desea abrir el archivo:
```
     "¿Desea abrir el archivo ahora?"
     [Sí] [No]
```
  
  8. Si confirma → abre Excel con `Process.Start()`

- **Librería usada:** ClosedXML
- **Optimización:** Solo exporta registros filtrados (no los 5000 completos)
- **Estado actual:** ✅ IMPLEMENTADO Y FUNCIONAL

TENGO LAS SIGUIENTES CLASES:

```csharp


## 8. HELPERS Y CLASES AUXILIARES

### 8.1. DatabaseConnection

**Ubicación:** `Classes/DatabaseConnection.cs`

**Propósito:** Centralizar la gestión de conexiones a la base de datos.

```csharp
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace MOFIS_ERP.Classes
{
    /// <summary>
    /// Clase estática para manejar la conexión a SQL Server
    /// Proporciona métodos centralizados para obtener conexiones a la base de datos
    /// </summary>
    public static class DatabaseConnection
    {
        // Nombre de la connection string en App.config
        private const string CONNECTION_STRING_NAME = "FiducorpERP";

        /// <summary>
        /// Obtiene la cadena de conexión desde App.config
        /// </summary>
        private static string ConnectionString
        {
            get
            {
                try
                {
                    var connectionString = ConfigurationManager.ConnectionStrings[CONNECTION_STRING_NAME]?.ConnectionString;

                    if (string.IsNullOrEmpty(connectionString))
                    {
                        throw new InvalidOperationException(
                            $"No se encontró la connection string '{CONNECTION_STRING_NAME}' en App.config");
                    }

                    return connectionString;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Error al obtener la connection string: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Obtiene una nueva conexión a la base de datos
        /// IMPORTANTE: El código que llama este método debe cerrar la conexión (usando 'using')
        /// </summary>
        /// <returns>SqlConnection configurada pero NO abierta</returns>
        public static SqlConnection GetConnection()
        {
            try
            {
                return new SqlConnection(ConnectionString);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Error al crear la conexión a la base de datos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Prueba la conexión a la base de datos
        /// </summary>
        /// <returns>True si la conexión es exitosa, False si falla</returns>
        public static bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return connection.State == System.Data.ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Prueba la conexión y devuelve el mensaje de error si falla
        /// </summary>
        /// <param name="errorMessage">Mensaje de error en caso de fallo</param>
        /// <returns>True si la conexión es exitosa</returns>
        public static bool TestConnection(out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();

                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        errorMessage = "Conexión exitosa";
                        return true;
                    }
                    else
                    {
                        errorMessage = "No se pudo establecer la conexión";
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                errorMessage = $"Error de SQL Server: {sqlEx.Message}";
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error general: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Obtiene el nombre del servidor desde la connection string
        /// </summary>
        public static string GetServerName()
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(ConnectionString);
                return builder.DataSource;
            }
            catch
            {
                return "Desconocido";
            }
        }

        /// <summary>
        /// Obtiene el nombre de la base de datos desde la connection string
        /// </summary>
        public static string GetDatabaseName()
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(ConnectionString);
                return builder.InitialCatalog;
            }
            catch
            {
                return "Desconocido";
            }
        }
    }
}

### 8.2. PermisosHelper

**Ubicación:** `Classes/PermisosHelper.cs`

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace MOFIS_ERP.Classes
{
    /// <summary>
    /// Clase helper para gestión de permisos del sistema
    /// Evalúa permisos basándose en la estructura: Usuario > Rol > Formulario > Acción
    /// </summary>
    public static class PermisosHelper
    {
        /// <summary>
        /// Verifica si el usuario actual tiene permiso para realizar una acción en un formulario
        /// </summary>
        /// <param name="codigoFormulario">Código del formulario (ej: FGEUSR)</param>
        /// <param name="codigoAccion">Código de la acción (ej: VIEW, CREATE, EDIT, DELETE)</param>
        /// <returns>True si tiene permiso, False si no</returns>
        public static bool TienePermiso(string codigoFormulario, string codigoAccion)
        {
            return TienePermiso(SesionActual.UsuarioID, codigoFormulario, codigoAccion);
        }

        /// <summary>
        /// Verifica si un usuario específico tiene permiso para realizar una acción en un formulario
        /// </summary>
        /// <param name="usuarioID">ID del usuario</param>
        /// <param name="codigoFormulario">Código del formulario (ej: FGEUSR)</param>
        /// <param name="codigoAccion">Código de la acción (ej: VIEW, CREATE, EDIT, DELETE)</param>
        /// <returns>True si tiene permiso, False si no</returns>
        public static bool TienePermiso(int usuarioID, string codigoFormulario, string codigoAccion)
        {
            // ROOT siempre tiene acceso total
            if (SesionActual.EsRoot())
                return true;

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // PASO 1: Verificar si existe un permiso ESPECÍFICO para el USUARIO
                    // Esto permite excepciones individuales que sobrescriben el permiso del rol
                    string sqlUsuario = @"
                        SELECT PU.Permitido
                        FROM PermisosUsuario PU
                        INNER JOIN CatalogoFormularios F ON PU.FormularioID = F.FormularioID
                        INNER JOIN CatalogoAcciones A ON PU.AccionID = A.AccionID
                        WHERE PU.UsuarioID = @UsuarioID
                          AND F.CodigoFormulario = @CodigoFormulario
                          AND A.CodigoAccion = @CodigoAccion
                          AND F.Activo = 1
                          AND A.Activo = 1
                          AND (PU.FechaExpiracion IS NULL OR PU.FechaExpiracion > GETDATE())";

                    using (var cmdUsuario = new SqlCommand(sqlUsuario, conn))
                    {
                        cmdUsuario.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        cmdUsuario.Parameters.AddWithValue("@CodigoFormulario", codigoFormulario);
                        cmdUsuario.Parameters.AddWithValue("@CodigoAccion", codigoAccion);

                        var resultadoUsuario = cmdUsuario.ExecuteScalar();

                        // Si existe un permiso específico para el usuario, usarlo
                        if (resultadoUsuario != null)
                        {
                            return Convert.ToBoolean(resultadoUsuario);
                        }
                    }

                    // PASO 2: Si no existe permiso de usuario, verificar permiso del ROL
                    string sqlRol = @"
                        SELECT PR.Permitido
                        FROM PermisosRol PR
                        INNER JOIN Usuarios U ON PR.RolID = U.RolID
                        INNER JOIN CatalogoFormularios F ON PR.FormularioID = F.FormularioID
                        INNER JOIN CatalogoAcciones A ON PR.AccionID = A.AccionID
                        WHERE U.UsuarioID = @UsuarioID
                          AND F.CodigoFormulario = @CodigoFormulario
                          AND A.CodigoAccion = @CodigoAccion
                          AND F.Activo = 1
                          AND A.Activo = 1";

                    using (var cmdRol = new SqlCommand(sqlRol, conn))
                    {
                        cmdRol.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        cmdRol.Parameters.AddWithValue("@CodigoFormulario", codigoFormulario);
                        cmdRol.Parameters.AddWithValue("@CodigoAccion", codigoAccion);

                        var resultadoRol = cmdRol.ExecuteScalar();

                        // Si existe permiso del rol, usarlo
                        if (resultadoRol != null)
                        {
                            return Convert.ToBoolean(resultadoRol);
                        }
                    }

                    // PASO 3: Si no existe ni permiso de usuario ni de rol, DENEGAR por defecto
                    return false;
                }
            }
            catch (Exception ex)
            {
                // En caso de error, denegar acceso por seguridad
                System.Windows.Forms.MessageBox.Show(
                    $"Error al verificar permisos:\n\n{ex.Message}",
                    "Error de Permisos",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error
                );
                return false;
            }
        }

        /// <summary>
        /// Verifica si el usuario tiene múltiples permisos a la vez
        /// Útil para verificar varios permisos de forma eficiente
        /// </summary>
        /// <param name="codigoFormulario">Código del formulario</param>
        /// <param name="codigosAcciones">Array de códigos de acciones</param>
        /// <returns>Diccionario con el resultado de cada acción</returns>
        public static bool[] TienePermisos(string codigoFormulario, params string[] codigosAcciones)
        {
            bool[] resultados = new bool[codigosAcciones.Length];

            for (int i = 0; i < codigosAcciones.Length; i++)
            {
                resultados[i] = TienePermiso(codigoFormulario, codigosAcciones[i]);
            }

            return resultados;
        }

        /// <summary>
        /// Obtiene la información detallada de permisos para un formulario
        /// Útil para configurar la UI de un formulario completo
        /// </summary>
        public static PermisosFormulario ObtenerPermisosFormulario(string codigoFormulario)
        {
            return new PermisosFormulario
            {
                PuedeVer = TienePermiso(codigoFormulario, "VIEW"),
                PuedeCrear = TienePermiso(codigoFormulario, "CREATE"),
                PuedeEditar = TienePermiso(codigoFormulario, "EDIT"),
                PuedeEliminar = TienePermiso(codigoFormulario, "DELETE"),
                PuedeImprimir = TienePermiso(codigoFormulario, "PRINT"),
                PuedeReimprimir = TienePermiso(codigoFormulario, "REPRINT"),
                PuedeExportar = TienePermiso(codigoFormulario, "EXPORT"),
                PuedeAprobar = TienePermiso(codigoFormulario, "APPROVE"),
                PuedeRechazar = TienePermiso(codigoFormulario, "REJECT"),
                PuedeActivar = TienePermiso(codigoFormulario, "ACTIVATE"),
                PuedeResetear = TienePermiso(codigoFormulario, "RESET")
            };
        }
    }

    /// <summary>
    /// Clase que encapsula todos los permisos posibles de un formulario
    /// </summary>
    public class PermisosFormulario
    {
        public bool PuedeVer { get; set; }
        public bool PuedeCrear { get; set; }
        public bool PuedeEditar { get; set; }
        public bool PuedeEliminar { get; set; }
        public bool PuedeImprimir { get; set; }
        public bool PuedeReimprimir { get; set; }
        public bool PuedeExportar { get; set; }
        public bool PuedeAprobar { get; set; }
        public bool PuedeRechazar { get; set; }
        public bool PuedeActivar { get; set; }
        public bool PuedeResetear { get; set; }

        /// <summary>
        /// Verifica si el usuario tiene al menos un permiso
        /// </summary>
        public bool TieneAlgunPermiso()
        {
            return PuedeVer || PuedeCrear || PuedeEditar || PuedeEliminar ||
                   PuedeImprimir || PuedeReimprimir || PuedeExportar ||
                   PuedeAprobar || PuedeRechazar || PuedeActivar || PuedeResetear;
        }

        /// <summary>
        /// Verifica si el usuario tiene permisos completos (todos los CRUD)
        /// </summary>
        public bool TienePermisoCompleto()
        {
            return PuedeVer && PuedeCrear && PuedeEditar && PuedeEliminar;
        }
    }
}

### 8.2. UsuarioDTO

**Ubicación:** `Classes/UsuarioDTO.cs`

using System;

namespace MOFIS_ERP.Classes
{
    /// <summary>
    /// Data Transfer Object para transportar información del usuario
    /// Se usa para pasar datos entre capas sin exponer la estructura de la BD
    /// </summary>
    public class UsuarioDTO
    {
        public int UsuarioID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public int RolID { get; set; }
        public string NombreRol { get; set; }
        public bool Activo { get; set; }
        public bool DebeCambiarPassword { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? UltimoAcceso { get; set; }
    }
}

### 8.2. SesionActual

**Ubicación:** `Classes/SesionActual.cs`

**Propósito:** Almacenar y gestionar la información de la sesión actual del usuario.

```csharp

using System;

namespace MOFIS_ERP.Classes
{
    /// <summary>
    /// Clase estática que mantiene la información del usuario actualmente logueado
    /// Actúa como sesión global de la aplicación
    /// </summary>
    public static class SesionActual
    {
        #region Propiedades del Usuario

        /// <summary>
        /// ID del usuario en la base de datos
        /// </summary>
        public static int UsuarioID { get; set; }

        /// <summary>
        /// Nombre de usuario (login)
        /// </summary>
        public static string Username { get; set; }

        /// <summary>
        /// Nombre completo del usuario
        /// </summary>
        public static string NombreCompleto { get; set; }

        /// <summary>
        /// Email del usuario
        /// </summary>
        public static string Email { get; set; }

        /// <summary>
        /// ID del rol asignado
        /// </summary>
        public static int RolID { get; set; }

        /// <summary>
        /// Nombre del rol (ROOT, ADMIN, CONTADOR)
        /// </summary>
        public static string NombreRol { get; set; }

        /// <summary>
        /// Fecha y hora del login
        /// </summary>
        public static DateTime FechaLogin { get; set; }

        #endregion

        #region Métodos de Control de Sesión

        /// <summary>
        /// Verifica si hay una sesión activa
        /// </summary>
        public static bool HaySesionActiva()
        {
            return UsuarioID > 0 && !string.IsNullOrEmpty(Username);
        }

        /// <summary>
        /// Inicializa la sesión con los datos del usuario
        /// </summary>
        public static void IniciarSesion(int usuarioID, string username, string nombreCompleto,
                                         string email, int rolID, string nombreRol)
        {
            UsuarioID = usuarioID;
            Username = username;
            NombreCompleto = nombreCompleto;
            Email = email;
            RolID = rolID;
            NombreRol = nombreRol;
            FechaLogin = DateTime.Now;
        }

        /// <summary>
        /// Cierra la sesión actual limpiando todos los datos
        /// </summary>
        public static void CerrarSesion()
        {
            UsuarioID = 0;
            Username = string.Empty;
            NombreCompleto = string.Empty;
            Email = string.Empty;
            RolID = 0;
            NombreRol = string.Empty;
            FechaLogin = DateTime.MinValue;
        }

        /// <summary>
        /// Verifica si el usuario actual tiene un rol específico
        /// </summary>
        public static bool TieneRol(string nombreRol)
        {
            return NombreRol.Equals(nombreRol, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Verifica si el usuario actual es ROOT
        /// </summary>
        public static bool EsRoot()
        {
            return TieneRol("ROOT");
        }

        /// <summary>
        /// Verifica si el usuario actual es ADMIN
        /// </summary>
        public static bool EsAdmin()
        {
            return TieneRol("ADMIN");
        }

        /// <summary>
        /// Verifica si el usuario actual es CONTADOR
        /// </summary>
        public static bool EsContador()
        {
            return TieneRol("CONTADOR");
        }

        /// <summary>
        /// Obtiene un resumen de la sesión actual para mostrar en UI
        /// </summary>
        public static string ObtenerResumenSesion()
        {
            if (!HaySesionActiva())
                return "Sin sesión activa";

            return $"{NombreCompleto} ({NombreRol}) - Sesión iniciada: {FechaLogin:dd/MM/yyyy HH:mm}";
        }

        #endregion
    }
}

### 8.3. AutenticacionHelper

**Ubicación:** `Classes/AutenticacionHelper.cs`

using System;
using System.Data.SqlClient;
using BCrypt.Net;

namespace MOFIS_ERP.Classes
{
    /// <summary>
    /// Clase auxiliar para manejar la autenticación de usuarios
    /// </summary>
    public static class AutenticacionHelper
    {
        /// <summary>
        /// Valida las credenciales de un usuario contra la base de datos
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>UsuarioDTO si las credenciales son válidas, null si son inválidas</returns>
        public static UsuarioDTO ValidarUsuario(string username, string password)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"SELECT 
                                    U.UsuarioID,
                                    U.Username,
                                    U.PasswordHash,
                                    U.NombreCompleto,
                                    U.Email,
                                    U.RolID,
                                    R.NombreRol,
                                    U.Activo,
                                    U.DebeCambiarPassword,
                                    U.FechaCreacion,
                                    U.UltimoAcceso
                                  FROM Usuarios U
                                  INNER JOIN Roles R ON U.RolID = R.RolID
                                  WHERE U.Username = @Username";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Verificar si el usuario está activo
                                bool activo = reader.GetBoolean(reader.GetOrdinal("Activo"));
                                if (!activo)
                                {
                                    return null; // Usuario inactivo
                                }

                                // Obtener el hash almacenado
                                string hashAlmacenado = reader.GetString(reader.GetOrdinal("PasswordHash"));

                                // Verificar la contraseña con BCrypt
                                if (BCrypt.Net.BCrypt.Verify(password, hashAlmacenado))
                                {
                                    // Credenciales válidas, crear DTO
                                    var usuario = new UsuarioDTO
                                    {
                                        UsuarioID = reader.GetInt32(reader.GetOrdinal("UsuarioID")),
                                        Username = reader.GetString(reader.GetOrdinal("Username")),
                                        PasswordHash = hashAlmacenado,
                                        NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto")),
                                        Email = reader.IsDBNull(reader.GetOrdinal("Email")) ?
                                                null : reader.GetString(reader.GetOrdinal("Email")),
                                        RolID = reader.GetInt32(reader.GetOrdinal("RolID")),
                                        NombreRol = reader.GetString(reader.GetOrdinal("NombreRol")),
                                        Activo = activo,
                                        DebeCambiarPassword = reader.GetBoolean(reader.GetOrdinal("DebeCambiarPassword")),
                                        FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                                        UltimoAcceso = reader.IsDBNull(reader.GetOrdinal("UltimoAcceso")) ?
                                                       (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UltimoAcceso"))
                                    };

                                    return usuario;
                                }
                            }
                        }
                    }
                }

                // Credenciales inválidas
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al validar usuario: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza la fecha de último acceso del usuario
        /// </summary>
        public static void ActualizarUltimoAcceso(int usuarioID)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = "UPDATE Usuarios SET UltimoAcceso = @UltimoAcceso WHERE UsuarioID = @UsuarioID";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UltimoAcceso", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Ignorar errores al actualizar último acceso
            }
        }

        /// <summary>
        /// Cambia la contraseña de un usuario
        /// </summary>
        /// <param name="usuarioID">ID del usuario</param>
        /// <param name="passwordActual">Contraseña actual en texto plano</param>
        /// <param name="passwordNueva">Nueva contraseña en texto plano</param>
        /// <returns>True si el cambio fue exitoso, False si la contraseña actual es incorrecta</returns>
        public static bool CambiarPassword(int usuarioID, string passwordActual, string passwordNueva)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // 1. Verificar contraseña actual
                    string sqlVerificar = "SELECT PasswordHash FROM Usuarios WHERE UsuarioID = @UsuarioID";
                    string hashAlmacenado;

                    using (var cmd = new SqlCommand(sqlVerificar, conn))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        hashAlmacenado = cmd.ExecuteScalar()?.ToString();
                    }

                    if (string.IsNullOrEmpty(hashAlmacenado))
                        return false;

                    // Verificar contraseña actual con BCrypt
                    if (!BCrypt.Net.BCrypt.Verify(passwordActual, hashAlmacenado))
                        return false;

                    // 2. Actualizar con nueva contraseña
                    string nuevoHash = BCrypt.Net.BCrypt.HashPassword(passwordNueva);
                    string sqlActualizar = @"UPDATE Usuarios 
                                            SET PasswordHash = @PasswordHash, 
                                                DebeCambiarPassword = 0 
                                            WHERE UsuarioID = @UsuarioID";

                    using (var cmd = new SqlCommand(sqlActualizar, conn))
                    {
                        cmd.Parameters.AddWithValue("@PasswordHash", nuevoHash);
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cambiar contraseña: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Valida la fortaleza de una contraseña según los requisitos del sistema
        /// </summary>
        /// <param name="password">Contraseña a validar</param>
        /// <returns>Mensaje de error si la contraseña es débil, null si es válida</returns>
        public static string ValidarFortalezaPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "La contraseña no puede estar vacía";

            if (password.Length < 8)
                return "La contraseña debe tener al menos 8 caracteres";

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]"))
                return "La contraseña debe contener al menos una letra mayúscula";

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]"))
                return "La contraseña debe contener al menos una letra minúscula";

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[0-9]"))
                return "La contraseña debe contener al menos un número";

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]"))
                return "La contraseña debe contener al menos un carácter especial (!@#$%^&*)";

            return null; // Contraseña válida
        }
    }
}

### 8.3. AuditoriaHelper

**Ubicación:** `Classes/AuditoriaHelper.cs`

**Propósito:** Centralizar el registro de acciones en la tabla de auditoría.

```csharp

CARPETA Classes.

using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;

namespace MOFIS_ERP.Classes
{
    /// <summary>
    /// Clase auxiliar para registrar todas las acciones de auditoría en la base de datos
    /// </summary>
    public static class AuditoriaHelper
    {
        /// <summary>
        /// Registra una acción en la tabla de Auditoría
        /// </summary>
        /// <param name="usuarioID">ID del usuario que realiza la acción</param>
        /// <param name="accion">Acción realizada (LOGIN, LOGOUT, CREAR, EDITAR, ELIMINAR, VER, IMPRIMIR)</param>
        /// <param name="categoria">Categoría del sistema (SISTEMA, CONTABILIDAD, etc.)</param>
        /// <param name="modulo">Módulo específico (Usuarios, Roles, Cuentas por Pagar, etc.)</param>
        /// <param name="formulario">Nombre del formulario (opcional)</param>
        /// <param name="registroID">ID del registro afectado (opcional)</param>
        /// <param name="detalle">Información adicional (opcional)</param>
        public static void RegistrarAccion(int usuarioID, string accion, string categoria = null,
                                          string modulo = null, string formulario = null,
                                          int? registroID = null, string detalle = null)
        {
            try
            {
                string ip = ObtenerDireccionIP();
                string nombreMaquina = ObtenerNombreMaquina();

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"INSERT INTO Auditoria 
                                  (UsuarioID, Accion, Categoria, Modulo, Formulario, 
                                   RegistroID, Detalle, FechaHora, DireccionIP, NombreMaquina)
                                  VALUES 
                                  (@UsuarioID, @Accion, @Categoria, @Modulo, @Formulario, 
                                   @RegistroID, @Detalle, @FechaHora, @DireccionIP, @NombreMaquina)";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        cmd.Parameters.AddWithValue("@Accion", accion);
                        cmd.Parameters.AddWithValue("@Categoria", (object)categoria ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Modulo", (object)modulo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Formulario", (object)formulario ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@RegistroID", (object)registroID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Detalle", (object)detalle ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaHora", DateTime.Now);
                        cmd.Parameters.AddWithValue("@DireccionIP", ip);
                        cmd.Parameters.AddWithValue("@NombreMaquina", nombreMaquina);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // En un sistema de producción, aquí se debería registrar el error en un log
                // Por ahora, solo lo ignoramos para no interrumpir la operación principal
                System.Diagnostics.Debug.WriteLine($"Error en auditoría: {ex.Message}");
            }
        }

        /// <summary>
        /// Registra un LOGIN exitoso
        /// </summary>
        public static void RegistrarLogin(int usuarioID, string username)
        {
            RegistrarAccion(usuarioID, "LOGIN", "SISTEMA", "Autenticación", "FormLogin",
                          detalle: $"Usuario {username} inició sesión exitosamente");
        }

        /// <summary>
        /// Registra un LOGOUT
        /// </summary>
        public static void RegistrarLogout(int usuarioID, string username)
        {
            RegistrarAccion(usuarioID, "LOGOUT", "SISTEMA", "Autenticación", "FormLogin",
                          detalle: $"Usuario {username} cerró sesión");
        }

        /// <summary>
        /// Registra un intento de login fallido
        /// </summary>
        public static void RegistrarLoginFallido(string username)
        {
            try
            {
                string ip = ObtenerDireccionIP();
                string nombreMaquina = ObtenerNombreMaquina();

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Para intentos fallidos, usamos UsuarioID = -1 (usuario no válido)
                    string sql = @"INSERT INTO Auditoria 
                                  (UsuarioID, Accion, Categoria, Modulo, Formulario, 
                                   Detalle, FechaHora, DireccionIP, NombreMaquina)
                                  VALUES 
                                  (-1, 'LOGIN_FALLIDO', 'SISTEMA', 'Autenticación', 'FormLogin', 
                                   @Detalle, @FechaHora, @DireccionIP, @NombreMaquina)";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Detalle", $"Intento de login fallido para usuario: {username}");
                        cmd.Parameters.AddWithValue("@FechaHora", DateTime.Now);
                        cmd.Parameters.AddWithValue("@DireccionIP", ip);
                        cmd.Parameters.AddWithValue("@NombreMaquina", nombreMaquina);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Ignorar errores en auditoría de fallos
            }
        }

        /// <summary>
        /// Obtiene la dirección IP local de la máquina
        /// </summary>
        private static string ObtenerDireccionIP()
        {
            try
            {
                string hostName = Dns.GetHostName();
                IPAddress[] addresses = Dns.GetHostAddresses(hostName);

                foreach (IPAddress address in addresses)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return address.ToString();
                    }
                }

                return "127.0.0.1";
            }
            catch
            {
                return "Desconocida";
            }
        }

        /// <summary>
        /// Obtiene el nombre de la máquina
        /// </summary>
        private static string ObtenerNombreMaquina()
        {
            try
            {
                return Environment.MachineName;
            }
            catch
            {
                return "Desconocida";
            }
        }
    }
}


### 8.3. PDFReporteGenerator

**Ubicación:** `Classes/PDFReporteGenerator.cs`

using System;
using System.Data;
using System.IO;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using MOFIS_ERP.Classes;

namespace MOFIS_ERP.Forms.Sistema.Auditoria
{
    /// <summary>
    /// Generador profesional de reportes PDF usando QuestPDF
    /// CORREGIDO: Compatible con C# 7.3 y sin conflictos de color
    /// </summary>
    public class PDFReporteGenerator
    {
        private readonly DataView datos;
        private readonly ConfiguracionReportePDF config;
        private readonly string rutaArchivo;

        // Información de filtros aplicados
        private readonly DateTime? fechaDesde;
        private readonly DateTime? fechaHasta;
        private readonly string moduloFiltrado;
        private readonly string usuarioFiltrado;
        private readonly string accionFiltrada;
        private readonly string textoBuscado;

        // ✅ CORRECCIÓN: Colores como constantes string (compatible con C# 7.3)
        private const string ColorAzul = "#0078D4";
        private const string ColorVerde = "#228B22";
        private const string ColorRojo = "#DC3545";
        private const string ColorGris = "#6C757D";
        private const string ColorGrisClaro = "#F0F0F0";

        public PDFReporteGenerator(
            DataView datos,
            ConfiguracionReportePDF config,
            string rutaArchivo,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            string moduloFiltrado = null,
            string usuarioFiltrado = null,
            string accionFiltrada = null,
            string textoBuscado = null)
        {
            this.datos = datos;
            this.config = config;
            this.rutaArchivo = rutaArchivo;
            this.fechaDesde = fechaDesde;
            this.fechaHasta = fechaHasta;
            this.moduloFiltrado = moduloFiltrado;
            this.usuarioFiltrado = usuarioFiltrado;
            this.accionFiltrada = accionFiltrada;
            this.textoBuscado = textoBuscado;

            // Configurar licencia de QuestPDF (Community License)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// Genera el PDF completo según la configuración
        /// </summary>
        public void Generar()
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Configurar página
                    ConfigurarPagina(page);

                    // Header
                    page.Header().Element(ComposeHeader);

                    // Contenido principal
                    page.Content().Element(ComposeContent);

                    // Footer
                    page.Footer().Element(ComposeFooter);
                });
            })
            .GeneratePdf(rutaArchivo);
        }

        private void ConfigurarPagina(PageDescriptor page)
        {
            // Tamaño de página
            PageSize tamanoPagina;
            switch (config.TamanoPagina)
            {
                case TamanoPagina.A4:
                    tamanoPagina = PageSizes.A4;
                    break;
                case TamanoPagina.Carta:
                    tamanoPagina = PageSizes.Letter;
                    break;
                case TamanoPagina.Legal:
                    tamanoPagina = PageSizes.Legal;
                    break;
                default:
                    tamanoPagina = PageSizes.A4;
                    break;
            }

            page.Size(tamanoPagina);

            // Orientación
            if (config.Orientacion == OrientacionPagina.Horizontal)
            {
                page.Size(tamanoPagina.Landscape());
            }

            // Márgenes
            page.Margin(40);

            // Color de fondo
            page.PageColor(Colors.White);
        }

        // ============================================
        // HEADER
        // ============================================
        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("MOFIS ERP")
                        .FontSize(16)
                        .SemiBold()
                        .FontColor(ColorAzul);

                    column.Item().Text("Sistema de Auditoría")
                        .FontSize(10)
                        .FontColor(ColorGris);
                });

                row.ConstantItem(100).Height(50).Placeholder();
            });
        }

        // ============================================
        // CONTENT
        // ============================================
        private void ComposeContent(IContainer container)
        {
            container.Column(column =>
            {
                // Portada
                if (config.IncluirPortada)
                {
                    column.Item().Element(ComposePortada);
                    column.Item().PageBreak();
                }

                // Índice
                if (config.IncluirIndice)
                {
                    column.Item().Element(ComposeIndice);
                    column.Item().PageBreak();
                }

                // Resumen Ejecutivo
                if (config.IncluirResumenEjecutivo)
                {
                    column.Item().Element(ComposeResumenEjecutivo);
                    column.Item().PageBreak();
                }

                // Criterios de Búsqueda
                if (config.IncluirCriteriosBusqueda)
                {
                    column.Item().Element(ComposeCriteriosBusqueda);
                    column.Item().PageBreak();
                }

                // Detalle de Registros
                if (config.IncluirDetalleRegistros)
                {
                    column.Item().Element(ComposeDetalleRegistros);
                    column.Item().PageBreak();
                }

                // Firmas
                if (config.IncluirFirmas)
                {
                    column.Item().Element(ComposeFirmas);
                }
            });
        }

        // ============================================
        // FOOTER - ✅ CORREGIDO
        // ============================================
        private void ComposeFooter(IContainer container)
        {
            // ✅ CORRECCIÓN: No intentar capturar el resultado de Text()
            container.AlignCenter().Text(text =>
            {
                text.Span("Página ");
                text.CurrentPageNumber();
                text.Span(" de ");
                text.TotalPages();
                text.DefaultTextStyle(style => style.FontSize(9).FontColor(ColorGris));
            });
        }

        // ============================================
        // PORTADA
        // ============================================
        private void ComposePortada(IContainer container)
        {
            container.Column(column =>
            {
                // Espaciado superior
                column.Item().Height(150);

                // Título principal
                column.Item().AlignCenter().Text("MOFIS ERP")
                    .FontSize(48)
                    .Bold()
                    .FontColor(ColorAzul);

                column.Item().PaddingVertical(10);

                // Subtítulo
                column.Item().AlignCenter().Text("REPORTE DE AUDITORÍA DEL SISTEMA")
                    .FontSize(24)
                    .SemiBold();

                column.Item().PaddingVertical(30);

                // Información
                column.Item().AlignCenter().Column(info =>
                {
                    info.Item().Text(string.Format("Periodo: {0}", ObtenerPeriodoTexto()))
                        .FontSize(14);

                    info.Item().PaddingVertical(5);

                    info.Item().Text(string.Format("Generado: {0:dd/MM/yyyy HH:mm:ss}", DateTime.Now))
                        .FontSize(12);

                    info.Item().Text(string.Format("Por: {0} ({1})", SesionActual.NombreCompleto, SesionActual.Username))
                        .FontSize(12);
                });

                column.Item().PaddingVertical(50);

                // Confidencial
                column.Item().AlignCenter().Border(2, ColorRojo).Padding(10)
                    .Text("CONFIDENCIAL")
                    .FontSize(16)
                    .Bold()
                    .FontColor(ColorRojo);
            });
        }

        // ============================================
        // ÍNDICE
        // ============================================
        private void ComposeIndice(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text("ÍNDICE")
                    .FontSize(24)
                    .Bold()
                    .FontColor(ColorAzul);

                column.Item().PaddingVertical(20);

                // Tabla de contenidos
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(4);
                        columns.ConstantColumn(50);
                    });

                    int pagina = 3;
                    AgregarLineaIndice(table, "1. Resumen Ejecutivo", pagina++);
                    AgregarLineaIndice(table, "2. Criterios de Filtrado", pagina++);
                    AgregarLineaIndice(table, "3. Detalle de Registros", pagina++);
                    AgregarLineaIndice(table, "4. Firmas y Aprobaciones", pagina++);
                });
            });
        }

        private void AgregarLineaIndice(TableDescriptor table, string seccion, int pagina)
        {
            table.Cell().PaddingVertical(5).Text(seccion).FontSize(12);
            table.Cell().AlignRight().PaddingVertical(5).Text(pagina.ToString()).FontSize(12);
        }

        // ============================================
        // RESUMEN EJECUTIVO
        // ============================================
        private void ComposeResumenEjecutivo(IContainer container)
        {
            container.Column(column =>
            {
                // Título
                column.Item().Text("RESUMEN EJECUTIVO")
                    .FontSize(20)
                    .Bold()
                    .FontColor(ColorAzul);

                column.Item().PaddingVertical(15);

                // Estadísticas (tarjetas)
                column.Item().Row(row =>
                {
                    // Tarjeta 1: Total de Registros
                    row.RelativeItem().Border(2, ColorAzul).Background(Colors.Blue.Lighten4).Padding(20)
                        .Column(col =>
                        {
                            col.Item().AlignCenter().Text(datos.Count.ToString())
                                .FontSize(36)
                                .Bold()
                                .FontColor(ColorAzul);

                            col.Item().AlignCenter().Text("Total de Registros")
                                .FontSize(12);
                        });

                    row.ConstantItem(20); // Espacio

                    // Tarjeta 2: Módulos Únicos
                    var modulosUnicos = datos.ToTable().AsEnumerable()
                        .Select(r => r.Field<string>("Modulo"))
                        .Where(m => !string.IsNullOrEmpty(m))
                        .Distinct()
                        .Count();

                    row.RelativeItem().Border(2, ColorVerde).Background(Colors.Green.Lighten4).Padding(20)
                        .Column(col =>
                        {
                            col.Item().AlignCenter().Text(modulosUnicos.ToString())
                                .FontSize(36)
                                .Bold()
                                .FontColor(ColorVerde);

                            col.Item().AlignCenter().Text("Módulos Activos")
                                .FontSize(12);
                        });
                });

                column.Item().PaddingVertical(20);

                // TOP 10 ACCIONES
                if (config.IncluirTop10)
                {
                    column.Item().Element(ComposeTop10Acciones);
                    column.Item().PaddingVertical(15);
                    column.Item().Element(ComposeTop10Usuarios);
                }
            });
        }

        private void ComposeTop10Acciones(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text("TOP 10 ACCIONES MÁS FRECUENTES")
                    .FontSize(14)
                    .Bold()
                    .FontColor(ColorAzul);

                column.Item().PaddingVertical(10);

                var accionesPorTipo = datos.ToTable().AsEnumerable()
                    .GroupBy(r => r.Field<string>("Accion"))
                    .Select(g => new { Accion = g.Key ?? "N/A", Cantidad = g.Count() })
                    .OrderByDescending(x => x.Cantidad)
                    .Take(10)
                    .ToList();

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(40);  // #
                        columns.RelativeColumn(3);   // Acción
                        columns.RelativeColumn(1);   // Cantidad
                        columns.RelativeColumn(1);   // %
                    });

                    // Headers
                    table.Header(header =>
                    {
                        header.Cell().Background(ColorAzul).Padding(5).Text("#").FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorAzul).Padding(5).Text("Acción").FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorAzul).Padding(5).Text("Cantidad").FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorAzul).Padding(5).Text("%").FontColor(Colors.White).Bold();
                    });

                    // Datos
                    int ranking = 1;
                    foreach (var item in accionesPorTipo)
                    {
                        double porcentaje = (double)item.Cantidad / datos.Count * 100;

                        // ✅ CORRECCIÓN: Variable explícita para el color
                        string bgColor = (ranking % 2 == 0) ? ColorGrisClaro : Colors.White.ToString();

                        table.Cell().Background(bgColor).Padding(5).AlignCenter().Text(ranking.ToString());
                        table.Cell().Background(bgColor).Padding(5).Text(item.Accion);
                        table.Cell().Background(bgColor).Padding(5).AlignCenter().Text(item.Cantidad.ToString());
                        table.Cell().Background(bgColor).Padding(5).AlignCenter().Text(string.Format("{0:F2}%", porcentaje));

                        ranking++;
                    }
                });
            });
        }

        private void ComposeTop10Usuarios(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text("TOP 10 USUARIOS MÁS ACTIVOS")
                    .FontSize(14)
                    .Bold()
                    .FontColor(ColorVerde);

                column.Item().PaddingVertical(10);

                var usuariosMasActivos = datos.ToTable().AsEnumerable()
                    .GroupBy(r => r.Field<string>("Username") ?? "N/A")
                    .Select(g => new { Usuario = g.Key, Cantidad = g.Count() })
                    .OrderByDescending(x => x.Cantidad)
                    .Take(10)
                    .ToList();

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(40);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(ColorVerde).Padding(5).Text("#").FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorVerde).Padding(5).Text("Usuario").FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorVerde).Padding(5).Text("Acciones").FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorVerde).Padding(5).Text("%").FontColor(Colors.White).Bold();
                    });

                    int ranking = 1;
                    foreach (var item in usuariosMasActivos)
                    {
                        double porcentaje = (double)item.Cantidad / datos.Count * 100;

                        string bgColor = (ranking % 2 == 0) ? ColorGrisClaro : Colors.White.ToString();

                        table.Cell().Background(bgColor).Padding(5).AlignCenter().Text(ranking.ToString());
                        table.Cell().Background(bgColor).Padding(5).Text(item.Usuario);
                        table.Cell().Background(bgColor).Padding(5).AlignCenter().Text(item.Cantidad.ToString());
                        table.Cell().Background(bgColor).Padding(5).AlignCenter().Text(string.Format("{0:F2}%", porcentaje));

                        ranking++;
                    }
                });
            });
        }

        // ============================================
        // CRITERIOS DE BÚSQUEDA
        // ============================================
        private void ComposeCriteriosBusqueda(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text("CRITERIOS DE FILTRADO APLICADOS")
                    .FontSize(16)
                    .Bold()
                    .FontColor(ColorAzul);

                column.Item().PaddingVertical(15);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(2);
                    });

                    bool hayCriterios = false;

                    if (fechaDesde.HasValue)
                    {
                        AgregarCriterio(table, "Fecha Desde:", fechaDesde.Value.ToString("dd/MM/yyyy"));
                        hayCriterios = true;
                    }

                    if (fechaHasta.HasValue)
                    {
                        AgregarCriterio(table, "Fecha Hasta:", fechaHasta.Value.ToString("dd/MM/yyyy"));
                        hayCriterios = true;
                    }

                    if (!string.IsNullOrWhiteSpace(moduloFiltrado))
                    {
                        AgregarCriterio(table, "Módulo:", moduloFiltrado);
                        hayCriterios = true;
                    }

                    if (!string.IsNullOrWhiteSpace(usuarioFiltrado))
                    {
                        AgregarCriterio(table, "Usuario:", usuarioFiltrado);
                        hayCriterios = true;
                    }

                    if (!string.IsNullOrWhiteSpace(accionFiltrada))
                    {
                        AgregarCriterio(table, "Acción:", accionFiltrada);
                        hayCriterios = true;
                    }

                    if (!string.IsNullOrWhiteSpace(textoBuscado))
                    {
                        AgregarCriterio(table, "Búsqueda:", textoBuscado);
                        hayCriterios = true;
                    }

                    if (!hayCriterios)
                    {
                        table.Cell().ColumnSpan(2).Padding(10)
                            .Text("Sin filtros aplicados (mostrando todos los registros)")
                            .Italic()
                            .FontColor(ColorGris);
                    }
                });
            });
        }

        private void AgregarCriterio(TableDescriptor table, string etiqueta, string valor)
        {
            table.Cell().Background(ColorGrisClaro).Padding(8).Text(etiqueta).Bold();
            table.Cell().Padding(8).Text(valor);
        }

        // ============================================
        // DETALLE DE REGISTROS
        // ============================================
        private void ComposeDetalleRegistros(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text("DETALLE COMPLETO DE REGISTROS")
                    .FontSize(16)
                    .Bold()
                    .FontColor(ColorAzul);

                column.Item().PaddingVertical(15);

                // Limitar a 1000 registros para no saturar el PDF
                int maxRegistros = Math.Min(datos.Count, 1000);

                column.Item().Table(table =>
                {
                    // Definir columnas según orientación
                    if (config.Orientacion == OrientacionPagina.Horizontal)
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1.2f); // Fecha/Hora
                            columns.RelativeColumn(1);    // Usuario
                            columns.RelativeColumn(1);    // Módulo
                            columns.RelativeColumn(1.5f); // Formulario
                            columns.RelativeColumn(1.5f); // Acción
                            columns.RelativeColumn(3);    // Detalle
                        });
                    }
                    else
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1.5f); // Fecha/Hora
                            columns.RelativeColumn(1);    // Usuario
                            columns.RelativeColumn(1);    // Acción
                            columns.RelativeColumn(3);    // Detalle
                        });
                    }

                    // Headers
                    table.Header(header =>
                    {
                        header.Cell().Background(ColorAzul).Padding(5).Text("Fecha/Hora").FontSize(8).FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorAzul).Padding(5).Text("Usuario").FontSize(8).FontColor(Colors.White).Bold();

                        if (config.Orientacion == OrientacionPagina.Horizontal)
                        {
                            header.Cell().Background(ColorAzul).Padding(5).Text("Módulo").FontSize(8).FontColor(Colors.White).Bold();
                            header.Cell().Background(ColorAzul).Padding(5).Text("Formulario").FontSize(8).FontColor(Colors.White).Bold();
                        }

                        header.Cell().Background(ColorAzul).Padding(5).Text("Acción").FontSize(8).FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorAzul).Padding(5).Text("Detalle").FontSize(8).FontColor(Colors.White).Bold();
                    });

                    // Datos
                    for (int i = 0; i < maxRegistros; i++)
                    {
                        DataRowView rowView = datos[i];
                        DataRow row = rowView.Row;

                        string bgColor = (i % 2 == 0) ? Colors.White.ToString() : ColorGrisClaro;

                        table.Cell().Background(bgColor).Padding(4).Text(ObtenerValor(row, "FechaHora")).FontSize(7);
                        table.Cell().Background(bgColor).Padding(4).Text(ObtenerValor(row, "Username")).FontSize(7);

                        if (config.Orientacion == OrientacionPagina.Horizontal)
                        {
                            table.Cell().Background(bgColor).Padding(4).Text(ObtenerValor(row, "Modulo")).FontSize(7);
                            table.Cell().Background(bgColor).Padding(4).Text(ObtenerValor(row, "Formulario")).FontSize(7);
                        }

                        table.Cell().Background(bgColor).Padding(4).Text(ObtenerValor(row, "Accion")).FontSize(7);
                        table.Cell().Background(bgColor).Padding(4).Text(ObtenerValor(row, "Detalle")).FontSize(7);
                    }
                });

                // Nota si hay más registros
                if (datos.Count > 1000)
                {
                    column.Item().PaddingTop(10).Text(string.Format("Nota: Se muestran los primeros 1,000 registros de un total de {0:N0}. Para ver el detalle completo, utilice la exportación a Excel.", datos.Count))
                        .FontSize(9)
                        .Italic()
                        .FontColor(ColorGris);
                }
            });
        }

        // ============================================
        // FIRMAS
        // ============================================
        private void ComposeFirmas(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().PaddingTop(100).Text("FIRMAS Y APROBACIONES")
                    .FontSize(16)
                    .Bold()
                    .FontColor(ColorAzul);

                column.Item().PaddingVertical(30);

                column.Item().Row(row =>
                {
                    // Generado por
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().AlignCenter().Text("Generado por:").Bold();
                        col.Item().PaddingTop(5).AlignCenter().Text(SesionActual.NombreCompleto);
                        col.Item().PaddingTop(40).AlignCenter().Text("_____________________");
                        col.Item().PaddingTop(5).AlignCenter().Text("Firma").FontSize(8);
                    });

                    row.ConstantItem(50);

                    // Revisado por
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().AlignCenter().Text("Revisado por:").Bold();
                        col.Item().PaddingTop(45).AlignCenter().Text("_____________________");
                        col.Item().PaddingTop(5).AlignCenter().Text("Firma").FontSize(8);
                    });
                });

                column.Item().PaddingTop(30).AlignCenter()
                    .Text(string.Format("Fecha: {0:dd/MM/yyyy}", DateTime.Now));
            });
        }

        // ============================================
        // HELPERS
        // ============================================
        private string ObtenerValor(DataRow row, string columna)
        {
            try
            {
                if (row[columna] == DBNull.Value) return "";

                if (columna == "FechaHora" && row[columna] is DateTime fecha)
                {
                    return fecha.ToString("dd/MM/yyyy HH:mm");
                }

                return row[columna].ToString();
            }
            catch
            {
                return "";
            }
        }

        private string ObtenerPeriodoTexto()
        {
            if (fechaDesde.HasValue && fechaHasta.HasValue)
            {
                return string.Format("{0:dd/MM/yyyy} - {1:dd/MM/yyyy}", fechaDesde.Value, fechaHasta.Value);
            }
            else if (fechaDesde.HasValue)
            {
                return string.Format("Desde {0:dd/MM/yyyy}", fechaDesde.Value);
            }
            else if (fechaHasta.HasValue)
            {
                return string.Format("Hasta {0:dd/MM/yyyy}", fechaHasta.Value);
            }
            else
            {
                return "Todos los registros";
            }
        }
    }
}

PARA BASE DE DATOS TENGO LO SIGUIENTE:

CARPETAS: Database

Carpeta: 01_Schema

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

-- ============================================================================
-- SCRIPT 2: CREAR TABLA ROLES
-- ============================================================================
-- Descripción: Crea la tabla para almacenar los roles del sistema
-- Roles del sistema: ROOT, ADMIN, CONTADOR
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla Roles ya existe. Eliminándola...';
    DROP TABLE dbo.Roles;
    PRINT '✓ Tabla Roles eliminada';
END
GO

-- Crear tabla Roles
CREATE TABLE dbo.Roles
(
    RolID INT IDENTITY(1,1) NOT NULL,
    NombreRol NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(200) NULL,
    Categoria NVARCHAR(100) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    
    -- Constraints
    CONSTRAINT PK_Roles PRIMARY KEY CLUSTERED (RolID),
    CONSTRAINT UQ_Roles_NombreRol UNIQUE (NombreRol)
);
GO

-- Crear índices
CREATE NONCLUSTERED INDEX IX_Roles_Activo 
ON dbo.Roles(Activo);
GO

CREATE NONCLUSTERED INDEX IX_Roles_Categoria 
ON dbo.Roles(Categoria);
GO

PRINT '✓ Tabla Roles creada exitosamente';
PRINT '✓ Índices creados';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: Roles';
PRINT 'Columnas: RolID, NombreRol, Descripcion, Categoria, Activo, FechaCreacion';
PRINT 'Estado: CREADA Y LISTA';
PRINT 'Siguiente paso: Ejecutar Script 3 - Crear tabla Usuarios';
PRINT '=======================================================';
GO

-- ============================================================================
-- SCRIPT 3: CREAR TABLA USUARIOS
-- ============================================================================
-- Descripción: Crea la tabla para almacenar los usuarios del sistema
-- Incluye control de contraseñas, roles y auditoría básica
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.Usuarios', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla Usuarios ya existe. Eliminándola...';
    DROP TABLE dbo.Usuarios;
    PRINT '✓ Tabla Usuarios eliminada';
END
GO

-- Crear tabla Usuarios
CREATE TABLE dbo.Usuarios
(
    UsuarioID INT IDENTITY(1,1) NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    NombreCompleto NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NULL,
    RolID INT NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    DebeCambiarPassword BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    UltimoAcceso DATETIME NULL,
    CreadoPorUsuarioID INT NULL,
    FechaModificacion DATETIME NULL,
    ModificadoPorUsuarioID INT NULL,
    
    -- Constraints
    CONSTRAINT PK_Usuarios PRIMARY KEY CLUSTERED (UsuarioID),
    CONSTRAINT UQ_Usuarios_Username UNIQUE (Username),
    CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (RolID) 
        REFERENCES dbo.Roles(RolID),
    CONSTRAINT FK_Usuarios_CreadoPor FOREIGN KEY (CreadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT FK_Usuarios_ModificadoPor FOREIGN KEY (ModificadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID)
);
GO

-- Crear índices
CREATE NONCLUSTERED INDEX IX_Usuarios_Username 
ON dbo.Usuarios(Username);
GO

CREATE NONCLUSTERED INDEX IX_Usuarios_RolID 
ON dbo.Usuarios(RolID);
GO

CREATE NONCLUSTERED INDEX IX_Usuarios_Activo 
ON dbo.Usuarios(Activo);
GO

CREATE NONCLUSTERED INDEX IX_Usuarios_Email 
ON dbo.Usuarios(Email);
GO

PRINT '✓ Tabla Usuarios creada exitosamente';
PRINT '✓ Relaciones (Foreign Keys) establecidas con tabla Roles';
PRINT '✓ Índices creados';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: Usuarios';
PRINT 'Columnas principales:';
PRINT '  - UsuarioID (PK)';
PRINT '  - Username (Unique)';
PRINT '  - PasswordHash (BCrypt)';
PRINT '  - NombreCompleto';
PRINT '  - RolID (FK → Roles)';
PRINT '  - DebeCambiarPassword (BIT)';
PRINT '  - Activo (BIT)';
PRINT 'Estado: CREADA Y LISTA';
PRINT 'Siguiente paso: Ejecutar Script 4 - Crear tabla Permisos';
PRINT '=======================================================';
GO

-- ============================================================================
-- SCRIPT 4: CREAR TABLA PERMISOS
-- ============================================================================
-- Descripción: Crea la tabla para almacenar los permisos por rol
-- Define qué acciones puede realizar cada rol en cada módulo/formulario
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.Permisos', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla Permisos ya existe. Eliminándola...';
    DROP TABLE dbo.Permisos;
    PRINT '✓ Tabla Permisos eliminada';
END
GO

-- Crear tabla Permisos
CREATE TABLE dbo.Permisos
(
    PermisoID INT IDENTITY(1,1) NOT NULL,
    RolID INT NOT NULL,
    Categoria NVARCHAR(50) NOT NULL,           -- Ej: SISTEMA, CONTABILIDAD, GERENCIA FINANCIERA
    Modulo NVARCHAR(50) NOT NULL,              -- Ej: Cuentas por Pagar, Usuarios, Roles
    Formulario NVARCHAR(100) NULL,             -- Ej: FormSolicitudPago, FormUsuarios (opcional)
    Accion NVARCHAR(50) NOT NULL,              -- Ej: Ver, Crear, Editar, Eliminar, Imprimir
    Permitido BIT NOT NULL DEFAULT 0,          -- 1 = Permitido, 0 = Denegado
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    CreadoPorUsuarioID INT NULL,
    
    -- Constraints
    CONSTRAINT PK_Permisos PRIMARY KEY CLUSTERED (PermisoID),
    CONSTRAINT FK_Permisos_Roles FOREIGN KEY (RolID) 
        REFERENCES dbo.Roles(RolID),
    CONSTRAINT FK_Permisos_CreadoPor FOREIGN KEY (CreadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    
    -- Constraint para evitar duplicados
    CONSTRAINT UQ_Permisos_Unico UNIQUE (RolID, Categoria, Modulo, Formulario, Accion)
);
GO

-- Crear índices para consultas rápidas
CREATE NONCLUSTERED INDEX IX_Permisos_RolID 
ON dbo.Permisos(RolID);
GO

CREATE NONCLUSTERED INDEX IX_Permisos_Categoria 
ON dbo.Permisos(Categoria);
GO

CREATE NONCLUSTERED INDEX IX_Permisos_Modulo 
ON dbo.Permisos(Modulo);
GO

CREATE NONCLUSTERED INDEX IX_Permisos_Accion 
ON dbo.Permisos(Accion);
GO

-- Índice compuesto para búsquedas de permisos
CREATE NONCLUSTERED INDEX IX_Permisos_Busqueda 
ON dbo.Permisos(RolID, Categoria, Modulo, Accion) 
INCLUDE (Permitido);
GO

PRINT '✓ Tabla Permisos creada exitosamente';
PRINT '✓ Relaciones (Foreign Keys) establecidas';
PRINT '✓ Índices creados para optimizar consultas';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: Permisos';
PRINT 'Columnas principales:';
PRINT '  - PermisoID (PK)';
PRINT '  - RolID (FK → Roles)';
PRINT '  - Categoria (SISTEMA, CONTABILIDAD, etc.)';
PRINT '  - Modulo (Cuentas por Pagar, Usuarios, etc.)';
PRINT '  - Formulario (Opcional)';
PRINT '  - Accion (Ver, Crear, Editar, Eliminar, etc.)';
PRINT '  - Permitido (BIT)';
PRINT 'Estado: CREADA Y LISTA';
PRINT 'Siguiente paso: Ejecutar Script 5 - Crear tabla Auditoria';
PRINT '=======================================================';
GO

-- ============================================================================
-- SCRIPT 5: CREAR TABLA AUDITORIA
-- ============================================================================
-- Descripción: Crea la tabla para registrar todas las acciones de los usuarios
-- Fundamental para trazabilidad y cumplimiento normativo
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.Auditoria', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla Auditoria ya existe. Eliminándola...';
    DROP TABLE dbo.Auditoria;
    PRINT '✓ Tabla Auditoria eliminada';
END
GO

-- Crear tabla Auditoria
CREATE TABLE dbo.Auditoria
(
    AuditoriaID BIGINT IDENTITY(1,1) NOT NULL,
    UsuarioID INT NOT NULL,
    Accion NVARCHAR(50) NOT NULL,              -- LOGIN, LOGOUT, CREAR, EDITAR, ELIMINAR, VER, IMPRIMIR
    Categoria NVARCHAR(50) NULL,               -- SISTEMA, CONTABILIDAD, GERENCIA FINANCIERA
    Modulo NVARCHAR(50) NULL,                  -- Usuarios, Roles, Cuentas por Pagar, etc.
    Formulario NVARCHAR(100) NULL,             -- FormUsuarios, FormSolicitudPago, etc.
    RegistroID INT NULL,                       -- ID del registro afectado (si aplica)
    Detalle NVARCHAR(MAX) NULL,                -- Información adicional en formato JSON o texto
    FechaHora DATETIME NOT NULL DEFAULT GETDATE(),
    DireccionIP NVARCHAR(50) NULL,             -- IP desde donde se realizó la acción
    NombreMaquina NVARCHAR(100) NULL,          -- Nombre de la PC cliente
    
    -- Constraints
    CONSTRAINT PK_Auditoria PRIMARY KEY CLUSTERED (AuditoriaID),
    CONSTRAINT FK_Auditoria_Usuarios FOREIGN KEY (UsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID)
);
GO

-- Crear índices para consultas de auditoría
CREATE NONCLUSTERED INDEX IX_Auditoria_UsuarioID 
ON dbo.Auditoria(UsuarioID);
GO

CREATE NONCLUSTERED INDEX IX_Auditoria_FechaHora 
ON dbo.Auditoria(FechaHora DESC);
GO

CREATE NONCLUSTERED INDEX IX_Auditoria_Accion 
ON dbo.Auditoria(Accion);
GO

CREATE NONCLUSTERED INDEX IX_Auditoria_Modulo 
ON dbo.Auditoria(Modulo);
GO

-- Índice compuesto para búsquedas frecuentes
CREATE NONCLUSTERED INDEX IX_Auditoria_Usuario_Fecha 
ON dbo.Auditoria(UsuarioID, FechaHora DESC) 
INCLUDE (Accion, Modulo, Formulario);
GO

-- Índice para búsqueda por registro afectado
CREATE NONCLUSTERED INDEX IX_Auditoria_RegistroID 
ON dbo.Auditoria(RegistroID, Modulo);
GO

PRINT '✓ Tabla Auditoria creada exitosamente';
PRINT '✓ Relación (Foreign Key) establecida con Usuarios';
PRINT '✓ Índices creados para optimizar consultas de auditoría';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: Auditoria';
PRINT 'Columnas principales:';
PRINT '  - AuditoriaID (PK, BIGINT para gran volumen)';
PRINT '  - UsuarioID (FK → Usuarios)';
PRINT '  - Accion (LOGIN, CREAR, EDITAR, etc.)';
PRINT '  - Categoria, Modulo, Formulario';
PRINT '  - RegistroID (ID del registro afectado)';
PRINT '  - Detalle (información adicional)';
PRINT '  - FechaHora';
PRINT '  - DireccionIP, NombreMaquina';
PRINT 'Estado: CREADA Y LISTA';
PRINT '';
PRINT '=======================================================';
PRINT '🎉 ESQUEMA COMPLETADO';
PRINT '=======================================================';
PRINT 'Todas las tablas del esquema han sido creadas:';
PRINT '  ✓ Roles';
PRINT '  ✓ Usuarios';
PRINT '  ✓ Permisos';
PRINT '  ✓ Auditoria';
PRINT '';
PRINT 'Siguiente paso: Insertar datos iniciales';
PRINT '  - Script 6: Insertar Roles (ROOT, ADMIN, CONTADOR)';
PRINT '  - Script 7: Crear usuario ROOT inicial';
PRINT '=======================================================';
GO

-- ============================================================================
-- SCRIPT 6: CREAR TABLA CATALOGO CATEGORIAS
-- ============================================================================
-- Descripción: Crea la tabla maestra de categorías del sistema
-- Define las grandes áreas funcionales (SISTEMA, CONTABILIDAD, etc.)
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.CatalogoCategorias', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla CatalogoCategorias ya existe. Eliminándola...';
    DROP TABLE dbo.CatalogoCategorias;
    PRINT '✓ Tabla CatalogoCategorias eliminada';
END
GO

-- Crear tabla CatalogoCategorias
CREATE TABLE dbo.CatalogoCategorias
(
    CategoriaID INT IDENTITY(1,1) NOT NULL,
    CodigoCategoria NVARCHAR(20) NOT NULL,     -- SYS, CONT, GERFIN, GERLEG, DEV
    NombreCategoria NVARCHAR(100) NOT NULL,    -- SISTEMA, CONTABILIDAD, etc.
    Descripcion NVARCHAR(500) NULL,
    Icono NVARCHAR(10) NULL,                   -- Emoji para UI
    OrdenVisualizacion INT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    
    -- Constraints
    CONSTRAINT PK_CatalogoCategorias PRIMARY KEY CLUSTERED (CategoriaID),
    CONSTRAINT UQ_CatalogoCategorias_Codigo UNIQUE (CodigoCategoria),
    CONSTRAINT UQ_CatalogoCategorias_Nombre UNIQUE (NombreCategoria)
);
GO

-- Crear índices
CREATE NONCLUSTERED INDEX IX_CatalogoCategorias_Activo 
ON dbo.CatalogoCategorias(Activo);
GO

CREATE NONCLUSTERED INDEX IX_CatalogoCategorias_Orden 
ON dbo.CatalogoCategorias(OrdenVisualizacion);
GO

PRINT '✓ Tabla CatalogoCategorias creada exitosamente';
PRINT '✓ Índices creados';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: CatalogoCategorias';
PRINT 'Columnas: CategoriaID, CodigoCategoria, NombreCategoria, Icono, Orden';
PRINT 'Estado: CREADA Y LISTA';
PRINT '=======================================================';
GO

-- ============================================================================
-- SCRIPT 7: CREAR TABLA CATALOGO MODULOS
-- ============================================================================
-- Descripción: Crea la tabla maestra de módulos del sistema
-- Define los módulos dentro de cada categoría
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.CatalogoModulos', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla CatalogoModulos ya existe. Eliminándola...';
    DROP TABLE dbo.CatalogoModulos;
    PRINT '✓ Tabla CatalogoModulos eliminada';
END
GO

-- Crear tabla CatalogoModulos
CREATE TABLE dbo.CatalogoModulos
(
    ModuloID INT IDENTITY(1,1) NOT NULL,
    CategoriaID INT NOT NULL,
    CodigoModulo NVARCHAR(20) NOT NULL,        -- GEUSR, CXP, IMP, REC
    NombreModulo NVARCHAR(100) NOT NULL,       -- Gestión de Usuarios, Cuentas por Pagar
    Descripcion NVARCHAR(500) NULL,
    Icono NVARCHAR(10) NULL,                   -- Emoji para UI
    OrdenVisualizacion INT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    
    -- Constraints
    CONSTRAINT PK_CatalogoModulos PRIMARY KEY CLUSTERED (ModuloID),
    CONSTRAINT UQ_CatalogoModulos_Codigo UNIQUE (CodigoModulo),
    CONSTRAINT FK_CatalogoModulos_Categoria FOREIGN KEY (CategoriaID) 
        REFERENCES dbo.CatalogoCategorias(CategoriaID)
);
GO

-- Crear índices
CREATE NONCLUSTERED INDEX IX_CatalogoModulos_CategoriaID 
ON dbo.CatalogoModulos(CategoriaID);
GO

CREATE NONCLUSTERED INDEX IX_CatalogoModulos_Activo 
ON dbo.CatalogoModulos(Activo);
GO

CREATE NONCLUSTERED INDEX IX_CatalogoModulos_Orden 
ON dbo.CatalogoModulos(OrdenVisualizacion);
GO

PRINT '✓ Tabla CatalogoModulos creada exitosamente';
PRINT '✓ Relación con CatalogoCategorias establecida';
PRINT '✓ Índices creados';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: CatalogoModulos';
PRINT 'Columnas: ModuloID, CategoriaID, CodigoModulo, NombreModulo';
PRINT 'Estado: CREADA Y LISTA';
PRINT '=======================================================';
GO

-- ============================================================================
-- SCRIPT 8: CREAR TABLA CATALOGO FORMULARIOS
-- ============================================================================
-- Descripción: Crea la tabla maestra de formularios del sistema
-- Define todos los formularios/pantallas con código único para permisos
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.CatalogoFormularios', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla CatalogoFormularios ya existe. Eliminándola...';
    DROP TABLE dbo.CatalogoFormularios;
    PRINT '✓ Tabla CatalogoFormularios eliminada';
END
GO

-- Crear tabla CatalogoFormularios
CREATE TABLE dbo.CatalogoFormularios
(
    FormularioID INT IDENTITY(1,1) NOT NULL,
    ModuloID INT NOT NULL,
    CodigoFormulario NVARCHAR(20) NOT NULL,    -- FGEUSR, FSOLPAG, FCARTRET
    NombreFormulario NVARCHAR(100) NOT NULL,   -- Gestión de Usuarios, Solicitud de Pago
    NombreClase NVARCHAR(100) NOT NULL,        -- FormGestionUsuarios, FormSolicitudPago
    RutaCompleta NVARCHAR(200) NULL,           -- Sistema > Gestión de Usuarios
    Descripcion NVARCHAR(500) NULL,
    OrdenVisualizacion INT NOT NULL DEFAULT 0,
    EsReporte BIT NOT NULL DEFAULT 0,          -- Distinguir formularios de reportes
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    CreadoPorUsuarioID INT NULL,
    
    -- Constraints
    CONSTRAINT PK_CatalogoFormularios PRIMARY KEY CLUSTERED (FormularioID),
    CONSTRAINT UQ_CatalogoFormularios_Codigo UNIQUE (CodigoFormulario),
    CONSTRAINT UQ_CatalogoFormularios_NombreClase UNIQUE (NombreClase),
    CONSTRAINT FK_CatalogoFormularios_Modulo FOREIGN KEY (ModuloID) 
        REFERENCES dbo.CatalogoModulos(ModuloID),
    CONSTRAINT FK_CatalogoFormularios_CreadoPor FOREIGN KEY (CreadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID)
);
GO

-- Crear índices
CREATE NONCLUSTERED INDEX IX_CatalogoFormularios_ModuloID 
ON dbo.CatalogoFormularios(ModuloID);
GO

CREATE NONCLUSTERED INDEX IX_CatalogoFormularios_Activo 
ON dbo.CatalogoFormularios(Activo);
GO

CREATE NONCLUSTERED INDEX IX_CatalogoFormularios_Codigo 
ON dbo.CatalogoFormularios(CodigoFormulario);
GO

PRINT '✓ Tabla CatalogoFormularios creada exitosamente';
PRINT '✓ Relación con CatalogoModulos establecida';
PRINT '✓ Índices creados';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: CatalogoFormularios';
PRINT 'Columnas: FormularioID, ModuloID, CodigoFormulario, NombreClase';
PRINT 'Estado: CREADA Y LISTA';
PRINT '=======================================================';
GO

-- ============================================================================
-- SCRIPT 9: CREAR TABLA CATALOGO ACCIONES
-- ============================================================================
-- Descripción: Crea la tabla maestra de acciones del sistema
-- Define todas las acciones posibles sobre los formularios
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.CatalogoAcciones', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla CatalogoAcciones ya existe. Eliminándola...';
    DROP TABLE dbo.CatalogoAcciones;
    PRINT '✓ Tabla CatalogoAcciones eliminada';
END
GO

-- Crear tabla CatalogoAcciones
CREATE TABLE dbo.CatalogoAcciones
(
    AccionID INT IDENTITY(1,1) NOT NULL,
    CodigoAccion NVARCHAR(20) NOT NULL,        -- VIEW, CREATE, EDIT, DELETE, etc.
    NombreAccion NVARCHAR(50) NOT NULL,        -- Ver, Crear, Editar, Eliminar
    Descripcion NVARCHAR(200) NULL,
    GrupoAccion NVARCHAR(50) NULL,             -- CRUD, IMPRESION, APROBACION, ADMINISTRACION
    OrdenVisualizacion INT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    
    -- Constraints
    CONSTRAINT PK_CatalogoAcciones PRIMARY KEY CLUSTERED (AccionID),
    CONSTRAINT UQ_CatalogoAcciones_Codigo UNIQUE (CodigoAccion)
);
GO

-- Crear índices
CREATE NONCLUSTERED INDEX IX_CatalogoAcciones_GrupoAccion 
ON dbo.CatalogoAcciones(GrupoAccion);
GO

CREATE NONCLUSTERED INDEX IX_CatalogoAcciones_Activo 
ON dbo.CatalogoAcciones(Activo);
GO

PRINT '✓ Tabla CatalogoAcciones creada exitosamente';
PRINT '✓ Índices creados';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: CatalogoAcciones';
PRINT 'Columnas: AccionID, CodigoAccion, NombreAccion, GrupoAccion';
PRINT 'Estado: CREADA Y LISTA';
PRINT '=======================================================';
GO

-- ============================================================================
-- SCRIPT 10: CREAR TABLA PERMISOS ROL
-- ============================================================================
-- Descripción: Nueva tabla de permisos basada en IDs (Foreign Keys)
-- Reemplaza la tabla Permisos antigua que usaba texto
-- Define qué acciones puede realizar cada ROL en cada FORMULARIO
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.PermisosRol', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla PermisosRol ya existe. Eliminándola...';
    DROP TABLE dbo.PermisosRol;
    PRINT '✓ Tabla PermisosRol eliminada';
END
GO

-- Crear tabla PermisosRol
CREATE TABLE dbo.PermisosRol
(
    PermisoRolID INT IDENTITY(1,1) NOT NULL,
    RolID INT NOT NULL,
    FormularioID INT NOT NULL,
    AccionID INT NOT NULL,
    Permitido BIT NOT NULL DEFAULT 0,          -- 1 = Permitido, 0 = Denegado
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    CreadoPorUsuarioID INT NULL,
    FechaModificacion DATETIME NULL,
    ModificadoPorUsuarioID INT NULL,
    
    -- Constraints
    CONSTRAINT PK_PermisosRol PRIMARY KEY CLUSTERED (PermisoRolID),
    CONSTRAINT FK_PermisosRol_Rol FOREIGN KEY (RolID) 
        REFERENCES dbo.Roles(RolID),
    CONSTRAINT FK_PermisosRol_Formulario FOREIGN KEY (FormularioID) 
        REFERENCES dbo.CatalogoFormularios(FormularioID),
    CONSTRAINT FK_PermisosRol_Accion FOREIGN KEY (AccionID) 
        REFERENCES dbo.CatalogoAcciones(AccionID),
    CONSTRAINT FK_PermisosRol_CreadoPor FOREIGN KEY (CreadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT FK_PermisosRol_ModificadoPor FOREIGN KEY (ModificadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    
    -- Constraint para evitar duplicados: un rol no puede tener dos permisos iguales
    CONSTRAINT UQ_PermisosRol_Unico UNIQUE (RolID, FormularioID, AccionID)
);
GO

-- Crear índices para consultas rápidas
CREATE NONCLUSTERED INDEX IX_PermisosRol_RolID 
ON dbo.PermisosRol(RolID);
GO

CREATE NONCLUSTERED INDEX IX_PermisosRol_FormularioID 
ON dbo.PermisosRol(FormularioID);
GO

CREATE NONCLUSTERED INDEX IX_PermisosRol_AccionID 
ON dbo.PermisosRol(AccionID);
GO

-- Índice compuesto para búsquedas de permisos (el más usado)
CREATE NONCLUSTERED INDEX IX_PermisosRol_Busqueda 
ON dbo.PermisosRol(RolID, FormularioID, AccionID) 
INCLUDE (Permitido);
GO

PRINT '✓ Tabla PermisosRol creada exitosamente';
PRINT '✓ Relaciones (Foreign Keys) establecidas';
PRINT '✓ Índices creados para optimizar consultas';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: PermisosRol';
PRINT 'Estructura: RolID + FormularioID + AccionID = Permiso';
PRINT 'Ventaja: Usa IDs en lugar de texto (sin errores de typo)';
PRINT 'Estado: CREADA Y LISTA';
PRINT '=======================================================';
GO

-- ============================================================================
-- SCRIPT 11: CREAR TABLA PERMISOS USUARIO
-- ============================================================================
-- Descripción: Tabla de excepciones de permisos por USUARIO
-- Permite sobrescribir permisos del ROL para usuarios específicos
-- Ejemplo: Un Contador puede tener permiso especial para aprobar pagos
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si la tabla ya existe y eliminarla
IF OBJECT_ID('dbo.PermisosUsuario', 'U') IS NOT NULL
BEGIN
    PRINT 'La tabla PermisosUsuario ya existe. Eliminándola...';
    DROP TABLE dbo.PermisosUsuario;
    PRINT '✓ Tabla PermisosUsuario eliminada';
END
GO

-- Crear tabla PermisosUsuario
CREATE TABLE dbo.PermisosUsuario
(
    PermisoUsuarioID INT IDENTITY(1,1) NOT NULL,
    UsuarioID INT NOT NULL,
    FormularioID INT NOT NULL,
    AccionID INT NOT NULL,
    Permitido BIT NOT NULL DEFAULT 0,          -- 1 = Permitido, 0 = Denegado
    Motivo NVARCHAR(500) NULL,                 -- Justificación del permiso especial
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    CreadoPorUsuarioID INT NULL,
    FechaModificacion DATETIME NULL,
    ModificadoPorUsuarioID INT NULL,
    FechaExpiracion DATETIME NULL,             -- Opcional: permisos temporales
    
    -- Constraints
    CONSTRAINT PK_PermisosUsuario PRIMARY KEY CLUSTERED (PermisoUsuarioID),
    CONSTRAINT FK_PermisosUsuario_Usuario FOREIGN KEY (UsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT FK_PermisosUsuario_Formulario FOREIGN KEY (FormularioID) 
        REFERENCES dbo.CatalogoFormularios(FormularioID),
    CONSTRAINT FK_PermisosUsuario_Accion FOREIGN KEY (AccionID) 
        REFERENCES dbo.CatalogoAcciones(AccionID),
    CONSTRAINT FK_PermisosUsuario_CreadoPor FOREIGN KEY (CreadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    CONSTRAINT FK_PermisosUsuario_ModificadoPor FOREIGN KEY (ModificadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID),
    
    -- Constraint para evitar duplicados
    CONSTRAINT UQ_PermisosUsuario_Unico UNIQUE (UsuarioID, FormularioID, AccionID)
);
GO

-- Crear índices para consultas rápidas
CREATE NONCLUSTERED INDEX IX_PermisosUsuario_UsuarioID 
ON dbo.PermisosUsuario(UsuarioID);
GO

CREATE NONCLUSTERED INDEX IX_PermisosUsuario_FormularioID 
ON dbo.PermisosUsuario(FormularioID);
GO

CREATE NONCLUSTERED INDEX IX_PermisosUsuario_AccionID 
ON dbo.PermisosUsuario(AccionID);
GO

-- Índice compuesto para búsquedas de permisos
CREATE NONCLUSTERED INDEX IX_PermisosUsuario_Busqueda 
ON dbo.PermisosUsuario(UsuarioID, FormularioID, AccionID) 
INCLUDE (Permitido);
GO

-- Índice para permisos expirados
CREATE NONCLUSTERED INDEX IX_PermisosUsuario_Expiracion 
ON dbo.PermisosUsuario(FechaExpiracion) 
WHERE FechaExpiracion IS NOT NULL;
GO

PRINT '✓ Tabla PermisosUsuario creada exitosamente';
PRINT '✓ Relaciones (Foreign Keys) establecidas';
PRINT '✓ Índices creados para optimizar consultas';
PRINT '';
PRINT '=======================================================';
PRINT 'Tabla: PermisosUsuario';
PRINT 'Propósito: Excepciones de permisos para usuarios específicos';
PRINT 'Prioridad: Los permisos de usuario SOBRESCRIBEN los del rol';
PRINT 'Estado: CREADA Y LISTA';
PRINT '=======================================================';
GO

CARPETA: 02_Data

-- ============================================================================
-- SCRIPT 1: INSERTAR ROLES INICIALES
-- ============================================================================
-- Descripción: Inserta los 3 roles fundamentales del sistema
-- ROOT, ADMIN, CONTADOR
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si ya existen roles
IF EXISTS (SELECT 1 FROM dbo.Roles)
BEGIN
    PRINT 'La tabla Roles ya contiene datos. Eliminando roles existentes...';
    DELETE FROM dbo.Roles;
    DBCC CHECKIDENT ('dbo.Roles', RESEED, 0);
    PRINT '✓ Roles anteriores eliminados';
END
GO

-- Insertar los 3 roles del sistema
INSERT INTO dbo.Roles (NombreRol, Descripcion, Categoria, Activo)
VALUES 
    ('ROOT', 
     'Administrador técnico del sistema. Acceso total y gestión completa de usuarios, roles, permisos y configuración.', 
     'SISTEMA', 
     1),
    
    ('ADMIN', 
     'Administrador funcional. Acceso completo a todas las categorías operativas (Contabilidad, Gerencia Financiera, Gerencia Legal). Gestión limitada de usuarios.', 
     'SISTEMA,CONTABILIDAD,GERENCIA FINANCIERA,GERENCIA LEGAL', 
     1),
    
    ('CONTADOR', 
     'Usuario operativo del área contable. Acceso exclusivo a módulos de Contabilidad. CRUD completo solo sobre sus propios registros.', 
     'CONTABILIDAD', 
     1);
GO

-- Verificar inserción
SELECT 
    RolID,
    NombreRol,
    Descripcion,
    Categoria,
    Activo,
    FechaCreacion
FROM dbo.Roles
ORDER BY RolID;
GO

PRINT '';
PRINT '=======================================================';
PRINT '✓ Roles insertados exitosamente';
PRINT '=======================================================';
PRINT 'Roles creados:';
PRINT '  1. ROOT     - Acceso total al sistema';
PRINT '  2. ADMIN    - Acceso funcional completo';
PRINT '  3. CONTADOR - Acceso solo a Contabilidad';
PRINT '';
PRINT 'Estado: COMPLETADO';
PRINT 'Siguiente paso: Script 7 - Crear usuario ROOT inicial';
PRINT '=======================================================';
GO

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

-- ============================================================================
-- SCRIPT 3: INSERTAR CATEGORÍAS INICIALES
-- ============================================================================
-- Descripción: Inserta las 5 categorías principales del sistema
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si ya existen categorías
IF EXISTS (SELECT 1 FROM dbo.CatalogoCategorias)
BEGIN
    PRINT 'La tabla CatalogoCategorias ya contiene datos.';
    PRINT 'Eliminando categorías existentes...';
    DELETE FROM dbo.CatalogoCategorias;
    DBCC CHECKIDENT ('dbo.CatalogoCategorias', RESEED, 0);
    PRINT '✓ Categorías anteriores eliminadas';
END
GO

-- Insertar categorías del sistema
INSERT INTO dbo.CatalogoCategorias 
    (CodigoCategoria, NombreCategoria, Descripcion, Icono, OrdenVisualizacion, Activo)
VALUES 
    ('SYS', 'SISTEMA', 
     'Configuración y administración del sistema', 
     '⚙️', 1, 1),
    
    ('CONT', 'CONTABILIDAD', 
     'Gestión contable y financiera', 
     '📊', 2, 1),
    
    ('GERFIN', 'GERENCIA FINANCIERA', 
     'Análisis y reportes financieros', 
     '💼', 3, 1),
    
    ('GERLEG', 'GERENCIA LEGAL', 
     'Gestión legal y contratos', 
     '⚖️', 4, 1),
    
    ('DEV', 'DESARROLLO', 
     'Herramientas de desarrollo y módulos futuros', 
     '🚀', 5, 1);
GO

-- Verificar inserción
SELECT 
    CategoriaID,
    CodigoCategoria,
    NombreCategoria,
    Icono,
    OrdenVisualizacion,
    Activo
FROM dbo.CatalogoCategorias
ORDER BY OrdenVisualizacion;
GO

PRINT '';
PRINT '=======================================================';
PRINT '✓ Categorías insertadas exitosamente';
PRINT '=======================================================';
PRINT 'Categorías creadas:';
PRINT '  1. SISTEMA';
PRINT '  2. CONTABILIDAD';
PRINT '  3. GERENCIA FINANCIERA';
PRINT '  4. GERENCIA LEGAL';
PRINT '  5. DESARROLLO';
PRINT '';
PRINT 'Estado: COMPLETADO';
PRINT '=======================================================';
GO

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

-- ============================================================================
-- SCRIPT 7: INSERTAR PERMISOS INICIALES
-- ============================================================================
-- Descripción: Inserta los permisos iniciales para los 3 roles del sistema
-- ROOT: Acceso total
-- ADMIN: Acceso limitado
-- CONTADOR: Solo consulta en Sistema, CRUD en Contabilidad (sus registros)
-- ============================================================================

USE FiducorpERP;
GO

-- Verificar si ya existen permisos
IF EXISTS (SELECT 1 FROM dbo.PermisosRol)
BEGIN
    PRINT 'La tabla PermisosRol ya contiene datos.';
    PRINT 'Eliminando permisos existentes...';
    DELETE FROM dbo.PermisosRol;
    DBCC CHECKIDENT ('dbo.PermisosRol', RESEED, 0);
    PRINT '✓ Permisos anteriores eliminados';
END
GO

-- ============================================================================
-- PERMISOS PARA ROOT (RolID = 1)
-- ROOT tiene acceso TOTAL a TODO
-- ============================================================================
PRINT 'Insertando permisos para ROOT...';

INSERT INTO dbo.PermisosRol (RolID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID)
SELECT 
    1 AS RolID,                    -- ROOT
    F.FormularioID,
    A.AccionID,
    1 AS Permitido,                -- Todo permitido
    1 AS CreadoPorUsuarioID        -- Creado por usuario ROOT
FROM dbo.CatalogoFormularios F
CROSS JOIN dbo.CatalogoAcciones A
WHERE F.Activo = 1 AND A.Activo = 1;

PRINT '✓ Permisos de ROOT insertados (acceso total)';
GO

-- ============================================================================
-- PERMISOS PARA ADMIN (RolID = 2)
-- ADMIN: Todo excepto crear/eliminar usuarios
-- ============================================================================
PRINT 'Insertando permisos para ADMIN...';

-- ADMIN: Acceso completo a todos los formularios
INSERT INTO dbo.PermisosRol (RolID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID)
SELECT 
    2 AS RolID,                    -- ADMIN
    F.FormularioID,
    A.AccionID,
    CASE 
        -- Denegar CREATE y DELETE en FormGestionUsuarios
        WHEN F.CodigoFormulario = 'FGEUSR' AND A.CodigoAccion IN ('CREATE', 'DELETE') THEN 0
        -- Denegar acceso a FormUsuario (crear/editar usuario)
        WHEN F.CodigoFormulario = 'FUSR' THEN 0
        -- Denegar acceso a FormConfirmarEliminacion
        WHEN F.CodigoFormulario = 'FCONFIRMDEL' THEN 0
        -- Todo lo demás: permitido
        ELSE 1
    END AS Permitido,
    1 AS CreadoPorUsuarioID
FROM dbo.CatalogoFormularios F
CROSS JOIN dbo.CatalogoAcciones A
WHERE F.Activo = 1 AND A.Activo = 1;

PRINT '✓ Permisos de ADMIN insertados';
GO

-- ============================================================================
-- PERMISOS PARA CONTADOR (RolID = 3)
-- CONTADOR: Solo consulta en Sistema, CRUD en Contabilidad
-- ============================================================================
PRINT 'Insertando permisos para CONTADOR...';

INSERT INTO dbo.PermisosRol (RolID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID)
SELECT 
    3 AS RolID,                    -- CONTADOR
    F.FormularioID,
    A.AccionID,
    CASE 
        -- ========================================
        -- SISTEMA: Solo consulta (VIEW)
        -- ========================================
        WHEN M.CodigoModulo IN ('GEUSR', 'GEROL', 'AUD') THEN
            CASE 
                WHEN A.CodigoAccion = 'VIEW' THEN 1
                ELSE 0
            END
        
        -- ========================================
        -- CONTABILIDAD: CRUD completo
        -- ========================================
        WHEN M.CodigoModulo IN ('CXP', 'IMP', 'REC', 'FUT') THEN
            CASE 
                -- Permitir operaciones CRUD básicas
                WHEN A.CodigoAccion IN ('VIEW', 'CREATE', 'EDIT', 'DELETE') THEN 1
                -- Permitir impresión y exportación
                WHEN A.CodigoAccion IN ('PRINT', 'EXPORT') THEN 1
                -- Denegar todo lo demás (aprobaciones, etc.)
                ELSE 0
            END
        
        -- Todo lo demás: denegado
        ELSE 0
    END AS Permitido,
    1 AS CreadoPorUsuarioID
FROM dbo.CatalogoFormularios F
INNER JOIN dbo.CatalogoModulos M ON F.ModuloID = M.ModuloID
CROSS JOIN dbo.CatalogoAcciones A
WHERE F.Activo = 1 AND A.Activo = 1;

PRINT '✓ Permisos de CONTADOR insertados';
GO

-- ============================================================================
-- VERIFICACIÓN DE PERMISOS INSERTADOS
-- ============================================================================
PRINT '';
PRINT '=======================================================';
PRINT 'VERIFICACIÓN DE PERMISOS';
PRINT '=======================================================';

-- Contar permisos por rol
SELECT 
    R.RolID,
    R.NombreRol,
    COUNT(*) AS TotalPermisos,
    SUM(CASE WHEN P.Permitido = 1 THEN 1 ELSE 0 END) AS Permitidos,
    SUM(CASE WHEN P.Permitido = 0 THEN 1 ELSE 0 END) AS Denegados
FROM dbo.PermisosRol P
INNER JOIN dbo.Roles R ON P.RolID = R.RolID
GROUP BY R.RolID, R.NombreRol
ORDER BY R.RolID;

PRINT '';
PRINT '=======================================================';
PRINT '✓ Permisos iniciales insertados exitosamente';
PRINT '=======================================================';
PRINT 'Resumen:';
PRINT '  ROOT: Acceso total a todo';
PRINT '  ADMIN: Todo excepto crear/eliminar usuarios';
PRINT '  CONTADOR: Solo consulta en Sistema, CRUD en Contabilidad';
PRINT '';
PRINT 'Estado: COMPLETADO';
PRINT 'Siguiente paso: Crear clase PermisosHelper.cs en C#';
PRINT '=======================================================';
GO

CARPETA: 03_Updates:

-- ============================================================================
-- UPDATE 001: AGREGAR COLUMNAS PARA ELIMINACIÓN LÓGICA DE USUARIOS
-- ============================================================================
-- Descripción: Agrega columnas necesarias para implementar eliminación lógica
--              en lugar de eliminación física de usuarios
-- Fecha: 2024-12-25
-- Autor: [Tu nombre]
-- ============================================================================

USE FiducorpERP;
GO

PRINT '=======================================================';
PRINT 'UPDATE 001: Agregar columnas para eliminación lógica';
PRINT '=======================================================';
PRINT '';

-- Verificar si las columnas ya existen antes de agregarlas
IF NOT EXISTS (SELECT 1 FROM sys.columns 
               WHERE object_id = OBJECT_ID('dbo.Usuarios') 
               AND name = 'EsEliminado')
BEGIN
    PRINT 'Agregando columna EsEliminado...';
    ALTER TABLE dbo.Usuarios
    ADD EsEliminado BIT NOT NULL DEFAULT 0;
    PRINT '✓ Columna EsEliminado agregada';
END
ELSE
BEGIN
    PRINT '⚠ Columna EsEliminado ya existe, saltando...';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns 
               WHERE object_id = OBJECT_ID('dbo.Usuarios') 
               AND name = 'FechaEliminacion')
BEGIN
    PRINT 'Agregando columna FechaEliminacion...';
    ALTER TABLE dbo.Usuarios
    ADD FechaEliminacion DATETIME NULL;
    PRINT '✓ Columna FechaEliminacion agregada';
END
ELSE
BEGIN
    PRINT '⚠ Columna FechaEliminacion ya existe, saltando...';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns 
               WHERE object_id = OBJECT_ID('dbo.Usuarios') 
               AND name = 'EliminadoPorUsuarioID')
BEGIN
    PRINT 'Agregando columna EliminadoPorUsuarioID...';
    ALTER TABLE dbo.Usuarios
    ADD EliminadoPorUsuarioID INT NULL;
    PRINT '✓ Columna EliminadoPorUsuarioID agregada';
END
ELSE
BEGIN
    PRINT '⚠ Columna EliminadoPorUsuarioID ya existe, saltando...';
END
GO

-- Crear Foreign Key para EliminadoPorUsuarioID
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys 
               WHERE name = 'FK_Usuarios_EliminadoPor')
BEGIN
    PRINT 'Creando Foreign Key FK_Usuarios_EliminadoPor...';
    ALTER TABLE dbo.Usuarios
    ADD CONSTRAINT FK_Usuarios_EliminadoPor 
        FOREIGN KEY (EliminadoPorUsuarioID) 
        REFERENCES dbo.Usuarios(UsuarioID);
    PRINT '✓ Foreign Key FK_Usuarios_EliminadoPor creada';
END
ELSE
BEGIN
    PRINT '⚠ Foreign Key FK_Usuarios_EliminadoPor ya existe, saltando...';
END
GO

-- Crear índice para mejorar consultas de usuarios no eliminados
IF NOT EXISTS (SELECT 1 FROM sys.indexes 
               WHERE name = 'IX_Usuarios_EsEliminado' 
               AND object_id = OBJECT_ID('dbo.Usuarios'))
BEGIN
    PRINT 'Creando índice IX_Usuarios_EsEliminado...';
    CREATE NONCLUSTERED INDEX IX_Usuarios_EsEliminado 
    ON dbo.Usuarios(EsEliminado);
    PRINT '✓ Índice IX_Usuarios_EsEliminado creado';
END
ELSE
BEGIN
    PRINT '⚠ Índice IX_Usuarios_EsEliminado ya existe, saltando...';
END
GO

PRINT '';
PRINT '=======================================================';
PRINT 'UPDATE 001 - COMPLETADO EXITOSAMENTE';
PRINT '=======================================================';
PRINT 'Columnas agregadas:';
PRINT '  ✓ EsEliminado (BIT, DEFAULT 0)';
PRINT '  ✓ FechaEliminacion (DATETIME NULL)';
PRINT '  ✓ EliminadoPorUsuarioID (INT NULL)';
PRINT '';
PRINT 'Relaciones:';
PRINT '  ✓ FK_Usuarios_EliminadoPor → Usuarios(UsuarioID)';
PRINT '';
PRINT 'Índices:';
PRINT '  ✓ IX_Usuarios_EsEliminado';
PRINT '';
PRINT 'Estado: LISTO PARA USAR';
PRINT 'Funcionalidad habilitada: Eliminación lógica de usuarios';
PRINT '=======================================================';
GO

USE FiducorpERP;
GO

-- Verificar e insertar roles faltantes
IF NOT EXISTS (SELECT 1 FROM Roles WHERE NombreRol = 'GERENTE')
BEGIN
    INSERT INTO Roles (NombreRol, Descripcion, Categoria, Activo)
    VALUES ('GERENTE', 'Gerente del área. Permisos de supervisión y aprobación.', 'GERENCIA FINANCIERA,GERENCIA LEGAL', 1);
    PRINT '✓ Rol GERENTE insertado';
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE NombreRol = 'ANALISTA')
BEGIN
    INSERT INTO Roles (NombreRol, Descripcion, Categoria, Activo)
    VALUES ('ANALISTA', 'Analista operativo. Permisos de consulta y análisis.', 'CONTABILIDAD,GERENCIA FINANCIERA', 1);
    PRINT '✓ Rol ANALISTA insertado';
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE NombreRol = 'PROBADOR')
BEGIN
    INSERT INTO Roles (NombreRol, Descripcion, Categoria, Activo)
    VALUES ('PROBADOR', 'Usuario de pruebas. Acceso limitado para testing.', 'DESARROLLO', 1);
    PRINT '✓ Rol PROBADOR insertado';
END

-- Verificar
SELECT RolID, NombreRol, Descripcion, Categoria FROM Roles ORDER BY RolID;
GO

USE FiducorpERP;
GO

-- Insertar permisos básicos para GERENTE (similar a ADMIN pero más limitado)
INSERT INTO PermisosRol (RolID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID)
SELECT 
    (SELECT RolID FROM Roles WHERE NombreRol = 'GERENTE'),
    F.FormularioID,
    A.AccionID,
    CASE 
        WHEN A.CodigoAccion IN ('VIEW', 'PRINT', 'EXPORT') THEN 1
        ELSE 0
    END AS Permitido,
    1
FROM CatalogoFormularios F
CROSS JOIN CatalogoAcciones A
WHERE F.Activo = 1 AND A.Activo = 1
    AND A.CodigoAccion IN ('VIEW', 'CREATE', 'EDIT', 'DELETE', 'PRINT', 'EXPORT')
    AND NOT EXISTS (
        SELECT 1 FROM PermisosRol PR 
        WHERE PR.RolID = (SELECT RolID FROM Roles WHERE NombreRol = 'GERENTE')
        AND PR.FormularioID = F.FormularioID
        AND PR.AccionID = A.AccionID
    );

-- Insertar permisos para ANALISTA (solo consulta)
INSERT INTO PermisosRol (RolID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID)
SELECT 
    (SELECT RolID FROM Roles WHERE NombreRol = 'ANALISTA'),
    F.FormularioID,
    A.AccionID,
    CASE 
        WHEN A.CodigoAccion = 'VIEW' THEN 1
        ELSE 0
    END AS Permitido,
    1
FROM CatalogoFormularios F
CROSS JOIN CatalogoAcciones A
WHERE F.Activo = 1 AND A.Activo = 1
    AND A.CodigoAccion IN ('VIEW', 'CREATE', 'EDIT', 'DELETE', 'PRINT', 'EXPORT')
    AND NOT EXISTS (
        SELECT 1 FROM PermisosRol PR 
        WHERE PR.RolID = (SELECT RolID FROM Roles WHERE NombreRol = 'ANALISTA')
        AND PR.FormularioID = F.FormularioID
        AND PR.AccionID = A.AccionID
    );

-- Insertar permisos para PROBADOR (acceso de testing)
INSERT INTO PermisosRol (RolID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID)
SELECT 
    (SELECT RolID FROM Roles WHERE NombreRol = 'PROBADOR'),
    F.FormularioID,
    A.AccionID,
    1 AS Permitido,
    1
FROM CatalogoFormularios F
CROSS JOIN CatalogoAcciones A
WHERE F.Activo = 1 AND A.Activo = 1
    AND A.CodigoAccion IN ('VIEW', 'CREATE', 'EDIT', 'DELETE', 'PRINT', 'EXPORT')
    AND NOT EXISTS (
        SELECT 1 FROM PermisosRol PR 
        WHERE PR.RolID = (SELECT RolID FROM Roles WHERE NombreRol = 'PROBADOR')
        AND PR.FormularioID = F.FormularioID
        AND PR.AccionID = A.AccionID
    );

PRINT '✓ Permisos insertados para todos los roles';
GO

-- ============================================================================
-- UPDATE 004: AGREGAR COLUMNA EsSistema A TABLA ROLES
-- ============================================================================
-- Descripción: Permite diferenciar roles del sistema (fijos) de roles personalizados
-- Fecha: 2025-12-27
-- Autor: Sistema MOFIS-ERP
-- ============================================================================

USE FiducorpERP;
GO

PRINT '=======================================================';
PRINT 'INICIO: UPDATE_002 - Agregar columna EsSistema';
PRINT '=======================================================';
PRINT '';

-- Verificar si la columna ya existe
IF NOT EXISTS (SELECT 1 FROM sys.columns 
               WHERE object_id = OBJECT_ID('dbo.Roles') 
               AND name = 'EsSistema')
BEGIN
    PRINT 'Agregando columna EsSistema a tabla Roles...';
    
    -- Agregar columna EsSistema
    ALTER TABLE dbo.Roles
    ADD EsSistema BIT NOT NULL DEFAULT 0;
    
    PRINT '✓ Columna EsSistema agregada exitosamente';
    PRINT '';
END
ELSE
BEGIN
    PRINT '⚠ La columna EsSistema ya existe. Omitiendo creación.';
    PRINT '';
END
GO

-- Marcar los roles actuales como "roles de sistema"
PRINT 'Marcando roles del sistema...';

UPDATE dbo.Roles
SET EsSistema = 1
WHERE NombreRol IN ('ROOT', 'ADMIN', 'GERENTE', 'CONTADOR', 'ANALISTA', 'PROBADOR');

DECLARE @RolesSistema INT = @@ROWCOUNT;
PRINT CONCAT('✓ ', @RolesSistema, ' roles marcados como roles del sistema');
PRINT '';
GO

-- Verificar resultado
PRINT 'Verificando resultado:';
PRINT '';

SELECT 
    RolID,
    NombreRol,
    CASE WHEN EsSistema = 1 THEN 'Sistema' ELSE 'Personalizado' END AS TipoRol,
    Activo,
    FechaCreacion
FROM dbo.Roles
ORDER BY EsSistema DESC, RolID;
GO

PRINT '';
PRINT '=======================================================';
PRINT '✓ UPDATE_002 completado exitosamente';
PRINT '=======================================================';
PRINT 'Resumen:';
PRINT '  - Columna EsSistema agregada';
PRINT '  - Roles del sistema marcados (ROOT, ADMIN, etc.)';
PRINT '  - Roles personalizados tendrán EsSistema = 0';
PRINT '';
PRINT 'Siguiente paso: Implementar gestión de roles dinámicos en UI';
PRINT '=======================================================';
GO


Actualmente estamos trabajando en el Modulo de Auditoria, dentro de la Categoria SISTEMA. Esto es lo que tenemos hasta ahora:

Carpetas: Forms - Sistema - Auditoria:

Clase: AuditoriaAcciones.cs

using System;

namespace MOFIS_ERP.Classes
{
    /// <summary>
    /// Constantes para todas las acciones de auditoría del sistema MOFIS-ERP
    /// Organizado por categoría y módulo para mantener consistencia
    /// </summary>
    public static class AuditoriaAcciones
    {
        // ═══════════════════════════════════════════════════════════════
        // CATEGORÍA: SISTEMA
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Acciones del módulo de Autenticación
        /// </summary>
        public static class Autenticacion
        {
            public const string LOGIN = "LOGIN";
            public const string LOGOUT = "LOGOUT";
            public const string LOGIN_FALLIDO = "LOGIN_FALLIDO";
            public const string CAMBIAR_PASSWORD = "CAMBIAR_PASSWORD";
        }

        /// <summary>
        /// Acciones del módulo de Gestión de Usuarios
        /// </summary>
        public static class GestionUsuarios
        {
            public const string CREAR_USUARIO = "CREAR_USUARIO";
            public const string MODIFICAR_USUARIO = "MODIFICAR_USUARIO";
            public const string ELIMINAR_USUARIO = "ELIMINAR_USUARIO";
            public const string ACTIVAR_USUARIO = "ACTIVAR_USUARIO";
            public const string DESACTIVAR_USUARIO = "DESACTIVAR_USUARIO";
            public const string RESET_PASSWORD_USUARIO = "RESET_PASSWORD_USUARIO";
        }

        /// <summary>
        /// Acciones del módulo de Gestión de Roles
        /// </summary>
        public static class GestionRoles
        {
            public const string CREAR_ROL = "CREAR_ROL";
            public const string ELIMINAR_ROL = "ELIMINAR_ROL";
            public const string MODIFICAR_PERMISOS_ROL = "MODIFICAR_PERMISOS_ROL";
            public const string MODIFICAR_PERMISOS_USUARIO = "MODIFICAR_PERMISOS_USUARIO";
            public const string COPIAR_PERMISOS = "COPIAR_PERMISOS";
            public const string CONSULTAR_PERMISOS = "CONSULTAR_PERMISOS";
            public const string CONFIRMAR_PASSWORD_CAMBIO_CRITICO = "CONFIRMAR_PASSWORD_CAMBIO_CRITICO";
            public const string GENERAR_REPORTE_PERMISOS = "GENERAR_REPORTE_PERMISOS";
            public const string GENERAR_REPORTE_MATRIZ = "GENERAR_REPORTE_MATRIZ";
            public const string GENERAR_REPORTE_EXCEPCIONES = "GENERAR_REPORTE_EXCEPCIONES";
            public const string GENERAR_REPORTE_ROL = "GENERAR_REPORTE_ROL";
            public const string EXPORTAR_PERMISOS_EXCEL = "EXPORTAR_PERMISOS_EXCEL";
            public const string EXPORTAR_PERMISOS_PDF = "EXPORTAR_PERMISOS_PDF";
        }

        /// <summary>
        /// Acciones del módulo de Auditoría General
        /// </summary>
        public static class AuditoriaGeneral
        {
            public const string CONSULTAR_AUDITORIA = "CONSULTAR_AUDITORIA";
            public const string VER_DETALLE_AUDITORIA = "VER_DETALLE_AUDITORIA";
            public const string EXPORTAR_AUDITORIA_EXCEL = "EXPORTAR_AUDITORIA_EXCEL";
            public const string EXPORTAR_AUDITORIA_PDF = "EXPORTAR_AUDITORIA_PDF";
            public const string FILTRAR_AUDITORIA = "FILTRAR_AUDITORIA";
        }

        // ═══════════════════════════════════════════════════════════════
        // HELPER: CATEGORÍAS Y MÓDULOS DEL SISTEMA
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Categorías principales del sistema (solo las que existen actualmente)
        /// </summary>
        public static class Categorias
        {
            public const string SISTEMA = "SISTEMA";
        }

        /// <summary>
        /// Módulos del sistema (solo los que existen actualmente)
        /// </summary>
        public static class Modulos
        {
            public const string AUTENTICACION = "Autenticación";
            public const string GESTION_USUARIOS = "Gestión de Usuarios";
            public const string GESTION_ROLES = "Gestión de Roles";
            public const string AUDITORIA_GENERAL = "Auditoría General";
        }
    }
}

CODIGO COMPLETO DEL FORMULARIO PRINCIPAL DEL MODULO: FormAuditoria

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;
using MOFIS_ERP.Classes;
using ClosedXML.Excel;
using System.Threading.Tasks;
using System.Text;
using System.Diagnostics;

namespace MOFIS_ERP.Forms.Sistema.Auditoria
{
    public partial class FormAuditoria : Form
    {
        private FormMain formPrincipal;
        private BindingSource bindingSource; // Para filtrado rápido
        private System.Windows.Forms.Timer busquedaTimer; // Debounce timer

        // ============================================================
        // CACHE PERSISTENTE (sobrevive entre aperturas del formulario)
        // ============================================================
        private static DataTable dtAuditoriaCompleta = null; // AHORA ES STATIC
        private static List<string> cacheModulos = null;
        private static List<string> cacheAcciones = null;
        private static DateTime? ultimaActualizacionCache = null;
        private static DateTime? ultimaActualizacionDatos = null; // NUEVO

        private const int CACHE_DURACION_MINUTOS = 10; // 10 minutos para filtros
        private const int DATOS_DURACION_MINUTOS = 15; // 15 minutos para datos completos

        // Colores corporativos
        private readonly Color colorCorporativo = Color.FromArgb(0, 120, 212);
        private readonly Color colorVerde = Color.FromArgb(34, 139, 34);
        private readonly Color colorRojo = Color.FromArgb(220, 53, 69);
        private readonly Color colorNaranja = Color.FromArgb(255, 152, 0);
        private readonly Color colorMorado = Color.FromArgb(156, 39, 176);
        private readonly Color colorGris = Color.FromArgb(108, 117, 125);

        public FormAuditoria(FormMain formMain)
        {
            InitializeComponent();
            formPrincipal = formMain;

            // Inicializar BindingSource
            bindingSource = new BindingSource();

            ConfigurarFormulario();

            // Cargar datos de forma asíncrona (no bloquea la UI)
            this.Load += async (s, e) =>
            {
                await CargarFormularioAsync();
                dgvAuditoria.ClearSelection();
            };
        }

        /// <summary>
        /// Carga asíncrona del formulario completo (ultra optimizado con persistencia)
        /// </summary>
        private async Task CargarFormularioAsync()
        {
            try
            {
                this.SuspendLayout();

                // Verificar si hay datos en cache y aún son válidos
                bool datosEnCache = dtAuditoriaCompleta != null &&
                                   ultimaActualizacionDatos.HasValue &&
                                   (DateTime.Now - ultimaActualizacionDatos.Value).TotalMinutes <= DATOS_DURACION_MINUTOS;

                if (datosEnCache)
                {
                    // ⚡ CARGA INSTANTÁNEA desde cache
                    lblTotalRegistros.Text = "⚡ Cargando desde cache...";
                    lblFechaConsulta.Text = $"Cache: {ultimaActualizacionDatos.Value:dd/MM/yyyy HH:mm:ss}";

                    // Cargar solo filtros (los datos ya están en memoria)
                    await CargarFiltrosAsync();

                    // Vincular datos desde cache
                    bindingSource.DataSource = dtAuditoriaCompleta;
                    dgvAuditoria.DataSource = bindingSource;
                    AplicarFiltros();
                }
                else
                {
                    // 🔄 CARGA COMPLETA desde BD (primera vez o cache expirado)
                    lblTotalRegistros.Text = "⏳ Cargando datos desde BD...";
                    lblFechaConsulta.Text = "";

                    // Cargar filtros y datos en paralelo (máxima velocidad)
                    await Task.WhenAll(
                        CargarFiltrosAsync(),
                        CargarDatosInicialesAsync()
                    );
                }

                this.ResumeLayout();
            }
            catch (Exception ex)
            {
                this.ResumeLayout();
                MessageBox.Show($"Error al cargar formulario:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarFormulario()
        {
            // Configurar apariencia de botones
            ConfigurarBoton(btnVolver, colorCorporativo, Color.White, true);
            ConfigurarBoton(btnLimpiar, colorGris, Color.White, false);
            ConfigurarBoton(btnDashboard, colorMorado, Color.White, false);
            ConfigurarBoton(btnBuscarDetalles, colorNaranja, Color.White, false);
            ConfigurarBoton(btnReportePDF, colorRojo, Color.White, false);
            ConfigurarBoton(btnActualizar, colorCorporativo, Color.White, false);
            ConfigurarBoton(btnExportar, colorVerde, Color.White, false);

            // Configurar DateTimePickers con valores por defecto
            dtpDesde.Value = DateTime.Now.AddMonths(-1);
            dtpHasta.Value = DateTime.Now;
            dtpDesde.Checked = true;
            dtpHasta.Checked = true;

            // Configurar DataGridView
            ConfigurarDataGridView();

            // Asignar eventos
            btnVolver.Click += BtnVolver_Click;
            btnLimpiar.Click += BtnLimpiar_Click;
            btnDashboard.Click += BtnDashboard_Click;
            btnBuscarDetalles.Click += BtnBuscarDetalles_Click;
            btnReportePDF.Click += BtnReportePDF_Click;
            btnActualizar.Click += BtnActualizar_Click;
            btnExportar.Click += BtnExportar_Click;
            dgvAuditoria.DoubleClick += DgvAuditoria_DoubleClick;

            // Placeholder de txtBuscar
            ConfigurarPlaceholder();

            // Configurar timer para búsqueda con delay
            busquedaTimer = new System.Windows.Forms.Timer();
            busquedaTimer.Interval = 300; // 300ms es suficiente
            busquedaTimer.Tick += (s, e) =>
            {
                busquedaTimer.Stop();
                AplicarFiltros();
            };

            // Búsqueda con delay al escribir
            txtBuscar.TextChanged += (s, e) =>
            {
                busquedaTimer.Stop();
                busquedaTimer.Start();
            };

            // Aplicar filtros al cambiar ComboBox
            dtpDesde.ValueChanged += (s, e) => { if (dtpDesde.Checked) AplicarFiltros(); };
            dtpHasta.ValueChanged += (s, e) => { if (dtpHasta.Checked) AplicarFiltros(); };
            cmbModulo.SelectedIndexChanged += (s, e) => AplicarFiltros();
            cmbUsuario.SelectedIndexChanged += (s, e) => AplicarFiltros();
            cmbAccion.SelectedIndexChanged += (s, e) => AplicarFiltros();
        }

        private void ConfigurarBoton(Button btn, Color backColor, Color foreColor, bool conBorde)
        {
            btn.BackColor = backColor;
            btn.ForeColor = foreColor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Cursor = Cursors.Hand;

            if (conBorde)
            {
                btn.FlatAppearance.BorderColor = backColor;
                btn.FlatAppearance.BorderSize = 1;
            }
            else
            {
                btn.FlatAppearance.BorderSize = 0;
            }

            // Hover effect
            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = ControlPaint.Dark(backColor, 0.1f);
            };

            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = backColor;
            };
        }

        private void ConfigurarDataGridView()
        {
            dgvAuditoria.AutoGenerateColumns = false;
            dgvAuditoria.Columns.Clear();

            // Columnas
            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AuditoriaID",
                DataPropertyName = "AuditoriaID",
                HeaderText = "ID",
                Width = 80,
                Visible = false
            });

            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FechaHora",
                DataPropertyName = "FechaHora",
                HeaderText = "Fecha/Hora",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy HH:mm:ss"
                }
            });

            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Username",
                DataPropertyName = "Username",
                HeaderText = "Usuario",
                Width = 120
            });

            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Categoria",
                DataPropertyName = "Categoria",
                HeaderText = "Categoría",
                Width = 120
            });

            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Modulo",
                DataPropertyName = "Modulo",
                HeaderText = "Módulo",
                Width = 140
            });

            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Formulario",
                DataPropertyName = "Formulario",
                HeaderText = "Formulario",
                Width = 160
            });

            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Accion",
                DataPropertyName = "Accion",
                HeaderText = "Acción",
                Width = 200
            });

            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Detalle",
                DataPropertyName = "Detalle",
                HeaderText = "Detalle",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 200
            });

            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DireccionIP",
                DataPropertyName = "DireccionIP",
                HeaderText = "IP",
                Width = 100
            });

            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NombreMaquina",
                DataPropertyName = "NombreMaquina",
                HeaderText = "Máquina",
                Width = 110
            });

            // Estilo de encabezados
            dgvAuditoria.ColumnHeadersDefaultCellStyle.BackColor = colorCorporativo;
            dgvAuditoria.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAuditoria.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvAuditoria.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvAuditoria.EnableHeadersVisualStyles = false;

            // Estilo de filas
            dgvAuditoria.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvAuditoria.DefaultCellStyle.SelectionBackColor = colorCorporativo;
            dgvAuditoria.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvAuditoria.CurrentCell = null;
        }

        private void ConfigurarPlaceholder()
        {
            string placeholder = "Buscar en detalle...";
            Color placeholderColor = Color.Gray;
            Color normalColor = Color.Black;

            txtBuscar.ForeColor = placeholderColor;
            txtBuscar.Text = placeholder;

            txtBuscar.GotFocus += (s, e) =>
            {
                if (txtBuscar.Text == placeholder)
                {
                    txtBuscar.Text = "";
                    txtBuscar.ForeColor = normalColor;
                }
            };

            txtBuscar.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtBuscar.Text))
                {
                    txtBuscar.Text = placeholder;
                    txtBuscar.ForeColor = placeholderColor;
                }
            };
        }

        /// <summary>
        /// Carga filtros de forma asíncrona con cache inteligente (ULTRA OPTIMIZADO)
        /// - Una sola conexión a BD
        /// - Cache de 10 minutos para módulos y acciones
        /// - Queries en paralelo
        /// </summary>
        private async Task CargarFiltrosAsync(bool forzarActualizacion = false)
        {
            await Task.Run(() =>
            {
                try
                {
                    // Verificar si necesitamos actualizar el cache
                    bool necesitaActualizacion = forzarActualizacion ||
                        cacheModulos == null ||
                        cacheAcciones == null ||
                        !ultimaActualizacionCache.HasValue ||
                        (DateTime.Now - ultimaActualizacionCache.Value).TotalMinutes > CACHE_DURACION_MINUTOS;

                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();

                        // Cargar TODOS los filtros en UNA SOLA conexión (optimización crítica)
                        if (necesitaActualizacion)
                        {
                            // Query 1: Módulos (con cache)
                            cacheModulos = new List<string>();
                            string sqlModulos = @"
                                SELECT DISTINCT Modulo
                                FROM Auditoria
                                WHERE Modulo IS NOT NULL
                                ORDER BY Modulo";

                            using (var cmd = new SqlCommand(sqlModulos, conn))
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    cacheModulos.Add(reader.GetString(0));
                                }
                            }

                            // Query 2: Acciones (con cache)
                            cacheAcciones = new List<string>();
                            string sqlAcciones = @"
                                SELECT DISTINCT Accion
                                FROM Auditoria
                                WHERE Accion IS NOT NULL
                                ORDER BY Accion";

                            using (var cmd = new SqlCommand(sqlAcciones, conn))
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    cacheAcciones.Add(reader.GetString(0));
                                }
                            }

                            ultimaActualizacionCache = DateTime.Now;
                        }

                        // Query 3: Usuarios (siempre actualizado, no cacheable porque cambia frecuentemente)
                        var usuarios = new List<ComboBoxUsuario>();
                        string sqlUsuarios = @"
                            SELECT UsuarioID, Username, NombreCompleto
                            FROM Usuarios
                            WHERE Activo = 1 AND EsEliminado = 0
                            ORDER BY NombreCompleto";

                        using (var cmd = new SqlCommand(sqlUsuarios, conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                usuarios.Add(new ComboBoxUsuario
                                {
                                    UsuarioID = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    NombreCompleto = reader.GetString(2)
                                });
                            }
                        }

                        // Actualizar UI en el thread principal (marshalling)
                        this.Invoke((MethodInvoker)delegate
                        {
                            // Suspender eventos temporalmente para evitar triggers innecesarios
                            cmbModulo.SelectedIndexChanged -= (s, e) => AplicarFiltros();
                            cmbUsuario.SelectedIndexChanged -= (s, e) => AplicarFiltros();
                            cmbAccion.SelectedIndexChanged -= (s, e) => AplicarFiltros();

                            // Módulos
                            cmbModulo.Items.Clear();
                            cmbModulo.Items.Add("Todos los Módulos");
                            foreach (var modulo in cacheModulos)
                            {
                                cmbModulo.Items.Add(modulo);
                            }
                            cmbModulo.SelectedIndex = 0;

                            // Usuarios
                            cmbUsuario.Items.Clear();
                            cmbUsuario.Items.Add("Todos los Usuarios");
                            foreach (var usuario in usuarios)
                            {
                                cmbUsuario.Items.Add(usuario);
                            }
                            cmbUsuario.DisplayMember = "Display";
                            cmbUsuario.SelectedIndex = 0;

                            // Acciones
                            cmbAccion.Items.Clear();
                            cmbAccion.Items.Add("Todas las Acciones");
                            foreach (var accion in cacheAcciones)
                            {
                                cmbAccion.Items.Add(accion);
                            }
                            cmbAccion.SelectedIndex = 0;

                            // Reactivar eventos
                            cmbModulo.SelectedIndexChanged += (s, e) => AplicarFiltros();
                            cmbUsuario.SelectedIndexChanged += (s, e) => AplicarFiltros();
                            cmbAccion.SelectedIndexChanged += (s, e) => AplicarFiltros();
                        });
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show($"Error al cargar filtros:\n\n{ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            });
        }

        /// <summary>
        /// Carga TODOS los datos una sola vez al abrir el formulario (ULTRA OPTIMIZADO - ASYNC)
        /// - Carga asíncrona para no bloquear UI
        /// - Query optimizado con NOLOCK
        /// - Persistencia en cache estático
        /// </summary>
        private async Task CargarDatosInicialesAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    DataTable dt = new DataTable();

                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();

                        // Query optimizado: Solo columnas necesarias, índices en FechaHora
                        string sql = @"
                            SELECT TOP 5000
                                A.AuditoriaID,
                                A.FechaHora,
                                U.Username,
                                A.Categoria,
                                A.Modulo,
                                A.Formulario,
                                A.Accion,
                                A.Detalle,
                                A.DireccionIP,
                                A.NombreMaquina
                            FROM Auditoria A WITH (NOLOCK)
                            LEFT JOIN Usuarios U WITH (NOLOCK) ON A.UsuarioID = U.UsuarioID
                            ORDER BY A.FechaHora DESC";

                        using (var cmd = new SqlCommand(sql, conn))
                        {
                            cmd.CommandTimeout = 30; // Timeout de 30 segundos
                            using (var adapter = new SqlDataAdapter(cmd))
                            {
                                adapter.Fill(dt);
                            }
                        }
                    }

                    // Actualizar cache estático y UI en el thread principal
                    this.Invoke((MethodInvoker)delegate
                    {
                        dtAuditoriaCompleta = dt; // Guardar en cache estático
                        ultimaActualizacionDatos = DateTime.Now; // Registrar timestamp

                        bindingSource.DataSource = dtAuditoriaCompleta;
                        dgvAuditoria.DataSource = bindingSource;

                        // Aplicar filtros iniciales
                        AplicarFiltros();
                    });
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show($"Error al cargar auditoría:\n\n{ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            });
        }

        /// <summary>
        /// Aplica filtros EN MEMORIA (sin ir a BD) - ULTRA OPTIMIZADO
        /// - Usa StringBuilder para construcción eficiente de filtros
        /// - Suspende DataGridView para evitar repintados múltiples
        /// - Filtrado instantáneo (<100ms)
        /// </summary>
        private void AplicarFiltros()
        {
            if (dtAuditoriaCompleta == null || dtAuditoriaCompleta.Rows.Count == 0)
            {
                lblTotalRegistros.Text = "Total: 0 registros";
                lblFechaConsulta.Text = "Última consulta: --";
                return;
            }

            try
            {
                // Suspender actualizaciones para evitar repintados múltiples
                dgvAuditoria.SuspendLayout();
                bindingSource.RaiseListChangedEvents = false;

                var filtros = new List<string>();

                // Filtro de fechas
                if (dtpDesde.Checked)
                {
                    string fechaDesde = dtpDesde.Value.Date.ToString("yyyy-MM-dd");
                    filtros.Add($"FechaHora >= #{fechaDesde}#");
                }

                if (dtpHasta.Checked)
                {
                    string fechaHasta = dtpHasta.Value.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
                    filtros.Add($"FechaHora <= #{fechaHasta}#");
                }

                // Filtro de módulo
                if (cmbModulo.SelectedIndex > 0)
                {
                    string modulo = cmbModulo.SelectedItem.ToString().Replace("'", "''");
                    filtros.Add($"Modulo = '{modulo}'");
                }

                // Filtro de usuario
                if (cmbUsuario.SelectedIndex > 0)
                {
                    var usuario = cmbUsuario.SelectedItem as ComboBoxUsuario;
                    filtros.Add($"Username = '{usuario.Username.Replace("'", "''")}'");
                }

                // Filtro de acción
                if (cmbAccion.SelectedIndex > 0)
                {
                    string accion = cmbAccion.SelectedItem.ToString().Replace("'", "''");
                    filtros.Add($"Accion = '{accion}'");
                }

                // Filtro de búsqueda (optimizado con StringBuilder)
                if (!string.IsNullOrWhiteSpace(txtBuscar.Text) && txtBuscar.Text != "Buscar en detalle...")
                {
                    string busqueda = txtBuscar.Text.Replace("'", "''");
                    var sb = new StringBuilder();
                    sb.Append("(");
                    sb.Append($"Detalle LIKE '%{busqueda}%' OR ");
                    sb.Append($"Accion LIKE '%{busqueda}%' OR ");
                    sb.Append($"Modulo LIKE '%{busqueda}%' OR ");
                    sb.Append($"Formulario LIKE '%{busqueda}%' OR ");
                    sb.Append($"Categoria LIKE '%{busqueda}%' OR ");
                    sb.Append($"Username LIKE '%{busqueda}%' OR ");
                    sb.Append($"DireccionIP LIKE '%{busqueda}%' OR ");
                    sb.Append($"NombreMaquina LIKE '%{busqueda}%'");
                    sb.Append(")");
                    filtros.Add(sb.ToString());
                }

                // Aplicar filtro
                bindingSource.Filter = filtros.Count > 0 ? string.Join(" AND ", filtros) : "";

                // Reanudar actualizaciones
                bindingSource.RaiseListChangedEvents = true;
                bindingSource.ResetBindings(false);

                // Actualizar estadísticas
                ActualizarEstadisticas();

                // Limpiar selección
                if (dgvAuditoria.Rows.Count > 0)
                {
                    dgvAuditoria.ClearSelection();
                    dgvAuditoria.CurrentCell = null;
                }

                dgvAuditoria.ResumeLayout();
            }
            catch (Exception ex)
            {
                dgvAuditoria.ResumeLayout();
                bindingSource.RaiseListChangedEvents = true;
                MessageBox.Show($"Error al aplicar filtros:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarEstadisticas()
        {
            int totalFiltrado = bindingSource.Count;
            int totalGeneral = dtAuditoriaCompleta?.Rows.Count ?? 0;

            lblTotalRegistros.Text = totalFiltrado == totalGeneral
                ? $"Total: {totalFiltrado:N0} registros"
                : $"Mostrando: {totalFiltrado:N0} de {totalGeneral:N0} registros";

            // Mostrar si viene de cache o BD
            string origen = ultimaActualizacionDatos.HasValue
                ? $"Última actualización: {ultimaActualizacionDatos.Value:dd/MM/yyyy HH:mm:ss}"
                : $"Última actualización: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

            lblFechaConsulta.Text = origen;
        }

        private async void BtnActualizar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "¿Desea recargar los datos desde la base de datos?\n\n" +
                "Esto descartará el cache y cargará los últimos 5000 registros actualizados.",
                "Actualizar Datos",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                lblTotalRegistros.Text = "⏳ Recargando datos desde BD...";
                lblFechaConsulta.Text = "";

                // Invalidar cache para forzar recarga completa
                dtAuditoriaCompleta = null;
                ultimaActualizacionDatos = null;

                // Recargar tanto filtros como datos (forzar actualización de cache)
                await Task.WhenAll(
                    CargarFiltrosAsync(forzarActualizacion: true),
                    CargarDatosInicialesAsync()
                );
            }
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            dtpDesde.Value = DateTime.Now.AddMonths(-1);
            dtpHasta.Value = DateTime.Now;
            dtpDesde.Checked = true;
            dtpHasta.Checked = true;
            cmbModulo.SelectedIndex = 0;
            cmbUsuario.SelectedIndex = 0;
            cmbAccion.SelectedIndex = 0;

            txtBuscar.Text = "Buscar en detalle...";
            txtBuscar.ForeColor = Color.Gray;

            AplicarFiltros();
        }

        private void DgvAuditoria_DoubleClick(object sender, EventArgs e)
        {
            if (dgvAuditoria.SelectedRows.Count == 0) return;

            var row = dgvAuditoria.SelectedRows[0];

            MessageBox.Show(
                $"Detalle de Auditoría\n\n" +
                $"ID: {row.Cells["AuditoriaID"].Value}\n" +
                $"Fecha: {row.Cells["FechaHora"].Value}\n" +
                $"Usuario: {row.Cells["Username"].Value}\n" +
                $"Acción: {row.Cells["Accion"].Value}\n\n" +
                $"FormDetalleAuditoria se implementará próximamente.",
                "Vista Previa de Detalle",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void BtnDashboard_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "📈 Dashboard de Auditoría\n\n" +
                "Se implementará próximamente.",
                "Dashboard",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void BtnBuscarDetalles_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "🔍 Búsqueda Avanzada de Auditoría\n\n" +
                "Se implementará próximamente.",
                "Búsqueda Avanzada",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void BtnReportePDF_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "📄 Reporte PDF de Auditoría\n\n" +
                "Se implementará próximamente.",
                "Reporte PDF",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            if (bindingSource == null || bindingSource.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ExportarExcel();
        }

        private void ExportarExcel()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Obtener vista filtrada
                DataView vistaFiltrada = (DataView)bindingSource.List;

                if (vistaFiltrada.Count == 0)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("No hay datos para exportar.", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (var workbook = new XLWorkbook())
                {
                    // ============================================
                    // HOJA 1: PORTADA
                    // ============================================
                    var wsPortada = workbook.Worksheets.Add("Portada");

                    // Logo y título principal (fusionar celdas A1:J5)
                    wsPortada.Range("A1:J5").Merge();
                    wsPortada.Cell("A1").Value = "MOFIS ERP";
                    wsPortada.Cell("A1").Style.Font.FontSize = 36;
                    wsPortada.Cell("A1").Style.Font.Bold = true;
                    wsPortada.Cell("A1").Style.Font.FontColor = XLColor.White;
                    wsPortada.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
                    wsPortada.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsPortada.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    // Subtítulo
                    wsPortada.Range("A7:J8").Merge();
                    wsPortada.Cell("A7").Value = "REPORTE DE AUDITORÍA DEL SISTEMA";
                    wsPortada.Cell("A7").Style.Font.FontSize = 24;
                    wsPortada.Cell("A7").Style.Font.Bold = true;
                    wsPortada.Cell("A7").Style.Font.FontColor = XLColor.FromArgb(0, 120, 212);
                    wsPortada.Cell("A7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsPortada.Cell("A7").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    // Información del reporte
                    int filaPortada = 12;

                    wsPortada.Cell($"C{filaPortada}").Value = "INFORMACIÓN DEL REPORTE";
                    wsPortada.Cell($"C{filaPortada}").Style.Font.FontSize = 14;
                    wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                    wsPortada.Cell($"C{filaPortada}").Style.Font.FontColor = XLColor.FromArgb(0, 120, 212);
                    filaPortada += 2;

                    // Tabla de información
                    var infoInicio = filaPortada;

                    wsPortada.Cell($"C{filaPortada}").Value = "Fecha de Generación:";
                    wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                    wsPortada.Cell($"D{filaPortada}").Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    filaPortada++;

                    wsPortada.Cell($"C{filaPortada}").Value = "Generado por:";
                    wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                    wsPortada.Cell($"D{filaPortada}").Value = SesionActual.NombreCompleto;
                    filaPortada++;

                    wsPortada.Cell($"C{filaPortada}").Value = "Usuario:";
                    wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                    wsPortada.Cell($"D{filaPortada}").Value = SesionActual.Username;
                    filaPortada++;

                    wsPortada.Cell($"C{filaPortada}").Value = "Rol:";
                    wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                    wsPortada.Cell($"D{filaPortada}").Value = SesionActual.NombreRol;
                    filaPortada++;

                    wsPortada.Cell($"C{filaPortada}").Value = "Total de Registros:";
                    wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                    wsPortada.Cell($"D{filaPortada}").Value = vistaFiltrada.Count;
                    wsPortada.Cell($"D{filaPortada}").Style.Font.Bold = true;
                    wsPortada.Cell($"D{filaPortada}").Style.Font.FontSize = 12;
                    wsPortada.Cell($"D{filaPortada}").Style.Font.FontColor = XLColor.FromArgb(0, 120, 212);

                    // Aplicar bordes a la tabla de información
                    wsPortada.Range($"C{infoInicio}:D{filaPortada}").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    wsPortada.Range($"C{infoInicio}:D{filaPortada}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    filaPortada += 3;

                    // Criterios de filtrado
                    wsPortada.Cell($"C{filaPortada}").Value = "CRITERIOS DE FILTRADO APLICADOS";
                    wsPortada.Cell($"C{filaPortada}").Style.Font.FontSize = 14;
                    wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                    wsPortada.Cell($"C{filaPortada}").Style.Font.FontColor = XLColor.FromArgb(0, 120, 212);
                    filaPortada += 2;

                    var criteriosInicio = filaPortada;

                    if (dtpDesde.Checked)
                    {
                        wsPortada.Cell($"C{filaPortada}").Value = "Fecha Desde:";
                        wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                        wsPortada.Cell($"D{filaPortada}").Value = dtpDesde.Value.ToString("dd/MM/yyyy");
                        filaPortada++;
                    }

                    if (dtpHasta.Checked)
                    {
                        wsPortada.Cell($"C{filaPortada}").Value = "Fecha Hasta:";
                        wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                        wsPortada.Cell($"D{filaPortada}").Value = dtpHasta.Value.ToString("dd/MM/yyyy");
                        filaPortada++;
                    }

                    if (cmbModulo.SelectedIndex > 0)
                    {
                        wsPortada.Cell($"C{filaPortada}").Value = "Módulo:";
                        wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                        wsPortada.Cell($"D{filaPortada}").Value = cmbModulo.SelectedItem.ToString();
                        filaPortada++;
                    }

                    if (cmbUsuario.SelectedIndex > 0)
                    {
                        var usuario = cmbUsuario.SelectedItem as ComboBoxUsuario;
                        wsPortada.Cell($"C{filaPortada}").Value = "Usuario:";
                        wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                        wsPortada.Cell($"D{filaPortada}").Value = usuario.NombreCompleto;
                        filaPortada++;
                    }

                    if (cmbAccion.SelectedIndex > 0)
                    {
                        wsPortada.Cell($"C{filaPortada}").Value = "Acción:";
                        wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                        wsPortada.Cell($"D{filaPortada}").Value = cmbAccion.SelectedItem.ToString();
                        filaPortada++;
                    }

                    if (!string.IsNullOrWhiteSpace(txtBuscar.Text) && txtBuscar.Text != "Buscar en detalle...")
                    {
                        wsPortada.Cell($"C{filaPortada}").Value = "Búsqueda:";
                        wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                        wsPortada.Cell($"D{filaPortada}").Value = txtBuscar.Text;
                        filaPortada++;
                    }

                    if (filaPortada > criteriosInicio)
                    {
                        wsPortada.Range($"C{criteriosInicio}:D{filaPortada - 1}").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                        wsPortada.Range($"C{criteriosInicio}:D{filaPortada - 1}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    }
                    else
                    {
                        wsPortada.Cell($"C{filaPortada}").Value = "Sin filtros aplicados (mostrando todos los registros)";
                        wsPortada.Cell($"C{filaPortada}").Style.Font.Italic = true;
                        wsPortada.Cell($"C{filaPortada}").Style.Font.FontColor = XLColor.Gray;
                    }

                    wsPortada.Columns().AdjustToContents();

                    // ============================================
                    // HOJA 2: RESUMEN EJECUTIVO
                    // ============================================
                    var wsResumen = workbook.Worksheets.Add("Resumen Ejecutivo");

                    // Título
                    wsResumen.Range("A1:F1").Merge();
                    wsResumen.Cell("A1").Value = "RESUMEN EJECUTIVO";
                    wsResumen.Cell("A1").Style.Font.FontSize = 20;
                    wsResumen.Cell("A1").Style.Font.Bold = true;
                    wsResumen.Cell("A1").Style.Font.FontColor = XLColor.White;
                    wsResumen.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
                    wsResumen.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsResumen.Row(1).Height = 35;

                    int filaResumen = 3;

                    // Tarjetas de estadísticas (4 tarjetas en una fila)
                    // Tarjeta 1: Total de Registros
                    wsResumen.Range($"A{filaResumen}:B{filaResumen + 2}").Merge();
                    wsResumen.Cell($"A{filaResumen}").Value = vistaFiltrada.Count;
                    wsResumen.Cell($"A{filaResumen}").Style.Font.FontSize = 28;
                    wsResumen.Cell($"A{filaResumen}").Style.Font.Bold = true;
                    wsResumen.Cell($"A{filaResumen}").Style.Font.FontColor = XLColor.FromArgb(0, 120, 212);
                    wsResumen.Cell($"A{filaResumen}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsResumen.Cell($"A{filaResumen}").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    wsResumen.Cell($"A{filaResumen + 3}").Value = "Total de Registros";
                    wsResumen.Cell($"A{filaResumen + 3}").Style.Font.Bold = true;
                    wsResumen.Cell($"A{filaResumen + 3}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsResumen.Range($"A{filaResumen}:B{filaResumen + 3}").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    wsResumen.Range($"A{filaResumen}:B{filaResumen + 3}").Style.Fill.BackgroundColor = XLColor.FromArgb(240, 248, 255);

                    // Tarjeta 2: Módulos únicos
                    var modulosUnicos = vistaFiltrada.ToTable().AsEnumerable()
                        .Select(r => r.Field<string>("Modulo"))
                        .Where(m => !string.IsNullOrEmpty(m))
                        .Distinct()
                        .Count();

                    wsResumen.Range($"D{filaResumen}:E{filaResumen + 2}").Merge();
                    wsResumen.Cell($"D{filaResumen}").Value = modulosUnicos;
                    wsResumen.Cell($"D{filaResumen}").Style.Font.FontSize = 28;
                    wsResumen.Cell($"D{filaResumen}").Style.Font.Bold = true;
                    wsResumen.Cell($"D{filaResumen}").Style.Font.FontColor = XLColor.FromArgb(34, 139, 34);
                    wsResumen.Cell($"D{filaResumen}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsResumen.Cell($"D{filaResumen}").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    wsResumen.Cell($"D{filaResumen + 3}").Value = "Módulos Activos";
                    wsResumen.Cell($"D{filaResumen + 3}").Style.Font.Bold = true;
                    wsResumen.Cell($"D{filaResumen + 3}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsResumen.Range($"D{filaResumen}:E{filaResumen + 3}").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    wsResumen.Range($"D{filaResumen}:E{filaResumen + 3}").Style.Fill.BackgroundColor = XLColor.FromArgb(240, 255, 240);

                    filaResumen += 6;

                    // TOP 10 ACCIONES MÁS FRECUENTES
                    wsResumen.Cell($"A{filaResumen}").Value = "TOP 10 ACCIONES MÁS FRECUENTES";
                    wsResumen.Cell($"A{filaResumen}").Style.Font.FontSize = 14;
                    wsResumen.Cell($"A{filaResumen}").Style.Font.Bold = true;
                    wsResumen.Cell($"A{filaResumen}").Style.Font.FontColor = XLColor.FromArgb(0, 120, 212);
                    filaResumen += 2;

                    var accionesPorTipo = vistaFiltrada.ToTable().AsEnumerable()
                        .GroupBy(r => r.Field<string>("Accion"))
                        .Select(g => new { Accion = g.Key ?? "N/A", Cantidad = g.Count() })
                        .OrderByDescending(x => x.Cantidad)
                        .Take(10)
                        .ToList();

                    // Encabezados
                    wsResumen.Cell($"A{filaResumen}").Value = "#";
                    wsResumen.Cell($"B{filaResumen}").Value = "Acción";
                    wsResumen.Cell($"C{filaResumen}").Value = "Cantidad";
                    wsResumen.Cell($"D{filaResumen}").Value = "Porcentaje";
                    wsResumen.Range($"A{filaResumen}:D{filaResumen}").Style.Font.Bold = true;
                    wsResumen.Range($"A{filaResumen}:D{filaResumen}").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
                    wsResumen.Range($"A{filaResumen}:D{filaResumen}").Style.Font.FontColor = XLColor.White;
                    wsResumen.Range($"A{filaResumen}:D{filaResumen}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsResumen.Range($"A{filaResumen}:D{filaResumen}").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    filaResumen++;

                    int ranking = 1;
                    foreach (var item in accionesPorTipo)
                    {
                        double porcentaje = (double)item.Cantidad / vistaFiltrada.Count * 100;

                        wsResumen.Cell($"A{filaResumen}").Value = ranking;
                        wsResumen.Cell($"A{filaResumen}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        wsResumen.Cell($"B{filaResumen}").Value = item.Accion;

                        wsResumen.Cell($"C{filaResumen}").Value = item.Cantidad;
                        wsResumen.Cell($"C{filaResumen}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        wsResumen.Cell($"D{filaResumen}").Value = porcentaje / 100;
                        wsResumen.Cell($"D{filaResumen}").Style.NumberFormat.Format = "0.00%";
                        wsResumen.Cell($"D{filaResumen}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // Color de fondo alternado
                        if (ranking % 2 == 0)
                        {
                            wsResumen.Range($"A{filaResumen}:D{filaResumen}").Style.Fill.BackgroundColor = XLColor.FromArgb(240, 240, 240);
                        }

                        wsResumen.Range($"A{filaResumen}:D{filaResumen}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        ranking++;
                        filaResumen++;
                    }

                    filaResumen += 2;

                    // TOP 10 USUARIOS MÁS ACTIVOS
                    wsResumen.Cell($"A{filaResumen}").Value = "TOP 10 USUARIOS MÁS ACTIVOS";
                    wsResumen.Cell($"A{filaResumen}").Style.Font.FontSize = 14;
                    wsResumen.Cell($"A{filaResumen}").Style.Font.Bold = true;
                    wsResumen.Cell($"A{filaResumen}").Style.Font.FontColor = XLColor.FromArgb(0, 120, 212);
                    filaResumen += 2;

                    var usuariosMasActivos = vistaFiltrada.ToTable().AsEnumerable()
                        .GroupBy(r => r.Field<string>("Username") ?? "N/A")
                        .Select(g => new { Usuario = g.Key, Cantidad = g.Count() })
                        .OrderByDescending(x => x.Cantidad)
                        .Take(10)
                        .ToList();

                    // Encabezados
                    wsResumen.Cell($"A{filaResumen}").Value = "#";
                    wsResumen.Cell($"B{filaResumen}").Value = "Usuario";
                    wsResumen.Cell($"C{filaResumen}").Value = "Total Acciones";
                    wsResumen.Cell($"D{filaResumen}").Value = "Porcentaje";
                    wsResumen.Range($"A{filaResumen}:D{filaResumen}").Style.Font.Bold = true;
                    wsResumen.Range($"A{filaResumen}:D{filaResumen}").Style.Fill.BackgroundColor = XLColor.FromArgb(34, 139, 34);
                    wsResumen.Range($"A{filaResumen}:D{filaResumen}").Style.Font.FontColor = XLColor.White;
                    wsResumen.Range($"A{filaResumen}:D{filaResumen}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsResumen.Range($"A{filaResumen}:D{filaResumen}").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    filaResumen++;

                    ranking = 1;
                    foreach (var item in usuariosMasActivos)
                    {
                        double porcentaje = (double)item.Cantidad / vistaFiltrada.Count * 100;

                        wsResumen.Cell($"A{filaResumen}").Value = ranking;
                        wsResumen.Cell($"A{filaResumen}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        wsResumen.Cell($"B{filaResumen}").Value = item.Usuario;

                        wsResumen.Cell($"C{filaResumen}").Value = item.Cantidad;
                        wsResumen.Cell($"C{filaResumen}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        wsResumen.Cell($"D{filaResumen}").Value = porcentaje / 100;
                        wsResumen.Cell($"D{filaResumen}").Style.NumberFormat.Format = "0.00%";
                        wsResumen.Cell($"D{filaResumen}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        if (ranking % 2 == 0)
                        {
                            wsResumen.Range($"A{filaResumen}:D{filaResumen}").Style.Fill.BackgroundColor = XLColor.FromArgb(240, 240, 240);
                        }

                        wsResumen.Range($"A{filaResumen}:D{filaResumen}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        ranking++;
                        filaResumen++;
                    }

                    wsResumen.Columns().AdjustToContents();

                    // ============================================
                    // HOJA 3: DETALLE COMPLETO
                    // ============================================
                    var wsDetalle = workbook.Worksheets.Add("Detalle Completo");

                    // Título
                    wsDetalle.Range("A1:I1").Merge();
                    wsDetalle.Cell("A1").Value = "DETALLE COMPLETO DE REGISTROS";
                    wsDetalle.Cell("A1").Style.Font.FontSize = 16;
                    wsDetalle.Cell("A1").Style.Font.Bold = true;
                    wsDetalle.Cell("A1").Style.Font.FontColor = XLColor.White;
                    wsDetalle.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
                    wsDetalle.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsDetalle.Row(1).Height = 30;

                    // Encabezados
                    string[] encabezados = { "Fecha/Hora", "Usuario", "Categoría", "Módulo", "Formulario", "Acción", "Detalle", "IP", "Máquina" };

                    for (int i = 0; i < encabezados.Length; i++)
                    {
                        wsDetalle.Cell(3, i + 1).Value = encabezados[i];
                    }

                    var headerRange = wsDetalle.Range(3, 1, 3, encabezados.Length);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Font.FontSize = 11;
                    headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
                    headerRange.Style.Font.FontColor = XLColor.White;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    wsDetalle.Row(3).Height = 25;

                    // Datos
                    int filaExcel = 4;
                    foreach (DataRowView rowView in vistaFiltrada)
                    {
                        DataRow row = rowView.Row;

                        if (row["FechaHora"] != DBNull.Value)
                        {
                            wsDetalle.Cell(filaExcel, 1).Value = Convert.ToDateTime(row["FechaHora"]);
                            wsDetalle.Cell(filaExcel, 1).Style.DateFormat.Format = "dd/mm/yyyy hh:mm:ss";
                        }

                        wsDetalle.Cell(filaExcel, 2).Value = row["Username"] != DBNull.Value ? row["Username"].ToString() : "N/A";
                        wsDetalle.Cell(filaExcel, 3).Value = row["Categoria"] != DBNull.Value ? row["Categoria"].ToString() : "";
                        wsDetalle.Cell(filaExcel, 4).Value = row["Modulo"] != DBNull.Value ? row["Modulo"].ToString() : "";
                        wsDetalle.Cell(filaExcel, 5).Value = row["Formulario"] != DBNull.Value ? row["Formulario"].ToString() : "";
                        wsDetalle.Cell(filaExcel, 6).Value = row["Accion"] != DBNull.Value ? row["Accion"].ToString() : "";
                        wsDetalle.Cell(filaExcel, 7).Value = row["Detalle"] != DBNull.Value ? row["Detalle"].ToString() : "";
                        wsDetalle.Cell(filaExcel, 8).Value = row["DireccionIP"] != DBNull.Value ? row["DireccionIP"].ToString() : "";
                        wsDetalle.Cell(filaExcel, 9).Value = row["NombreMaquina"] != DBNull.Value ? row["NombreMaquina"].ToString() : "";

                        // Alternar colores
                        if (filaExcel % 2 == 0)
                        {
                            wsDetalle.Range(filaExcel, 1, filaExcel, encabezados.Length).Style.Fill.BackgroundColor = XLColor.FromArgb(240, 248, 255);
                        }

                        // Bordes
                        wsDetalle.Range(filaExcel, 1, filaExcel, encabezados.Length).Style.Border.OutsideBorder = XLBorderStyleValues.Hair;

                        filaExcel++;
                    }

                    // Ajustar anchos
                    wsDetalle.Column(1).Width = 18;
                    wsDetalle.Column(2).Width = 20;
                    wsDetalle.Column(3).Width = 15;
                    wsDetalle.Column(4).Width = 20;
                    wsDetalle.Column(5).Width = 30;
                    wsDetalle.Column(6).Width = 25;
                    wsDetalle.Column(7).Width = 60;
                    wsDetalle.Column(8).Width = 15;
                    wsDetalle.Column(9).Width = 20;

                    // ============================================
                    // HOJA 4: POR MÓDULO
                    // ============================================
                    var wsPorModulo = workbook.Worksheets.Add("Por Módulo");

                    // Título
                    wsPorModulo.Range("A1:D1").Merge();
                    wsPorModulo.Cell("A1").Value = "RESUMEN POR MÓDULO";
                    wsPorModulo.Cell("A1").Style.Font.FontSize = 16;
                    wsPorModulo.Cell("A1").Style.Font.Bold = true;
                    wsPorModulo.Cell("A1").Style.Font.FontColor = XLColor.White;
                    wsPorModulo.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
                    wsPorModulo.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsPorModulo.Row(1).Height = 30;

                    var datosPorModulo = vistaFiltrada.ToTable().AsEnumerable()
                        .GroupBy(r => r.Field<string>("Modulo") ?? "N/A")
                        .Select(g => new
                        {
                            Modulo = g.Key,
                            Cantidad = g.Count(),
                            PrimeraAccion = g.Min(r => r.Field<DateTime>("FechaHora")),
                            UltimaAccion = g.Max(r => r.Field<DateTime>("FechaHora"))
                        })
                        .OrderByDescending(x => x.Cantidad)
                        .ToList();

                    // Encabezados
                    wsPorModulo.Cell("A3").Value = "Módulo";
                    wsPorModulo.Cell("B3").Value = "Total Acciones";
                    wsPorModulo.Cell("C3").Value = "Primera Acción";
                    wsPorModulo.Cell("D3").Value = "Última Acción";

                    var headerModulo = wsPorModulo.Range("A3:D3");
                    headerModulo.Style.Font.Bold = true;
                    headerModulo.Style.Font.FontSize = 11;
                    headerModulo.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
                    headerModulo.Style.Font.FontColor = XLColor.White;
                    headerModulo.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    headerModulo.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                    int filaModulo = 4;
                    foreach (var item in datosPorModulo)
                    {
                        wsPorModulo.Cell(filaModulo, 1).Value = item.Modulo;
                        wsPorModulo.Cell(filaModulo, 2).Value = item.Cantidad;
                        wsPorModulo.Cell(filaModulo, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        wsPorModulo.Cell(filaModulo, 3).Value = item.PrimeraAccion;
                        wsPorModulo.Cell(filaModulo, 3).Style.DateFormat.Format = "dd/mm/yyyy hh:mm";
                        wsPorModulo.Cell(filaModulo, 4).Value = item.UltimaAccion;
                        wsPorModulo.Cell(filaModulo, 4).Style.DateFormat.Format = "dd/mm/yyyy hh:mm";

                        if (filaModulo % 2 == 0)
                        {
                            wsPorModulo.Range(filaModulo, 1, filaModulo, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(240, 248, 255);
                        }

                        wsPorModulo.Range(filaModulo, 1, filaModulo, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        filaModulo++;
                    }

                    wsPorModulo.Columns().AdjustToContents();

                    // ============================================
                    // HOJA 5: POR USUARIO
                    // ============================================
                    var wsPorUsuario = workbook.Worksheets.Add("Por Usuario");

                    // Título
                    wsPorUsuario.Range("A1:D1").Merge();
                    wsPorUsuario.Cell("A1").Value = "RESUMEN POR USUARIO";
                    wsPorUsuario.Cell("A1").Style.Font.FontSize = 16;
                    wsPorUsuario.Cell("A1").Style.Font.Bold = true;
                    wsPorUsuario.Cell("A1").Style.Font.FontColor = XLColor.White;
                    wsPorUsuario.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromArgb(34, 139, 34);
                    wsPorUsuario.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsPorUsuario.Row(1).Height = 30;

                    var datosPorUsuario = vistaFiltrada.ToTable().AsEnumerable()
                        .GroupBy(r => r.Field<string>("Username") ?? "N/A")
                        .Select(g => new
                        {
                            Usuario = g.Key,
                            Cantidad = g.Count(),
                            PrimeraAccion = g.Min(r => r.Field<DateTime>("FechaHora")),
                            UltimaAccion = g.Max(r => r.Field<DateTime>("FechaHora"))
                        })
                        .OrderByDescending(x => x.Cantidad)
                        .ToList();

                    // Encabezados
                    wsPorUsuario.Cell("A3").Value = "Usuario";
                    wsPorUsuario.Cell("B3").Value = "Total Acciones";
                    wsPorUsuario.Cell("C3").Value = "Primera Acción";
                    wsPorUsuario.Cell("D3").Value = "Última Acción";

                    var headerUsuario = wsPorUsuario.Range("A3:D3");
                    headerUsuario.Style.Font.Bold = true;
                    headerUsuario.Style.Font.FontSize = 11;
                    headerUsuario.Style.Fill.BackgroundColor = XLColor.FromArgb(34, 139, 34);
                    headerUsuario.Style.Font.FontColor = XLColor.White;
                    headerUsuario.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    headerUsuario.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                    int filaUsuario = 4;
                    foreach (var item in datosPorUsuario)
                    {
                        wsPorUsuario.Cell(filaUsuario, 1).Value = item.Usuario;
                        wsPorUsuario.Cell(filaUsuario, 2).Value = item.Cantidad;
                        wsPorUsuario.Cell(filaUsuario, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        wsPorUsuario.Cell(filaUsuario, 3).Value = item.PrimeraAccion;
                        wsPorUsuario.Cell(filaUsuario, 3).Style.DateFormat.Format = "dd/mm/yyyy hh:mm";
                        wsPorUsuario.Cell(filaUsuario, 4).Value = item.UltimaAccion;
                        wsPorUsuario.Cell(filaUsuario, 4).Style.DateFormat.Format = "dd/mm/yyyy hh:mm";

                        if (filaUsuario % 2 == 0)
                        {
                            wsPorUsuario.Range(filaUsuario, 1, filaUsuario, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(240, 255, 240);
                        }

                        wsPorUsuario.Range(filaUsuario, 1, filaUsuario, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        filaUsuario++;
                    }

                    wsPorUsuario.Columns().AdjustToContents();

                    // Guardar archivo
                    SaveFileDialog saveDialog = new SaveFileDialog
                    {
                        Filter = "Excel Files|*.xlsx",
                        FileName = $"Auditoria_Completo_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                        Title = "Guardar Reporte Completo de Auditoría"
                    };

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        workbook.SaveAs(saveDialog.FileName);

                        // Registrar en auditoría
                        AuditoriaHelper.RegistrarAccion(
                            SesionActual.UsuarioID,
                            AuditoriaAcciones.AuditoriaGeneral.EXPORTAR_AUDITORIA_EXCEL,
                            AuditoriaAcciones.Categorias.SISTEMA,
                            AuditoriaAcciones.Modulos.AUDITORIA_GENERAL,
                            "FormAuditoria",
                            detalle: $"Exportación completa de {vistaFiltrada.Count} registros a Excel. Archivo: {System.IO.Path.GetFileName(saveDialog.FileName)}"
                        );

                        MessageBox.Show(
                            $"✅ Reporte exportado exitosamente\n\n" +
                            $"Archivo: {System.IO.Path.GetFileName(saveDialog.FileName)}\n" +
                            $"Registros: {vistaFiltrada.Count:N0}\n\n" +
                            $"Hojas generadas:\n" +
                            $"• Portada\n" +
                            $"• Resumen Ejecutivo (estadísticas y top 10)\n" +
                            $"• Detalle Completo (todos los registros)\n" +
                            $"• Por Módulo (agrupado)\n" +
                            $"• Por Usuario (agrupado)",
                            "Exportación Exitosa",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        // Preguntar si desea abrir el archivo
                        DialogResult abrirArchivo = MessageBox.Show(
                            "¿Desea abrir el archivo ahora?",
                            "Abrir Archivo",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (abrirArchivo == DialogResult.Yes)
                        {
                            try
                            {
                                System.Diagnostics.Process.Start(saveDialog.FileName);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"No se pudo abrir el archivo:\n\n{ex.Message}", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al exportar:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void BtnVolver_Click(object sender, EventArgs e)
        {
            FormDashboardSistema dashboardSistema = new FormDashboardSistema(formPrincipal);
            formPrincipal.CargarContenidoPanel(dashboardSistema);
        }

        private class ComboBoxUsuario
        {
            public int UsuarioID { get; set; }
            public string Username { get; set; }
            public string NombreCompleto { get; set; }
            public string Display => $"{NombreCompleto} ({Username})";
        }
    }
}

**FIN DEL DOCUMENTO**


📌 Regla de oro

OpenXML / QuestPDF NO se usan dentro de Forms, solo en clases de reporte.