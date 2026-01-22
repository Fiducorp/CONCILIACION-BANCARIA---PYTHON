using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MOFIS_ERP.Forms;

namespace MOFIS_ERP.Forms.Contabilidad.CuentasPorPagar.CartasSolicitudes
{
    public partial class FormMenuCartasSolicitudes : Form
    {

        // ═══════════════════════════════════════════════════════════════
        // CONFIGURACIÓN DE ESTILOS (AJUSTA A TU GUSTO)
        // ═══════════════════════════════════════════════════════════════
        // Campos
        private FormMain formPrincipal;
        private Timer timerReloj;
        private Timer timerAnimacion;
        private bool primeraSeleccion = false;
        private bool menuExpandido = true;
        private bool animacionEnProgreso = false;
        private Button botonSeleccionado = null;

        // Variables para pantalla de carga
        private int loadingDots = 0;
        private Form formularioPendiente = null;


        // Dimensiones del menú (TUS MEDIDAS)
        private int MENU_ANCHO_EXPANDIDO = 391;
        private int MENU_ANCHO_CONTRAIDO = 100;
        private int BOTON_ANCHO_EXPANDIDO = 360;
        private int BOTON_ANCHO_CONTRAIDO = 70;
        // private int BOTON_ALTO = 65; // Altura fija para todos los botones (No se usa)

        // Tamaños de fuente (TUS MEDIDAS)
        private readonly float FUENTE_BOTON_EXPANDIDO = 16f;
        private readonly float FUENTE_BOTON_CONTRAIDO = 20f;  // Más grande para iconos

        // Colores para hover
        private readonly Color colorBotonNormal = Color.FromArgb(41, 70, 125);
        private readonly Color colorBotonHover = Color.FromArgb(0, 120, 212);
        private readonly Color colorTextoNormal = Color.FromArgb(200, 200, 210);
        private readonly Color colorTextoHover = Color.White;
        private readonly Color colorIconoContraido = Color.White;
        private readonly Color colorBotonSeleccionado = Color.FromArgb(0, 90, 160);

        // ═══════════════════════════════════════════════════════════════
        // CONFIGURACIÓN DE VELOCIDADES (AJUSTA A TU GUSTO)
        // ═══════════════════════════════════════════════════════════════
        private readonly int VELOCIDAD_MENU_MS = 50;           // Duración animación menú
        // private readonly int VELOCIDAD_HOVER_MS = 50;          // Duración animación hover (No se usa)
        private readonly int VELOCIDAD_FADE_MS = 50;           // Duración fade in/out
        private readonly int DELAY_CASCADA_MS = 5;             // Delay entre cada botón en cascada
        private readonly int INTERVALO_ANIMACION = 5;          // Intervalo del timer (más bajo = más suave)

        // ═══════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ═══════════════════════════════════════════════════════════════
        public FormMenuCartasSolicitudes(FormMain principal)
        {
            InitializeComponent();
            formPrincipal = principal;

            ConfigurarFormulario();
            ConfigurarEventosHover();
            ConfigurarEventosClick();
            ConfigurarMenuContraible();
            IniciarReloj();
            CargarDatosResumen();
        }

        // Constructor sin parámetros para el diseñador
        public FormMenuCartasSolicitudes()
        {
            InitializeComponent();
        }

        // ═══════════════════════════════════════════════════════════════
        // CONFIGURACIÓN INICIAL
        // ═══════════════════════════════════════════════════════════════
        private void ConfigurarFormulario()
        {
            this.Dock = DockStyle.Fill;

            // IMPORTANTE: Traer el menú al frente para que quede sobre el área de trabajo
            panelMenu.BringToFront();

            // NUEVO: Configurar posición inicial del área de trabajo
            ConfigurarAreaTrabajo();

            // Cargar nombre del usuario
            lblBienvenidaUsuario.Text = $"Bienvenido, {ObtenerNombreUsuario()}";

            // Cargar último inicio
            lblUltimoInicioFecha.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");

            // Cargar logos
            CargarLogos();

            // NUEVO: Configurar timer de carga
            if (timerCarga != null)
            {
                timerCarga.Tick += TimerCarga_Tick;
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // CONFIGURAR ÁREA DE TRABAJO (respeta espacio del menú contraído)
        // ═══════════════════════════════════════════════════════════════
        private void ConfigurarAreaTrabajo()
        {
            // Establecer margen izquierdo igual al ancho del menú contraído
            panelAreaTrabajo.Left = MENU_ANCHO_CONTRAIDO;
            panelAreaTrabajo.Top = 0;
            panelAreaTrabajo.Width = this.ClientSize.Width - MENU_ANCHO_CONTRAIDO;
            panelAreaTrabajo.Height = this.ClientSize.Height;

            // Anchor para que se ajuste al redimensionar
            panelAreaTrabajo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }

        // ═══════════════════════════════════════════════════════════════
        // ANIMACIÓN DE PANTALLA DE CARGA
        // ═══════════════════════════════════════════════════════════════
        private void TimerCarga_Tick(object sender, EventArgs e)
        {
            loadingDots++;
            if (loadingDots > 3) loadingDots = 0;

            string dots = new string('.', loadingDots);
            lblCargando.Text = $"Cargando{dots}";

            // Rotar el ícono de carga (opcional, si tienes una imagen)
            if (picLoadingIcon.Image != null)
            {
                picLoadingIcon.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                picLoadingIcon.Refresh();
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // MOSTRAR PANTALLA DE CARGA
        // ═══════════════════════════════════════════════════════════════
        private void MostrarPantallaCarga(string nombreFormulario)
        {
            if (panelCargando == null) return;

            // Configurar texto
            lblNombreFormulario.Text = nombreFormulario;
            lblCargando.Text = "Cargando";
            loadingDots = 0;

            // Centrar controles
            CentrarControlesCarga();

            // Mostrar panel y traer al frente
            panelCargando.Visible = true;
            panelCargando.BringToFront();

            // Iniciar animación
            if (timerCarga != null)
            {
                timerCarga.Start();
            }

            // Forzar actualización visual
            panelCargando.Refresh();
            Application.DoEvents();
        }

        // ═══════════════════════════════════════════════════════════════
        // OCULTAR PANTALLA DE CARGA
        // ═══════════════════════════════════════════════════════════════
        private void OcultarPantallaCarga()
        {
            if (timerCarga != null)
            {
                timerCarga.Stop();
            }

            if (panelCargando != null)
            {
                panelCargando.Visible = false;
                panelCargando.SendToBack();
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // CENTRAR CONTROLES DE CARGA
        // ═══════════════════════════════════════════════════════════════
        private void CentrarControlesCarga()
        {
            if (panelCargando == null) return;

            int centerX = panelCargando.Width / 2;
            int centerY = panelCargando.Height / 2;

            // Centrar ícono
            if (picLoadingIcon != null)
            {
                picLoadingIcon.Left = centerX - (picLoadingIcon.Width / 2);
                picLoadingIcon.Top = centerY - 120;
            }

            // Centrar label "Cargando..."
            if (lblCargando != null)
            {
                lblCargando.Left = centerX - (lblCargando.Width / 2);
                lblCargando.Top = centerY - 30;
            }

            // Centrar label nombre formulario
            if (lblNombreFormulario != null)
            {
                lblNombreFormulario.Left = centerX - (lblNombreFormulario.Width / 2);
                lblNombreFormulario.Top = centerY + 20;
            }
        }



        // ═══════════════════════════════════════════════════════════════
        // MENÚ CONTRAÍBLE
        // ═══════════════════════════════════════════════════════════════
        private void ConfigurarMenuContraible()
        {
            // Guardar ancho original del menú
            MENU_ANCHO_EXPANDIDO = panelMenu.Width;

            // Evento click del botón toggle
            btnToggleMenu.Click += BtnToggleMenu_Click;

            // Hover del botón toggle
            btnToggleMenu.MouseEnter += (s, e) => btnToggleMenu.BackColor = Color.FromArgb(40, 40, 52);
            btnToggleMenu.MouseLeave += (s, e) => btnToggleMenu.BackColor = Color.Transparent;

            // Click en área de trabajo para contraer
            panelAreaTrabajo.Click += (s, e) =>
            {
                if (menuExpandido && primeraSeleccion)
                {
                    ContraerMenu();
                }
            };
        }

        private void BtnToggleMenu_Click(object sender, EventArgs e)
        {
            if (menuExpandido)
                ContraerMenu();
            else
                ExpandirMenu();
        }

        private void ContraerMenu()
        {
            if (!menuExpandido || animacionEnProgreso) return;

            menuExpandido = false;
            btnToggleMenu.Text = "☰";

            // Primero ocultar textos con animación
            OcultarTextoBotones();

            // Luego animar el menú
            AnimarMenuSuave(MENU_ANCHO_CONTRAIDO);
        }

        private void ExpandirMenu()
        {
            if (menuExpandido || animacionEnProgreso) return;

            menuExpandido = true;
            btnToggleMenu.Text = "✕";

            // Animar el menú primero
            AnimarMenuSuave(MENU_ANCHO_EXPANDIDO, () =>
            {
                // Cuando termine, mostrar textos con animación
                MostrarTextoBotones();
            });
        }

        private void AnimarMenu(int anchoObjetivo)
        {
            // Detener animación anterior si existe
            timerAnimacion?.Stop();
            timerAnimacion?.Dispose();

            int anchoActual = panelMenu.Width;
            int diferencia = anchoObjetivo - anchoActual;
            int pasos = 10;
            int paso = diferencia / pasos;

            if (paso == 0)
            {
                panelMenu.Width = anchoObjetivo;
                return;
            }

            int contadorPasos = 0;

            timerAnimacion = new Timer();
            timerAnimacion.Interval = 20;
            timerAnimacion.Tick += (s, e) =>
            {
                contadorPasos++;

                if (contadorPasos >= pasos)
                {
                    panelMenu.Width = anchoObjetivo;
                    timerAnimacion.Stop();
                    timerAnimacion.Dispose();
                    timerAnimacion = null;
                }
                else
                {
                    panelMenu.Width += paso;
                }
            };

            timerAnimacion.Start();
        }

        private void OcultarTextoBotones()
        {
            Button[] botones = { btnInicio, btnSolicitud, btnCertificado, btnRelacionPago,
                         btnAnticipos, btnDesistimiento, btnConsulta, btnConfiguracion };

            // Calcular posición centrada
            int posicionCentrada = (MENU_ANCHO_CONTRAIDO - BOTON_ANCHO_CONTRAIDO) / 2;

            // Aplicar a cada botón directamente (sin delay para evitar problemas)
            foreach (Button btn in botones)
            {
                // Guardar texto original
                if (string.IsNullOrEmpty(btn.AccessibleDescription))
                {
                    btn.AccessibleDescription = btn.Text;
                }

                // Extraer solo el emoji
                string texto = btn.Text.Trim();
                int espacioIndex = texto.IndexOf("   ");
                if (espacioIndex > 0)
                {
                    btn.Text = texto.Substring(0, espacioIndex).Trim();
                }

                // Aplicar propiedades
                btn.Width = BOTON_ANCHO_CONTRAIDO;
                btn.Left = posicionCentrada;
                btn.TextAlign = ContentAlignment.MiddleCenter;
                btn.Padding = new Padding(0);
                btn.Font = new Font("Segoe UI", FUENTE_BOTON_CONTRAIDO, FontStyle.Regular);

                // Color según si está seleccionado o no
                if (btn == botonSeleccionado)
                {
                    btn.BackColor = colorBotonSeleccionado;
                    btn.ForeColor = colorTextoHover;
                }
                else
                {
                    btn.BackColor = colorBotonNormal;
                    btn.ForeColor = colorIconoContraido;
                }
            }

            // Ocultar elementos del encabezado
            lblTitulo.Visible = false;
            lblSubtitulo.Visible = false;
            picLogo.Visible = false;
            picLogo.Dock = DockStyle.None;

            // Cambiar Dock del botón toggle
            btnToggleMenu.Dock = DockStyle.Fill;

            // Ajustar botón volver - más grande y centrado
            btnVolver.Text = "←";
            btnVolver.TextAlign = ContentAlignment.MiddleCenter;
            btnVolver.Font = new Font("Segoe UI", 18f, FontStyle.Bold);
            btnVolver.Width = MENU_ANCHO_CONTRAIDO - 20;
            btnVolver.Left = 10;
        }

        private void MostrarTextoBotones()
        {
            Button[] botones = { btnInicio, btnSolicitud, btnCertificado, btnRelacionPago,
                         btnAnticipos, btnDesistimiento, btnConsulta, btnConfiguracion };

            // Animar cada botón con delay escalonado
            for (int i = 0; i < botones.Length; i++)
            {
                AnimarBotonAExpandido(botones[i], i);
            }

            // Mostrar elementos del encabezado
            lblTitulo.Visible = true;
            lblSubtitulo.Visible = true;
            picLogo.Dock = DockStyle.Left;
            picLogo.Width = 106;
            picLogo.Visible = true;

            // Cambiar Dock del botón toggle a Right (estado inicial)
            btnToggleMenu.Dock = DockStyle.Right;

            // Restaurar botón volver
            btnVolver.Text = "←  Volver";
            btnVolver.TextAlign = ContentAlignment.MiddleCenter;
            btnVolver.Font = new Font("Segoe UI", 16f, FontStyle.Regular);
            btnVolver.Width = MENU_ANCHO_EXPANDIDO - 40;
            btnVolver.Left = 20;
        }

        private void AnimarFadeOut(Control control)
        {
            Timer timer = new Timer();
            timer.Interval = 50;
            int pasos = 5;
            int pasoActual = 0;

            timer.Tick += (s, e) =>
            {
                pasoActual++;
                if (pasoActual >= pasos)
                {
                    timer.Stop();
                    timer.Dispose();
                    control.Visible = false;
                }
            };
            timer.Start();
        }

        private void AnimarFadeIn(Control control)
        {
            control.Visible = true;

            Timer timer = new Timer();
            timer.Interval = 50;
            int pasos = 5;
            int pasoActual = 0;

            timer.Tick += (s, e) =>
            {
                pasoActual++;
                if (pasoActual >= pasos)
                {
                    timer.Stop();
                    timer.Dispose();
                }
            };
            timer.Start();
        }

        private void AnimarPosicionControl(Control control, int xObjetivo)
        {
            int xInicial = control.Left;
            int diferencia = xObjetivo - xInicial;
            int pasos = 10;
            int pasoActual = 0;

            Timer timer = new Timer();
            timer.Interval = 20;
            timer.Tick += (s, e) =>
            {
                pasoActual++;

                double progreso = (double)pasoActual / pasos;
                double easing = 1 - Math.Pow(1 - progreso, 2);

                control.Left = xInicial + (int)(diferencia * easing);

                if (pasoActual >= pasos)
                {
                    timer.Stop();
                    timer.Dispose();
                    control.Left = xObjetivo;
                }
            };
            timer.Start();
        }

        // ══════════════════════════════════════════════════════════════
        // CARGAR LOGOS DESDE RESOURCES
        // ══════════════════════════════════════════════════════════════
        private void CargarLogos()
        {
            try
            {
                // Logo tipo (pequeño) para el menú lateral
                string rutaLogotipo = System.IO.Path.Combine(Application.StartupPath, "Resources", "LOGOTIPO.png");
                if (System.IO.File.Exists(rutaLogotipo))
                {
                    picLogo.Image = Image.FromFile(rutaLogotipo);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Logotipo no encontrado en: {rutaLogotipo}");
                }

                // Logo completo para la pantalla de bienvenida
                string rutaLogoCompleto = System.IO.Path.Combine(Application.StartupPath, "Resources", "MOFIS ERP -LOGO.png");
                if (System.IO.File.Exists(rutaLogoCompleto))
                {
                    picLogoBienvenida.Image = Image.FromFile(rutaLogoCompleto);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Logo completo no encontrado en: {rutaLogoCompleto}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar logos: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // EFECTOS HOVER PARA BOTONES DEL MENÚ
        // ═══════════════════════════════════════════════════════════════
        private void ConfigurarEventosHover()
        {
            // Lista de botones del menú
            Button[] botonesMenu = new Button[]
            {
                btnInicio,
                btnSolicitud,
                btnCertificado,
                btnRelacionPago,
                btnAnticipos,
                btnDesistimiento,
                btnConsulta,
                btnConfiguracion
            };

            foreach (Button btn in botonesMenu)
            {
                btn.MouseEnter += BotonMenu_MouseEnter;
                btn.MouseLeave += BotonMenu_MouseLeave;
            }

            // Hover para botones de accesos rápidos
            btnNuevaSolicitud.MouseEnter += BotonAcceso_MouseEnter;
            btnNuevaSolicitud.MouseLeave += BotonAcceso_MouseLeave;

            btnBuscar.MouseEnter += BotonAcceso_MouseEnter;
            btnBuscar.MouseLeave += BotonAcceso_MouseLeave;

            btnActividadHoy.MouseEnter += BotonAcceso_MouseEnter;
            btnActividadHoy.MouseLeave += BotonAcceso_MouseLeave;

            btnReporteRapido.MouseEnter += BotonAcceso_MouseEnter;
            btnReporteRapido.MouseLeave += BotonAcceso_MouseLeave;

            btnExportar.MouseEnter += BotonAcceso_MouseEnter;
            btnExportar.MouseLeave += BotonAcceso_MouseLeave;

            // Hover para botón volver
            btnVolver.MouseEnter += BtnVolver_MouseEnter;
            btnVolver.MouseLeave += BtnVolver_MouseLeave;
        }

        // ═══════════════════════════════════════════════════════════════
        // EVENTOS HOVER - BOTONES MENÚ
        // ═══════════════════════════════════════════════════════════════
        private void BotonMenu_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            // Si no es el botón seleccionado, aplicar hover
            if (btn != botonSeleccionado)
            {
                btn.BackColor = colorBotonHover;
                btn.ForeColor = colorTextoHover;
            }

            // Negrita siempre en hover (incluso si está seleccionado)
            btn.Font = new Font(btn.Font.FontFamily, btn.Font.Size, FontStyle.Bold);
        }

        private void BotonMenu_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            // Si no es el botón seleccionado, quitar hover y negrita
            if (btn != botonSeleccionado)
            {
                btn.BackColor = colorBotonNormal;
                btn.ForeColor = menuExpandido ? colorTextoNormal : colorIconoContraido;
                btn.Font = new Font(btn.Font.FontFamily, btn.Font.Size, FontStyle.Regular);
            }
            // Si es el seleccionado, mantener negrita y colores de selección
        }

        private void SeleccionarBoton(Button btn)
        {
            // Deseleccionar el anterior
            if (botonSeleccionado != null && botonSeleccionado != btn)
            {
                Color colorTextoAnterior = menuExpandido ? colorTextoNormal : colorIconoContraido;
                botonSeleccionado.BackColor = colorBotonNormal;
                botonSeleccionado.ForeColor = colorTextoAnterior;
                botonSeleccionado.Font = new Font(botonSeleccionado.Font.FontFamily, botonSeleccionado.Font.Size, FontStyle.Regular);
            }

            // Seleccionar el nuevo
            botonSeleccionado = btn;
            btn.BackColor = colorBotonSeleccionado;
            btn.ForeColor = colorTextoHover;
            btn.Font = new Font(btn.Font.FontFamily, btn.Font.Size, FontStyle.Bold);
        }

        // ═══════════════════════════════════════════════════════════════
        // EVENTOS HOVER - BOTONES ACCESO RÁPIDO
        // ═══════════════════════════════════════════════════════════════
        private void BotonAcceso_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            btn.BackColor = Color.FromArgb(0, 90, 170); // Azul más oscuro en hover
        }

        private void BotonAcceso_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            btn.BackColor = Color.FromArgb(0, 120, 212);
        }

        private void BotonAccesoGris_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            btn.BackColor = Color.FromArgb(80, 80, 90);
        }

        private void BotonAccesoGris_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            btn.BackColor = Color.FromArgb(100, 100, 110);
        }

        private void BtnVolver_MouseEnter(object sender, EventArgs e)
        {
            btnVolver.BackColor = Color.FromArgb(80, 80, 90);
        }

        private void BtnVolver_MouseLeave(object sender, EventArgs e)
        {
            btnVolver.BackColor = Color.FromArgb(60, 60, 70);
        }

        // ═══════════════════════════════════════════════════════════════
        // EVENTOS CLICK
        // ═══════════════════════════════════════════════════════════════
        private void ConfigurarEventosClick()
        {
            // Botones del menú
            btnInicio.Click += BtnInicio_Click;
            btnSolicitud.Click += BtnSolicitud_Click;
            btnCertificado.Click += BtnCertificado_Click;
            btnRelacionPago.Click += BtnRelacionPago_Click;
            btnAnticipos.Click += BtnAnticipos_Click;
            btnDesistimiento.Click += BtnDesistimiento_Click;
            btnConsulta.Click += BtnConsulta_Click;
            btnConfiguracion.Click += BtnConfiguracion_Click;
            btnVolver.Click += BtnVolver_Click;

            // Botones de accesos rápidos
            btnNuevaSolicitud.Click += BtnNuevaSolicitud_Click;
            btnBuscar.Click += BtnBuscar_Click;
            btnActividadHoy.Click += BtnActividadHoy_Click;
            btnReporteRapido.Click += BtnReporteRapido_Click;
            btnExportar.Click += BtnExportar_Click;
        }

        // ═══════════════════════════════════════════════════════════════
        // LIMPIAR FORMULARIOS ANTERIORES (evita memoria y animaciones rotas)
        // ═══════════════════════════════════════════════════════════════
        private void LimpiarFormulariosAnteriores()
        {
            // Recorrer desde el final para evitar problemas de índice
            for (int i = panelAreaTrabajo.Controls.Count - 1; i >= 0; i--)
            {
                Control control = panelAreaTrabajo.Controls[i];

                // Si es un Form, removerlo y liberarlo
                if (control is Form)
                {
                    panelAreaTrabajo.Controls.Remove(control);
                    control.Dispose();
                }
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // ACCIONES - BOTONES MENÚ
        // ═══════════════════════════════════════════════════════════════
        private void BtnInicio_Click(object sender, EventArgs e)
        {
            // Ir al dashboard principal de categorías
            try
            {
                var dashboard = new FormDashboardCategorias(formPrincipal);
                CargarEnPanel(dashboard);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al ir al inicio:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSolicitud_Click(object sender, EventArgs e)
        {
            SeleccionarBoton(btnSolicitud);

            // PRIMERO: Contraer el menú si está expandido
            if (menuExpandido)
            {
                ContraerMenu();
            }

            // SEGUNDO: Mostrar pantalla de carga
            MostrarPantallaCarga("Solicitud de Pago");

            // TERCERO: Fade out de la bienvenida
            AnimarFadeOutBienvenida(() =>
            {
                // Ocultar controles de bienvenida
                picLogoBienvenida.Visible = false;
                lblTituloBienvenida.Visible = false;
                lblBienvenidaUsuario.Visible = false;
                panelResumen.Visible = false;
                panelAccesos.Visible = false;
                lblFechaHora.Visible = false;

                // CUARTO: Limpiar formularios anteriores
                LimpiarFormulariosAnteriores();

                // QUINTO: Crear formulario en segundo plano
                FormSolicitudPago formSolicitud = new FormSolicitudPago();
                formSolicitud.TopLevel = false;
                formSolicitud.FormBorderStyle = FormBorderStyle.None;
                formSolicitud.Dock = DockStyle.Fill;

                // Manejar cuando se cierra el formulario
                formSolicitud.FormClosed += (s, args) =>
                {
                    MostrarPantallaBienvenida();
                };

                // SEXTO: Agregar al panel pero mantenerlo oculto
                formSolicitud.Visible = false;
                panelAreaTrabajo.Controls.Add(formSolicitud);

                // SÉPTIMO: Esperar a que se cargue completamente
                Timer timerEspera = new Timer();
                timerEspera.Interval = 300; // Dar tiempo a que se renderice
                timerEspera.Tick += (s, args) =>
                {
                    timerEspera.Stop();
                    timerEspera.Dispose();

                    // Mostrar formulario
                    formSolicitud.Visible = true;
                    formSolicitud.BringToFront();
                    formSolicitud.Show();

                    // Ocultar pantalla de carga
                    OcultarPantallaCarga();
                };
                timerEspera.Start();
            });
        }

        private void BtnCertificado_Click(object sender, EventArgs e)
        {
            SeleccionarBoton(btnCertificado);
            // Contraer menú si está expandido
            if (menuExpandido)
            {
                ContraerMenu();
            }
            MarcarPrimeraSeleccion();
            MostrarEnAreaTrabajo("📄", "Certificado de Retención", "Formulario en desarrollo...");
        }

        private void BtnRelacionPago_Click(object sender, EventArgs e)
        {
            SeleccionarBoton(btnRelacionPago);
            // Contraer menú si está expandido
            if (menuExpandido)
            {
                ContraerMenu();
            }
            MarcarPrimeraSeleccion();
            MostrarEnAreaTrabajo("💳", "Relación de Pago", "Formulario en desarrollo...");
        }

        private void BtnAnticipos_Click(object sender, EventArgs e)
        {
            SeleccionarBoton(btnAnticipos);
            // Contraer menú si está expandido
            if (menuExpandido)
            {
                ContraerMenu();
            }
            MarcarPrimeraSeleccion();
            MostrarEnAreaTrabajo("💰", "Relación de Anticipos", "Formulario en desarrollo...");
        }

        private void BtnDesistimiento_Click(object sender, EventArgs e)
        {
            SeleccionarBoton(btnDesistimiento);
            // Contraer menú si está expandido
            if (menuExpandido)
            {
                ContraerMenu();
            }
            MarcarPrimeraSeleccion();
            MostrarEnAreaTrabajo("✉️", "Carta de Desistimiento", "Formulario en desarrollo...");
        }

        private void BtnConsulta_Click(object sender, EventArgs e)
        {
            SeleccionarBoton(btnConsulta);
            // Contraer menú si está expandido
            if (menuExpandido)
            {
                ContraerMenu();
            }
            MarcarPrimeraSeleccion();
            MostrarEnAreaTrabajo("🔍", "Consulta de Solicitudes", "Formulario en desarrollo...");
        }

        private void BtnConfiguracion_Click(object sender, EventArgs e)
        {
            SeleccionarBoton(btnConfiguracion);
            // Contraer menú si está expandido
            if (menuExpandido)
            {
                ContraerMenu();
            }
            MarcarPrimeraSeleccion();
            MostrarEnAreaTrabajo("⚙️", "Configuración", "Formulario en desarrollo...");
        }

        private void BtnVolver_Click(object sender, EventArgs e)
        {
            // Volver al dashboard de Cuentas por Pagar
            try
            {
                var dashboard = new FormDashboardCuentasPorPagar(formPrincipal);
                CargarEnPanel(dashboard);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al volver:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // ACCIONES - BOTONES ACCESO RÁPIDO
        // ═══════════════════════════════════════════════════════════════
        private void BtnNuevaSolicitud_Click(object sender, EventArgs e)
        {
            BtnSolicitud_Click(sender, e);
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            BtnConsulta_Click(sender, e);
        }

        private void BtnActividadHoy_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Actividad de Hoy\n\nEsta función mostrará un resumen de todas las acciones realizadas hoy en el módulo Cuentas por Pagar.\n\n(Próximamente)",
                "Actividad de Hoy", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnReporteRapido_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Reporte Rápido\n\nEsta función generará un reporte resumido de las solicitudes de pago.\n\n(Próximamente)",
                "Reporte Rápido", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Exportar\n\nEsta función permitirá exportar datos a Excel o PDF.\n\n(Próximamente)",
                "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ═══════════════════════════════════════════════════════════════
        // MÉTODOS AUXILIARES
        // ═══════════════════════════════════════════════════════════════
        private void MarcarPrimeraSeleccion()
        {
            if (!primeraSeleccion)
            {
                primeraSeleccion = true;
                btnToggleMenu.Visible = true;
            }

            // Contraer el menú después de un pequeño delay para que los botones se configuren primero
            if (menuExpandido)
            {
                Timer timerContraer = new Timer();
                timerContraer.Interval = 50;
                timerContraer.Tick += (s, e) =>
                {
                    timerContraer.Stop();
                    timerContraer.Dispose();
                    ContraerMenu();
                };
                timerContraer.Start();
            }
        }

        private void MostrarEnAreaTrabajo(string icono, string titulo, string mensaje)
        {
            // Mostrar pantalla de carga
            MostrarPantallaCarga(titulo);

            // Fade out suave de la pantalla de bienvenida
            AnimarFadeOutBienvenida(() =>
            {
                // LIMPIAR formularios anteriores
                LimpiarFormulariosAnteriores();

                // Mostrar placeholder del formulario
                Label lblPlaceholder = panelAreaTrabajo.Controls["lblPlaceholder"] as Label;

                if (lblPlaceholder == null)
                {
                    lblPlaceholder = new Label
                    {
                        Name = "lblPlaceholder",
                        Font = new Font("Segoe UI", 24, FontStyle.Regular),
                        ForeColor = Color.FromArgb(150, 150, 160),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Fill
                    };
                    panelAreaTrabajo.Controls.Add(lblPlaceholder);
                }

                lblPlaceholder.Text = $"{icono}\n\n{titulo}\n\n{mensaje}";
                lblPlaceholder.Visible = false; // Oculto inicialmente

                // Esperar antes de mostrar
                Timer timerEspera = new Timer();
                timerEspera.Interval = 200;
                timerEspera.Tick += (s, args) =>
                {
                    timerEspera.Stop();
                    timerEspera.Dispose();

                    lblPlaceholder.Visible = true;
                    lblPlaceholder.BringToFront();

                    // Ocultar pantalla de carga
                    OcultarPantallaCarga();
                };
                timerEspera.Start();
            });
        }

        private void MostrarPantallaBienvenida()
        {
            // LIMPIAR formularios cargados en el panel
            LimpiarFormulariosAnteriores();

            // Mostrar controles de bienvenida
            picLogoBienvenida.Visible = true;
            lblTituloBienvenida.Visible = true;
            lblBienvenidaUsuario.Visible = true;
            panelResumen.Visible = true;
            panelAccesos.Visible = true;
            lblFechaHora.Visible = true;

            // Aplicar fade in
            AnimarFadeIn(picLogoBienvenida);
            AnimarFadeIn(lblTituloBienvenida);
            AnimarFadeIn(lblBienvenidaUsuario);
            AnimarFadeIn(panelResumen);
            AnimarFadeIn(panelAccesos);
            AnimarFadeIn(lblFechaHora);

            // Expandir el menú si estaba contraído
            if (!menuExpandido)
            {
                ExpandirMenu();
            }
        }

        private void CargarEnPanel(Form formulario)
        {
            if (formPrincipal == null) return;

            try
            {
                var metodo = formPrincipal.GetType().GetMethod("CargarContenidoPanel");
                if (metodo != null)
                {
                    metodo.Invoke(formPrincipal, new object[] { formulario });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar formulario:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ObtenerNombreUsuario()
        {
            try
            {
                var tipoSesion = Type.GetType("MOFIS_ERP.Classes.SesionActual");
                if (tipoSesion != null)
                {
                    var propNombre = tipoSesion.GetProperty("NombreCompleto");
                    if (propNombre != null)
                    {
                        string nombre = propNombre.GetValue(null)?.ToString();
                        if (!string.IsNullOrEmpty(nombre))
                            return nombre;
                    }
                }
            }
            catch { }

            return "Usuario";
        }

        // ═══════════════════════════════════════════════════════════════
        // ANIMACIÓN SUAVE CON EASING
        // ═══════════════════════════════════════════════════════════════
        // ═══════════════════════════════════════════════════════════════
        // ANIMACIÓN DEL MENÚ
        // ═══════════════════════════════════════════════════════════════
        private void AnimarMenuSuave(int anchoObjetivo, Action alFinalizar = null)
        {
            if (animacionEnProgreso) return;

            timerAnimacion?.Stop();
            timerAnimacion?.Dispose();

            animacionEnProgreso = true;

            int anchoInicial = panelMenu.Width;
            int diferencia = anchoObjetivo - anchoInicial;
            int totalPasos = VELOCIDAD_MENU_MS / INTERVALO_ANIMACION;
            int pasoActual = 0;

            timerAnimacion = new Timer();
            timerAnimacion.Interval = INTERVALO_ANIMACION;
            timerAnimacion.Tick += (s, e) =>
            {
                pasoActual++;

                // Easing: ease-out cubic (desacelera al final)
                double progreso = (double)pasoActual / totalPasos;
                double easing = 1 - Math.Pow(1 - progreso, 3);

                panelMenu.Width = anchoInicial + (int)(diferencia * easing);

                if (pasoActual >= totalPasos)
                {
                    panelMenu.Width = anchoObjetivo;
                    timerAnimacion.Stop();
                    timerAnimacion.Dispose();
                    timerAnimacion = null;
                    animacionEnProgreso = false;

                    alFinalizar?.Invoke();
                }
            };

            timerAnimacion.Start();
        }

        // ═══════════════════════════════════════════════════════════════
        // ANIMACIÓN DE BOTONES AL CONTRAER
        // ═══════════════════════════════════════════════════════════════
        private void AnimarBotonAContraido(Button btn, int indice)
        {
            // Este método ya no se usa directamente, pero lo mantenemos por si acaso
            int posicionCentrada = (MENU_ANCHO_CONTRAIDO - BOTON_ANCHO_CONTRAIDO) / 2;

            if (string.IsNullOrEmpty(btn.AccessibleDescription))
            {
                btn.AccessibleDescription = btn.Text;
            }

            string texto = btn.Text.Trim();
            int espacioIndex = texto.IndexOf("   ");
            if (espacioIndex > 0)
            {
                btn.Text = texto.Substring(0, espacioIndex).Trim();
            }

            btn.Width = BOTON_ANCHO_CONTRAIDO;
            btn.Left = posicionCentrada;
            btn.TextAlign = ContentAlignment.MiddleCenter;
            btn.Padding = new Padding(0);
            btn.Font = new Font("Segoe UI", FUENTE_BOTON_CONTRAIDO, FontStyle.Regular);

            if (btn == botonSeleccionado)
            {
                btn.BackColor = colorBotonSeleccionado;
                btn.ForeColor = colorTextoHover;
            }
            else
            {
                btn.BackColor = colorBotonNormal;
                btn.ForeColor = colorIconoContraido;
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // ANIMACIÓN DE BOTONES AL EXPANDIR
        // ═══════════════════════════════════════════════════════════════
        private void AnimarBotonAExpandido(Button btn, int indice)
        {
            int delay = Math.Max(1, indice * DELAY_CASCADA_MS);

            Timer timerDelay = new Timer();
            timerDelay.Interval = delay;
            timerDelay.Tick += (s, e) =>
            {
                timerDelay.Stop();
                timerDelay.Dispose();

                // Restaurar texto original
                if (!string.IsNullOrEmpty(btn.AccessibleDescription))
                {
                    btn.Text = btn.AccessibleDescription;
                }

                // Calcular posición centrada
                int posicionCentrada = (MENU_ANCHO_EXPANDIDO - BOTON_ANCHO_EXPANDIDO) / 2;

                // Aplicar propiedades
                btn.Width = BOTON_ANCHO_EXPANDIDO;
                btn.Left = posicionCentrada;
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Padding = new Padding(15, 0, 0, 0);
                btn.Font = new Font("Segoe UI", FUENTE_BOTON_EXPANDIDO, FontStyle.Regular);

                // Color y estilo según si está seleccionado o no
                if (btn == botonSeleccionado)
                {
                    btn.BackColor = colorBotonSeleccionado;
                    btn.ForeColor = colorTextoHover;
                    btn.Font = new Font("Segoe UI", FUENTE_BOTON_EXPANDIDO, FontStyle.Bold);
                }
                else
                {
                    btn.BackColor = colorBotonNormal;
                    btn.ForeColor = colorTextoNormal;
                }
            };
            timerDelay.Start();
        }

        // ═══════════════════════════════════════════════════════════════
        // FADE OUT PARA PANTALLA DE BIENVENIDA
        // ═══════════════════════════════════════════════════════════════
        private void AnimarFadeOutBienvenida(Action alFinalizar = null)
        {
            Control[] controles = { picLogoBienvenida, lblTituloBienvenida, lblBienvenidaUsuario,
                            panelResumen, panelAccesos, lblFechaHora };

            int totalPasos = VELOCIDAD_FADE_MS / INTERVALO_ANIMACION;
            int pasoActual = 0;

            Timer timer = new Timer();
            timer.Interval = INTERVALO_ANIMACION;
            timer.Tick += (s, e) =>
            {
                pasoActual++;

                if (pasoActual >= totalPasos)
                {
                    timer.Stop();
                    timer.Dispose();

                    foreach (Control ctrl in controles)
                    {
                        ctrl.Visible = false;
                    }

                    alFinalizar?.Invoke();
                }
            };
            timer.Start();
        }

        // ═══════════════════════════════════════════════════════════════
        // RELOJ EN TIEMPO REAL
        // ═══════════════════════════════════════════════════════════════
        private void IniciarReloj()
        {
            timerReloj = new Timer();
            timerReloj.Interval = 1000;
            timerReloj.Tick += TimerReloj_Tick;
            timerReloj.Start();
        }

        private void TimerReloj_Tick(object sender, EventArgs e)
        {
            if (lblFechaHora != null && !lblFechaHora.IsDisposed)
            {
                lblFechaHora.Text = DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy  |  hh:mm:ss tt");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // CARGAR DATOS DEL RESUMEN (DESDE BD)
        // ═══════════════════════════════════════════════════════════════
        private void CargarDatosResumen()
        {
            try
            {
                // TODO: Implementar consultas reales a la base de datos
                // Por ahora, valores de ejemplo

                lblSolicitudesNumero.Text = "0";
                lblCertificadosNumero.Text = "0";
                lblCartasNumero.Text = "0";
                lblRegistrosHoyNumero.Text = "0";
                lblActividadPendiente.Text = "Ninguna";
                lblActividadPendiente.ForeColor = Color.FromArgb(16, 124, 16);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar resumen: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // MANEJAR REDIMENSIONAMIENTO
        // ═══════════════════════════════════════════════════════════════
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Reconfigurar área de trabajo al redimensionar
            ConfigurarAreaTrabajo();

            // Recentrar controles de carga
            if (panelCargando != null && panelCargando.Visible)
            {
                CentrarControlesCarga();
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // LIMPIEZA
        // ═══════════════════════════════════════════════════════════════
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            timerReloj?.Stop();
            timerReloj?.Dispose();
            timerAnimacion?.Stop();
            timerAnimacion?.Dispose();
            timerCarga?.Stop(); // NUEVO
            timerCarga?.Dispose(); // NUEVO
            base.OnFormClosing(e);
        }
    }
}