# 📋 PLAN DE DESARROLLO: FormSolicitudPago.cs

## MÓDULO: CONTABILIDAD > CUENTAS POR PAGAR > CARTAS Y SOLICITUDES

**Fecha de creación:** 19/01/2026  
**Versión:** 1.0  
**Proyecto:** MOFIS-ERP (Fiducorp)  
**Desarrollador:** Cysero  

---

## 📌 ÍNDICE

1. [Resumen Ejecutivo](#1-resumen-ejecutivo)
2. [Arquitectura y Navegación](#2-arquitectura-y-navegación)
3. [Estructura de Base de Datos](#3-estructura-de-base-de-datos)
4. [Diseño Visual del Formulario](#4-diseño-visual-del-formulario)
5. [Secciones del Formulario](#5-secciones-del-formulario)
6. [Mini-Forms Modales](#6-mini-forms-modales)
7. [Lógica de Cálculos](#7-lógica-de-cálculos)
8. [Conversión de Moneda](#8-conversión-de-moneda)
9. [Validaciones](#9-validaciones)
10. [Sistema de Firmas Digitales](#10-sistema-de-firmas-digitales)
11. [Exportación e Impresión](#11-exportación-e-impresión)
12. [Configuraciones del Módulo](#12-configuraciones-del-módulo)
13. [Plan de Implementación por Fases](#13-plan-de-implementación-por-fases)
14. [Controles a Crear en el Diseñador](#14-controles-a-crear-en-el-diseñador)

---

## 1. RESUMEN EJECUTIVO

### 1.1 Descripción
FormSolicitudPago es el formulario principal del módulo Cuentas por Pagar. Permite registrar solicitudes de pago a proveedores con soporte para:

- Múltiples subtotales por solicitud
- Múltiples comprobantes NCF
- Cálculo automático de ITBIS y retenciones
- Conversión multi-moneda (5 métodos)
- Notas de Crédito y Débito (2 maneras de afectación)
- Sistema de avances y anticipos
- Firmas digitales
- Exportación a PDF y Excel

### 1.2 Ubicación en el Sistema
```
FormMain (panelContenedor)
└── FormDashboardCategorias
    └── FormDashboardContabilidad
        └── FormDashboardCuentasPorPagar
            └── FormMenuCartasSolicitudes
                └── panelAreaTrabajo
                    └── FormSolicitudPago ← ESTE FORMULARIO
```

### 1.3 Características Clave
| Característica | Descripción |
|----------------|-------------|
| **Layout** | Compacto, minimizar scroll vertical |
| **GroupBoxes** | Colapsables con resumen visible |
| **Panel Totales** | Fijo en la parte inferior, siempre visible |
| **Botones** | Barra inferior fija |
| **Autocompletado** | Fideicomiso y Proveedor |
| **Mini-forms** | Para agregar Fideicomiso/Proveedor sin salir |

---

## 2. ARQUITECTURA Y NAVEGACIÓN

### 2.1 Flujo de Navegación
```
FormMenuCartasSolicitudes
├── btnSolicitud (Click)
│   └── Cargar FormSolicitudPago en panelAreaTrabajo
│
├── Modos de Operación:
│   ├── NUEVO: Formulario vacío, genera SP-XXXXXX automático
│   ├── EDITAR: Carga solicitud existente por ID
│   └── CONSULTA: Búsqueda rápida por ID
```

### 2.2 Integración con el Menú Lateral
- El formulario se carga dentro de `panelAreaTrabajo` del `FormMenuCartasSolicitudes`
- El menú lateral permanece visible (contraído)
- Permite cambiar entre formularios sin perder contexto

---

## 3. ESTRUCTURA DE BASE DE DATOS

### 3.1 Tablas de Catálogos

#### TiposNCF (Comprobantes Fiscales DGII)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| TipoNCFID | INT | PK, Identity |
| Codigo | NVARCHAR(3) | B01, E31, etc. |
| CodigoNumerico | NVARCHAR(2) | 01, 31, etc. |
| Serie | CHAR(1) | B o E |
| Nombre | NVARCHAR(100) | Factura de Crédito Fiscal |
| NombreCorto | NVARCHAR(50) | Crédito Fiscal |
| EsElectronico | BIT | 1 = e-NCF (Serie E) |
| LongitudSecuencia | INT | 8 para B, 10 para E |
| LongitudTotal | INT | 11 para B, 13 para E |
| RequiereRNC | BIT | Si requiere RNC del receptor |
| PermiteCredito | BIT | Si permite crédito fiscal |
| Activo | BIT | Estado |

**Valores principales:**
- **Serie B (Tradicionales):** B01, B02, B03, B04, B11, B12, B13, B14, B15, B16, B17
- **Serie E (Electrónicos):** E31, E32, E33, E34, E41, E42, E43, E44, E45, E46, E47

#### Monedas (ISO 4217)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| MonedaID | INT | PK, Identity |
| CodigoISO | CHAR(3) | DOP, USD, EUR |
| Simbolo | NVARCHAR(5) | RD$, US$, € |
| Nombre | NVARCHAR(50) | Peso Dominicano |
| EsLocal | BIT | 1 = DOP |
| Activo | BIT | Estado |

#### TiposPago
| Campo | Tipo | Descripción |
|-------|------|-------------|
| TipoPagoID | INT | PK, Identity |
| Codigo | NVARCHAR(5) | TRF, CHQ, EFE |
| Nombre | NVARCHAR(50) | Transferencia, Cheque |
| RequiereCuenta | BIT | Si requiere cuenta bancaria |
| RequiereReferencia | BIT | Si requiere número referencia |

**Valores:** Transferencia, Cheque, Efectivo, Tarjeta Crédito, Tarjeta Débito, Otro

#### TiposComprobante
| Campo | Tipo | Descripción |
|-------|------|-------------|
| TipoComprobanteID | INT | PK, Identity |
| Codigo | NVARCHAR(5) | NCF, CUB, COT |
| Nombre | NVARCHAR(50) | NCF, Cubicación |
| RequiereNCF | BIT | Si debe ingresar NCF |

**Valores:** NCF, Cubicación, Cotización, Factura Simple, Recibo, Contrato, Otro

#### TiposFideicomiso
| Campo | Tipo | Descripción |
|-------|------|-------------|
| TipoFideicomisoID | INT | PK, Identity |
| Codigo | NVARCHAR(10) | INMOB, ADMIN |
| Nombre | NVARCHAR(100) | Inmobiliario y Garantía |

**Valores:** Inmobiliario y Garantía, De Administración y Pago, De Bajo Costo, Residencial, Plaza Comercial, Otro

#### MetodosConversion
| Campo | Tipo | Descripción |
|-------|------|-------------|
| MetodoConversionID | INT | PK, Identity |
| Codigo | NVARCHAR(10) | DIRECTO, BASE |
| Nombre | NVARCHAR(100) | Conversión Directa Total |

**Valores:** DIRECTO, BASE, SELECT, INDIV, MANUAL

---

### 3.2 Tablas Maestras

#### Fideicomisos
| Campo | Tipo | Descripción |
|-------|------|-------------|
| FideicomisoID | INT | PK, Identity |
| Codigo | NVARCHAR(20) | Código manual por usuario |
| Nombre | NVARCHAR(200) | Nombre del fideicomiso |
| RNC | NVARCHAR(15) | RNC (formato: 000-00000-0) |
| TipoFideicomisoID | INT | FK a TiposFideicomiso |
| Activo | BIT | Estado |
| *Campos de auditoría* | | |

#### Proveedores
| Campo | Tipo | Descripción |
|-------|------|-------------|
| ProveedorID | INT | PK, Identity |
| Nombre | NVARCHAR(200) | Nombre o Razón Social |
| TipoDocumento | CHAR(1) | R = RNC, C = Cédula |
| NumeroDocumento | NVARCHAR(15) | RNC o Cédula |
| Telefono | NVARCHAR(20) | Opcional |
| Email | NVARCHAR(100) | Opcional |
| Activo | BIT | Estado |
| *Campos de auditoría* | | |

---

### 3.3 Tabla Principal: SolicitudesPago

#### Datos Generales
| Campo | Tipo | Descripción |
|-------|------|-------------|
| SolicitudPagoID | INT | PK, Identity |
| NumeroSolicitud | NVARCHAR(20) | SP-000001 (auto) |
| FechaSolicitud | DATE | Fecha de la solicitud |
| FideicomisoID | INT | FK a Fideicomisos |
| ProveedorID | INT | FK a Proveedores |
| TipoPagoID | INT | FK a TiposPago |
| TipoComprobanteID | INT | FK a TiposComprobante |
| NumeroSolicitudExterno | NVARCHAR(50) | Número externo (opcional) |

#### Concepto y Observaciones
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Concepto | NVARCHAR(2000) | Descripción de la factura |
| Observaciones | NVARCHAR(1000) | Notas adicionales |

#### Montos Principales
| Campo | Tipo | Descripción |
|-------|------|-------------|
| SubtotalCalculado | DECIMAL(18,2) | Suma de subtotales |
| Exento | DECIMAL(18,2) | Monto exento de ITBIS |
| DireccionTecnica | DECIMAL(18,2) | Dirección técnica |
| Descuento | DECIMAL(18,2) | Descuento aplicado |
| HorasExtras | DECIMAL(18,2) | Horas extras |
| OtrosImpuestos | DECIMAL(18,2) | Otros impuestos |
| OtrosImpuestosDescripcion | NVARCHAR(200) | Descripción otros |

#### Nota de Crédito
| Campo | Tipo | Descripción |
|-------|------|-------------|
| NotaCreditoMonto | DECIMAL(18,2) | Monto NC |
| NotaCreditoITBIS | DECIMAL(18,2) | ITBIS de la NC |
| NotaCreditoDescripcion | NVARCHAR(500) | Motivo |
| NotaCreditoManera | INT | 1=Afecta Total, 2=Afecta Subtotal |
| NotaCreditoMostrarDetalle | BIT | Mostrar en impresión |

#### Nota de Débito
| Campo | Tipo | Descripción |
|-------|------|-------------|
| NotaDebitoMonto | DECIMAL(18,2) | Monto ND |
| NotaDebitoITBIS | DECIMAL(18,2) | ITBIS de la ND |
| NotaDebitoDescripcion | NVARCHAR(500) | Motivo |
| NotaDebitoManera | INT | 1=Afecta Total, 2=Afecta Subtotal |
| NotaDebitoMostrarDetalle | BIT | Mostrar en impresión |

#### ITBIS
| Campo | Tipo | Descripción |
|-------|------|-------------|
| ITBISPorcentaje | DECIMAL(5,2) | 16, 18, u otro |
| ITBISBase | CHAR(1) | S=Subtotal, D=Dirección Técnica |
| ITBISCalculado | DECIMAL(18,2) | Calculado automáticamente |
| ITBISIngresado | DECIMAL(18,2) | Ingresado manualmente |
| ITBISUsarIngresado | BIT | Usar ingresado vs calculado |

#### Retenciones
| Campo | Tipo | Descripción |
|-------|------|-------------|
| RetencionITBISPorcentaje | DECIMAL(5,2) | 30 o 100 |
| RetencionITBISMonto | DECIMAL(18,2) | Calculado |
| RetencionISRPorcentaje | DECIMAL(5,2) | 2, 10 o 27 |
| RetencionISRMonto | DECIMAL(18,2) | Calculado |
| RetencionSFSMonto | DECIMAL(18,2) | Manual |
| RetencionAFPMonto | DECIMAL(18,2) | Manual |

#### Anticipos y Avances
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Anticipo | DECIMAL(18,2) | Ya pagado (resta del total) |
| AvanceParaPagar | DECIMAL(18,2) | Cuánto se pagará (informativo) |
| TieneAvancePrevio | BIT | Si tiene avance anterior |
| SolicitudPagoOrigenID | INT | FK a solicitud original |

#### Totales Calculados
| Campo | Tipo | Descripción |
|-------|------|-------------|
| TotalFactura | DECIMAL(18,2) | Subtotal + ITBIS + Exento |
| TotalRetencion | DECIMAL(18,2) | Suma de retenciones |
| TotalDescuento | DECIMAL(18,2) | Descuento + NC |
| TotalAPagar | DECIMAL(18,2) | Total final |

#### Moneda y Conversión
| Campo | Tipo | Descripción |
|-------|------|-------------|
| MonedaID | INT | FK a Monedas (default DOP) |
| TasaCambio | DECIMAL(18,6) | Solo si moneda ≠ DOP |
| MetodoConversionID | INT | FK a MetodosConversion |
| MostrarConversionEnFormulario | BIT | Mostrar en pantalla |
| SubtotalConvertido | DECIMAL(18,2) | Para impresión |
| ITBISConvertido | DECIMAL(18,2) | Para impresión |
| TotalFacturaConvertido | DECIMAL(18,2) | Para impresión |
| TotalAPagarConvertido | DECIMAL(18,2) | Para impresión |

#### Firma Digital
| Campo | Tipo | Descripción |
|-------|------|-------------|
| IncluirFirma | BIT | Si incluir firma |
| FirmaUsuarioID | INT | FK a FirmasUsuarios |

#### Estado y Control
| Campo | Tipo | Descripción |
|-------|------|-------------|
| Estado | NVARCHAR(20) | BORRADOR, GUARDADO, IMPRESO, PAGADO, ANULADO |
| FechaImpresion | DATETIME | Última impresión |
| VecesImpreso | INT | Contador |
| UsuarioPropietarioID | INT | Quien creó |
| *Campos de auditoría* | | |

---

### 3.4 Tablas Relacionadas

#### SolicitudesPagoSubtotales
| Campo | Tipo | Descripción |
|-------|------|-------------|
| SubtotalID | INT | PK |
| SolicitudPagoID | INT | FK |
| Orden | INT | Orden de visualización |
| Monto | DECIMAL(18,2) | Monto del subtotal |
| Cantidad | INT | Veces que se repite |
| SubtotalLinea | DECIMAL(18,2) | Monto × Cantidad |

#### SolicitudesPagoComprobantes
| Campo | Tipo | Descripción |
|-------|------|-------------|
| ComprobanteID | INT | PK |
| SolicitudPagoID | INT | FK |
| Orden | INT | Orden de visualización |
| TipoNCFID | INT | FK a TiposNCF |
| NumeroComprobante | NVARCHAR(13) | B0100000306 |

#### SolicitudesPagoAvances
| Campo | Tipo | Descripción |
|-------|------|-------------|
| AvanceID | INT | PK |
| SolicitudPagoID | INT | FK |
| FechaAvance | DATETIME | Cuándo se realizó |
| MontoAvance | DECIMAL(18,2) | Monto avanzado |
| MontoPendiente | DECIMAL(18,2) | Lo que quedó pendiente |
| SolicitudContinuacionID | INT | FK a solicitud continuación |

#### FirmasUsuarios
| Campo | Tipo | Descripción |
|-------|------|-------------|
| FirmaID | INT | PK |
| UsuarioID | INT | FK a Usuarios |
| NombreFirma | NVARCHAR(100) | "Firma Principal" |
| ImagenFirma | VARBINARY(MAX) | PNG con transparencia |
| EsPrincipal | BIT | Firma por defecto |
| Activa | BIT | Estado |

---

## 4. DISEÑO VISUAL DEL FORMULARIO

### 4.1 Layout General (Pantalla 1920x1080)

```
┌──────────────────────────────────────────────────────────────────────────────────────────────────────┐
│ [← Volver]                    SOLICITUD DE PAGO                         ID: SP-000001  [🔍 Buscar]  │
├──────────────────────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                                      │
│ ┌─ DATOS GENERALES ──────────────────────────────────────────────────────────────────────────────┐  │
│ │ Fecha: [17/01/2026 📅]  Tipo Pago: [Transferencia ▼]  Moneda: [DOP ▼]  Tasa: [______] N°Ext:[__]│  │
│ │                                                                                                 │  │
│ │ Fideicomiso: Cód:[___][+] [_________________________________▼] RNC: 000-00000-0                │  │
│ │ Proveedor:   RNC:[___-______-_][+] [________________________▼] Tel: 809-000-0000               │  │
│ └─────────────────────────────────────────────────────────────────────────────────────────────────┘  │
│                                                                                                      │
│ ┌─ COMPROBANTES ─────────────────────────────┐ ┌─ CONCEPTO ─────────────────────────────────────┐   │
│ │ Tipo: [NCF ▼] [B01▼][___________][+Agregar]│ │ ┌─────────────────────────────────────────────┐│   │
│ │ ┌───────────────────────────────────────┐  │ │ │Pago correspondiente a factura por          ││   │
│ │ │ 1. B0100000306 [×]                    │  │ │ │servicios de consultoría enero 2026...      ││   │
│ │ │ 2. B0100000307 [×]                    │  │ │ │                                            ││   │
│ │ └───────────────────────────────────────┘  │ │ └─────────────────────────────────────────────┘│   │
│ └────────────────────────────────────────────┘ │ 0/2000 caracteres                              │   │
│                                                └─────────────────────────────────────────────────┘   │
│                                                                                                      │
│ ┌─ MONTOS ─────────────────────────────┐ ┌─ OTROS MONTOS ──────────┐ ┌─ NOTAS CR/DB ────────────┐  │
│ │ Subtotales:         [+ Agregar]      │ │ Exento:    [_________]  │ │ Nota Crédito: [____][⚙] │  │
│ │ ┌──────────────────────────────────┐ │ │ Dir.Téc:   [_________]  │ │ Nota Débito:  [____][⚙] │  │
│ │ │ 1. RD$ 50,000 x1 = 50,000 [×]   │ │ │ Descuento: [_________]  │ │                          │  │
│ │ │ 2. RD$ 25,000 x2 = 50,000 [×]   │ │ │ H.Extras:  [_________]  │ │ Anticipo:     [________] │  │
│ │ └──────────────────────────────────┘ │ │ Otros Imp: [_____][⚙]  │ │ Avance Pagar: [________] │  │
│ │ SUBTOTAL:        RD$ 100,000.00      │ │                         │ │                          │  │
│ └──────────────────────────────────────┘ └─────────────────────────┘ └──────────────────────────┘  │
│                                                                                                      │
│ ┌─ IMPUESTOS Y RETENCIONES ──────────────────────────────────────────────────────────────────────┐  │
│ │ ITBIS: [18%▼] Base:[○Subt ○Dir.Téc]  Calc: RD$18,000  Manual:[________] ⚠Dif: RD$0            │  │
│ │                                                                                                 │  │
│ │ Ret.ITBIS:[30%▼]=RD$5,400   Ret.ISR:[2%▼]=RD$2,000   Ret.SFS:[______]   Ret.AFP:[______]      │  │
│ └─────────────────────────────────────────────────────────────────────────────────────────────────┘  │
│                                                                                                      │
│ ┌─ OBSERVACIONES ────────────────────────────────────────────────────────────────────────────────┐  │
│ │ [________________________________________________________________________________] 0/1000       │  │
│ │ ☑ Incluir firma: [Mi Firma ▼]    ☐ Mostrar conversión    [⚙ Config. Conversión]              │  │
│ └─────────────────────────────────────────────────────────────────────────────────────────────────┘  │
│                                                                                                      │
╠══════════════════════════════════════════════════════════════════════════════════════════════════════╣
║ SUBTOTAL: RD$100,000 │ ITBIS: RD$18,000 │ EXENTO: RD$0 ║ TOTAL FACTURA: RD$118,000                  ║
║ RET.ITBIS: RD$5,400  │ RET.ISR: RD$2,000│ OTRAS: RD$0  ║ TOTAL RETENCIÓN: RD$7,400                  ║
║──────────────────────────────────────────────────────────────────────────────────────────────────────║
║                              ▶▶▶  TOTAL A PAGAR:  RD$ 110,600.00  ◀◀◀                               ║
╠══════════════════════════════════════════════════════════════════════════════════════════════════════╣
│  [🧹 Limpiar]    [💾 Guardar]    [🖨️ Imprimir]    [📄 PDF]    [📊 Excel]                            │
└──────────────────────────────────────────────────────────────────────────────────────────────────────┘
```

### 4.2 Estrategias de Compactación

| Elemento | Estrategia |
|----------|------------|
| **Datos Generales** | Todo en 2 líneas, campos inline |
| **Comprobantes + Concepto** | Side-by-side (2 columnas) |
| **Montos** | 3 columnas: Subtotales │ Otros Montos │ Notas CR/DB |
| **Impuestos** | Todo en 2 líneas horizontales |
| **Observaciones** | 1 línea con opciones inline |
| **Panel de Totales** | FIJO en la parte inferior |
| **Botones** | Barra inferior fija |

### 4.3 GroupBoxes Colapsables

```
Estado Expandido:
┌─ MONTOS [▼] ─────────────────────────┐
│ (contenido visible)                   │
└───────────────────────────────────────┘

Estado Colapsado (muestra resumen):
┌─ MONTOS [▶] ─── Subtotal: RD$100,000.00 ──┐
└───────────────────────────────────────────┘
```

---

## 5. SECCIONES DEL FORMULARIO

### 5.1 Encabezado
- Botón "← Volver" (regresa al menú)
- Título "SOLICITUD DE PAGO"
- ID: SP-XXXXXX (auto-generado)
- Botón "🔍 Buscar" (carga solicitud existente)

### 5.2 Datos Generales
| Control | Tipo | Descripción |
|---------|------|-------------|
| dtpFecha | DateTimePicker | Fecha de solicitud |
| cboTipoPago | ComboBox | Transferencia, Cheque, etc. |
| cboMoneda | ComboBox | DOP, USD, EUR, etc. |
| txtTasaCambio | TextBox | Solo si moneda ≠ DOP |
| txtNumeroExterno | TextBox | Número solicitud externo |
| txtCodigoFideicomiso | TextBox | Código manual |
| btnAgregarFideicomiso | Button | [+] Abre mini-form |
| cboFideicomiso | ComboBox | Con autocompletado |
| lblRNCFideicomiso | Label | Se llena automáticamente |
| txtRNCProveedor | MaskedTextBox | Formato: 000-00000-0 |
| btnAgregarProveedor | Button | [+] Abre mini-form |
| cboProveedor | ComboBox | Con autocompletado |
| lblTelefonoProveedor | Label | Se llena automáticamente |

### 5.3 Comprobantes
| Control | Tipo | Descripción |
|---------|------|-------------|
| cboTipoComprobante | ComboBox | NCF, Cubicación, etc. |
| cboTipoNCF | ComboBox | B01, B02, E31, etc. |
| txtNumeroNCF | MaskedTextBox | Secuencia del NCF |
| btnAgregarComprobante | Button | [+ Agregar] |
| lstComprobantes | ListBox/DataGridView | Lista de NCF agregados |
| btnEliminarComprobante | Button | [×] por cada item |

**Límite:** Máximo 10 comprobantes por solicitud (configurable)

### 5.4 Concepto
| Control | Tipo | Descripción |
|---------|------|-------------|
| txtConcepto | TextBox | Multiline, 2000 caracteres máx |
| lblContadorConcepto | Label | "0/2000" |

### 5.5 Montos (Subtotales)
| Control | Tipo | Descripción |
|---------|------|-------------|
| btnAgregarSubtotal | Button | [+ Agregar] |
| dgvSubtotales | DataGridView | Monto, Cantidad, Total |
| lblSubtotalTotal | Label | Suma de subtotales |

**Columnas DataGridView:**
- Orden (auto)
- Monto (editable)
- Cantidad (editable, default 1)
- Subtotal Línea (calculado: Monto × Cantidad)
- Botón eliminar [×]

**Límite:** Máximo 10 subtotales (configurable)

### 5.6 Otros Montos
| Control | Tipo | Descripción |
|---------|------|-------------|
| txtExento | TextBox | Monto exento de ITBIS |
| txtDireccionTecnica | TextBox | Dirección técnica |
| txtDescuento | TextBox | Descuento |
| txtHorasExtras | TextBox | Horas extras |
| txtOtrosImpuestos | TextBox | Otros impuestos |
| btnConfigOtrosImpuestos | Button | [⚙] Abre configuración |

### 5.7 Notas de Crédito/Débito
| Control | Tipo | Descripción |
|---------|------|-------------|
| txtNotaCredito | TextBox | Monto |
| btnConfigNotaCredito | Button | [⚙] Abre mini-form |
| txtNotaDebito | TextBox | Monto |
| btnConfigNotaDebito | Button | [⚙] Abre mini-form |
| txtAnticipo | TextBox | Monto ya pagado |
| txtAvancePagar | TextBox | Monto a pagar (informativo) |

### 5.8 Impuestos y Retenciones
| Control | Tipo | Descripción |
|---------|------|-------------|
| cboITBISPorcentaje | ComboBox | 0%, 16%, 18% |
| rbITBISBaseSubtotal | RadioButton | Base = Subtotal |
| rbITBISBaseDirTec | RadioButton | Base = Dir. Técnica |
| lblITBISCalculado | Label | Calculado automáticamente |
| txtITBISManual | TextBox | Ingresado manualmente |
| lblITBISDiferencia | Label | Muestra diferencia |
| cboRetencionITBIS | ComboBox | 0%, 30%, 100% |
| lblRetencionITBISMonto | Label | Calculado |
| cboRetencionISR | ComboBox | 0%, 2%, 10%, 27% |
| lblRetencionISRMonto | Label | Calculado |
| txtRetencionSFS | TextBox | Manual |
| txtRetencionAFP | TextBox | Manual |

### 5.9 Observaciones
| Control | Tipo | Descripción |
|---------|------|-------------|
| txtObservaciones | TextBox | Multiline, 1000 caracteres |
| lblContadorObservaciones | Label | "0/1000" |
| chkIncluirFirma | CheckBox | ☑ Incluir firma digital |
| cboFirma | ComboBox | Lista de firmas del usuario |
| chkMostrarConversion | CheckBox | ☐ Mostrar conversión |
| btnConfigConversion | Button | [⚙ Config. Conversión] |

### 5.10 Panel de Totales (FIJO)
| Control | Tipo | Descripción |
|---------|------|-------------|
| lblSubtotal | Label | Suma subtotales |
| lblITBIS | Label | ITBIS calculado |
| lblExento | Label | Exento |
| lblTotalFactura | Label | Subtotal + ITBIS + Exento |
| lblRetencionITBIS | Label | Retención ITBIS |
| lblRetencionISR | Label | Retención ISR |
| lblOtrasRetenciones | Label | SFS + AFP |
| lblTotalRetencion | Label | Suma retenciones |
| lblTotalAPagar | Label | **TOTAL FINAL** (destacado) |

### 5.11 Barra de Botones (FIJA)
| Control | Texto | Acción |
|---------|-------|--------|
| btnLimpiar | 🧹 Limpiar | Limpia formulario |
| btnGuardar | 💾 Guardar | Guarda en BD |
| btnImprimir | 🖨️ Imprimir | Envía a impresora |
| btnPDF | 📄 PDF | Genera PDF |
| btnExcel | 📊 Excel | Genera Excel |

---

## 6. MINI-FORMS MODALES

### 6.1 Mini-Form: Agregar Fideicomiso

```
┌─────────────────────────────────────────────────┐
│ ✕                NUEVO FIDEICOMISO              │
├─────────────────────────────────────────────────┤
│                                                 │
│  Código*:   [________]                          │
│                                                 │
│  Nombre*:   [_____________________________]     │
│                                                 │
│  RNC*:      [___-______-_]                      │
│                                                 │
│  Tipo:      [Inmobiliario y Garantía      ▼]   │
│             (Opcional)                          │
│                                                 │
│  ☑ Activo                                       │
│                                                 │
├─────────────────────────────────────────────────┤
│           [Cancelar]    [💾 Guardar]            │
└─────────────────────────────────────────────────┘
```

**Comportamiento:**
- Al guardar, el fideicomiso queda seleccionado en el combo principal
- Validación de RNC único
- Validación de código único

### 6.2 Mini-Form: Agregar Proveedor

```
┌─────────────────────────────────────────────────┐
│ ✕                 NUEVO PROVEEDOR               │
├─────────────────────────────────────────────────┤
│                                                 │
│  Nombre/Razón Social*: [____________________]   │
│                                                 │
│  Tipo Documento:  ○ RNC (Empresa)               │
│                   ○ Cédula (Persona)            │
│                                                 │
│  RNC/Cédula*:     [___-______-_]                │
│                                                 │
│  Teléfono:        [_______________] (Opcional)  │
│                                                 │
│  Email:           [_______________] (Opcional)  │
│                                                 │
│  ☑ Activo                                       │
│                                                 │
├─────────────────────────────────────────────────┤
│           [Cancelar]    [💾 Guardar]            │
└─────────────────────────────────────────────────┘
```

**Comportamiento:**
- Máscara cambia según tipo documento (RNC o Cédula)
- Validación de documento único
- Al guardar, proveedor queda seleccionado

### 6.3 Mini-Form: Configurar Nota de Crédito

```
┌─────────────────────────────────────────────────┐
│ ✕              NOTA DE CRÉDITO                  │
├─────────────────────────────────────────────────┤
│                                                 │
│  Monto:           [RD$ __________]              │
│                                                 │
│  ITBIS:           RD$ 0.00 (calculado)          │
│                                                 │
│  Manera de afectación:                          │
│  ○ Afecta el Total a Pagar (después de ITBIS)  │
│  ○ Afecta el Subtotal (antes del ITBIS)        │
│                                                 │
│  Descripción:     [________________________]    │
│                                                 │
│  ☐ Mostrar detalle en impresión                │
│                                                 │
├─────────────────────────────────────────────────┤
│           [Cancelar]    [✓ Aplicar]             │
└─────────────────────────────────────────────────┘
```

### 6.4 Mini-Form: Configurar Nota de Débito
(Estructura idéntica a Nota de Crédito)

### 6.5 Mini-Form: Configurar Conversión de Moneda

```
┌─────────────────────────────────────────────────────────────┐
│ ✕            CONFIGURACIÓN DE CONVERSIÓN                    │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Moneda Destino:  USD - Dólar Estadounidense                │
│  Tasa de Cambio:  59.50                                     │
│                                                             │
│  Método de Conversión:                                      │
│  ─────────────────────                                      │
│                                                             │
│  ○ Método 1: Conversión Directa Total                       │
│    Todos los montos × Tasa                                  │
│                                                             │
│  ○ Método 2: Conversión Base + Recálculo                    │
│    Solo Subtotal × Tasa, ITBIS y retenciones recalculados   │
│                                                             │
│  ○ Método 3: Conversión Selectiva (Personalizada)           │
│    Elegir qué campos convertir directamente                 │
│                                                             │
│  ○ Método 4: Conversión Individual de Subtotales            │
│    Cada subtotal se convierte antes de sumar                │
│                                                             │
│  ○ Método 5: Conversión Manual/Mixta                        │
│    Ingresar valores convertidos manualmente                 │
│                                                             │
│   ───────────────────────────────────────────────────────── │
│   Subtotales (si Método 4):                                 │
│   ○ Convertir cada uno y luego sumar                        │
│    ○ Sumar en DOP y luego convertir el total                │
├─────────────────────────────────────────────────────────────┤
│              [Cancelar]    [✓ Aplicar Configuración]        │
└─────────────────────────────────────────────────────────────┘
```

---

## 7. LÓGICA DE CÁLCULOS

### 7.1 Fórmulas de Cálculo

| Código | Cálculo | Fórmula |
|--------|---------|---------|
| C1 | ITBIS | (Subtotal o Dir.Técnica) × %ITBIS |
| C2 | Retención ITBIS | ITBIS × (30% o 100%) |
| C3 | Retención ISR | Subtotal × (2%, 10% o 27%) |
| C4 | Retención SFS | Ingreso manual |
| C5 | Retención AFP | Ingreso manual |
| C6 | Total Factura | Subtotal + ITBIS + Exento |
| C7 | Total Retención | Ret.ITBIS + Ret.ISR + SFS + AFP |
| C8 | Total Descuento | Descuento + Nota Crédito (según manera) |
| C9 | Total a Pagar | Total Factura - Total Retención - Total Descuento - Anticipo |

### 7.2 Notas de Crédito/Débito - Dos Maneras

**Manera 1: Afecta Total a Pagar**
```
Total a Pagar = Total Factura - Retenciones - Nota Crédito - Anticipo
```
La NC se resta al final, después de calcular ITBIS y retenciones.

**Manera 2: Afecta Subtotal**
```
Subtotal Ajustado = Subtotal - Nota Crédito
ITBIS = Subtotal Ajustado × %ITBIS
Total Factura = Subtotal Ajustado + ITBIS + Exento
```
La NC se resta del subtotal, afectando el cálculo de ITBIS.

### 7.3 Orden de Cálculo (importante)
1. Sumar subtotales
2. Aplicar Nota Crédito/Débito si Manera 2
3. Calcular ITBIS
4. Calcular retenciones
5. Calcular Total Factura
6. Aplicar Nota Crédito/Débito si Manera 1
7. Restar Anticipo
8. Calcular Total a Pagar

---

## 8. CONVERSIÓN DE MONEDA

### 8.1 Método 1: Conversión Directa Total
```csharp
SubtotalConvertido = Subtotal * TasaCambio;
ITBISConvertido = ITBIS * TasaCambio;
RetencionesConvertidas = Retenciones * TasaCambio;
TotalConvertido = Total * TasaCambio;
```

### 8.2 Método 2: Conversión Base + Recálculo
```csharp
SubtotalConvertido = Subtotal * TasaCambio;
ITBISConvertido = SubtotalConvertido * PorcentajeITBIS;
RetencionesConvertidas = Recalcular(SubtotalConvertido);
TotalConvertido = SubtotalConvertido + ITBISConvertido - RetencionesConvertidas;
```

### 8.3 Método 3: Conversión Selectiva
El usuario elige qué campos convertir directamente y cuáles recalcular.

### 8.4 Método 4: Conversión Individual de Subtotales
```csharp
// Opción A: Convertir cada uno y sumar
foreach (subtotal in Subtotales)
    SubtotalConvertido += subtotal.Monto * TasaCambio;

// Opción B: Sumar y luego convertir
SubtotalTotal = Subtotales.Sum(s => s.Monto);
SubtotalConvertido = SubtotalTotal * TasaCambio;
```

### 8.5 Método 5: Conversión Manual/Mixta
El usuario ingresa valores convertidos manualmente. El sistema guarda ambos valores (calculado e ingresado) para detectar diferencias.

---

## 9. VALIDACIONES

### 9.1 Validación de RNC (9 dígitos)
```
Formato: 000-00000-0
Algoritmo: Módulo 11
Regex: ^\d{3}-\d{5}-\d{1}$
```

### 9.2 Validación de Cédula (11 dígitos)
```
Formato: 000-0000000-0
Algoritmo: Luhn modificado
Regex: ^\d{3}-\d{7}-\d{1}$
```

### 9.3 Validación de NCF Tradicional (Serie B)
```
Formato: B0100000001
Longitud: 11 caracteres
Regex: ^B(0[1-4]|1[1-7])\d{8}$
```

### 9.4 Validación de e-NCF (Serie E)
```
Formato: E310000000001
Longitud: 13 caracteres
Regex: ^E(3[1-4]|4[1-7])\d{10}$
```

### 9.5 Validaciones de Negocio
- Fideicomiso es obligatorio
- Proveedor es obligatorio
- Al menos un subtotal > 0
- Concepto es obligatorio (mínimo 10 caracteres)
- NCF no puede repetirse en el sistema
- Fecha no puede ser futura

---

## 10. SISTEMA DE FIRMAS DIGITALES

### 10.1 Requisitos
- Formato: PNG con fondo transparente
- Tamaño recomendado: 300x100 píxeles
- Solo el dueño puede usar su firma
- Una firma principal por usuario

### 10.2 Tabla FirmasUsuarios
Ver sección 3.4

### 10.3 Flujo
1. Usuario sube imagen de firma en Configuración
2. Al crear solicitud, marca "Incluir firma"
3. Selecciona qué firma usar
4. Al imprimir/exportar, se incluye la firma

---

## 11. EXPORTACIÓN E IMPRESIÓN

### 11.1 Formato de Impresión
- Layout inteligente según cantidad de datos
- Logo de MOFIS-ERP
- Datos del fideicomiso y proveedor
- Lista de comprobantes
- Desglose de montos
- Totales destacados
- Firma digital (si se incluye)
- Pie de página con fecha/hora y usuario

### 11.2 Exportación PDF
- Mismo formato que impresión
- Nombre archivo: `SP-XXXXXX_FECHA.pdf`

### 11.3 Exportación Excel
- Hoja 1: Datos generales
- Hoja 2: Detalle de subtotales
- Hoja 3: Detalle de comprobantes
- Nombre archivo: `SP-XXXXXX_FECHA.xlsx`

---

## 12. CONFIGURACIONES DEL MÓDULO

### 12.1 Tabla ConfiguracionModuloCXP

| Clave | Valor Default | Descripción |
|-------|---------------|-------------|
| LIMITE_SUBTOTALES | 10 | Máximo subtotales por solicitud |
| LIMITE_COMPROBANTES | 10 | Máximo NCF por solicitud |
| ITBIS_DEFAULT | 18 | Porcentaje ITBIS por defecto |
| MONEDA_DEFAULT | DOP | Moneda por defecto |
| CONVERSION_DEFAULT | 1 | Método conversión por defecto |
| FORMATO_FECHA | dd/MM/yyyy | Formato de fecha |
| DECIMALES_MONEDA | 2 | Decimales para montos |
| DECIMALES_TASA | 6 | Decimales para tasa de cambio |

---

## 13. PLAN DE IMPLEMENTACIÓN POR FASES

### FASE 1: Formulario Base (2-3 días)
- [ ] Crear FormSolicitudPago.cs en el diseñador
- [ ] Agregar todos los controles según sección 14
- [ ] Configurar propiedades básicas
- [ ] Conectar con FormMenuCartasSolicitudes

### FASE 2: Datos Generales (1-2 días)
- [ ] Implementar carga de combos (TiposPago, Monedas)
- [ ] Implementar autocompletado Fideicomiso
- [ ] Implementar autocompletado Proveedor
- [ ] Crear mini-form Agregar Fideicomiso
- [ ] Crear mini-form Agregar Proveedor

### FASE 3: Comprobantes y Concepto (1 día)
- [ ] Implementar carga de TiposNCF
- [ ] Implementar agregar/eliminar comprobantes
- [ ] Validar formato NCF
- [ ] Contador de caracteres concepto

### FASE 4: Montos y Subtotales (1-2 días)
- [ ] Implementar DataGridView de subtotales
- [ ] Agregar/editar/eliminar subtotales
- [ ] Cálculo automático de suma
- [ ] Implementar otros montos

### FASE 5: Impuestos y Retenciones (1-2 días)
- [ ] Cálculo automático ITBIS
- [ ] Selección base ITBIS
- [ ] Cálculo retenciones
- [ ] Diferencia ITBIS manual vs calculado

### FASE 6: Notas CR/DB y Avances (1 día)
- [ ] Crear mini-form Nota Crédito
- [ ] Crear mini-form Nota Débito
- [ ] Implementar las 2 maneras de afectación
- [ ] Sistema de anticipos y avances

### FASE 7: Panel de Totales (0.5 días)
- [ ] Panel fijo inferior
- [ ] Actualización en tiempo real
- [ ] Formato de números

### FASE 8: Guardado y Carga (2 días)
- [ ] Guardar en base de datos
- [ ] Cargar solicitud existente
- [ ] Búsqueda por ID
- [ ] Auditoría

### FASE 9: Conversión de Moneda (1-2 días)
- [ ] Crear mini-form configuración
- [ ] Implementar 5 métodos de conversión
- [ ] Mostrar/ocultar conversión

### FASE 10: Firma Digital (1 día)
- [ ] Subir firma en configuración
- [ ] Seleccionar firma en solicitud
- [ ] Incluir en impresión/exportación

### FASE 11: Exportación e Impresión (2 días)
- [ ] Diseñar formato de impresión
- [ ] Generar PDF
- [ ] Generar Excel
- [ ] Vista previa

### FASE 12: Pruebas y Ajustes (2-3 días)
- [ ] Pruebas de validación
- [ ] Pruebas de cálculos
- [ ] Pruebas de exportación
- [ ] Ajustes de UI/UX

**Tiempo estimado total: 15-20 días**

---

## 14. CONTROLES A CREAR EN EL DISEÑADOR

### 14.1 Panel Principal
| Control | Name | Tipo | Dock |
|---------|------|------|------|
| Panel Encabezado | panelEncabezado | Panel | Top |
| Panel Contenido | panelContenido | Panel | Fill |
| Panel Totales | panelTotales | Panel | Bottom |
| Panel Botones | panelBotones | Panel | Bottom |

### 14.2 Panel Encabezado
| Control | Name | Tipo |
|---------|------|------|
| Botón Volver | btnVolver | Button |
| Título | lblTitulo | Label |
| ID Solicitud | lblNumeroSolicitud | Label |
| Botón Buscar | btnBuscar | Button |

### 14.3 GroupBox Datos Generales
| Control | Name | Tipo | Comentario |
|---------|------|------|------------|
| Fecha | dtpFecha | DateTimePicker | |
| Tipo Pago | cboTipoPago | ComboBox | |
| Moneda | cboMoneda | ComboBox | |
| Tasa Cambio | txtTasaCambio | TextBox | Visible si moneda ≠ DOP |
| N° Externo | txtNumeroExterno | TextBox | |
| Código Fideicomiso | txtCodigoFideicomiso | TextBox | |
| Agregar Fideicomiso | btnAgregarFideicomiso | Button | [+] |
| Combo Fideicomiso | cboFideicomiso | ComboBox | Autocompletado |
| RNC Fideicomiso | lblRNCFideicomiso | Label | Auto |
| RNC Proveedor | txtRNCProveedor | MaskedTextBox | 000-00000-0 |
| Agregar Proveedor | btnAgregarProveedor | Button | [+] |
| Combo Proveedor | cboProveedor | ComboBox | Autocompletado |
| Teléfono Proveedor | lblTelefonoProveedor | Label | Auto |

### 14.4 GroupBox Comprobantes
| Control | Name | Tipo |
|---------|------|------|
| Tipo Comprobante | cboTipoComprobante | ComboBox |
| Tipo NCF | cboTipoNCF | ComboBox |
| Número NCF | txtNumeroNCF | MaskedTextBox |
| Agregar | btnAgregarComprobante | Button |
| Lista | lstComprobantes | ListBox |

### 14.5 GroupBox Concepto
| Control | Name | Tipo |
|---------|------|------|
| Concepto | txtConcepto | TextBox (Multiline) |
| Contador | lblContadorConcepto | Label |

### 14.6 GroupBox Montos
| Control | Name | Tipo |
|---------|------|------|
| Agregar Subtotal | btnAgregarSubtotal | Button |
| Grid Subtotales | dgvSubtotales | DataGridView |
| Total Subtotales | lblSubtotalTotal | Label |

### 14.7 GroupBox Otros Montos
| Control | Name | Tipo |
|---------|------|------|
| Exento | txtExento | TextBox |
| Dir. Técnica | txtDireccionTecnica | TextBox |
| Descuento | txtDescuento | TextBox |
| Horas Extras | txtHorasExtras | TextBox |
| Otros Impuestos | txtOtrosImpuestos | TextBox |
| Config Otros | btnConfigOtrosImpuestos | Button |

### 14.8 GroupBox Notas CR/DB
| Control | Name | Tipo |
|---------|------|------|
| Nota Crédito | txtNotaCredito | TextBox |
| Config NC | btnConfigNotaCredito | Button |
| Nota Débito | txtNotaDebito | TextBox |
| Config ND | btnConfigNotaDebito | Button |
| Anticipo | txtAnticipo | TextBox |
| Avance Pagar | txtAvancePagar | TextBox |

### 14.9 GroupBox Impuestos
| Control | Name | Tipo |
|---------|------|------|
| ITBIS % | cboITBISPorcentaje | ComboBox |
| Base Subtotal | rbITBISBaseSubtotal | RadioButton |
| Base Dir.Téc | rbITBISBaseDirTec | RadioButton |
| ITBIS Calculado | lblITBISCalculado | Label |
| ITBIS Manual | txtITBISManual | TextBox |
| Diferencia | lblITBISDiferencia | Label |
| Ret. ITBIS % | cboRetencionITBIS | ComboBox |
| Ret. ITBIS Monto | lblRetencionITBISMonto | Label |
| Ret. ISR % | cboRetencionISR | ComboBox |
| Ret. ISR Monto | lblRetencionISRMonto | Label |
| Ret. SFS | txtRetencionSFS | TextBox |
| Ret. AFP | txtRetencionAFP | TextBox |

### 14.10 GroupBox Observaciones
| Control | Name | Tipo |
|---------|------|------|
| Observaciones | txtObservaciones | TextBox (Multiline) |
| Contador | lblContadorObservaciones | Label |
| Incluir Firma | chkIncluirFirma | CheckBox |
| Combo Firma | cboFirma | ComboBox |
| Mostrar Conversión | chkMostrarConversion | CheckBox |
| Config Conversión | btnConfigConversion | Button |

### 14.11 Panel Totales
| Control | Name | Tipo | Comentario |
|---------|------|------|------------|
| Subtotal | lblTotalSubtotal | Label | |
| ITBIS | lblTotalITBIS | Label | |
| Exento | lblTotalExento | Label | |
| Total Factura | lblTotalFactura | Label | Destacado |
| Ret. ITBIS | lblTotalRetITBIS | Label | |
| Ret. ISR | lblTotalRetISR | Label | |
| Otras Ret. | lblTotalOtrasRet | Label | |
| Total Retención | lblTotalRetencion | Label | |
| **Total a Pagar** | lblTotalAPagar | Label | **MUY destacado** |

### 14.12 Panel Botones
| Control | Name | Tipo | Texto |
|---------|------|------|-------|
| Limpiar | btnLimpiar | Button | 🧹 Limpiar |
| Guardar | btnGuardar | Button | 💾 Guardar |
| Imprimir | btnImprimir | Button | 🖨️ Imprimir |
| PDF | btnPDF | Button | 📄 PDF |
| Excel | btnExcel | Button | 📊 Excel |

---

## 📝 NOTAS FINALES

### Consideraciones Importantes
1. El formulario debe ser **responsivo** - ajustarse a diferentes resoluciones
2. Los **GroupBoxes colapsables** son clave para minimizar scroll
3. El **Panel de Totales** siempre debe estar visible
4. La **auditoría** es obligatoria para todas las operaciones
5. Implementar **guardado en memoria temporal** para recuperar datos no guardados

### Archivos a Crear
```
Forms/
└── Contabilidad/
    └── CuentasPorPagar/
        └── CartasSolicitudes/
            ├── FormMenuCartasSolicitudes.cs (YA EXISTE)
            ├── FormSolicitudPago.cs ← PRINCIPAL
            ├── FormAgregarFideicomiso.cs ← Mini-form
            ├── FormAgregarProveedor.cs ← Mini-form
            ├── FormConfigNotaCredito.cs ← Mini-form
            ├── FormConfigNotaDebito.cs ← Mini-form
            └── FormConfigConversion.cs ← Mini-form
```

### Clases de Soporte
```
Classes/
└── CuentasPorPagar/
    ├── SolicitudPago.cs (POCO)
    ├── SolicitudPagoSubtotal.cs (POCO)
    ├── SolicitudPagoComprobante.cs (POCO)
    ├── SolicitudPagoAvance.cs (POCO)
    ├── Fideicomiso.cs (POCO)
    ├── Proveedor.cs (POCO)
    ├── FirmaUsuario.cs (POCO)
    └── CalculadoraSolicitud.cs (Lógica de cálculos)
```

---

**Documento generado para MOFIS-ERP**  
**Versión 1.0 - 19/01/2026**
