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
    public partial class FormCrearRol : Form
    {
        public bool RolCreado { get; private set; }
        public int NuevoRolID { get; private set; }

        public FormCrearRol()
        {
            InitializeComponent();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            this.BackColor = Color.FromArgb(240, 240, 240);
            chkActivo.Checked = true;

            // Configurar estilos de botones
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnCancelar.FlatAppearance.BorderSize = 0;

            // Cargar categorías
            CargarCategorias();

            // Eventos
            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            // Enter para guardar, Escape para cancelar
            this.AcceptButton = btnGuardar;
            this.CancelButton = btnCancelar;

            // Focus inicial
            txtNombreRol.Focus();
        }

        private void CargarCategorias()
        {
            try
            {
                cmbCategorias.Items.Clear();
                cmbCategorias.Items.Add("Seleccione categoría...");

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"SELECT NombreCategoria 
                          FROM CatalogoCategorias 
                          WHERE Activo = 1 
                          ORDER BY OrdenVisualizacion";

                    using (var cmd = new SqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbCategorias.Items.Add(reader.GetString(0));
                        }
                    }
                }

                cmbCategorias.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar categorías:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(txtNombreRol.Text))
            {
                MessageBox.Show("Debe ingresar un nombre para el rol.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombreRol.Focus();
                return;
            }

            string nombreRol = txtNombreRol.Text.Trim().ToUpper();

            // Validar longitud
            if (nombreRol.Length < 3)
            {
                MessageBox.Show("El nombre del rol debe tener al menos 3 caracteres.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombreRol.Focus();
                return;
            }

            // Validar que no exista
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sqlExiste = "SELECT COUNT(*) FROM Roles WHERE UPPER(NombreRol) = @NombreRol";
                    using (var cmdExiste = new SqlCommand(sqlExiste, conn))
                    {
                        cmdExiste.Parameters.AddWithValue("@NombreRol", nombreRol);
                        int existe = (int)cmdExiste.ExecuteScalar();

                        if (existe > 0)
                        {
                            MessageBox.Show($"Ya existe un rol con el nombre '{nombreRol}'.", "Rol Duplicado",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtNombreRol.SelectAll();
                            txtNombreRol.Focus();
                            return;
                        }
                    }

                    // Insertar nuevo rol
                    string sqlInsert = @"INSERT INTO Roles 
                                        (NombreRol, Descripcion, Categoria, Activo, EsSistema)
                                        VALUES (@NombreRol, @Descripcion, @Categoria, @Activo, 0);
                                        SELECT SCOPE_IDENTITY();";

                    using (var cmdInsert = new SqlCommand(sqlInsert, conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@NombreRol", nombreRol);
                        cmdInsert.Parameters.AddWithValue("@Descripcion",
                            string.IsNullOrWhiteSpace(txtDescripcion.Text)
                                ? (object)DBNull.Value
                                : txtDescripcion.Text.Trim());
                        // Obtener categoría seleccionada
                        string categoriaSeleccionada = null;
                        if (cmbCategorias.SelectedIndex > 0)
                        {
                            categoriaSeleccionada = cmbCategorias.SelectedItem.ToString();
                        }

                        cmdInsert.Parameters.AddWithValue("@Categoria",
                            string.IsNullOrWhiteSpace(categoriaSeleccionada)
                                ? (object)DBNull.Value
                                : categoriaSeleccionada);
                        // CheckBox Activo
                        cmdInsert.Parameters.AddWithValue("@Activo", chkActivo.Checked);

                        NuevoRolID = Convert.ToInt32(cmdInsert.ExecuteScalar());
                    }

                    // Registrar en auditoría
                    AuditoriaHelper.RegistrarAccion(
                        SesionActual.UsuarioID,
                        "CREAR_ROL",
                        "SISTEMA",
                        "Gestión de Roles",
                        "FormCrearRol",
                        registroID: NuevoRolID,
                        detalle: $"Rol '{nombreRol}' creado sin permisos"
                    );

                    RolCreado = true;

                    MessageBox.Show(
                        $"Rol '{nombreRol}' creado exitosamente.\n\n" +
                        $"ID: {NuevoRolID}\n\n" +
                        "Ahora puede asignarle permisos desde el tab 'Permisos por Rol'.",
                        "Rol Creado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear rol:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}