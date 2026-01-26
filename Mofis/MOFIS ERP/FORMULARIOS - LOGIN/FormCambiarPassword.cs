using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOFIS_ERP.Classes;

namespace MOFIS_ERP
{
    public partial class FormCambiarPassword : Form
    {
        private int _usuarioID;
        private string _username;
        private bool _esForzado;

        /// <summary>
        /// Constructor del formulario de cambio de contraseña
        /// </summary>
        /// <param name="usuarioID">ID del usuario que cambiará la contraseña</param>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="esForzado">Si es true, el usuario no puede cancelar (contraseña temporal)</param>
        public FormCambiarPassword(int usuarioID, string username, bool esForzado = false)
        {
            InitializeComponent();

            _usuarioID = usuarioID;
            _username = username;
            _esForzado = esForzado;

            ConfigurarFormulario();
            ConfigurarNavegacion();
        }

        /// <summary>
        /// Configura el formulario según si el cambio es forzado o voluntario
        /// </summary>
        private void ConfigurarFormulario()
        {
            if (_esForzado)
            {
                // Si es forzado, no puede cancelar ni cerrar
                lblInstrucciones.Visible = true;
                btnCancelar.Visible = false;

                // Centrar botón de cambiar
                btnCambiar.Left = (this.ClientSize.Width - btnCambiar.Width) / 2;

                // Quitar botón de cerrar (X)
                this.ControlBox = false;
            }
            else
            {
                // Si es voluntario, puede cancelar
                lblInstrucciones.Visible = false;
                this.ControlBox = true;
            }
        }

        /// <summary>
        /// Evento del botón Cambiar Contraseña
        /// </summary>
        private void btnCambiar_Click(object sender, EventArgs e)
        {
            // Validación 1: Contraseña actual no vacía
            if (string.IsNullOrWhiteSpace(txtPasswordActual.Text))
            {
                MessageBox.Show("Ingrese la contraseña actual", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPasswordActual.Focus();
                return;
            }

            // Validación 2: Nueva contraseña no vacía
            if (string.IsNullOrWhiteSpace(txtPasswordNueva.Text))
            {
                MessageBox.Show("Ingrese la nueva contraseña", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPasswordNueva.Focus();
                return;
            }

            // Validación 3: Confirmar contraseña no vacía
            if (string.IsNullOrWhiteSpace(txtPasswordConfirmar.Text))
            {
                MessageBox.Show("Confirme la nueva contraseña", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPasswordConfirmar.Focus();
                return;
            }

            // Validación 4: Las contraseñas nuevas coinciden
            if (txtPasswordNueva.Text != txtPasswordConfirmar.Text)
            {
                MessageBox.Show("Las contraseñas no coinciden", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPasswordConfirmar.Focus();
                txtPasswordConfirmar.SelectAll();
                return;
            }

            // Validación 5: Fortaleza de la contraseña
            string mensajeError = AutenticacionHelper.ValidarFortalezaPassword(txtPasswordNueva.Text);
            if (!string.IsNullOrEmpty(mensajeError))
            {
                MessageBox.Show(mensajeError, "Contraseña Débil",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPasswordNueva.Focus();
                txtPasswordNueva.SelectAll();
                return;
            }

            // Deshabilitar botón y cambiar cursor
            btnCambiar.Enabled = false;
            btnCancelar.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                // Intentar cambiar la contraseña
                bool exito = AutenticacionHelper.CambiarPassword(
                    _usuarioID,
                    txtPasswordActual.Text,
                    txtPasswordNueva.Text
                );

                if (exito)
                {
                    // Registrar en auditoría
                    AuditoriaHelper.RegistrarAccion(
                        _usuarioID,
                        "CAMBIO_PASSWORD",
                        "SISTEMA",
                        "Seguridad",
                        "FormCambiarPassword",
                        detalle: $"Usuario {_username} cambió su contraseña exitosamente"
                    );

                    MessageBox.Show("Contraseña cambiada exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    // Contraseña actual incorrecta
                    MessageBox.Show("La contraseña actual es incorrecta", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPasswordActual.Focus();
                    txtPasswordActual.SelectAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cambiar contraseña:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Rehabilitar botones y cursor
                btnCambiar.Enabled = true;
                btnCancelar.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Evento del botón Cancelar
        /// </summary>
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Evento al cargar el formulario
        /// </summary>
        private void FormCambiarPassword_Load(object sender, EventArgs e)
        {
            // Enfocar el primer campo
            txtPasswordActual.Focus();
        }

        private void ConfigurarNavegacion()
        {
            // Enter = Tab entre campos
            txtPasswordActual.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    txtPasswordNueva.Focus();
                }
            };

            txtPasswordNueva.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    txtPasswordConfirmar.Focus();
                }
            };

            txtPasswordConfirmar.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnCambiar.PerformClick();
                }
            };

            // Escape = Cancelar
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape && btnCancelar != null && btnCancelar.Visible)
                {
                    btnCancelar.PerformClick();
                }
            };
        }
    }
}