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

namespace MOFIS_ERP.Forms.Sistema.GestionRoles
{
    public partial class FormConfirmarPassword : Form
    {
        public bool PasswordConfirmado { get; private set; }
        private string rolCritico;

        public FormConfirmarPassword(string nombreRol)
        {
            InitializeComponent();
            rolCritico = nombreRol;
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            lblMensaje.Text = $"Está modificando permisos del rol crítico: {rolCritico}\n\n" +
                             "Por seguridad, ingrese su contraseña para confirmar:";

            // Estilos de botones
            btnConfirmar.FlatAppearance.BorderSize = 0;
            btnCancelar.FlatAppearance.BorderSize = 0;

            // Eventos
            btnConfirmar.Click += BtnConfirmar_Click;
            btnCancelar.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            chkMostrarPassword.CheckedChanged += (s, e) =>
            {
                txtPassword.UseSystemPasswordChar = !chkMostrarPassword.Checked;
            };

            // Navegación con teclado
            this.AcceptButton = btnConfirmar;
            this.CancelButton = btnCancelar;

            // Focus en el textbox
            txtPassword.Focus();
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show(
                    "Debe ingresar su contraseña.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtPassword.Focus();
                return;
            }

            // Verificar contraseña del usuario actual
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = "SELECT PasswordHash FROM Usuarios WHERE UsuarioID = @UsuarioID";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioID", SesionActual.UsuarioID);
                        string passwordHash = cmd.ExecuteScalar() as string;

                        if (string.IsNullOrEmpty(passwordHash))
                        {
                            MessageBox.Show(
                                "Error al verificar credenciales.",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            return;
                        }

                        // Verificar contraseña con BCrypt
                        bool passwordValido = BCrypt.Net.BCrypt.Verify(txtPassword.Text, passwordHash);

                        if (!passwordValido)
                        {
                            MessageBox.Show(
                                "❌ Contraseña incorrecta.\n\nNo se puede continuar con la operación.",
                                "Acceso Denegado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            txtPassword.Clear();
                            txtPassword.Focus();
                            return;
                        }

                        // Contraseña correcta
                        PasswordConfirmado = true;

                        // Registrar en auditoría
                        AuditoriaHelper.RegistrarAccion(
                            SesionActual.UsuarioID,
                            "CONFIRMAR_PASSWORD_CAMBIO_CRITICO",
                            "SISTEMA",
                            "Gestión de Roles",
                            "FormConfirmarPassword",
                            detalle: $"Contraseña confirmada para modificar permisos de rol: {rolCritico}"
                        );

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al verificar contraseña:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}