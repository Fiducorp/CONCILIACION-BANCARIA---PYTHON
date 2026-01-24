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
        // ═══════════════════════════════════════════════════════════════
        // CAMPOS PRIVADOS
        // ═══════════════════════════════════════════════════════════════
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

        // ═══════════════════════════════════════════════════════════════
        // CONSTRUCTOR CON PARÁMETRO (para navegación)
        // ═══════════════════════════════════════════════════════════════
        public FormSolicitudPago(FormMain principal)
        {
            InitializeComponent();
            formPrincipal = principal;
            ConfigurarFormulario();
            CargarDatosIniciales();
        }

        // ═══════════════════════════════════════════════════════════════
        // CONSTRUCTOR SIN PARÁMETROS (para el diseñador y carga directa)
        // ═══════════════════════════════════════════════════════════════
        public FormSolicitudPago()
        {
            InitializeComponent();
            ConfigurarFormulario();
            CargarDatosIniciales();
        }

        // ═══════════════════════════════════════════════════════════════
        // CONFIGURACIÓN INICIAL
        // ═══════════════════════════════════════════════════════════════
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

        // Permite parametrizar la interpretación de txtTasa:
        // integerDigits = número máximo de dígitos en la parte entera
        // decimalDigits = número máximo de dígitos en la parte decimal (hasta 6 según tus requerimientos)
        private void SetTasaFormat(int integerDigits, int decimalDigits)
        {
            if (integerDigits < 0) integerDigits = 0;
            if (decimalDigits < 0) decimalDigits = 0;
            if (decimalDigits > 18) decimalDigits = 18; // protección
            if (integerDigits > 18) integerDigits = 18;

            tasaIntegerDigits = integerDigits;
            tasaDecimalDigits = decimalDigits;
        }

        private void ConfigurarEventos()
        {
            // Eventos de Fideicomiso
            txtCodigoFideicomiso.Leave += TxtCodigoFideicomiso_Leave;
            cboFideicomiso.SelectedIndexChanged += CboFideicomiso_SelectedIndexChanged;

            // Eventos de Proveedor
            txtRNCProveedor.Enter += TxtRNCProveedor_Enter;
            txtRNCProveedor.Leave += TxtRNCProveedor_Leave;
            cboProveedor.SelectedIndexChanged += CboProveedor_SelectedIndexChanged;

            // Evento de Moneda (mostrar/ocultar tasa)
            cboMoneda.SelectedIndexChanged += CboMoneda_SelectedIndexChanged;

            // Evento de Concepto (contador de caracteres)
            txtConcepto.TextChanged += TxtConcepto_TextChanged;

            // Evento de Observaciones (contador de caracteres)
            txtObservaciones.TextChanged += TxtObservaciones_TextChanged;

            // Evento del botón Agregar Fideicomiso
            btnAgregarFideicomiso.Click += BtnAgregarFideicomiso_Click;

            // Evento maestro: activar/desactivar cálculo automático del ITBIS
            if (chkCalcularITBIS != null)
            {
                chkCalcularITBIS.CheckedChanged += ChkCalcularITBIS_CheckedChanged;
            }

            // Evento de ITBIS Manual (override)
            if (chkITBISManual != null)
            {
                chkITBISManual.CheckedChanged += ChkITBISManual_CheckedChanged;
            }

            // CheckBoxes para retenciones (habilitan/deshabilitan sus TextBoxes)
            if (chkRetSFS != null)
            {
                chkRetSFS.CheckedChanged += ChkRetSFS_CheckedChanged;
            }

            if (chkRetAFP != null)
            {
                chkRetAFP.CheckedChanged += ChkRetAFP_CheckedChanged;
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

                // Para los TextBoxes de montos (currencyTextBoxes) vamos a controlar el formateo
                // en Enter/Leave para preservar comas de miles; no adjuntamos el sanitizador genérico.
                if (!currencyTextBoxes.Contains(tb) && tb != txtTasa)
                {
                    tb.TextChanged += (s, e) => SanitizeDecimalTextBox(tb);
                }
            };

            // No usar attachDecimal para txtTasa — tendrá comportamiento especial
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

            // Manejadores especiales para txtTasa: solo dígitos, punto automático y hasta N decimales
            txtTasa.KeyPress += Tasa_KeyPress;
            txtTasa.TextChanged += Tasa_TextChanged;
            txtTasa.Enter += (s, e) => { /* mantener caret al final */ txtTasa.SelectionStart = txtTasa.Text.Length; };

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

        // Obtiene el símbolo de moneda seleccionado (ej. "RD$") y actualiza monedaNumberFormat
        private void UpdateMonedaFormatFromCombo()
        {
            monedaSimbolo = string.Empty;

            // Base con separadores fijos: miles "," y decimal "." según requisito
            monedaNumberFormat = BuildNumberFormat();

            if (cboMoneda != null && cboMoneda.SelectedItem is DataRowView drv)
            {
                monedaSimbolo = (drv["Simbolo"] ?? string.Empty).ToString();

                // Ajuste de dígitos decimales por moneda si la información está disponible
                // Intentamos leer una columna "CodigoISO" para excepciones (ej. JPY sin decimales)
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

        // Formatea número (sin símbolo) con grouping y punto decimal. Conserva la cantidad de decimales
        // provista en decimalPartLength si > 0, o forza 2 si = 0.
        private string FormatNumericPart(string integerPart, string decimalPart)
        {
            var nfi = monedaNumberFormat ?? BuildNumberFormat();

            // Formatear la parte entera con grouping
            long intVal = 0;
            string intFormatted = integerPart;
            if (long.TryParse(integerPart, out intVal))
            {
                intFormatted = intVal.ToString("N0", nfi); // sin decimales
            }

            if (!string.IsNullOrEmpty(decimalPart))
            {
                return intFormatted + "." + decimalPart;
            }

            // Forzar .00
            return intFormatted + ".00";
        }

        // Evento Enter de los TextBoxes de moneda: quitar símbolo y dejar número (con comas si había)
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

            // Eliminar cualquier caracter que no sea dígito, coma, punto o signo menos
            string cleaned = Regex.Replace(text, "[^0-9,\\.\\-]", string.Empty);

            // Si viene con comas de miles, mantenerlas (usuario quiere verlas), pero KeyPress bloquea la coma;
            // el TextChanged sanitizador no está adjuntado para estos TBs, así que las comas se preservan.
            tb.Text = cleaned;
            tb.SelectionStart = tb.Text.Length;
        }

        // Evento Leave de los TextBoxes de moneda: aplicar formato con símbolo y separadores
        private void CurrencyTextBox_Leave(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            string text = tb.Text ?? string.Empty;
            // Limpiar: mantener dígitos y punto decimal
            string cleaned = Regex.Replace(text, @"[^0-9.\-]", string.Empty);

            if (string.IsNullOrWhiteSpace(cleaned))
            {
                tb.Text = string.Empty;
                return;
            }

            // Separar parte entera y decimal según '.'
            string[] parts = cleaned.Split('.');
            string intPart = parts.Length > 0 ? parts[0] : "0";
            string decPart = parts.Length > 1 ? parts[1] : null;

            // Quitar ceros a la izquierda en la parte entera (pero dejar "0" si vacío)
            intPart = intPart.TrimStart('0');
            if (string.IsNullOrEmpty(intPart)) intPart = "0";

            string numericFormatted = FormatNumericPart(intPart, decPart);

            if (!string.IsNullOrEmpty(monedaSimbolo))
                tb.Text = monedaSimbolo + " " + numericFormatted;
            else
                tb.Text = numericFormatted;
        }

        // Actualiza todos los labels y textboxes formateados cuando cambia la moneda
        private void UpdateAllCurrencyDisplays()
        {
            UpdateMonedaFormatFromCombo();

            // Re-formatear los TextBoxes si ya tienen valor
            foreach (var tb in currencyTextBoxes)
            {
                if (tb == null) continue;
                // Simular Leave para aplicar formato si tiene texto
                if (!string.IsNullOrWhiteSpace(tb.Text))
                {
                    CurrencyTextBox_Leave(tb, EventArgs.Empty);
                }
            }

            // Panel impuestos: estos labels contienen solo el número calculado
            // Si hay valores parseables, los re-formateamos. Si están en texto vacio o no parseable, los dejamos.
            ReformatLabelNumberIfPossible(lblITBISCalculado);
            ReformatLabelNumberIfPossible(lblITBISDiferencia);
            ReformatLabelNumberIfPossible(lblRetITBISMonto);
            ReformatLabelNumberIfPossible(lblRetISRMonto);

            // Panel totales: reemplazar sólo la parte numérica en cada label
            ReformatAmountPortionInLabel(lblTotalSubtotalTitulo);
            ReformatAmountPortionInLabel(lblTotalITBISTitulo);
            ReformatAmountPortionInLabel(lblTotalExentoTitulo);
            ReformatAmountPortionInLabel(lblTotalFacturaTitulo);
            ReformatAmountPortionInLabel(lblRetITBISTitulo);
            ReformatAmountPortionInLabel(lblRetISRTitulo);
            ReformatAmountPortionInLabel(lblOtrasRetTitulo);
            ReformatAmountPortionInLabel(lblTotalRetencionTitulo);
            ReformatAmountPortionInLabel(lblTotalAPagar);
        }

        // Intenta parsear el texto completo del label como número y lo formatea con símbolo
        private void ReformatLabelNumberIfPossible(Label lbl)
        {
            if (lbl == null) return;
            string text = lbl.Text ?? string.Empty;
            // Limpiar posibles símbolos y espacios
            string cleaned = Regex.Replace(text, @"[^0-9.\-]", string.Empty);
            if (string.IsNullOrWhiteSpace(cleaned)) return;

            decimal val;
            if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
            {
                lbl.Text = (!string.IsNullOrEmpty(monedaSimbolo) ? monedaSimbolo + " " : string.Empty) + val.ToString("N2", monedaNumberFormat ?? BuildNumberFormat());
            }
        }

        // Reemplaza la porción numérica (última coincidencia) dentro del texto del label por el número formateado
        private void ReformatAmountPortionInLabel(Label lbl)
        {
            if (lbl == null) return;
            string text = lbl.Text ?? string.Empty;

            // Buscar la última secuencia de dígitos, comas o puntos
            var matches = Regex.Matches(text, @"[0-9][0-9,\.]*");
            if (matches.Count == 0) return;

            var last = matches[matches.Count - 1];
            string numericPortion = last.Value;

            // Limpiar la porción numérica para parsear
            string cleaned = numericPortion.Replace(",", string.Empty);

            decimal val;
            if (!decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
            {
                return;
            }

            // Formatear la porción numérica (sin símbolo) usando los decimales configurados
            string formattedNumeric = val.ToString("N" + (monedaNumberFormat?.NumberDecimalDigits ?? 2), monedaNumberFormat ?? BuildNumberFormat());

            // Determinar si existe un símbolo inmediatamente antes de la porción numérica
            string before = text.Substring(0, last.Index);
            string after = text.Substring(last.Index + last.Length);

            // Buscar el último token no vacío antes del número
            var m = Regex.Match(before, @"(\S+)\s*$");
            string newBefore;
            if (m.Success)
            {
                string token = m.Groups[1].Value;
                // Si el token contiene caracteres no numéricos (ej. '$' o letras), considerarlo símbolo y reemplazar
                bool tokenIsSymbol = token.Any(c => !char.IsDigit(c) && c != ':' && c != '\\' && c != '\n');

                if (tokenIsSymbol)
                {
                    // Reemplazamos ese token por el símbolo actual (si existe) seguido de un espacio
                    string prefix = string.Empty;
                    if (!string.IsNullOrEmpty(monedaSimbolo))
                        prefix = monedaSimbolo + " ";

                    newBefore = before.Substring(0, m.Index) + prefix;
                }
                else
                {
                    // No hay token símbolo: insertar símbolo si existe
                    newBefore = before;
                    if (!string.IsNullOrEmpty(monedaSimbolo))
                        newBefore = newBefore + monedaSimbolo + " ";
                }
            }
            else
            {
                // No hay token: simplemente insertar símbolo si procede
                newBefore = before;
                if (!string.IsNullOrEmpty(monedaSimbolo))
                    newBefore = newBefore + monedaSimbolo + " ";
            }

            string newText = newBefore + formattedNumeric + after;

            lbl.Text = newText;
        }

        // ═══════════════════════════════════════════════════════════════
        // CARGA DE DATOS INICIALES
        // ═══════════════════════════════════════════════════════════════
        private void CargarDatosIniciales()
        {
            try
            {
                // Cargar todos los combos
                CargarTiposPago();
                CargarMonedas();
                CargarTiposComprobante();
                CargarTiposNCF();
                CargarPorcentajesITBIS();
                CargarPorcentajesRetenciones();

                // Cargar datos para autocompletado
                CargarFideicomisos();
                CargarProveedores();

                // Configurar autocompletado
                ConfigurarAutocompletadoFideicomiso();
                ConfigurarAutocompletadoProveedor();

                // Generar número de solicitud
                GenerarNumeroSolicitud();

                // Seleccionar valores por defecto
                SeleccionarValoresPorDefecto();

                // Aplicar formato inicial de moneda (según selección por defecto)
                UpdateAllCurrencyDisplays();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos iniciales: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // SELECCIÓN DE VALORES POR DEFECTO
        // ═══════════════════════════════════════════════════════════════
        private void SeleccionarValoresPorDefecto()
        {
            // Seleccionar DOP como moneda por defecto
            foreach (DataRowView item in cboMoneda.Items)
            {
                if (item["CodigoISO"].ToString() == "DOP")
                {
                    cboMoneda.SelectedItem = item;
                    break;
                }
            }

            // Seleccionar Transferencia como tipo de pago por defecto
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

            // Seleccionar 18% como ITBIS por defecto
            foreach (var item in cboITBISPorcentaje.Items)
            {
                if (item.ToString() == "18%")
                {
                    cboITBISPorcentaje.SelectedItem = item;
                    break;
                }
            }

            // Ocultar tasa de cambio si moneda es DOP
            txtTasa.Visible = false;
            lblTasa.Visible = false;

            // Inicializar el CheckBox maestro (por defecto activado para calcular ITBIS)
            if (chkCalcularITBIS != null)
            {
                chkCalcularITBIS.Checked = true;
            }

            // Inicializar estado del combo de firma según el checkbox
            if (chkIncluirFirma != null && cboFirma != null)
            {
                cboFirma.Enabled = chkIncluirFirma.Checked;
            }

            // Inicializar placeholder del NCF según el tipo seleccionado
            SetNumeroNCFPlaceholder();

            // Actualizar estados relacionados con ITBIS y firma
            UpdateEstadoCalculoITBIS();
            ActualizarEstadoITBISManual();

            if (chkRetSFS != null)
            {
                chkRetSFS.Checked = false; // por defecto desactivado
                txtRetSFS.Enabled = chkRetSFS.Checked;
                txtRetSFS.BackColor = SystemColors.Control;
            }

            if (chkRetAFP != null)
            {
                chkRetAFP.Checked = false; // por defecto desactivado
                txtRetAFP.Enabled = chkRetAFP.Checked;
                txtRetAFP.BackColor = SystemColors.Control;
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // GENERAR NÚMERO DE SOLICITUD
        // ═══════════════════════════════════════════════════════════════
        private void GenerarNumeroSolicitud()
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Obtener siguiente valor de la secuencia
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
                // Si falla, mostrar placeholder
                lblNumeroSolicitud.Text = "SP-NUEVO";
                System.Diagnostics.Debug.WriteLine($"Error generando número: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // REGIÓN: CARGA DE COMBOS DESDE BASE DE DATOS
        // ═══════════════════════════════════════════════════════════════

        #region Carga de Combos

        // Cargar Tipos de Pago
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
        // Cargar Monedas
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
        // Cargar Tipos de Comprobante
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
        // Cargar Tipos de NCF
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

                    // Agregar columna combinada para mostrar
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
        // Cargar Porcentajes de ITBIS
        private void CargarPorcentajesITBIS()
        {
            cboITBISPorcentaje.Items.Clear();
            cboITBISPorcentaje.Items.Add("0%");
            cboITBISPorcentaje.Items.Add("16%");
            cboITBISPorcentaje.Items.Add("18%");
            cboITBISPorcentaje.SelectedIndex = -1;
        }
        // Cargar Porcentajes de Retenciones
        private void CargarPorcentajesRetenciones()
        {
            // Retención ITBIS
            cboRetITBIS.Items.Clear();
            cboRetITBIS.Items.Add("0%");
            cboRetITBIS.Items.Add("30%");
            cboRetITBIS.Items.Add("100%");
            cboRetITBIS.SelectedIndex = 0;

            // Retención ISR
            cboRetISR.Items.Clear();
            cboRetISR.Items.Add("0%");
            cboRetISR.Items.Add("2%");
            cboRetISR.Items.Add("10%");
            cboRetISR.Items.Add("27%");
            cboRetISR.SelectedIndex = 0;
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════
        // REGIÓN: AUTOCOMPLETADO DE FIDEICOMISO
        // ═══════════════════════════════════════════════════════════════

        #region Autocompletado Fideicomiso

        // Función auxiliar para dar formato (x-xx-xxxxx-x o xxx-xxxxxxx-x)
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

                cboFideicomiso.DataSource = null;
                cboFideicomiso.DisplayMember = "Nombre";
                cboFideicomiso.ValueMember = "FideicomisoID";
                cboFideicomiso.DataSource = dtFideicomisos;
                cboFideicomiso.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando fideicomisos: {ex.Message}");
                dtFideicomisos = new DataTable();
            }
        }

        private void ConfigurarAutocompletadoFideicomiso()
        {
            // Configurar el ComboBox con autocompletado
            cboFideicomiso.DataSource = dtFideicomisos;
            cboFideicomiso.DisplayMember = "Nombre";
            cboFideicomiso.ValueMember = "FideicomisoID";
            cboFideicomiso.SelectedIndex = -1;

            // Configurar autocompletado
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

        private void CboFideicomiso_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboFideicomiso.SelectedIndex >= 0 && cboFideicomiso.SelectedValue != null)
            {
                // Obtener la fila seleccionada
                DataRowView drv = cboFideicomiso.SelectedItem as DataRowView;

                if (drv != null)
                {
                    // Actualizar código
                    txtCodigoFideicomiso.Text = drv["Codigo"].ToString();

                    // CAMBIO APLICADO AQUÍ: Actualizar y Formatear RNC
                    string rnc = drv["RNC"].ToString();
                    lblRNCFideicomiso.Text = string.IsNullOrEmpty(rnc)
                        ? "RNC: ---"
                        : $"RNC: {FormatearRNC(rnc)}";
                }
            }
            else
            {
                lblRNCFideicomiso.Text = "RNC: ---";
            }
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════
        // REGIÓN: AUTOCOMPLETADO DE PROVEEDOR
        // ═══════════════════════════════════════════════════════════════

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
            // Configurar el ComboBox con autocompletado
            cboProveedor.DataSource = dtProveedores;
            cboProveedor.DisplayMember = "Nombre";
            cboProveedor.ValueMember = "ProveedorID";
            cboProveedor.SelectedIndex = -1;

            // Configurar autocompletado
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
                lblTelefonoProveedor.Text = string.IsNullOrEmpty(telefono) ? "Tel: ---" : $"Tel: {telefono}";
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

        private void CboProveedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProveedor.SelectedIndex >= 0 && cboProveedor.SelectedValue != null)
            {
                // Obtener la fila seleccionada
                DataRowView drv = cboProveedor.SelectedItem as DataRowView;

                if (drv != null)
                {
                    // Actualizar RNC/Cédula en formato visual apropiado
                    string numeroRaw = drv["NumeroDocumento"]?.ToString() ?? string.Empty;
                    txtRNCProveedor.Text = FormatRncDisplay(numeroRaw);

                    // Actualizar teléfono
                    string telefono = drv["Telefono"]?.ToString();
                    lblTelefonoProveedor.Text = string.IsNullOrEmpty(telefono) ? "Tel: ---" : $"Tel: {telefono}";
                }
            }
            else
            {
                lblTelefonoProveedor.Text = "Tel: ---";
            }
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════
        // REGIÓN: EVENTOS DE CONTROLES
        // ═══════════════════════════════════════════════════════════════

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
            if (txtITBISManual != null) txtITBISManual.Enabled = manualChecked;

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

        /// <summary>
        /// Construye y aplica el placeholder para txtNumeroNCF según el tipo seleccionado en cboTipoNCF.
        /// Ej.: "B01 00000000" (11 chars) o "E31 0000000000" (13 chars).
        /// </summary>
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

        /// <summary>
        /// Devuelve la longitud total esperada para el NCF según la Serie del tipo seleccionado:
        /// - Serie "B" => 11
        /// - Serie "E" => 13
        /// Si no puede determinar, devuelve 11 por defecto.
        /// </summary>
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
                // ignore y fallback
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
                // fallback silencioso
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
        }

        private void ChkRetAFP_CheckedChanged(object sender, EventArgs e)
        {
            if (txtRetAFP == null || chkRetAFP == null)
                return;

            // Habilita o deshabilita el TextBox según el estado del CheckBox
            txtRetAFP.Enabled = chkRetAFP.Checked;

            // Opcional: ajustar color para indicar estado
            txtRetAFP.BackColor = txtRetAFP.Enabled ? Color.White : SystemColors.Control;
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

        // ═══════════════════════════════════════════════════════════════
        // ABRIR MINI-FORM AGREGAR FIDEICOMISO
        // ═══════════════════════════════════════════════════════════════
        private void BtnAgregarFideicomiso_Click(object sender, EventArgs e)
        {
            using (FormAgregarFideicomiso form = new FormAgregarFideicomiso(this))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // El mini-form ya refrescó los datos y seleccionó el fideicomiso
                    // No es necesario hacer nada adicional aquí
                }
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // REFRESCAR FIDEICOMISOS DESDE MINI-FORM
        // ═══════════════════════════════════════════════════════════════
        public void RefrescarFideicomisos(int? fideicomisoIDSeleccionar)
        {
            // Recargar datos desde BD
            CargarFideicomisos();

            // Si se especificó un ID, seleccionarlo
            if (fideicomisoIDSeleccionar.HasValue)
            {
                cboFideicomiso.SelectedValue = fideicomisoIDSeleccionar.Value;
            }
            else
            {
                // Limpiar selección (por ejemplo, si se eliminó el fideicomiso actual)
                txtCodigoFideicomiso.Text = string.Empty;
                cboFideicomiso.SelectedIndex = -1;
                lblRNCFideicomiso.Text = "RNC: ---";
            }
        }
    }
}