using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace MOFIS_ERP.Forms.Sistema.Auditoria
{
    public partial class FormConfiguracionReportePDF : Form
    {
        public DataView DatosFiltrados { get; set; }
        public ConfiguracionReportePDF Configuracion { get; private set; }

        // Colores corporativos
        private readonly Color colorCorporativo = Color.FromArgb(0, 120, 212);
        private readonly Color colorGris = Color.FromArgb(108, 117, 125);

        public FormConfiguracionReportePDF(DataView datosFiltrados, int totalRegistros)
        {
            InitializeComponent();

            DatosFiltrados = datosFiltrados;
            lblInfoRegistros.Text = $"Se generarán {totalRegistros:N0} registros en el PDF";

            // Configuración inicial
            cmbTamanoPagina.SelectedIndex = 0; // A4 por defecto

            // Eventos
            btnGenerar.Click += BtnGenerar_Click;
            btnCancelar.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            // Configurar hover de botones
            ConfigurarBotonHover(btnGenerar, colorCorporativo);
            ConfigurarBotonHover(btnCancelar, colorGris);

            // Cambiar opciones según tipo de reporte
            rbCompleto.CheckedChanged += TipoReporte_CheckedChanged;
            rbEjecutivo.CheckedChanged += TipoReporte_CheckedChanged;
            rbDetallado.CheckedChanged += TipoReporte_CheckedChanged;
            rbComparativo.CheckedChanged += TipoReporte_CheckedChanged;
        }

        private void TipoReporte_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEjecutivo.Checked)
            {
                // Reporte ejecutivo: solo resumen y gráficos
                chkResumenEjecutivo.Checked = true;
                chkCriteriosBusqueda.Checked = true;
                chkEstadisticas.Checked = true;
                chkTop10.Checked = true;
                chkGraficosAnaliticos.Checked = true;
                chkDetalleRegistros.Checked = false;
            }
            else if (rbDetallado.Checked)
            {
                // Reporte detallado: solo listado completo
                chkDetalleRegistros.Checked = true;
                chkGraficosAnaliticos.Checked = false;
                chkGraficos.Checked = false;
            }
            else if (rbCompleto.Checked)
            {
                // Reporte completo: todo marcado
                chkResumenEjecutivo.Checked = true;
                chkCriteriosBusqueda.Checked = true;
                chkEstadisticas.Checked = true;
                chkTop10.Checked = true;
                chkDetalleRegistros.Checked = true;
                chkResumenModulo.Checked = true;
                chkResumenUsuario.Checked = true;
                chkGraficosAnaliticos.Checked = true;
            }
        }

        /// <summary>
        /// Configura el efecto hover de los botones
        /// </summary>
        private void ConfigurarBotonHover(Button btn, Color colorBase)
        {
            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Dark(colorBase, 0.1f);
            btn.MouseLeave += (s, e) => btn.BackColor = colorBase;
        }

        private void BtnGenerar_Click(object sender, EventArgs e)
        {
            // Recopilar configuración
            Configuracion = new ConfiguracionReportePDF
            {
                TipoReporte = ObtenerTipoReporte(),
                Orientacion = rbVertical.Checked ? OrientacionPagina.Vertical : OrientacionPagina.Horizontal,
                TamanoPagina = (TamanoPagina)cmbTamanoPagina.SelectedIndex,

                IncluirPortada = chkPortada.Checked,
                IncluirIndice = chkIndice.Checked,
                IncluirGraficos = chkGraficos.Checked,
                IncluirFirmas = chkFirmas.Checked,

                IncluirResumenEjecutivo = chkResumenEjecutivo.Checked,
                IncluirCriteriosBusqueda = chkCriteriosBusqueda.Checked,
                IncluirEstadisticas = chkEstadisticas.Checked,
                IncluirTop10 = chkTop10.Checked,
                IncluirDetalleRegistros = chkDetalleRegistros.Checked,
                IncluirResumenModulo = chkResumenModulo.Checked,
                IncluirResumenUsuario = chkResumenUsuario.Checked,
                IncluirGraficosAnaliticos = chkGraficosAnaliticos.Checked,
                IncluirConclusiones = chkConclusiones.Checked
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private TipoReportePDF ObtenerTipoReporte()
        {
            if (rbCompleto.Checked) return TipoReportePDF.Completo;
            if (rbEjecutivo.Checked) return TipoReportePDF.Ejecutivo;
            if (rbDetallado.Checked) return TipoReportePDF.Detallado;
            if (rbComparativo.Checked) return TipoReportePDF.Comparativo;
            return TipoReportePDF.Completo;
        }
    }

    // ========================================
    // ENUMS Y CLASES DE CONFIGURACIÓN
    // ========================================

    /// <summary>
    /// Tipo de reporte PDF a generar
    /// </summary>
    public enum TipoReportePDF
    {
        Completo,       // Todo incluido
        Ejecutivo,      // Resumen + gráficos
        Detallado,      // Solo listado completo
        Comparativo     // Dos periodos lado a lado
    }

    /// <summary>
    /// Orientación de la página
    /// </summary>
    public enum OrientacionPagina
    {
        Vertical,
        Horizontal
    }

    /// <summary>
    /// Tamaño de página
    /// </summary>
    public enum TamanoPagina
    {
        A4,      // 210 x 297 mm
        Carta,   // 216 x 279 mm
        Legal    // 216 x 356 mm
    }

    /// <summary>
    /// Configuración completa del reporte PDF
    /// </summary>
    public class ConfiguracionReportePDF
    {
        // Formato del documento
        public TipoReportePDF TipoReporte { get; set; }
        public OrientacionPagina Orientacion { get; set; }
        public TamanoPagina TamanoPagina { get; set; }

        // Opciones del documento
        public bool IncluirPortada { get; set; }
        public bool IncluirIndice { get; set; }
        public bool IncluirGraficos { get; set; }
        public bool IncluirFirmas { get; set; }

        // Secciones a incluir
        public bool IncluirResumenEjecutivo { get; set; }
        public bool IncluirCriteriosBusqueda { get; set; }
        public bool IncluirEstadisticas { get; set; }
        public bool IncluirTop10 { get; set; }
        public bool IncluirDetalleRegistros { get; set; }
        public bool IncluirResumenModulo { get; set; }
        public bool IncluirResumenUsuario { get; set; }
        public bool IncluirGraficosAnaliticos { get; set; }
        public bool IncluirConclusiones { get; set; }
    }
}
