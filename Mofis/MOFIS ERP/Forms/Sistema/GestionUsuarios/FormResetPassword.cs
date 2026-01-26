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
using System.Text.RegularExpressions;
using MOFIS_ERP.Classes;

namespace MOFIS_ERP.Forms.Sistema.GestionUsuarios
{
    public partial class FormResetPassword : Form
    {
        private int usuarioID;
        private string username;
        private string nombreCompleto;
        private string passwordGenerada = "";

        public FormResetPassword(int idUsuario, string user, string nombre)
        {
            InitializeComponent();
            usuarioID = idUsuario;
            username = user;
            nombreCompleto = nombre;

            ConfigurarFormulario();
            GenerarPasswordTemporal();
        }

        private void ConfigurarFormulario()
        {
            // Mostrar nombre del usuario
            lblUsuario.Text = $"{nombreCompleto} ({username})";

            // Estilos de botones
            btnResetear.BackColor = Color.FromArgb(255, 152, 0); // Naranja
            btnResetear.ForeColor = Color.White;
            btnResetear.FlatStyle = FlatStyle.Flat;
            btnResetear.FlatAppearance.BorderSize = 0;
            btnResetear.Cursor = Cursors.Hand;

            btnCancelar.BackColor = Color.FromArgb(108, 117, 125); // Gris
            btnCancelar.ForeColor = Color.White;
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Cursor = Cursors.Hand;

            btnGenerarPassword.BackColor = Color.FromArgb(0, 120, 212); // Azul
            btnGenerarPassword.ForeColor = Color.White;
            btnGenerarPassword.FlatStyle = FlatStyle.Flat;
            btnGenerarPassword.FlatAppearance.BorderSize = 0;
            btnGenerarPassword.Cursor = Cursors.Hand;

            // Eventos
            btnResetear.Click += BtnResetear_Click;
            btnCancelar.Click += BtnCancelar_Click;
            btnGenerarPassword.Click += BtnGenerarPassword_Click;

            // Navegación con teclado
            ConfigurarNavegacion();
        }

        private void ConfigurarNavegacion()
        {
            // Enter = Tab
            chkForzarCambio.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnResetear.Focus();
                }
            };

            btnResetear.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnResetear.PerformClick();
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

        private void BtnResetear_Click(object sender, EventArgs e)
        {
            // Confirmar acción
            var resultado = MessageBox.Show(
                $"¿Está seguro de resetear la contraseña de {nombreCompleto}?\n\n" +
                $"Nueva contraseña temporal:\n{passwordGenerada}\n\n" +
                $"Esta acción no se puede deshacer.",
                "Confirmar Reset de Contraseña",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (resultado != DialogResult.Yes)
                return;

            try
            {
                ResetearPassword();

                // Mostrar contraseña para anotar
                MessageBox.Show(
                    $"Contraseña reseteada exitosamente.\n\n" +
                    $"Nueva contraseña temporal:\n{passwordGenerada}\n\n" +
                    $"IMPORTANTE: Anote esta contraseña y entréguela al usuario de forma segura.",
                    "Contraseña Reseteada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al resetear contraseña:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetearPassword()
        {
            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                // Hash de la nueva password
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(passwordGenerada);

                string sql = @"UPDATE Usuarios 
                              SET PasswordHash = @PasswordHash,
                                  DebeCambiarPassword = @DebeCambiarPassword,
                                  FechaModificacion = GETDATE(),
                                  ModificadoPorUsuarioID = @ModificadoPorUsuarioID
                              WHERE UsuarioID = @UsuarioID";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    cmd.Parameters.AddWithValue("@DebeCambiarPassword", chkForzarCambio.Checked);
                    cmd.Parameters.AddWithValue("@ModificadoPorUsuarioID", SesionActual.UsuarioID);
                    cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);

                    cmd.ExecuteNonQuery();
                }

                // Registrar en auditoría
                AuditoriaHelper.RegistrarAccion(
                    SesionActual.UsuarioID,
                    "RESET_PASSWORD",
                    "SISTEMA",
                    "Gestión de Usuarios",
                    "FormResetPassword",
                    registroID: usuarioID,
                    detalle: $"Contraseña reseteada para usuario: {username} ({nombreCompleto})"
                );
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}