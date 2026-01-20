using System;
using System.Drawing;
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

        // ═══════════════════════════════════════════════════════════════
        // CONSTRUCTOR CON PARÁMETRO (para navegación)
        // ═══════════════════════════════════════════════════════════════
        public FormSolicitudPago(FormMain principal)
        {
            InitializeComponent();
            formPrincipal = principal;
            ConfigurarFormulario();
        }

        // ═══════════════════════════════════════════════════════════════
        // CONSTRUCTOR SIN PARÁMETROS (para el diseñador)
        // ═══════════════════════════════════════════════════════════════
        public FormSolicitudPago()
        {
            InitializeComponent();
        }

        // ═══════════════════════════════════════════════════════════════
        // CONFIGURACIÓN INICIAL
        // ═══════════════════════════════════════════════════════════════
        private void ConfigurarFormulario()
        {
            // Configurar Dock para que llene el panel contenedor
            this.Dock = DockStyle.Fill;

            // Generar número de solicitud para nuevo registro
            GenerarNumeroSolicitud();
        }

        // ═══════════════════════════════════════════════════════════════
        // GENERAR NÚMERO DE SOLICITUD
        // ═══════════════════════════════════════════════════════════════
        private void GenerarNumeroSolicitud()
        {
            // TODO: Obtener siguiente número desde la secuencia en BD
            // Por ahora, mostrar placeholder
            // lblNumeroSolicitud.Text = "SP-000001";
        }
    }
}