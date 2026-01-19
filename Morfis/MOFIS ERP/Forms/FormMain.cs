using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOFIS_ERP.Classes;

namespace MOFIS_ERP.Forms
{
    public partial class FormMain : Form
    {
        private Timer timerFechaHora;

        public FormMain()
        {
            InitializeComponent();
            ConfigurarFormulario();
            CargarInformacionUsuario();
            IniciarReloj();
            CargarDashboardInicial();
        }

        /// <summary>
        /// Configuración inicial del formulario
        /// </summary>
        private void ConfigurarFormulario()
        {
            // Configurar como MDI
            this.IsMdiContainer = true;
            this.WindowState = FormWindowState.Maximized;

            // Color de fondo del contenedor MDI
            foreach (Control control in this.Controls)
            {
                if (control is MdiClient)
                {
                    control.BackColor = Color.FromArgb(240, 240, 240);
                }
            }

            this.MaximizeBox = false; // Deshabilitar botón maximizar/restaurar
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Evitar redimensionar
        }

        /// <summary>
        /// Carga la información del usuario logueado en la barra de estado
        /// </summary>
        private void CargarInformacionUsuario()
        {
            if (SesionActual.HaySesionActiva())
            {
                // Solo mostrar nombre completo
                lblUsuario.Text = SesionActual.NombreCompleto;
                lblRol.Text = SesionActual.NombreRol;

                // Cambiar color según rol
                switch (SesionActual.NombreRol.ToUpper())
                {
                    case "ROOT":
                        lblRol.ForeColor = Color.FromArgb(192, 0, 0); // Rojo oscuro
                        break;
                    case "ADMIN":
                        lblRol.ForeColor = Color.FromArgb(0, 120, 212); // Azul corporativo
                        break;
                    case "CONTADOR":
                        lblRol.ForeColor = Color.FromArgb(16, 124, 16); // Verde oscuro
                        break;
                }
            }
            else
            {
                lblUsuario.Text = "Desconocido";
                lblRol.Text = "N/A";
            }
        }

        /// <summary>
        /// Inicia el timer para actualizar fecha y hora cada segundo
        /// </summary>
        private void IniciarReloj()
        {
            timerFechaHora = new Timer();
            timerFechaHora.Interval = 1000; // 1 segundo
            timerFechaHora.Tick += TimerFechaHora_Tick;
            timerFechaHora.Start();

            // Actualizar inmediatamente
            ActualizarFechaHora();
        }

        /// <summary>
        /// Evento del timer para actualizar la fecha/hora
        /// </summary>
        private void TimerFechaHora_Tick(object sender, EventArgs e)
        {
            ActualizarFechaHora();
        }

        /// <summary>
        /// Actualiza el label de fecha/hora con el formato: hh:mm:ss dd/MM/yyyy
        /// </summary>
        private void ActualizarFechaHora()
        {
            lblFecha.Text = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
        }

        /// <summary>
        /// Carga el dashboard inicial de categorías
        /// </summary>
        private void CargarDashboardInicial()
        {
            // Crear y mostrar el dashboard de categorías
            FormDashboardCategorias dashboardCategorias = new FormDashboardCategorias(this);
            CargarContenidoPanel(dashboardCategorias);
        }

        /// <summary>
        /// Abre un formulario hijo en el área MDI
        /// </summary>
        /// <param name="formularioHijo">Formulario a abrir</param>
        public void AbrirFormularioHijo(Form formularioHijo)
        {
            // Cerrar todos los formularios hijos abiertos
            foreach (Form hijo in this.MdiChildren)
            {
                hijo.Close();
            }

            // Configurar el nuevo formulario como hijo MDI
            formularioHijo.MdiParent = this;
            formularioHijo.WindowState = FormWindowState.Maximized;
            formularioHijo.FormBorderStyle = FormBorderStyle.None;
            formularioHijo.Dock = DockStyle.Fill;
            formularioHijo.Show();
        }

        /// <summary>
        /// Carga el contenido en el panel contenedor (para dashboards)
        /// </summary>
        /// <param name="control">Control a mostrar</param>
        public void CargarContenidoPanel(Form formulario)
        {
            panelContenedor.Controls.Clear();

            formulario.TopLevel = false;
            formulario.FormBorderStyle = FormBorderStyle.None;
            formulario.Dock = DockStyle.Fill;

            panelContenedor.Controls.Add(formulario);
            formulario.Show();
        }

        /// <summary>
        /// Evento al cerrar el formulario
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Confirmar cierre
            var resultado = MessageBox.Show(
                "¿Está seguro que desea salir del sistema?",
                "Confirmar Salida",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (resultado == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            // Detener timer
            if (timerFechaHora != null)
            {
                timerFechaHora.Stop();
                timerFechaHora.Dispose();
            }

            // Registrar logout en auditoría
            if (SesionActual.HaySesionActiva())
            {
                AuditoriaHelper.RegistrarLogout(SesionActual.UsuarioID, SesionActual.Username);
                SesionActual.CerrarSesion();
            }
        }
    }
}