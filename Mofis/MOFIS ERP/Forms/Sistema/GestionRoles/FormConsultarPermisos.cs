using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using MOFIS_ERP.Classes;
using ClosedXML.Excel;

namespace MOFIS_ERP.Forms.Sistema.GestionRoles
{
    public partial class FormConsultarPermisos : Form
    {
        private FormMain formPrincipal;
        private DataTable dtPermisos;
        private bool modoUsuario = false;
        private int entidadSeleccionadaID = 0; // RolID o UsuarioID según el modo
        private bool cargandoDatos = false;

        // Placeholder
        private const string PLACEHOLDER_BUSCAR = "Buscar formulario...";
        private bool isPlaceholder = true;

        public FormConsultarPermisos(FormMain formMain)
        {
            InitializeComponent();
            formPrincipal = formMain;
            ConfigurarFormulario();
            ConfigurarEventos();
            CargarCombos();
        }

        private void ConfigurarFormulario()
        {
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            // Configurar DataGridView como ReadOnly
            ConfigurarDataGridView();

            // Configurar estilos de botones
            ConfigurarEstiloBoton(btnVolver, Color.White, Color.FromArgb(0, 120, 212), true);
            ConfigurarEstiloBoton(btnExportar, Color.FromArgb(34, 139, 34), Color.White, false);
            ConfigurarEstiloBoton(btnLimpiarFiltros, Color.FromArgb(108, 117, 125), Color.White, false);

            // Configurar placeholder del buscador
            ConfigurarPlaceholder();

            // Radio buttons
            rbPorRol.Checked = true;
            modoUsuario = false;
        }

        private void ConfigurarEstiloBoton(Button btn, Color backColor, Color foreColor, bool esBorde)
        {
            if (btn == null) return;

            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = backColor;
            btn.ForeColor = foreColor;
            btn.Cursor = Cursors.Hand;

            if (esBorde)
            {
                btn.FlatAppearance.BorderColor = foreColor;
                btn.FlatAppearance.BorderSize = 2;
            }
            else
            {
                btn.FlatAppearance.BorderSize = 0;
            }

            // Hover effect
            btn.MouseEnter += (s, e) =>
            {
                if (esBorde)
                {
                    btn.BackColor = Color.FromArgb(0, 120, 212);
                    btn.ForeColor = Color.White;
                }
                else
                {
                    btn.BackColor = ControlPaint.Light(backColor, 0.1f);
                }
            };

            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = backColor;
                btn.ForeColor = foreColor;
            };
        }

        private void ConfigurarPlaceholder()
        {
            txtBuscar.Text = PLACEHOLDER_BUSCAR;
            txtBuscar.ForeColor = Color.Gray;
            isPlaceholder = true;

            txtBuscar.Enter += (s, e) =>
            {
                if (isPlaceholder)
                {
                    txtBuscar.Text = "";
                    txtBuscar.ForeColor = Color.Black;
                    isPlaceholder = false;
                }
            };

            txtBuscar.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtBuscar.Text))
                {
                    txtBuscar.Text = PLACEHOLDER_BUSCAR;
                    txtBuscar.ForeColor = Color.Gray;
                    isPlaceholder = true;
                }
            };
        }

        private void ConfigurarEventos()
        {
            rbPorRol.CheckedChanged += RbTipoVista_CheckedChanged;
            rbPorUsuario.CheckedChanged += RbTipoVista_CheckedChanged;
            cmbEntidad.SelectedIndexChanged += CmbEntidad_SelectedIndexChanged;
            cmbCategoria.SelectedIndexChanged += Filtros_Changed;
            cmbModulo.SelectedIndexChanged += Filtros_Changed;
            txtBuscar.TextChanged += TxtBuscar_TextChanged;
            btnVolver.Click += BtnVolver_Click;
            btnExportar.Click += BtnExportar_Click;
            btnLimpiarFiltros.Click += BtnLimpiarFiltros_Click;
        }

        private void ConfigurarDataGridView()
        {
            dgvPermisos.Columns.Clear();
            dgvPermisos.AutoGenerateColumns = false;
            dgvPermisos.AllowUserToAddRows = false;
            dgvPermisos.AllowUserToDeleteRows = false;
            dgvPermisos.ReadOnly = true; // ← CLAVE: Solo lectura
            dgvPermisos.RowHeadersVisible = false;
            dgvPermisos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPermisos.BorderStyle = BorderStyle.FixedSingle;
            dgvPermisos.BackgroundColor = Color.White;
            dgvPermisos.GridColor = Color.FromArgb(230, 230, 230);
            dgvPermisos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Deshabilitar redimensionamiento
            dgvPermisos.AllowUserToResizeRows = false;
            dgvPermisos.AllowUserToResizeColumns = false;

            // Estilo de encabezados
            dgvPermisos.EnableHeadersVisualStyles = false;
            dgvPermisos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 212);
            dgvPermisos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPermisos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            dgvPermisos.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPermisos.ColumnHeadersHeight = 38;

            // Estilo de filas
            dgvPermisos.RowTemplate.Height = 45;
            dgvPermisos.DefaultCellStyle.Font = new Font("Segoe UI", 11);
            dgvPermisos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            // Columnas de información
            dgvPermisos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Categoria",
                HeaderText = "Categoría",
                FillWeight = 15,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleLeft }
            });

            dgvPermisos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Modulo",
                HeaderText = "Módulo",
                FillWeight = 18,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleLeft }
            });

            dgvPermisos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Formulario",
                HeaderText = "Formulario",
                FillWeight = 20,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleLeft }
            });

            // Columnas de acciones (checkboxes - READONLY)
            var acciones = new[]
            {
                new { Codigo = "VIEW", Nombre = "Ver" },
                new { Codigo = "CREATE", Nombre = "Crear" },
                new { Codigo = "EDIT", Nombre = "Editar" },
                new { Codigo = "DELETE", Nombre = "Eliminar" },
                new { Codigo = "PRINT", Nombre = "Imprimir" },
                new { Codigo = "REPRINT", Nombre = "Reimp." },
                new { Codigo = "EXPORT", Nombre = "Exportar" },
                new { Codigo = "RESET", Nombre = "Reset" },
                new { Codigo = "ACTIVATE", Nombre = "Activar" }
            };

            foreach (var accion in acciones)
            {
                var col = new DataGridViewCheckBoxColumn
                {
                    Name = accion.Codigo,
                    HeaderText = accion.Nombre,
                    FillWeight = 5,
                    ReadOnly = true // ← CLAVE: Solo lectura
                };
                dgvPermisos.Columns.Add(col);
            }
        }

        private void CargarCombos()
        {
            try
            {
                cargandoDatos = true;

                // Cargar categorías
                cmbCategoria.Items.Clear();
                cmbCategoria.Items.Add("Todas las Categorías");

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Categorías
                    string sqlCat = "SELECT CategoriaID, NombreCategoria FROM CatalogoCategorias WHERE Activo = 1 ORDER BY OrdenVisualizacion";
                    using (var cmd = new SqlCommand(sqlCat, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbCategoria.Items.Add(new ComboBoxItem
                            {
                                Value = reader.GetInt32(0),
                                Text = reader.GetString(1)
                            });
                        }
                    }
                }

                cmbCategoria.DisplayMember = "Text";
                cmbCategoria.ValueMember = "Value";
                cmbCategoria.SelectedIndex = 0;

                // Cargar módulos (todos)
                CargarModulos();

                // Cargar roles o usuarios según el modo
                CargarEntidades();

                cargandoDatos = false;
            }
            catch (Exception ex)
            {
                cargandoDatos = false;
                MessageBox.Show($"Error al cargar combos:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarModulos(int? categoriaID = null)
        {
            try
            {
                cmbModulo.Items.Clear();
                cmbModulo.Items.Add("Todos los Módulos");

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = "SELECT ModuloID, NombreModulo FROM CatalogoModulos WHERE Activo = 1";
                    if (categoriaID.HasValue)
                        sql += " AND CategoriaID = @CategoriaID";
                    sql += " ORDER BY OrdenVisualizacion";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        if (categoriaID.HasValue)
                            cmd.Parameters.AddWithValue("@CategoriaID", categoriaID.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cmbModulo.Items.Add(new ComboBoxItem
                                {
                                    Value = reader.GetInt32(0),
                                    Text = reader.GetString(1)
                                });
                            }
                        }
                    }
                }

                cmbModulo.DisplayMember = "Text";
                cmbModulo.ValueMember = "Value";
                cmbModulo.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar módulos:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarEntidades()
        {
            try
            {
                cargandoDatos = true;

                cmbEntidad.Items.Clear();

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    if (!modoUsuario)
                    {
                        // Cargar roles
                        string sql = "SELECT RolID, NombreRol, Descripcion FROM Roles WHERE Activo = 1 ORDER BY EsSistema DESC, NombreRol";
                        using (var cmd = new SqlCommand(sql, conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cmbEntidad.Items.Add(new ComboBoxRol
                                {
                                    RolID = reader.GetInt32(0),
                                    NombreRol = reader.GetString(1),
                                    Descripcion = reader.IsDBNull(2) ? "" : reader.GetString(2)
                                });
                            }
                        }
                    }
                    else
                    {
                        // Cargar usuarios
                        string sql = @"
                            SELECT U.UsuarioID, U.Username, U.NombreCompleto, R.NombreRol
                            FROM Usuarios U
                            INNER JOIN Roles R ON U.RolID = R.RolID
                            WHERE U.Activo = 1 AND U.EsEliminado = 0
                            ORDER BY U.NombreCompleto";

                        using (var cmd = new SqlCommand(sql, conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cmbEntidad.Items.Add(new ComboBoxUsuario
                                {
                                    UsuarioID = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    NombreCompleto = reader.GetString(2),
                                    NombreRol = reader.GetString(3)
                                });
                            }
                        }
                    }
                }

                cmbEntidad.DisplayMember = "Display";
                cmbEntidad.ValueMember = modoUsuario ? "UsuarioID" : "RolID";

                if (cmbEntidad.Items.Count > 0)
                    cmbEntidad.SelectedIndex = 0;

                cargandoDatos = false;
            }
            catch (Exception ex)
            {
                cargandoDatos = false;
                MessageBox.Show($"Error al cargar entidades:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarPermisos()
        {
            if (entidadSeleccionadaID == 0)
            {
                dgvPermisos.Rows.Clear();
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = "";

                    if (!modoUsuario)
                    {
                        // Permisos por ROL
                        sql = @"
                            SELECT
                                C.CategoriaID,
                                C.NombreCategoria,
                                M.ModuloID,
                                M.NombreModulo,
                                F.FormularioID,
                                F.CodigoFormulario,
                                F.NombreFormulario,
                                A.AccionID,
                                A.CodigoAccion,
                                A.NombreAccion,
                                ISNULL(PR.Permitido, 0) AS Permitido,
                                0 AS EsExcepcion
                            FROM CatalogoFormularios F
                            INNER JOIN CatalogoModulos M ON F.ModuloID = M.ModuloID
                            INNER JOIN CatalogoCategorias C ON M.CategoriaID = C.CategoriaID
                            CROSS JOIN CatalogoAcciones A
                            LEFT JOIN PermisosRol PR ON PR.FormularioID = F.FormularioID
                                AND PR.AccionID = A.AccionID
                                AND PR.RolID = @EntidadID
                            WHERE F.Activo = 1 AND A.Activo = 1
                                AND A.CodigoAccion IN ('VIEW', 'CREATE', 'EDIT', 'DELETE', 'PRINT', 'REPRINT', 'EXPORT', 'RESET', 'ACTIVATE')
                            ORDER BY C.OrdenVisualizacion, M.OrdenVisualizacion, F.OrdenVisualizacion, A.OrdenVisualizacion";
                    }
                    else
                    {
                        // Permisos por USUARIO (heredados + excepciones)
                        sql = @"
                            SELECT
                                C.CategoriaID,
                                C.NombreCategoria,
                                M.ModuloID,
                                M.NombreModulo,
                                F.FormularioID,
                                F.CodigoFormulario,
                                F.NombreFormulario,
                                A.AccionID,
                                A.CodigoAccion,
                                A.NombreAccion,
                                COALESCE(PU.Permitido, PR.Permitido, 0) AS Permitido,
                                CASE WHEN PU.PermisoUsuarioID IS NOT NULL THEN 1 ELSE 0 END AS EsExcepcion
                            FROM CatalogoFormularios F
                            INNER JOIN CatalogoModulos M ON F.ModuloID = M.ModuloID
                            INNER JOIN CatalogoCategorias C ON M.CategoriaID = C.CategoriaID
                            CROSS JOIN CatalogoAcciones A
                            LEFT JOIN Usuarios U ON U.UsuarioID = @EntidadID
                            LEFT JOIN PermisosRol PR ON PR.FormularioID = F.FormularioID
                                AND PR.AccionID = A.AccionID
                                AND PR.RolID = U.RolID
                            LEFT JOIN PermisosUsuario PU ON PU.FormularioID = F.FormularioID
                                AND PU.AccionID = A.AccionID
                                AND PU.UsuarioID = @EntidadID
                            WHERE F.Activo = 1 AND A.Activo = 1
                                AND A.CodigoAccion IN ('VIEW', 'CREATE', 'EDIT', 'DELETE', 'PRINT', 'REPRINT', 'EXPORT', 'RESET', 'ACTIVATE')
                            ORDER BY C.OrdenVisualizacion, M.OrdenVisualizacion, F.OrdenVisualizacion, A.OrdenVisualizacion";
                    }

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@EntidadID", entidadSeleccionadaID);

                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            dtPermisos = new DataTable();
                            adapter.Fill(dtPermisos);
                        }
                    }
                }

                LlenarDataGridView();
                AplicarFiltros();

                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al cargar permisos:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LlenarDataGridView()
        {
            dgvPermisos.Rows.Clear();

            if (dtPermisos == null || dtPermisos.Rows.Count == 0)
                return;

            // Agrupar por formulario
            var formularios = dtPermisos.AsEnumerable()
                .Select(r => new
                {
                    FormularioID = r.Field<int>("FormularioID"),
                    Categoria = r.Field<string>("NombreCategoria"),
                    Modulo = r.Field<string>("NombreModulo"),
                    Formulario = r.Field<string>("NombreFormulario")
                })
                .Distinct()
                .ToList();

            foreach (var form in formularios)
            {
                var row = new DataGridViewRow();
                row.CreateCells(dgvPermisos);

                row.Cells[0].Value = form.Categoria;
                row.Cells[1].Value = form.Modulo;
                row.Cells[2].Value = form.Formulario;

                // Guardar metadata en Tag
                row.Tag = new { FormularioID = form.FormularioID, Categoria = form.Categoria, Modulo = form.Modulo };

                // Obtener permisos de este formulario
                var permisos = dtPermisos.AsEnumerable()
                    .Where(r => r.Field<int>("FormularioID") == form.FormularioID)
                    .ToList();

                // Llenar columnas de acciones
                foreach (DataRow permiso in permisos)
                {
                    string codigoAccion = permiso.Field<string>("CodigoAccion");
                    bool permitido = permiso.Field<bool>("Permitido");
                    bool esExcepcion = permiso.Field<int>("EsExcepcion") == 1;

                    var colIndex = dgvPermisos.Columns[codigoAccion]?.Index;
                    if (colIndex.HasValue)
                    {
                        row.Cells[colIndex.Value].Value = permitido;

                        // Aplicar colores según estado
                        if (permitido)
                        {
                            if (esExcepcion)
                            {
                                // Naranja: Excepción del usuario
                                row.Cells[colIndex.Value].Style.BackColor = Color.FromArgb(255, 243, 205);
                                row.Cells[colIndex.Value].ToolTipText = "⚠️ Excepción personal del usuario";
                            }
                            else
                            {
                                // Verde: Permitido
                                row.Cells[colIndex.Value].Style.BackColor = Color.FromArgb(200, 255, 200);
                            }
                        }
                        else
                        {
                            // Rojo: Denegado
                            row.Cells[colIndex.Value].Style.BackColor = Color.FromArgb(255, 200, 200);
                        }
                    }
                }

                dgvPermisos.Rows.Add(row);
            }

            lblTotal.Text = $"Total: {dgvPermisos.Rows.Count} formularios";
        }

        private void AplicarFiltros()
        {
            if (dgvPermisos.Rows.Count == 0)
                return;

            // Obtener filtros
            string categoriaFiltro = cmbCategoria.SelectedIndex > 0
                ? ((ComboBoxItem)cmbCategoria.SelectedItem).Text
                : "";

            string moduloFiltro = cmbModulo.SelectedIndex > 0
                ? ((ComboBoxItem)cmbModulo.SelectedItem).Text
                : "";

            string busqueda = !isPlaceholder ? txtBuscar.Text.Trim().ToLower() : "";

            int visibles = 0;

            foreach (DataGridViewRow row in dgvPermisos.Rows)
            {
                bool mostrar = true;

                // Filtro por categoría
                if (!string.IsNullOrEmpty(categoriaFiltro))
                {
                    string catRow = row.Cells["Categoria"].Value?.ToString() ?? "";
                    if (catRow != categoriaFiltro)
                        mostrar = false;
                }

                // Filtro por módulo
                if (!string.IsNullOrEmpty(moduloFiltro))
                {
                    string modRow = row.Cells["Modulo"].Value?.ToString() ?? "";
                    if (modRow != moduloFiltro)
                        mostrar = false;
                }

                // Filtro por búsqueda
                if (!string.IsNullOrEmpty(busqueda))
                {
                    string formRow = row.Cells["Formulario"].Value?.ToString()?.ToLower() ?? "";
                    if (!formRow.Contains(busqueda))
                        mostrar = false;
                }

                row.Visible = mostrar;
                if (mostrar) visibles++;
            }

            lblTotal.Text = $"Mostrando: {visibles} de {dgvPermisos.Rows.Count} formularios";
        }

        #region Eventos

        private void RbTipoVista_CheckedChanged(object sender, EventArgs e)
        {
            if (cargandoDatos) return;

            modoUsuario = rbPorUsuario.Checked;
            lblEntidad.Text = modoUsuario ? "Usuario:" : "Rol:";
            CargarEntidades();
        }

        private void CmbEntidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cargandoDatos || cmbEntidad.SelectedIndex < 0)
                return;

            if (!modoUsuario)
            {
                var rol = cmbEntidad.SelectedItem as ComboBoxRol;
                entidadSeleccionadaID = rol.RolID;
            }
            else
            {
                var usuario = cmbEntidad.SelectedItem as ComboBoxUsuario;
                entidadSeleccionadaID = usuario.UsuarioID;
            }

            CargarPermisos();
        }

        private void Filtros_Changed(object sender, EventArgs e)
        {
            if (cargandoDatos) return;

            // Si cambia categoría, recargar módulos
            if (sender == cmbCategoria && cmbCategoria.SelectedIndex > 0)
            {
                var catItem = cmbCategoria.SelectedItem as ComboBoxItem;
                CargarModulos(catItem.Value);
            }

            AplicarFiltros();
        }

        private void TxtBuscar_TextChanged(object sender, EventArgs e)
        {
            if (!isPlaceholder)
                AplicarFiltros();
        }

        private void BtnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            cargandoDatos = true;

            cmbCategoria.SelectedIndex = 0;
            cmbModulo.SelectedIndex = 0;
            txtBuscar.Text = PLACEHOLDER_BUSCAR;
            txtBuscar.ForeColor = Color.Gray;
            isPlaceholder = true;

            cargandoDatos = false;

            AplicarFiltros();
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            if (dgvPermisos.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    FileName = $"Consulta_Permisos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return;

                this.Cursor = Cursors.WaitCursor;

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Permisos");

                    // Encabezados
                    for (int i = 0; i < dgvPermisos.Columns.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = dgvPermisos.Columns[i].HeaderText;
                    }

                    // Estilo encabezados
                    var headerRange = worksheet.Range(1, 1, 1, dgvPermisos.Columns.Count);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(0, 120, 212);
                    headerRange.Style.Font.FontColor = XLColor.White;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Datos
                    int row = 2;
                    foreach (DataGridViewRow dgvRow in dgvPermisos.Rows)
                    {
                        if (!dgvRow.Visible) continue;

                        for (int col = 0; col < dgvPermisos.Columns.Count; col++)
                        {
                            var cell = worksheet.Cell(row, col + 1);
                            var value = dgvRow.Cells[col].Value;

                            if (value is bool)
                            {
                                cell.Value = (bool)value ? "✓" : "✗";
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                // Aplicar colores
                                if ((bool)value)
                                    cell.Style.Fill.BackgroundColor = XLColor.FromArgb(200, 255, 200); // Verde
                                else
                                    cell.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 200, 200); // Rojo
                            }
                            else
                            {
                                cell.Value = value?.ToString() ?? "";
                            }
                        }

                        row++;
                    }

                    // Auto-ajustar columnas
                    worksheet.Columns().AdjustToContents();

                    workbook.SaveAs(saveDialog.FileName);
                }

                this.Cursor = Cursors.Default;

                MessageBox.Show("Archivo exportado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Auditoría
                AuditoriaHelper.RegistrarAccion(
                    SesionActual.UsuarioID,
                    "EXPORTAR_CONSULTA_PERMISOS",
                    "SISTEMA",
                    "Gestión de Roles",
                    "FormConsultarPermisos",
                    detalle: $"Consulta exportada: {saveDialog.FileName}"
                );
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al exportar:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnVolver_Click(object sender, EventArgs e)
        {
            FormDashboardRoles dashboard = new FormDashboardRoles(formPrincipal);
            formPrincipal.CargarContenidoPanel(dashboard);
        }

        #endregion

        #region Clases auxiliares

        private class ComboBoxItem
        {
            public int Value { get; set; }
            public string Text { get; set; }
        }

        private class ComboBoxRol
        {
            public int RolID { get; set; }
            public string NombreRol { get; set; }
            public string Descripcion { get; set; }
            public string Display => $"{NombreRol} - {Descripcion}";
        }

        private class ComboBoxUsuario
        {
            public int UsuarioID { get; set; }
            public string Username { get; set; }
            public string NombreCompleto { get; set; }
            public string NombreRol { get; set; }
            public string Display => $"{NombreCompleto} ({Username}) - {NombreRol}";
        }

        #endregion
    }
}
