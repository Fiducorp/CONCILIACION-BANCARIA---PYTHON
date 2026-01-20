# 📋 RESUMEN DE IMPLEMENTACIÓN: FormSolicitudPago.cs

## PROYECTO: MOFIS-ERP (Fiducorp - República Dominicana)

**Fecha de actualización:** 20/01/2026  
**Desarrollador:** Cysero  
**Tecnología:** C# Windows Forms, SQL Server, Visual Studio  
**Estado:** FASE 1 COMPLETADA - Diseño Visual

---

## 📌 ÍNDICE

1. [Contexto del Proyecto](#1-contexto-del-proyecto)
2. [Ubicación del Formulario](#2-ubicación-del-formulario)
3. [Estructura Visual Implementada](#3-estructura-visual-implementada)
4. [Controles Creados por Sección](#4-controles-creados-por-sección)
5. [Propiedades de Estilo Utilizadas](#5-propiedades-de-estilo-utilizadas)
6. [Código Base Implementado](#6-código-base-implementado)
7. [Funcionalidades Pendientes](#7-funcionalidades-pendientes)
8. [Base de Datos Disponible](#8-base-de-datos-disponible)
9. [Mini-Forms Pendientes](#9-mini-forms-pendientes)
10. [Notas Importantes](#10-notas-importantes)

---

## 1. CONTEXTO DEL PROYECTO

### 1.1 Descripción
FormSolicitudPago es el formulario principal del módulo **Cuentas por Pagar** dentro de la categoría **Contabilidad**. Permite registrar solicitudes de pago a proveedores con soporte para:

- Múltiples subtotales por solicitud (hasta 10, configurable)
- Múltiples comprobantes NCF (hasta 10, configurable)
- Cálculo automático de ITBIS y retenciones
- Conversión multi-moneda (5 métodos)
- Notas de Crédito y Débito (2 maneras de afectación)
- Sistema de avances y anticipos
- Firmas digitales
- Exportación a PDF y Excel

### 1.2 Stack Tecnológico
| Componente | Tecnología |
|------------|------------|
| Lenguaje | C# |
| Framework | .NET Windows Forms |
| Base de Datos | SQL Server |
| IDE | Visual Studio |
| Control de Versiones | GitHub |

---

## 2. UBICACIÓN DEL FORMULARIO

### 2.1 Ruta del Archivo
```
Forms/Contabilidad/CuentasPorPagar/CartasSolicitudes/FormSolicitudPago.cs
```

### 2.2 Flujo de Navegación
```
FormMain (panelContenedor)
└── FormDashboardCategorias
    └── FormDashboardContabilidad
        └── FormDashboardCuentasPorPagar
            └── FormMenuCartasSolicitudes
                └── panelAreaTrabajo
                    └── FormSolicitudPago ← ESTE FORMULARIO
```

### 2.3 Carga del Formulario
El formulario se carga dentro de `panelAreaTrabajo` de `FormMenuCartasSolicitudes` cuando el usuario hace click en el botón "Solicitud de Pago" del menú lateral.

---

## 3. ESTRUCTURA VISUAL IMPLEMENTADA

### 3.1 Jerarquía de Paneles Principales

```
FormSolicitudPago (FormBorderStyle: None, Dock: Fill via código)
│
├── panelEncabezado (Dock: Top, Height: 60, BackColor: White)
│   ├── btnVolver (Dock: Left)
│   ├── lblTituloFormulario (Dock: Fill)
│   └── panelDerecho (Dock: Right)
│       ├── lblNumeroSolicitud (Dock: Left)
│       └── btnBuscar (Dock: Right)
│
├── panelContenido (Dock: Fill, BackColor: 245,247,250, AutoScroll: True)
│   │
│   ├── tableLayoutPrincipal (Dock: Top, Height: ~450)
│   │   │
│   │   ├── Fila 0: gbDatosGenerales (ColumnSpan: 3)
│   │   │
│   │   ├── Fila 1: gbComprobantes (Col 0) 
│   │   │          + gbConcepto (Col 1-2, ColumnSpan: 2)
│   │   │
│   │   └── Fila 2: gbMontos (Col 0) 
│   │              + gbOtrosMontos (Col 1) 
│   │              + gbNotasCRDB (Col 2)
│   │
│   └── panelFilaImpuestosObs (Dock: Top, debajo de tableLayoutPrincipal)
│       │
│       ├── panelImpuestosContenedor (Dock: Fill)
│       │   └── gbImpuestos
│       │       ├── Controles ITBIS (organizados manualmente)
│       │       ├── panelBaseITBIS (RadioButtons)
│       │       └── tableRetenciones
│       │
│       └── panelObservacionesContenedor (Dock: Right, Width: 280)
│           └── gbObservaciones
│
├── panelTotales (Dock: Bottom, Height: 130, BackColor: 24,24,32)
│   └── tableTotales (4 columnas × 3 filas)
│
└── panelBotones (Dock: Bottom, Height: 55, BackColor: White)
    └── tableBotones (botones centrados con columnas 50% a los lados)
```

### 3.2 Distribución de tableLayoutPrincipal

| FILA | COLUMNA 0 | COLUMNA 1 | COLUMNA 2 |
|------|-----------|-----------|-----------|
| **0** | gbDatosGenerales (ColumnSpan=3) | --- | --- |
| **1** | gbComprobantes | gbConcepto (ColumnSpan=2) | --- |
| **2** | gbMontos | gbOtrosMontos | gbNotasCRDB |

### 3.3 Configuración de tableLayoutPrincipal
- **ColumnCount:** 3
- **RowCount:** 3
- **Columnas:** 40%, 30%, 30%
- **Filas:** 130px, 150px, 150px (aproximado)

---

## 4. CONTROLES CREADOS POR SECCIÓN

### 4.1 Panel Encabezado

| Control | Tipo | Propiedades Clave |
|---------|------|-------------------|
| btnVolver | Button | Dock: Left, Width: 100, Text: "← Volver", FlatStyle: Flat, BackColor: Transparent, ForeColor: 0,120,212 |
| lblTituloFormulario | Label | Dock: Fill, Text: "SOLICITUD DE PAGO", TextAlign: MiddleCenter, Font: Segoe UI 16pt Bold |
| panelDerecho | Panel | Dock: Right, Width: 220 |
| lblNumeroSolicitud | Label | Dock: Left, Width: 120, Text: "SP-000001", Font: Segoe UI 12pt Bold, ForeColor: 0,120,212 |
| btnBuscar | Button | Dock: Right, Width: 95, Text: "🔍 Buscar", BackColor: 0,120,212, ForeColor: White |

### 4.2 GroupBox Datos Generales (gbDatosGenerales)

#### Fila 0 - tableDatosGenerales:

| Control | Tipo | Propiedades Clave |
|---------|------|-------------------|
| lblFecha | Label | Text: "Fecha:", TextAlign: MiddleRight |
| dtpFecha | DateTimePicker | Format: Short |
| lblTipoPago | Label | Text: "Tipo Pago:", TextAlign: MiddleRight |
| cboTipoPago | ComboBox | DropDownStyle: DropDownList |
| lblMoneda | Label | Text: "Moneda:", TextAlign: MiddleRight |
| cboMoneda | ComboBox | DropDownStyle: DropDownList |
| lblTasa | Label | Text: "Tasa:", TextAlign: MiddleRight |
| panelTasaExterno | Panel | Contiene txtTasa + lblNumeroExterno + txtNumeroExterno |
| txtTasa | TextBox | Width: 70 |
| lblNumeroExterno | Label | Text: "N° Ext:" |
| txtNumeroExterno | TextBox | Width: 100 |

#### Fila 1 - tableFideicomisoProveedor (12 columnas):

| Control | Tipo | Propiedades Clave |
|---------|------|-------------------|
| lblCodigoFideicomiso | Label | Text: "Cód. Fideicomiso:", TextAlign: MiddleRight |
| txtCodigoFideicomiso | TextBox | MaxLength: 20 |
| btnAgregarFideicomiso | Button | Text: "+", BackColor: 0,120,212, ForeColor: White, Font: 12pt Bold |
| cboFideicomiso | ComboBox | DropDownStyle: DropDown, AutoCompleteMode: SuggestAppend |
| lblRNCFideicomiso | Label | Text: "RNC: ---", ForeColor: 100,100,100 |
| lblRNCProveedorTitulo | Label | Text: "RNC/Céd:", TextAlign: MiddleRight |
| txtRNCProveedor | TextBox | MaxLength: 15 |
| btnAgregarProveedor | Button | Text: "+", BackColor: 0,120,212, ForeColor: White, Font: 12pt Bold |
| cboProveedor | ComboBox | DropDownStyle: DropDown, AutoCompleteMode: SuggestAppend |
| lblTelefonoProveedor | Label | Text: "Tel: ---", ForeColor: 100,100,100 |

### 4.3 GroupBox Comprobantes (gbComprobantes)

| Control | Tipo | Propiedades Clave |
|---------|------|-------------------|
| lblTipoComprobante | Label | Text: "Tipo:", TextAlign: MiddleRight |
| cboTipoComprobante | ComboBox | DropDownStyle: DropDownList |
| cboTipoNCF | ComboBox | DropDownStyle: DropDownList |
| txtNumeroNCF | TextBox | MaxLength: 13 |
| btnAgregarComprobante | Button | Text: "+ Agregar", BackColor: 0,120,212, ForeColor: White |
| lstComprobantes | ListBox | BorderStyle: FixedSingle, ColumnSpan: 5 |

### 4.4 GroupBox Concepto (gbConcepto)

| Control | Tipo | Propiedades Clave |
|---------|------|-------------------|
| panelConcepto | Panel | Dock: Fill |
| txtConcepto | TextBox | Dock: Fill, Multiline: True, ScrollBars: Vertical, MaxLength: 2000 |
| lblContadorConcepto | Label | Dock: Bottom, Text: "0 / 2000 caracteres", TextAlign: MiddleRight |

### 4.5 GroupBox Montos/Subtotales (gbMontos)

| Control | Tipo | Propiedades Clave |
|---------|------|-------------------|
| panelMontos | Panel | Dock: Fill |
| btnAgregarSubtotal | Button | Dock: Top, Text: "+ Agregar Subtotal", BackColor: 0,120,212 |
| dgvSubtotales | DataGridView | Dock: Fill, RowHeadersVisible: False, AllowUserToAddRows: False |
| lblSubtotalTotal | Label | Dock: Bottom, Text: "SUBTOTAL: RD$ 0.00", Font: 10pt Bold, ForeColor: 0,120,212 |

### 4.6 GroupBox Otros Montos (gbOtrosMontos)

| Control | Tipo | Propiedades Clave |
|---------|------|-------------------|
| tableOtrosMontos | TableLayoutPanel | 2 columnas × 5 filas |
| lblExento | Label | Text: "Exento:" |
| txtExento | TextBox | TextAlign: Right |
| lblDireccionTecnica | Label | Text: "Dir. Técnica:" |
| txtDireccionTecnica | TextBox | TextAlign: Right |
| lblDescuento | Label | Text: "Descuento:" |
| txtDescuento | TextBox | TextAlign: Right |
| lblHorasExtras | Label | Text: "Horas Extras:" |
| txtHorasExtras | TextBox | TextAlign: Right |
| lblOtrosImpuestos | Label | Text: "Otros Imp.:" |
| panelOtrosImpuestos | Panel | Contiene txtOtrosImpuestos + btnConfigOtros |
| txtOtrosImpuestos | TextBox | TextAlign: Right |
| btnConfigOtros | Button | Text: "⚙", Width: 30, BackColor: 108,117,125 |

### 4.7 GroupBox Notas CR/DB (gbNotasCRDB)

| Control | Tipo | Propiedades Clave |
|---------|------|-------------------|
| tableNotasCRDB | TableLayoutPanel | 3 columnas × 4 filas |
| lblNotaCredito | Label | Text: "Nota Crédito:" |
| txtNotaCredito | TextBox | TextAlign: Right |
| btnConfigNC | Button | Text: "⚙", BackColor: 108,117,125 |
| lblNotaDebito | Label | Text: "Nota Débito:" |
| txtNotaDebito | TextBox | TextAlign: Right |
| btnConfigND | Button | Text: "⚙", BackColor: 108,117,125 |
| lblAnticipo | Label | Text: "Anticipo:" |
| txtAnticipo | TextBox | TextAlign: Right |
| lblAvancePagar | Label | Text: "Avance Pagar:" |
| txtAvancePagar | TextBox | TextAlign: Right |

### 4.8 GroupBox Impuestos (gbImpuestos) - Dentro de panelImpuestosContenedor

#### Controles ITBIS (organizados manualmente):

| Control | Tipo | Propiedades Clave |
|---------|------|-------------------|
| lblITBIS | Label | Text: "ITBIS:" |
| cboITBISPorcentaje | ComboBox | DropDownStyle: DropDownList |
| lblITBISBase | Label | Text: "Base:" |
| panelBaseITBIS | Panel | Contiene RadioButtons |
| rbBaseSubtotal | RadioButton | Text: "Subt.", Checked: True |
| rbBaseDirTec | RadioButton | Text: "Dir.Téc" |
| lblITBISCalc | Label | Text: "Calc:" |
| lblITBISCalculado | Label | Text: "RD$ 0.00", Font: Bold, ForeColor: 0,120,212 |
| lblITBISMan | Label | Text: "Man:" |
| txtITBISManual | TextBox | TextAlign: Right |
| lblDif | Label | Text: "Dif:" |
| lblITBISDiferencia | Label | Text: "RD$ 0.00", ForeColor: 128,128,128 |

#### Controles Retenciones (tableRetenciones):

| Control | Tipo | Propiedades Clave |
|---------|------|-------------------|
| lblRetITBIS | Label | Text: "Ret.ITBIS:" |
| cboRetITBIS | ComboBox | DropDownStyle: DropDownList |
| lblRetITBISMonto | Label | Text: "RD$ 0.00", ForeColor: 220,53,69 |
| lblRetISR | Label | Text: "Ret.ISR:" |
| cboRetISR | ComboBox | DropDownStyle: DropDownList |
| lblRetISRMonto | Label | Text: "RD$ 0.00", ForeColor: 220,53,69 |
| lblRetSFS | Label | Text: "SFS:" |
| txtRetSFS | TextBox | TextAlign: Right |
| lblRetAFP | Label | Text: "AFP:" |
| txtRetAFP | TextBox | TextAlign: Right |

### 4.9 GroupBox Observaciones (gbObservaciones) - Dentro de panelObservacionesContenedor

| Control | Tipo | Propiedades Clave |
|---------|------|-------------------|
| panelObservacionesInterno | Panel | Dock: Fill |
| txtObservaciones | TextBox | Dock: Fill, Multiline: True, MaxLength: 1000 |
| panelOpcionesFirma | Panel | Dock: Bottom, Height: 75 |
| tableOpcionesFirma | TableLayoutPanel | 2 columnas × 3 filas |
| chkIncluirFirma | CheckBox | Text: "Incluir firma:" |
| cboFirma | ComboBox | DropDownStyle: DropDownList, Enabled: False |
| chkMostrarConversion | CheckBox | Text: "Conversión:" |
| btnConfigConversion | Button | Text: "⚙ Configurar", BackColor: 108,117,125, Enabled: False |
| lblContadorObservaciones | Label | Text: "0 / 1000 caracteres", ColumnSpan: 2 |

### 4.10 Panel Totales (panelTotales)

| Control | Tipo | Propiedades Clave |
|---------|------|-------------------|
| tableTotales | TableLayoutPanel | 4 columnas × 3 filas |
| lblTotalSubtotalTitulo | Label | Text: "SUBTOTAL: RD$ 0.00", ForeColor: White |
| lblTotalITBISTitulo | Label | Text: "ITBIS: RD$ 0.00", ForeColor: White |
| lblTotalExentoTitulo | Label | Text: "EXENTO: RD$ 0.00", ForeColor: White |
| lblTotalFacturaTitulo | Label | Text: "TOTAL FACTURA: RD$ 0.00", ForeColor: 0,200,255, Font: Bold |
| lblRetITBISTitulo | Label | Text: "RET. ITBIS: RD$ 0.00", ForeColor: 255,180,180 |
| lblRetISRTitulo | Label | Text: "RET. ISR: RD$ 0.00", ForeColor: 255,180,180 |
| lblOtrasRetTitulo | Label | Text: "OTRAS RET: RD$ 0.00", ForeColor: 255,180,180 |
| lblTotalRetencionTitulo | Label | Text: "TOTAL RETENCIÓN: RD$ 0.00", ForeColor: 255,100,100, Font: Bold |
| lblTotalAPagar | Label | Text: "▶▶▶ TOTAL A PAGAR: RD$ 0.00 ◀◀◀", ForeColor: 0,255,150, Font: 14pt Bold, ColumnSpan: 4 |

### 4.11 Panel Botones (panelBotones)

| Control | Tipo | Propiedades Clave |
|---------|------|-------------------|
| tableBotones | TableLayoutPanel | 7 columnas (50% + 5 botones + 50%) |
| btnLimpiar | Button | Text: "🧹 Limpiar", BackColor: 108,117,125, ForeColor: White, Margin: 10,5,10,5 |
| btnGuardar | Button | Text: "💾 Guardar", BackColor: 40,167,69, ForeColor: White, Margin: 10,5,10,5 |
| btnImprimir | Button | Text: "🖨️ Imprimir", BackColor: 0,123,255, ForeColor: White, Margin: 10,5,10,5 |
| btnPDF | Button | Text: "📄 PDF", BackColor: 220,53,69, ForeColor: White, Margin: 10,5,10,5 |
| btnExcel | Button | Text: "📊 Excel", BackColor: 32,136,61, ForeColor: White, Margin: 10,5,10,5 |

---

## 5. PROPIEDADES DE ESTILO UTILIZADAS

### 5.1 Colores Corporativos

| Uso | Color RGB | Hex |
|-----|-----------|-----|
| Azul primario | 0, 120, 212 | #0078D4 |
| Fondo formulario | 245, 247, 250 | #F5F7FA |
| Panel totales | 24, 24, 32 | #181820 |
| Botón gris | 108, 117, 125 | #6C757D |
| Botón verde guardar | 40, 167, 69 | #28A745 |
| Botón rojo PDF | 220, 53, 69 | #DC3545 |
| Botón verde Excel | 32, 136, 61 | #20883D |
| Botón azul imprimir | 0, 123, 255 | #007BFF |
| Texto labels | 64, 64, 64 | #404040 |
| Texto secundario | 128, 128, 128 | #808080 |
| Retenciones (rojo) | 220, 53, 69 | #DC3545 |
| Total a pagar (verde) | 0, 255, 150 | #00FF96 |

### 5.2 Fuentes Utilizadas

| Elemento | Fuente |
|----------|--------|
| Título formulario | Segoe UI, 16pt, Bold |
| Número solicitud | Segoe UI, 12pt, Bold |
| GroupBox títulos | Segoe UI, 9pt, Bold |
| Labels generales | Segoe UI, 9pt |
| TextBoxes | Segoe UI, 9pt |
| Botones | Segoe UI, 10pt |
| Total a pagar | Segoe UI, 14pt, Bold |
| Contadores | Segoe UI, 8pt |

### 5.3 Propiedades Comunes de Botones

```
FlatStyle: Flat
FlatAppearance.BorderSize: 0
Cursor: Hand
Margin: 10, 5, 10, 5 (para botones principales)
```

---

## 6. CÓDIGO BASE IMPLEMENTADO

### 6.1 FormSolicitudPago.cs

```csharp
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MOFIS_ERP.Forms.Contabilidad.CuentasPorPagar.CartasSolicitudes
{
    public partial class FormSolicitudPago : Form
    {
        // ═══════════════════════════════════════════════════════════════
        // CAMPOS PRIVADOS
        // ═══════════════════════════════════════════════════════════════
        private FormMain formPrincipal;
        private bool esNuevoRegistro = true;
        private int solicitudPagoID = 0;

        // ═══════════════════════════════════════════════════════════════
        // CONSTRUCTOR CON PARÁMETRO (para navegación)
        // ═══════════════════════════════════════════════════════════════
        public FormSolicitudPago(FormMain principal)
        {
            InitializeComponent();
            formPrincipal = principal;
            ConfigurarFormulario();
        }

        // ═══════════════════════════════════════════════════════════════
        // CONSTRUCTOR SIN PARÁMETROS (para el diseñador)
        // ═══════════════════════════════════════════════════════════════
        public FormSolicitudPago()
        {
            InitializeComponent();
        }

        // ═══════════════════════════════════════════════════════════════
        // CONFIGURACIÓN INICIAL
        // ═══════════════════════════════════════════════════════════════
        private void ConfigurarFormulario()
        {
            // Configurar Dock para que llene el panel contenedor
            this.Dock = DockStyle.Fill;
            
            // Generar número de solicitud para nuevo registro
            GenerarNumeroSolicitud();
        }

        // ═══════════════════════════════════════════════════════════════
        // GENERAR NÚMERO DE SOLICITUD
        // ═══════════════════════════════════════════════════════════════
        private void GenerarNumeroSolicitud()
        {
            // TODO: Obtener siguiente número desde la secuencia en BD
            // Por ahora, mostrar placeholder
            // lblNumeroSolicitud.Text = "SP-000001";
        }
    }
}
```

---

## 7. FUNCIONALIDADES PENDIENTES

### 7.1 FASE 2: Conexión con FormMenuCartasSolicitudes

- [ ] Modificar `BtnSolicitud_Click` en FormMenuCartasSolicitudes para cargar FormSolicitudPago
- [ ] Implementar carga del formulario en panelAreaTrabajo
- [ ] Manejar el estado del menú lateral al cargar el formulario

### 7.2 FASE 3: Carga de Datos desde BD (Combos)

- [ ] Cargar TiposPago en cboTipoPago
- [ ] Cargar Monedas en cboMoneda
- [ ] Cargar TiposComprobante en cboTipoComprobante
- [ ] Cargar TiposNCF en cboTipoNCF
- [ ] Cargar porcentajes ITBIS en cboITBISPorcentaje (0%, 16%, 18%)
- [ ] Cargar porcentajes Ret. ITBIS en cboRetITBIS (0%, 30%, 100%)
- [ ] Cargar porcentajes Ret. ISR en cboRetISR (0%, 2%, 10%, 27%)
- [ ] Cargar Firmas del usuario en cboFirma

### 7.3 FASE 4: Autocompletado Fideicomiso y Proveedor

- [ ] Implementar búsqueda por código en txtCodigoFideicomiso
- [ ] Implementar autocompletado en cboFideicomiso
- [ ] Mostrar RNC automáticamente en lblRNCFideicomiso
- [ ] Implementar búsqueda por RNC/Cédula en txtRNCProveedor
- [ ] Implementar autocompletado en cboProveedor
- [ ] Mostrar teléfono automáticamente en lblTelefonoProveedor

### 7.4 FASE 5: Gestión de Comprobantes

- [ ] Implementar agregar comprobante a lstComprobantes
- [ ] Validar formato NCF (Serie B: 11 chars, Serie E: 13 chars)
- [ ] Validar NCF no duplicado
- [ ] Implementar eliminar comprobante de la lista
- [ ] Limitar a máximo 10 comprobantes (configurable)

### 7.5 FASE 6: Gestión de Subtotales

- [ ] Configurar columnas de dgvSubtotales (Orden, Monto, Cantidad, Subtotal, Eliminar)
- [ ] Implementar agregar subtotal
- [ ] Implementar editar subtotal inline
- [ ] Implementar eliminar subtotal
- [ ] Calcular subtotal línea (Monto × Cantidad)
- [ ] Calcular suma total de subtotales
- [ ] Actualizar lblSubtotalTotal en tiempo real
- [ ] Limitar a máximo 10 subtotales (configurable)

### 7.6 FASE 7: Contadores de Caracteres

- [ ] Implementar contador para txtConcepto → lblContadorConcepto
- [ ] Implementar contador para txtObservaciones → lblContadorObservaciones

### 7.7 FASE 8: Cálculos Automáticos

- [ ] Calcular ITBIS según base seleccionada (Subtotal o Dir. Técnica)
- [ ] Mostrar ITBIS calculado en lblITBISCalculado
- [ ] Calcular diferencia entre ITBIS calculado y manual
- [ ] Mostrar diferencia en lblITBISDiferencia (rojo si hay diferencia)
- [ ] Calcular Retención ITBIS
- [ ] Mostrar en lblRetITBISMonto
- [ ] Calcular Retención ISR
- [ ] Mostrar en lblRetISRMonto
- [ ] Calcular Total Factura (Subtotal + ITBIS + Exento)
- [ ] Calcular Total Retención
- [ ] Calcular Total a Pagar
- [ ] Actualizar panel de totales en tiempo real

### 7.8 FASE 9: Notas de Crédito/Débito

- [ ] Implementar dos maneras de afectación:
  - Manera 1: Afecta el Total a Pagar (después de ITBIS)
  - Manera 2: Afecta el Subtotal (antes del ITBIS)
- [ ] Crear mini-form FormConfigNotaCredito
- [ ] Crear mini-form FormConfigNotaDebito

### 7.9 FASE 10: Sistema de Conversión de Moneda

- [ ] Mostrar/ocultar txtTasa según moneda seleccionada
- [ ] Habilitar/deshabilitar chkMostrarConversion y btnConfigConversion
- [ ] Implementar 5 métodos de conversión:
  1. Conversión Directa Total
  2. Conversión Base + Recálculo
  3. Conversión Selectiva
  4. Conversión Individual de Subtotales
  5. Conversión Manual/Mixta
- [ ] Crear mini-form FormConfigConversion

### 7.10 FASE 11: Sistema de Firmas

- [ ] Habilitar cboFirma cuando chkIncluirFirma está marcado
- [ ] Cargar firmas del usuario actual
- [ ] Incluir firma en impresión/exportación

### 7.11 FASE 12: Validaciones

- [ ] Validar campos obligatorios antes de guardar
- [ ] Validar formato RNC (9 dígitos: 000-00000-0)
- [ ] Validar formato Cédula (11 dígitos: 000-0000000-0)
- [ ] Validar formato NCF
- [ ] Validar fecha no futura
- [ ] Validar al menos un subtotal > 0
- [ ] Validar concepto mínimo 10 caracteres
- [ ] Habilitar/deshabilitar btnGuardar según validaciones

### 7.12 FASE 13: Guardado en BD

- [ ] Generar número de solicitud desde secuencia
- [ ] Guardar en tabla SolicitudesPago
- [ ] Guardar subtotales en SolicitudesPagoSubtotales
- [ ] Guardar comprobantes en SolicitudesPagoComprobantes
- [ ] Registrar en auditoría
- [ ] Mostrar mensaje de éxito
- [ ] Preguntar si desea imprimir

### 7.13 FASE 14: Carga de Solicitud Existente

- [ ] Implementar búsqueda por ID (btnBuscar)
- [ ] Cargar todos los datos en el formulario
- [ ] Cargar subtotales en dgvSubtotales
- [ ] Cargar comprobantes en lstComprobantes
- [ ] Cambiar modo a edición (esNuevoRegistro = false)

### 7.14 FASE 15: Exportación e Impresión

- [ ] Diseñar formato de impresión (layout inteligente)
- [ ] Implementar btnImprimir
- [ ] Implementar btnPDF (generar PDF con iTextSharp)
- [ ] Implementar btnExcel (generar Excel)
- [ ] Incluir firma digital si está marcada

### 7.15 FASE 16: Funcionalidades Adicionales

- [ ] Implementar btnLimpiar (limpiar formulario con confirmación)
- [ ] Implementar btnVolver (volver a pantalla bienvenida)
- [ ] Implementar detección de cambios no guardados
- [ ] Implementar guardado en memoria temporal
- [ ] Implementar hover en botones

---

## 8. BASE DE DATOS DISPONIBLE

### 8.1 Scripts Ejecutados

**Carpeta 01_Schema:**
- SCRIPT_12_CrearTablasCatalogosCXP.sql
- SCRIPT_13_CrearTablasFideicomisosProveedores.sql
- SCRIPT_14_CrearTablaSolicitudesPago.sql
- SCRIPT_15_CrearTablasFirmasConfiguracion.sql

**Carpeta 02_Data:**
- SCRIPT_08_InsertarDatosCatalogosCXP.sql
- SCRIPT_09_InsertarConfiguracionCXP.sql
- SCRIPT_10_ActualizarCatalogosCXP.sql

### 8.2 Tablas de Catálogos Disponibles

| Tabla | Registros | Descripción |
|-------|-----------|-------------|
| TiposNCF | 22 | Comprobantes fiscales (B01-B17, E31-E47) |
| Monedas | 12 | ISO 4217 (DOP, USD, EUR, etc.) |
| TiposPago | 6 | Transferencia, Cheque, Efectivo, etc. |
| TiposComprobante | 7 | NCF, Cubicación, Cotización, etc. |
| TiposFideicomiso | 6 | Inmobiliario, Administración, etc. |
| MetodosConversion | 5 | DIRECTO, BASE, SELECT, INDIV, MANUAL |

### 8.3 Tablas Maestras

| Tabla | Descripción |
|-------|-------------|
| Fideicomisos | Codigo, Nombre, RNC, TipoFideicomisoID |
| Proveedores | Nombre, TipoDocumento, NumeroDocumento, Telefono, Email |

### 8.4 Tablas Transaccionales

| Tabla | Descripción |
|-------|-------------|
| SolicitudesPago | Tabla principal (~70 campos) |
| SolicitudesPagoSubtotales | Múltiples subtotales por solicitud |
| SolicitudesPagoComprobantes | Múltiples NCF por solicitud |
| SolicitudesPagoAvances | Historial de avances |
| FirmasUsuarios | Firmas digitales PNG |
| ConfiguracionModuloCXP | Parámetros del módulo |

### 8.5 Secuencia

- `SEQ_SolicitudPago` - Genera números SP-000001, SP-000002, etc.

---

## 9. MINI-FORMS PENDIENTES

| Mini-Form | Descripción | Prioridad |
|-----------|-------------|-----------|
| FormAgregarFideicomiso | Agregar fideicomiso sin salir de solicitud | Alta |
| FormAgregarProveedor | Agregar proveedor sin salir de solicitud | Alta |
| FormConfigNotaCredito | Configurar nota de crédito (monto, ITBIS, manera) | Media |
| FormConfigNotaDebito | Configurar nota de débito (monto, ITBIS, manera) | Media |
| FormConfigConversion | Configurar método de conversión de moneda | Media |
| FormBuscarSolicitud | Buscar solicitud por ID o filtros | Media |

---

## 10. NOTAS IMPORTANTES

### 10.1 Metodología de Desarrollo
- Todos los controles se crean **manualmente desde el diseñador** de Visual Studio
- No se genera código automático (problemas de renderizado previos)
- Claude instruye paso a paso, Cysero implementa

### 10.2 Convenciones de Nombres

```
btn... = Button
lbl... = Label
txt... = TextBox
cbo... = ComboBox
dtp... = DateTimePicker
dgv... = DataGridView
lst... = ListBox
chk... = CheckBox
rb...  = RadioButton
panel... = Panel
table... = TableLayoutPanel
gb... = GroupBox
```

### 10.3 Navegación
```csharp
// Siempre usar:
formPrincipal.CargarContenidoPanel(nuevoFormulario);

// Nunca usar:
form.MdiParent = this.MdiParent;
```

### 10.4 Consideraciones de UI
- El formulario se carga dentro de panelAreaTrabajo de FormMenuCartasSolicitudes
- El menú lateral permanece visible (contraído) al lado izquierdo
- Panel de totales siempre visible en la parte inferior
- Botones de acción siempre visibles en la parte inferior
- AutoScroll en panelContenido para manejar resoluciones pequeñas

### 10.5 Formato de Montos
- Separador de miles: coma (,)
- Separador de decimales: punto (.)
- Siempre mostrar 2 decimales
- Prefijo según moneda (RD$, US$, €)

### 10.6 Formato de Documentos
- RNC: 000-00000-0 (9 dígitos)
- Cédula: 000-0000000-0 (11 dígitos)
- NCF Serie B: B0100000001 (11 caracteres)
- NCF Serie E: E310000000001 (13 caracteres)

---

## 📝 PRÓXIMOS PASOS SUGERIDOS

1. **Conectar con FormMenuCartasSolicitudes** - Hacer que el botón "Solicitud de Pago" cargue el formulario
2. **Cargar combos desde BD** - Implementar la carga de datos en todos los ComboBox
3. **Implementar autocompletado** - Fideicomiso y Proveedor
4. **Implementar cálculos** - ITBIS y retenciones en tiempo real
5. **Implementar guardado** - Guardar en base de datos con auditoría

---

**Documento generado para MOFIS-ERP**  
**Versión 1.0 - 20/01/2026**
