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

namespace MOFIS_ERP.Forms.Sistema
{
    public partial class FormDashboardSistema : Form
    {
        private FormMain formPrincipal;

        public FormDashboardSistema(FormMain formMain)
        {
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

            // AGREGAR ESTO: Logo centrado arriba
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

                    // Centrar horizontalmente
                    picLogo.Location = new Point((this.ClientSize.Width - picLogo.Width) / 2, -40);

                    // Recentrar cuando se redimensione la ventana
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

            // Título de la categoría
            Label lblCategoria = new Label
            {
                Text = "SISTEMA",
                Font = new Font("Segoe UI", 36, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 212),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelCentral.Controls.Add(lblCategoria);

            // Subtítulo
            Label lblSubtitulo = new Label
            {
                Text = "Módulos de Configuración y Administración",
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                ForeColor = Color.Gray,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelCentral.Controls.Add(lblSubtitulo);

            // Panel para las cards de módulos
            FlowLayoutPanel panelModulos = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = true,
                Padding = new Padding(0, 50, 0, 0),
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.LeftToRight
            };
            panelCentral.Controls.Add(panelModulos);

            // Definir los módulos de Sistema
            var modulos = new[]
            {
                new { Nombre = "GESTIÓN DE USUARIOS", Icono = "👥", Descripcion = "Crear y administrar usuarios" },
                new { Nombre = "GESTIÓN DE ROLES", Icono = "🔐", Descripcion = "Configurar permisos" },
                new { Nombre = "AUDITORÍA", Icono = "📋", Descripcion = "Registro de actividades" }
            };

            // Crear una card por cada módulo
            foreach (var modulo in modulos)
            {
                // Verificar permisos según rol
                if (!TieneAccesoModulo(modulo.Nombre))
                    continue;

                CardControl card = new CardControl
                {
                    Titulo = modulo.Nombre,
                    Icono = modulo.Icono,
                    Descripcion = modulo.Descripcion,
                    Size = new Size(320, 280),
                    Margin = new Padding(25)
                };

                // Evento click de la card
                card.CardClick += (s, e) => Modulo_Click(modulo.Nombre);

                panelModulos.Controls.Add(card);
            }

            // Centrar todo el contenido
            panelPrincipal.Resize += (s, e) => CentrarContenido(panelPrincipal, panelCentral);
            CentrarContenido(panelPrincipal, panelCentral);

            // Posicionar elementos dentro del panel central
            LayoutContenido(lblCategoria, lblSubtitulo, panelModulos);
        }

        private void LayoutContenido(Label titulo, Label subtitulo, FlowLayoutPanel panelModulos)
        {
            // Calcular ancho total
            int anchoModulos = Math.Max(titulo.Width, panelModulos.PreferredSize.Width);

            // Centrar título
            titulo.Location = new Point(
                (Math.Max(titulo.Width, anchoModulos) - titulo.Width) / 2,
                20
            );

            // Centrar subtítulo
            subtitulo.Location = new Point(
                (Math.Max(subtitulo.Width, anchoModulos) - subtitulo.Width) / 2,
                titulo.Bottom + 10
            );

            // Posicionar panel de módulos
            panelModulos.Location = new Point(0, subtitulo.Bottom + 20);
        }

        private void CentrarContenido(Panel contenedor, Panel contenido)
        {
            contenido.Location = new Point(
                Math.Max(0, (contenedor.ClientSize.Width - contenido.Width) / 2),
                Math.Max(80, (contenedor.ClientSize.Height - contenido.Height) / 2)
            );
        }

        /// <summary>
        /// Verifica si el usuario tiene acceso a un módulo según su rol
        /// </summary>
        private bool TieneAccesoModulo(string modulo)
        {
            // TODOS los usuarios pueden ver TODOS los módulos
            // Las restricciones CRUD se aplican dentro de cada formulario
            return true;
        }

        /// <summary>
        /// Maneja el click en una card de módulo
        /// </summary>
        private void Modulo_Click(string nombreModulo)
        {
            switch (nombreModulo.ToUpper())
            {
                case "GESTIÓN DE USUARIOS":
                    AbrirGestionUsuarios();
                    break;

                case "GESTIÓN DE ROLES":
                    AbrirGestionRoles();
                    break;

                case "AUDITORÍA":
                    AbrirAuditoria();
                    break;
            }
        }

        /// <summary>
        /// Abre el Modulo de Gestión de Usuarios
        /// </summary>
        private void AbrirGestionUsuarios()
        {
            // Abrir el formulario de Gestión de Usuarios
            GestionUsuarios.FormGestionUsuarios formGestionUsuarios = new GestionUsuarios.FormGestionUsuarios(formPrincipal);
            formPrincipal.CargarContenidoPanel(formGestionUsuarios);
        }

        /// <summary>
        /// Abre el Modulo de Gestión de Roles
        /// </summary>
        private void AbrirGestionRoles()
        {
            GestionRoles.FormDashboardRoles formDashboardRoles = new GestionRoles.FormDashboardRoles(formPrincipal);
            formPrincipal.CargarContenidoPanel(formDashboardRoles);
        }

        /// <summary>
        /// Abre el Modulo de Auditoría
        /// </summary>
        private void AbrirAuditoria()
        {
            Auditoria.FormAuditoria formAuditoria = new Auditoria.FormAuditoria(formPrincipal);
            formPrincipal.CargarContenidoPanel(formAuditoria);
        }

        /// <summary>
        /// Vuelve al dashboard de categorías
        /// </summary>
        private void BtnVolver_Click(object sender, EventArgs e)
        {
            // Volver al dashboard de categorías
            FormDashboardCategorias dashboardCategorias = new FormDashboardCategorias(formPrincipal);
            formPrincipal.CargarContenidoPanel(dashboardCategorias);
        }
    }
}