using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using MOFIS_ERP.Classes;

namespace MOFIS_ERP
{
    public partial class FormLogin : Form
    {

        // Textos placeholder
        private const string PLACEHOLDER_USUARIO = "Usuario";
        private const string PLACEHOLDER_PASSWORD = "Contraseña";
        public FormLogin()
        {
            InitializeComponent();
            ConfigurarPlaceholders();
            CargarUsuarioRecordado();
        }

        private void CargarUsuarioRecordado()
        {
            string usuarioRecordado = Properties.Settings.Default.UsuarioRecordado;

            if (!string.IsNullOrEmpty(usuarioRecordado))
            {
                txtName.Text = usuarioRecordado;
                txtName.ForeColor = Color.LightGray;
                checkRecordarUser.Checked = true;
                txtPassword.Focus(); // Enfocar directamente en password
            }
        }
        private void ConfigurarPlaceholders()
        {
            // Configurar txtName
            txtName.Text = PLACEHOLDER_USUARIO;
            txtName.ForeColor = Color.Gray;
            txtName.Enter += TxtName_Enter;
            txtName.Leave += TxtName_Leave;
            txtName.KeyDown += TxtName_KeyDown; // Enter = Tab

            // Configurar txtPassword
            txtPassword.Text = PLACEHOLDER_PASSWORD;
            txtPassword.ForeColor = Color.Gray;
            txtPassword.Enter += TxtPassword_Enter;
            txtPassword.Leave += TxtPassword_Leave;
            txtPassword.KeyDown += TxtPassword_KeyDown; // Enter = Login

            // Configurar checkRecordarUser
            checkRecordarUser.KeyDown += CheckRecordarUser_KeyDown; // Enter = Tab
        }   

        // Eventos para txtName
        private void TxtName_Enter(object sender, EventArgs e)
        {
            if (txtName.Text == PLACEHOLDER_USUARIO)
            {
                txtName.Text = "";
                txtName.ForeColor = Color.LightGray;
            }
        }

        private void TxtName_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                txtName.Text = PLACEHOLDER_USUARIO;
                txtName.ForeColor = Color.Gray;
            }
        }

        // Eventos para txtPassword
        private void TxtPassword_Enter(object sender, EventArgs e)
        {
            if (txtPassword.Text == PLACEHOLDER_PASSWORD)
            {
                txtPassword.Text = "";
                txtPassword.ForeColor = Color.LightGray;
                txtPassword.PasswordChar = '•'; // Activar caracteres ocultos
            }
        }

        private void TxtPassword_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                txtPassword.Text = PLACEHOLDER_PASSWORD;
                txtPassword.ForeColor = Color.Gray;
                txtPassword.PasswordChar = '\0'; // Desactivar caracteres ocultos
            }
        }

        private void TxtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Evitar "beep"
                txtPassword.Focus(); // Ir a password
            }
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                checkRecordarUser.Focus(); // Ir a checkbox
            }
        }

        private void CheckRecordarUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnLogin.PerformClick(); // Ejecutar login
            }
        }

        // Método principal de login con validación real
        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Validar que no sean los placeholders
            if (txtName.Text == PLACEHOLDER_USUARIO || txtPassword.Text == PLACEHOLDER_PASSWORD)
            {
                MessageBox.Show("Por favor complete todos los campos", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validar que no estén vacíos
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Por favor complete todos los campos", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string usuario = txtName.Text.Trim();
            string password = txtPassword.Text;

            // Deshabilitar botón para evitar múltiples clicks
            btnLogin.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                // Validar usuario contra la base de datos
                var datosUsuario = Classes.AutenticacionHelper.ValidarUsuario(usuario, password);

                if (datosUsuario == null)
                {
                    // Credenciales inválidas
                    MessageBox.Show("Usuario o contraseña incorrectos", "Error de Autenticación",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Registrar intento fallido
                    Classes.AuditoriaHelper.RegistrarLoginFallido(usuario);

                    // Limpiar contraseña
                    txtPassword.Text = "";
                    txtPassword.Focus();
                    return;
                }

                // Verificar si debe cambiar contraseña
                if (datosUsuario.DebeCambiarPassword)
                {
                    MessageBox.Show("Debe cambiar su contraseña temporal",
                        "Cambio de Contraseña Requerido",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Abrir formulario de cambio de contraseña en modo forzado
                    var formCambio = new FormCambiarPassword(
                        datosUsuario.UsuarioID,
                        datosUsuario.Username,
                        esForzado: true
                    );

                    if (formCambio.ShowDialog() != DialogResult.OK)
                    {
                        // Si cancela o falla, no puede continuar
                        MessageBox.Show("Debe cambiar la contraseña para continuar",
                            "Acceso Denegado",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Actualizar flag después de cambiar contraseña
                    datosUsuario.DebeCambiarPassword = false;
                }

                // Iniciar sesión
                Classes.SesionActual.IniciarSesion(
                    datosUsuario.UsuarioID,
                    datosUsuario.Username,
                    datosUsuario.NombreCompleto,
                    datosUsuario.Email,
                    datosUsuario.RolID,
                    datosUsuario.NombreRol
                );

                // Guardar usuario si marcó "Recordar"
                if (checkRecordarUser.Checked)
                {
                    Properties.Settings.Default.UsuarioRecordado = usuario;
                }
                else
                {
                    Properties.Settings.Default.UsuarioRecordado = "";
                }
                Properties.Settings.Default.Save();

                // Actualizar último acceso
                Classes.AutenticacionHelper.ActualizarUltimoAcceso(datosUsuario.UsuarioID);

                // Registrar login en auditoría
                Classes.AuditoriaHelper.RegistrarLogin(datosUsuario.UsuarioID, datosUsuario.Username);

                // Mostrar mensaje de bienvenida
                MessageBox.Show($"Bienvenido, {datosUsuario.NombreCompleto}\n\nRol: {datosUsuario.NombreRol}",
                    "Login Exitoso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // TODO: Aquí se abrirá el MDI principal
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al intentar iniciar sesión:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Rehabilitar botón y cursor
                btnLogin.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

    }
}
