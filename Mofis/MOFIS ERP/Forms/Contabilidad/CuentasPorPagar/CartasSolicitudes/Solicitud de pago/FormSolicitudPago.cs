using MOFIS_ERP.Classes;
using MOFIS_ERP.Forms.Contabilidad.CuentasPorPagar.CartasSolicitudes.Solicitud_de_pago;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MOFIS_ERP.Forms.Contabilidad.CuentasPorPagar.CartasSolicitudes
{
    public partial class FormSolicitudPago : Form
    {
        // =========================================================
        // CAMPOS PRIVADOS
        // =========================================================
        private FormMain formPrincipal;
        private bool esNuevoRegistro = true;
        private int solicitudPagoID = 0;

        // DataTables para almacenar datos de catálogos
        private DataTable dtFideicomisos;
        private DataTable dtProveedores;
        private bool numeroNCFPlaceholderActivo = false;

        // Formato de moneda actual (se construye según cboMoneda)
        private NumberFormatInfo monedaNumberFormat;
        private string monedaSimbolo = string.Empty;

        // Lista de TextBoxes que usan formato moneda
        private TextBox[] currencyTextBoxes;

        // Configuración de txtTasa: parte entera y parte decimal parametrizables
        // Por defecto: 2 enteros, 6 decimales (puedes cambiar con SetTasaFormat)
        private int tasaIntegerDigits = 2;
        private int tasaDecimalDigits = 6;

        // Flags para evitar eventos recursivos en combos
        private bool suppressFideicomisoEvents = false;
        private bool suppressProveedorEvents = false;

        // Configuración de Otros Impuestos
        private string otrosImpuestosNombre = "Otros Impuestos:";
        private bool otrosImpuestosSumar = true;

        // Estructura para guardar info de la nota de Crédito/Débito
        private struct NotaInfo
        {
            public decimal Subtotal;
            public bool AplicaITBIS;
            public decimal PorcentajeITBIS;
            public decimal MontoITBIS;
            public decimal Total;
            public string Descripcion;
            public bool AfectaSubtotal; // Modo 2: Afecta base imponible
            public bool EsManual;       // Si true, se ignora todo lo anterior y se usa el valor del TextBox directo
        }

        private NotaInfo notaCredito = new NotaInfo { EsManual = true };
        private NotaInfo notaDebito = new NotaInfo { EsManual = true };
        private bool isRecalculating = false; // Flag para evitar bucles infinitos en TextChanged

        // Constantes de Retención (Valores Estándar aproximados, ajustables)
        // El usuario mencionó que están en el código, si no están, usaremos estos por defecto.
        private const decimal CONST_RET_AFP_PCT = 0.0287m; // 2.87%
        private const decimal CONST_RET_SFS_PCT = 0.0304m; // 3.04%

        // Método de conversión seleccionado (1=DIRECTO, 2=BASE, null si no hay)
        private int? metodoConversionSeleccionado = null;
        private string metodoConversionNombre = string.Empty;

        // =========================================================

        // =========================================================
        // CONSTRUCTORES
        // =========================================================
        public FormSolicitudPago(FormMain principal)
        {
            InitializeComponent();
            formPrincipal = principal;
            ConfigurarFormulario();
            CargarDatosIniciales();
        }

        public FormSolicitudPago()
        {
            InitializeComponent();
            ConfigurarFormulario();
            CargarDatosIniciales();
        }

        // =========================================================
        // CONFIGURACIÓN INICIAL
        // =========================================================
        private void ConfigurarFormulario()
        {
            this.Dock = DockStyle.Fill;

            // Configurar fecha actual
            dtpFecha.Value = DateTime.Today;

            // Inicializar lista de TextBoxes que usaran formato moneda
            currencyTextBoxes = new TextBox[]
            {
                txtExento,
                txtDireccionTecnica,
                txtDescuento,
                txtHorasExtras,
                txtOtrosImpuestos,
                txtNotaCredito,
                txtNotaDebito,
                txtAnticipo,
                txtAvancePagar,
                txtITBISManual,
                txtRetAFP,
                txtRetSFS
            };

            // Establecer formato por defecto de tasa (puedes cambiarlo llamando SetTasaFormat)
            SetTasaFormat(2, 6);

            // Configurar eventos
            ConfigurarEventos();

            // Construir formato por defecto (por si cboMoneda no tiene selección aún)
            monedaNumberFormat = BuildNumberFormat();
        }

        private void SetTasaFormat(int integerDigits, int decimalDigits)
        {
            if (integerDigits < 0) integerDigits = 0;
            if (decimalDigits < 0) decimalDigits = 0;
            if (decimalDigits > 18) decimalDigits = 18;
            if (integerDigits > 18) integerDigits = 18;

            tasaIntegerDigits = integerDigits;
            tasaDecimalDigits = decimalDigits;
        }

        private void ConfigurarEventos()
        {
            // Eventos de Fideicomiso
            txtCodigoFideicomiso.Leave += TxtCodigoFideicomiso_Leave;
            txtCodigoFideicomiso.TextChanged += TxtCodigoFideicomiso_TextChanged;
            cboFideicomiso.SelectedIndexChanged += CboFideicomiso_SelectedIndexChanged;

            // Eventos de Proveedor
            txtRNCProveedor.Enter += TxtRNCProveedor_Enter;
            txtRNCProveedor.Leave += TxtRNCProveedor_Leave;
            txtRNCProveedor.TextChanged += TxtRNCProveedor_TextChanged;
            cboProveedor.SelectedIndexChanged += CboProveedor_SelectedIndexChanged;

            // Evento de Moneda (mostrar/ocultar tasa)
            cboMoneda.SelectedIndexChanged += CboMoneda_SelectedIndexChanged;

            // Eventos de Conversión de Moneda
            btnConfigConversion.Click += BtnConfigConversion_Click;
            chkMostrarConversion.CheckedChanged += ChkMostrarConversion_CheckedChanged;

            // Evento de Concepto (contador de caracteres)
            txtConcepto.TextChanged += TxtConcepto_TextChanged;

            // Evento de Observaciones (contador de caracteres)
            txtObservaciones.TextChanged += TxtObservaciones_TextChanged;

            // Evento del botón Agregar Fideicomiso
            btnAgregarFideicomiso.Click += BtnAgregarFideicomiso_Click;

            // Evento del botón Agregar Proveedor (mini-form)
            btnAgregarProveedor.Click += BtnAgregarProveedor_Click;
            
            // Evento Configuración Otros Impuestos
            btnConfigOtros.Click += BtnConfigOtros_Click;

            // Eventos Notas Crédito / Débito
            btnConfigNC.Click += BtnConfigNC_Click;
            btnConfigND.Click += BtnConfigND_Click;

            // Eventos manuales de Notas (para limpiar config si el usuario escribe)
            // SOLO si el valor cambia realmente y tiene foco (evita borrado por formateo)
            txtNotaCredito.TextChanged += (s, e) => 
            {
                if (isRecalculating) return;
                
                if (txtNotaCredito.Focused)
                {
                     decimal valTexto = ParseMoneda(txtNotaCredito.Text);
                     // Si el valor numérico cambió y no era manual, pasar a manual
                     if (!notaCredito.EsManual && Math.Abs(valTexto - notaCredito.Total) > 0.01m)
                     {
                         notaCredito.EsManual = true;
                         notaCredito.AfectaSubtotal = false; 
                     }
                }
                RecalcularTodo(); 
            };
            
            txtNotaDebito.TextChanged += (s, e) => 
            { 
                 if (isRecalculating) return;
                 
                 if (txtNotaDebito.Focused)
                 {
                     decimal valTexto = ParseMoneda(txtNotaDebito.Text);
                     if (!notaDebito.EsManual && Math.Abs(valTexto - notaDebito.Total) > 0.01m)
                     {
                         notaDebito.EsManual = true;
                         notaDebito.AfectaSubtotal = false;
                     }
                 }
                 RecalcularTodo();
            };

            // Asegurar que RecalcularTodo se llame en cambios clave
            if (cboITBISPorcentaje != null) cboITBISPorcentaje.SelectedIndexChanged += (s, e) => RecalcularTodo();
            if (rbBaseSubtotal != null) rbBaseSubtotal.CheckedChanged += (s, e) => RecalcularTodo();
            if (rbBaseDirTec != null) rbBaseDirTec.CheckedChanged += (s, e) => RecalcularTodo();
            if (cboRetITBIS != null) cboRetITBIS.SelectedIndexChanged += (s, e) => RecalcularTodo();
            if (cboRetISR != null) cboRetISR.SelectedIndexChanged += (s, e) => RecalcularTodo();
            if (chkITBISManual != null) chkITBISManual.CheckedChanged += (s, e) => { ActualizarEstadoITBISManual(); RecalcularTodo(); };
            if (txtITBISManual != null) txtITBISManual.TextChanged += (s, e) => RecalcularTodo();

            // Suscribir todos los campos monetarios a RecalcularTodo
            if (currencyTextBoxes != null)
            {
                foreach (var tb in currencyTextBoxes)
                {
                    if (tb != null && tb != txtNotaCredito && tb != txtNotaDebito && tb != txtITBISManual) 
                    {
                        tb.TextChanged += (s, e) => RecalcularTodo();
                    }
                }
            }

            // =========================================================
            // A partir de aquí se listan los cambios solicitados


            // =========================================================
            // A partir de aquí se listan los cambios solicitados
            // =========================================================

            // Evento del botón Agregar Comprobante (mini-form)
            if (btnAgregarComprobante != null) btnAgregarComprobante.Click += BtnAgregarComprobante_Click;

            // Evento maestro: activar/desactivar cálculo automático del ITBIS
            if (chkCalcularITBIS != null)
            {
                chkCalcularITBIS.CheckedChanged += (s, e) => {
                     UpdateEstadoCalculoITBIS();
                     RecalcularTodo();
                };
            }
            if (chkCalcularITBIS != null)
            {
                // chkCalcularITBIS.CheckedChanged += ChkCalcularITBIS_CheckedChanged; // Eliminamos la asignacion duplicada si ya existia para evitar doble llamada o conflictos
            }

            // Evento de ITBIS Manual (override)
            if (chkITBISManual != null)
            {
                // chkITBISManual.CheckedChanged += ChkITBISManual_CheckedChanged; // Lo integramos arriba
            }

            // CheckBoxes para retenciones (habilitan/deshabilitan sus TextBoxes)
            if (chkRetSFS != null)
            {
                chkRetSFS.CheckedChanged += (s, e) => {
                     ChkRetSFS_CheckedChanged(s, e);
                     ActualizarRetencionesLey();
                };
            }

            if (chkRetAFP != null)
            {
                chkRetAFP.CheckedChanged += (s, e) => {
                     ChkRetAFP_CheckedChanged(s, e);
                     ActualizarRetencionesLey();
                };
            }


            // Evento para incluir firma -> habilita/deshabilita cboFirma
            if (chkIncluirFirma != null)
            {
                chkIncluirFirma.CheckedChanged += ChkIncluirFirma_CheckedChanged;
            }

            // Evento de tipo NCF (placeholder inteligente)
            cboTipoNCF.SelectedIndexChanged += CboTipoNCF_SelectedIndexChanged;
            txtNumeroNCF.Enter += TxtNumeroNCF_Enter;
            txtNumeroNCF.Leave += TxtNumeroNCF_Leave;

            // Evento de Comprobante (nuevo handler)
            cboTipoComprobante.SelectedIndexChanged += CboTipoComprobante_SelectedIndexChanged;

            // -------------------------------------------------------
            // Restricciones de entrada NUMÉRICA - solo para los
            // campos solicitados (KeyPress + TextChanged sanitizador)
            // -------------------------------------------------------

            // Enteros (solo dígitos)
            txtCodigoFideicomiso.KeyPress += NumericInteger_KeyPress;
            txtCodigoFideicomiso.TextChanged += (s, e) => SanitizeIntegerTextBox(txtCodigoFideicomiso);

            txtRNCProveedor.KeyPress += NumericInteger_KeyPress;
            // Nota: no adjuntamos SanitizeIntegerTextBox a TextChanged de txtRNCProveedor
            // porque queremos mostrar formateo (guiones) en Leave / al seleccionar proveedor.

            // Decimales (dígitos y un punto decimal) para la mayoría de TextBoxes
            Action<TextBox> attachDecimal = tb =>
            {
                if (tb == null) return;
                tb.KeyPress += NumericDecimal_KeyPress;

                if (!currencyTextBoxes.Contains(tb) && tb != txtTasa)
                {
                    tb.TextChanged += (s, e) => SanitizeDecimalTextBox(tb);
                }
            };

            attachDecimal(txtExento);
            attachDecimal(txtDireccionTecnica);
            attachDecimal(txtDescuento);
            attachDecimal(txtHorasExtras);
            attachDecimal(txtOtrosImpuestos);
            attachDecimal(txtNotaCredito);
            attachDecimal(txtNotaDebito);
            attachDecimal(txtAnticipo);
            attachDecimal(txtAvancePagar);
            attachDecimal(txtITBISManual);
            attachDecimal(txtRetAFP);
            attachDecimal(txtRetSFS);

            // Manejadores especiales para txtTasa
            txtTasa.KeyPress += Tasa_KeyPress;
            txtTasa.TextChanged += Tasa_TextChanged;
            txtTasa.Enter += (s, e) => { txtTasa.SelectionStart = txtTasa.Text.Length; };

            // Evento del botón Agregar Subtotal
            btnAgregarSubtotal.Click += BtnAgregarSubtotal_Click;

            // Eventos del DataGridView Subtotales


            // Configurar Grid al inicio
            ConfigurarGridSubtotales();
            
            // Adjuntar manejadores de formato moneda (Enter/Leave) a los TextBoxes listados
            foreach (var tb in currencyTextBoxes)
            {
                if (tb == null) continue;
                tb.Enter += CurrencyTextBox_Enter;
                tb.Leave += CurrencyTextBox_Leave;
            }
        }

        // Construye NumberFormatInfo con separador de miles "," y decimal "."
        private NumberFormatInfo BuildNumberFormat()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = ",";
            nfi.NumberDecimalSeparator = ".";
            return nfi;
        }

        private void UpdateMonedaFormatFromCombo()
        {
            monedaSimbolo = string.Empty;
            monedaNumberFormat = BuildNumberFormat();

            if (cboMoneda != null && cboMoneda.SelectedItem is DataRowView drv)
            {
                monedaSimbolo = (drv["Simbolo"] ?? string.Empty).ToString();

                try
                {
                    string iso = (drv["CodigoISO"] ?? string.Empty).ToString().ToUpper();
                    if (iso == "JPY" || iso == "CLP")
                    {
                        monedaNumberFormat.NumberDecimalDigits = 0;
                    }
                    else
                    {
                        monedaNumberFormat.NumberDecimalDigits = 2;
                    }
                }
                catch
                {
                    monedaNumberFormat.NumberDecimalDigits = 2;
                }
            }

            monedaSimbolo = monedaSimbolo ?? string.Empty;
        }

        private string FormatNumericPart(string integerPart, string decimalPart)
        {
            var nfi = monedaNumberFormat ?? BuildNumberFormat();

            long intVal = 0;
            string intFormatted = integerPart;
            if (long.TryParse(integerPart, out intVal))
            {
                intFormatted = intVal.ToString("N0", nfi);
            }

            if (!string.IsNullOrEmpty(decimalPart))
            {
                return intFormatted + "." + decimalPart;
            }

            return intFormatted + ".00";
        }

        private void CurrencyTextBox_Enter(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            string text = tb.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(text))
            {
                tb.Text = string.Empty;
                return;
            }

            string cleaned = Regex.Replace(text, "[^0-9,\\.\\-]", string.Empty);
            tb.Text = cleaned;
            tb.SelectionStart = tb.Text.Length;
        }

        private void CurrencyTextBox_Leave(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            string text = tb.Text ?? string.Empty;
            string cleaned = Regex.Replace(text, @"[^0-9.\-]", string.Empty);

            if (string.IsNullOrWhiteSpace(cleaned))
            {
                tb.Text = string.Empty;
                return;
            }

            string[] parts = cleaned.Split('.');
            string intPart = parts.Length > 0 ? parts[0] : "0";
            string decPart = parts.Length > 1 ? parts[1] : null;

            intPart = intPart.TrimStart('0');
            if (string.IsNullOrEmpty(intPart)) intPart = "0";

            string numericFormatted = FormatNumericPart(intPart, decPart);

            if (!string.IsNullOrEmpty(monedaSimbolo))
                tb.Text = monedaSimbolo + " " + numericFormatted;
            else
                tb.Text = numericFormatted;
        }

        private void UpdateAllCurrencyDisplays()
        {
            UpdateMonedaFormatFromCombo();

            foreach (var tb in currencyTextBoxes)
            {
                if (tb == null) continue;
                if (!string.IsNullOrWhiteSpace(tb.Text))
                {
                    CurrencyTextBox_Leave(tb, EventArgs.Empty);
                }
            }

            ReformatLabelNumberIfPossible(lblITBISCalculado);
            ReformatLabelNumberIfPossible(lblITBISDiferencia);
            ReformatLabelNumberIfPossible(lblRetITBISMonto);
            ReformatLabelNumberIfPossible(lblRetISRMonto);

            ReformatAmountPortionInLabel(lblTotalSubtotalTitulo);
            ReformatAmountPortionInLabel(lblTotalITBISTitulo);
            ReformatAmountPortionInLabel(lblTotalExentoTitulo);
            ReformatAmountPortionInLabel(lblTotalFacturaTitulo);
            ReformatAmountPortionInLabel(lblRetITBISTitulo);
            ReformatAmountPortionInLabel(lblRetISRTitulo);
            ReformatAmountPortionInLabel(lblOtrasRetTitulo);
            ReformatAmountPortionInLabel(lblTotalRetencionTitulo);
            ReformatAmountPortionInLabel(lblTotalAPagar);


            // Actualizar formato del grid de subtotales y su label
            ActualizarFormatoGridSubtotales();
            
            // Llamada maestra al recalculo
            RecalcularTodo();
        }

        // =========================================================
        // LÓGICA DE NOTAS CRÉDITO / DÉBITO Y CÁLCULO FINAL
        // =========================================================
        
        private void BtnConfigNC_Click(object sender, EventArgs e)
        {
            AbrirConfigNota(true);
        }

        private void BtnConfigND_Click(object sender, EventArgs e)
        {
            AbrirConfigNota(false);
        }

        private void AbrirConfigNota(bool esCredito)
        {
            decimal subtotalSolicitud = ObtenerSubtotalSimple(); // El subtotal "bruto" del grid
            bool tieneSubtotal = subtotalSolicitud > 0;

            NotaInfo infoActual = esCredito ? notaCredito : notaDebito;
            
            // Si estaba en manual, el valor del textbox quizás no cuadre con la info interna
            // Pasaremos null como subtotalActual si es manual, para que inicie limpio,
            // o podríamos intentar inferir. Por simplicidad, si es manual, abrimos limpio o con el valor total como subtotal provisional.
            decimal? subPrevio = infoActual.EsManual ? (decimal?)null : infoActual.Subtotal;
            
            using (var form = new FormConfigNota(esCredito, tieneSubtotal, 
                                                monedaNumberFormat ?? BuildNumberFormat(),
                                                monedaSimbolo,
                                                subPrevio, 
                                                infoActual.AplicaITBIS, 
                                                infoActual.PorcentajeITBIS > 0 ? infoActual.PorcentajeITBIS : 18m,
                                                infoActual.Descripcion,
                                                infoActual.AfectaSubtotal))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    NotaInfo nuevaInfo = new NotaInfo
                    {
                        Subtotal = form.Subtotal,
                        AplicaITBIS = form.AplicaITBIS,
                        PorcentajeITBIS = form.PorcentajeITBIS,
                        MontoITBIS = form.MontoITBIS,
                        Total = form.TotalNota,
                        Descripcion = form.Descripcion,
                        AfectaSubtotal = form.AfectaSubtotal,
                        EsManual = false
                    };

                    // IMPORTANTE: Primero actualizamos el texto CON FORMATO
                    // Seteamos el texto y la info primero, y luego RecalcularTodo
                    // Para evitar que el primer RecalcularTodo (disparado por TextChanged) 
                    // vea un estado inconsistente.
                    
                    if (esCredito)
                        notaCredito = nuevaInfo;
                    else
                        notaDebito = nuevaInfo;
                    
                    if (esCredito)
                        txtNotaCredito.Text = FormatearMoneda(nuevaInfo.Total);
                    else
                        txtNotaDebito.Text = FormatearMoneda(nuevaInfo.Total);

                    RecalcularTodo();
                }
            }
        }


        // Método central de cálculo
        private void RecalcularTodo()
        {
            if (isRecalculating) return;
            isRecalculating = true;

            try
            {
                // =========================================================
                // 1. OBTENER SUBTOTALES Y BASES
                // =========================================================
                
                // Subtotal del Grid (Suma de items)
                decimal subtotalGrid = ObtenerSubtotalSimple();

                // Montos de Dirección Técnica y Exento
                decimal montoDirTecnica = ParseMoneda(txtDireccionTecnica?.Text);
                decimal montoExento = ParseMoneda(txtExento?.Text);
                decimal montoOtrosImp = ParseMoneda(txtOtrosImpuestos?.Text);
                if (!otrosImpuestosSumar) montoOtrosImp = -montoOtrosImp;
                
                decimal montoHorasExtras = ParseMoneda(txtHorasExtras?.Text);
                
                // --- Variables de control para ITBIS ---
                bool usareManual = chkITBISManual != null && chkITBISManual.Checked;
                bool calcularAuto = chkCalcularITBIS != null && chkCalcularITBIS.Checked;

                // VALIDACIÓN BASE DIRECCIÓN TÉCNICA
                if (rbBaseDirTec != null && rbBaseDirTec.Checked)
                {
                    if (montoDirTecnica <= 0 && calcularAuto)
                    {
                        MessageBox.Show("No se puede calcular ITBIS sobre Dirección Técnica porque el monto es 0.\nSe cambiará la base a 'Subtotal'.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        rbBaseSubtotal.Checked = true;
                        // Al cambiar el radio button, se disparará RecalcularTodo nuevamente
                        return; 
                    }
                }

                // =========================================================
                // 2. APLICAR NOTAS MODO 2 (AFECTAN BASE)
                // =========================================================
                
                decimal ajusteSubtotalNotas = 0m;

                // Verificamos si los textboxes están vacíos o cambiaron para sincronizar estado manual
                
                string txtNC = txtNotaCredito?.Text ?? "";
                decimal valNC_UI = ParseMoneda(txtNC);
                
                if (string.IsNullOrWhiteSpace(txtNC) && notaCredito.Total != 0)
                {
                    // Usuario borró el texto manual -> Reset total a 0 y esManual
                    notaCredito = new NotaInfo { EsManual = true, Total = 0 }; 
                }
                else if (notaCredito.EsManual)
                {
                    notaCredito.Total = valNC_UI;
                }

                string txtND = txtNotaDebito?.Text ?? "";
                decimal valND_UI = ParseMoneda(txtND);
                if (string.IsNullOrWhiteSpace(txtND) && notaDebito.Total != 0)
                {
                    notaDebito = new NotaInfo { EsManual = true, Total = 0 };
                }
                else if (notaDebito.EsManual)
                {
                     notaDebito.Total = valND_UI;
                }

                // Ajustamos variables locales para usar los valores finales según modo
                decimal valNC_final = notaCredito.EsManual ? valNC_UI : notaCredito.Subtotal;
                decimal valND_final = notaDebito.EsManual ? valND_UI : notaDebito.Subtotal;

                // Aplicar lógica Modo 2 (Afectan Subtotal)
                if (!notaCredito.EsManual && notaCredito.AfectaSubtotal)
                    ajusteSubtotalNotas -= valNC_final;

                if (!notaDebito.EsManual && notaDebito.AfectaSubtotal)
                    ajusteSubtotalNotas += valND_final;

                // Base Imponible Real (Subtotal Grid +/- Notas Modo 2)
                decimal baseImponibleReal = subtotalGrid + ajusteSubtotalNotas;

                if (baseImponibleReal < 0) baseImponibleReal = 0;


                // =========================================================
                // 3. CÁLCULO DE ITBIS (AUTO vs MANUAL)
                // =========================================================

                decimal itbisFinal = 0m;
                decimal itbisCalculadoAuto = 0m; // Para mostrar en label y calcular diferencia

                // --- Cálculo Automático ---
                if (calcularAuto)
                {
                    // Determinar Base para ITBIS
                    decimal baseParaITBIS = baseImponibleReal; // Por defecto rbBaseSubtotal
                    
                    if (rbBaseDirTec != null && rbBaseDirTec.Checked)
                    {
                        baseParaITBIS = montoDirTecnica;
                    }

                    // Obtener Porcentaje
                    decimal pctITBIS = 0.18m;
                    if (cboITBISPorcentaje != null && cboITBISPorcentaje.SelectedItem != null)
                    {
                         string sPct = cboITBISPorcentaje.SelectedItem.ToString().Replace("%", "").Trim();
                         if (decimal.TryParse(sPct, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal p))
                             pctITBIS = p / 100m;
                    }

                    itbisCalculadoAuto = Math.Round(baseParaITBIS * pctITBIS, 2);
                }

                // --- Selección Final ITBIS ---
                if (usareManual)
                {
                    itbisFinal = ParseMoneda(txtITBISManual?.Text);
                }
                else
                {
                    itbisFinal = itbisCalculadoAuto;
                }

                // --- Actualización UI ITBIS ---
                if (lblITBISCalculado != null) 
                    lblITBISCalculado.Text = FormatearMonedaPlain(itbisCalculadoAuto); // Solo número o con símbolo? User dijo "Muestra el ITBIS calculado". Mejor con formato completo:
                if (lblITBISCalculado != null) lblITBISCalculado.Text = FormatearMoneda(itbisCalculadoAuto);

                if (lblITBISDiferencia != null)
                {
                     decimal dif = itbisCalculadoAuto - itbisFinal;
                     lblITBISDiferencia.Text = $"{FormatearMoneda(dif)}";
                     // Color coding opcional
                     lblITBISDiferencia.ForeColor = (Math.Abs(dif) > 0.01m) ? Color.Red : Color.Green;
                }

                // =========================================================
                // 4. RETENCIONES
                // =========================================================
                
                decimal totalRetenciones = 0m;
                
                // RETENCIÓN ITBIS (Sobre el ITBIS Final seleccionado)
                decimal retItbisMonto = 0m;
                if (cboRetITBIS != null && cboRetITBIS.SelectedItem != null)
                {
                    string sRetItb = cboRetITBIS.SelectedItem.ToString().Replace("%", "").Trim();
                     if (decimal.TryParse(sRetItb, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal pRet))
                     {
                         retItbisMonto = Math.Round(itbisFinal * (pRet / 100m), 2);
                     }
                }
                if (lblRetITBISMonto != null) lblRetITBISMonto.Text = FormatearMoneda(retItbisMonto);
                if (lblRetITBISTitulo != null) lblRetITBISTitulo.Text = $"RETENCIÓN ITBIS: {FormatearMoneda(retItbisMonto)}";
                
                // RETENCIÓN ISR (Sobre el Subtotal / Base Imponible Real)
                // User: "se hace en base al subtotal en el label 'lblSubtotalTotal'" -> baseImponibleReal (que es lo que muestra ese label ahora)
                decimal retIsrMonto = 0m;
                if (cboRetISR != null && cboRetISR.SelectedItem != null)
                {
                    string sRetIsr = cboRetISR.SelectedItem.ToString().Replace("%", "").Trim();
                     if (decimal.TryParse(sRetIsr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal pIsr))
                     {
                         retIsrMonto = Math.Round(baseImponibleReal * (pIsr / 100m), 2);
                     }
                }
                if (lblRetISRMonto != null) lblRetISRMonto.Text = FormatearMoneda(retIsrMonto);
                if (lblRetISRTitulo != null) lblRetISRTitulo.Text = $"RETENCIÓN ISR: {FormatearMoneda(retIsrMonto)}";


                // RETENCIONES AFP / SFS (Constantes sobre Subtotal)
                // Lógica requerida: Si check, calcular automático. Si usuario cambia, permitir pero avisar (no implementaré aviso invasivo ahora, solo lógica de cálculo).
                // Si check SFS -> Calcular. Pero si el campo tiene valor ingresado manual que difiere, ¿respetamos manual o forzamos?
                // REGLA: "al habilitarlos, el calculo debe ser automatico... y luego el usuario quiere ingresar otro monto diferente, el sistema debe permitirlo"
                // Implementación: Solo calculamos y sobrescribimos SI el usuario acaba de activar el check (evento CheckedChanged aparte) O si el campo está vacío/cero. 
                // Pero en RecalculateTodo, sobrescribir cada vez impediría edición manual.
                // SOLUCIÓN: El cálculo automático de AFP/SFS lo haremos en los eventos CheckedChanged o cuando cambie el Subtotal SOLO SI el usuario no lo ha 'tocado' manualmente o si forzamos.
                // Como RecalcularTodo corre siempre, mejor leemos el valor del TextBox. La lógica de "poner el valor automático" la delegamos a un helper que se llame cuando cambie el Check o el Subtotal.
                
                // Para este paso, simplemente SUMAMOS lo que haya en los TextBoxes si están habilitados.
                decimal valAFP = (chkRetAFP != null && chkRetAFP.Checked) ? ParseMoneda(txtRetAFP?.Text) : 0m;
                decimal valSFS = (chkRetSFS != null && chkRetSFS.Checked) ? ParseMoneda(txtRetSFS?.Text) : 0m;

                totalRetenciones = retItbisMonto + retIsrMonto + valAFP + valSFS;

                if (lblTotalRetencionTitulo != null) lblTotalRetencionTitulo.Text = $"TOTAL RETENCIÓN: {FormatearMoneda(totalRetenciones)}";
                
                // Otras Retenciones (Label) -> Debe reflejar suma AFP + SFS según requerimiento
                if (lblOtrasRetTitulo != null)
                {
                    decimal otras = valAFP + valSFS;
                    lblOtrasRetTitulo.Text = $"OTRAS RETENCIONES: {FormatearMoneda(otras)}";
                }
                
                // =========================================================
                // 5. TOTAL FACTURA

                // =========================================================
                // Componentes: Base + ITBIS + Exento + DirTec + HorasExtra + OtrosImp
                // (Anticipo se resta ahora en el Total a Pagar, no aquí)
                
                decimal montoAnticipo = ParseMoneda(txtAnticipo?.Text);

                decimal totalFactura = baseImponibleReal + itbisFinal + montoExento + montoDirTecnica + montoHorasExtras + montoOtrosImp;

                if (lblTotalFacturaTitulo != null) lblTotalFacturaTitulo.Text = $"TOTAL FACTURA: {FormatearMoneda(totalFactura)}";
                if (lblTotalExentoTitulo != null) lblTotalExentoTitulo.Text = $"EXENTO: {FormatearMoneda(montoExento)}";
                if (lblTotalITBISTitulo != null) lblTotalITBISTitulo.Text = $"ITBIS: {FormatearMoneda(itbisFinal)}";

                // =========================================================
                // 6. TOTAL A PAGAR
                // =========================================================
                // Total A Pagar = Total Factura - Retenciones - Descuento - Anticipo - Avance + AjustesNotasModo1
                
                decimal montoDescuento = ParseMoneda(txtDescuento?.Text);
                decimal montoAvance = ParseMoneda(txtAvancePagar?.Text); // Referencia solo

                // Notas Modo 1 o Manuales
                decimal ajusteNotasPagar = 0m;
                
                // Nota Crédito
                if (notaCredito.EsManual)
                {
                    ajusteNotasPagar -= valNC_UI;
                }
                else if (!notaCredito.AfectaSubtotal) // Modo 1 Aut
                {
                    ajusteNotasPagar -= notaCredito.Total;
                }

                // Nota Débito
                if (notaDebito.EsManual)
                {
                    ajusteNotasPagar += valND_UI;
                }
                else if (!notaDebito.AfectaSubtotal) // Modo 1 Aut
                {
                    ajusteNotasPagar += notaDebito.Total;
                }

                decimal totalAPagar = totalFactura - totalRetenciones - montoDescuento - montoAnticipo + ajusteNotasPagar;

                if (lblTotalAPagar != null)
                    lblTotalAPagar.Text = $"▶▶▶  TOTAL A PAGAR:  {FormatearMoneda(totalAPagar)}  ◀◀◀";

                // Sincronizar Label de Subtotal arriba del grid también para reflejar impacto
                if (lblSubtotalTotal != null)
                {
                    lblSubtotalTotal.Text = $"SUBTOTAL: {FormatearMoneda(baseImponibleReal)}";
                    lblSubtotalTotal.ForeColor = (notaCredito.AfectaSubtotal || notaDebito.AfectaSubtotal) ? Color.OrangeRed : Color.FromArgb(64, 64, 64);
                }

                // Sincronizar TextBoxes de notas si NO tienen foco (para que tengan el formato correcto)
                if (txtNotaCredito != null && !txtNotaCredito.Focused)
                {
                    decimal val = notaCredito.EsManual ? valNC_UI : notaCredito.Total;
                    string res = val != 0 ? FormatearMoneda(val) : "";
                    if (txtNotaCredito.Text != res) txtNotaCredito.Text = res;
                }
                if (txtNotaDebito != null && !txtNotaDebito.Focused)
                {
                    decimal val = notaDebito.EsManual ? valND_UI : notaDebito.Total;
                    string res = val != 0 ? FormatearMoneda(val) : "";
                    if (txtNotaDebito.Text != res) txtNotaDebito.Text = res;
                }




                // Update Subtotal Header
                if (lblTotalSubtotalTitulo != null) 
                {
                    lblTotalSubtotalTitulo.Text = $"SUBTOTAL: {FormatearMoneda(baseImponibleReal)}";
                    lblTotalSubtotalTitulo.ForeColor = (notaCredito.AfectaSubtotal || notaDebito.AfectaSubtotal) ? Color.Orange : Color.White;
                }
                
                // =========================================================
                // 7. ACTUALIZACIÓN AUTOMÁTICA AFP/SFS (Si aplica)
                // =========================================================
                // Si los checks están activos, queremos que el valor se mantenga actualizado con el subtotal
                // SIEMPRE QUE el usuario no lo haya editado a mano.
                // Esto es complejo de detectar sin un flag extra. 
                // Simplificación: Recalculamos siempre si el check está activo, SALVO que estemos editando ese campo específico.
                // Pero el user quiere poder editar. 
                // Estrategia: Solo actualizar AFP/SFS cuando cambie la Base (Subtotal) o se activen los checks.
                // Lo haré en un método aparte llamado desde los eventos pertinentes, no aquí en RecalcularTodo para evitar sobrescritura constante.
            }
            catch (Exception ex)
            {
               System.Diagnostics.Debug.WriteLine("Error al recalcular: " + ex.Message);
            }
            finally
            {
                isRecalculating = false;
            }
        }
        
        // Método helper para actualizar AFP/SFS cuando cambia la base o se activan
        private void ActualizarRetencionesLey()
        {
            if (isRecalculating) return;
            
            decimal baseCalculo = ObtenerSubtotalSimple(); 
            // ¿Base original o afectada por notas? User dijo "base al subtotal en el label 'lblSubtotalTotal'".
            // Ese label ahora muestra la base afectada por notas Modo 2.
            // Usaremos la base afectada si hay notas Modo 2.
             decimal ajuste = 0m;
             if (!notaCredito.EsManual && notaCredito.AfectaSubtotal) ajuste -= notaCredito.Subtotal;
             if (!notaDebito.EsManual && notaDebito.AfectaSubtotal) ajuste += notaDebito.Subtotal;
             baseCalculo += ajuste;
             if (baseCalculo < 0) baseCalculo = 0;

            if (chkRetAFP != null && chkRetAFP.Checked)
            {
                decimal val = Math.Round(baseCalculo * CONST_RET_AFP_PCT, 2);
                if (txtRetAFP != null) txtRetAFP.Text = FormatearMonedaPlain(val);
            }
            
            if (chkRetSFS != null && chkRetSFS.Checked)
            {
                 decimal val = Math.Round(baseCalculo * CONST_RET_SFS_PCT, 2);
                 if (txtRetSFS != null) txtRetSFS.Text = FormatearMonedaPlain(val);
            }
            
            // Esto actualizará los textbox -> TextChanged -> RecalcularTodo
        }

        private decimal ObtenerSubtotalSimple()
        {
            decimal total = 0m;
            foreach (Control ctrl in flpSubtotales.Controls)
            {
                if (ctrl is SubtotalItemControl item)
                    total += item.Monto;
            }
            return total;
        }

        private decimal ParseMoneda(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0m;
            string clean = Regex.Replace(text, "[^0-9.-]", "");
            if (decimal.TryParse(clean, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal val))
                return val;
            return 0m;
        }

        private string FormatearMoneda(decimal valor)
        {
             string simbolo = !string.IsNullOrEmpty(monedaSimbolo) ? monedaSimbolo : "RD$";
             var nfi = monedaNumberFormat ?? BuildNumberFormat();
             return $"{simbolo} {valor.ToString("N2", nfi)}";
        }
        
        private string FormatearMonedaPlain(decimal valor)
        {
             var nfi = monedaNumberFormat ?? BuildNumberFormat();
             return valor.ToString("N2", nfi);
        }

        private decimal ObtenerValorLabel(Label lbl)
        {
            if (lbl == null) return 0m;
            // Extraer números del texto (ej "RET ITBIS: RD$ 450.00")
            // Usamos regex para sacar el último bloque numérico
            var match = Regex.Match(lbl.Text, @"[0-9]+(?:\.[0-9]+)?");
            // Buscar la coincidencia que parezca monto (con decimales o al final)
            // Mejor limpieza:
             string clean = Regex.Replace(lbl.Text, "[^0-9.-]", "");
             // Ojo con guiones en textos no numéricos.
             if (decimal.TryParse(clean, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal val))
                return val;
            return 0m;
        }


        private void ReformatLabelNumberIfPossible(Label lbl)
        {
            if (lbl == null) return;
            string text = lbl.Text ?? string.Empty;
            string cleaned = Regex.Replace(text, @"[^0-9.\-]", string.Empty);
            if (string.IsNullOrWhiteSpace(cleaned)) return;

            decimal val;
            if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
            {
                lbl.Text = (!string.IsNullOrEmpty(monedaSimbolo) ? monedaSimbolo + " " : string.Empty) + val.ToString("N2", monedaNumberFormat ?? BuildNumberFormat());
            }
        }

        private void ReformatAmountPortionInLabel(Label lbl)
        {
            if (lbl == null) return;
            string text = lbl.Text ?? string.Empty;

            var matches = Regex.Matches(text, @"[0-9][0-9,\.]*");
            if (matches.Count == 0) return;

            var last = matches[matches.Count - 1];
            string numericPortion = last.Value;

            string cleaned = numericPortion.Replace(",", string.Empty);

            decimal val;
            if (!decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
            {
                return;
            }

            string formattedNumeric = val.ToString("N" + (monedaNumberFormat?.NumberDecimalDigits ?? 2), monedaNumberFormat ?? BuildNumberFormat());

            string before = text.Substring(0, last.Index);
            string after = text.Substring(last.Index + last.Length);

            var m = Regex.Match(before, @"(\S+)\s*$");
            string newBefore;
            if (m.Success)
            {
                string token = m.Groups[1].Value;
                bool tokenIsSymbol = token.Any(c => !char.IsDigit(c) && c != ':' && c != '\\' && c != '\n');

                if (tokenIsSymbol)
                {
                    string prefix = string.Empty;
                    if (!string.IsNullOrEmpty(monedaSimbolo))
                        prefix = monedaSimbolo + " ";
                    newBefore = before.Substring(0, m.Index) + prefix;
                }
                else
                {
                    newBefore = before;
                    if (!string.IsNullOrEmpty(monedaSimbolo))
                        newBefore = newBefore + monedaSimbolo + " ";
                }
            }
            else
            {
                newBefore = before;
                if (!string.IsNullOrEmpty(monedaSimbolo))
                    newBefore = newBefore + monedaSimbolo + " ";
            }

            string newText = newBefore + formattedNumeric + after;

            lbl.Text = newText;
        }

        // =========================================================
        // GESTIÓN DE SUBTOTALES (GRUPO "gbMontos")
        // =========================================================
        #region Subtotales

        private void ConfigurarGridSubtotales()
        {
            // Ya no se usa DataGridView, inicializamos estado del FlowLayoutPanel si es necesario
            flpSubtotales.Controls.Clear();
        }

        private void BtnAgregarSubtotal_Click(object sender, EventArgs e)
        {
            AbrirInputSubtotal();
        }

        private void BtnConfigOtros_Click(object sender, EventArgs e)
        {
            using (var form = new FormConfigOtrosImpuestos(otrosImpuestosNombre, otrosImpuestosSumar))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    otrosImpuestosNombre = form.NombreImpuesto;
                    otrosImpuestosSumar = form.SumarAlTotal;

                    // Actualizar el label (asegurar el sufijo :)
                    string sufijo = otrosImpuestosNombre.EndsWith(":") ? "" : ":";
                    lblOtrosImpuestos.Text = otrosImpuestosNombre + sufijo;

                    // Recalcular para reflejar el cambio de signo si ya hay un monto
                    RecalcularTodo();
                }
            }
        }

        private void AbrirInputSubtotal()
        {
            using (var form = new FormInputSubtotal())
            {
                var result = form.ShowDialog(this);

                if (result == DialogResult.OK)
                {
                    AgregarMontoGrid(form.MontoIngresado);

                    if (form.AgregarOtro)
                    {
                        // Llamada recursiva (o bucle) para agregar otro inmediatamente
                        AbrirInputSubtotal();
                    }
                }
            }
        }

        private void AgregarMontoGrid(decimal monto)
        {
            string simbolo = !string.IsNullOrEmpty(monedaSimbolo) ? monedaSimbolo : "RD$";
            var nfi = monedaNumberFormat ?? BuildNumberFormat();
            
            SubtotalItemControl item = new SubtotalItemControl(monto, simbolo, nfi);
            
            // Suscribir al evento de eliminar
            item.RemoveRequested += (s, e) =>
            {
                if (MessageBox.Show("¿Eliminar este subtotal?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    flpSubtotales.Controls.Remove(item);
                    item.Dispose();
                    ActualizarTotalSubtotales();
                }
            };

            flpSubtotales.Controls.Add(item);
            
            // Hacer scroll al final para que se vea el nuevo elemento
            flpSubtotales.ScrollControlIntoView(item);
            
            ActualizarTotalSubtotales();
        }

        private void ActualizarTotalSubtotales()
        {
            decimal total = 0m;
            int count = 0;
            
            foreach (Control ctrl in flpSubtotales.Controls)
            {
                if (ctrl is SubtotalItemControl item)
                {
                    total += item.Monto;
                    count++;
                }
            }

            // Ajustar padding según la cantidad de elementos para centrado visual
            if (count >= 2)
                flpSubtotales.Padding = new Padding(60, 4, 4, 4);
            else
                flpSubtotales.Padding = new Padding(4, 4, 4, 4);

            // Actualizar Label con formato de moneda dinámico
            string simbolo = !string.IsNullOrEmpty(monedaSimbolo) ? monedaSimbolo : "RD$";
            var nfi = monedaNumberFormat ?? BuildNumberFormat();
            string textoTotal = $"{simbolo} {total.ToString("N2", nfi)}";

            lblSubtotalTotal.Text = $"SUBTOTAL: {textoTotal}";
            
            // Si cambia el subtotal, deben actualizarse AFP y SFS si están en automático
            ActualizarRetencionesLey();
            RecalcularTodo();
        }

        private void ActualizarFormatoGridSubtotales()
        {
            string simbolo = !string.IsNullOrEmpty(monedaSimbolo) ? monedaSimbolo : "RD$";
            var nfi = monedaNumberFormat ?? BuildNumberFormat();

            foreach (Control ctrl in flpSubtotales.Controls)
            {
                if (ctrl is SubtotalItemControl item)
                {
                    item.ActualizarFormato(simbolo, nfi);
                }
            }
            
            ActualizarTotalSubtotales();
        }

        #endregion

        // =========================================================
        // CARGA DE DATOS INICIALES
        // =========================================================
        private void CargarDatosIniciales()
        {
            try
            {
                CargarTiposPago();
                CargarMonedas();
                CargarTiposComprobante();
                CargarTiposNCF();
                CargarPorcentajesITBIS();
                CargarPorcentajesRetenciones();

                CargarFideicomisos();
                CargarProveedores();

                ConfigurarAutocompletadoFideicomiso();
                ConfigurarAutocompletadoProveedor();

                GenerarNumeroSolicitud();

                SeleccionarValoresPorDefecto();

                UpdateAllCurrencyDisplays();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos iniciales: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // SELECCIÓN DE VALORES POR DEFECTO
        // =========================================================
        private void SeleccionarValoresPorDefecto()
        {
            foreach (DataRowView item in cboMoneda.Items)
            {
                if (item["CodigoISO"].ToString() == "DOP")
                {
                    cboMoneda.SelectedItem = item;
                    break;
                }
            }

            if (cboTipoPago.Items.Count > 0)
            {
                foreach (DataRowView item in cboTipoPago.Items)
                {
                    if (item["Nombre"].ToString().ToUpper().Contains("TRANSFERENCIA"))
                    {
                        cboTipoPago.SelectedItem = item;
                        break;
                    }
                }
            }

            foreach (var item in cboITBISPorcentaje.Items)
            {
                if (item.ToString() == "18%")
                {
                    cboITBISPorcentaje.SelectedItem = item;
                    break;
                }
            }

            txtTasa.Visible = false;
            lblTasa.Visible = false;

            if (chkCalcularITBIS != null)
            {
                chkCalcularITBIS.Checked = true;
            }

            if (chkIncluirFirma != null && cboFirma != null)
            {
                cboFirma.Enabled = chkIncluirFirma.Checked;
            }

            SetNumeroNCFPlaceholder();

            UpdateEstadoCalculoITBIS();
            ActualizarEstadoITBISManual();

            if (chkRetSFS != null)
            {
                chkRetSFS.Checked = false;
                txtRetSFS.Enabled = chkRetSFS.Checked;
                txtRetSFS.BackColor = SystemColors.Control;
            }

            if (chkRetAFP != null)
            {
                chkRetAFP.Checked = false;
                txtRetAFP.Enabled = chkRetAFP.Checked;
                txtRetAFP.BackColor = SystemColors.Control;
            }
        }

        // =========================================================
        // GENERAR NÚMERO DE SOLICITUD
        // =========================================================
        private void GenerarNumeroSolicitud()
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT NEXT VALUE FOR SEQ_SolicitudPago";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        int siguiente = Convert.ToInt32(cmd.ExecuteScalar());
                        lblNumeroSolicitud.Text = $"SP-{siguiente:D6}";
                    }
                }
            }
            catch (Exception ex)
            {
                lblNumeroSolicitud.Text = "SP-NUEVO";
                System.Diagnostics.Debug.WriteLine($"Error generando número: {ex.Message}");
            }
        }

        // =========================================================
        // CARGA DE COMBOS
        // =========================================================
        #region Carga de Combos

        private void CargarTiposPago()
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT TipoPagoID, Codigo, Nombre 
                                     FROM TiposPago 
                                     WHERE Activo = 1 
                                     ORDER BY Nombre";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cboTipoPago.DataSource = dt;
                    cboTipoPago.DisplayMember = "Nombre";
                    cboTipoPago.ValueMember = "TipoPagoID";
                    cboTipoPago.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando tipos de pago: {ex.Message}");
            }
        }

        private void CargarMonedas()
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT MonedaID, CodigoISO, Simbolo, Nombre, EsLocal 
                                     FROM Monedas 
                                     WHERE Activo = 1 
                                     ORDER BY EsLocal DESC, Nombre";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cboMoneda.DataSource = dt;
                    cboMoneda.DisplayMember = "Nombre";
                    cboMoneda.ValueMember = "MonedaID";
                    cboMoneda.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando monedas: {ex.Message}");
            }
        }

        private void CargarTiposComprobante()
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT TipoComprobanteID, Codigo, Nombre, RequiereNCF 
                                     FROM TiposComprobante 
                                     WHERE Activo = 1 
                                     ORDER BY Nombre";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cboTipoComprobante.DataSource = dt;
                    cboTipoComprobante.DisplayMember = "Nombre";
                    cboTipoComprobante.ValueMember = "TipoComprobanteID";
                    cboTipoComprobante.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando tipos de comprobante: {ex.Message}");
            }
        }

        private void CargarTiposNCF()
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT TipoNCFID, Codigo, NombreCorto, Serie, EsElectronico 
                                     FROM TiposNCF 
                                     WHERE Activo = 1 
                                     ORDER BY Serie, Codigo";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dt.Columns.Add("Display", typeof(string));
                    foreach (DataRow row in dt.Rows)
                    {
                        row["Display"] = $"{row["Codigo"]} - {row["NombreCorto"]}";
                    }

                    cboTipoNCF.DataSource = dt;
                    cboTipoNCF.DisplayMember = "Display";
                    cboTipoNCF.ValueMember = "TipoNCFID";
                    cboTipoNCF.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando tipos NCF: {ex.Message}");
            }
        }

        private void CargarPorcentajesITBIS()
        {
            cboITBISPorcentaje.Items.Clear();
            cboITBISPorcentaje.Items.Add("0%");
            cboITBISPorcentaje.Items.Add("16%");
            cboITBISPorcentaje.Items.Add("18%");
            cboITBISPorcentaje.SelectedIndex = -1;
        }

        private void CargarPorcentajesRetenciones()
        {
            cboRetITBIS.Items.Clear();
            cboRetITBIS.Items.Add("0%");
            cboRetITBIS.Items.Add("30%");
            cboRetITBIS.Items.Add("100%");
            cboRetITBIS.SelectedIndex = 0;

            cboRetISR.Items.Clear();
            cboRetISR.Items.Add("0%");
            cboRetISR.Items.Add("2%");
            cboRetISR.Items.Add("10%");
            cboRetISR.Items.Add("27%");
            cboRetISR.SelectedIndex = 0;
        }

        #endregion

        // =========================================================
        // AUTOCOMPLETADO FIDEICOMISO
        // =========================================================
        #region Autocompletado Fideicomiso

        private string FormatearRNC(string rnc)
        {
            if (string.IsNullOrEmpty(rnc)) return "";

            string limpio = rnc.Replace("-", "").Trim();

            // RNC de Empresa (9 dígitos)
            if (limpio.Length == 9)
                return Convert.ToInt64(limpio).ToString("0-00-00000-0");

            // Cédula / Persona Física (11 dígitos)
            if (limpio.Length == 11)
                return Convert.ToInt64(limpio).ToString("000-0000000-0");

            return rnc; // Retorna original si no cumple formato
        }

        private void CargarFideicomisos()
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT FideicomisoID, Codigo, Nombre, RNC 
                                     FROM Fideicomisos 
                                     WHERE Activo = 1 AND EsEliminado = 0
                                     ORDER BY Nombre";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    dtFideicomisos = new DataTable();
                    da.Fill(dtFideicomisos);
                }

                suppressFideicomisoEvents = true;
                cboFideicomiso.DataSource = null;
                cboFideicomiso.DisplayMember = "Nombre";
                cboFideicomiso.ValueMember = "FideicomisoID";
                cboFideicomiso.DataSource = dtFideicomisos;
                cboFideicomiso.SelectedIndex = -1;
                txtCodigoFideicomiso.Text = string.Empty;
                lblRNCFideicomiso.Text = "RNC: ---";
                suppressFideicomisoEvents = false;

                // opcional: asegurar que txtCodigoFideicomiso comienza vacío
                txtCodigoFideicomiso.Text = string.Empty;
                lblRNCFideicomiso.Text = "RNC: ---";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando fideicomisos: {ex.Message}");
                dtFideicomisos = new DataTable();
            }
        }

        private void ConfigurarAutocompletadoFideicomiso()
        {
            cboFideicomiso.DataSource = dtFideicomisos;
            cboFideicomiso.DisplayMember = "Nombre";
            cboFideicomiso.ValueMember = "FideicomisoID";
            cboFideicomiso.SelectedIndex = -1;

            cboFideicomiso.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboFideicomiso.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void TxtCodigoFideicomiso_Leave(object sender, EventArgs e)
        {
            // Buscar fideicomiso por código cuando el usuario sale del campo
            string codigo = txtCodigoFideicomiso.Text.Trim();

            if (string.IsNullOrEmpty(codigo))
            {
                return;
            }

            // Buscar en el DataTable
            DataRow[] rows = dtFideicomisos.Select($"Codigo = '{codigo}'");

            if (rows.Length > 0)
            {
                // Encontrado - seleccionar en el combo
                int fideicomisoID = Convert.ToInt32(rows[0]["FideicomisoID"]);
                cboFideicomiso.SelectedValue = fideicomisoID;

                // CAMBIO APLICADO AQUÍ: Formatear RNC
                string rncRaw = rows[0]["RNC"].ToString();
                lblRNCFideicomiso.Text = $"RNC: {FormatearRNC(rncRaw)}";
            }
            else
            {
                // No encontrado
                lblRNCFideicomiso.Text = "RNC: No encontrado";
                cboFideicomiso.SelectedIndex = -1;
            }
        }

        private void TxtCodigoFideicomiso_TextChanged(object sender, EventArgs e)
        {
            if (suppressFideicomisoEvents) return;
            if (dtFideicomisos == null || dtFideicomisos.Rows.Count == 0) return;

            string codigo = txtCodigoFideicomiso.Text.Trim();
            if (string.IsNullOrEmpty(codigo))
            {
                suppressFideicomisoEvents = true;
                cboFideicomiso.SelectedIndex = -1;
                lblRNCFideicomiso.Text = "RNC: ---";
                suppressFideicomisoEvents = false;
                return;
            }

            // Buscar solo coincidencia exacta (no prefijos)
            string esc = codigo.Replace("'", "''");
            DataRow[] rows = dtFideicomisos.Select($"Codigo = '{esc}'");

            if (rows.Length > 0)
            {
                int fideicomisoID = Convert.ToInt32(rows[0]["FideicomisoID"]);

                // seleccionar por índice (seguro en cualquier momento del binding)
                int indexToSelect = -1;
                for (int i = 0; i < cboFideicomiso.Items.Count; i++)
                {
                    if (cboFideicomiso.Items[i] is DataRowView drv && Convert.ToInt32(drv["FideicomisoID"]) == fideicomisoID)
                    {
                        indexToSelect = i;
                        break;
                    }
                }

                if (indexToSelect >= 0)
                {
                    suppressFideicomisoEvents = true;
                    cboFideicomiso.SelectedIndex = indexToSelect;
                    suppressFideicomisoEvents = false;
                }

                string rncRaw = rows[0]["RNC"]?.ToString() ?? string.Empty;
                lblRNCFideicomiso.Text = string.IsNullOrEmpty(rncRaw) ? "RNC: ---" : $"RNC: {FormatearRNC(rncRaw)}";
            }
            else
            {
                lblRNCFideicomiso.Text = "RNC: No encontrado";
                suppressFideicomisoEvents = true;
                cboFideicomiso.SelectedIndex = -1;
                suppressFideicomisoEvents = false;
            }
        }

        private void CboFideicomiso_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (suppressFideicomisoEvents) return;

            if (cboFideicomiso.SelectedIndex >= 0 && cboFideicomiso.SelectedValue != null)
            {
                DataRowView drv = cboFideicomiso.SelectedItem as DataRowView;
                if (drv != null)
                {
                    // Solo actualizar txtCodigoFideicomiso si el combo tiene el foco (usuario seleccionó)
                    if (cboFideicomiso.Focused)
                    {
                        txtCodigoFideicomiso.Text = drv["Codigo"].ToString();
                    }

                    string rnc = drv["RNC"].ToString();
                    lblRNCFideicomiso.Text = string.IsNullOrEmpty(rnc) ? "RNC: ---" : $"RNC: {FormatearRNC(rnc)}";
                }
            }
            else
            {
                lblRNCFideicomiso.Text = "RNC: ---";
            }
        }

        #endregion

        // =========================================================
        // AUTOCOMPLETADO PROVEEDOR
        // =========================================================
        #region Autocompletado Proveedor

        private void CargarProveedores()
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT ProveedorID, Nombre, TipoDocumento, NumeroDocumento, Telefono 
                                     FROM Proveedores 
                                     WHERE Activo = 1 AND EsEliminado = 0
                                     ORDER BY Nombre";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    dtProveedores = new DataTable();
                    da.Fill(dtProveedores);

                    dtProveedores.Columns.Add("NumeroDocumentoClean", typeof(string));
                    foreach (DataRow r in dtProveedores.Rows)
                    {
                        var raw = (r["NumeroDocumento"] ?? string.Empty).ToString();
                        r["NumeroDocumentoClean"] = System.Text.RegularExpressions.Regex.Replace(raw, @"\D", string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando proveedores: {ex.Message}");
                dtProveedores = new DataTable();
            }
        }

        private void ConfigurarAutocompletadoProveedor()
        {
            cboProveedor.DataSource = dtProveedores;
            cboProveedor.DisplayMember = "Nombre";
            cboProveedor.ValueMember = "ProveedorID";
            cboProveedor.SelectedIndex = -1;

            cboProveedor.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboProveedor.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void TxtRNCProveedor_Leave(object sender, EventArgs e)
        {
            // Buscar proveedor por RNC/Cédula cuando el usuario sale del campo
            if (txtRNCProveedor == null)
                return;

            // Obtener solo dígitos para búsqueda
            string documento = Regex.Replace(txtRNCProveedor.Text ?? string.Empty, @"\D", string.Empty);

            if (string.IsNullOrEmpty(documento))
            {
                // Restaurar campo vacío si nada ingresado
                txtRNCProveedor.Text = string.Empty;
                return;
            }

            // Buscar en el DataTable (sin guiones para comparar)
                            DataRow[] rows = dtProveedores.Select($"NumeroDocumentoClean = '{documento}'");

            // Si no encuentra, intentar búsqueda directa por el campo tal cual
            if (rows.Length == 0)
            {
                rows = dtProveedores.Select($"NumeroDocumento = '{txtRNCProveedor.Text.Trim()}'");
            }                   

            if (rows.Length > 0)
            {
                // Encontrado - seleccionar en el combo
                int proveedorID = Convert.ToInt32(rows[0]["ProveedorID"]);
                cboProveedor.SelectedValue = proveedorID;

                // Mostrar teléfono
                string telefono = rows[0]["Telefono"]?.ToString();
                lblTelefonoProveedor.Text = string.IsNullOrEmpty(telefono) ? "Tel: ---" : $"Tel: {FormatPhone(telefono)}";
            }
            else
            {
                // No encontrado
                lblTelefonoProveedor.Text = "Tel: No encontrado";
                cboProveedor.SelectedIndex = -1;
            }

            // Aplicar formato visual apropiado (guiones) según cantidad de dígitos
            txtRNCProveedor.Text = FormatRncDisplay(documento);
        }

        private void TxtRNCProveedor_TextChanged(object sender, EventArgs e)
        {
            if (suppressProveedorEvents) return;
            if (dtProveedores == null || dtProveedores.Rows.Count == 0) return;

            string documento = Regex.Replace(txtRNCProveedor.Text ?? string.Empty, @"\D", string.Empty);

            if (string.IsNullOrEmpty(documento))
            {
                suppressProveedorEvents = true;
                cboProveedor.SelectedIndex = -1;
                lblTelefonoProveedor.Text = "Tel: ---";
                suppressProveedorEvents = false;
                return;
            }

            // Buscar solo coincidencia exacta (NumeroDocumentoClean)
            DataRow[] rows = dtProveedores.Select($"NumeroDocumentoClean = '{documento}'");

            if (rows.Length > 0)
            {
                int proveedorID = Convert.ToInt32(rows[0]["ProveedorID"]);

                // seleccionar por índice (evita dependencia de ValueMember en momentos de rebinding)
                int indexToSelect = -1;
                for (int i = 0; i < cboProveedor.Items.Count; i++)
                {
                    if (cboProveedor.Items[i] is DataRowView drv && Convert.ToInt32(drv["ProveedorID"]) == proveedorID)
                    {
                        indexToSelect = i;
                        break;
                    }
                }

                if (indexToSelect >= 0)
                {
                    suppressProveedorEvents = true;
                    cboProveedor.SelectedIndex = indexToSelect;
                    suppressProveedorEvents = false;
                }

                string telefono = rows[0]["Telefono"]?.ToString();
                lblTelefonoProveedor.Text = string.IsNullOrEmpty(telefono) ? "Tel: ---" : $"Tel: {FormatPhone(telefono)}";
            }
            else
            {
                lblTelefonoProveedor.Text = "Tel: No encontrado";
                suppressProveedorEvents = true;
                cboProveedor.SelectedIndex = -1;
                suppressProveedorEvents = false;
            }
        }

        private void CboProveedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (suppressProveedorEvents) return;

            if (cboProveedor.SelectedIndex >= 0 && cboProveedor.SelectedValue != null)
            {
                DataRowView drv = cboProveedor.SelectedItem as DataRowView;
                if (drv != null)
                {
                    // Solo actualizar txtRNCProveedor si el combo tiene el foco (usuario seleccionó)
                    if (cboProveedor.Focused)
                    {
                        string numeroRaw = drv["NumeroDocumento"]?.ToString() ?? string.Empty;
                        txtRNCProveedor.Text = FormatRncDisplay(numeroRaw);
                    }

                    // Actualizar teléfono
                    string telefono = drv["Telefono"]?.ToString();
                    lblTelefonoProveedor.Text = string.IsNullOrEmpty(telefono) ? "Tel: ---" : $"Tel: {FormatPhone(telefono)}";
                }
            }
            else
            {
                lblTelefonoProveedor.Text = "Tel: ---";
            }
        }

        #endregion

        // =========================================================
        // EVENTOS DE CONTROLES
        // =========================================================
        #region Eventos de Controles

        private void CboMoneda_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMoneda.SelectedIndex >= 0 && cboMoneda.SelectedValue != null)
            {
                DataRowView drv = cboMoneda.SelectedItem as DataRowView;

                if (drv != null)
                {
                    bool esLocal = Convert.ToBoolean(drv["EsLocal"]);

                    // Mostrar/ocultar campos de tasa de cambio
                    lblTasa.Visible = !esLocal;
                    txtTasa.Visible = !esLocal;
                    chkMostrarConversion.Visible = !esLocal;
                    btnConfigConversion.Visible = !esLocal;

                    // Si no es local, habilitar configuración de conversión
                    if (chkMostrarConversion != null)
                    {
                        chkMostrarConversion.Enabled = !esLocal;
                        btnConfigConversion.Enabled = !esLocal;
                        chkMostrarConversion.Visible = !esLocal;
                        btnConfigConversion.Visible = !esLocal;
                    }

                    if (!esLocal)
                    {
                        txtTasa.Focus();
                    }
                }

                // Reaplicar formatos según la nueva moneda
                UpdateAllCurrencyDisplays();
            }
        }

        /// <summary>
        /// Abre el miniformulario de selección de método de conversión.
        /// Requiere que txtTasa tenga un valor > 0.
        /// </summary>
        private void BtnConfigConversion_Click(object sender, EventArgs e)
        {
            // Validar que exista una tasa válida
            decimal tasa = 0m;
            string textoTasa = txtTasa.Text.Trim().Replace(",", "").Replace(" ", "");
            decimal.TryParse(textoTasa, NumberStyles.Any, CultureInfo.InvariantCulture, out tasa);

            if (tasa <= 0m)
            {
                MessageBox.Show(
                    "Debe ingresar una tasa de cambio mayor a 0 antes de configurar el método de conversión.",
                    "Tasa Requerida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                txtTasa.Focus();
                return;
            }

            // Abrir miniformulario de selección
            using (var formConversion = new FormMetodoConversion(metodoConversionSeleccionado))
            {
                if (formConversion.ShowDialog(this) == DialogResult.OK)
                {
                    // Guardar el método seleccionado
                    metodoConversionSeleccionado = formConversion.MetodoSeleccionado;
                    metodoConversionNombre = formConversion.NombreMetodo;

                    // Actualizar el checkbox para indicar que hay método seleccionado
                    if (metodoConversionSeleccionado.HasValue)
                    {
                        chkMostrarConversion.Checked = true;
                        chkMostrarConversion.Text = $"{metodoConversionNombre}";
                    }
                }
            }
        }

        /// <summary>
        /// Cuando el usuario desmarca el checkbox, se limpia la selección del método de conversión.
        /// Se bloquea el marcado manual si no hay un método preseleccionado.
        /// </summary>
        private void ChkMostrarConversion_CheckedChanged(object sender, EventArgs e)
        {
            // Evitar marcado manual si no se ha seleccionado un método mediante el diálogo de configuración
            if (chkMostrarConversion.Checked && !metodoConversionSeleccionado.HasValue)
            {
                // Si intenta marcarse sin método, lo revertimos
                // No mostramos mensaje para evitar interrumpir el flujo del usuario si fue un click accidental
                chkMostrarConversion.Checked = false;
                return;
            }

            if (!chkMostrarConversion.Checked)
            {
                // El usuario desmarcó manualmente => limpiar método seleccionado
                metodoConversionSeleccionado = null;
                metodoConversionNombre = string.Empty;
                chkMostrarConversion.Text = "Cambiar Conversión:";
            }
        }

        private void TxtConcepto_TextChanged(object sender, EventArgs e)
        {
            // Actualizar contador de caracteres
            int longitud = txtConcepto.Text.Length;
            int maximo = 2000;

            lblContadorConcepto.Text = $"{longitud} / {maximo} caracteres";

            // Cambiar color si está cerca del límite
            if (longitud > maximo * 0.9)
            {
                lblContadorConcepto.ForeColor = Color.Red;
            }
            else if (longitud > maximo * 0.75)
            {
                lblContadorConcepto.ForeColor = Color.Orange;
            }
            else
            {
                lblContadorConcepto.ForeColor = Color.Gray;
            }
        }

        private void TxtObservaciones_TextChanged(object sender, EventArgs e)
        {
            // Actualizar contador de caracteres
            int longitud = txtObservaciones.Text.Length;
            int maximo = 1000;

            lblContadorObservaciones.Text = $"{longitud} / {maximo} caracteres";

            // Cambiar color si está cerca del límite
            if (longitud > maximo * 0.9)
            {
                lblContadorObservaciones.ForeColor = Color.Red;
            }
            else if (longitud > maximo * 0.75)
            {
                lblContadorObservaciones.ForeColor = Color.Orange;
            }
            else
            {
                lblContadorObservaciones.ForeColor = Color.Gray;
            }
        }
        private void ChkITBISManual_CheckedChanged(object sender, EventArgs e)
        {
            ActualizarEstadoITBISManual();
        }

        // Handler para el CheckBox maestro (activar/desactivar calculo automático)
        private void ChkCalcularITBIS_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEstadoCalculoITBIS();
        }

        // Actualiza habilitación/visibilidad de los controles de cálculo automático y del flujo manual
        private void UpdateEstadoCalculoITBIS()
        {
            bool calcular = chkCalcularITBIS != null && chkCalcularITBIS.Checked;

            // Controles del cálculo automático
            if (lblITBIS != null) lblITBIS.Enabled = calcular;
            if (cboITBISPorcentaje != null) cboITBISPorcentaje.Enabled = calcular;
            if (rbBaseSubtotal != null) rbBaseSubtotal.Enabled = calcular;
            if (rbBaseDirTec != null) rbBaseDirTec.Enabled = calcular;
            if (lblITBISCalc != null) lblITBISCalc.Enabled = calcular;
            if (lblITBISCalculado != null) lblITBISCalculado.Enabled = calcular;

            // Si no se calcula ITBIS automáticamente, no permitimos entrada manual
            if (chkITBISManual != null)
            {
                // Desactivar el checkbox manual si no se puede calcular
                chkITBISManual.Enabled = calcular;

                // Cambiar color según habilitado
                chkITBISManual.ForeColor = calcular ? Color.FromArgb(64, 64, 64) : Color.FromArgb(120, 120, 120);

                // Si deshabilitamos el manual, limpiamos su estado y los controles manuales asociados
                if (!calcular)
                {
                    chkITBISManual.Checked = false;
                    // Limpiar también el valor manual
                    if (txtITBISManual != null) txtITBISManual.Text = string.Empty;
                }
            }

            // Asegurar coherencia del estado manual (habilitar/deshabilitar controles manuales)
            ActualizarEstadoITBISManual();
        }

        // Actualiza habilitación/visibilidad de controles relacionados con ITBIS manual
        private void ActualizarEstadoITBISManual()
        {
            bool manualChecked = chkITBISManual != null && chkITBISManual.Checked;
            // Nota: chkITBISManual.Enabled ya se gestiona por UpdateEstadoCalculoITBIS

            // Habilitar / deshabilitar input manual
            if (txtITBISManual != null) 
            {
                txtITBISManual.Enabled = manualChecked;
                // Si se deshabilita, limpiar
                if (!manualChecked)
                    txtITBISManual.Text = string.Empty;
            }

            // Mostrar / ocultar diferencia
            if (lblDif != null) lblDif.Visible = manualChecked;
            if (lblITBISDiferencia != null) lblITBISDiferencia.Visible = manualChecked;

            // Ajuste visual del texto del CheckBox manual (si se quiere resaltar cuando está activo)
            if (chkITBISManual != null)
            {
                if (!chkITBISManual.Enabled)
                {
                    // ya gris en UpdateEstadoCalculoITBIS, pero reforzamos
                    chkITBISManual.ForeColor = Color.FromArgb(120, 120, 120);
                }
                else
                {
                    // Si está habilitado, color estándar; opcionalmente cambiar si está marcado
                    chkITBISManual.ForeColor = manualChecked ? Color.FromArgb(0, 120, 212) : Color.FromArgb(64, 64, 64);
                }
            }
        }

        // Hacer clic sobre la etiqueta alterna el CheckBox
        private void LblITBISMan_Click(object sender, EventArgs e)
        {
            if (chkITBISManual == null)
                return;

            // Alternar el estado Checked del CheckBox siempre que exista.
            chkITBISManual.Checked = !chkITBISManual.Checked;
        }

        // Habilita o deshabilita el ComboBox de firma según el estado del CheckBox
        private void ChkIncluirFirma_CheckedChanged(object sender, EventArgs e)
        {
            // Si el control no existe, no hacer nada
            if (cboFirma == null || chkIncluirFirma == null)
                return;

            cboFirma.Enabled = chkIncluirFirma.Checked;
        }

        private void CboTipoNCF_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Actualizar placeholder cada vez que cambia el tipo de NCF seleccionado
            // Pero solo mostrar la advertencia si la acción fue iniciada por el usuario (control con foco),
            // para evitar mensajes al cargar/cargar datos programáticamente.
            try
            {
                bool tipoComprobanteEsNCF = false;
                if (cboTipoComprobante != null && cboTipoComprobante.SelectedItem is System.Data.DataRowView drvTipo)
                {
                    if (drvTipo.Row.Table.Columns.Contains("RequiereNCF"))
                        tipoComprobanteEsNCF = Convert.ToBoolean(drvTipo["RequiereNCF"]);
                    if (!tipoComprobanteEsNCF && drvTipo["Nombre"]?.ToString().Trim().Equals("NCF", StringComparison.OrdinalIgnoreCase) == true)
                        tipoComprobanteEsNCF = true;
                }

                // Solo advertir si el usuario realmente interactuó con cboTipoNCF (tiene foco)
                if (!tipoComprobanteEsNCF && cboTipoNCF != null && cboTipoNCF.SelectedIndex >= 0 && cboTipoNCF.Focused)
                {
                    MessageBox.Show("Nota: ha seleccionado un tipo de NCF, pero el tipo de comprobante actual no es 'NCF'. El tipo de NCF seleccionado no se aplicará automáticamente. Aún puede ingresar un número o referencia en el campo y agregar el comprobante.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                // silencioso en caso de error
            }

            SetNumeroNCFPlaceholder();
        }

        private void TxtNumeroNCF_Enter(object sender, EventArgs e)
        {
            // Si el placeholder está activo, limpiarlo para que el usuario escriba
            if (numeroNCFPlaceholderActivo && txtNumeroNCF != null)
            {
                txtNumeroNCF.Text = string.Empty;
                txtNumeroNCF.ForeColor = Color.FromArgb(64, 64, 64);
                numeroNCFPlaceholderActivo = false;
            }
        }

        private void TxtNumeroNCF_Leave(object sender, EventArgs e)
        {
            // Si el usuario no ingresó nada, restaurar placeholder
            if (txtNumeroNCF == null)
                return;

            if (string.IsNullOrWhiteSpace(txtNumeroNCF.Text))
            {
                SetNumeroNCFPlaceholder();
            }
            else
            {
                // Opcional: validar longitud mínima/formato aquí o marcar visualmente si no coincide
                // Por ahora dejamos el valor ingresado por el usuario.
            }
        }

        private void SetNumeroNCFPlaceholder()
        {
            if (txtNumeroNCF == null || cboTipoNCF == null)
                return;

            // Obtener prefijo (ej. "B01" o "E31") a partir del item seleccionado
            string prefix = GetNCFPrefixFromSelectedTipo(); // devuelve null si no hay selección válida
            int expectedLength = GetNCFExpectedLength();    // 11 o 13 por defecto

            if (string.IsNullOrEmpty(prefix) || expectedLength <= prefix.Length)
            {
                // Fallback genérico: mostrar ejemplo con 11 caracteres si no se puede determinar
                prefix = "B01";
                expectedLength = 11;
            }

            int zerosCount = expectedLength - prefix.Length;
            if (zerosCount < 0) zerosCount = 0;

            string zeros = new string('0', zerosCount);

            // Sin espacio: "PREFIX000..." para que el usuario no piense que debe dejar un espacio
            string placeholder = $"{prefix}{zeros}";

            // Aplicar placeholder visualmente
            txtNumeroNCF.Text = placeholder;
            txtNumeroNCF.ForeColor = Color.Gray;
            numeroNCFPlaceholderActivo = true;
        }

        private int GetNCFExpectedLength()
        {
            try
            {
                if (cboTipoNCF.SelectedItem is DataRowView drv)
                {
                    var serieObj = drv["Serie"];
                    if (serieObj != null)
                    {
                        string serie = serieObj.ToString();
                        if (serie.Equals("E", StringComparison.OrdinalIgnoreCase))
                            return 13;
                        if (serie.Equals("B", StringComparison.OrdinalIgnoreCase))
                            return 11;
                    }
                }
            }
            catch
            {
            }

            return 11;
        }

        // Reemplaza el método GetNCFPrefixFromSelectedTipo por esta versión (evita duplicar la serie)
        private string GetNCFPrefixFromSelectedTipo()
        {
            try
            {
                if (cboTipoNCF.SelectedItem is DataRowView drv)
                {
                    string serie = (drv["Serie"] ?? string.Empty).ToString().Trim().ToUpper(); // "B" o "E"
                    string codigo = (drv["Codigo"] ?? string.Empty).ToString().Trim();        // "01", "31" o a veces "B01"

                    if (string.IsNullOrEmpty(codigo) && string.IsNullOrEmpty(serie))
                        return null;

                    // Si el codigo ya incluye la serie (p. ej. "B01" o "E31") devolverlo normalizado
                    if (!string.IsNullOrEmpty(codigo) && !string.IsNullOrEmpty(serie) &&
                        codigo.StartsWith(serie, StringComparison.OrdinalIgnoreCase))
                    {
                        return codigo.ToUpper();
                    }

                    // Normalizar codigo a 2 dígitos
                    if (!string.IsNullOrEmpty(codigo))
                        codigo = codigo.PadLeft(2, '0');

                    // Construir prefijo: si hay serie la anteponemos, si no devolvemos solo el codigo
                    if (!string.IsNullOrEmpty(serie))
                        return (serie + codigo).ToUpper();

                    return codigo.ToUpper();
                }
            }
            catch
            {
            }

            return null;
        }

        private void ChkRetSFS_CheckedChanged(object sender, EventArgs e)
        {
            if (txtRetSFS == null || chkRetSFS == null)
                return;

            // Habilita o deshabilita el TextBox según el estado del CheckBox
            txtRetSFS.Enabled = chkRetSFS.Checked;

            // Opcional: ajustar color para indicar estado
            txtRetSFS.BackColor = txtRetSFS.Enabled ? Color.White : SystemColors.Control;
            
            // Si se deshabilita, limpiar
            if (!chkRetSFS.Checked)
            {
                txtRetSFS.Text = string.Empty;
                // No llamamos RecalcularTodo aquí excesivamente porque UpdateRetencionesLey puede llamarlo o el TextChanged
                // Pero es mejor asegurarse
                if (!isRecalculating) RecalcularTodo();
            }
        }

        private void ChkRetAFP_CheckedChanged(object sender, EventArgs e)
        {
            if (txtRetAFP == null || chkRetAFP == null)
                return;

            // Habilita o deshabilita el TextBox según el estado del CheckBox
            txtRetAFP.Enabled = chkRetAFP.Checked;

            // Opcional: ajustar color para indicar estado
            txtRetAFP.BackColor = txtRetAFP.Enabled ? Color.White : SystemColors.Control;
            
            // Si se deshabilita, limpiar
            if (!chkRetAFP.Checked)
            {
                txtRetAFP.Text = string.Empty;
                if (!isRecalculating) RecalcularTodo();
            }
        }

        // Validador para enteros (solo dígitos)
        private void NumericInteger_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir control (backspace, etc.) y dígitos únicamente
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // Validador para decimales (dígitos y un único punto '.')
        private void NumericDecimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            var tb = sender as TextBox;

            if (char.IsControl(e.KeyChar))
                return;

            if (char.IsDigit(e.KeyChar))
                return;

            if (e.KeyChar == '.')
            {
                // permitir un solo punto decimal
                if (tb != null && tb.Text.IndexOf('.') >= 0)
                    e.Handled = true;
                return;
            }

            // cualquier otro carácter se bloquea
            e.Handled = true;
        }

        // Sanitizador para enteros: elimina cualquier caracter no dígito (protege contra pegado)
        private void SanitizeIntegerTextBox(TextBox tb)
        {
            if (tb == null) return;

            string original = tb.Text;
            string cleaned = string.Empty;
            foreach (char c in original)
            {
                if (char.IsDigit(c)) cleaned += c;
            }

            if (cleaned != original)
            {
                int sel = cleaned.Length;
                tb.Text = cleaned;
                tb.SelectionStart = sel;
            }
        }

        // Sanitizador para decimales: mantiene dígitos y a lo sumo un punto; corrige posiciones
        private void SanitizeDecimalTextBox(TextBox tb)
        {
            if (tb == null) return;

            string original = tb.Text;
            if (string.IsNullOrEmpty(original)) return;

            char decimalSep = '.';
            var sb = new System.Text.StringBuilder();
            bool seenDecimal = false;

            foreach (char c in original)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                    continue;
                }

                if (c == decimalSep && !seenDecimal)
                {
                    // si el punto es el primer carácter, prefijar "0"
                    if (sb.Length == 0)
                        sb.Append('0');

                    sb.Append(decimalSep);
                    seenDecimal = true;
                    continue;
                }

                // ignorar cualquier otro carácter
            }

            string cleaned = sb.ToString();

            // si el texto termina en punto (ej. "123.") permitimos mantenerlo
            // deja tal cual. Si se han eliminado caracteres, actualizar y colocar el caret al final.
            if (cleaned != original)
            {
                tb.Text = cleaned;
                tb.SelectionStart = cleaned.Length;
            }
        }

        // Añadir estos dos métodos dentro de la clase FormSolicitudPago (por ejemplo, junto a otros handlers)

        // Se ejecuta al entrar en el TextBox de RNC/Proveedor: quita el formateo para facilitar edición
        private void TxtRNCProveedor_Enter(object sender, EventArgs e)
        {
            if (txtRNCProveedor == null) return;

            // Quitar cualquier carácter no numérico para dejar sólo dígitos
            string digits = System.Text.RegularExpressions.Regex.Replace(txtRNCProveedor.Text ?? string.Empty, @"\D", string.Empty);
            txtRNCProveedor.Text = digits;
            txtRNCProveedor.SelectionStart = txtRNCProveedor.Text.Length;
        }

        // Formatea la visualización del RNC/Cédula:
        // - 11 dígitos => XXX-XXXXXXX-X (cédula / RNC persona física)
        // - 9 dígitos  => X-XX-XXXXX-X (RNC persona jurídica)
        // - cualquier otra longitud => devuelve los dígitos sin guiones
        private string FormatRncDisplay(string digits)
        {
            if (string.IsNullOrEmpty(digits))
                return string.Empty;

            // Mantener solo dígitos
            digits = System.Text.RegularExpressions.Regex.Replace(digits, @"\D", string.Empty);

            if (digits.Length == 11)
            {
                // 3-7-1
                return $"{digits.Substring(0, 3)}-{digits.Substring(3, 7)}-{digits.Substring(10, 1)}";
            }

            if (digits.Length == 9)
            {
                // 1-2-5-1
                return $"{digits.Substring(0, 1)}-{digits.Substring(1, 2)}-{digits.Substring(3, 5)}-{digits.Substring(8, 1)}";
            }

            // Fallback: devolver solo dígitos (sin guiones)
            return digits;
        }

        // Maneja la entrada en txtTasa: solo dígitos permitidos; el punto decimal se inserta automáticamente
        // interpretando los últimos hasta 6 dígitos como decimales.
        private void Tasa_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir control (backspace) y dígitos únicamente
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        
        private void Tasa_TextChanged(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            string original = tb.Text ?? string.Empty;

            // Mantener solo dígitos
            string digits = Regex.Replace(original, "[^0-9]", string.Empty);

            if (string.IsNullOrEmpty(digits))
            {
                tb.Text = string.Empty;
                return;
            }

            // Limitar a la longitud máxima permitida por la configuración de tasa
            int maxTotal = Math.Min(18, tasaIntegerDigits + tasaDecimalDigits);
            if (digits.Length > maxTotal)
                digits = digits.Substring(0, maxTotal);

            // Si aún no se exceden los enteros parametrizados, mostrar la parte entera tal cual
            if (digits.Length <= tasaIntegerDigits)
            {
                // Mostrar los dígitos tal cual (sin punto)
                if (tb.Text != digits)
                {
                    tb.Text = digits;
                    tb.SelectionStart = tb.Text.Length;
                }
                return;
            }

            // Cuando se excede la cantidad de enteros, separar
            string intPart = digits.Substring(0, tasaIntegerDigits);
            string decPart = digits.Substring(tasaIntegerDigits);

            // No forzar padding de decimales: usar lo que el usuario escribió (hasta tasaDecimalDigits)
            if (decPart.Length > tasaDecimalDigits)
                decPart = decPart.Substring(0, tasaDecimalDigits);

            string formatted = intPart + "." + decPart;

            if (tb.Text != formatted)
            {
                tb.Text = formatted;
                tb.SelectionStart = tb.Text.Length;
            }
        }

        #endregion

        // =========================================================
        // ABRIR MINI-FORM AGREGAR FIDEICOMISO
        // =========================================================
        private void BtnAgregarFideicomiso_Click(object sender, EventArgs e)
        {
            using (FormAgregarFideicomiso form = new FormAgregarFideicomiso(this))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // El mini-form ya refrescó los datos y seleccionó el fideicomiso
                }
            }
        }

        // =========================================================
        // ABRIR MINI-FORM AGREGAR PROVEEDOR
        // =========================================================
        private void BtnAgregarProveedor_Click(object sender, EventArgs e)
        {
            using (FormAgregarProveedor form = new FormAgregarProveedor(this))
            {
                form.ShowDialog();
                // FormAgregarProveedor invoca RefrescarProveedores en padre cuando corresponde.
            }
        }

        // =========================================================
        // REFRESCAR FIDEICOMISOS DESDE MINI-FORM
        // =========================================================
        public void RefrescarFideicomisos(int? fideicomisoIDSeleccionar)
        {
            // Recargar datos desde BD
            CargarFideicomisos();

            if (fideicomisoIDSeleccionar.HasValue)
            {
                // Selección programática: seleccionar combo y además actualizar txtCodigoFideicomiso y lblRNCFideicomiso
                suppressFideicomisoEvents = true;
                try
                {
                    // Seleccionar en el combo si existe
                    try
                    {
                        cboFideicomiso.SelectedValue = fideicomisoIDSeleccionar.Value;
                    }
                    catch
                    {
                        cboFideicomiso.SelectedIndex = -1;
                    }

                    // Obtener fila correspondiente del DataTable y actualizar los TextBoxes/labels directamente
                    if (dtFideicomisos != null && dtFideicomisos.Rows.Count > 0)
                    {
                        DataRow[] rows = dtFideicomisos.Select($"FideicomisoID = {fideicomisoIDSeleccionar.Value}");
                        if (rows.Length > 0)
                        {
                            string codigo = rows[0]["Codigo"]?.ToString() ?? string.Empty;
                            string rnc = rows[0]["RNC"]?.ToString() ?? string.Empty;

                            // Actualizar sin depender del SelectedIndexChanged del combo
                            txtCodigoFideicomiso.Text = codigo;
                            lblRNCFideicomiso.Text = string.IsNullOrEmpty(rnc) ? "RNC: ---" : $"RNC: {FormatearRNC(rnc)}";
                        }
                    }
                }
                finally
                {
                    suppressFideicomisoEvents = false;
                }
            }
            else
            {
                txtCodigoFideicomiso.Text = string.Empty;
                suppressFideicomisoEvents = true;
                cboFideicomiso.SelectedIndex = -1;
                suppressFideicomisoEvents = false;
                lblRNCFideicomiso.Text = "RNC: ---";
            }
        }

        // =========================================================
        // REFRESCAR PROVEEDORES DESDE MINI-FORM
        // =========================================================
        public void RefrescarProveedores(int? proveedorIDSeleccionar)
        {
            // Recargar datos desde BD
            CargarProveedores();
            ConfigurarAutocompletadoProveedor();

            if (proveedorIDSeleccionar.HasValue)
            {
                // Selección programática en el hilo UI
                this.BeginInvoke(new Action(() =>
                {
                    suppressProveedorEvents = true;
                    try
                    {
                        try
                        {
                            cboProveedor.SelectedValue = proveedorIDSeleccionar.Value;
                        }
                        catch
                        {
                            cboProveedor.SelectedIndex = -1;
                        }

                        // Obtener fila correspondiente y actualizar txtRNCProveedor y lblTelefonoProveedor
                        if (dtProveedores != null && dtProveedores.Rows.Count > 0)
                        {
                            DataRow[] rows = dtProveedores.Select($"ProveedorID = {proveedorIDSeleccionar.Value}");
                            if (rows.Length > 0)
                            {
                                string numero = (rows[0]["NumeroDocumento"] ?? string.Empty).ToString();
                                string numeroClean = System.Text.RegularExpressions.Regex.Replace(numero, @"\D", string.Empty);
                                string telefono = (rows[0]["Telefono"] ?? string.Empty).ToString();

                                // Formatear y actualizar directamente para que el usuario vea los valores guardados
                                txtRNCProveedor.Text = FormatRncDisplay(numeroClean);
                                lblTelefonoProveedor.Text = string.IsNullOrEmpty(telefono) ? "Tel: ---" : $"Tel: {FormatPhone(telefono)}";
                            }
                        }
                    }
                    finally
                    {
                        suppressProveedorEvents = false;
                    }
                }));
            }
            else
            {
                txtRNCProveedor.Text = string.Empty;
                suppressProveedorEvents = true;
                cboProveedor.SelectedIndex = -1;
                suppressProveedorEvents = false;
                lblTelefonoProveedor.Text = "Tel: ---";
            }
        }

        // =========================================================
        // Helper para formatear teléfono para display (lbl)
        // =========================================================
        private string FormatPhone(string digits)
        {
            if (string.IsNullOrWhiteSpace(digits)) return string.Empty;
            string d = Regex.Replace(digits, @"\D", string.Empty);
            if (d.Length == 10)
                return $"({d.Substring(0,3)}) {d.Substring(3,3)}-{d.Substring(6,4)}";
            if (d.Length == 7)
                return $"{d.Substring(0,3)}-{d.Substring(3,4)}";
            return digits;
        }

        private void CboTipoComprobante_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Si el usuario cambia el TipoComprobante a una opción que NO sea NCF,
            // limpiamos la selección de cboTipoNCF (si tenía alguna) pero no lo deshabilitamos.
            bool requiereNCF = false;
            try
            {
                if (cboTipoComprobante.SelectedItem is System.Data.DataRowView drvTipo)
                {
                    if (drvTipo.Row.Table.Columns.Contains("RequiereNCF"))
                        requiereNCF = Convert.ToBoolean(drvTipo["RequiereNCF"]);
                    if (!requiereNCF && drvTipo["Nombre"]?.ToString().Trim().Equals("NCF", StringComparison.OrdinalIgnoreCase) == true)
                        requiereNCF = true;
                }
            }
            catch { }

            if (!requiereNCF)
            {
                // limpiar selección solo si había una
                if (cboTipoNCF != null && cboTipoNCF.SelectedIndex >= 0)
                {
                    cboTipoNCF.SelectedIndex = -1;
                    // restablecer placeholder (si lo deseas)
                    SetNumeroNCFPlaceholder();
                }
            }
        }
    }
}