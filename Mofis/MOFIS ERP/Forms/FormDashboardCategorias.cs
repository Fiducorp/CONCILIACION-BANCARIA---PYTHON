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

namespace MOFIS_ERP.Forms
{
    public partial class FormDashboardCategorias : Form
    {
        private FormMain formPrincipal;

        public FormDashboardCategorias(FormMain formMain)
        {
            // Configurar propiedades del formulario
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.FormBorderStyle = FormBorderStyle.None;

            formPrincipal = formMain;
            ConfigurarFormulario();
            CrearCards();
        }

        // Configura el formulario principal del dashboard de categorías
        private void ConfigurarFormulario()
        {
            this.BackColor = Color.FromArgb(240, 240, 240);
        }

        // Crea las cards de categorías dinámicamente según el rol del usuario actual 
        private void CrearCards()
        {
            // Logo del sistema centrado arriba (PRIMERO)
            PictureBox picLogo = null;
            try
            {
                // Usar la imagen embebida en Resources (nombre: LOGO)
                var logoImage = Properties.Resources.LOGO as Image;
                if (logoImage != null)
                {
                    picLogo = new PictureBox
                    {
                        Image = logoImage,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Size = new Size(400, 250), // Más grande
                        BackColor = Color.Transparent,
                        Anchor = AnchorStyles.Top
                    };
                    this.Controls.Add(picLogo);
                    picLogo.BringToFront();

                    // Centrar horizontalmente y mover arriba para compensar espacio extra
                    picLogo.Location = new Point((this.ClientSize.Width - picLogo.Width) / 2, -40); // Posición Y negativa

                    // Recentrar cuando se redimensione la ventana
                    this.Resize += (s, e) =>
                    {
                        if (picLogo != null)
                            picLogo.Location = new Point((this.ClientSize.Width - picLogo.Width) / 2, -40);
                    };
                }
            }
            catch
            {
                // No interrumpir UI si falla la carga del logo
            }

            // Panel principal con scroll (con margen superior para el logo)
            Panel panelPrincipal = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(this.ClientSize.Width, this.ClientSize.Height),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoScroll = true,
                BackColor = Color.FromArgb(240, 240, 240)
            };
            this.Controls.Add(panelPrincipal);
            panelPrincipal.SendToBack(); // Enviar al fondo para que el logo quede encima

            // Contenedor centralizado
            Panel panelCentral = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Transparent
            };
            panelPrincipal.Controls.Add(panelCentral);

            // Título principal
            Label lblTitulo = new Label
            {
                Text = "Seleccione una Categoría",
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 51, 51),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelCentral.Controls.Add(lblTitulo);

            // Subtítulo con nombre de usuario
            Label lblSubtitulo = new Label
            {
                Text = $"Bienvenido, {SesionActual.NombreCompleto}",
                Font = new Font("Segoe UI", 16, FontStyle.Regular),
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
                Padding = new Padding(0, 40, 0, 0),
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.LeftToRight
            };
            panelCentral.Controls.Add(panelCards);

            // Definir las 5 categorías
            var categorias = new[]
            {
                new { Nombre = "SISTEMA", Icono = "\u2699\ufe0f", Descripcion = "Configuración y Usuarios" },
                new { Nombre = "CONTABILIDAD", Icono = "\ud83d\udcca", Descripcion = "Gestión Contable" },
                new { Nombre = "GERENCIA FINANCIERA", Icono = "\ud83d\udcbc", Descripcion = "Análisis Financiero" },
                new { Nombre = "GERENCIA LEGAL", Icono = "\u2696\ufe0f", Descripcion = "Gestión Legal" },
                new { Nombre = "DESARROLLO", Icono = "\ud83d\ude80", Descripcion = "Módulos Futuros" }
            };

            // Crear una card por cada categoría
            foreach (var categoria in categorias)
            {
                // Verificar permisos según rol
                if (!TieneAccesoCategoria(categoria.Nombre))
                    continue;

                CardControl card = new CardControl
                {
                    Titulo = categoria.Nombre,
                    Icono = categoria.Icono,
                    Descripcion = categoria.Descripcion,
                    Size = new Size(320, 280),
                    Margin = new Padding(25)
                };

                // Evento click de la card
                card.CardClick += (s, e) => Card_Click(categoria.Nombre);

                panelCards.Controls.Add(card);
            }

            // Centrar todo el contenido
            panelPrincipal.Resize += (s, e) => CentrarContenido(panelPrincipal, panelCentral);
            CentrarContenido(panelPrincipal, panelCentral);

            // Posicionar elementos dentro del panel central
            LayoutContenido(lblTitulo, lblSubtitulo, panelCards);
        }

        // Organiza la disposición del título, subtítulo y panel de cards dentro del contenedor central 
        private void LayoutContenido(Label titulo, Label subtitulo, FlowLayoutPanel panelCards)
        {
            // Calcular ancho total del panel de cards
            int anchoCards = 0;
            foreach (Control card in panelCards.Controls)
            {
                anchoCards = Math.Max(anchoCards, panelCards.PreferredSize.Width);
            }

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

        // Centra el contenido dentro del contenedor dado 
        private void CentrarContenido(Panel contenedor, Panel contenido)
        {
            contenido.Location = new Point(
                Math.Max(0, (contenedor.ClientSize.Width - contenido.Width) / 2),
                Math.Max(50, (contenedor.ClientSize.Height - contenido.Height) / 2)
            );
        }

        /// <summary>
        /// Verifica si el usuario tiene acceso a una categoría según su rol
        /// </summary>
        private bool TieneAccesoCategoria(string categoria)
        {
            // TODOS los usuarios pueden ver TODAS las categorías
            // Las restricciones se aplican dentro de cada formulario
            return true;
        }

        /// <summary>
        /// Maneja el click en una card de categoría
        /// </summary>
        private void Card_Click(string nombreCategoria)
        {
            switch (nombreCategoria.ToUpper())
            {

                case "SISTEMA":
                    AbrirDashboardSistema();
                    break;

                case "CONTABILIDAD":
                    AbrirDashboardContabilidad();
                    break;

                case "GERENCIA FINANCIERA":
                    MessageBox.Show("Dashboard de Gerencia Financiera\n(Próximamente)",
                        "En Desarrollo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;

                case "GERENCIA LEGAL":
                    MessageBox.Show("Dashboard de Gerencia Legal\n(Próximamente)",
                        "En Desarrollo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;

                case "DESARROLLO":
                    MessageBox.Show("Área de Desarrollo\n(Módulos Futuros)",
                        "En Desarrollo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        /// <summary>
        /// Abre el dashboard de la categoría Sistema
        /// </summary>
        private void AbrirDashboardSistema()
        {
            // Crear y abrir el dashboard de Sistema
            Sistema.FormDashboardSistema dashboardSistema = new Sistema.FormDashboardSistema(formPrincipal);
            formPrincipal.CargarContenidoPanel(dashboardSistema);
        }

        /// <summary>
        /// Abre el dashboard de la categoría Contabilidad
        /// </summary>
        private void AbrirDashboardContabilidad()
        {
            Contabilidad.FormDashboardContabilidad dashboardContabilidad = new Contabilidad.FormDashboardContabilidad(formPrincipal);
            formPrincipal.CargarContenidoPanel(dashboardContabilidad);
        }
    }
}