using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;

namespace MOFIS_ERP.Forms.Contabilidad.CuentasPorPagar.CartasSolicitudes.Solicitud_de_pago
{
    public partial class FormConfigNota : Form
    {
        // Propiedades de retorno
        public decimal Subtotal { get; private set; }
        public bool AplicaITBIS { get; private set; }
        public decimal PorcentajeITBIS { get; private set; }
        public decimal MontoITBIS { get; private set; }
        public decimal TotalNota { get; private set; }
        public string Descripcion { get; private set; }
        public bool AfectaSubtotal { get; private set; }

        // Variables de estado
        private bool esCredito;
        private bool tieneSubtotalSolicitud; // Para validar si se puede usar opción de afectar subtotal
        private NumberFormatInfo nfi;
        private string simboloMoneda;

        public FormConfigNota(bool esCredito, bool tieneSubtotalSolicitud, 
                              NumberFormatInfo nfiMain,
                              string simboloMonedaMain,
                              decimal? subtotalActual = null, 
                              bool aplicaITBIS = true, 
                              decimal porcentajeITBIS = 18m, 
                              string descripcion = "", 
                              bool afectaSubtotal = false)
        {
            InitializeComponent();
            this.esCredito = esCredito;
            this.tieneSubtotalSolicitud = tieneSubtotalSolicitud;
            this.nfi = nfiMain;
            this.simboloMoneda = simboloMonedaMain;

            ConfigurarUI();


            // Cargar valores si es edición
            if (subtotalActual.HasValue)
            {
                txtSubtotal.Text = subtotalActual.Value.ToString("N2", nfi);
                chkAplicaITBIS.Checked = aplicaITBIS;
                numPorcentajeITBIS.Value = porcentajeITBIS;
                txtDescripcion.Text = descripcion;
                
                // Solo marcar afectar subtotal si es posible
                if (tieneSubtotalSolicitud && aplicaITBIS)
                {
                    chkAfectarSubtotal.Checked = afectaSubtotal;
                }
                else
                {
                    chkAfectarSubtotal.Checked = false;
                }
            }
            else
            {
                numPorcentajeITBIS.Value = 18m;
                chkAplicaITBIS.Checked = true;
                chkAfectarSubtotal.Checked = false;
            }

            Recalcular();
        }

        private void ConfigurarUI()
        {
            this.Text = esCredito ? "Configurar Nota de Crédito" : "Configurar Nota de Débito";
            lblTitulo.Text = esCredito ? "NOTA DE CRÉDITO" : "NOTA DE DÉBITO";
            lblTitulo.ForeColor = esCredito ? Color.FromArgb(220, 53, 69) : Color.FromArgb(0, 120, 212); // Rojo crédito, Azul débito

            // Eventos
            txtSubtotal.KeyPress += NumericDecimal_KeyPress;
            txtSubtotal.Enter += (s, e) => {
                string text = txtSubtotal.Text;
                string cleaned = System.Text.RegularExpressions.Regex.Replace(text, @"[^0-9.\-]", string.Empty);
                txtSubtotal.Text = cleaned;
                txtSubtotal.SelectionStart = txtSubtotal.Text.Length;
            };
            txtSubtotal.Leave += (s, e) => {
                decimal val = ParseDecimal(txtSubtotal.Text);
                if (val != 0)
                    txtSubtotal.Text = FormatearMoneda(val);
                else
                    txtSubtotal.Text = string.Empty;
            };
            
            txtSubtotal.TextChanged += (s, e) => Recalcular();
            chkAplicaITBIS.CheckedChanged += (s, e) => {
                numPorcentajeITBIS.Enabled = chkAplicaITBIS.Checked;
                Recalcular();
            };
            numPorcentajeITBIS.ValueChanged += (s, e) => Recalcular();
            chkAfectarSubtotal.CheckedChanged += (s, e) => ValidarModoAplicacion();

            btnAceptar.Click += BtnAceptar_Click;
            btnCancelar.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
        }

        private string FormatearMoneda(decimal val)
        {
            string fmt = val.ToString("N2", nfi);
            return string.IsNullOrEmpty(simboloMoneda) ? fmt : $"{simboloMoneda} {fmt}";
        }


        private void Recalcular()
        {
            decimal sub = ParseDecimal(txtSubtotal.Text);
            decimal pct = chkAplicaITBIS.Checked ? numPorcentajeITBIS.Value : 0m;
            decimal itbis = 0m;

            if (chkAplicaITBIS.Checked)
            {
                itbis = Math.Round(sub * (pct / 100m), 2);
            }

            decimal total = sub + itbis;

            lblMontoITBIS.Text = FormatearMoneda(itbis);
            lblTotalNota.Text = FormatearMoneda(total);

            ValidarModoAplicacion();
        }

        private void ValidarModoAplicacion()
        {
            // Regla: Solo se puede afectar subtotal si:
            // 1. Hay un subtotal en la solicitud (tieneSubtotalSolicitud)
            // 2. Se aplica ITBIS en la nota (chkAplicaITBIS) -> Según requerimiento usuario: "Disponible solo si chkAplicaITBIS está activado"
            
            bool puedeAfectar = tieneSubtotalSolicitud && chkAplicaITBIS.Checked;

            chkAfectarSubtotal.Enabled = puedeAfectar;
            
            if (!puedeAfectar && chkAfectarSubtotal.Checked)
            {
                chkAfectarSubtotal.Checked = false;
            }

            // Explicación visual
            if (chkAfectarSubtotal.Checked)
            {
                lblInfoModo.Text = "Modo: El Subtotal de la nota modificará la Base Imponible de la solicitud.\nSe recalcularán impuestos generales.";
                lblInfoModo.ForeColor = Color.OrangeRed;
            }
            else
            {
                lblInfoModo.Text = "Modo: El Total de la nota se aplicará directamente al Total a Pagar.\nNo modifica impuestos de la solicitud.";
                lblInfoModo.ForeColor = Color.Green;
            }
        }

        private void BtnAceptar_Click(object sender, EventArgs e)
        {
            Subtotal = ParseDecimal(txtSubtotal.Text);
            AplicaITBIS = chkAplicaITBIS.Checked;
            PorcentajeITBIS = AplicaITBIS ? numPorcentajeITBIS.Value : 0m;
            
            // Recalcular final para asegurar precisión
            MontoITBIS = AplicaITBIS ? Math.Round(Subtotal * (PorcentajeITBIS / 100m), 2) : 0m;
            TotalNota = Subtotal + MontoITBIS;
            
            Descripcion = txtDescripcion.Text.Trim();
            AfectaSubtotal = chkAfectarSubtotal.Checked;

            if (Subtotal <= 0)
            {
                MessageBox.Show("El subtotal debe ser mayor a cero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // Helpers
        private decimal ParseDecimal(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0m;
            
            // Limpiar todo excepto dígitos y punto
            string clean = System.Text.RegularExpressions.Regex.Replace(text, "[^0-9.]", "");
            
            if (decimal.TryParse(clean, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal val))
                return val;
            return 0m;
        }

        private void NumericDecimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            // Solo un punto decimal
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
    }
}
