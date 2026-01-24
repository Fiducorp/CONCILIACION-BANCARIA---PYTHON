using MOFIS_ERP.Classes;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MOFIS_ERP.Forms.Contabilidad.CuentasPorPagar.CartasSolicitudes.Solicitud_de_pago
{
    public partial class FormAgregarFideicomiso : Form
    {
        // ═══════════════════════════════════════════════════════════════
        // CAMPOS PRIVADOS
        // ═══════════════════════════════════════════════════════════════
        private FormSolicitudPago formPadre;
        private bool esModificacion = false;
        private int fideicomisoIDCargado = 0;
        private string codigoOriginal = string.Empty;
        private string rncOriginal = string.Empty;

        // ═══════════════════════════════════════════════════════════════
        // PROPIEDADES PÚBLICAS (para comunicación con FormSolicitudPago)
        // ═══════════════════════════════════════════════════════════════
        public int FideicomisoIDResultado { get; private set; }
        public string CodigoResultado { get; private set; }
        public string NombreResultado { get; private set; }
        public string RNCResultado { get; private set; }

        // ═══════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ═══════════════════════════════════════════════════════════════
        public FormAgregarFideicomiso(FormSolicitudPago padre)
        {
            InitializeComponent();
            formPadre = padre;
            ConfigurarFormulario();
        }

        // ═══════════════════════════════════════════════════════════════
        // CONFIGURACIÓN INICIAL
        // ═══════════════════════════════════════════════════════════════
        private void ConfigurarFormulario()
        {
            // Configurar como diálogo modal
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Cargar tipos de fideicomiso
            CargarTiposFideicomiso();

            // Configurar eventos
            ConfigurarEventos();

            // Estado inicial: modo nuevo registro
            EstablecerModoNuevo();
        }

        private void ConfigurarEventos()
        {
            // Eventos de botones
            btnBuscarCodigo.Click += BtnBuscarCodigo_Click;
            btnGuardarFideicomiso.Click += BtnGuardarFideicomiso_Click;
            btnEliminarFideicomiso.Click += BtnEliminarFideicomiso_Click;
            btnCancelarFideicomiso.Click += BtnCancelarFideicomiso_Click;

            // Eventos de RNC (formateo)
            txtRNC.Enter += TxtRNC_Enter;
            txtRNC.Leave += TxtRNC_Leave;
            txtRNC.KeyPress += NumericInteger_KeyPress;

            // Evento de código (solo números)
            txtCodigo.KeyPress += NumericInteger_KeyPress;

            // Eventos para detectar cambios (determinar si es modificación o nuevo)
            txtCodigo.TextChanged += Txt_CambioDetectado;
            txtNombre.TextChanged += Txt_CambioDetectado;
            txtRNC.TextChanged += Txt_CambioDetectado;
        }

        // ═══════════════════════════════════════════════════════════════
        // CARGAR TIPOS DE FIDEICOMISO
        // ═══════════════════════════════════════════════════════════════
        private void CargarTiposFideicomiso()
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT TipoFideicomisoID, Codigo, Nombre 
                                     FROM TiposFideicomiso 
                                     WHERE Activo = 1 
                                     ORDER BY Nombre";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cboTipoFideicomiso.DataSource = dt;
                    cboTipoFideicomiso.DisplayMember = "Nombre";
                    cboTipoFideicomiso.ValueMember = "TipoFideicomisoID";
                    cboTipoFideicomiso.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar tipos de fideicomiso: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // BUSCAR FIDEICOMISO POR CÓDIGO
        // ═══════════════════════════════════════════════════════════════
        private void BtnBuscarCodigo_Click(object sender, EventArgs e)
        {
            string codigo = txtCodigo.Text.Trim();

            if (string.IsNullOrEmpty(codigo))
            {
                MessageBox.Show("Ingrese un código para buscar.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigo.Focus();
                return;
            }

            BuscarYCargarFideicomiso(codigo);
        }

        private void BuscarYCargarFideicomiso(string codigo)
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT FideicomisoID, Codigo, Nombre, RNC, TipoFideicomisoID
                                     FROM Fideicomisos 
                                     WHERE Codigo = @Codigo 
                                     AND Activo = 1 
                                     AND EsEliminado = 0";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codigo);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Fideicomiso encontrado - cargar datos
                                fideicomisoIDCargado = Convert.ToInt32(reader["FideicomisoID"]);
                                codigoOriginal = reader["Codigo"].ToString();
                                rncOriginal = reader["RNC"].ToString();

                                txtCodigo.Text = codigoOriginal;
                                txtNombre.Text = reader["Nombre"].ToString();
                                txtRNC.Text = rncOriginal;

                                // Cargar tipo de fideicomiso si existe
                                if (reader["TipoFideicomisoID"] != DBNull.Value)
                                {
                                    cboTipoFideicomiso.SelectedValue = Convert.ToInt32(reader["TipoFideicomisoID"]);
                                }
                                else
                                {
                                    cboTipoFideicomiso.SelectedIndex = -1;
                                }

                                // Cambiar a modo modificación
                                EstablecerModoModificacion();

                                MessageBox.Show("Fideicomiso cargado correctamente.",
                                    "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show($"No se encontró un fideicomiso con el código '{codigo}'.",
                                    "No encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar fideicomiso: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // GUARDAR FIDEICOMISO (NUEVO O MODIFICACIÓN)
        // ═══════════════════════════════════════════════════════════════
        private void BtnGuardarFideicomiso_Click(object sender, EventArgs e)
        {
            // Validar campos obligatorios
            if (!ValidarCampos())
                return;

            // Determinar si es nuevo registro o modificación
            string codigoActual = txtCodigo.Text.Trim();
            string rncActual = Regex.Replace(txtRNC.Text, @"\D", string.Empty);

            bool cambioCodigoYRNC = esModificacion &&
                                    (codigoActual != codigoOriginal && rncActual != rncOriginal);

            if (cambioCodigoYRNC)
            {
                // Cambio de código Y RNC = nuevo registro
                DialogResult result = MessageBox.Show(
                    "Al cambiar tanto el código como el RNC, se creará un NUEVO fideicomiso.\n\n¿Desea continuar?",
                    "Nuevo Registro",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    GuardarNuevoFideicomiso();
                }
            }
            else if (esModificacion)
            {
                // Es una modificación
                ModificarFideicomiso();
            }
            else
            {
                // Es un nuevo registro
                GuardarNuevoFideicomiso();
            }
        }

        private void GuardarNuevoFideicomiso()
        {
            try
            {
                string codigo = txtCodigo.Text.Trim();
                string nombre = txtNombre.Text.Trim();
                string rnc = Regex.Replace(txtRNC.Text, @"\D", string.Empty);
                int? tipoFideicomisoID = cboTipoFideicomiso.SelectedIndex >= 0
                    ? (int?)cboTipoFideicomiso.SelectedValue
                    : null;

                // Validar unicidad
                if (!ValidarUnicidad(codigo, nombre, rnc, null))
                    return;

                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO Fideicomisos 
                                    (Codigo, Nombre, RNC, TipoFideicomisoID, Activo, CreadoPorUsuarioID)
                                    VALUES 
                                    (@Codigo, @Nombre, @RNC, @TipoFideicomisoID, 1, @UsuarioID);
                                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codigo);
                        cmd.Parameters.AddWithValue("@Nombre", nombre);
                        cmd.Parameters.AddWithValue("@RNC", rnc);
                        cmd.Parameters.AddWithValue("@TipoFideicomisoID",
                            tipoFideicomisoID.HasValue ? (object)tipoFideicomisoID.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@UsuarioID", SesionActual.UsuarioID);

                        int nuevoID = (int)cmd.ExecuteScalar();

                        // Auditoría

                        AuditoriaHelper.RegistrarAccion(
                            nuevoID,
                            "Fideicomisos",
                            "INSERT",
                            $"Nuevo fideicomiso: {codigo} - {nombre}"
                        );

                        // Guardar datos del resultado
                        FideicomisoIDResultado = nuevoID;
                        CodigoResultado = codigo;
                        NombreResultado = nombre;
                        RNCResultado = rnc;

                        // Refrescar datos en FormSolicitudPago y seleccionar el nuevo
                        formPadre?.RefrescarFideicomisos(nuevoID);

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar fideicomiso: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ModificarFideicomiso()
        {
            try
            {
                string codigo = txtCodigo.Text.Trim();
                string nombre = txtNombre.Text.Trim();
                string rnc = Regex.Replace(txtRNC.Text, @"\D", string.Empty);
                int? tipoFideicomisoID = cboTipoFideicomiso.SelectedIndex >= 0
                    ? (int?)cboTipoFideicomiso.SelectedValue
                    : null;

                // Validar unicidad (excluyendo el registro actual)
                if (!ValidarUnicidad(codigo, nombre, rnc, fideicomisoIDCargado))
                    return;

                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE Fideicomisos 
                                    SET Codigo = @Codigo,
                                        Nombre = @Nombre,
                                        RNC = @RNC,
                                        TipoFideicomisoID = @TipoFideicomisoID,
                                        FechaModificacion = GETDATE(),
                                        ModificadoPorUsuarioID = @UsuarioID
                                    WHERE FideicomisoID = @FideicomisoID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FideicomisoID", fideicomisoIDCargado);
                        cmd.Parameters.AddWithValue("@Codigo", codigo);
                        cmd.Parameters.AddWithValue("@Nombre", nombre);
                        cmd.Parameters.AddWithValue("@RNC", rnc);
                        cmd.Parameters.AddWithValue("@TipoFideicomisoID",
                            tipoFideicomisoID.HasValue ? (object)tipoFideicomisoID.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@UsuarioID", SesionActual.UsuarioID);

                        cmd.ExecuteNonQuery();

                        // Auditoría
                        AuditoriaHelper.RegistrarAccion(
                            fideicomisoIDCargado,
                            "Fideicomisos",
                            "UPDATE",
                            $"Modificado: {codigo} - {nombre}"
                        );

                        // Guardar datos del resultado
                        FideicomisoIDResultado = fideicomisoIDCargado;
                        CodigoResultado = codigo;
                        NombreResultado = nombre;
                        RNCResultado = rnc;

                        // Refrescar datos en FormSolicitudPago y mantener seleccionado
                        formPadre?.RefrescarFideicomisos(fideicomisoIDCargado);

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar fideicomiso: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // ELIMINAR FIDEICOMISO (ELIMINACIÓN LÓGICA)
        // ═══════════════════════════════════════════════════════════════
        private void BtnEliminarFideicomiso_Click(object sender, EventArgs e)
        {
            if (!esModificacion || fideicomisoIDCargado == 0)
            {
                MessageBox.Show("Debe cargar un fideicomiso antes de eliminarlo.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"¿Está seguro que desea eliminar el fideicomiso '{txtNombre.Text}'?\n\nEsta acción no se puede deshacer.",
                "Confirmar Eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                EliminarFideicomiso();
            }
        }

        private void EliminarFideicomiso()
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE Fideicomisos 
                                    SET EsEliminado = 1,
                                        FechaEliminacion = GETDATE(),
                                        EliminadoPorUsuarioID = @UsuarioID
                                    WHERE FideicomisoID = @FideicomisoID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FideicomisoID", fideicomisoIDCargado);
                        cmd.Parameters.AddWithValue("@UsuarioID", SesionActual.UsuarioID);

                        cmd.ExecuteNonQuery();

                        // Auditoría
                        AuditoriaHelper.RegistrarAccion(
                            fideicomisoIDCargado,
                            "Fideicomisos",
                            "DELETE",
                            $"Eliminado: {txtCodigo.Text} - {txtNombre.Text}"
                        );

                        MessageBox.Show("Fideicomiso eliminado exitosamente.",
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Refrescar datos en FormSolicitudPago (esto quitará la selección si estaba seleccionado)
                        formPadre?.RefrescarFideicomisos(null);

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar fideicomiso: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // VALIDACIONES
        // ═══════════════════════════════════════════════════════════════
        private bool ValidarCampos()
        {
            // Código
            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("El código es obligatorio.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigo.Focus();
                return false;
            }

            // Nombre
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            // RNC
            string rnc = Regex.Replace(txtRNC.Text, @"\D", string.Empty);
            if (string.IsNullOrEmpty(rnc))
            {
                MessageBox.Show("El RNC es obligatorio.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRNC.Focus();
                return false;
            }

            if (rnc.Length != 9)
            {
                MessageBox.Show("El RNC debe tener 9 dígitos.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRNC.Focus();
                return false;
            }

            return true;
        }

        private bool ValidarUnicidad(string codigo, string nombre, string rnc, int? excluirFideicomisoID)
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Validar código único
                    string queryCodigo = @"SELECT COUNT(*) FROM Fideicomisos 
                                          WHERE Codigo = @Codigo 
                                          AND EsEliminado = 0 
                                          AND (@ExcluirID IS NULL OR FideicomisoID != @ExcluirID)";

                    using (SqlCommand cmd = new SqlCommand(queryCodigo, conn))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codigo);
                        cmd.Parameters.AddWithValue("@ExcluirID",
                            excluirFideicomisoID.HasValue ? (object)excluirFideicomisoID.Value : DBNull.Value);

                        int count = (int)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show($"Ya existe un fideicomiso con el código '{codigo}'.",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtCodigo.Focus();
                            return false;
                        }
                    }

                    // Validar nombre único
                    string queryNombre = @"SELECT COUNT(*) FROM Fideicomisos 
                                          WHERE Nombre = @Nombre 
                                          AND EsEliminado = 0 
                                          AND (@ExcluirID IS NULL OR FideicomisoID != @ExcluirID)";

                    using (SqlCommand cmd = new SqlCommand(queryNombre, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", nombre);
                        cmd.Parameters.AddWithValue("@ExcluirID",
                            excluirFideicomisoID.HasValue ? (object)excluirFideicomisoID.Value : DBNull.Value);

                        int count = (int)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show($"Ya existe un fideicomiso con el nombre '{nombre}'.",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtNombre.Focus();
                            return false;
                        }
                    }

                    // Validar RNC único
                    string queryRNC = @"SELECT COUNT(*) FROM Fideicomisos 
                                       WHERE RNC = @RNC 
                                       AND EsEliminado = 0 
                                       AND (@ExcluirID IS NULL OR FideicomisoID != @ExcluirID)";

                    using (SqlCommand cmd = new SqlCommand(queryRNC, conn))
                    {
                        cmd.Parameters.AddWithValue("@RNC", rnc);
                        cmd.Parameters.AddWithValue("@ExcluirID",
                            excluirFideicomisoID.HasValue ? (object)excluirFideicomisoID.Value : DBNull.Value);

                        int count = (int)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show($"Ya existe un fideicomiso con el RNC '{FormatRncDisplay(rnc)}'.",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtRNC.Focus();
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al validar unicidad: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // FORMATEO DE RNC
        // ═══════════════════════════════════════════════════════════════
        private void TxtRNC_Enter(object sender, EventArgs e)
        {
            // Quitar formato al entrar para facilitar edición
            string digits = Regex.Replace(txtRNC.Text, @"\D", string.Empty);
            txtRNC.Text = digits;
            txtRNC.SelectionStart = txtRNC.Text.Length;
        }

        private void TxtRNC_Leave(object sender, EventArgs e)
        {
            // Aplicar formato al salir
            string digits = Regex.Replace(txtRNC.Text, @"\D", string.Empty);

            if (string.IsNullOrEmpty(digits))
            {
                txtRNC.Text = string.Empty;
                return;
            }

            txtRNC.Text = FormatRncDisplay(digits);
        }

        private string FormatRncDisplay(string digits)
        {
            if (string.IsNullOrEmpty(digits))
                return string.Empty;

            // Mantener solo dígitos
            digits = Regex.Replace(digits, @"\D", string.Empty);

            if (digits.Length == 9)
            {
                // Formato: 1-31-12345-6
                return $"{digits.Substring(0, 1)}-{digits.Substring(1, 2)}-{digits.Substring(3, 5)}-{digits.Substring(8, 1)}";
            }

            // Fallback: devolver solo dígitos
            return digits;
        }

        // ═══════════════════════════════════════════════════════════════
        // DETECTAR CAMBIOS PARA DETERMINAR MODO
        // ═══════════════════════════════════════════════════════════════
        private void Txt_CambioDetectado(object sender, EventArgs e)
        {
            if (esModificacion)
            {
                string codigoActual = txtCodigo.Text.Trim();
                string rncActual = Regex.Replace(txtRNC.Text, @"\D", string.Empty);

                // Actualizar texto del botón según el tipo de operación
                bool cambioCodigoYRNC = (codigoActual != codigoOriginal && rncActual != rncOriginal);

                if (cambioCodigoYRNC)
                {
                    btnGuardarFideicomiso.Text = "💾 Guardar como Nuevo";
                    btnGuardarFideicomiso.BackColor = Color.FromArgb(40, 167, 69);
                }
                else
                {
                    btnGuardarFideicomiso.Text = "✏️ Modificar";
                    btnGuardarFideicomiso.BackColor = Color.FromArgb(255, 193, 7);
                }
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // ESTADOS DEL FORMULARIO
        // ═══════════════════════════════════════════════════════════════
        private void EstablecerModoNuevo()
        {
            esModificacion = false;
            fideicomisoIDCargado = 0;
            codigoOriginal = string.Empty;
            rncOriginal = string.Empty;

            txtCodigo.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtRNC.Text = string.Empty;
            cboTipoFideicomiso.SelectedIndex = -1;

            btnGuardarFideicomiso.Text = "💾 Guardar";
            btnGuardarFideicomiso.BackColor = Color.FromArgb(40, 167, 69);
            btnEliminarFideicomiso.Enabled = false;
            btnEliminarFideicomiso.BackColor = SystemColors.Control;

            txtCodigo.Focus();
        }

        private void EstablecerModoModificacion()
        {
            esModificacion = true;

            btnGuardarFideicomiso.Text = "✏️ Modificar";
            btnGuardarFideicomiso.BackColor = Color.FromArgb(255, 193, 7);
            btnEliminarFideicomiso.Enabled = true;
            btnEliminarFideicomiso.BackColor = Color.FromArgb(220, 53, 69);

            txtNombre.Focus();
        }

        // ═══════════════════════════════════════════════════════════════
        // VALIDADOR NUMÉRICO
        // ═══════════════════════════════════════════════════════════════
        private void NumericInteger_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // CANCELAR
        // ═══════════════════════════════════════════════════════════════
        private void BtnCancelarFideicomiso_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}