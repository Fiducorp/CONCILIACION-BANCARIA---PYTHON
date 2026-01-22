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
using MOFIS_ERP.Forms.Contabilidad.CuentasPorPagar;

namespace MOFIS_ERP.Forms.Contabilidad
{
    public partial class FormDashboardContabilidad : Form
    {
        private FormMain formPrincipal;

        public FormDashboardContabilidad(FormMain formMain)
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

            // Logo centrado arriba (carga desde Resources, con fallback)
            PictureBox picLogo = null;
            try
            {
                Image logoImage = null;

                // Intentar la propiedad fuertemente tipada
                try { logoImage = Properties.Resources.LOGO as Image; } catch { logoImage = null; }

                // Fallback: buscar por nombre en ResourceManager (variantes)
                if (logoImage == null)
                {
                    try
                    {
                        object obj = Properties.Resources.ResourceManager.GetObject("LOGO", Properties.Resources.Culture)
                                     ?? Properties.Resources.ResourceManager.GetObject("logo", Properties.Resources.Culture);
                        logoImage = obj as Image;
                    }
                    catch { logoImage = null; }
                }

                // Último fallback: recurso conocido existente
                if (logoImage == null)
                {
                    try { logoImage = Properties.Resources.icon_Adv as Image; } catch { logoImage = null; }
                }

                if (logoImage != null)
                {
                    picLogo = new PictureBox
                    {
                        Image = logoImage,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Size = new Size(400, 250),
                        BackColor = Color.Transparent,
                        Anchor = AnchorStyles.Top
                    };
                    panelPrincipal.Controls.Add(picLogo);
                    picLogo.BringToFront();

                    // Centrar horizontalmente y mantener en resize (igual que en los otros dashboards)
                    panelPrincipal.Resize += (s, e) =>
                    {
                        if (picLogo != null)
                            picLogo.Location = new Point((panelPrincipal.ClientSize.Width - picLogo.Width) / 2, -40);
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando logo desde Resources: {ex.Message}");
            }   

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
                Text = "CONTABILIDAD",
                Font = new Font("Segoe UI", 36, FontStyle.Bold),
                ForeColor = Color.FromArgb(16, 124, 16), // Verde contable
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelCentral.Controls.Add(lblCategoria);

            // Subtítulo
            Label lblSubtitulo = new Label
            {
                Text = "Módulos de Gestión Contable y Financiera",
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

            // Definir los módulos de Contabilidad
            var modulos = new[]
            {
                new { Nombre = "CUENTAS POR PAGAR", Icono = "💳", Descripcion = "Gestión de pagos" },
                new { Nombre = "RECAUDO", Icono = "💰", Descripcion = "Control de ingresos" },
                new { Nombre = "IMPUESTOS", Icono = "📑", Descripcion = "Administración tributaria" },
                new { Nombre = "CONCILIACIONES", Icono = "🔄", Descripcion = "Conciliaciones bancarias" }
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
                case "CUENTAS POR PAGAR":
                    AbrirCuentasPorPagar();
                    break;

                case "RECAUDO":
                    AbrirRecaudo();
                    break;

                case "IMPUESTOS":
                    AbrirImpuestos();
                    break;

                case "CONCILIACIONES":
                    AbrirConciliaciones();
                    break;
            }
        }

        /// <summary>
        /// Abre el Módulo de Cuentas por Pagar
        /// </summary>
        private void AbrirCuentasPorPagar()
        {
            try
            {
                FormDashboardCuentasPorPagar dashboardCXP = new FormDashboardCuentasPorPagar(formPrincipal);
                formPrincipal.CargarContenidoPanel(dashboardCXP);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al abrir Cuentas por Pagar:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Abre el Módulo de Recaudo
        /// </summary>
        private void AbrirRecaudo()
        {
            MessageBox.Show(
                "Módulo de Recaudo\n(Próximamente)",
                "En Desarrollo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        /// <summary>
        /// Abre el Módulo de Impuestos
        /// </summary>
        private void AbrirImpuestos()
        {
            MessageBox.Show(
                "Módulo de Impuestos\n(Próximamente)",
                "En Desarrollo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        /// <summary>
        /// Abre el Módulo de Conciliaciones
        /// </summary>
        private void AbrirConciliaciones()
        {
            MessageBox.Show(
                "Módulo de Conciliaciones\n(Próximamente)",
                "En Desarrollo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
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