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

            InitializeComponents();
            UpdateDisplay();
        }

        private void InitializeComponents()
        {
            this.Height = 56;
            this.Width = 180;
            this.Margin = new Padding(6);
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;

            lblMain = new Label
            {
                AutoEllipsis = true,
                Location = new Point(8, 6),
                Size = new Size(this.Width - 36, 44),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(48, 48, 48)
            };
            lblMain.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.Controls.Add(lblMain);

            btnDelete = new Button
            {
                Text = "X",
                Size = new Size(24, 24),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(this.Width - 30, 4)
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
                // si el ratón sale del control y no está encima del botón ocultarlo
                var pt = this.PointToClient(Cursor.Position);
                if (!btnDelete.Bounds.Contains(pt))
                    btnDelete.Visible = false;
            };

            // También mostrar X cuando mouse entra en la etiqueta
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
                lblMain.Width = Math.Max(60, this.Width - 36);
                btnDelete.Location = new Point(this.Width - 30, 4);
            };
        }

        private void UpdateDisplay()
        {
            if (!string.IsNullOrEmpty(TipoNCF))
                lblMain.Text = $"{TipoComprobante} · {TipoNCF} · {NumeroNCF}";
            else
                lblMain.Text = $"{TipoComprobante} · {NumeroNCF}";
        }
    }
}
