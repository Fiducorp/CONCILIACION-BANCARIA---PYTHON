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
using MOFIS_ERP.Forms.Contabilidad.CuentasPorPagar.CartasSolicitudes;

namespace MOFIS_ERP.Forms.Contabilidad.CuentasPorPagar
{
    /// <summary>
    /// Dashboard principal del módulo Cuentas por Pagar
    /// Contiene 2 cards: Cartas y Solicitudes, Configuración
    /// </summary>
    public partial class FormDashboardCuentasPorPagar : Form
    {
        private FormMain formPrincipal;

        // Colores del módulo (azul corporativo para CXP)
        private readonly Color ColorPrimario = Color.FromArgb(0, 120, 212);
        private readonly Color ColorSecundario = Color.FromArgb(0, 99, 177);
        private readonly Color ColorFondo = Color.FromArgb(240, 240, 240);

        public FormDashboardCuentasPorPagar(FormMain formMain)
        {
            this.formPrincipal = formMain;

            // Configuración básica del formulario
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = ColorFondo;
            this.Dock = DockStyle.Fill;
            this.AutoScroll = true;

            ConfigurarFormulario();
            CrearInterfaz();
        }

        private void ConfigurarFormulario()
        {
            this.BackColor = ColorFondo;
            this.DoubleBuffered = true;
        }

        private void CrearInterfaz()
        {
            // ═══════════════════════════════════════════════════════════════
            // PANEL PRINCIPAL CON SCROLL
            // ═══════════════════════════════════════════════════════════════
            Panel panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = ColorFondo,
                Padding = new Padding(20)
            };
            this.Controls.Add(panelPrincipal);

            // ═══════════════════════════════════════════════════════════════
            // BOTÓN VOLVER (Esquina superior izquierda)
            // ═══════════════════════════════════════════════════════════════
            Button btnVolver = new Button
            {
                Text = "← Volver",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Size = new Size(120, 40),
                Location = new Point(20, 20),
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                ForeColor = ColorPrimario
            };
            btnVolver.FlatAppearance.BorderColor = ColorPrimario;
            btnVolver.FlatAppearance.BorderSize = 1;
            btnVolver.Click += BtnVolver_Click;

            // Hover effects
            btnVolver.MouseEnter += (s, e) => {
                btnVolver.BackColor = ColorPrimario;
                btnVolver.ForeColor = Color.White;
            };
            btnVolver.MouseLeave += (s, e) => {
                btnVolver.BackColor = Color.White;
                btnVolver.ForeColor = ColorPrimario;
            };

            panelPrincipal.Controls.Add(btnVolver);

            // ═══════════════════════════════════════════════════════════════
            // LOGO CENTRADO (Arriba)
            // ═══════════════════════════════════════════════════════════════
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
                        Size = new Size(350, 200),
                        BackColor = Color.Transparent,
                        Anchor = AnchorStyles.Top
                    };
                    panelPrincipal.Controls.Add(picLogo);
                    picLogo.BringToFront();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando logo: {ex.Message}");
            }

            // ═══════════════════════════════════════════════════════════════
            // CONTENEDOR CENTRAL (Para centrar todo el contenido)
            // ═══════════════════════════════════════════════════════════════
            Panel panelCentral = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Transparent
            };
            panelPrincipal.Controls.Add(panelCentral);

            // ═══════════════════════════════════════════════════════════════
            // TÍTULO DEL MÓDULO
            // ═══════════════════════════════════════════════════════════════
            Label lblTitulo = new Label
            {
                Text = "CUENTAS POR PAGAR",
                Font = new Font("Segoe UI", 36, FontStyle.Bold),
                ForeColor = ColorPrimario,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelCentral.Controls.Add(lblTitulo);

            // ═══════════════════════════════════════════════════════════════
            // SUBTÍTULO
            // ═══════════════════════════════════════════════════════════════
            Label lblSubtitulo = new Label
            {
                Text = "Gestión de Pagos, Solicitudes y Documentos",
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelCentral.Controls.Add(lblSubtitulo);

            // ═══════════════════════════════════════════════════════════════
            // PANEL PARA LAS CARDS
            // ═══════════════════════════════════════════════════════════════
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

            // ═══════════════════════════════════════════════════════════════
            // DEFINIR LAS 2 CARDS DEL MÓDULO
            // ═══════════════════════════════════════════════════════════════
            var cards = new[]
            {
                new {
                    Nombre = "CARTAS Y SOLICITUDES",
                    Icono = "📝",
                    Descripcion = "Solicitudes de pago, certificados,\nrelaciones y documentos",
                    ColorCard = Color.FromArgb(0, 120, 212)  // Azul
                },
                new {
                    Nombre = "CONFIGURACIÓN",
                    Icono = "⚙️",
                    Descripcion = "Firmas digitales, parámetros\ny formatos de impresión",
                    ColorCard = Color.FromArgb(107, 107, 107)  // Gris
                }
            };

            // ═══════════════════════════════════════════════════════════════
            // CREAR CADA CARD
            // ═══════════════════════════════════════════════════════════════
            foreach (var card in cards)
            {
                // Verificar permisos (todos pueden ver el dashboard)
                CardControl cardControl = new CardControl
                {
                    Titulo = card.Nombre,
                    Icono = card.Icono,
                    Descripcion = card.Descripcion,
                    Size = new Size(320, 280),
                    Margin = new Padding(30)
                };

                // Evento click
                string nombreCard = card.Nombre;
                cardControl.CardClick += (s, e) => Card_Click(nombreCard);

                panelCards.Controls.Add(cardControl);
            }

            // ═══════════════════════════════════════════════════════════════
            // EVENTOS DE LAYOUT
            // ═══════════════════════════════════════════════════════════════

            // Centrar contenido cuando se redimensiona
            panelPrincipal.Resize += (s, e) => CentrarContenido(panelPrincipal, panelCentral, picLogo);

            // Layout inicial
            this.Load += (s, e) => {
                LayoutContenido(lblTitulo, lblSubtitulo, panelCards);
                CentrarContenido(panelPrincipal, panelCentral, picLogo);
            };
        }

        /// <summary>
        /// Posiciona los elementos dentro del panel central
        /// </summary>
        private void LayoutContenido(Label titulo, Label subtitulo, FlowLayoutPanel panelCards)
        {
            int anchoTotal = Math.Max(titulo.Width, Math.Max(subtitulo.Width, panelCards.PreferredSize.Width));

            // Centrar título
            titulo.Location = new Point(
                (anchoTotal - titulo.Width) / 2,
                20
            );

            // Centrar subtítulo
            subtitulo.Location = new Point(
                (anchoTotal - subtitulo.Width) / 2,
                titulo.Bottom + 10
            );

            // Posicionar panel de cards
            panelCards.Location = new Point(
                (anchoTotal - panelCards.PreferredSize.Width) / 2,
                subtitulo.Bottom + 20
            );
        }

        /// <summary>
        /// Centra el contenido en el panel principal
        /// </summary>
        private void CentrarContenido(Panel contenedor, Panel contenido, PictureBox logo)
        {
            // Centrar panel de contenido
            int x = Math.Max(0, (contenedor.ClientSize.Width - contenido.Width) / 2);
            int y = Math.Max(120, (contenedor.ClientSize.Height - contenido.Height) / 2);
            contenido.Location = new Point(x, y);

            // Centrar logo
            if (logo != null)
            {
                logo.Location = new Point(
                    (contenedor.ClientSize.Width - logo.Width) / 2,
                    -30
                );
            }
        }

        /// <summary>
        /// Maneja el click en una card
        /// </summary>
        private void Card_Click(string nombreCard)
        {
            switch (nombreCard.ToUpper())
            {
                case "CARTAS Y SOLICITUDES":
                    AbrirCartasSolicitudes();
                    break;

                case "CONFIGURACIÓN":
                    AbrirConfiguracion();
                    break;
            }
        }

        /// <summary>
        /// Abre el submódulo de Cartas y Solicitudes
        /// </summary>
        private void AbrirCartasSolicitudes()
        {
            try
            {
                FormMenuCartasSolicitudes formMenu = new FormMenuCartasSolicitudes(formPrincipal);
                formPrincipal.CargarContenidoPanel(formMenu);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al abrir Cartas y Solicitudes:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Abre el submódulo de Configuración
        /// </summary>
        private void AbrirConfiguracion()
        {
            try
            {
                // TODO: Crear FormConfiguracionCXP
                // FormConfiguracionCXP formConfig = new FormConfiguracionCXP(formPrincipal);
                // formPrincipal.CargarContenidoPanel(formConfig);

                // Temporal mientras se implementa
                MessageBox.Show(
                    "Configuración de Cuentas por Pagar\n\n" +
                    "• Gestión de Firmas Digitales\n" +
                    "• Parámetros del Módulo\n" +
                    "• Formatos de Impresión\n" +
                    "• Gestión de Fideicomisos\n" +
                    "• Gestión de Proveedores\n\n" +
                    "(Próximamente)",
                    "Configuración CXP",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al abrir Configuración:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Vuelve al dashboard de Contabilidad
        /// </summary>
        private void BtnVolver_Click(object sender, EventArgs e)
        {
            try
            {
                FormDashboardContabilidad dashboardContabilidad = new FormDashboardContabilidad(formPrincipal);
                formPrincipal.CargarContenidoPanel(dashboardContabilidad);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al volver:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}