using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MOFIS_ERP.Forms.Sistema.GestionUsuarios
{
    public partial class FormConfirmarEliminacion : Form
    {
        private string usuarioAEliminar;

        public FormConfirmarEliminacion(string username)
        {
            InitializeComponent();
            usuarioAEliminar = username;
            ConfigurarFormulario();
            ConfigurarEventos();
        }

        private void ConfigurarFormulario()
        {
            // Mostrar el usuario a eliminar
            lblUsuario.Text = usuarioAEliminar;

            // Estilos de botones
            btnConfirmar.BackColor = Color.FromArgb(220, 53, 69); // Rojo
            btnConfirmar.ForeColor = Color.White;
            btnConfirmar.FlatStyle = FlatStyle.Flat;
            btnConfirmar.FlatAppearance.BorderSize = 0;
            btnConfirmar.Cursor = Cursors.Hand;
            btnConfirmar.Enabled = false; // Deshabilitado hasta escribir CONFIRMAR

            btnCancelar.BackColor = Color.FromArgb(108, 117, 125); // Gris
            btnCancelar.ForeColor = Color.White;
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Cursor = Cursors.Hand;

            // Focus inicial en el TextBox
            txtConfirmacion.Select();

            // Navegación con teclado
            ConfigurarNavegacion();
        }

        private void ConfigurarEventos()
        {
            txtConfirmacion.TextChanged += TxtConfirmacion_TextChanged;
            btnConfirmar.Click += BtnConfirmar_Click;
            btnCancelar.Click += BtnCancelar_Click;
        }

        private void ConfigurarNavegacion()
        {
            // Enter en el TextBox = Click en Confirmar (si está habilitado)
            txtConfirmacion.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && btnConfirmar.Enabled)
                {
                    e.SuppressKeyPress = true;
                    btnConfirmar.PerformClick();
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

        private void TxtConfirmacion_TextChanged(object sender, EventArgs e)
        {
            // Habilitar botón solo si escribe exactamente "CONFIRMAR"
            if (txtConfirmacion.Text.Trim() == "CONFIRMAR")
            {
                btnConfirmar.Enabled = true;
                btnConfirmar.BackColor = Color.FromArgb(200, 35, 51); // Rojo más oscuro
            }
            else
            {
                btnConfirmar.Enabled = false;
                btnConfirmar.BackColor = Color.FromArgb(220, 53, 69); // Rojo normal (deshabilitado)
            }
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            // Validación final
            if (txtConfirmacion.Text.Trim() != "CONFIRMAR")
            {
                MessageBox.Show(
                    "Debe escribir exactamente 'CONFIRMAR' en mayúsculas.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtConfirmacion.Focus();
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}