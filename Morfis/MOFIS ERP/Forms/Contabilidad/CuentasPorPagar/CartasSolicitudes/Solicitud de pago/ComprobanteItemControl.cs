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
            // Tarjeta compacta con padding para evitar recortes del botón
            this.Height = 36;
            this.Width = 120;
            this.Margin = new Padding(4);
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Padding = new Padding(4); // espacio interno para que el botón no quede pegado al borde

            // Botón docked a la derecha para que siempre se vea entero
            btnDelete = new Button
            {
                Text = "X",
                Width = 28,
                Dock = DockStyle.Right,
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(6, 4, 4, 4)
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Visible = false;
            btnDelete.Click += (s, e) => RemoveRequested?.Invoke(this, EventArgs.Empty);
            this.Controls.Add(btnDelete);
            btnDelete.BringToFront();

            // Label ocupa el resto del espacio
            lblMain = new Label
            {
                AutoEllipsis = true,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(48, 48, 48),
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(4, 4, 4, 4)
            };
            this.Controls.Add(lblMain);
            lblMain.BringToFront();

            // Mostrar el botón X al pasar el ratón (control o label)
            this.MouseEnter += (s, e) => btnDelete.Visible = true;
            this.MouseLeave += (s, e) =>
            {
                var pt = this.PointToClient(Cursor.Position);
                // si el cursor no está sobre el botón ni dentro del control, ocultar
                if (!btnDelete.Bounds.Contains(pt) && !this.ClientRectangle.Contains(pt))
                    btnDelete.Visible = false;
                else
                {
                    // si está dentro del control pero no sobre el botón, dejamos visible solo si el cursor está dentro
                    if (!btnDelete.Bounds.Contains(pt) && this.ClientRectangle.Contains(pt))
                        btnDelete.Visible = true;
                }
            };

            // También manejar hover del label y del botón para evitar flicker
            lblMain.MouseEnter += (s, e) => btnDelete.Visible = true;
            lblMain.MouseLeave += (s, e) =>
            {
                var pt = this.PointToClient(Cursor.Position);
                if (!btnDelete.Bounds.Contains(pt) && !this.ClientRectangle.Contains(pt))
                    btnDelete.Visible = false;
            };

            btnDelete.MouseEnter += (s, e) => btnDelete.Visible = true;
            btnDelete.MouseLeave += (s, e) =>
            {
                var pt = this.PointToClient(Cursor.Position);
                if (!this.ClientRectangle.Contains(pt))
                    btnDelete.Visible = false;
            };

            // Ajuste responsivo al cambiar tamaño
            this.Resize += (s, e) =>
            {
                // si quieres botones más grandes en anchos mayores puedes ajustar Width aquí
                if (this.Width > 140) btnDelete.Width = 30; else btnDelete.Width = 28;
            };
        }

        private void UpdateDisplay()
        {
            // Mostrar sólo el comprobante completo (prefijo + secuencial)
            lblMain.Text = NumeroNCF;
        }
    }
}
