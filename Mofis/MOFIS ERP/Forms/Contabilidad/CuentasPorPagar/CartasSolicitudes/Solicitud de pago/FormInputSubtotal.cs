using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;

namespace MOFIS_ERP.Forms.Contabilidad.CuentasPorPagar.CartasSolicitudes
{
    public partial class FormInputSubtotal : Form
    {
        public decimal MontoIngresado { get; private set; }
        public bool AgregarOtro { get; private set; }

        public FormInputSubtotal()
        {
            InitializeComponent();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            this.AcceptButton = btnAgregar;
            this.CancelButton = btnCancelar;

            // Eventos
            txtMonto.KeyPress += TxtMonto_KeyPress;
            txtMonto.TextChanged += TxtMonto_TextChanged;
            txtMonto.Leave += TxtMonto_Leave;

            btnAgregar.Click += BtnAgregar_Click;
            btnAgregarOtro.Click += BtnAgregarOtro_Click;
            btnCancelar.Click += (s, e) => this.Close();
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (ProcesarMonto())
            {
                AgregarOtro = false;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void BtnAgregarOtro_Click(object sender, EventArgs e)
        {
            if (ProcesarMonto())
            {
                AgregarOtro = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool ProcesarMonto()
        {
            // Limpiar cualquier símbolo de moneda y comas
            string texto = txtMonto.Text;
            texto = Regex.Replace(texto, @"[^0-9.\-]", "");

            if (decimal.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal monto))
            {
                if (monto <= 0)
                {
                    MessageBox.Show("El monto debe ser mayor a 0.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                MontoIngresado = monto;
                return true;
            }
            else
            {
                MessageBox.Show("Por favor ingrese un monto válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Manejo básico de input numérico (similar al form principal)
        private void TxtMonto_KeyPress(object sender, KeyPressEventArgs e)
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

        private void TxtMonto_TextChanged(object sender, EventArgs e)
        {
            // Opcional: validación en tiempo real
        }

        private void TxtMonto_Leave(object sender, EventArgs e)
        {
            string texto = txtMonto.Text;
            texto = Regex.Replace(texto, @"[^0-9.\-]", "");

            if (decimal.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal val))
            {
                // Mostrar con formato de comas para miles
                var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
                nfi.NumberGroupSeparator = ",";
                nfi.NumberDecimalSeparator = ".";
                txtMonto.Text = val.ToString("N2", nfi);
            }
        }
    }
}
