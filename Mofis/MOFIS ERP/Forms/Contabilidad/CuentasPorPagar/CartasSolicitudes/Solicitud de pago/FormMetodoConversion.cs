using System;
using System.Drawing;
using System.Windows.Forms;

namespace MOFIS_ERP.Forms.Contabilidad.CuentasPorPagar.CartasSolicitudes.Solicitud_de_pago
{
    /// <summary>
    /// Formulario para seleccionar el método de conversión de moneda.
    /// </summary>
    public partial class FormMetodoConversion : Form
    {
        // =========================================================
        // CONSTANTES - MÉTODOS DE CONVERSIÓN
        // =========================================================
        public const int METODO_DIRECTO = 1;
        public const int METODO_BASE = 2;

        // =========================================================
        // PROPIEDADES PÚBLICAS
        // =========================================================

        /// <summary>
        /// Método de conversión seleccionado (1=DIRECTO, 2=BASE, null si canceló).
        /// </summary>
        public int? MetodoSeleccionado { get; private set; } = null;

        /// <summary>
        /// Nombre descriptivo del método seleccionado.
        /// </summary>
        public string NombreMetodo { get; private set; } = string.Empty;

        // =========================================================
        // CAMPOS PRIVADOS
        // =========================================================
        private int? metodoActual = null;
        private readonly Color colorBotonNormal = Color.White;
        private readonly Color colorBotonSeleccionado = Color.FromArgb(0, 120, 212);
        private readonly Color colorTextoNormal = Color.FromArgb(64, 64, 64);
        private readonly Color colorTextoSeleccionado = Color.White;

        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        /// <summary>
        /// Crea el formulario de selección de método.
        /// </summary>
        /// <param name="metodoPreseleccionado">Método actualmente seleccionado (si existe).</param>
        public FormMetodoConversion(int? metodoPreseleccionado = null)
        {
            InitializeComponent();
            ConfigurarEventos();

            // Si hay un método preseleccionado, mostrarlo
            if (metodoPreseleccionado.HasValue)
            {
                SeleccionarMetodo(metodoPreseleccionado.Value);
            }
        }

        // =========================================================
        // CONFIGURACIÓN DE EVENTOS
        // =========================================================
        private void ConfigurarEventos()
        {
            btnMetodoDirecto.Click += BtnMetodoDirecto_Click;
            btnMetodoBase.Click += BtnMetodoBase_Click;
            chkConfirmarMetodo.CheckedChanged += ChkConfirmarMetodo_CheckedChanged;
            btnAplicar.Click += BtnAplicar_Click;
            btnCancelar.Click += BtnCancelar_Click;
        }

        // =========================================================
        // EVENTOS DE BOTONES DE MÉTODOS
        // =========================================================
        private void BtnMetodoDirecto_Click(object sender, EventArgs e)
        {
            SeleccionarMetodo(METODO_DIRECTO);
        }

        private void BtnMetodoBase_Click(object sender, EventArgs e)
        {
            SeleccionarMetodo(METODO_BASE);
        }

        // =========================================================
        // LÓGICA DE SELECCIÓN
        // =========================================================
        private void SeleccionarMetodo(int metodo)
        {
            metodoActual = metodo;

            // Resetear estilos de ambos botones
            ResetearEstiloBoton(btnMetodoDirecto);
            ResetearEstiloBoton(btnMetodoBase);

            // Aplicar estilo al botón seleccionado
            Button btnSeleccionado = metodo == METODO_DIRECTO ? btnMetodoDirecto : btnMetodoBase;
            btnSeleccionado.BackColor = colorBotonSeleccionado;
            btnSeleccionado.ForeColor = colorTextoSeleccionado;

            // Actualizar panel derecho
            ActualizarDescripcion(metodo);

            // Habilitar checkbox
            chkConfirmarMetodo.Enabled = true;
            chkConfirmarMetodo.Checked = false;

            // Actualizar texto del checkbox
            string nombreCorto = metodo == METODO_DIRECTO ? "Conversión Directa" : "Base + Recálculo";
            chkConfirmarMetodo.Text = $"Seleccionar método: {nombreCorto}";
        }

        private void ResetearEstiloBoton(Button btn)
        {
            btn.BackColor = colorBotonNormal;
            btn.ForeColor = colorTextoNormal;
        }

        private void ActualizarDescripcion(int metodo)
        {
            if (metodo == METODO_DIRECTO)
            {
                lblTituloMetodo.Text = "💱 Conversión Directa Total";
                lblDescripcionMetodo.Text =
                    "Todos los montos de la solicitud se multiplican directamente por la tasa de cambio ingresada.\n\n" +
                    "• Subtotal × Tasa\n" +
                    "• ITBIS × Tasa\n" +
                    "• Retenciones × Tasa\n" +
                    "• Exento × Tasa\n" +
                    "• Otros montos × Tasa\n\n" +
                    "Ideal cuando los cálculos ya están correctos en moneda extranjera.";
            }
            else if (metodo == METODO_BASE)
            {
                lblTituloMetodo.Text = "📊 Conversión Base + Recálculo";
                lblDescripcionMetodo.Text =
                    "El subtotal se convierte por la tasa, y los valores dependientes (ITBIS, retenciones) se recalculan en base a ese subtotal convertido.\n\n" +
                    "• Subtotal × Tasa → Subtotal Convertido\n" +
                    "• ITBIS = Subtotal Convertido × %ITBIS\n" +
                    "• Retenciones = Recalculadas\n" +
                    "• Otros montos (Exento, etc.) × Tasa\n\n" +
                    "Ideal cuando se requiere recálculo exacto en moneda local.";
            }
        }

        // =========================================================
        // EVENTOS DE CONFIRMACIÓN
        // =========================================================
        private void ChkConfirmarMetodo_CheckedChanged(object sender, EventArgs e)
        {
            // Solo habilitar Aplicar si hay método seleccionado y checkbox marcado
            btnAplicar.Enabled = metodoActual.HasValue && chkConfirmarMetodo.Checked;
        }

        private void BtnAplicar_Click(object sender, EventArgs e)
        {
            if (!metodoActual.HasValue)
            {
                MessageBox.Show("Debe seleccionar un método de conversión.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!chkConfirmarMetodo.Checked)
            {
                MessageBox.Show("Debe marcar la casilla de confirmación para aplicar el método.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Establecer valores de retorno
            MetodoSeleccionado = metodoActual;
            NombreMetodo = metodoActual == METODO_DIRECTO ? "Conversión Directa" : "Base + Recálculo";

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            MetodoSeleccionado = null;
            NombreMetodo = string.Empty;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
