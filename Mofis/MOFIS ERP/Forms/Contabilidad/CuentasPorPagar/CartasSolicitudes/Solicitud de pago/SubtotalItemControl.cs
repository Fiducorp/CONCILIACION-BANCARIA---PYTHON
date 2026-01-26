using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace MOFIS_ERP.Forms.Contabilidad.CuentasPorPagar.CartasSolicitudes
{
    public partial class SubtotalItemControl : UserControl
    {
        private Label lblMonto;
        private Button btnDelete;

        /// <summary>
        /// Valor decimal exacto del subtotal (sin redondeo).
        /// </summary>
        public decimal Monto { get; private set; }

        /// <summary>
        /// Símbolo de moneda actual para mostrar (ej: "RD$", "US$").
        /// </summary>
        public string SimboloMoneda { get; private set; }

        /// <summary>
        /// NumberFormatInfo para formatear con comas y puntos.
        /// </summary>
        public NumberFormatInfo FormatoNumero { get; private set; }

        /// <summary>
        /// Evento disparado cuando el usuario solicita eliminar este subtotal.
        /// </summary>
        public event EventHandler RemoveRequested;

        public SubtotalItemControl(decimal monto, string simboloMoneda, NumberFormatInfo formatoNumero)
        {
            Monto = monto;
            SimboloMoneda = simboloMoneda ?? "RD$";
            FormatoNumero = formatoNumero ?? BuildDefaultFormat();

            InitializeComponent();
            InitControl();
            UpdateDisplay();
        }

        private NumberFormatInfo BuildDefaultFormat()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = ",";
            nfi.NumberDecimalSeparator = ".";
            return nfi;
        }

        private void InitControl()
        {
            // Dimensiones del control
            this.Height = 32;
            this.Width = 160;
            this.Margin = new Padding(4, 2, 4, 2);
            this.BackColor = Color.FromArgb(240, 248, 255); // Alice Blue - color suave
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Padding = new Padding(2);
            this.Cursor = Cursors.Hand;

            // Botón X - docked a la derecha, oculto por defecto
            btnDelete = new Button
            {
                Text = "✕",
                Width = 24,
                Dock = DockStyle.Right,
                BackColor = Color.FromArgb(220, 53, 69), // Rojo
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Visible = false;
            btnDelete.Click += (s, e) => RemoveRequested?.Invoke(this, EventArgs.Empty);
            this.Controls.Add(btnDelete);
            btnDelete.BringToFront();

            // Label del monto - ocupa el resto del espacio
            lblMonto = new Label
            {
                AutoEllipsis = true,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                ForeColor = Color.FromArgb(30, 30, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(4, 0, 4, 0)
            };
            this.Controls.Add(lblMonto);
            lblMonto.SendToBack();

            // Eventos de hover para mostrar/ocultar botón X
            SetupHoverEvents(this);
            SetupHoverEvents(lblMonto);
            SetupHoverEvents(btnDelete);
        }

        private void SetupHoverEvents(Control control)
        {
            control.MouseEnter += (s, e) => btnDelete.Visible = true;
            control.MouseLeave += (s, e) =>
            {
                var pt = this.PointToClient(Cursor.Position);
                if (!this.ClientRectangle.Contains(pt))
                    btnDelete.Visible = false;
            };
        }

        private void UpdateDisplay()
        {
            // Formatear el monto con los decimales exactos que tiene
            // Detectar cuántos decimales tiene el valor
            int decimales = GetDecimalPlaces(Monto);
            if (decimales < 2) decimales = 2; // Mínimo 2 decimales para visualización

            string formato = "N" + decimales;
            string montoFormateado = Monto.ToString(formato, FormatoNumero);

            lblMonto.Text = $"{SimboloMoneda} {montoFormateado}";
        }

        /// <summary>
        /// Calcula la cantidad de decimales que tiene un valor decimal.
        /// </summary>
        private int GetDecimalPlaces(decimal value)
        {
            value = Math.Abs(value);
            value = value - Math.Truncate(value);
            int count = 0;
            while (value > 0 && count < 10)
            {
                value *= 10;
                value = value - Math.Truncate(value);
                count++;
            }
            return count == 0 ? 0 : count;
        }

        /// <summary>
        /// Actualiza el formato de moneda (cuando el usuario cambia la moneda en el formulario principal).
        /// </summary>
        public void ActualizarFormato(string nuevoSimbolo, NumberFormatInfo nuevoFormato)
        {
            SimboloMoneda = nuevoSimbolo ?? "RD$";
            FormatoNumero = nuevoFormato ?? BuildDefaultFormat();
            UpdateDisplay();
        }
    }
}
