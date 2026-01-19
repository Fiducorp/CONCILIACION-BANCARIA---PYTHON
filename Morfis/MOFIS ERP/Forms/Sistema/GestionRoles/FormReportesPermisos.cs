using ClosedXML.Excel;
using MOFIS_ERP.Classes;
using QuestPDF.Fluent;      // ← Para el API de generación de PDF
using QuestPDF.Helpers;     // ← Para constantes y helpers
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MOFIS_ERP.Forms.Sistema.GestionRoles
{
    public partial class FormReportesPermisos : Form
    {
        // ===== CAMPOS PRIVADOS =====
        private FormMain formPrincipal;
        private DataTable dtAuditoria;

        // ===== CONSTANTES DE ESTILO (como CSS) =====
        // Colores
        private static readonly Color COLOR_PRIMARIO = Color.FromArgb(0, 120, 212);      // Azul
        private static readonly Color COLOR_EXITO = Color.FromArgb(34, 139, 34);         // Verde
        private static readonly Color COLOR_PELIGRO = Color.FromArgb(220, 53, 69);       // Rojo
        private static readonly Color COLOR_SECUNDARIO = Color.FromArgb(108, 117, 125);  // Gris
        private static readonly Color COLOR_FONDO_CLARO = Color.FromArgb(235, 235, 235); // Gris claro
        private static readonly Color COLOR_FONDO = Color.FromArgb(240, 240, 240);       // Gris medio

        // Fuentes
        private static readonly Font FUENTE_TITULO = new Font("Segoe UI", 24, FontStyle.Bold);
        private static readonly Font FUENTE_TAB = new Font("Segoe UI", 14, FontStyle.Bold);
        private static readonly Font FUENTE_LABEL = new Font("Segoe UI", 14, FontStyle.Bold);
        private static readonly Font FUENTE_CONTROL = new Font("Segoe UI", 14, FontStyle.Regular);
        private static readonly Font FUENTE_BOTON = new Font("Segoe UI", 12, FontStyle.Bold);
        private static readonly Font FUENTE_GRID_HEADER = new Font("Segoe UI", 12, FontStyle.Bold);
        private static readonly Font FUENTE_GRID_CELL = new Font("Segoe UI", 11, FontStyle.Regular);


        // Controles del TabControl
        // private TabPage tabAuditoria;
        private TabControl tabControl;
        private TabPage tabHistorial;
        private TabPage tabEstadisticas;

        // ===== CONSTRUCTOR =====
        public FormReportesPermisos(FormMain formMain)
        {
            InitializeComponent();

            // Configurar licencia Community de QuestPDF
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            formPrincipal = formMain;

            ConfigurarFormularioBase();
            VerificarPermisos();
            CrearTabs();
        }

        // ===== CONFIGURACIÓN INICIAL =====
        private void ConfigurarFormularioBase()
        {
            // Configuración visual del formulario
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            // Limpiar controles del Designer
            this.Controls.Clear();
        }

        private void VerificarPermisos()
        {
            // Solo ROOT, ADMIN y GERENTE pueden acceder
            if (!SesionActual.EsRoot() &&
                !SesionActual.EsAdmin() &&
                SesionActual.NombreRol.ToUpper() != "GERENTE")
            {
                MessageBox.Show(
                    "No tiene permisos para acceder a este módulo.",
                    "Acceso Denegado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                VolverAlDashboard();
            }
        }

        // ===== CREACIÓN DE LA INTERFAZ =====
        private void CrearTabs()
        {
            // Panel contenedor principal
            Panel panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 240, 240),
                Padding = new Padding(20)
            };
            this.Controls.Add(panelPrincipal);

            // Botón Volver (esquina superior izquierda)
            Button btnVolver = new Button
            {
                Text = "← Volver",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Size = new Size(120, 40),
                Location = new Point(20, 20),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(0, 120, 212),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnVolver.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 212);
            btnVolver.FlatAppearance.BorderSize = 2;
            btnVolver.Click += (s, e) => VolverAlDashboard();
            panelPrincipal.Controls.Add(btnVolver);

            // Título principal
            Label lblTitulo = new Label
            {
                Text = "REPORTES Y AUDITORÍA DE PERMISOS",
                Font = FUENTE_TITULO,  // Fuente grande para el título
                ForeColor = COLOR_PRIMARIO,  // Color primario
                AutoSize = true,
                Location = new Point(20, 70)
            };
            panelPrincipal.Controls.Add(lblTitulo);

            // TabControl (las 4 pestañas)
            tabControl = new TabControl
            {
                Location = new Point(20, 125),
                Size = new Size(panelPrincipal.Width - 40, panelPrincipal.Height - 155),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = FUENTE_TAB  // Fuente para las pestañas
            };
            panelPrincipal.Controls.Add(tabControl);

            // Crear las 2 pestañas vacías
            CrearTabHistorial();
            CrearTabEstadisticas();

            // Evento cuando cambia de pestaña
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabEstadisticas)
            {
                CargarEstadisticas();
            }
        }

        // ===== TAB 1: HISTORIAL DE CAMBIOS =====
        // ===== TAB 1: HISTORIAL DE CAMBIOS =====
        // ===== TAB 1: HISTORIAL DE CAMBIOS =====
        private void CrearTabHistorial()
        {
            tabHistorial = new TabPage("📋 Historial de Cambios")
            {
                BackColor = Color.White,
                Padding = new Padding(0)  // Sin padding para control total
            };
            tabControl.TabPages.Add(tabHistorial);

            // ========== PANEL SUPERIOR: FILTROS Y BOTONES ==========
            Panel panelSuperior = new Panel
            {
                Dock = DockStyle.Top,
                Height = 175,
                BackColor = COLOR_FONDO_CLARO,
                Padding = new Padding(20, 15, 20, 10)
            };
            tabHistorial.Controls.Add(panelSuperior);

            // === FILA 1: FILTROS DE FECHA ===
            // === DISTRIBUCIÓN: 2 COLUMNAS VERTICALES ===
            int y = 15;             // Posición Y inicial
            int col1X = 20;         // Columna izquierda
            int col2X = 470;        // Columna derecha
            int labelWidth = 100;   // Ancho de las etiquetas
            int controlWidth = 300; // Ancho de los controles

            // ========== COLUMNA 1 ==========

            // Fecha Desde (arriba)
            Label lblDesde = new Label
            {
                Text = "Desde:",
                Location = new Point(col1X, y + 3),
                Size = new Size(labelWidth, 25),
                Font = FUENTE_LABEL,
                TextAlign = ContentAlignment.MiddleLeft
            };
            panelSuperior.Controls.Add(lblDesde);

            DateTimePicker dtpDesde = new DateTimePicker
            {
                Name = "dtpDesde",
                Location = new Point(col1X + labelWidth + 5, y),
                Size = new Size(controlWidth, 30),
                Format = DateTimePickerFormat.Short,
                Font = FUENTE_CONTROL,
                Value = DateTime.Now.AddMonths(-1)
            };
            panelSuperior.Controls.Add(dtpDesde);

            y += 45;  // Bajar a la siguiente fila

            // Hasta (abajo)
            Label lblHasta = new Label
            {
                Text = "Hasta:",
                Location = new Point(col1X, y + 3),
                Size = new Size(labelWidth, 25),
                Font = FUENTE_LABEL,
                TextAlign = ContentAlignment.MiddleLeft
            };
            panelSuperior.Controls.Add(lblHasta);

            DateTimePicker dtpHasta = new DateTimePicker
            {
                Name = "dtpHasta",
                Location = new Point(col1X + labelWidth + 5, y),
                Size = new Size(controlWidth, 30),
                Format = DateTimePickerFormat.Short,
                Font = FUENTE_CONTROL,
                Value = DateTime.Now
            };
            panelSuperior.Controls.Add(dtpHasta);

            // ========== COLUMNA 2 ==========
            y = 15;  // Resetear Y para la columna 2

            // Usuario (arriba)
            Label lblUsuario = new Label
            {
                Text = "Usuario:",
                Location = new Point(col2X, y + 3),
                Size = new Size(labelWidth, 25),
                Font = FUENTE_LABEL,
                TextAlign = ContentAlignment.MiddleLeft
            };
            panelSuperior.Controls.Add(lblUsuario);

            ComboBox cmbUsuario = new ComboBox
            {
                Name = "cmbUsuario",
                Location = new Point(col2X + labelWidth + 5, y),
                Size = new Size(controlWidth, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = FUENTE_CONTROL
            };
            panelSuperior.Controls.Add(cmbUsuario);

            y += 45;  // Bajar a la siguiente fila

            // Acción (abajo)
            Label lblAccion = new Label
            {
                Text = "Acción:",
                Location = new Point(col2X, y + 3),
                Size = new Size(labelWidth, 25),
                Font = FUENTE_LABEL,
                TextAlign = ContentAlignment.MiddleLeft
            };
            panelSuperior.Controls.Add(lblAccion);

            ComboBox cmbAccion = new ComboBox
            {
                Name = "cmbAccion",
                Location = new Point(col2X + labelWidth + 5, y),
                Size = new Size(controlWidth, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = FUENTE_CONTROL
            };
            panelSuperior.Controls.Add(cmbAccion);

            // === FILA 3: BOTONES ALINEADOS HORIZONTALMENTE ===
            y = 120;  // Posición fija para los botones

            int anchoBoton = 140;
            int espaciado = 20;
            int xBoton = 20;

            Button btnBuscar = CrearBoton(
                "🔍 Buscar",
                new Point(xBoton, y),
                new Size(anchoBoton, 40),
                COLOR_PRIMARIO,
                (s, e) => CargarHistorial()
            );
            panelSuperior.Controls.Add(btnBuscar);
            xBoton += anchoBoton + espaciado;

            Button btnLimpiar = CrearBoton(
                "🗑️ Limpiar",
                new Point(xBoton, y),
                new Size(anchoBoton, 40),
                COLOR_SECUNDARIO,
                (s, e) => LimpiarFiltrosHistorial()
            );
            panelSuperior.Controls.Add(btnLimpiar);
            xBoton += anchoBoton + espaciado;

            Button btnExportarExcel = CrearBoton(
                "📊 Excel",
                new Point(xBoton, y),
                new Size(anchoBoton, 40),
                COLOR_EXITO,
                (s, e) => ExportarHistorialExcel()
            );
            panelSuperior.Controls.Add(btnExportarExcel);
            xBoton += anchoBoton + espaciado;

            Button btnExportarPDF = CrearBoton(
                "📄 PDF",
                new Point(xBoton, y),
                new Size(anchoBoton, 40),
                COLOR_PELIGRO,
                (s, e) => ExportarAuditoriaPDF()
            );
            panelSuperior.Controls.Add(btnExportarPDF);

            // Label de total (alineado a la derecha)
            Label lblTotal = new Label
            {
                Name = "lblTotal",
                Text = "Total de registros: 0",
                Location = new Point(panelSuperior.Width - 250, y + 8),
                Size = new Size(230, 25),
                Font = FUENTE_LABEL,
                ForeColor = COLOR_PRIMARIO,
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            panelSuperior.Controls.Add(lblTotal);

            // ========== PANEL INFERIOR: DATAGRIDVIEW ==========
            Panel panelInferior = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(0, 170, 0, 0)  // Dejar espacio para el panel superior
            };
            tabHistorial.Controls.Add(panelInferior);

            DataGridView dgvHistorial = new DataGridView
            {
                Name = "dgvHistorial",
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.White,
                GridColor = Color.FromArgb(230, 230, 230),
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToResizeRows = false,
                EnableHeadersVisualStyles = false
            };

            // Estilo de encabezados
            dgvHistorial.ColumnHeadersDefaultCellStyle.BackColor = COLOR_PRIMARIO;
            dgvHistorial.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvHistorial.ColumnHeadersDefaultCellStyle.Font = FUENTE_GRID_HEADER;
            dgvHistorial.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvHistorial.ColumnHeadersHeight = 45;

            // Estilo de filas
            dgvHistorial.RowTemplate.Height = 50;
            dgvHistorial.DefaultCellStyle.Font = FUENTE_GRID_CELL;
            dgvHistorial.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

            // COLUMNAS
            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AuditoriaID",
                HeaderText = "ID",
                Visible = false
            });

            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FechaHora",
                HeaderText = "Fecha y Hora",
                FillWeight = 12,  // ← REDUCIDO (de 15 a 12)
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,  // ← CENTRALIZADO
                    Format = "dd/MM/yyyy HH:mm:ss"
                }
            });

            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Usuario",
                HeaderText = "Usuario",
                FillWeight = 12
            });

            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Accion",
                HeaderText = "Acción Realizada",
                FillWeight = 18
            });

            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Modulo",
                HeaderText = "Módulo",
                FillWeight = 12
            });

            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Detalle",
                HeaderText = "Detalle del Cambio",
                FillWeight = 33  // ← AUMENTADO (de 30 a 33, ganó los 3 de FechaHora)
            });

            dgvHistorial.DoubleClick += (s, e) => MostrarDetalleCompleto();
            panelInferior.Controls.Add(dgvHistorial);

            // CARGAR DATOS INICIALES
            CargarFiltrosHistorial();
            CargarHistorial();
        }

        // Método auxiliar para crear botones con estilo uniforme
        private Button CrearBoton(string texto, Point ubicacion, Size tamano, Color colorFondo, EventHandler onClick)
        {
            Button btn = new Button
            {
                Text = texto,
                Location = ubicacion,
                Size = tamano,
                BackColor = colorFondo,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = FUENTE_BOTON
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += onClick;

            // Efecto hover
            Color colorHover = ControlPaint.Dark(colorFondo, 0.1f);
            btn.MouseEnter += (s, e) => btn.BackColor = colorHover;
            btn.MouseLeave += (s, e) => btn.BackColor = colorFondo;

            return btn;
        }

        // ===== TAB 2: ESTADÍSTICAS Y ANÁLISIS =====
        private void CrearTabEstadisticas()
        {
            tabEstadisticas = new TabPage("📊 Estadísticas y Análisis")
            {
                BackColor = Color.White,
                Padding = new Padding(0)
            };
            tabControl.TabPages.Add(tabEstadisticas);

            // Panel contenedor principal con scroll
            Panel panelContenedor = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White
            };
            tabEstadisticas.Controls.Add(panelContenedor);

            // ===== PANEL DE FILTROS (SUPERIOR) =====
            Panel panelFiltros = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(panelContenedor.Width - 30, 80),
                BackColor = COLOR_FONDO_CLARO,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            panelContenedor.Controls.Add(panelFiltros);

            // Centrado vertical: (altura panel - altura control) / 2
            int centroVertical = (panelFiltros.Height - 30) / 2;

            Label lblPeriodo = new Label
            {
                Text = "Período de Análisis:",
                Location = new Point(15, centroVertical + 3),
                Size = new Size(200, 25),
                Font = FUENTE_LABEL,
                TextAlign = ContentAlignment.MiddleLeft
            };
            panelFiltros.Controls.Add(lblPeriodo);

            DateTimePicker dtpEstadDesde = new DateTimePicker
            {
                Name = "dtpEstadDesde",
                Location = new Point(220, centroVertical),
                Size = new Size(150, 30),
                Format = DateTimePickerFormat.Short,
                Font = FUENTE_CONTROL,
                Value = DateTime.Now.AddMonths(-3)
            };
            panelFiltros.Controls.Add(dtpEstadDesde);

            Label lblA = new Label
            {
                Text = "a",
                Location = new Point(380, centroVertical + 3),
                Size = new Size(20, 25),
                Font = FUENTE_CONTROL,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelFiltros.Controls.Add(lblA);

            DateTimePicker dtpEstadHasta = new DateTimePicker
            {
                Name = "dtpEstadHasta",
                Location = new Point(405, centroVertical),
                Size = new Size(150, 30),
                Format = DateTimePickerFormat.Short,
                Font = FUENTE_CONTROL,
                Value = DateTime.Now
            };
            panelFiltros.Controls.Add(dtpEstadHasta);

            Button btnActualizar = new Button
            {
                Text = "🔄 Actualizar Estadísticas",
                Location = new Point(570, centroVertical - 5),
                Size = new Size(230, 40),
                BackColor = COLOR_PRIMARIO,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = FUENTE_BOTON
            };
            btnActualizar.FlatAppearance.BorderSize = 0;
            btnActualizar.Click += (s, e) => CargarEstadisticas();
            panelFiltros.Controls.Add(btnActualizar);

            // ===== PANEL DE KPIs (4 CARDS) =====
            Panel panelKPIs = new Panel
            {
                Name = "panelKPIs",
                Location = new Point(10, 100),
                Size = new Size(panelContenedor.Width - 30, 120),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            panelContenedor.Controls.Add(panelKPIs);

            // ===== GRÁFICOS - DISTRIBUCIÓN OPTIMIZADA CON TODO EL ANCHO =====
            int margenLateral = 5;
            int espacioEntreGraficos = 15;

            // COLUMNA 1: Gráficos apilados (Cambios por Día + Top 10 Usuarios) - MÁS ANCHA
            int anchoColumna1 = 600;
            int altoGrafico1 = 280;   // Cambios por Día
            int altoGrafico2 = 285;   // Top 5 Usuarios

            // COLUMNAS 2 y 3: Lado a lado (Distribución + Módulos) - MÁS ANCHAS
            int anchoColumna2y3 = 610;  // Más anchos para ocupar todo el espacio
            int altoGrafico3y4 = 580;   // Altura total

            int yInicial = 230;
            int col1X = margenLateral;
            int col2X = margenLateral + anchoColumna1 + espacioEntreGraficos;
            int col3X = col2X + anchoColumna2y3 + espacioEntreGraficos;

            // ===== COLUMNA 1 (IZQUIERDA - 2 GRÁFICOS APILADOS) =====

            // Gráfico 1: Cambios por Día (Líneas)
            Chart chartCambiosPorDia = new Chart
            {
                Name = "chartCambiosPorDia",
                Location = new Point(col1X, yInicial),
                Size = new Size(anchoColumna1, altoGrafico1),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            ConfigurarChartBasico(chartCambiosPorDia, "Cambios por Día");
            panelContenedor.Controls.Add(chartCambiosPorDia);

            // Gráfico 2: Top 10 Usuarios (Barras)
            Chart chartUsuarios = new Chart
            {
                Name = "chartUsuarios",
                Location = new Point(col1X, yInicial + altoGrafico1 + espacioEntreGraficos),
                Size = new Size(anchoColumna1, altoGrafico2),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            ConfigurarChartBasico(chartUsuarios, "Top 5 Usuarios Más Activos");
            panelContenedor.Controls.Add(chartUsuarios);

            // ===== COLUMNA 2 (CENTRO) =====

            // Gráfico 3: Distribución por Acción (Torta) - Gráfico más pequeño, leyenda más grande
            Chart chartAcciones = new Chart
            {
                Name = "chartAcciones",
                Location = new Point(col2X, yInicial),
                Size = new Size(anchoColumna2y3, altoGrafico3y4),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            ConfigurarChartBasico(chartAcciones, "Distribución por Tipo de Acción");
            panelContenedor.Controls.Add(chartAcciones);

            // ===== COLUMNA 3 (DERECHA) =====

            // Gráfico 4: Cambios por Módulo (Columnas)
            Chart chartModulos = new Chart
            {
                Name = "chartModulos",
                Location = new Point(col3X, yInicial),
                Size = new Size(anchoColumna2y3, altoGrafico3y4),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            ConfigurarChartBasico(chartModulos, "Cambios por Módulo");
            panelContenedor.Controls.Add(chartModulos);

            // Cargar datos al entrar al tab
            CargarEstadisticas();
        }

        private void ConfigurarChartBasico(Chart chart, string titulo)
        {
            chart.BackColor = Color.White;
            chart.BorderlineColor = Color.FromArgb(200, 200, 200);
            chart.BorderlineWidth = 1;
            chart.BorderlineDashStyle = ChartDashStyle.Solid;

            // Crear área del gráfico
            ChartArea area = new ChartArea("MainArea");
            area.BackColor = Color.White;
            area.BorderColor = Color.FromArgb(220, 220, 220);
            area.BorderWidth = 1;
            area.BorderDashStyle = ChartDashStyle.Solid;

            // Estilo de ejes
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 9);
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 9);

            chart.ChartAreas.Add(area);

            // Título del gráfico
            Title title = new Title
            {
                Text = titulo,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = COLOR_PRIMARIO,
                Alignment = ContentAlignment.TopCenter,
                Docking = Docking.Top
            };
            chart.Titles.Add(title);
        }

        private void CargarEstadisticas()
        {
            try
            {
                var dtpDesde = EncontrarControl<DateTimePicker>(tabEstadisticas, "dtpEstadDesde");
                var dtpHasta = EncontrarControl<DateTimePicker>(tabEstadisticas, "dtpEstadHasta");

                DateTime fechaDesde = dtpDesde?.Value.Date ?? DateTime.Now.AddMonths(-3);
                DateTime fechaHasta = dtpHasta?.Value.Date.AddDays(1) ?? DateTime.Now.AddDays(1);

                this.Cursor = Cursors.WaitCursor;

                // Cargar componentes
                CargarKPIs(fechaDesde, fechaHasta);
                CargarGraficoCambiosPorDia(fechaDesde, fechaHasta);
                CargarGraficoAcciones(fechaDesde, fechaHasta);
                CargarGraficoUsuarios(fechaDesde, fechaHasta);
                CargarGraficoModulos(fechaDesde, fechaHasta);

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al cargar estadísticas:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarKPIs(DateTime fechaDesde, DateTime fechaHasta)
        {
            var panelKPIs = EncontrarControl<Panel>(tabEstadisticas, "panelKPIs");
            if (panelKPIs == null) return;

            panelKPIs.Controls.Clear();

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                // MÉTRICA 1: Total de cambios
                string sqlTotal = @"
            SELECT COUNT(*) 
            FROM Auditoria 
            WHERE FechaHora >= @Desde AND FechaHora < @Hasta
            AND Categoria = 'SISTEMA' AND Modulo = 'Gestión de Roles'";

                int totalCambios = 0;
                using (var cmd = new SqlCommand(sqlTotal, conn))
                {
                    cmd.Parameters.AddWithValue("@Desde", fechaDesde);
                    cmd.Parameters.AddWithValue("@Hasta", fechaHasta);
                    totalCambios = (int)cmd.ExecuteScalar();
                }

                // MÉTRICA 2: Usuarios activos
                string sqlUsuarios = @"
            SELECT COUNT(DISTINCT UsuarioID) 
            FROM Auditoria 
            WHERE FechaHora >= @Desde AND FechaHora < @Hasta
            AND Categoria = 'SISTEMA' AND Modulo = 'Gestión de Roles'";

                int usuariosActivos = 0;
                using (var cmd = new SqlCommand(sqlUsuarios, conn))
                {
                    cmd.Parameters.AddWithValue("@Desde", fechaDesde);
                    cmd.Parameters.AddWithValue("@Hasta", fechaHasta);
                    usuariosActivos = (int)cmd.ExecuteScalar();
                }

                // MÉTRICA 3: Promedio por día
                int dias = Math.Max(1, (fechaHasta - fechaDesde).Days);
                decimal promedioDia = (decimal)totalCambios / dias;

                // MÉTRICA 4: Última modificación
                string sqlUltimo = @"
            SELECT TOP 1 FechaHora 
            FROM Auditoria 
            WHERE FechaHora >= @Desde AND FechaHora < @Hasta
            AND Categoria = 'SISTEMA' AND Modulo = 'Gestión de Roles'
            ORDER BY FechaHora DESC";

                DateTime? ultimaModif = null;
                using (var cmd = new SqlCommand(sqlUltimo, conn))
                {
                    cmd.Parameters.AddWithValue("@Desde", fechaDesde);
                    cmd.Parameters.AddWithValue("@Hasta", fechaHasta);
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        ultimaModif = (DateTime)result;
                }

                // CREAR LAS 4 CARDS
                int cardWidth = (panelKPIs.Width - 50) / 4;
                int xPos = 10;

                CrearCardKPI(panelKPIs, "Total de Cambios", totalCambios.ToString("N0"),
                    COLOR_PRIMARIO, new Point(xPos, 10), cardWidth);
                xPos += cardWidth + 10;

                CrearCardKPI(panelKPIs, "Usuarios Activos", usuariosActivos.ToString("N0"),
                    COLOR_EXITO, new Point(xPos, 10), cardWidth);
                xPos += cardWidth + 10;

                CrearCardKPI(panelKPIs, "Promedio por Día", promedioDia.ToString("N1"),
                    Color.FromArgb(255, 152, 0), new Point(xPos, 10), cardWidth);
                xPos += cardWidth + 10;

                // ← CAMBIO: Formato completo dd/MM/yyyy HH:mm:ss
                CrearCardKPI(panelKPIs, "Última Modificación",
                    ultimaModif?.ToString("dd/MM/yyyy HH:mm:ss") ?? "N/A",
                    Color.FromArgb(156, 39, 176), new Point(xPos, 10), cardWidth);
            }
        }

        private void CrearCardKPI(Panel panel, string titulo, string valor, Color color, Point ubicacion, int ancho)
        {
            Panel card = new Panel
            {
                Location = ubicacion,
                Size = new Size(ancho, 100),
                BackColor = color,
                BorderStyle = BorderStyle.None
            };

            // Título (más grande)
            Label lblTitulo = new Label
            {
                Text = titulo,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold), // ← Fuente más grande para el título
                Location = new Point(10, 12),
                AutoSize = true
            };
            card.Controls.Add(lblTitulo);

            // Valor grande
            Label lblValor = new Label
            {
                Text = valor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 26, FontStyle.Bold),  // ← Ligeramente reducido para dar espacio al título
                Location = new Point(10, 38),
                Size = new Size(ancho - 20, 50),
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(lblValor);

            panel.Controls.Add(card);
        }

        private void CargarGraficoCambiosPorDia(DateTime fechaDesde, DateTime fechaHasta)
        {
            var chart = EncontrarControl<Chart>(tabEstadisticas, "chartCambiosPorDia");
            if (chart == null) return;

            chart.Series.Clear();

            Series series = new Series("Cambios")
            {
                ChartType = SeriesChartType.Line,
                Color = COLOR_PRIMARIO,
                BorderWidth = 3,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 8,
                MarkerColor = COLOR_PRIMARIO,
                IsValueShownAsLabel = true,  // ← MOSTRAR VALORES EN CADA PUNTO
                Font = new Font("Segoe UI", 9, FontStyle.Bold)  // ← Fuente para los valores
            };

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string sql = @"
            SELECT CAST(FechaHora AS DATE) AS Fecha, COUNT(*) AS Total
            FROM Auditoria
            WHERE FechaHora >= @Desde AND FechaHora < @Hasta
            AND Categoria = 'SISTEMA' AND Modulo = 'Gestión de Roles'
            GROUP BY CAST(FechaHora AS DATE)
            ORDER BY Fecha";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Desde", fechaDesde);
                    cmd.Parameters.AddWithValue("@Hasta", fechaHasta);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime fecha = reader.GetDateTime(0);
                            int total = reader.GetInt32(1);
                            series.Points.AddXY(fecha.ToString("dd/MM"), total);
                        }
                    }
                }
            }

            chart.Series.Add(series);

            // Configurar ejes
            if (chart.ChartAreas.Count > 0)
            {
                chart.ChartAreas[0].AxisX.Interval = 1;
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
                chart.ChartAreas[0].AxisY.Minimum = 0;

                // ← AGREGAR ESTA LÍNEA para controlar el área interna
                chart.ChartAreas[0].InnerPlotPosition = new ElementPosition(7, 0, 90, 85);
                //                                                          X   Y   Ancho Alto
            }
        }

        private void CargarGraficoAcciones(DateTime fechaDesde, DateTime fechaHasta)
        {
            var chart = EncontrarControl<Chart>(tabEstadisticas, "chartAcciones");
            if (chart == null) return;

            chart.Series.Clear();
            chart.Legends.Clear();

            Series series = new Series("Acciones")
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),  // ← TAMAÑO MÁS GRANDE (14pt)
                LabelForeColor = Color.White  // ← COLOR BLANCO
            };

            // Colores personalizados
            Color[] colores = new Color[]
            {
                COLOR_PRIMARIO,
                COLOR_EXITO,
                Color.FromArgb(255, 152, 0),
                Color.FromArgb(156, 39, 176),
                Color.FromArgb(220, 53, 69),
                Color.FromArgb(108, 117, 125),
                Color.FromArgb(23, 162, 184),
                Color.FromArgb(253, 126, 20)
            };

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string sql = @"
            SELECT TOP 8 Accion, COUNT(*) AS Total
            FROM Auditoria
            WHERE FechaHora >= @Desde AND FechaHora < @Hasta
            AND Categoria = 'SISTEMA' AND Modulo = 'Gestión de Roles'
            GROUP BY Accion
            ORDER BY Total DESC";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Desde", fechaDesde);
                    cmd.Parameters.AddWithValue("@Hasta", fechaHasta);

                    using (var reader = cmd.ExecuteReader())
                    {
                        int colorIndex = 0;
                        while (reader.Read())
                        {
                            string accion = reader.GetString(0);
                            int total = reader.GetInt32(1);

                            int pointIndex = series.Points.AddXY(accion, total);
                            series.Points[pointIndex].Color = colores[colorIndex % colores.Length];
                            series.Points[pointIndex].LabelForeColor = Color.White;  // Blanco

                            colorIndex++;
                        }
                    }
                }
            }

            chart.Series.Add(series);

            // Configurar ChartArea - MÁS ESPACIO PARA EL TÍTULO
            if (chart.ChartAreas.Count > 0)
            {
                chart.ChartAreas[0].Position = new ElementPosition(5, 10, 90, 60);  // ← Empieza en 10% (más espacio arriba)
            }

            // Leyenda con MENOS ESPACIO VERTICAL entre items
            Legend legend = new Legend("Legend")
            {
                Docking = Docking.Bottom,
                Alignment = StringAlignment.Center,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                IsTextAutoFit = false,
                ItemColumnSpacing = 15,
                LegendStyle = LegendStyle.Table,
                TableStyle = LegendTableStyle.Wide
            };
            legend.Position = new ElementPosition(5, 72, 100, 27);  // ← 72% desde arriba, 26% de altura (más compacto)
            chart.Legends.Add(legend);
        }

        private void CargarGraficoUsuarios(DateTime fechaDesde, DateTime fechaHasta)
        {
            var chart = EncontrarControl<Chart>(tabEstadisticas, "chartUsuarios");
            if (chart == null) return;

            chart.Series.Clear();
            chart.Legends.Clear();

            // Gráfico de columnas verticales
            Series series = new Series("Cambios")
            {
                ChartType = SeriesChartType.Column,  // Columnas verticales
                Color = COLOR_EXITO,
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                LabelForeColor = Color.Black
            };

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string sql = @"
            SELECT TOP 5 U.Username, COUNT(*) AS Total
            FROM Auditoria A
            INNER JOIN Usuarios U ON A.UsuarioID = U.UsuarioID
            WHERE A.FechaHora >= @Desde AND A.FechaHora < @Hasta
            AND A.Categoria = 'SISTEMA' AND A.Modulo = 'Gestión de Roles'
            GROUP BY U.Username
            ORDER BY Total DESC";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Desde", fechaDesde);
                    cmd.Parameters.AddWithValue("@Hasta", fechaHasta);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string usuario = reader.GetString(0);
                            int total = reader.GetInt32(1);
                            series.Points.AddXY(usuario, total);
                        }
                    }
                }
            }

            chart.Series.Add(series);

            // Configurar ejes - Columnas verticales limpias
            if (chart.ChartAreas.Count > 0)
            {
                // Eje X: Nombres de usuario
                chart.ChartAreas[0].AxisX.Interval = 1;
                chart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = 0;  // Horizontal
                chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;

                // Eje Y: Valores
                chart.ChartAreas[0].AxisY.Minimum = 0;
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(230, 230, 230);
            }
        }

        private void CargarGraficoModulos(DateTime fechaDesde, DateTime fechaHasta)
        {
            var chart = EncontrarControl<Chart>(tabEstadisticas, "chartModulos");
            if (chart == null) return;

            chart.Series.Clear();

            Series series = new Series("Cambios")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.FromArgb(255, 152, 0),
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string sql = @"
            SELECT COALESCE(Formulario, 'Sin Especificar') AS Formulario, COUNT(*) AS Total
            FROM Auditoria
            WHERE FechaHora >= @Desde AND FechaHora < @Hasta
            AND Categoria = 'SISTEMA' AND Modulo = 'Gestión de Roles'
            GROUP BY COALESCE(Formulario, 'Sin Especificar')
            ORDER BY Total DESC";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Desde", fechaDesde);
                    cmd.Parameters.AddWithValue("@Hasta", fechaHasta);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string modulo = reader.GetString(0);
                            int total = reader.GetInt32(1);
                            series.Points.AddXY(modulo, total);
                        }
                    }
                }
            }

            chart.Series.Add(series);

            // Configurar ejes con etiquetas HORIZONTALES
            if (chart.ChartAreas.Count > 0)
            {
                chart.ChartAreas[0].Position = new ElementPosition(5, 10, 90, 85);

                // Etiquetas HORIZONTALES (ángulo 0)
                chart.ChartAreas[0].AxisX.Interval = 1;
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = 0;  // ← HORIZONTAL (antes era -45)
                chart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Segoe UI", 8, FontStyle.Regular);  // Más pequeño
                chart.ChartAreas[0].AxisX.IsLabelAutoFit = true;
                chart.ChartAreas[0].AxisX.LabelAutoFitMaxFontSize = 8;
                chart.ChartAreas[0].AxisX.LabelAutoFitMinFontSize = 7;

                // Menos espacio interno para dar más espacio a las etiquetas
                chart.ChartAreas[0].InnerPlotPosition = new ElementPosition(10, 0, 88, 80);

                chart.ChartAreas[0].AxisY.Minimum = 0;

                // ← AGREGAR ESTA LÍNEA para controlar el área interna
                chart.ChartAreas[0].InnerPlotPosition = new ElementPosition(11, 0, 85, 80);
                //                                                          X   Y   Ancho Alto
            }
        }

        // ===== MÉTODOS AUXILIARES =====
        private void VolverAlDashboard()
        {
            FormDashboardRoles dashboard = new FormDashboardRoles(formPrincipal);
            formPrincipal.CargarContenidoPanel(dashboard);
        }

        // ===== MÉTODOS DEL TAB 1: HISTORIAL =====

        private void CargarFiltrosHistorial()
        {
            try
            {
                var cmbUsuario = EncontrarControl<ComboBox>(tabHistorial, "cmbUsuario");
                var cmbAccion = EncontrarControl<ComboBox>(tabHistorial, "cmbAccion");

                if (cmbUsuario == null || cmbAccion == null) return;

                // CARGAR ComboBox de USUARIOS
                cmbUsuario.Items.Clear();
                cmbUsuario.Items.Add("Todos los Usuarios");

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"
                SELECT UsuarioID, Username, NombreCompleto 
                FROM Usuarios 
                WHERE Activo = 1 
                ORDER BY NombreCompleto";

                    using (var cmd = new SqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbUsuario.Items.Add(new ComboBoxUsuario
                            {
                                UsuarioID = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                NombreCompleto = reader.GetString(2)
                            });
                        }
                    }
                }

                cmbUsuario.DisplayMember = "Display";
                cmbUsuario.ValueMember = "UsuarioID";
                cmbUsuario.SelectedIndex = 0;

                // CARGAR ComboBox de ACCIONES
                // Solo acciones relacionadas con Gestión de Roles y Permisos
                cmbAccion.Items.Clear();
                cmbAccion.Items.Add("Todas las Acciones");
                cmbAccion.Items.Add("MODIFICAR_PERMISOS_ROL");
                cmbAccion.Items.Add("MODIFICAR_PERMISOS_USUARIO");
                cmbAccion.Items.Add("CREAR_ROL");
                cmbAccion.Items.Add("MODIFICAR_ROL");
                cmbAccion.Items.Add("ELIMINAR_ROL");
                cmbAccion.Items.Add("COPIAR_PERMISOS");
                cmbAccion.Items.Add("RESTAURAR_PERMISOS");
                cmbAccion.Items.Add("CONFIRMAR_PASSWORD_CAMBIO_CRITICO");
                cmbAccion.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cargar filtros:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void CargarHistorial()
        {
            try
            {
                // Obtener controles
                var dgv = EncontrarControl<DataGridView>(tabHistorial, "dgvHistorial");
                var dtpDesde = EncontrarControl<DateTimePicker>(tabHistorial, "dtpDesde");
                var dtpHasta = EncontrarControl<DateTimePicker>(tabHistorial, "dtpHasta");
                var cmbUsuario = EncontrarControl<ComboBox>(tabHistorial, "cmbUsuario");
                var cmbAccion = EncontrarControl<ComboBox>(tabHistorial, "cmbAccion");
                var lblTotal = EncontrarControl<Label>(tabHistorial, "lblTotal");

                if (dgv == null) return;

                this.Cursor = Cursors.WaitCursor;

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Construir query SQL con filtros
                    StringBuilder sql = new StringBuilder(@"
                SELECT TOP 1000
                    A.AuditoriaID,
                    A.FechaHora,
                    U.Username,
                    A.Accion,
                    A.Categoria,
                    A.Modulo,
                    A.Formulario,
                    A.Detalle
                FROM Auditoria A
                INNER JOIN Usuarios U ON A.UsuarioID = U.UsuarioID
                WHERE 1=1");

                    // Filtro: Solo acciones de Gestión de Roles y Permisos
                    sql.Append(" AND (A.Categoria = 'SISTEMA' AND A.Modulo = 'Gestión de Roles')");

                    // Filtro: Rango de fechas
                    sql.Append(" AND A.FechaHora >= @FechaDesde");
                    sql.Append(" AND A.FechaHora < @FechaHasta");

                    // Filtro: Usuario específico (si se seleccionó)
                    if (cmbUsuario != null && cmbUsuario.SelectedIndex > 0)
                    {
                        sql.Append(" AND A.UsuarioID = @UsuarioID");
                    }

                    // Filtro: Acción específica (si se seleccionó)
                    if (cmbAccion != null && cmbAccion.SelectedIndex > 0)
                    {
                        sql.Append(" AND A.Accion = @Accion");
                    }

                    // Filtro de permisos: ADMIN no ve acciones de ROOT
                    if (!SesionActual.EsRoot() && SesionActual.EsAdmin())
                    {
                        sql.Append(" AND U.RolID <> 1");
                    }

                    sql.Append(" ORDER BY A.FechaHora DESC");

                    using (var cmd = new SqlCommand(sql.ToString(), conn))
                    {
                        // Parámetros de fecha
                        cmd.Parameters.AddWithValue("@FechaDesde", dtpDesde?.Value.Date ?? DateTime.Now.AddMonths(-1));
                        cmd.Parameters.AddWithValue("@FechaHasta", (dtpHasta?.Value.Date.AddDays(1)) ?? DateTime.Now.AddDays(1));

                        // Parámetro de usuario
                        if (cmbUsuario != null && cmbUsuario.SelectedIndex > 0)
                        {
                            var usuario = cmbUsuario.SelectedItem as ComboBoxUsuario;
                            if (usuario != null)
                                cmd.Parameters.AddWithValue("@UsuarioID", usuario.UsuarioID);
                        }

                        // Parámetro de acción
                        if (cmbAccion != null && cmbAccion.SelectedIndex > 0)
                        {
                            cmd.Parameters.AddWithValue("@Accion", cmbAccion.SelectedItem.ToString());
                        }

                        // Ejecutar query y llenar DataTable
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            dtAuditoria = new DataTable();
                            adapter.Fill(dtAuditoria);
                        }
                    }
                }

                // Limpiar y llenar el DataGridView
                dgv.Rows.Clear();

                foreach (DataRow row in dtAuditoria.Rows)
                {
                    var dgvRow = new DataGridViewRow();
                    dgvRow.CreateCells(dgv);

                    dgvRow.Cells[0].Value = row["AuditoriaID"];
                    dgvRow.Cells[1].Value = row["FechaHora"];
                    dgvRow.Cells[2].Value = row["Username"];
                    dgvRow.Cells[3].Value = row["Accion"];
                    dgvRow.Cells[4].Value = row["Modulo"];
                    dgvRow.Cells[5].Value = row["Detalle"];

                    // Guardar datos completos en Tag para el modal de detalle
                    dgvRow.Tag = new
                    {
                        FechaHora = Convert.ToDateTime(row["FechaHora"]),
                        Usuario = row["Username"].ToString(),
                        Accion = row["Accion"].ToString(),
                        Categoria = row["Categoria"] != DBNull.Value ? row["Categoria"].ToString() : "",
                        Modulo = row["Modulo"] != DBNull.Value ? row["Modulo"].ToString() : "",
                        Formulario = row["Formulario"] != DBNull.Value ? row["Formulario"].ToString() : "",
                        Detalle = row["Detalle"] != DBNull.Value ? row["Detalle"].ToString() : ""
                    };

                    dgv.Rows.Add(dgvRow);
                }

                // Actualizar label de total
                if (lblTotal != null)
                    lblTotal.Text = $"Total de registros: {dtAuditoria.Rows.Count}";

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(
                    $"Error al cargar historial:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void LimpiarFiltrosHistorial()
        {
            var dtpDesde = EncontrarControl<DateTimePicker>(tabHistorial, "dtpDesde");
            var dtpHasta = EncontrarControl<DateTimePicker>(tabHistorial, "dtpHasta");
            var cmbUsuario = EncontrarControl<ComboBox>(tabHistorial, "cmbUsuario");
            var cmbAccion = EncontrarControl<ComboBox>(tabHistorial, "cmbAccion");

            // Resetear valores a los predeterminados
            if (dtpDesde != null) dtpDesde.Value = DateTime.Now.AddMonths(-1);
            if (dtpHasta != null) dtpHasta.Value = DateTime.Now;
            if (cmbUsuario != null) cmbUsuario.SelectedIndex = 0;
            if (cmbAccion != null) cmbAccion.SelectedIndex = 0;

            // Recargar con filtros limpios
            CargarHistorial();
        }

        private void ExportarHistorialExcel()
        {
            var dgv = EncontrarControl<DataGridView>(tabHistorial, "dgvHistorial");

            if (dgv == null || dgv.Rows.Count == 0)
            {
                MessageBox.Show(
                    "No hay datos para exportar.",
                    "Información",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    FileName = $"Historial_Permisos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveDialog.ShowDialog() != DialogResult.OK) return;

                this.Cursor = Cursors.WaitCursor;

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Historial de Cambios");

                    // ENCABEZADO DEL REPORTE
                    worksheet.Cell(1, 1).Value = "HISTORIAL DE CAMBIOS - GESTIÓN DE ROLES Y PERMISOS";
                    worksheet.Range(1, 1, 1, 6).Merge();
                    worksheet.Cell(1, 1).Style.Font.Bold = true;
                    worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                    worksheet.Cell(1, 1).Style.Font.FontColor = XLColor.FromArgb(0, 120, 212);
                    worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // INFORMACIÓN DEL REPORTE
                    worksheet.Cell(2, 1).Value = $"Generado por: {SesionActual.NombreCompleto} ({SesionActual.Username})";
                    worksheet.Cell(2, 1).Style.Font.Italic = true;

                    worksheet.Cell(3, 1).Value = $"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    worksheet.Cell(3, 1).Style.Font.Italic = true;

                    // FILTROS APLICADOS
                    var dtpDesde = EncontrarControl<DateTimePicker>(tabHistorial, "dtpDesde");
                    var dtpHasta = EncontrarControl<DateTimePicker>(tabHistorial, "dtpHasta");
                    var cmbUsuario = EncontrarControl<ComboBox>(tabHistorial, "cmbUsuario");
                    var cmbAccion = EncontrarControl<ComboBox>(tabHistorial, "cmbAccion");

                    worksheet.Cell(4, 1).Value = "Filtros aplicados:";
                    worksheet.Cell(4, 1).Style.Font.Bold = true;

                    worksheet.Cell(5, 1).Value = $"Período: {dtpDesde?.Value:dd/MM/yyyy} al {dtpHasta?.Value:dd/MM/yyyy}";
                    worksheet.Cell(6, 1).Value = $"Usuario: {cmbUsuario?.SelectedItem}  |  Acción: {cmbAccion?.SelectedItem}";

                    // ENCABEZADOS DE COLUMNAS
                    int headerRow = 8;
                    for (int i = 1; i < dgv.Columns.Count; i++) // Saltar columna ID (oculta)
                    {
                        worksheet.Cell(headerRow, i).Value = dgv.Columns[i].HeaderText;
                    }

                    // Estilo de encabezados
                    var headerRange = worksheet.Range(headerRow, 1, headerRow, dgv.Columns.Count - 1);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
                    headerRange.Style.Font.FontColor = XLColor.White;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // DATOS
                    int row = headerRow + 1;
                    foreach (DataGridViewRow dgvRow in dgv.Rows)
                    {
                        for (int col = 1; col < dgv.Columns.Count; col++) // Saltar columna ID
                        {
                            var cell = worksheet.Cell(row, col);
                            var value = dgvRow.Cells[col].Value;

                            if (value is DateTime)
                                cell.Value = ((DateTime)value).ToString("dd/MM/yyyy HH:mm:ss");
                            else
                                cell.Value = value?.ToString() ?? "";
                        }
                        row++;
                    }

                    // AUTO-AJUSTAR COLUMNAS
                    worksheet.Columns().AdjustToContents();

                    // BORDES
                    worksheet.Range(headerRow, 1, row - 1, dgv.Columns.Count - 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(headerRow, 1, row - 1, dgv.Columns.Count - 1).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    // GUARDAR ARCHIVO
                    workbook.SaveAs(saveDialog.FileName);
                }

                this.Cursor = Cursors.Default;

                MessageBox.Show(
                    "Archivo exportado exitosamente.",
                    "Éxito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Registrar en auditoría
                AuditoriaHelper.RegistrarAccion(
                    SesionActual.UsuarioID,
                    "EXPORTAR_HISTORIAL_PERMISOS",
                    "SISTEMA",
                    "Gestión de Roles",
                    "FormReportesPermisos",
                    detalle: $"Archivo: {saveDialog.FileName}"
                );
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(
                    $"Error al exportar:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void ExportarAuditoriaPDF()
        {
            // CAMBIO: tabAuditoria → tabHistorial, dgvAuditoria → dgvHistorial
            var dgv = EncontrarControl<DataGridView>(tabHistorial, "dgvHistorial");
            if (dgv == null || dgv.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Files|*.pdf",
                    FileName = $"Analisis_Auditoria_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (saveDialog.ShowDialog() != DialogResult.OK) return;

                this.Cursor = Cursors.WaitCursor;

                // Obtener filtros aplicados - CAMBIO: tabAuditoria → tabHistorial
                var dtpDesde = EncontrarControl<DateTimePicker>(tabHistorial, "dtpDesde");
                var dtpHasta = EncontrarControl<DateTimePicker>(tabHistorial, "dtpHasta");
                var cmbUsuario = EncontrarControl<ComboBox>(tabHistorial, "cmbUsuario");
                var cmbAccion = EncontrarControl<ComboBox>(tabHistorial, "cmbAccion");

                string filtroUsuario = cmbUsuario?.SelectedIndex > 0 ? cmbUsuario.SelectedItem.ToString() : "Todos";
                string filtroAccion = cmbAccion?.SelectedIndex > 0 ? cmbAccion.SelectedItem.ToString() : "Todas";

                // Calcular estadísticas del dataset actual
                var estadisticas = CalcularEstadisticasPDF();

                // Generar PDF con QuestPDF
                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(40);

                        // HEADER
                        page.Header().Column(column =>
                        {
                            column.Item().AlignCenter().Text("ANÁLISIS DE AUDITORÍA DE PERMISOS")
                                .FontSize(20).Bold().FontColor("#0078D4");

                            column.Item().PaddingTop(10).AlignCenter().Text("MOFIS ERP - Sistema de Gestión")
                                .FontSize(12).FontColor("#6C757D");

                            column.Item().PaddingTop(5).LineHorizontal(2).LineColor("#0078D4");
                        });

                        // CONTENT
                        page.Content().Column(column =>
                        {
                            // SECCIÓN 1: Información del Reporte
                            column.Item().PaddingTop(20).Column(info =>
                            {
                                info.Item().Text("INFORMACIÓN DEL REPORTE")
                                    .FontSize(14).Bold().FontColor("#0078D4");

                                info.Item().PaddingTop(8).Row(row =>
                                {
                                    row.RelativeItem().Text($"Generado por: {SesionActual.NombreCompleto} ({SesionActual.Username})").FontSize(10);
                                });

                                info.Item().PaddingTop(4).Row(row =>
                                {
                                    row.RelativeItem().Text($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}").FontSize(10);
                                });

                                info.Item().PaddingTop(4).Row(row =>
                                {
                                    row.RelativeItem().Text($"Período analizado: {dtpDesde?.Value:dd/MM/yyyy} - {dtpHasta?.Value:dd/MM/yyyy}").FontSize(10);
                                });
                            });

                            // SECCIÓN 2: Filtros Aplicados
                            column.Item().PaddingTop(20).Column(filtros =>
                            {
                                filtros.Item().Text("FILTROS APLICADOS")
                                    .FontSize(14).Bold().FontColor("#0078D4");

                                filtros.Item().PaddingTop(8).Row(row =>
                                {
                                    row.RelativeItem().Text($"Usuario: {filtroUsuario}").FontSize(10);
                                });

                                filtros.Item().PaddingTop(4).Row(row =>
                                {
                                    row.RelativeItem().Text($"Acción: {filtroAccion}").FontSize(10);
                                });
                            });

                            // SECCIÓN 3: Resumen Ejecutivo
                            column.Item().PaddingTop(20).Column(resumen =>
                            {
                                resumen.Item().Text("RESUMEN EJECUTIVO")
                                    .FontSize(14).Bold().FontColor("#0078D4");

                                resumen.Item().PaddingTop(10).Background("#F0F8FF").Padding(15).Column(stats =>
                                {
                                    stats.Item().Text($"Total de Registros: {estadisticas["TotalRegistros"]}")
                                        .FontSize(12).Bold().FontColor("#228B22");

                                    stats.Item().PaddingTop(8).Text("Distribución por Tipo de Acción:")
                                        .FontSize(11).Bold();

                                    foreach (var accion in estadisticas["Acciones"] as Dictionary<string, int>)
                                    {
                                        stats.Item().PaddingTop(4).PaddingLeft(15).Text($"• {accion.Key}: {accion.Value} cambios")
                                            .FontSize(10);
                                    }

                                    stats.Item().PaddingTop(8).Text("Usuarios Involucrados:")
                                        .FontSize(11).Bold();

                                    foreach (var usuario in estadisticas["Usuarios"] as Dictionary<string, int>)
                                    {
                                        stats.Item().PaddingTop(4).PaddingLeft(15).Text($"• {usuario.Key}: {usuario.Value} acciones")
                                            .FontSize(10);
                                    }
                                });
                            });

                            // SECCIÓN 4: Top 10 Cambios Más Recientes
                            column.Item().PaddingTop(20).Column(tabla =>
                            {
                                tabla.Item().Text("TOP 10 CAMBIOS MÁS RECIENTES")
                                    .FontSize(14).Bold().FontColor("#0078D4");

                                tabla.Item().PaddingTop(10).Table(table =>
                                {
                                    // Definir columnas
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2); // Fecha/Hora
                                        columns.RelativeColumn(2); // Usuario
                                        columns.RelativeColumn(3); // Acción
                                        columns.RelativeColumn(3); // Detalle
                                    });

                                    // Header
                                    table.Header(header =>
                                    {
                                        header.Cell().Background("#0078D4").Padding(5).Text("Fecha/Hora")
                                            .FontSize(9).Bold().FontColor("#FFFFFF");
                                        header.Cell().Background("#0078D4").Padding(5).Text("Usuario")
                                            .FontSize(9).Bold().FontColor("#FFFFFF");
                                        header.Cell().Background("#0078D4").Padding(5).Text("Acción")
                                            .FontSize(9).Bold().FontColor("#FFFFFF");
                                        header.Cell().Background("#0078D4").Padding(5).Text("Detalle")
                                            .FontSize(9).Bold().FontColor("#FFFFFF");
                                    });

                                    // Rows (Top 10)
                                    int count = 0;
                                    foreach (DataGridViewRow row in dgv.Rows)
                                    {
                                        if (count >= 10) break;

                                        var bgColor = count % 2 == 0 ? "#FFFFFF" : "#F5F5F5";

                                        table.Cell().Background(bgColor).BorderBottom(1).BorderColor("#E0E0E0")
                                            .Padding(5).Text(Convert.ToDateTime(row.Cells["FechaHora"].Value).ToString("dd/MM/yyyy HH:mm"))
                                            .FontSize(8);

                                        table.Cell().Background(bgColor).BorderBottom(1).BorderColor("#E0E0E0")
                                            .Padding(5).Text(row.Cells["Usuario"].Value?.ToString() ?? "")
                                            .FontSize(8);

                                        table.Cell().Background(bgColor).BorderBottom(1).BorderColor("#E0E0E0")
                                            .Padding(5).Text(row.Cells["Accion"].Value?.ToString() ?? "")
                                            .FontSize(8);

                                        string detalle = row.Cells["Detalle"].Value?.ToString() ?? "";
                                        if (detalle.Length > 60)
                                            detalle = detalle.Substring(0, 57) + "...";

                                        table.Cell().Background(bgColor).BorderBottom(1).BorderColor("#E0E0E0")
                                            .Padding(5).Text(detalle)
                                            .FontSize(8);

                                        count++;
                                    }
                                });
                            });

                            // SECCIÓN 5: Conclusiones
                            column.Item().PaddingTop(20).Column(conclusiones =>
                            {
                                conclusiones.Item().Text("OBSERVACIONES")
                                    .FontSize(14).Bold().FontColor("#0078D4");

                                conclusiones.Item().PaddingTop(8).Text(GenerarObservacionesAutomaticas(estadisticas))
                                    .FontSize(10).LineHeight(1.5f);
                            });
                        });

                        // FOOTER
                        page.Footer().AlignCenter().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(8).FontColor("#6C757D"));
                            text.Span("Página ");
                            text.CurrentPageNumber();
                            text.Span(" de ");
                            text.TotalPages();
                            text.Span($" • Generado el {DateTime.Now:dd/MM/yyyy HH:mm}");
                        });
                    });
                }).GeneratePdf(saveDialog.FileName);

                this.Cursor = Cursors.Default;
                MessageBox.Show("Análisis PDF exportado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                AuditoriaHelper.RegistrarAccion(SesionActual.UsuarioID, "EXPORTAR_AUDITORIA_PDF", "SISTEMA",
                    "Gestión de Roles", "FormReportesPermisos", detalle: $"Análisis PDF exportado: {saveDialog.FileName}");
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al exportar PDF:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private Dictionary<string, object> CalcularEstadisticasPDF()
        {
            var stats = new Dictionary<string, object>();
            // CAMBIO: tabAuditoria → tabHistorial, dgvAuditoria → dgvHistorial
            var dgv = EncontrarControl<DataGridView>(tabHistorial, "dgvHistorial");

            if (dgv == null || dgv.Rows.Count == 0)
            {
                stats["TotalRegistros"] = 0;
                stats["Acciones"] = new Dictionary<string, int>();
                stats["Usuarios"] = new Dictionary<string, int>();
                return stats;
            }

            stats["TotalRegistros"] = dgv.Rows.Count;

            // Contar por acción
            var acciones = new Dictionary<string, int>();
            var usuarios = new Dictionary<string, int>();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                string accion = row.Cells["Accion"].Value?.ToString() ?? "Sin Acción";
                string usuario = row.Cells["Usuario"].Value?.ToString() ?? "Sin Usuario";

                if (!acciones.ContainsKey(accion))
                    acciones[accion] = 0;
                acciones[accion]++;

                if (!usuarios.ContainsKey(usuario))
                    usuarios[usuario] = 0;
                usuarios[usuario]++;
            }

            // Ordenar y tomar top 5
            stats["Acciones"] = acciones.OrderByDescending(x => x.Value).Take(5).ToDictionary(x => x.Key, x => x.Value);
            stats["Usuarios"] = usuarios.OrderByDescending(x => x.Value).Take(5).ToDictionary(x => x.Key, x => x.Value);

            return stats;
        }

        private string GenerarObservacionesAutomaticas(Dictionary<string, object> estadisticas)
        {
            StringBuilder observaciones = new StringBuilder();
            int total = (int)estadisticas["TotalRegistros"];

            if (total == 0)
            {
                observaciones.AppendLine("No se encontraron registros en el período seleccionado.");
                return observaciones.ToString();
            }

            observaciones.AppendLine($"• Se registraron {total} cambios en el período analizado.");

            var acciones = estadisticas["Acciones"] as Dictionary<string, int>;
            if (acciones != null && acciones.Any())
            {
                var accionPrincipal = acciones.First();
                double porcentaje = (double)accionPrincipal.Value / total * 100;
                observaciones.AppendLine($"• La acción más frecuente fue '{accionPrincipal.Key}' con {accionPrincipal.Value} registros ({porcentaje:F1}%).");
            }

            var usuarios = estadisticas["Usuarios"] as Dictionary<string, int>;
            if (usuarios != null && usuarios.Any())
            {
                var usuarioPrincipal = usuarios.First();
                double porcentaje = (double)usuarioPrincipal.Value / total * 100;
                observaciones.AppendLine($"• El usuario más activo fue '{usuarioPrincipal.Key}' con {usuarioPrincipal.Value} acciones ({porcentaje:F1}%).");
            }

            if (total > 50)
            {
                observaciones.AppendLine($"• Se detectó un volumen alto de actividad en el período ({total} cambios). Se recomienda revisar los patrones.");
            }
            else if (total < 10)
            {
                observaciones.AppendLine($"• Se detectó un volumen bajo de actividad en el período ({total} cambios).");
            }

            return observaciones.ToString();
        }
        

        private void MostrarDetalleCompleto()
        {
            var dgv = EncontrarControl<DataGridView>(tabHistorial, "dgvHistorial");

            if (dgv == null || dgv.SelectedRows.Count == 0) return;

            var row = dgv.SelectedRows[0];
            var detalle = row.Tag as dynamic;

            if (detalle == null) return;

            // Mostrar modal con detalle completo
            FormDetalleAuditoria formDetalle = new FormDetalleAuditoria(
                detalle.FechaHora,
                detalle.Usuario,
                detalle.Accion,
                detalle.Categoria,
                detalle.Modulo,
                detalle.Formulario,
                detalle.Detalle
            );

            formDetalle.ShowDialog();
        }


        // Método helper para buscar controles por nombre
        private T EncontrarControl<T>(Control parent, string nombre) where T : Control
        {
            foreach (Control control in parent.Controls)
            {
                if (control is T && control.Name == nombre)
                    return (T)control;

                var found = EncontrarControl<T>(control, nombre);
                if (found != null)
                    return found;
            }
            return null;
        }
    }

    // ===== CLASE AUXILIAR: ComboBoxUsuario =====
    // Clase para mostrar usuarios en el ComboBox con formato: "Nombre Completo (Username)"
    public class ComboBoxUsuario
    {
        public int UsuarioID { get; set; }
        public string Username { get; set; }
        public string NombreCompleto { get; set; }

        // Propiedad Display: se muestra en el ComboBox
        public string Display => $"{NombreCompleto} ({Username})";

        public override string ToString()
        {
            return Display;
        }
    }

    // ===== FORMULARIO MODAL: Detalle Completo de Auditoría =====
    public class FormDetalleAuditoria : Form
    {
        public FormDetalleAuditoria(
            DateTime fechaHora,
            string usuario,
            string accion,
            string categoria,
            string modulo,
            string formulario,
            string detalle)
        {
            InitializeComponent();

            // Configuración del formulario modal
            this.Text = "Detalle Completo de Auditoría";
            this.Size = new Size(750, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Panel principal con padding
            Panel panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(25),
                BackColor = Color.White
            };
            this.Controls.Add(panelPrincipal);

            // TÍTULO DEL MODAL
            Label lblTitulo = new Label
            {
                Text = "INFORMACIÓN DETALLADA DEL CAMBIO",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 212),
                AutoSize = true,
                Location = new Point(25, 25)
            };
            panelPrincipal.Controls.Add(lblTitulo);

            // Línea divisoria decorativa
            Panel lineaDivisoria = new Panel
            {
                Height = 3,
                Width = 700,
                BackColor = Color.FromArgb(0, 120, 212),
                Location = new Point(25, 60)
            };
            panelPrincipal.Controls.Add(lineaDivisoria);

            // CAMPOS DE INFORMACIÓN
            int y = 80;

            AgregarCampo(panelPrincipal, "📅 Fecha y Hora:", fechaHora.ToString("dddd, dd 'de' MMMM 'de' yyyy - HH:mm:ss"), ref y);
            AgregarCampo(panelPrincipal, "👤 Usuario:", usuario, ref y);
            AgregarCampo(panelPrincipal, "⚡ Acción Realizada:", accion, ref y);

            if (!string.IsNullOrEmpty(categoria))
                AgregarCampo(panelPrincipal, "📂 Categoría:", categoria, ref y);

            if (!string.IsNullOrEmpty(modulo))
                AgregarCampo(panelPrincipal, "🔧 Módulo:", modulo, ref y);

            if (!string.IsNullOrEmpty(formulario))
                AgregarCampo(panelPrincipal, "📝 Formulario:", formulario, ref y);

            // SECCIÓN DE DETALLE (con TextBox multilínea)
            Label lblDetalleTitulo = new Label
            {
                Text = "📋 Detalle Completo:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 212),
                AutoSize = true,
                Location = new Point(25, y)
            };
            panelPrincipal.Controls.Add(lblDetalleTitulo);

            y += 30;

            // Panel contenedor del TextBox (con borde)
            Panel panelDetalle = new Panel
            {
                Location = new Point(25, y),
                Size = new Size(680, 180),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 250, 250)
            };
            panelPrincipal.Controls.Add(panelDetalle);

            TextBox txtDetalle = new TextBox
            {
                Text = detalle,
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.None,
                Padding = new Padding(10)
            };
            panelDetalle.Controls.Add(txtDetalle);

            y += 195;

            // BOTÓN CERRAR
            Button btnCerrar = new Button
            {
                Text = "✖ Cerrar",
                Size = new Size(140, 45),
                Location = new Point(565, y),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => this.Close();

            // Efecto hover en botón
            btnCerrar.MouseEnter += (s, e) => btnCerrar.BackColor = Color.FromArgb(90, 98, 104);
            btnCerrar.MouseLeave += (s, e) => btnCerrar.BackColor = Color.FromArgb(108, 117, 125);

            panelPrincipal.Controls.Add(btnCerrar);
        }

        // Método auxiliar para agregar campos de información
        private void AgregarCampo(Panel panel, string etiqueta, string valor, ref int y)
        {
            // Panel contenedor del campo (con fondo alternado)
            Panel panelCampo = new Panel
            {
                Location = new Point(25, y),
                Size = new Size(680, 40),
                BackColor = Color.FromArgb(248, 249, 250)
            };
            panel.Controls.Add(panelCampo);

            // Etiqueta (label a la izquierda)
            Label lblEtiqueta = new Label
            {
                Text = etiqueta,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                AutoSize = false,
                Size = new Size(200, 40),
                Location = new Point(15, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };
            panelCampo.Controls.Add(lblEtiqueta);

            // Valor (label a la derecha)
            Label lblValor = new Label
            {
                Text = valor,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(33, 37, 41),
                AutoSize = false,
                Size = new Size(450, 40),
                Location = new Point(220, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };
            panelCampo.Controls.Add(lblValor);

            y += 45;
        }

        // Método requerido por Windows Forms
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(750, 550);
            this.Name = "FormDetalleAuditoria";
            this.ResumeLayout(false);
        }
    }
}
