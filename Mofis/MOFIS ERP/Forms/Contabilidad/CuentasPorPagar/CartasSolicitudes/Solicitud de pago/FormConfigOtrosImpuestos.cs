using System;
using System.Drawing;
using System.Windows.Forms;

namespace MOFIS_ERP.Forms.Contabilidad.CuentasPorPagar.CartasSolicitudes
{
    public partial class FormConfigOtrosImpuestos : Form
    {
        public string NombreImpuesto { get; private set; }
        public bool SumarAlTotal { get; private set; }

        public FormConfigOtrosImpuestos(string nombreActual, bool sumarActual)
        {
            InitializeComponent();
            txtNombre.Text = nombreActual.Replace(":", "").Trim(); // Remove suffix for editing
            
            if (sumarActual)
                rbSumar.Checked = true;
            else
                rbRestar.Checked = true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("El nombre del impuesto no puede estar vacío.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Enforce max length manually just in case, though MaxLength property handles it
            if (nombre.Length > 15)
                nombre = nombre.Substring(0, 15);

            // Append colon if not present (logic handled in main form, but we return clean name here)
            NombreImpuesto = nombre;
            SumarAlTotal = rbSumar.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
