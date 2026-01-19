using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOFIS_ERP.Controls;
using MOFIS_ERP.Classes;

namespace MOFIS_ERP.Forms.Sistema.GestionRoles
{
    public partial class FormDashboardRoles : Form
    {
        private FormMain formPrincipal;

        public FormDashboardRoles(FormMain formMain)
        {
            InitializeComponent();

            this.BackColor = Color.FromArgb(240, 240, 240);
            this.FormBorderStyle = FormBorderStyle.None;

            formPrincipal = formMain;
            ConfigurarFormulario();
            CrearInterfaz();
        }

        private void ConfigurarFormulario()
        {
            this.BackColor = Color.FromArgb(240, 240, 240);
        }

        private void CrearInterfaz()
        {
            // Panel principal con scroll
            Panel panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(240, 240, 240)
            };
            this.Controls.Add(panelPrincipal);

            // Botón volver (esquina superior izquierda)
            Button btnVolver = new Button
            {
                Text = "← Volver",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Size = new Size(120, 40),
                Location = new Point(20, 20),
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                ForeColor = Color.FromArgb(0, 120, 212)
            };
            btnVolver.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 212);
            btnVolver.Click += BtnVolver_Click;
            panelPrincipal.Controls.Add(btnVolver);

            // Logo centrado arriba
            PictureBox picLogo = null;
            try
            {
                string logoPath = System.IO.Path.Combine(Application.StartupPath, "Resources", "MOFIS ERP -LOGO.png");

                if (System.IO.File.Exists(logoPath))
                {
                    picLogo = new PictureBox
                    {
                        Image = Image.FromFile(logoPath),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Size = new Size(400, 250),
                        BackColor = Color.Transparent,
                        Anchor = AnchorStyles.Top
                    };
                    this.Controls.Add(picLogo);
                    picLogo.BringToFront();

                    picLogo.Location = new Point((this.ClientSize.Width - picLogo.Width) / 2, -40);

                    this.Resize += (s, e) =>
                    {
                        if (picLogo != null)
                            picLogo.Location = new Point((this.ClientSize.Width - picLogo.Width) / 2, -40);
                    };
                }
            }
            catch { }

            // Contenedor centralizado
            Panel panelCentral = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Transparent
            };
            panelPrincipal.Controls.Add(panelCentral);

            // Título de la sección
            Label lblTitulo = new Label
            {
                Text = "GESTIÓN DE ROLES Y PERMISOS",
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 212),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelCentral.Controls.Add(lblTitulo);

            // Subtítulo
            Label lblSubtitulo = new Label
            {
                Text = "Sistema de Administración de Seguridad",
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                ForeColor = Color.Gray,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelCentral.Controls.Add(lblSubtitulo);

            // Panel para las cards (FlowLayoutPanel centrado)
            FlowLayoutPanel panelCards = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = true,
                Padding = new Padding(0, 50, 0, 0),
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.LeftToRight
            };
            panelCentral.Controls.Add(panelCards);

            // Definir las 2 opciones principales del módulo
            var opciones = new[]
            {
                new {
                    Nombre = "ADMINISTRAR PERMISOS",
                    Icono = "🔧",
                    Descripcion = "Gestión completa de permisos: Roles, Usuarios, Excepciones y Configuración del sistema de seguridad",
                    Accion = "ADMINISTRAR"
                },
                new {
                    Nombre = "REPORTES Y AUDITORÍA",
                    Icono = "📊",
                    Descripcion = "Historial de cambios, estadísticas y reportes detallados del sistema de permisos",
                    Accion = "REPORTES"
                }
            };

            // Crear una card por cada opción
            foreach (var opcion in opciones)
            {
                // Verificar permisos (solo ROOT y ADMIN pueden acceder)
                if (!SesionActual.EsRoot() && !SesionActual.EsAdmin())
                    continue;

                CardControl card = new CardControl
                {
                    Titulo = opcion.Nombre,
                    Icono = opcion.Icono,
                    Descripcion = opcion.Descripcion,
                    Size = new Size(350, 300),
                    Margin = new Padding(25)
                };

                // Evento click de la card
                card.CardClick += (s, e) => Card_Click(opcion.Accion);

                panelCards.Controls.Add(card);
            }

            // Centrar todo el contenido
            panelPrincipal.Resize += (s, e) => CentrarContenido(panelPrincipal, panelCentral);
            CentrarContenido(panelPrincipal, panelCentral);

            // Posicionar elementos dentro del panel central
            LayoutContenido(lblTitulo, lblSubtitulo, panelCards);
        }

        private void LayoutContenido(Label titulo, Label subtitulo, FlowLayoutPanel panelCards)
        {
            // Calcular ancho total del panel de cards
            int anchoCards = panelCards.PreferredSize.Width;

            // Centrar título
            titulo.Location = new Point(
                (Math.Max(titulo.Width, anchoCards) - titulo.Width) / 2,
                20
            );

            // Centrar subtítulo
            subtitulo.Location = new Point(
                (Math.Max(subtitulo.Width, anchoCards) - subtitulo.Width) / 2,
                titulo.Bottom + 10
            );

            // Posicionar panel de cards
            panelCards.Location = new Point(0, subtitulo.Bottom + 20);
        }

        private void CentrarContenido(Panel contenedor, Panel contenido)
        {
            contenido.Location = new Point(
                Math.Max(0, (contenedor.ClientSize.Width - contenido.Width) / 2),
                Math.Max(80, (contenedor.ClientSize.Height - contenido.Height) / 2)
            );
        }

        /// <summary>
        /// Maneja el click en una card de opción
        /// </summary>
        private void Card_Click(string accion)
        {
            switch (accion)
            {
                case "ADMINISTRAR":
                    AbrirAdministrarPermisos();
                    break;

                case "REPORTES":
                    AbrirReportesPermisos();
                    break;
            }
        }

        /// <summary>
        /// Abre el formulario de administración de permisos
        /// Incluye gestión de roles, usuarios y excepciones
        /// </summary>
        private void AbrirAdministrarPermisos()
        {
            FormAdministrarPermisos formAdministrar = new FormAdministrarPermisos(formPrincipal);
            formPrincipal.CargarContenidoPanel(formAdministrar);
        }

        /// <summary>
        /// Abre el formulario de reportes y auditoría de permisos
        /// Incluye historial completo y estadísticas
        /// </summary>
        private void AbrirReportesPermisos()
        {
            FormReportesPermisos formReportes = new FormReportesPermisos(formPrincipal);
            formPrincipal.CargarContenidoPanel(formReportes);
        }

        /// <summary>
        /// Vuelve al dashboard de Sistema
        /// </summary>
        private void BtnVolver_Click(object sender, EventArgs e)
        {
            FormDashboardSistema dashboardSistema = new FormDashboardSistema(formPrincipal);
            formPrincipal.CargarContenidoPanel(dashboardSistema);
        }
    }
}