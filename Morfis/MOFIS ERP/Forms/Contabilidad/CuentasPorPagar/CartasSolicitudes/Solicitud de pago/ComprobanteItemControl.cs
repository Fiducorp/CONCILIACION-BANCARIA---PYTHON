using System;
using System.Drawing;
using System.Windows.Forms;

namespace MOFIS_ERP.Forms.Contabilidad.CuentasPorPagar.CartasSolicitudes
{
    public partial class ComprobanteItemControl : UserControl
    {
        private Label lblMain;
        private Button btnDelete;
        public string TipoComprobante { get; private set; }
        public string TipoNCF { get; private set; }    // puede ser null
        public string NumeroNCF { get; private set; }  // NCF completo o referencia

        public event EventHandler RemoveRequested;

        public ComprobanteItemControl(string tipoComprobante, string tipoNCF, string numeroNCF)
        {
            TipoComprobante = tipoComprobante ?? string.Empty;
            TipoNCF = tipoNCF;
            NumeroNCF = numeroNCF ?? string.Empty;

            InitControl();
            UpdateDisplay();
        }

        private void InitControl()
        {
            // Tarjeta más compacta, con fuente más grande para legibilidad
            this.Height = 36;
            this.Width = 120;
            this.Margin = new Padding(4);
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;

            lblMain = new Label
            {
                AutoEllipsis = true,
                Location = new Point(6, 4),
                Size = new Size(this.Width - 34, this.Height - 8),
                Font = new Font("Segoe UI", 12.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(48, 48, 48),
                TextAlign = ContentAlignment.MiddleLeft
            };
            lblMain.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.Controls.Add(lblMain);

            btnDelete = new Button
            {
                Text = "X",
                Size = new Size(20, 20),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(this.Width - 26, 6)
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDelete.Visible = false;
            btnDelete.Click += (s, e) => RemoveRequested?.Invoke(this, EventArgs.Empty);
            this.Controls.Add(btnDelete);

            // Mostrar el botón X al pasar el ratón
            this.MouseEnter += (s, e) => btnDelete.Visible = true;
            this.MouseLeave += (s, e) =>
            {
                var pt = this.PointToClient(Cursor.Position);
                if (!btnDelete.Bounds.Contains(pt))
                    btnDelete.Visible = false;
            };

            lblMain.MouseEnter += (s, e) => btnDelete.Visible = true;
            lblMain.MouseLeave += (s, e) =>
            {
                var pt = this.PointToClient(Cursor.Position);
                if (!btnDelete.Bounds.Contains(pt))
                    btnDelete.Visible = false;
            };

            // Ajuste responsivo al cambiar tamaño
            this.Resize += (s, e) =>
            {
                lblMain.Width = Math.Max(40, this.Width - 34);
                btnDelete.Location = new Point(this.Width - 26, 6);
            };
        }

        private void UpdateDisplay()
        {
            // Mostrar sólo el comprobante completo (prefijo + secuencial) para ocupar menos espacio
            lblMain.Text = NumeroNCF;
        }
    }
}
