using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Data.SqlClient;
using MOFIS_ERP.Classes;

namespace MOFIS_ERP.Forms.Sistema.GestionUsuarios
{
    public partial class FormUsuario : Form
    {
        private int? usuarioID = null; // null = modo creación, valor = modo edición
        private bool modoEdicion = false;
        private string passwordGenerada = "";

        // Constructor para CREAR nuevo usuario
        public FormUsuario()
        {
            InitializeComponent();
            modoEdicion = false;
            ConfigurarFormulario();
            GenerarPasswordTemporal();
        }

        // Constructor para EDITAR usuario existente
        public FormUsuario(int id)
        {
            InitializeComponent();
            usuarioID = id;
            modoEdicion = true;
            ConfigurarFormulario();
            CargarDatosUsuario();
        }

        private void ConfigurarFormulario()
        {
            // Configurar según modo
            if (modoEdicion)
            {
                lblTitulo.Text = "Editar Usuario";
                this.Text = "Editar Usuario";

                // Ocultar sección de password temporal
                grpPasswordTemporal.Visible = false;

                // Username no editable
                txtUsername.ReadOnly = true;
                txtUsername.BackColor = Color.FromArgb(240, 240, 240);

                // Ajustar tamaño del formulario
                this.Height = 480;

                // Reposicionar botones
                btnGuardar.Location = new Point(200, 365);
                btnCancelar.Location = new Point(365, 365);
            }
            else
            {
                lblTitulo.Text = "Nuevo Usuario";
                this.Text = "Nuevo Usuario";
                grpPasswordTemporal.Visible = true;
            }

            // Configurar ComboBox de roles
            CargarRoles();

            // ROOT puede asignar cualquier rol, ADMIN solo roles no-sistema
            if (SesionActual.EsAdmin() && !modoEdicion)
            {
                // Filtrar solo roles personalizados
                FiltrarRolesParaAdmin();
            }

            // Estilos de botones
            btnGuardar.BackColor = Color.FromArgb(40, 167, 69);
            btnGuardar.ForeColor = Color.White;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Cursor = Cursors.Hand;

            btnCancelar.BackColor = Color.FromArgb(108, 117, 125);
            btnCancelar.ForeColor = Color.White;
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Cursor = Cursors.Hand;

            if (!modoEdicion)
            {
                btnGenerarPassword.BackColor = Color.FromArgb(0, 120, 212);
                btnGenerarPassword.ForeColor = Color.White;
                btnGenerarPassword.FlatStyle = FlatStyle.Flat;
                btnGenerarPassword.FlatAppearance.BorderSize = 0;
                btnGenerarPassword.Cursor = Cursors.Hand;
            }

            // Eventos
            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += BtnCancelar_Click;
            if (!modoEdicion) btnGenerarPassword.Click += BtnGenerarPassword_Click;

            // Navegación con teclado
            ConfigurarNavegacion();
        }

        private void CargarRoles()
        {
            try
            {
                cmbRol.Items.Clear();

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
                            var item = new ComboBoxItem
                            {
                                Value = reader.GetInt32(0),
                                Text = reader.GetString(1),
                                EsSistema = reader.GetBoolean(2)
                            };

                            cmbRol.Items.Add(item);
                        }
                    }
                }

                cmbRol.DisplayMember = "Display";
                cmbRol.ValueMember = "Value";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar roles:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FiltrarRolesParaAdmin()
        {
            // Quitar roles del sistema (ROOT, ADMIN, etc.)
            List<ComboBoxItem> itemsPermitidos = new List<ComboBoxItem>();

            foreach (var item in cmbRol.Items)
            {
                if (item is ComboBoxItem rolItem && !rolItem.EsSistema)
                {
                    itemsPermitidos.Add(rolItem);
                }
            }

            cmbRol.Items.Clear();

            foreach (var item in itemsPermitidos)
            {
                cmbRol.Items.Add(item);
            }

            if (cmbRol.Items.Count > 0)
            {
                cmbRol.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("No hay roles personalizados disponibles.\n\nContacte a ROOT para crear roles.",
                    "Sin Roles", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRol.Enabled = false;
            }
        }

        // Clase auxiliar para ComboBox de roles
        private class ComboBoxItem
        {
            public int Value { get; set; }
            public string Text { get; set; }
            public bool EsSistema { get; set; }
            public string Display => Text; // Sin etiquetas
        }

        private void ConfigurarNavegacion()
        {
            // TabIndex ya configurado en diseñador

            // Enter = Tab entre campos
            txtUsername.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    txtNombreCompleto.Focus();
                }
            };

            txtNombreCompleto.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    txtEmail.Focus();
                }
            };

            txtEmail.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    cmbRol.Focus();
                }
            };

            cmbRol.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    chkActivo.Focus();
                }
            };

            chkActivo.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnGuardar.Focus();
                }
            };

            // Escape = Cancelar
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    btnCancelar.PerformClick();
                }
            };
        }

        private void GenerarPasswordTemporal()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%&*";
            var random = new Random();
            var password = "";

            // Generar password de 12 caracteres
            for (int i = 0; i < 12; i++)
            {
                password += chars[random.Next(chars.Length)];
            }

            // Asegurar que cumple requisitos
            if (!Regex.IsMatch(password, @"[A-Z]")) password += "A";
            if (!Regex.IsMatch(password, @"[a-z]")) password += "b";
            if (!Regex.IsMatch(password, @"[0-9]")) password += "7";
            if (!Regex.IsMatch(password, @"[!@#$%&*]")) password += "!";

            passwordGenerada = password;
            txtPasswordTemporal.Text = password;
        }

        private void BtnGenerarPassword_Click(object sender, EventArgs e)
        {
            GenerarPasswordTemporal();
        }

        private void CargarDatosUsuario()
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"SELECT U.Username, U.NombreCompleto, U.Email, 
                                  R.NombreRol, U.Activo
                                  FROM Usuarios U
                                  INNER JOIN Roles R ON U.RolID = R.RolID
                                  WHERE U.UsuarioID = @UsuarioID";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtUsername.Text = reader["Username"].ToString();
                                txtNombreCompleto.Text = reader["NombreCompleto"].ToString();
                                txtEmail.Text = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "";

                                string rol = reader["NombreRol"].ToString();

                                // Buscar y seleccionar el rol en el ComboBox
                                foreach (var item in cmbRol.Items)
                                {
                                    if (item is ComboBoxItem rolItem && rolItem.Text == rol)
                                    {
                                        cmbRol.SelectedItem = item;
                                        break;
                                    }
                                }

                                // Solo ROOT puede cambiar roles
                                if (!SesionActual.EsRoot())
                                {
                                    cmbRol.Enabled = false;
                                }

                                chkActivo.Checked = Convert.ToBoolean(reader["Activo"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuario:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            // Validaciones
            if (!ValidarCampos())
                return;

            try
            {
                if (modoEdicion)
                    ActualizarUsuario();
                else
                    CrearUsuario();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarCampos()
        {
            // Username
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Ingrese el nombre de usuario", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return false;
            }

            if (txtUsername.Text.Length < 4)
            {
                MessageBox.Show("El username debe tener al menos 4 caracteres", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return false;
            }

            if (txtUsername.Text.Contains(" "))
            {
                MessageBox.Show("El username no puede contener espacios", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return false;
            }

            // Nombre completo
            if (string.IsNullOrWhiteSpace(txtNombreCompleto.Text))
            {
                MessageBox.Show("Ingrese el nombre completo", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombreCompleto.Focus();
                return false;
            }

            if (txtNombreCompleto.Text.Length < 3)
            {
                MessageBox.Show("El nombre debe tener al menos 3 caracteres", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombreCompleto.Focus();
                return false;
            }

            // Email (opcional pero debe ser válido si se proporciona)
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBox.Show("Ingrese un email válido", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return false;
                }
            }

            // Rol
            if (cmbRol.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione un rol", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRol.Focus();
                return false;
            }

            return true;
        }

        private void CrearUsuario()
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                // Verificar que username no exista
                string sqlCheck = "SELECT COUNT(*) FROM Usuarios WHERE Username = @Username";
                using (var cmdCheck = new SqlCommand(sqlCheck, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                    int existe = (int)cmdCheck.ExecuteScalar();

                    if (existe > 0)
                    {
                        MessageBox.Show("El username ya existe", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // Obtener RolID
                int rolID = ObtenerRolID(cmbRol.SelectedItem.ToString());

                // Hash de password
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(passwordGenerada);

                // Insertar usuario
                string sql = @"INSERT INTO Usuarios 
                              (Username, PasswordHash, NombreCompleto, Email, RolID, 
                               Activo, DebeCambiarPassword, FechaCreacion, CreadoPorUsuarioID)
                              VALUES 
                              (@Username, @PasswordHash, @NombreCompleto, @Email, @RolID,
                               @Activo, 1, GETDATE(), @CreadoPorUsuarioID)";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    cmd.Parameters.AddWithValue("@NombreCompleto", txtNombreCompleto.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(txtEmail.Text) ?
                        (object)DBNull.Value : txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@RolID", rolID);
                    cmd.Parameters.AddWithValue("@Activo", chkActivo.Checked);
                    cmd.Parameters.AddWithValue("@CreadoPorUsuarioID", SesionActual.UsuarioID);

                    cmd.ExecuteNonQuery();
                }

                // Auditoría
                AuditoriaHelper.RegistrarAccion(
                    SesionActual.UsuarioID,
                    "CREAR",
                    "SISTEMA",
                    "Gestión de Usuarios",
                    "FormUsuario",
                    detalle: $"Usuario creado: {txtUsername.Text} - Rol: {cmbRol.SelectedItem}"
                );

                // Mostrar password temporal
                MessageBox.Show($"Usuario creado exitosamente.\n\n" +
                    $"Contraseña temporal:\n{passwordGenerada}\n\n" +
                    $"IMPORTANTE: Anote esta contraseña, no se volverá a mostrar.",
                    "Usuario Creado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ActualizarUsuario()
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                int rolID = ((ComboBoxItem)cmbRol.SelectedItem).Value;

                string sql = @"UPDATE Usuarios 
                              SET NombreCompleto = @NombreCompleto,
                                  Email = @Email,
                                  RolID = @RolID,
                                  Activo = @Activo,
                                  FechaModificacion = GETDATE(),
                                  ModificadoPorUsuarioID = @ModificadoPorUsuarioID
                              WHERE UsuarioID = @UsuarioID";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@NombreCompleto", txtNombreCompleto.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(txtEmail.Text) ?
                        (object)DBNull.Value : txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@RolID", rolID);
                    cmd.Parameters.AddWithValue("@Activo", chkActivo.Checked);
                    cmd.Parameters.AddWithValue("@ModificadoPorUsuarioID", SesionActual.UsuarioID);
                    cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);

                    cmd.ExecuteNonQuery();
                }

                // Auditoría
                AuditoriaHelper.RegistrarAccion(
                    SesionActual.UsuarioID,
                    "EDITAR",
                    "SISTEMA",
                    "Gestión de Usuarios",
                    "FormUsuario",
                    registroID: usuarioID,
                    detalle: $"Usuario actualizado: {txtUsername.Text}"
                );

                MessageBox.Show("Usuario actualizado exitosamente", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private int ObtenerRolID(string nombreRol)
        {
            // Ya no necesitamos buscar en BD, el ComboBoxItem tiene el Value (RolID)
            if (cmbRol.SelectedItem is ComboBoxItem item)
            {
                return item.Value;
            }

            // Fallback por si acaso
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = "SELECT RolID FROM Roles WHERE NombreRol = @NombreRol";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@NombreRol", nombreRol);
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}