using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MOFIS_ERP.Classes;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace MOFIS_ERP.Forms.Sistema.Auditoria
{
    public partial class FormDashboardAuditoria : Form
    {
        private FormMain formPrincipal;

        // Colores corporativos
        private readonly Color colorAzul = Color.FromArgb(0, 120, 212);
        private readonly Color colorMorado = Color.FromArgb(156, 39, 176);
        private readonly Color colorVerde = Color.FromArgb(34, 139, 34);
        private readonly Color colorRojo = Color.FromArgb(220, 53, 69);
        private readonly Color colorNaranja = Color.FromArgb(255, 152, 0);
        private readonly Color colorPurpura = Color.FromArgb(103, 58, 183);

        // Variables de datos
        private DateTime fechaDesde;
        private DateTime fechaHasta;

        // Variables para filtros
        private string filtroModulo = null;
        private string filtroUsuario = null;

        // Tamaños originales de los gráficos
        private Dictionary<Chart, Size> tamañosOriginalesGraficos;
        private Dictionary<Chart, Point> posicionesOriginalesGraficos;

        public FormDashboardAuditoria(FormMain formMain)
        {
            InitializeComponent();
            formPrincipal = formMain;

            GuardarTamañosOriginales();
            ConfigurarFormulario();

            this.Load += async (s, e) =>
            {
                AplicarTamañosFijos();
                await CargarDatosDashboardAsync();
            };

            this.Resize += (s, e) => AplicarTamañosFijos();
        }

        private void GuardarTamañosOriginales()
        {
            tamañosOriginalesGraficos = new Dictionary<Chart, Size>
            {
                { chartTendencia, new Size(704, 320) },
                { chartTopUsuarios, new Size(704, 320) },
                { chartDistribucion, new Size(534, 565) },
                { chartPorModulo, new Size(525, 565) },
                { chartHeatmap, new Size(944, 320) },
                { chartComparativo, new Size(896, 301) }
            };

            posicionesOriginalesGraficos = new Dictionary<Chart, Point>
            {
                { chartTendencia, new Point(20, 10) },
                { chartTopUsuarios, new Point(20, 340) },
                { chartDistribucion, new Point(779, 28) },
                { chartPorModulo, new Point(1387, 28) },
                { chartHeatmap, new Point(20, 666) },
                { chartComparativo, new Point(970, 666) }
            };
        }

        private void AplicarTamañosFijos()
        {
            foreach (var kvp in tamañosOriginalesGraficos)
            {
                kvp.Key.Size = kvp.Value;
                kvp.Key.MinimumSize = kvp.Value;
                kvp.Key.MaximumSize = kvp.Value;
            }

            foreach (var kvp in posicionesOriginalesGraficos)
                kvp.Key.Location = kvp.Value;
        }

        private void ConfigurarFormulario()
        {
            cmbPeriodo.SelectedIndex = 2;
            CalcularFechasPeriodo();
            dtpDesde.Value = fechaDesde;
            dtpHasta.Value = fechaHasta;

            btnVolver.Click += BtnVolver_Click;
            btnActualizar.Click += BtnActualizar_Click;
            btnExportarPDF.Click += BtnExportarPDF_Click;

            cmbPeriodo.SelectedIndexChanged += CmbPeriodo_SelectedIndexChanged;
            cmbModulo.SelectedIndexChanged += CmbModulo_SelectedIndexChanged;
            cmbUsuario.SelectedIndexChanged += CmbUsuario_SelectedIndexChanged;
            dtpDesde.ValueChanged += DtpFechas_ValueChanged;
            dtpHasta.ValueChanged += DtpFechas_ValueChanged;

            ConfigurarBotonHover(btnVolver, Color.White, colorMorado);
            ConfigurarBotonHover(btnActualizar, colorAzul, Color.White);
            ConfigurarBotonHover(btnExportarPDF, colorRojo, Color.White);

            ConfigurarGraficos();
        }

        private async void DtpFechas_ValueChanged(object sender, EventArgs e)
        {
            if (cmbPeriodo.SelectedIndex == 4)
            {
                fechaDesde = dtpDesde.Value.Date;
                fechaHasta = dtpHasta.Value.Date.AddDays(1).AddSeconds(-1);
                await AplicarFiltrosAsync();
            }
        }

        private async void CmbPeriodo_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalcularFechasPeriodo();
            bool esPersonalizado = cmbPeriodo.SelectedIndex == 4;
            lblDesde.Visible = esPersonalizado;
            dtpDesde.Visible = esPersonalizado;
            lblHasta.Visible = esPersonalizado;
            dtpHasta.Visible = esPersonalizado;

            if (esPersonalizado)
            {
                dtpDesde.Value = fechaDesde;
                dtpHasta.Value = fechaHasta;
            }

            await AplicarFiltrosAsync();
        }

        private async void CmbModulo_SelectedIndexChanged(object sender, EventArgs e)
        {
            filtroModulo = cmbModulo.SelectedIndex > 0 ? cmbModulo.SelectedItem.ToString() : null;
            await AplicarFiltrosAsync();
        }

        private async void CmbUsuario_SelectedIndexChanged(object sender, EventArgs e)
        {
            filtroUsuario = cmbUsuario.SelectedIndex > 0 ? cmbUsuario.SelectedItem.ToString() : null;
            await AplicarFiltrosAsync();
        }

        private void ConfigurarGraficos()
        {
            ConfigurarGraficoBase(chartTendencia, "📈 TENDENCIA DE ACCIONES", colorAzul);
            chartTendencia.Series[0].ChartType = SeriesChartType.Line;
            chartTendencia.Series[0].BorderWidth = 3;
            chartTendencia.Series[0].Color = colorAzul;

            ConfigurarGraficoBase(chartPorModulo, "📦 ACCIONES POR MÓDULO", colorVerde);
            chartPorModulo.Series[0].ChartType = SeriesChartType.Column;

            ConfigurarGraficoBase(chartTopUsuarios, "👥 TOP 10 USUARIOS MÁS ACTIVOS", colorMorado);
            chartTopUsuarios.Series[0].ChartType = SeriesChartType.Bar;

            ConfigurarGraficoBase(chartDistribucion, "🎯 DISTRIBUCIÓN POR TIPO DE ACCIÓN", colorNaranja);
            chartDistribucion.Series[0].ChartType = SeriesChartType.Doughnut;
            chartDistribucion.Legends[0].Enabled = true;

            ConfigurarGraficoBase(chartHeatmap, "⏰ ACTIVIDAD POR HORA DEL DÍA", colorRojo);
            chartHeatmap.Series[0].ChartType = SeriesChartType.Column;

            ConfigurarGraficoBase(chartComparativo, "📊 COMPARATIVA PERIODOS", colorAzul);
            chartComparativo.Series.Clear();
            chartComparativo.Series.Add("Periodo Actual");
            chartComparativo.Series.Add("Periodo Anterior");
            chartComparativo.Series[0].ChartType = SeriesChartType.Column;
            chartComparativo.Series[1].ChartType = SeriesChartType.Column;
            chartComparativo.Series[0].Color = colorAzul;
            chartComparativo.Series[1].Color = Color.Gray;
        }

        private void ConfigurarGraficoBase(Chart chart, string titulo, Color color)
        {
            chart.Titles.Clear();
            chart.Titles.Add(titulo);
            chart.Titles[0].Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold);
            chart.Titles[0].ForeColor = color;

            chart.ChartAreas[0].BackColor = Color.White;
            chart.ChartAreas[0].BorderColor = Color.LightGray;
            chart.ChartAreas[0].BorderWidth = 1;
            chart.ChartAreas[0].BorderDashStyle = ChartDashStyle.Solid;

            if (chart.Series.Count > 0)
            {
                chart.Series[0].Color = color;
                chart.Series[0].IsValueShownAsLabel = true;
                chart.Series[0].Font = new System.Drawing.Font("Segoe UI", 8);
            }

            chart.Legends[0].Enabled = false;
            chart.BackColor = Color.Transparent;

            var area = chart.ChartAreas[0];
            area.CursorX.IsUserEnabled = true;
            area.CursorX.IsUserSelectionEnabled = true;
            area.AxisX.ScaleView.Zoomable = true;
            area.CursorY.IsUserEnabled = true;
            area.CursorY.IsUserSelectionEnabled = true;
            area.AxisY.ScaleView.Zoomable = true;

            chart.MouseDoubleClick += (s, e) =>
            {
                area.AxisX.ScaleView.ZoomReset();
                area.AxisY.ScaleView.ZoomReset();
            };

            chart.MouseClick += Chart_MouseClick;
        }

        private void Chart_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            var chart = sender as Chart;
            if (chart == null) return;

            HitTestResult result = chart.HitTest(e.X, e.Y);
            if (result.ChartElementType == ChartElementType.DataPoint)
            {
                var point = result.Series.Points[result.PointIndex];
                string label = point.AxisLabel;
                double valor = point.YValues[0];

                string mensaje = $"📊 Detalle de: {label}\n\n" +
                                 $"Total de acciones: {valor:N0}\n" +
                                 $"Periodo: {fechaDesde:dd/MM/yyyy} - {fechaHasta:dd/MM/yyyy}";

                if (filtroModulo != null) mensaje += $"\nMódulo: {filtroModulo}";
                if (filtroUsuario != null) mensaje += $"\nUsuario: {filtroUsuario}";

                MessageBox.Show(mensaje, "Detalle", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ConfigurarBotonHover(Button btn, Color colorNormal, Color colorTexto)
        {
            btn.ForeColor = colorTexto;
            Color colorOriginal = btn.BackColor;

            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Dark(colorOriginal, 0.1f);
            btn.MouseLeave += (s, e) => btn.BackColor = colorOriginal;
        }

        private void CalcularFechasPeriodo()
        {
            fechaHasta = DateTime.Now;
            switch (cmbPeriodo.SelectedIndex)
            {
                case 0: fechaDesde = DateTime.Today; break;
                case 1: fechaDesde = DateTime.Now.AddDays(-7); break;
                case 2: fechaDesde = DateTime.Now.AddMonths(-1); break;
                case 3: fechaDesde = DateTime.Now.AddYears(-1); break;
                case 4:
                    if (dtpDesde != null && dtpHasta != null)
                    {
                        fechaDesde = dtpDesde.Value.Date;
                        fechaHasta = dtpHasta.Value.Date.AddDays(1).AddSeconds(-1);
                    }
                    else fechaDesde = DateTime.Now.AddMonths(-1);
                    break;
                default: fechaDesde = DateTime.Now.AddMonths(-1); break;
            }
        }

        private string ConstruirFiltrosSQL()
        {
            var filtros = new List<string> { "A.FechaHora BETWEEN @FechaDesde AND @FechaHasta" };
            if (!string.IsNullOrEmpty(filtroModulo)) filtros.Add("A.Modulo = @Modulo");
            if (!string.IsNullOrEmpty(filtroUsuario)) filtros.Add("U.Username = @Usuario");
            return string.Join(" AND ", filtros);
        }

        private void AgregarParametrosFiltro(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@FechaDesde", fechaDesde);
            cmd.Parameters.AddWithValue("@FechaHasta", fechaHasta);
            if (!string.IsNullOrEmpty(filtroModulo)) cmd.Parameters.AddWithValue("@Modulo", filtroModulo);
            if (!string.IsNullOrEmpty(filtroUsuario)) cmd.Parameters.AddWithValue("@Usuario", filtroUsuario);
        }

        private async Task CargarDatosDashboardAsync()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                await CargarFiltrosAsync();
                await CargarKPIsAsync();

                await Task.WhenAll(
                    CargarGraficoTendenciaAsync(),
                    CargarGraficoPorModuloAsync(),
                    CargarGraficoTopUsuariosAsync(),
                    CargarGraficoDistribucionAsync(),
                    CargarGraficoHeatmapAsync(),
                    CargarGraficoComparativoAsync()
                );

                AuditoriaHelper.RegistrarAccion(
                    SesionActual.UsuarioID,
                    AuditoriaAcciones.AuditoriaGeneral.CONSULTAR_AUDITORIA,
                    AuditoriaAcciones.Categorias.SISTEMA,
                    AuditoriaAcciones.Modulos.AUDITORIA_GENERAL,
                    "FormDashboardAuditoria",
                    detalle: "Consulta de Dashboard de Auditoría"
                );

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al cargar dashboard:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CargarFiltrosAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();
                        var modulos = new List<string>();
                        using (var cmd = new SqlCommand("SELECT DISTINCT Modulo FROM Auditoria WHERE Modulo IS NOT NULL ORDER BY Modulo", conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read()) modulos.Add(reader.GetString(0));
                        }

                        // Cargar TODOS los usuarios activos del sistema (no solo los de auditoría)
                        var usuarios = new List<string>();
                        using (var cmd = new SqlCommand(@"SELECT Username FROM Usuarios 
                            WHERE Activo = 1 AND EsEliminado = 0 ORDER BY Username", conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read()) usuarios.Add(reader.GetString(0));
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            cmbModulo.SelectedIndexChanged -= CmbModulo_SelectedIndexChanged;
                            cmbUsuario.SelectedIndexChanged -= CmbUsuario_SelectedIndexChanged;

                            cmbModulo.Items.Clear();
                            cmbModulo.Items.Add("Todos los Módulos");
                            cmbModulo.Items.AddRange(modulos.ToArray());
                            cmbModulo.SelectedIndex = 0;

                            cmbUsuario.Items.Clear();
                            cmbUsuario.Items.Add("Todos los Usuarios");
                            cmbUsuario.Items.AddRange(usuarios.ToArray());
                            cmbUsuario.SelectedIndex = 0;

                            cmbModulo.SelectedIndexChanged += CmbModulo_SelectedIndexChanged;
                            cmbUsuario.SelectedIndexChanged += CmbUsuario_SelectedIndexChanged;
                        });
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show($"Error al cargar filtros:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            });
        }

        private async Task CargarKPIsAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();
                        int totalRegistros = 0;
                        using (var cmd = new SqlCommand($"SELECT COUNT(*) FROM Auditoria A LEFT JOIN Usuarios U ON A.UsuarioID = U.UsuarioID WHERE {ConstruirFiltrosSQL()}", conn))
                        {
                            AgregarParametrosFiltro(cmd);
                            totalRegistros = (int)cmd.ExecuteScalar();
                        }

                        string usuarioActivo = "N/A";
                        using (var cmd = new SqlCommand($@"SELECT TOP 1 U.Username FROM Auditoria A 
                            INNER JOIN Usuarios U ON A.UsuarioID = U.UsuarioID WHERE {ConstruirFiltrosSQL()} 
                            GROUP BY U.Username ORDER BY COUNT(*) DESC", conn))
                        {
                            AgregarParametrosFiltro(cmd);
                            var result = cmd.ExecuteScalar();
                            if (result != null) usuarioActivo = result.ToString();
                        }

                        string moduloMasUsado = "N/A";
                        using (var cmd = new SqlCommand($@"SELECT TOP 1 A.Modulo FROM Auditoria A 
                            LEFT JOIN Usuarios U ON A.UsuarioID = U.UsuarioID WHERE {ConstruirFiltrosSQL()} 
                            AND A.Modulo IS NOT NULL GROUP BY A.Modulo ORDER BY COUNT(*) DESC", conn))
                        {
                            AgregarParametrosFiltro(cmd);
                            var result = cmd.ExecuteScalar();
                            if (result != null) moduloMasUsado = result.ToString();
                        }

                        string horaPico = "N/A";
                        using (var cmd = new SqlCommand($@"SELECT TOP 1 DATEPART(HOUR, A.FechaHora) FROM Auditoria A 
                            LEFT JOIN Usuarios U ON A.UsuarioID = U.UsuarioID WHERE {ConstruirFiltrosSQL()} 
                            GROUP BY DATEPART(HOUR, A.FechaHora) ORDER BY COUNT(*) DESC", conn))
                        {
                            AgregarParametrosFiltro(cmd);
                            var result = cmd.ExecuteScalar();
                            if (result != null) horaPico = $"{Convert.ToInt32(result):D2}:00";
                        }

                        int diasPeriodo = Math.Max(1, (int)(fechaHasta - fechaDesde).TotalDays);
                        int promedioDiario = totalRegistros / diasPeriodo;

                        this.Invoke((MethodInvoker)delegate
                        {
                            lblTotalRegistrosValor.Text = totalRegistros.ToString("N0");
                            lblUsuarioActivoValor.Text = usuarioActivo;
                            lblModuloMasUsadoValor.Text = moduloMasUsado;
                            lblHoraPicoValor.Text = horaPico;
                            lblAccionesPorDiaValor.Text = promedioDiario.ToString("N0");
                        });
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show($"Error al cargar KPIs:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            });
        }

        private async Task CargarGraficoTendenciaAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();
                        var datos = new Dictionary<DateTime, int>();
                        using (var cmd = new SqlCommand($@"SELECT CAST(A.FechaHora AS DATE) AS Fecha, COUNT(*) AS Total 
                            FROM Auditoria A LEFT JOIN Usuarios U ON A.UsuarioID = U.UsuarioID 
                            WHERE {ConstruirFiltrosSQL()} GROUP BY CAST(A.FechaHora AS DATE) ORDER BY Fecha", conn))
                        {
                            AgregarParametrosFiltro(cmd);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read()) datos.Add(reader.GetDateTime(0), reader.GetInt32(1));
                            }
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            chartTendencia.Series[0].Points.Clear();
                            foreach (var item in datos)
                                chartTendencia.Series[0].Points.AddXY(item.Key.ToString("dd/MM"), item.Value);
                            chartTendencia.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
                            chartTendencia.ChartAreas[0].AxisX.Interval = 1;
                            AnimarGrafico(chartTendencia);
                        });
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show($"Error en gráfico de tendencia:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            });
        }

        private async Task CargarGraficoPorModuloAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();
                        var datos = new Dictionary<string, int>();
                        using (var cmd = new SqlCommand($@"SELECT A.Modulo, COUNT(*) AS Total FROM Auditoria A 
                            LEFT JOIN Usuarios U ON A.UsuarioID = U.UsuarioID WHERE {ConstruirFiltrosSQL()} 
                            AND A.Modulo IS NOT NULL GROUP BY A.Modulo ORDER BY Total DESC", conn))
                        {
                            AgregarParametrosFiltro(cmd);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read()) datos.Add(reader.GetString(0), reader.GetInt32(1));
                            }
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            chartPorModulo.Series[0].Points.Clear();
                            Color[] colores = { colorVerde, colorAzul, colorNaranja, colorMorado, colorRojo };
                            int colorIndex = 0;
                            foreach (var item in datos)
                            {
                                var point = chartPorModulo.Series[0].Points.AddXY(item.Key, item.Value);
                                chartPorModulo.Series[0].Points[point].Color = colores[colorIndex % colores.Length];
                                colorIndex++;
                            }
                            chartPorModulo.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
                            AnimarGrafico(chartPorModulo);
                        });
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show($"Error en gráfico por módulo:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            });
        }

        private async Task CargarGraficoTopUsuariosAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();
                        var datos = new List<KeyValuePair<string, int>>();
                        using (var cmd = new SqlCommand($@"SELECT TOP 10 U.Username, COUNT(*) AS Total FROM Auditoria A 
                            INNER JOIN Usuarios U ON A.UsuarioID = U.UsuarioID WHERE {ConstruirFiltrosSQL()} 
                            GROUP BY U.Username ORDER BY Total DESC", conn))
                        {
                            AgregarParametrosFiltro(cmd);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                    datos.Add(new KeyValuePair<string, int>(reader.GetString(0), reader.GetInt32(1)));
                            }
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            chartTopUsuarios.Series[0].Points.Clear();
                            datos.Reverse();
                            foreach (var item in datos)
                                chartTopUsuarios.Series[0].Points.AddXY(item.Key, item.Value);
                            chartTopUsuarios.Series[0].Color = colorMorado;
                            chartTopUsuarios.Series[0].IsValueShownAsLabel = true;
                            AnimarGrafico(chartTopUsuarios);
                        });
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show($"Error en gráfico top usuarios:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            });
        }

        private async Task CargarGraficoDistribucionAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();
                        var datos = new Dictionary<string, int>();
                        // Solo TOP 5 acciones
                        using (var cmd = new SqlCommand($@"SELECT TOP 5 A.Accion, COUNT(*) AS Total FROM Auditoria A 
                            LEFT JOIN Usuarios U ON A.UsuarioID = U.UsuarioID WHERE {ConstruirFiltrosSQL()} 
                            AND A.Accion IS NOT NULL GROUP BY A.Accion ORDER BY Total DESC", conn))
                        {
                            AgregarParametrosFiltro(cmd);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read()) datos.Add(reader.GetString(0), reader.GetInt32(1));
                            }
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            chartDistribucion.Series[0].Points.Clear();
                            Color[] colores = { colorAzul, colorVerde, colorNaranja, colorMorado, colorRojo };

                            int colorIndex = 0;
                            int totalGeneral = datos.Values.Sum();

                            foreach (var item in datos)
                            {
                                var point = chartDistribucion.Series[0].Points.AddXY(item.Key, item.Value);
                                chartDistribucion.Series[0].Points[point].Color = colores[colorIndex % colores.Length];
                                double porcentaje = totalGeneral > 0 ? (double)item.Value / totalGeneral * 100 : 0;
                                // Etiqueta DENTRO del gráfico
                                chartDistribucion.Series[0].Points[point].Label = $"{porcentaje:F1}%";
                                chartDistribucion.Series[0].Points[point].LegendText = $"{item.Key} ({item.Value:N0})";
                                colorIndex++;
                            }

                            // Configurar etiquetas DENTRO del gráfico (Inside)
                            chartDistribucion.Series[0]["PieLabelStyle"] = "Inside";
                            chartDistribucion.Series[0]["DoughnutRadius"] = "50";
                            chartDistribucion.Series[0].Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);
                            chartDistribucion.Series[0].LabelForeColor = Color.White;

                            chartDistribucion.Legends[0].Enabled = true;
                            chartDistribucion.Legends[0].Docking = Docking.Right;
                            chartDistribucion.Legends[0].Font = new System.Drawing.Font("Segoe UI", 9);

                            AnimarGrafico(chartDistribucion);
                        });
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show($"Error en gráfico de distribución:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            });
        }

        private async Task CargarGraficoHeatmapAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();
                        var datos = new Dictionary<int, int>();
                        for (int i = 0; i < 24; i++) datos[i] = 0;

                        using (var cmd = new SqlCommand($@"SELECT DATEPART(HOUR, A.FechaHora) AS Hora, COUNT(*) AS Total 
                            FROM Auditoria A LEFT JOIN Usuarios U ON A.UsuarioID = U.UsuarioID 
                            WHERE {ConstruirFiltrosSQL()} GROUP BY DATEPART(HOUR, A.FechaHora) ORDER BY Hora", conn))
                        {
                            AgregarParametrosFiltro(cmd);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read()) datos[reader.GetInt32(0)] = reader.GetInt32(1);
                            }
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            chartHeatmap.Series[0].Points.Clear();
                            int maxValor = datos.Values.Max();

                            foreach (var item in datos.OrderBy(x => x.Key))
                            {
                                var point = chartHeatmap.Series[0].Points.AddXY($"{item.Key:D2}:00", item.Value);
                                double intensidad = maxValor > 0 ? (double)item.Value / maxValor : 0;

                                if (intensidad > 0.7) chartHeatmap.Series[0].Points[point].Color = colorRojo;
                                else if (intensidad > 0.4) chartHeatmap.Series[0].Points[point].Color = colorNaranja;
                                else if (intensidad > 0.2) chartHeatmap.Series[0].Points[point].Color = colorVerde;
                                else chartHeatmap.Series[0].Points[point].Color = colorAzul;
                            }

                            chartHeatmap.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
                            chartHeatmap.ChartAreas[0].AxisX.Interval = 1;
                            AnimarGrafico(chartHeatmap);
                        });
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show($"Error en gráfico de heatmap:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            });
        }

        private async Task CargarGraficoComparativoAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();
                        TimeSpan diferencia = fechaHasta - fechaDesde;
                        DateTime fechaDesdePrevio = fechaDesde.Subtract(diferencia);
                        DateTime fechaHastaPrevio = fechaDesde;

                        var datosActuales = new Dictionary<string, int>();
                        using (var cmd = new SqlCommand($@"SELECT A.Modulo, COUNT(*) AS Total FROM Auditoria A 
                            LEFT JOIN Usuarios U ON A.UsuarioID = U.UsuarioID WHERE {ConstruirFiltrosSQL()} 
                            AND A.Modulo IS NOT NULL GROUP BY A.Modulo ORDER BY A.Modulo", conn))
                        {
                            AgregarParametrosFiltro(cmd);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read()) datosActuales.Add(reader.GetString(0), reader.GetInt32(1));
                            }
                        }

                        var datosAnteriores = new Dictionary<string, int>();
                        using (var cmd = new SqlCommand(@"SELECT Modulo, COUNT(*) AS Total FROM Auditoria 
                            WHERE FechaHora BETWEEN @FechaDesdeP AND @FechaHastaP AND Modulo IS NOT NULL 
                            GROUP BY Modulo ORDER BY Modulo", conn))
                        {
                            cmd.Parameters.AddWithValue("@FechaDesdeP", fechaDesdePrevio);
                            cmd.Parameters.AddWithValue("@FechaHastaP", fechaHastaPrevio);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read()) datosAnteriores.Add(reader.GetString(0), reader.GetInt32(1));
                            }
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            chartComparativo.Series[0].Points.Clear();
                            chartComparativo.Series[1].Points.Clear();

                            var todasLasClaves = datosActuales.Keys.Union(datosAnteriores.Keys).OrderBy(x => x);
                            foreach (var modulo in todasLasClaves)
                            {
                                int valorActual = datosActuales.ContainsKey(modulo) ? datosActuales[modulo] : 0;
                                int valorAnterior = datosAnteriores.ContainsKey(modulo) ? datosAnteriores[modulo] : 0;
                                chartComparativo.Series[0].Points.AddXY(modulo, valorActual);
                                chartComparativo.Series[1].Points.AddXY(modulo, valorAnterior);
                            }

                            chartComparativo.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
                            chartComparativo.Legends[0].Enabled = true;
                            AnimarGrafico(chartComparativo);
                        });
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show($"Error en gráfico comparativo:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            });
        }

        private async Task AplicarFiltrosAsync()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                await CargarKPIsAsync();

                await Task.WhenAll(
                    CargarGraficoTendenciaAsync(),
                    CargarGraficoPorModuloAsync(),
                    CargarGraficoTopUsuariosAsync(),
                    CargarGraficoDistribucionAsync(),
                    CargarGraficoHeatmapAsync(),
                    CargarGraficoComparativoAsync()
                );

                AplicarTamañosFijos();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al aplicar filtros:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AnimarGrafico(Chart chart)
        {
            if (chart.Series.Count == 0 || chart.Series[0].Points.Count == 0) return;

            var valoresOriginales = new List<double[]>();
            foreach (var serie in chart.Series)
            {
                var valores = new double[serie.Points.Count];
                for (int i = 0; i < serie.Points.Count; i++)
                {
                    valores[i] = serie.Points[i].YValues[0];
                    serie.Points[i].YValues[0] = 0;
                }
                valoresOriginales.Add(valores);
            }
            chart.Invalidate();

            int paso = 0;
            int pasosTotales = 10;
            Timer timerAnimacion = new Timer { Interval = 30 };
            timerAnimacion.Tick += (s, e) =>
            {
                paso++;
                double progreso = (double)paso / pasosTotales;
                for (int serieIdx = 0; serieIdx < chart.Series.Count; serieIdx++)
                {
                    for (int i = 0; i < chart.Series[serieIdx].Points.Count; i++)
                        chart.Series[serieIdx].Points[i].YValues[0] = valoresOriginales[serieIdx][i] * progreso;
                }
                chart.Invalidate();

                if (paso >= pasosTotales)
                {
                    timerAnimacion.Stop();
                    timerAnimacion.Dispose();
                    for (int serieIdx = 0; serieIdx < chart.Series.Count; serieIdx++)
                    {
                        for (int i = 0; i < chart.Series[serieIdx].Points.Count; i++)
                            chart.Series[serieIdx].Points[i].YValues[0] = valoresOriginales[serieIdx][i];
                    }
                    chart.Invalidate();
                }
            };
            timerAnimacion.Start();
        }

        private void BtnVolver_Click(object sender, EventArgs e)
        {
            FormAuditoria formAuditoria = new FormAuditoria(formPrincipal);
            formPrincipal.CargarContenidoPanel(formAuditoria);
        }

        private async void BtnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                filtroModulo = null;
                filtroUsuario = null;

                cmbPeriodo.SelectedIndexChanged -= CmbPeriodo_SelectedIndexChanged;
                cmbModulo.SelectedIndexChanged -= CmbModulo_SelectedIndexChanged;
                cmbUsuario.SelectedIndexChanged -= CmbUsuario_SelectedIndexChanged;

                cmbPeriodo.SelectedIndex = 2;
                if (cmbModulo.Items.Count > 0) cmbModulo.SelectedIndex = 0;
                if (cmbUsuario.Items.Count > 0) cmbUsuario.SelectedIndex = 0;

                lblDesde.Visible = false;
                dtpDesde.Visible = false;
                lblHasta.Visible = false;
                dtpHasta.Visible = false;

                cmbPeriodo.SelectedIndexChanged += CmbPeriodo_SelectedIndexChanged;
                cmbModulo.SelectedIndexChanged += CmbModulo_SelectedIndexChanged;
                cmbUsuario.SelectedIndexChanged += CmbUsuario_SelectedIndexChanged;

                CalcularFechasPeriodo();
                await CargarDatosDashboardAsync();

                MessageBox.Show("✅ Dashboard actualizado correctamente.", "Actualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExportarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Files|*.pdf",
                    FileName = $"Dashboard_Auditoria_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                    Title = "Guardar Reporte PDF del Dashboard"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;
                    GenerarPDFProfesional(saveDialog.FileName);
                    this.Cursor = Cursors.Default;

                    AuditoriaHelper.RegistrarAccion(
                        SesionActual.UsuarioID,
                        "EXPORTAR_DASHBOARD_PDF",
                        AuditoriaAcciones.Categorias.SISTEMA,
                        AuditoriaAcciones.Modulos.AUDITORIA_GENERAL,
                        "FormDashboardAuditoria",
                        detalle: $"Exportación de Dashboard a PDF: {Path.GetFileName(saveDialog.FileName)}"
                    );

                    MessageBox.Show($"✅ PDF generado exitosamente\n\nArchivo: {Path.GetFileName(saveDialog.FileName)}",
                        "Exportación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (MessageBox.Show("¿Desea abrir el archivo ahora?", "Abrir Archivo",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al exportar dashboard:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerarPDFProfesional(string rutaArchivo)
        {
            var colorCorporativoPDF = new BaseColor(0, 120, 212);
            var colorMoradoPDF = new BaseColor(156, 39, 176);
            var colorVerdePDF = new BaseColor(34, 139, 34);
            var colorRojoPDF = new BaseColor(220, 53, 69);
            var colorNaranjaPDF = new BaseColor(255, 152, 0);

            // Página horizontal para mejor visualización de gráficos
            Document documento = new Document(PageSize.A4.Rotate(), 30, 30, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(documento, new FileStream(rutaArchivo, FileMode.Create));
            documento.Open();

            var fuenteTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 28, colorCorporativoPDF);
            var fuenteSubtitulo = FontFactory.GetFont(FontFactory.HELVETICA, 14, BaseColor.DARK_GRAY);
            var fuenteSeccion = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, colorMoradoPDF);
            var fuenteNormal = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
            var fuenteAnalisis = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.DARK_GRAY);

            // PORTADA
            documento.Add(new Paragraph("\n\n\n\n"));
            var titulo = new Paragraph("MOFIS ERP", fuenteTitulo) { Alignment = Element.ALIGN_CENTER };
            documento.Add(titulo);
            documento.Add(new Paragraph("\n"));
            var subtitulo = new Paragraph("DASHBOARD DE AUDITORÍA DEL SISTEMA", fuenteSeccion) { Alignment = Element.ALIGN_CENTER };
            documento.Add(subtitulo);
            documento.Add(new Paragraph("\n\n"));

            PdfPTable tablaInfo = new PdfPTable(2) { WidthPercentage = 50, HorizontalAlignment = Element.ALIGN_CENTER };
            AgregarFilaInfo(tablaInfo, "Fecha de Generación:", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            AgregarFilaInfo(tablaInfo, "Generado por:", SesionActual.NombreCompleto);
            AgregarFilaInfo(tablaInfo, "Periodo:", $"{fechaDesde:dd/MM/yyyy} - {fechaHasta:dd/MM/yyyy}");
            AgregarFilaInfo(tablaInfo, "Módulo Filtrado:", filtroModulo ?? "Todos");
            AgregarFilaInfo(tablaInfo, "Usuario Filtrado:", filtroUsuario ?? "Todos");
            documento.Add(tablaInfo);

            // RESUMEN EJECUTIVO
            documento.NewPage();
            documento.Add(new Paragraph("RESUMEN EJECUTIVO", fuenteSeccion) { SpacingAfter = 15 });

            PdfPTable tablaKPIs = new PdfPTable(5) { WidthPercentage = 100 };
            tablaKPIs.SetWidths(new float[] { 1, 1, 1, 1, 1 });
            AgregarCeldaKPI(tablaKPIs, "Total Registros", lblTotalRegistrosValor.Text, colorCorporativoPDF);
            AgregarCeldaKPI(tablaKPIs, "Usuario Más Activo", lblUsuarioActivoValor.Text, colorVerdePDF);
            AgregarCeldaKPI(tablaKPIs, "Módulo Más Usado", lblModuloMasUsadoValor.Text, colorNaranjaPDF);
            AgregarCeldaKPI(tablaKPIs, "Hora Pico", lblHoraPicoValor.Text, colorRojoPDF);
            AgregarCeldaKPI(tablaKPIs, "Promedio Diario", lblAccionesPorDiaValor.Text, new BaseColor(103, 58, 183));
            documento.Add(tablaKPIs);

            documento.Add(new Paragraph("\n"));
            documento.Add(new Paragraph(GenerarResumenEjecutivo(), fuenteNormal) { Alignment = Element.ALIGN_JUSTIFIED });

            // PÁGINAS DE GRÁFICOS CON ANÁLISIS (todo en misma página)
            AgregarPaginaGraficoConAnalisis(documento, chartTendencia,
                "TENDENCIA DE ACCIONES EN EL TIEMPO",
                GenerarAnalisisTendencia(),
                fuenteSeccion, fuenteAnalisis);

            AgregarPaginaGraficoConAnalisis(documento, chartPorModulo,
                "DISTRIBUCIÓN DE ACCIONES POR MÓDULO",
                GenerarAnalisisModulos(),
                fuenteSeccion, fuenteAnalisis);

            AgregarPaginaGraficoConAnalisis(documento, chartTopUsuarios,
                "RANKING DE USUARIOS MÁS ACTIVOS",
                GenerarAnalisisUsuarios(),
                fuenteSeccion, fuenteAnalisis);

            AgregarPaginaGraficoConAnalisis(documento, chartDistribucion,
                "COMPOSICIÓN POR TIPO DE ACCIÓN (TOP 5)",
                GenerarAnalisisDistribucion(),
                fuenteSeccion, fuenteAnalisis);

            AgregarPaginaGraficoConAnalisis(documento, chartHeatmap,
                "PATRÓN DE ACTIVIDAD HORARIA",
                GenerarAnalisisHeatmap(),
                fuenteSeccion, fuenteAnalisis);

            AgregarPaginaGraficoConAnalisis(documento, chartComparativo,
                "ANÁLISIS COMPARATIVO DE PERIODOS",
                GenerarAnalisisComparativo(),
                fuenteSeccion, fuenteAnalisis);

            // CONCLUSIONES (una sola página)
            documento.NewPage();
            documento.Add(new Paragraph("CONCLUSIONES Y RECOMENDACIONES", fuenteSeccion) { SpacingAfter = 15 });
            documento.Add(new Paragraph(GenerarConclusionesSimplificadas(), fuenteNormal) { Alignment = Element.ALIGN_JUSTIFIED });

            documento.Close();
        }

        private void AgregarPaginaGraficoConAnalisis(Document documento, Chart chart, string titulo, string analisis,
            iTextSharp.text.Font fuenteTitulo, iTextSharp.text.Font fuenteAnalisis)
        {
            documento.NewPage();

            // Título de la página
            documento.Add(new Paragraph(titulo, fuenteTitulo) { SpacingAfter = 10 });

            // Guardar título original del gráfico y quitarlo temporalmente
            string tituloOriginal = chart.Titles.Count > 0 ? chart.Titles[0].Text : "";
            if (chart.Titles.Count > 0)
                chart.Titles[0].Text = "";

            // Capturar gráfico - tamaño optimizado para que quepa con el análisis
            using (MemoryStream ms = new MemoryStream())
            {
                // Usar tamaño fijo que se vea bien
                int width = 1200;
                int height = 500;

                using (Bitmap bmp = new Bitmap(width, height))
                {
                    bmp.SetResolution(150, 150);
                    chart.DrawToBitmap(bmp, new System.Drawing.Rectangle(0, 0, width, height));
                    bmp.Save(ms, ImageFormat.Png);
                }

                var imagen = iTextSharp.text.Image.GetInstance(ms.ToArray());
                // Escalar para que quepa en la página horizontal con espacio para análisis
                float maxWidth = documento.PageSize.Width - documento.LeftMargin - documento.RightMargin;
                float maxHeight = 320; // Altura fija para dejar espacio al análisis
                imagen.ScaleToFit(maxWidth, maxHeight);
                imagen.Alignment = Element.ALIGN_CENTER;
                documento.Add(imagen);
            }

            // Restaurar título del gráfico
            if (chart.Titles.Count > 0)
                chart.Titles[0].Text = tituloOriginal;

            documento.Add(new Paragraph("\n"));

            // Cuadro de análisis compacto
            PdfPTable tablaAnalisis = new PdfPTable(1) { WidthPercentage = 100 };
            PdfPCell celdaAnalisis = new PdfPCell(new Phrase(analisis, fuenteAnalisis))
            {
                BackgroundColor = new BaseColor(248, 249, 250),
                Padding = 10,
                Border = PdfPCell.BOX,
                BorderColor = new BaseColor(200, 200, 200),
                BorderWidth = 1
            };
            tablaAnalisis.AddCell(celdaAnalisis);
            documento.Add(tablaAnalisis);
        }

        private void AgregarFilaInfo(PdfPTable tabla, string etiqueta, string valor)
        {
            var fuenteEtiqueta = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.DARK_GRAY);
            var fuenteValor = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.BLACK);

            tabla.AddCell(new PdfPCell(new Phrase(etiqueta, fuenteEtiqueta))
            { Border = 0, PaddingBottom = 8, HorizontalAlignment = Element.ALIGN_RIGHT });
            tabla.AddCell(new PdfPCell(new Phrase(valor, fuenteValor))
            { Border = 0, PaddingBottom = 8, PaddingLeft = 10 });
        }

        private void AgregarCeldaKPI(PdfPTable tabla, string etiqueta, string valor, BaseColor color)
        {
            PdfPCell celda = new PdfPCell { BackgroundColor = color, Padding = 15, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER };
            celda.AddElement(new Paragraph(valor, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, BaseColor.WHITE))
            { Alignment = Element.ALIGN_CENTER });
            celda.AddElement(new Paragraph(etiqueta, FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.WHITE))
            { Alignment = Element.ALIGN_CENTER, SpacingBefore = 5 });
            tabla.AddCell(celda);
        }

        private string GenerarResumenEjecutivo()
        {
            int totalRegistros = int.Parse(lblTotalRegistrosValor.Text.Replace(",", "").Replace(".", ""));
            int promedioDiario = int.Parse(lblAccionesPorDiaValor.Text.Replace(",", "").Replace(".", ""));
            int diasPeriodo = Math.Max(1, (int)(fechaHasta - fechaDesde).TotalDays);

            return $"Este informe presenta un análisis integral de la actividad del sistema MOFIS ERP durante el periodo " +
                   $"comprendido entre el {fechaDesde:dd/MM/yyyy} y el {fechaHasta:dd/MM/yyyy} " +
                   $"({diasPeriodo} días). Durante este tiempo, se registraron {totalRegistros:N0} acciones en total, " +
                   $"con un promedio de {promedioDiario:N0} acciones diarias. El usuario con mayor actividad fue " +
                   $"'{lblUsuarioActivoValor.Text}', mientras que el módulo más utilizado fue '{lblModuloMasUsadoValor.Text}'. " +
                   $"El pico de actividad se concentra a las {lblHoraPicoValor.Text}, lo cual refleja los horarios de mayor " +
                   $"operación del negocio.";
        }

        private string GenerarAnalisisTendencia()
        {
            int totalRegistros = int.Parse(lblTotalRegistrosValor.Text.Replace(",", "").Replace(".", ""));
            int promedioDiario = int.Parse(lblAccionesPorDiaValor.Text.Replace(",", "").Replace(".", ""));
            int diasPeriodo = Math.Max(1, (int)(fechaHasta - fechaDesde).TotalDays);
            string tendencia = promedioDiario > 50 ? "alta" : promedioDiario > 20 ? "moderada" : "baja";

            return $"ANÁLISIS DE TENDENCIA TEMPORAL\n\n" +
                   $"- Periodo analizado: {diasPeriodo} días ({fechaDesde:dd/MM/yyyy} - {fechaHasta:dd/MM/yyyy})\n" +
                   $"- Total de acciones registradas: {totalRegistros:N0}\n" +
                   $"- Promedio diario: {promedioDiario:N0} acciones/día\n" +
                   $"- Nivel de actividad: {tendencia.ToUpper()}\n\n" +
                   $"INTERPRETACIÓN: El gráfico muestra la evolución diaria de las acciones del sistema. " +
                   $"Los picos representan días de mayor operación, mientras que los valles pueden indicar " +
                   $"fines de semana, feriados o periodos de menor actividad. Una línea ascendente sugiere " +
                   $"crecimiento en el uso del sistema.";
        }

        private string GenerarAnalisisModulos()
        {
            return $"ANÁLISIS DE USO POR MÓDULO\n\n" +
                   $"- Módulo más utilizado: {lblModuloMasUsadoValor.Text}\n" +
                   $"- Este gráfico revela qué áreas del sistema tienen mayor demanda operativa.\n\n" +
                   $"INTERPRETACIÓN: Los módulos con mayor cantidad de acciones representan las áreas críticas " +
                   $"del negocio que requieren mayor atención en términos de rendimiento, estabilidad y posibles " +
                   $"mejoras. Los módulos con poca actividad podrían necesitar capacitación adicional para los " +
                   $"usuarios o evaluación de su utilidad real para la organización.";
        }

        private string GenerarAnalisisUsuarios()
        {
            return $"ANÁLISIS DE ACTIVIDAD POR USUARIO\n\n" +
                   $"- Usuario más activo: {lblUsuarioActivoValor.Text}\n" +
                   $"- El ranking muestra los 10 usuarios con mayor número de acciones registradas.\n\n" +
                   $"INTERPRETACIÓN: Este gráfico permite identificar a los usuarios clave del sistema. " +
                   $"Una distribución muy desigual (pocos usuarios con mucha actividad) puede indicar:\n" +
                   $"  * Dependencia excesiva de ciertos empleados\n" +
                   $"  * Necesidad de redistribuir cargas de trabajo\n" +
                   $"  * Oportunidad de identificar usuarios expertos para capacitación interna\n" +
                   $"También es útil para detectar patrones inusuales que podrían indicar problemas de seguridad.";
        }

        private string GenerarAnalisisDistribucion()
        {
            return $"ANÁLISIS DE TIPOS DE ACCIÓN\n\n" +
                   $"- Este gráfico muestra la proporción de cada tipo de acción sobre el total.\n" +
                   $"- Las acciones más frecuentes dominan el uso diario del sistema.\n\n" +
                   $"INTERPRETACIÓN: La distribución por tipo de acción revela el comportamiento operativo:\n" +
                   $"  * Alto porcentaje de CONSULTAS: Usuarios buscan información frecuentemente\n" +
                   $"  * Alto porcentaje de CREAR/MODIFICAR: Alta carga transaccional\n" +
                   $"  * Alto porcentaje de LOGIN/LOGOUT: Posibles problemas de sesión\n" +
                   $"  * Alto porcentaje de ELIMINAR: Requiere revisión de procesos";
        }

        private string GenerarAnalisisHeatmap()
        {
            return $"ANÁLISIS DE PATRÓN HORARIO\n\n" +
                   $"- Hora de mayor actividad: {lblHoraPicoValor.Text}\n" +
                   $"- Los colores más intensos (rojo/naranja) indican mayor concentración de acciones.\n\n" +
                   $"INTERPRETACIÓN: El mapa de calor horario permite:\n" +
                   $"  * Identificar las horas de mayor demanda del sistema\n" +
                   $"  * Planificar mantenimientos en horarios de baja actividad\n" +
                   $"  * Dimensionar recursos de infraestructura según la demanda\n" +
                   $"  * Detectar actividad fuera de horario laboral (alertas de seguridad)\n\n" +
                   $"RECOMENDACIÓN: Programar respaldos y actualizaciones durante las horas de menor actividad.";
        }

        private string GenerarAnalisisComparativo()
        {
            int diasPeriodo = Math.Max(1, (int)(fechaHasta - fechaDesde).TotalDays);

            return $"ANÁLISIS COMPARATIVO DE PERIODOS\n\n" +
                   $"- Comparación entre el periodo actual y el periodo anterior de igual duración ({diasPeriodo} días).\n" +
                   $"- Barras azules: Periodo actual | Barras grises: Periodo anterior\n\n" +
                   $"INTERPRETACIÓN: La comparativa permite identificar:\n" +
                   $"  * Crecimiento o decrecimiento en el uso de cada módulo\n" +
                   $"  * Cambios en los patrones de uso del sistema\n" +
                   $"  * Efectos de nuevas implementaciones o capacitaciones\n" +
                   $"  * Tendencias estacionales o cíclicas\n\n" +
                   $"Un aumento significativo puede indicar nuevos procesos o mayor adopción del sistema.";
        }

        private string GenerarConclusionesSimplificadas()
        {
            int totalRegistros = int.Parse(lblTotalRegistrosValor.Text.Replace(",", "").Replace(".", ""));
            int promedioDiario = int.Parse(lblAccionesPorDiaValor.Text.Replace(",", "").Replace(".", ""));
            int diasPeriodo = Math.Max(1, (int)(fechaHasta - fechaDesde).TotalDays);
            string nivelActividad = promedioDiario > 100 ? "MUY ALTO" : promedioDiario > 50 ? "ALTO" :
                                    promedioDiario > 20 ? "MODERADO" : "BAJO";

            return $"1. RESUMEN DE ACTIVIDAD\n" +
                   $"─────────────────────────────────────────────────────────────────────\n" +
                   $"   Periodo de análisis: {fechaDesde:dd/MM/yyyy} al {fechaHasta:dd/MM/yyyy} ({diasPeriodo} días)\n" +
                   $"   Total de transacciones: {totalRegistros:N0}\n" +
                   $"   Promedio diario: {promedioDiario:N0} acciones\n" +
                   $"   Nivel de actividad: {nivelActividad}\n\n" +

                   $"2. HALLAZGOS PRINCIPALES\n" +
                   $"─────────────────────────────────────────────────────────────────────\n" +
                   $"   - Usuario más activo: {lblUsuarioActivoValor.Text}\n" +
                   $"   - Módulo más utilizado: {lblModuloMasUsadoValor.Text}\n" +
                   $"   - Hora pico de actividad: {lblHoraPicoValor.Text}\n\n" +

                   $"─────────────────────────────────────────────────────────────────────\n" +
                   $"Reporte generado por MOFIS ERP - Módulo de Auditoría\n" +
                   $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm:ss} | Usuario: {SesionActual.NombreCompleto}";
        }
    }
}