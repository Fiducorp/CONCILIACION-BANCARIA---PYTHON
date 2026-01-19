using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOFIS_ERP.Classes;

namespace MOFIS_ERP.Forms.Sistema.GestionUsuarios
{
    public partial class FormGestionUsuarios : Form
    {
        private FormMain formPrincipal;
        private const string PLACEHOLDER_BUSCAR = "Nombre o usuario...";
        private bool isPlaceholder = true;
        private bool cargaInicial = true;

        public FormGestionUsuarios(FormMain formMain)
        {
            InitializeComponent();
            formPrincipal = formMain;
            ConfigurarFormulario();
            ConfigurarPlaceholder();
            ConfigurarEventos();
            ConfigurarPermisos();
            CargarUsuarios();
            cargaInicial = false;

            // Ejecutar limpieza inicial para deseleccionar todo
            BtnLimpiar_Click(null, null);
        }

        private void ConfigurarFormulario()
        {
            // Configurar controles para pantalla completa
            panelFiltros.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelAcciones.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvUsuarios.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // ComboBoxes
            CargarRoles();
            cmbRol.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbEstado.Items.Clear();
            cmbEstado.Items.AddRange(new object[] { "Todos", "Activos", "Inactivos" });
            cmbEstado.SelectedIndex = 0;
            cmbEstado.DropDownStyle = ComboBoxStyle.DropDownList;

            // Estilo botón Volver
            btnVolver.Text = "← Volver";
            btnVolver.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnVolver.Size = new Size(120, 40);
            btnVolver.BackColor = Color.White;
            btnVolver.ForeColor = Color.FromArgb(0, 120, 212);
            btnVolver.FlatStyle = FlatStyle.Flat;
            btnVolver.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 212);
            btnVolver.Cursor = Cursors.Hand;

            // Estilo botón Limpiar (elegante)
            btnLimpiar.BackColor = Color.White;
            btnLimpiar.ForeColor = Color.FromArgb(0, 120, 212);
            btnLimpiar.FlatStyle = FlatStyle.Flat;
            btnLimpiar.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 212);
            btnLimpiar.FlatAppearance.BorderSize = 1;
            btnLimpiar.Cursor = Cursors.Hand;
            btnLimpiar.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnLimpiar.Text = "🔄 Limpiar";

            ConfigurarDataGridView();
            ConfigurarHoverBotones();
        }

        private void CargarRoles()
        {
            try
            {
                cmbRol.Items.Clear();
                cmbRol.Items.Add("Todos");

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Cargar TODOS los roles activos (sistema y personalizados)
                    string sql = @"SELECT RolID, NombreRol, EsSistema
                          FROM Roles 
                          WHERE Activo = 1 
                          ORDER BY EsSistema DESC, NombreRol";

                    using (var cmd = new SqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string nombreRol = reader.GetString(1);
                            bool esSistema = reader.GetBoolean(2);

                            string displayText = nombreRol; // Sin etiquetas en FormGestionUsuarios
                            cmbRol.Items.Add(displayText);
                        }
                    }
                }

                cmbRol.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar roles:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarHoverBotones()
        {
            // Color azul corporativo para: Nuevo, Reset Password, Activar/Desactivar
            Color azulCorporativo = Color.FromArgb(0, 120, 212);
            Color azulCorporativoHover = Color.FromArgb(0, 100, 180);

            // Hover para btnNuevo
            ConfigurarHoverBoton(btnNuevo, azulCorporativo, azulCorporativoHover);

            // Hover para btnEditar (verde)
            ConfigurarHoverBoton(btnEditar, Color.FromArgb(40, 167, 69), Color.FromArgb(33, 136, 56));

            // Hover para btnResetPassword (mismo azul)
            ConfigurarHoverBoton(btnResetPassword, azulCorporativo, azulCorporativoHover);

            // Hover para btnActivarDesactivar (mismo azul)
            ConfigurarHoverBoton(btnActivarDesactivar, azulCorporativo, azulCorporativoHover);

            // Hover para btnEliminar (rojo)
            ConfigurarHoverBoton(btnEliminar, Color.FromArgb(220, 53, 69), Color.FromArgb(200, 35, 51));

            // Hover especial para btnLimpiar
            btnLimpiar.MouseEnter += (s, e) =>
            {
                btnLimpiar.BackColor = Color.FromArgb(0, 120, 212);
                btnLimpiar.ForeColor = Color.White;
            };
            btnLimpiar.MouseLeave += (s, e) =>
            {
                btnLimpiar.BackColor = Color.White;
                btnLimpiar.ForeColor = Color.FromArgb(0, 120, 212);
            };
        }

        private void ConfigurarHoverBoton(Button btn, Color colorNormal, Color colorHover)
        {
            btn.BackColor = colorNormal;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 14, FontStyle.Bold); // Tamaño de fuente


            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = colorHover;
                btn.FlatAppearance.BorderSize = 2;
                btn.FlatAppearance.BorderColor = Color.White;
            };

            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = colorNormal;
                btn.FlatAppearance.BorderSize = 0;
            };
        }

        private void ConfigurarDataGridView()
        {
            dgvUsuarios.Columns.Clear();
            dgvUsuarios.AutoGenerateColumns = false;
            dgvUsuarios.AllowUserToResizeColumns = false;
            dgvUsuarios.RowHeadersVisible = false;
            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None; // Para no usar anchos fijos

            // Columnas
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "UsuarioID",
                HeaderText = "ID",
                DataPropertyName = "UsuarioID",
                Width = 150
            });

            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Username",
                HeaderText = "Usuario",
                DataPropertyName = "Username",
                Width = 280,

            }); 

            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NombreCompleto",
                HeaderText = "Nombre Completo",
                DataPropertyName = "NombreCompleto",
                Width = 370
            });

            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Email",
                HeaderText = "Email",
                DataPropertyName = "Email",
                Width = 370
            });

            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NombreRol",
                HeaderText = "Rol",
                DataPropertyName = "NombreRol",
                Width = 160,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                HeaderCell = { Style = { Alignment = DataGridViewContentAlignment.MiddleCenter } }

            });

            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Estado",
                HeaderText = "Estado",
                DataPropertyName = "EstadoTexto",
                Width = 160,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                HeaderCell = { Style = { Alignment = DataGridViewContentAlignment.MiddleCenter } }
            });

            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "UltimoAcceso",
                HeaderText = "Último Acceso",
                DataPropertyName = "UltimoAccesoTexto",
                Width = 215
            });

            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FechaCreacion",
                HeaderText = "Fecha Creación",
                DataPropertyName = "FechaCreacionTexto",
                Width = 215

            });

            // Estilos
            dgvUsuarios.EnableHeadersVisualStyles = false;
            dgvUsuarios.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 212);
            dgvUsuarios.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUsuarios.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            dgvUsuarios.ColumnHeadersHeight = 40;
            dgvUsuarios.DefaultCellStyle.Font = new Font("Segoe UI", 14);
            dgvUsuarios.RowTemplate.Height = 35;
            dgvUsuarios.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
        }

        private void ConfigurarPlaceholder()
        {
            txtBuscar.Text = PLACEHOLDER_BUSCAR;
            txtBuscar.ForeColor = Color.Gray;
            isPlaceholder = true;

            txtBuscar.Enter += TxtBuscar_Enter;
            txtBuscar.Leave += TxtBuscar_Leave;
        }

        private void TxtBuscar_Enter(object sender, EventArgs e)
        {
            if (isPlaceholder)
            {
                txtBuscar.Text = "";
                txtBuscar.ForeColor = Color.Black;
                isPlaceholder = false;
            }
        }

        private void TxtBuscar_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = PLACEHOLDER_BUSCAR;
                txtBuscar.ForeColor = Color.Gray;
                isPlaceholder = true;
                AplicarFiltros();
            }
        }

        private void ConfigurarEventos()
        {
            // Búsqueda en tiempo real
            txtBuscar.TextChanged += (s, e) =>
            {
                if (!isPlaceholder)
                    AplicarFiltros();
            };

            cmbRol.SelectedIndexChanged += (s, e) => AplicarFiltros();
            cmbEstado.SelectedIndexChanged += (s, e) => AplicarFiltros();

            btnLimpiar.Click += BtnLimpiar_Click;
            btnNuevo.Click += BtnNuevo_Click;
            btnEditar.Click += BtnEditar_Click;
            btnResetPassword.Click += BtnResetPassword_Click;
            btnActivarDesactivar.Click += BtnActivarDesactivar_Click;
            btnEliminar.Click += BtnEliminar_Click;
            btnVolver.Click += BtnVolver_Click;

            dgvUsuarios.CellDoubleClick += DgvUsuarios_CellDoubleClick;
            dgvUsuarios.SelectionChanged += DgvUsuarios_SelectionChanged;
        }

        // Configurar permisos de usuario para botones
        private void ConfigurarPermisos()
        {
            // Obtener todos los permisos del formulario de una vez
            var permisos = PermisosHelper.ObtenerPermisosFormulario("FGEUSR");

            // Configurar botones según permisos
            btnNuevo.Enabled = permisos.PuedeCrear;
            btnEditar.Enabled = permisos.PuedeEditar;
            btnResetPassword.Enabled = permisos.PuedeResetear;
            btnActivarDesactivar.Enabled = permisos.PuedeActivar;
            btnEliminar.Enabled = permisos.PuedeEliminar;

            // Configurar tooltips solo para botones deshabilitados
            ToolTip tooltip = new ToolTip();

            if (!btnNuevo.Enabled)
                tooltip.SetToolTip(btnNuevo, "Sin permisos para crear usuarios");

            if (!btnEditar.Enabled)
                tooltip.SetToolTip(btnEditar, "Sin permisos para editar usuarios");

            if (!btnResetPassword.Enabled)
                tooltip.SetToolTip(btnResetPassword, "Sin permisos para resetear contraseñas");

            if (!btnActivarDesactivar.Enabled)
                tooltip.SetToolTip(btnActivarDesactivar, "Sin permisos para activar/desactivar usuarios");

            if (!btnEliminar.Enabled)
                tooltip.SetToolTip(btnEliminar, "Sin permisos para eliminar usuarios");
        }

        // Cargar usuarios desde la base de datos
        private void CargarUsuarios()
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"SELECT 
                                    U.UsuarioID, U.Username, U.NombreCompleto, U.Email,
                                    U.RolID, R.NombreRol, U.Activo, U.DebeCambiarPassword,
                                    U.FechaCreacion, U.UltimoAcceso, U.CreadoPorUsuarioID,
                                    U.FechaModificacion, U.ModificadoPorUsuarioID
                                  FROM Usuarios U
                                  INNER JOIN Roles R ON U.RolID = R.RolID
                                  WHERE U.EsEliminado = 0
                                  ORDER BY U.FechaCreacion DESC";

                    using (var cmd = new SqlCommand(sql, conn))
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dt.Columns.Add("EstadoTexto", typeof(string));
                        dt.Columns.Add("UltimoAccesoTexto", typeof(string));
                        dt.Columns.Add("FechaCreacionTexto", typeof(string));

                        foreach (DataRow row in dt.Rows)
                        {
                            row["EstadoTexto"] = Convert.ToBoolean(row["Activo"]) ? "Activo" : "Inactivo";
                            row["UltimoAccesoTexto"] = row["UltimoAcceso"] != DBNull.Value
                                ? Convert.ToDateTime(row["UltimoAcceso"]).ToString("dd/MM/yyyy HH:mm")
                                : "Nunca";
                            row["FechaCreacionTexto"] = Convert.ToDateTime(row["FechaCreacion"]).ToString("dd/MM/yyyy");
                        }

                        dgvUsuarios.DataSource = dt;

                        // Forzar deselección inicial
                        dgvUsuarios.CurrentCell = null;
                    }
                }

                ActualizarContador();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AplicarFiltros()
        {
            if (dgvUsuarios.DataSource == null || cargaInicial) return;

            DataTable dt = (DataTable)dgvUsuarios.DataSource;
            string filtro = "1=1";

            if (!isPlaceholder && !string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                string busqueda = txtBuscar.Text.Trim().Replace("'", "''");
                filtro += $" AND (Username LIKE '%{busqueda}%' OR NombreCompleto LIKE '%{busqueda}%')";
            }

            if (cmbRol.SelectedIndex > 0)
            {
                // Extraer solo el nombre del rol (quitar "(Personalizado)" si existe)
                string rolSeleccionado = cmbRol.SelectedItem.ToString();
                if (rolSeleccionado.Contains(" (Personalizado)"))
                {
                    rolSeleccionado = rolSeleccionado.Replace(" (Personalizado)", "");
                }
                filtro += $" AND NombreRol = '{rolSeleccionado}'";
            }

            if (cmbEstado.SelectedIndex > 0)
            {
                bool activo = cmbEstado.SelectedItem.ToString() == "Activos";
                filtro += $" AND Activo = {(activo ? "True" : "False")}";
            }

            dt.DefaultView.RowFilter = filtro;
            dgvUsuarios.CurrentCell = null; // Deseleccionar
            ActualizarContador();
        }

        private void ActualizarContador()
        {
            if (dgvUsuarios.DataSource != null)
            {
                DataTable dt = (DataTable)dgvUsuarios.DataSource;
                int total = dt.DefaultView.Count;
                this.Text = $"Gestión de Usuarios ({total} usuario{(total != 1 ? "s" : "")})";
            }
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = PLACEHOLDER_BUSCAR;
            txtBuscar.ForeColor = Color.Gray;
            isPlaceholder = true;

            cmbRol.SelectedIndex = 0;
            cmbEstado.SelectedIndex = 0;

            AplicarFiltros();
        }

        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            var formUsuario = new FormUsuario();
            if (formUsuario.ShowDialog() == DialogResult.OK)
            {
                CargarUsuarios(); // Recargar lista
            }
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un usuario para editar",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int usuarioID = Convert.ToInt32(dgvUsuarios.SelectedRows[0].Cells["UsuarioID"].Value);
            var formUsuario = new FormUsuario(usuarioID);
            if (formUsuario.ShowDialog() == DialogResult.OK)
            {
                CargarUsuarios(); // Recargar lista
            }
        }

        private void BtnResetPassword_Click(object sender, EventArgs e)
        {
            // Verificar que hay una fila seleccionada
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Debe seleccionar un usuario para resetear su contraseña.",
                    "Selección requerida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            DataGridViewRow filaSeleccionada = dgvUsuarios.SelectedRows[0];

            int usuarioID = Convert.ToInt32(filaSeleccionada.Cells["UsuarioID"].Value);
            string username = filaSeleccionada.Cells["Username"].Value.ToString();
            string nombreCompleto = filaSeleccionada.Cells["NombreCompleto"].Value.ToString();

            // Obtener el valor de Activo desde el DataRowView subyacente
            DataRowView rowView = (DataRowView)filaSeleccionada.DataBoundItem;
            bool activo = Convert.ToBoolean(rowView["Activo"]);

            // Verificar que el usuario esté activo
            if (!activo)
            {
                MessageBox.Show(
                    "No se puede resetear la contraseña de un usuario inactivo.",
                    "Usuario inactivo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Confirmar acción
            DialogResult confirmacion = MessageBox.Show(
                $"¿Está seguro que desea resetear la contraseña del usuario?\n\n" +
                $"Usuario: {username}\n" +
                $"Nombre: {nombreCompleto}\n\n" +
                $"Se generará una nueva contraseña temporal.",
                "Confirmar Reset de Contraseña",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmacion == DialogResult.Yes)
            {
                // Abrir formulario de reset
                using (FormResetPassword formReset = new FormResetPassword(usuarioID, username, nombreCompleto))
                {
                    if (formReset.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show(
                            "Contraseña reseteada exitosamente.",
                            "Éxito",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        // No necesitamos recargar el grid ya que solo cambió la password
                    }
                }
            }
        }

        private void BtnActivarDesactivar_Click(object sender, EventArgs e)
        {
            // Verificar que hay una fila seleccionada
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Debe seleccionar un usuario para cambiar su estado.",
                    "Selección requerida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            DataGridViewRow filaSeleccionada = dgvUsuarios.SelectedRows[0];

            int usuarioID = Convert.ToInt32(filaSeleccionada.Cells["UsuarioID"].Value);
            string username = filaSeleccionada.Cells["Username"].Value.ToString();
            string nombreCompleto = filaSeleccionada.Cells["NombreCompleto"].Value.ToString();

            // Obtener el valor de Activo desde el DataRowView subyacente
            DataRowView rowView = (DataRowView)filaSeleccionada.DataBoundItem;
            bool activo = Convert.ToBoolean(rowView["Activo"]);

            // NO permitir que el usuario se desactive a sí mismo
            if (usuarioID == SesionActual.UsuarioID)
            {
                MessageBox.Show(
                    "No puede cambiar su propio estado.\n\nDebe pedir a otro administrador que lo haga.",
                    "Operación no permitida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Determinar acción
            string accion = activo ? "desactivar" : "activar";
            string estadoNuevo = activo ? "INACTIVO" : "ACTIVO";

            // Confirmar acción
            DialogResult confirmacion = MessageBox.Show(
                $"¿Está seguro que desea {accion.ToUpper()} este usuario?\n\n" +
                $"Usuario: {username}\n" +
                $"Nombre: {nombreCompleto}\n" +
                $"Estado actual: {(activo ? "ACTIVO" : "INACTIVO")}\n" +
                $"Nuevo estado: {estadoNuevo}\n\n" +
                (activo ? "⚠️ El usuario NO podrá iniciar sesión mientras esté inactivo." : "✅ El usuario podrá iniciar sesión nuevamente."),
                $"Confirmar {accion}",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmacion == DialogResult.Yes)
            {
                try
                {
                    CambiarEstadoUsuario(usuarioID, !activo, username, nombreCompleto);

                    MessageBox.Show(
                        $"Usuario {accion}do exitosamente.",
                        "Éxito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // Recargar la lista para reflejar el cambio
                    CargarUsuarios();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Error al {accion} usuario:\n\n{ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void CambiarEstadoUsuario(int usuarioID, bool nuevoEstado, string username, string nombreCompleto)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                string sql = @"UPDATE Usuarios 
                      SET Activo = @Activo,
                          FechaModificacion = GETDATE(),
                          ModificadoPorUsuarioID = @ModificadoPorUsuarioID
                      WHERE UsuarioID = @UsuarioID";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Activo", nuevoEstado);
                    cmd.Parameters.AddWithValue("@ModificadoPorUsuarioID", SesionActual.UsuarioID);
                    cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);

                    cmd.ExecuteNonQuery();
                }

                // Registrar en auditoría
                string accion = nuevoEstado ? "ACTIVAR_USUARIO" : "DESACTIVAR_USUARIO";
                string detalle = $"Usuario {(nuevoEstado ? "activado" : "desactivado")}: {username} ({nombreCompleto})";

                AuditoriaHelper.RegistrarAccion(
                    SesionActual.UsuarioID,
                    accion,
                    "SISTEMA",
                    "Gestión de Usuarios",
                    "FormGestionUsuarios",
                    registroID: usuarioID,
                    detalle: detalle
                );
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            // Verificar que hay una fila seleccionada
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Debe seleccionar un usuario para eliminar.",
                    "Selección requerida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            DataGridViewRow filaSeleccionada = dgvUsuarios.SelectedRows[0];

            int usuarioID = Convert.ToInt32(filaSeleccionada.Cells["UsuarioID"].Value);
            string username = filaSeleccionada.Cells["Username"].Value.ToString();
            string nombreCompleto = filaSeleccionada.Cells["NombreCompleto"].Value.ToString();

            // NO permitir que ROOT se elimine a sí mismo
            if (usuarioID == SesionActual.UsuarioID)
            {
                MessageBox.Show(
                    "No puede eliminarse a sí mismo.\n\n" +
                    "Esta es una medida de seguridad para evitar quedarse sin acceso al sistema.",
                    "Operación no permitida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            // Primera confirmación
            DialogResult confirmacion1 = MessageBox.Show(
                $"⚠️ ADVERTENCIA: Esta acción eliminará permanentemente el usuario.\n\n" +
                $"Usuario: {username}\n" +
                $"Nombre: {nombreCompleto}\n\n" +
                $"El usuario NO podrá iniciar sesión nunca más.\n" +
                $"Esta acción NO se puede deshacer.\n\n" +
                $"¿Está completamente seguro?",
                "⚠️ CONFIRMAR ELIMINACIÓN",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirmacion1 != DialogResult.Yes)
                return;

            // Segunda confirmación con entrada de texto
            FormConfirmarEliminacion formConfirmar = new FormConfirmarEliminacion(username);

            if (formConfirmar.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    EliminarUsuario(usuarioID, username, nombreCompleto);

                    MessageBox.Show(
                        $"Usuario eliminado exitosamente.\n\n" +
                        $"Usuario: {username}\n" +
                        $"Nombre: {nombreCompleto}",
                        "Usuario Eliminado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // Recargar la lista
                    CargarUsuarios();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Error al eliminar usuario:\n\n{ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void EliminarUsuario(int usuarioID, string username, string nombreCompleto)
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                // Eliminación LÓGICA (no física)
                string sql = @"UPDATE Usuarios 
                      SET EsEliminado = 1,
                          Activo = 0,
                          FechaEliminacion = GETDATE(),
                          EliminadoPorUsuarioID = @EliminadoPorUsuarioID,
                          FechaModificacion = GETDATE(),
                          ModificadoPorUsuarioID = @ModificadoPorUsuarioID
                      WHERE UsuarioID = @UsuarioID";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@EliminadoPorUsuarioID", SesionActual.UsuarioID);
                    cmd.Parameters.AddWithValue("@ModificadoPorUsuarioID", SesionActual.UsuarioID);
                    cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);

                    cmd.ExecuteNonQuery();
                }

                // Registrar en auditoría
                AuditoriaHelper.RegistrarAccion(
                    SesionActual.UsuarioID,
                    "ELIMINAR_USUARIO",
                    "SISTEMA",
                    "Gestión de Usuarios",
                    "FormGestionUsuarios",
                    registroID: usuarioID,
                    detalle: $"Usuario eliminado: {username} ({nombreCompleto})"
                );
            }
        }

        private void BtnVolver_Click(object sender, EventArgs e)
        {
            FormDashboardSistema dashboardSistema = new FormDashboardSistema(formPrincipal);
            formPrincipal.CargarContenidoPanel(dashboardSistema);
        }

        private void DgvUsuarios_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                BtnEditar_Click(sender, e);
            }
        }

        private void DgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            // Actualizar texto del botón Activar/Desactivar según estado del usuario seleccionado
            if (dgvUsuarios.SelectedRows.Count > 0)
            {
                DataRowView rowView = (DataRowView)dgvUsuarios.SelectedRows[0].DataBoundItem;
                bool activo = Convert.ToBoolean(rowView["Activo"]);

                btnActivarDesactivar.Text = activo ? "🔴 Desactivar" : "🟢 Activar";
            }
            else
            {
                btnActivarDesactivar.Text = "⚪ Activar/Desactivar";
            }
        }
    }
}