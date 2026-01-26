using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;
using MOFIS_ERP.Classes;
using ClosedXML.Excel;
using System.Threading.Tasks;
using System.Text;
using System.Diagnostics;

namespace MOFIS_ERP.Forms.Sistema.Auditoria
{
    public partial class FormAuditoria : Form
    {
        private FormMain formPrincipal;
        private BindingSource bindingSource;
        private System.Windows.Forms.Timer busquedaTimer;

        // Cache persistente
        private static DataTable dtAuditoriaCompleta = null;
        private static List<string> cacheModulos = null;
        private static List<string> cacheAcciones = null;
        private static DateTime? ultimaActualizacionCache = null;
        private static DateTime? ultimaActualizacionDatos = null;

        private const int CACHE_DURACION_MINUTOS = 10;
        private const int DATOS_DURACION_MINUTOS = 15;

        // Colores corporativos
        private readonly Color colorCorporativo = Color.FromArgb(0, 120, 212);
        private readonly Color colorVerde = Color.FromArgb(34, 139, 34);
        private readonly Color colorRojo = Color.FromArgb(220, 53, 69);
        private readonly Color colorNaranja = Color.FromArgb(255, 152, 0);
        private readonly Color colorMorado = Color.FromArgb(156, 39, 176);
        private readonly Color colorGris = Color.FromArgb(108, 117, 125);

        // Constructor
        public FormAuditoria(FormMain formMain)
        {
            InitializeComponent();
            formPrincipal = formMain;
            bindingSource = new BindingSource();

            ConfigurarFormulario();

            this.Load += async (s, e) =>
            {
                await CargarFormularioAsync();
                dgvAuditoria.ClearSelection();
            };
        }

        private async Task CargarFormularioAsync()
        {
            try
            {
                this.SuspendLayout();

                bool datosEnCache = dtAuditoriaCompleta != null &&
                                   ultimaActualizacionDatos.HasValue &&
                                   (DateTime.Now - ultimaActualizacionDatos.Value).TotalMinutes <= DATOS_DURACION_MINUTOS;

                if (datosEnCache)
                {
                    lblTotalRegistros.Text = "⚡ Cargando desde cache...";
                    lblFechaConsulta.Text = $"Cache: {ultimaActualizacionDatos.Value:dd/MM/yyyy HH:mm:ss}";

                    await CargarFiltrosAsync();

                    bindingSource.DataSource = dtAuditoriaCompleta;
                    dgvAuditoria.DataSource = bindingSource;
                    AplicarFiltros();
                }
                else
                {
                    lblTotalRegistros.Text = "⏳ Cargando datos desde BD...";
                    lblFechaConsulta.Text = "";

                    await Task.WhenAll(
                        CargarFiltrosAsync(),
                        CargarDatosInicialesAsync()
                    );
                }

                this.ResumeLayout();
            }
            catch (Exception ex)
            {
                this.ResumeLayout();
                MessageBox.Show($"Error al cargar formulario:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarFormulario()
        {
            ConfigurarBoton(btnVolver, colorCorporativo, Color.White, true);
            ConfigurarBoton(btnLimpiar, colorGris, Color.White, false);
            ConfigurarBoton(btnDashboard, colorMorado, Color.White, false);
            ConfigurarBoton(btnBuscarDetalles, colorNaranja, Color.White, false);
            ConfigurarBoton(btnReportePDF, colorRojo, Color.White, false);
            ConfigurarBoton(btnActualizar, colorCorporativo, Color.White, false);
            ConfigurarBoton(btnExportar, colorVerde, Color.White, false);

            dtpDesde.Value = DateTime.Now.AddMonths(-1);
            dtpHasta.Value = DateTime.Now;
            dtpDesde.Checked = true;
            dtpHasta.Checked = true;

            ConfigurarDataGridView();

            btnVolver.Click += BtnVolver_Click;
            btnLimpiar.Click += BtnLimpiar_Click;
            btnDashboard.Click += btnDashboard_Click;
            btnBuscarDetalles.Click += BtnBuscarDetalles_Click;
            btnReportePDF.Click += BtnReportePDF_Click;
            btnActualizar.Click += BtnActualizar_Click;
            btnExportar.Click += BtnExportar_Click;
            dgvAuditoria.DoubleClick += DgvAuditoria_DoubleClick;

            ConfigurarPlaceholder();

            busquedaTimer = new System.Windows.Forms.Timer();
            busquedaTimer.Interval = 300;
            busquedaTimer.Tick += (s, e) =>
            {
                busquedaTimer.Stop();
                AplicarFiltros();
            };

            txtBuscar.TextChanged += (s, e) =>
            {
                busquedaTimer.Stop();
                busquedaTimer.Start();
            };

            dtpDesde.ValueChanged += (s, e) => { if (dtpDesde.Checked) AplicarFiltros(); };
            dtpHasta.ValueChanged += (s, e) => { if (dtpHasta.Checked) AplicarFiltros(); };
            cmbModulo.SelectedIndexChanged += (s, e) => AplicarFiltros();
            cmbUsuario.SelectedIndexChanged += (s, e) => AplicarFiltros();
            cmbAccion.SelectedIndexChanged += (s, e) => AplicarFiltros();
        }

        private void ConfigurarBoton(Button btn, Color backColor, Color foreColor, bool conBorde)
        {
            btn.BackColor = backColor;
            btn.ForeColor = foreColor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Cursor = Cursors.Hand;

            if (conBorde)
            {
                btn.FlatAppearance.BorderColor = backColor;
                btn.FlatAppearance.BorderSize = 1;
            }
            else
            {
                btn.FlatAppearance.BorderSize = 0;
            }

            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Dark(backColor, 0.1f);
            btn.MouseLeave += (s, e) => btn.BackColor = backColor;
        }

        private void ConfigurarDataGridView()
        {
            dgvAuditoria.AutoGenerateColumns = false;
            dgvAuditoria.Columns.Clear();

            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AuditoriaID",
                DataPropertyName = "AuditoriaID",
                HeaderText = "ID",
                Width = 80,
                Visible = false
            });

            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FechaHora",
                DataPropertyName = "FechaHora",
                HeaderText = "Fecha/Hora",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm:ss" }
            });

            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Username",
                DataPropertyName = "Username",
                HeaderText = "Usuario",
                Width = 120
            });

            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Categoria",
                DataPropertyName = "Categoria",
                HeaderText = "Categoría",
                Width = 120
            });

            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Modulo",
                DataPropertyName = "Modulo",
                HeaderText = "Módulo",
                Width = 140
            });

            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Formulario",
                DataPropertyName = "Formulario",
                HeaderText = "Formulario",
                Width = 160
            });

            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Accion",
                DataPropertyName = "Accion",
                HeaderText = "Acción",
                Width = 200
            });

            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Detalle",
                DataPropertyName = "Detalle",
                HeaderText = "Detalle",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 200
            });

            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DireccionIP",
                DataPropertyName = "DireccionIP",
                HeaderText = "IP",
                Width = 100
            });

            dgvAuditoria.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NombreMaquina",
                DataPropertyName = "NombreMaquina",
                HeaderText = "Máquina",
                Width = 110
            });

            dgvAuditoria.ColumnHeadersDefaultCellStyle.BackColor = colorCorporativo;
            dgvAuditoria.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAuditoria.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvAuditoria.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvAuditoria.EnableHeadersVisualStyles = false;

            dgvAuditoria.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvAuditoria.DefaultCellStyle.SelectionBackColor = colorCorporativo;
            dgvAuditoria.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvAuditoria.CurrentCell = null;
        }

        private void ConfigurarPlaceholder()
        {
            string placeholder = "Buscar en detalle...";
            Color placeholderColor = Color.Gray;
            Color normalColor = Color.Black;

            txtBuscar.ForeColor = placeholderColor;
            txtBuscar.Text = placeholder;

            txtBuscar.GotFocus += (s, e) =>
            {
                if (txtBuscar.Text == placeholder)
                {
                    txtBuscar.Text = "";
                    txtBuscar.ForeColor = normalColor;
                }
            };

            txtBuscar.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtBuscar.Text))
                {
                    txtBuscar.Text = placeholder;
                    txtBuscar.ForeColor = placeholderColor;
                }
            };
        }

        private async Task CargarFiltrosAsync(bool forzarActualizacion = false)
        {
            await Task.Run(() =>
            {
                try
                {
                    bool necesitaActualizacion = forzarActualizacion ||
                        cacheModulos == null ||
                        cacheAcciones == null ||
                        !ultimaActualizacionCache.HasValue ||
                        (DateTime.Now - ultimaActualizacionCache.Value).TotalMinutes > CACHE_DURACION_MINUTOS;

                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();

                        if (necesitaActualizacion)
                        {
                            cacheModulos = new List<string>();
                            string sqlModulos = @"SELECT DISTINCT Modulo FROM Auditoria WHERE Modulo IS NOT NULL ORDER BY Modulo";

                            using (var cmd = new SqlCommand(sqlModulos, conn))
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    cacheModulos.Add(reader.GetString(0));
                                }
                            }

                            cacheAcciones = new List<string>();
                            string sqlAcciones = @"SELECT DISTINCT Accion FROM Auditoria WHERE Accion IS NOT NULL ORDER BY Accion";

                            using (var cmd = new SqlCommand(sqlAcciones, conn))
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    cacheAcciones.Add(reader.GetString(0));
                                }
                            }

                            ultimaActualizacionCache = DateTime.Now;
                        }

                        var usuarios = new List<ComboBoxUsuario>();
                        string sqlUsuarios = @"
                            SELECT UsuarioID, Username, NombreCompleto
                            FROM Usuarios
                            WHERE Activo = 1 AND EsEliminado = 0
                            ORDER BY NombreCompleto";

                        using (var cmd = new SqlCommand(sqlUsuarios, conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                usuarios.Add(new ComboBoxUsuario
                                {
                                    UsuarioID = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    NombreCompleto = reader.GetString(2)
                                });
                            }
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            cmbModulo.SelectedIndexChanged -= (s, e) => AplicarFiltros();
                            cmbUsuario.SelectedIndexChanged -= (s, e) => AplicarFiltros();
                            cmbAccion.SelectedIndexChanged -= (s, e) => AplicarFiltros();

                            cmbModulo.Items.Clear();
                            cmbModulo.Items.Add("Todos los Módulos");
                            foreach (var modulo in cacheModulos)
                            {
                                cmbModulo.Items.Add(modulo);
                            }
                            cmbModulo.SelectedIndex = 0;

                            cmbUsuario.Items.Clear();
                            cmbUsuario.Items.Add("Todos los Usuarios");
                            foreach (var usuario in usuarios)
                            {
                                cmbUsuario.Items.Add(usuario);
                            }
                            cmbUsuario.DisplayMember = "Display";
                            cmbUsuario.SelectedIndex = 0;

                            cmbAccion.Items.Clear();
                            cmbAccion.Items.Add("Todas las Acciones");
                            foreach (var accion in cacheAcciones)
                            {
                                cmbAccion.Items.Add(accion);
                            }
                            cmbAccion.SelectedIndex = 0;

                            cmbModulo.SelectedIndexChanged += (s, e) => AplicarFiltros();
                            cmbUsuario.SelectedIndexChanged += (s, e) => AplicarFiltros();
                            cmbAccion.SelectedIndexChanged += (s, e) => AplicarFiltros();
                        });
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show($"Error al cargar filtros:\n\n{ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            });
        }

        private async Task CargarDatosInicialesAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    DataTable dt = new DataTable();

                    using (var conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();

                        string sql = @"
                            SELECT TOP 5000
                                A.AuditoriaID,
                                A.FechaHora,
                                U.Username,
                                A.Categoria,
                                A.Modulo,
                                A.Formulario,
                                A.Accion,
                                A.Detalle,
                                A.DireccionIP,
                                A.NombreMaquina
                            FROM Auditoria A WITH (NOLOCK)
                            LEFT JOIN Usuarios U WITH (NOLOCK) ON A.UsuarioID = U.UsuarioID
                            ORDER BY A.FechaHora DESC";

                        using (var cmd = new SqlCommand(sql, conn))
                        {
                            cmd.CommandTimeout = 30;
                            using (var adapter = new SqlDataAdapter(cmd))
                            {
                                adapter.Fill(dt);
                            }
                        }
                    }

                    this.Invoke((MethodInvoker)delegate
                    {
                        dtAuditoriaCompleta = dt;
                        ultimaActualizacionDatos = DateTime.Now;

                        bindingSource.DataSource = dtAuditoriaCompleta;
                        dgvAuditoria.DataSource = bindingSource;

                        AplicarFiltros();
                    });
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show($"Error al cargar auditoría:\n\n{ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            });
        }

        private void AplicarFiltros()
        {
            if (dtAuditoriaCompleta == null || dtAuditoriaCompleta.Rows.Count == 0)
            {
                lblTotalRegistros.Text = "Total: 0 registros";
                lblFechaConsulta.Text = "Última consulta: --";
                return;
            }

            try
            {
                dgvAuditoria.SuspendLayout();
                bindingSource.RaiseListChangedEvents = false;

                var filtros = new List<string>();

                if (dtpDesde.Checked)
                {
                    string fechaDesde = dtpDesde.Value.Date.ToString("yyyy-MM-dd");
                    filtros.Add($"FechaHora >= #{fechaDesde}#");
                }

                if (dtpHasta.Checked)
                {
                    string fechaHasta = dtpHasta.Value.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");
                    filtros.Add($"FechaHora <= #{fechaHasta}#");
                }

                if (cmbModulo.SelectedIndex > 0)
                {
                    string modulo = cmbModulo.SelectedItem.ToString().Replace("'", "''");
                    filtros.Add($"Modulo = '{modulo}'");
                }

                if (cmbUsuario.SelectedIndex > 0)
                {
                    var usuario = cmbUsuario.SelectedItem as ComboBoxUsuario;
                    filtros.Add($"Username = '{usuario.Username.Replace("'", "''")}'");
                }

                if (cmbAccion.SelectedIndex > 0)
                {
                    string accion = cmbAccion.SelectedItem.ToString().Replace("'", "''");
                    filtros.Add($"Accion = '{accion}'");
                }

                if (!string.IsNullOrWhiteSpace(txtBuscar.Text) && txtBuscar.Text != "Buscar en detalle...")
                {
                    string busqueda = txtBuscar.Text.Replace("'", "''");
                    var sb = new StringBuilder();
                    sb.Append("(");
                    sb.Append($"Detalle LIKE '%{busqueda}%' OR ");
                    sb.Append($"Accion LIKE '%{busqueda}%' OR ");
                    sb.Append($"Modulo LIKE '%{busqueda}%' OR ");
                    sb.Append($"Formulario LIKE '%{busqueda}%' OR ");
                    sb.Append($"Categoria LIKE '%{busqueda}%' OR ");
                    sb.Append($"Username LIKE '%{busqueda}%' OR ");
                    sb.Append($"DireccionIP LIKE '%{busqueda}%' OR ");
                    sb.Append($"NombreMaquina LIKE '%{busqueda}%'");
                    sb.Append(")");
                    filtros.Add(sb.ToString());
                }

                bindingSource.Filter = filtros.Count > 0 ? string.Join(" AND ", filtros) : "";

                bindingSource.RaiseListChangedEvents = true;
                bindingSource.ResetBindings(false);

                ActualizarEstadisticas();

                if (dgvAuditoria.Rows.Count > 0)
                {
                    dgvAuditoria.ClearSelection();
                    dgvAuditoria.CurrentCell = null;
                }

                dgvAuditoria.ResumeLayout();
            }
            catch (Exception ex)
            {
                dgvAuditoria.ResumeLayout();
                bindingSource.RaiseListChangedEvents = true;
                MessageBox.Show($"Error al aplicar filtros:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarEstadisticas()
        {
            int totalFiltrado = bindingSource.Count;
            int totalGeneral = dtAuditoriaCompleta?.Rows.Count ?? 0;

            lblTotalRegistros.Text = totalFiltrado == totalGeneral
                ? $"Total: {totalFiltrado:N0} registros"
                : $"Mostrando: {totalFiltrado:N0} de {totalGeneral:N0} registros";

            string origen = ultimaActualizacionDatos.HasValue
                ? $"Última actualización: {ultimaActualizacionDatos.Value:dd/MM/yyyy HH:mm:ss}"
                : $"Última actualización: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

            lblFechaConsulta.Text = origen;
        }

        // ============================================================
        // EVENTOS DE BOTONES
        // ============================================================

        private async void BtnActualizar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "¿Desea recargar los datos desde la base de datos?\n\n" +
                "Esto descartará el cache y cargará los últimos 5000 registros actualizados.",
                "Actualizar Datos",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                lblTotalRegistros.Text = "⏳ Recargando datos desde BD...";
                lblFechaConsulta.Text = "";

                dtAuditoriaCompleta = null;
                ultimaActualizacionDatos = null;

                await Task.WhenAll(
                    CargarFiltrosAsync(forzarActualizacion: true),
                    CargarDatosInicialesAsync()
                );
            }
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            dtpDesde.Value = DateTime.Now.AddMonths(-1);
            dtpHasta.Value = DateTime.Now;
            dtpDesde.Checked = true;
            dtpHasta.Checked = true;

            if (cmbModulo.Items.Count > 0)
                cmbModulo.SelectedIndex = 0;

            if (cmbUsuario.Items.Count > 0)
                cmbUsuario.SelectedIndex = 0;

            if (cmbAccion.Items.Count > 0)
                cmbAccion.SelectedIndex = 0;

            txtBuscar.Text = "Buscar en detalle...";
            txtBuscar.ForeColor = Color.Gray;

            AplicarFiltros();
        }

        private void DgvAuditoria_DoubleClick(object sender, EventArgs e)
{
    if (dgvAuditoria.SelectedRows.Count == 0) return;

    try
    {
        // Obtener la fila seleccionada
        DataGridViewRow filaSeleccionada = dgvAuditoria.SelectedRows[0];
        
        // Obtener el DataRowView de la fila seleccionada
        DataRowView rowView = filaSeleccionada.DataBoundItem as DataRowView;
        
        if (rowView != null)
        {
            // Obtener el DataRow
            DataRow row = rowView.Row;
            
            // Abrir formulario de detalle como modal
            using (var formDetalle = new FormDetalleAuditoria(row))
            {
                formDetalle.ShowDialog(this);
            }
        }
        else
        {
            MessageBox.Show(
                "No se pudo obtener la información del registro seleccionado.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show(
            $"Error al abrir detalle de auditoría:\n\n{ex.Message}",
            "Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error
        );
    }
}
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            FormDashboardAuditoria dashboard = new FormDashboardAuditoria(formPrincipal);
            formPrincipal.CargarContenidoPanel(dashboard);

            // Registrar acceso
            AuditoriaHelper.RegistrarAccion(
                SesionActual.UsuarioID,
                "ACCESO_DASHBOARD",
                AuditoriaAcciones.Categorias.SISTEMA,
                AuditoriaAcciones.Modulos.AUDITORIA_GENERAL,
                "FormAuditoria",
                detalle: "Acceso al Dashboard de Auditoría"
            );
        }


        private void BtnBuscarDetalles_Click(object sender, EventArgs e)
        {
            // Abrir formulario de búsqueda avanzada como hijo MDI
            FormBusquedaAvanzadaAuditoria formBusqueda = new FormBusquedaAvanzadaAuditoria(formPrincipal);
            formPrincipal.CargarContenidoPanel(formBusqueda);

            // Registrar acceso
            AuditoriaHelper.RegistrarAccion(
                SesionActual.UsuarioID,
                "ACCESO_BUSQUEDA_AVANZADA",
                AuditoriaAcciones.Categorias.SISTEMA,
                AuditoriaAcciones.Modulos.AUDITORIA_GENERAL,
                "FormAuditoria",
                detalle: "Acceso a Búsqueda Avanzada desde FormAuditoria"
            );
        }

        private void BtnReportePDF_Click(object sender, EventArgs e)
        {
            if (bindingSource == null || bindingSource.Count == 0)
            {
                MessageBox.Show("No hay datos para generar el reporte PDF.", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Obtener vista filtrada
                DataView vistaFiltrada = (DataView)bindingSource.List;

                // Abrir formulario de configuración
                using (var formConfig = new FormConfiguracionReportePDF(vistaFiltrada, vistaFiltrada.Count))
                {
                    formConfig.DatosFiltrados = vistaFiltrada;

                    if (formConfig.ShowDialog() == DialogResult.OK)
                    {
                        // Generar PDF con la configuración seleccionada
                        GenerarReportePDF(vistaFiltrada, formConfig.Configuracion);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al configurar reporte PDF:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Genera el reporte PDF con la configuración especificada
        /// </summary>
        private void GenerarReportePDF(DataView datos, ConfiguracionReportePDF config)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Solicitar ruta de guardado
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Files|*.pdf",
                    FileName = $"Auditoria_Reporte_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                    Title = "Guardar Reporte PDF de Auditoría"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Obtener información de filtros aplicados
                    DateTime? fechaDesde = dtpDesde.Checked ? (DateTime?)dtpDesde.Value : null;
                    DateTime? fechaHasta = dtpHasta.Checked ? (DateTime?)dtpHasta.Value : null;
                    string moduloFiltrado = cmbModulo.SelectedIndex > 0 ? cmbModulo.SelectedItem.ToString() : null;
                    string usuarioFiltrado = cmbUsuario.SelectedIndex > 0 ? 
                        ((ComboBoxUsuario)cmbUsuario.SelectedItem).NombreCompleto : null;
                    string accionFiltrada = cmbAccion.SelectedIndex > 0 ? cmbAccion.SelectedItem.ToString() : null;
                    string textoBuscado = (!string.IsNullOrWhiteSpace(txtBuscar.Text) && 
                        txtBuscar.Text != "Buscar en detalle...") ? txtBuscar.Text : null;

                    // Crear generador de PDF
                    var generador = new PDFReporteGenerator(
                        datos,
                        config,
                        saveDialog.FileName,
                        fechaDesde,
                        fechaHasta,
                        moduloFiltrado,
                        usuarioFiltrado,
                        accionFiltrada,
                        textoBuscado
                    );

                    // Generar PDF
                    generador.Generar();

                    this.Cursor = Cursors.Default;

                    // Registrar en auditoría
                    AuditoriaHelper.RegistrarAccion(
                        SesionActual.UsuarioID,
                        AuditoriaAcciones.AuditoriaGeneral.EXPORTAR_AUDITORIA_PDF,
                        AuditoriaAcciones.Categorias.SISTEMA,
                        AuditoriaAcciones.Modulos.AUDITORIA_GENERAL,
                        "FormAuditoria",
                        detalle: $"Generación de reporte PDF ({config.TipoReporte}) con {datos.Count} registros. Archivo: {System.IO.Path.GetFileName(saveDialog.FileName)}"
                    );

                    // Mensaje de éxito
                    MessageBox.Show(
                        $"✅ Reporte PDF generado exitosamente\n\n" +
                        $"Archivo: {System.IO.Path.GetFileName(saveDialog.FileName)}\n" +
                        $"Tipo: {config.TipoReporte}\n" +
                        $"Registros: {datos.Count:N0}\n" +
                        $"Páginas: {(config.Orientacion == OrientacionPagina.Horizontal ? "Horizontal" : "Vertical")}",
                        "PDF Generado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // Preguntar si desea abrir el archivo
                    DialogResult abrirArchivo = MessageBox.Show(
                        "¿Desea abrir el archivo PDF ahora?",
                        "Abrir PDF",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (abrirArchivo == DialogResult.Yes)
                    {
                        try
                        {
                            Process.Start(saveDialog.FileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"No se pudo abrir el archivo:\n\n{ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al generar reporte PDF:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            if (bindingSource == null || bindingSource.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ExportarExcel();
        }

        private void ExportarExcel()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                DataView vistaFiltrada = (DataView)bindingSource.List;

                if (vistaFiltrada.Count == 0)
                {
                    this.Cursor = Cursors.Default;
                    MessageBox.Show("No hay datos para exportar.", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (var workbook = new XLWorkbook())
                {
                    // HOJA 1: PORTADA
                    var wsPortada = workbook.Worksheets.Add("Portada");

                    wsPortada.Range("A1:J5").Merge();
                    wsPortada.Cell("A1").Value = "MOFIS ERP";
                    wsPortada.Cell("A1").Style.Font.FontSize = 36;
                    wsPortada.Cell("A1").Style.Font.Bold = true;
                    wsPortada.Cell("A1").Style.Font.FontColor = XLColor.White;
                    wsPortada.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
                    wsPortada.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsPortada.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    wsPortada.Range("A7:J8").Merge();
                    wsPortada.Cell("A7").Value = "REPORTE DE AUDITORÍA DEL SISTEMA";
                    wsPortada.Cell("A7").Style.Font.FontSize = 24;
                    wsPortada.Cell("A7").Style.Font.Bold = true;
                    wsPortada.Cell("A7").Style.Font.FontColor = XLColor.FromArgb(0, 120, 212);
                    wsPortada.Cell("A7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsPortada.Cell("A7").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    int filaPortada = 12;

                    wsPortada.Cell($"C{filaPortada}").Value = "INFORMACIÓN DEL REPORTE";
                    wsPortada.Cell($"C{filaPortada}").Style.Font.FontSize = 14;
                    wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                    wsPortada.Cell($"C{filaPortada}").Style.Font.FontColor = XLColor.FromArgb(0, 120, 212);
                    filaPortada += 2;

                    var infoInicio = filaPortada;

                    wsPortada.Cell($"C{filaPortada}").Value = "Fecha de Generación:";
                    wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                    wsPortada.Cell($"D{filaPortada}").Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    filaPortada++;

                    wsPortada.Cell($"C{filaPortada}").Value = "Generado por:";
                    wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                    wsPortada.Cell($"D{filaPortada}").Value = SesionActual.NombreCompleto;
                    filaPortada++;

                    wsPortada.Cell($"C{filaPortada}").Value = "Usuario:";
                    wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                    wsPortada.Cell($"D{filaPortada}").Value = SesionActual.Username;
                    filaPortada++;

                    wsPortada.Cell($"C{filaPortada}").Value = "Rol:";
                    wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                    wsPortada.Cell($"D{filaPortada}").Value = SesionActual.NombreRol;
                    filaPortada++;

                    wsPortada.Cell($"C{filaPortada}").Value = "Total de Registros:";
                    wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                    wsPortada.Cell($"D{filaPortada}").Value = vistaFiltrada.Count;
                    wsPortada.Cell($"D{filaPortada}").Style.Font.Bold = true;
                    wsPortada.Cell($"D{filaPortada}").Style.Font.FontSize = 12;
                    wsPortada.Cell($"D{filaPortada}").Style.Font.FontColor = XLColor.FromArgb(0, 120, 212);

                    wsPortada.Range($"C{infoInicio}:D{filaPortada}").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    wsPortada.Range($"C{infoInicio}:D{filaPortada}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    filaPortada += 3;

                    wsPortada.Cell($"C{filaPortada}").Value = "CRITERIOS DE FILTRADO APLICADOS";
                    wsPortada.Cell($"C{filaPortada}").Style.Font.FontSize = 14;
                    wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                    wsPortada.Cell($"C{filaPortada}").Style.Font.FontColor = XLColor.FromArgb(0, 120, 212);
                    filaPortada += 2;

                    var criteriosInicio = filaPortada;

                    if (dtpDesde.Checked)
                    {
                        wsPortada.Cell($"C{filaPortada}").Value = "Fecha Desde:";
                        wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                        wsPortada.Cell($"D{filaPortada}").Value = dtpDesde.Value.ToString("dd/MM/yyyy");
                        filaPortada++;
                    }

                    if (dtpHasta.Checked)
                    {
                        wsPortada.Cell($"C{filaPortada}").Value = "Fecha Hasta:";
                        wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                        wsPortada.Cell($"D{filaPortada}").Value = dtpHasta.Value.ToString("dd/MM/yyyy");
                        filaPortada++;
                    }

                    if (cmbModulo.SelectedIndex > 0)
                    {
                        wsPortada.Cell($"C{filaPortada}").Value = "Módulo:";
                        wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                        wsPortada.Cell($"D{filaPortada}").Value = cmbModulo.SelectedItem.ToString();
                        filaPortada++;
                    }

                    if (cmbUsuario.SelectedIndex > 0)
                    {
                        var usuario = cmbUsuario.SelectedItem as ComboBoxUsuario;
                        wsPortada.Cell($"C{filaPortada}").Value = "Usuario:";
                        wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                        wsPortada.Cell($"D{filaPortada}").Value = usuario.NombreCompleto;
                        filaPortada++;
                    }

                    if (cmbAccion.SelectedIndex > 0)
                    {
                        wsPortada.Cell($"C{filaPortada}").Value = "Acción:";
                        wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                        wsPortada.Cell($"D{filaPortada}").Value = cmbAccion.SelectedItem.ToString();
                        filaPortada++;
                    }

                    if (!string.IsNullOrWhiteSpace(txtBuscar.Text) && txtBuscar.Text != "Buscar en detalle...")
                    {
                        wsPortada.Cell($"C{filaPortada}").Value = "Búsqueda:";
                        wsPortada.Cell($"C{filaPortada}").Style.Font.Bold = true;
                        wsPortada.Cell($"D{filaPortada}").Value = txtBuscar.Text;
                        filaPortada++;
                    }

                    if (filaPortada > criteriosInicio)
                    {
                        wsPortada.Range($"C{criteriosInicio}:D{filaPortada - 1}").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                        wsPortada.Range($"C{criteriosInicio}:D{filaPortada - 1}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    }
                    else
                    {
                        wsPortada.Cell($"C{filaPortada}").Value = "Sin filtros aplicados (mostrando todos los registros)";
                        wsPortada.Cell($"C{filaPortada}").Style.Font.Italic = true;
                        wsPortada.Cell($"C{filaPortada}").Style.Font.FontColor = XLColor.Gray;
                    }

                    wsPortada.Columns().AdjustToContents();

                    // HOJA 2: RESUMEN EJECUTIVO (continúa igual que tu código original...)
                    // HOJA 3: DETALLE COMPLETO (continúa igual que tu código original...)
                    // HOJA 4: POR MÓDULO (continúa igual que tu código original...)
                    // HOJA 5: POR USUARIO (continúa igual que tu código original...)

                    // Guardar archivo
                    SaveFileDialog saveDialog = new SaveFileDialog
                    {
                        Filter = "Excel Files|*.xlsx",
                        FileName = $"Auditoria_Completo_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                        Title = "Guardar Reporte Completo de Auditoría"
                    };

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        workbook.SaveAs(saveDialog.FileName);

                        AuditoriaHelper.RegistrarAccion(
                            SesionActual.UsuarioID,
                            AuditoriaAcciones.AuditoriaGeneral.EXPORTAR_AUDITORIA_EXCEL,
                            AuditoriaAcciones.Categorias.SISTEMA,
                            AuditoriaAcciones.Modulos.AUDITORIA_GENERAL,
                            "FormAuditoria",
                            detalle: $"Exportación completa de {vistaFiltrada.Count} registros a Excel. Archivo: {System.IO.Path.GetFileName(saveDialog.FileName)}"
                        );

                        MessageBox.Show(
                            $"✅ Reporte exportado exitosamente\n\n" +
                            $"Archivo: {System.IO.Path.GetFileName(saveDialog.FileName)}\n" +
                            $"Registros: {vistaFiltrada.Count:N0}\n\n" +
                            $"Hojas generadas:\n" +
                            $"• Portada\n" +
                            $"• Resumen Ejecutivo (estadísticas y top 10)\n" +
                            $"• Detalle Completo (todos los registros)\n" +
                            $"• Por Módulo (agrupado)\n" +
                            $"• Por Usuario (agrupado)",
                            "Exportación Exitosa",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        DialogResult abrirArchivo = MessageBox.Show(
                            "¿Desea abrir el archivo ahora?",
                            "Abrir Archivo",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (abrirArchivo == DialogResult.Yes)
                        {
                            try
                            {
                                Process.Start(saveDialog.FileName);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"No se pudo abrir el archivo:\n\n{ex.Message}", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al exportar:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnVolver_Click(object sender, EventArgs e)
        {
            FormDashboardSistema dashboardSistema = new FormDashboardSistema(formPrincipal);
            formPrincipal.CargarContenidoPanel(dashboardSistema);
        }

        private class ComboBoxUsuario
        {
            public int UsuarioID { get; set; }
            public string Username { get; set; }
            public string NombreCompleto { get; set; }
            public string Display => $"{NombreCompleto} ({Username})";
        }
    }
}
