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
            cboTipoFideicomiso.SelectedIndexChanged += (s, e) => Txt_CambioDetectado(s, e);
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
                if (errProviderFideicomiso != null)
                    errProviderFideicomiso.SetError(btnGuardarFideicomiso, $"Error cargando tipos: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // BUSCAR FIDEICOMISO POR CÓDIGO
        // ═══════════════════════════════════════════════════════════════
        private void BtnBuscarCodigo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                if (errProviderFideicomiso != null) errProviderFideicomiso.SetError(txtCodigo, "Ingrese un código para buscar.");
                txtCodigo.Focus();
                return;
            }

            if (errProviderFideicomiso != null) errProviderFideicomiso.SetError(txtCodigo, string.Empty);
            BuscarYCargarFideicomiso(txtCodigo.Text.Trim());
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
                                if (errProviderFideicomiso != null) errProviderFideicomiso.Clear();

                                MessageBox.Show("Fideicomiso cargado correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show($"No se encontró un fideicomiso con el código '{codigo}'.", "No encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (errProviderFideicomiso != null)
                    errProviderFideicomiso.SetError(btnBuscarCodigo, $"Error al buscar: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // GUARDAR FIDEICOMISO (NUEVO O MODIFICACIÓN)
        // ═══════════════════════════════════════════════════════════════
        private void BtnGuardarFideicomiso_Click(object sender, EventArgs e)
        {
            // Limpiar errores previos
            if (errProviderFideicomiso != null) errProviderFideicomiso.Clear();

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
                DialogResult result = MessageBox.Show("Al cambiar tanto el código como el RNC, se creará un NUEVO fideicomiso.\n\n¿Desea continuar?", "Nuevo Registro", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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
                string nombre = txtNombre.Text.Trim().ToUpperInvariant();
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

                        // Auditoría (registro coherente)
                        AuditoriaHelper.RegistrarAccion(
                            SesionActual.UsuarioID,
                            "CREAR",
                            "CONTABILIDAD",
                            "Cuentas por Pagar",
                            "FormAgregarFideicomiso",
                            nuevoID,
                            $"Codigo={codigo}; Nombre={nombre}; RNC={rnc}; TipoFideicomisoID={(tipoFideicomisoID.HasValue ? tipoFideicomisoID.Value.ToString() : "")}"
                        );

                          // Guardar datos del resultado
                        FideicomisoIDResultado = nuevoID;
                        CodigoResultado = codigo;
                        NombreResultado = nombre;
                        RNCResultado = rnc;

                        // Refrescar datos en FormSolicitudPago y seleccionar el nuevo
                        formPadre?.RefrescarFideicomisos(nuevoID);

                        MessageBox.Show("Fideicomiso guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                if (errProviderFideicomiso != null)
                    errProviderFideicomiso.SetError(btnGuardarFideicomiso, $"Error guardando: {ex.Message}");
            }
        }

        private void ModificarFideicomiso()
        {
            try
            {
                string codigo = txtCodigo.Text.Trim();
                string nombre = txtNombre.Text.Trim().ToUpperInvariant();
                string rnc = Regex.Replace(txtRNC.Text, @"\D", string.Empty);
                int? tipoFideicomisoID = cboTipoFideicomiso.SelectedIndex >= 0
                    ? (int?)cboTipoFideicomiso.SelectedValue
                    : null;

                if (!ValidarUnicidad(codigo, nombre, rnc, fideicomisoIDCargado))
                    return;

                // Leer fila completa antes de actualizar
                string antesCodigo = string.Empty, antesNombre = string.Empty, antesRNC = string.Empty;
                string antesTipoID = string.Empty;
                try
                {
                    using (SqlConnection conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("SELECT Codigo, Nombre, RNC, TipoFideicomisoID FROM Fideicomisos WHERE FideicomisoID = @ID", conn))
                        {
                            cmd.Parameters.AddWithValue("@ID", fideicomisoIDCargado);
                            using (var rdr = cmd.ExecuteReader())
                            {
                                if (rdr.Read())
                                {
                                    antesCodigo = rdr["Codigo"]?.ToString() ?? string.Empty;
                                    antesNombre = rdr["Nombre"]?.ToString() ?? string.Empty;
                                    antesRNC = Regex.Replace(rdr["RNC"]?.ToString() ?? string.Empty, @"\D", string.Empty);
                                    antesTipoID = rdr["TipoFideicomisoID"] == DBNull.Value ? "" : rdr["TipoFideicomisoID"].ToString();
                                }
                            }
                        }
                    }
                }
                catch
                {
                    if (errProviderFideicomiso != null) errProviderFideicomiso.SetError(btnGuardarFideicomiso, "Error leyendo valores previos (auditoría).");
                }

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

                        // Preparar valores para auditoría fuera de la interpolación
                        string despuesTipoID = tipoFideicomisoID.HasValue ? tipoFideicomisoID.Value.ToString() : string.Empty;
                        string detalle = $"Antes: Codigo={antesCodigo}; Nombre={antesNombre}; RNC={antesRNC}; TipoFideicomisoID={antesTipoID} | " +
                                         $"Después: Codigo={codigo}; Nombre={nombre}; RNC={rnc}; TipoFideicomisoID={despuesTipoID}";

                        AuditoriaHelper.RegistrarAccion(
                            SesionActual.UsuarioID,
                            "EDITAR",
                            "CONTABILIDAD",
                            "Cuentas por Pagar",
                            "FormAgregarFideicomiso",
                            fideicomisoIDCargado,
                            detalle
                        );

                        FideicomisoIDResultado = fideicomisoIDCargado;
                        CodigoResultado = codigo;
                        NombreResultado = nombre;
                        RNCResultado = rnc;

                        formPadre?.RefrescarFideicomisos(fideicomisoIDCargado);

                        MessageBox.Show("Fideicomiso actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                if (errProviderFideicomiso != null)
                    errProviderFideicomiso.SetError(btnGuardarFideicomiso, $"Error modificando: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // ELIMINAR FIDEICOMISO (ELIMINACIÓN LÓGICA)
        // ═══════════════════════════════════════════════════════════════
        private void BtnEliminarFideicomiso_Click(object sender, EventArgs e)
        {
            if (!esModificacion || fideicomisoIDCargado == 0)
            {
                if (errProviderFideicomiso != null) errProviderFideicomiso.SetError(btnEliminarFideicomiso, "Debe cargar un fideicomiso antes de eliminar.");
                return;
            }

            DialogResult result = MessageBox.Show($"¿Está seguro que desea eliminar el fideicomiso '{txtNombre.Text}'?\n\nEsta acción no se puede deshacer.", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

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

                        // Preparar valores para auditoría evitando escapes dentro de la interpolación
                        string codigo = txtCodigo?.Text ?? string.Empty;
                        string nombre = txtNombre?.Text ?? string.Empty;
                        string rncClean = Regex.Replace(txtRNC?.Text ?? string.Empty, "\\D", string.Empty);

                        AuditoriaHelper.RegistrarAccion(
                            SesionActual.UsuarioID,
                            "ELIMINAR",
                            "CONTABILIDAD",
                            "Cuentas por Pagar",
                            "FormAgregarFideicomiso",
                            fideicomisoIDCargado,
                            $"Eliminado: Codigo={codigo}; Nombre={nombre}; RNC={rncClean}"
                        );

                        MessageBox.Show("Fideicomiso eliminado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Refrescar datos en FormSolicitudPago (esto quitará la selección si estaba seleccionado)
                        formPadre?.RefrescarFideicomisos(null);

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                if (errProviderFideicomiso != null)
                    errProviderFideicomiso.SetError(btnEliminarFideicomiso, $"Error eliminando: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // VALIDACIONES
        // ═══════════════════════════════════════════════════════════════
        private bool ValidarCampos()
        {
            bool valido = true;
            if (errProviderFideicomiso != null) errProviderFideicomiso.Clear();

            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                if (errProviderFideicomiso != null) errProviderFideicomiso.SetError(txtCodigo, "El código es obligatorio.");
                txtCodigo.Focus();
                valido = false;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                if (errProviderFideicomiso != null) errProviderFideicomiso.SetError(txtNombre, "El nombre es obligatorio.");
                if (valido) txtNombre.Focus();
                valido = false;
            }

            string rnc = Regex.Replace(txtRNC.Text, @"\D", string.Empty);
            if (string.IsNullOrEmpty(rnc))
            {
                if (errProviderFideicomiso != null) errProviderFideicomiso.SetError(txtRNC, "El RNC es obligatorio.");
                if (valido) txtRNC.Focus();
                valido = false;
            }
            else if (rnc.Length != 9)
            {
                if (errProviderFideicomiso != null) errProviderFideicomiso.SetError(txtRNC, "El RNC debe tener 9 dígitos.");
                if (valido) txtRNC.Focus();
                valido = false;
            }

            return valido;
        }

        private bool ValidarUnicidad(string codigo, string nombre, string rnc, int? excluirFideicomisoID)
        {
            try
            {
                if (errProviderFideicomiso != null) errProviderFideicomiso.Clear();

                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string queryCodigo = @"SELECT COUNT(*) FROM Fideicomisos 
                                          WHERE Codigo = @Codigo 
                                          AND EsEliminado = 0 
                                          AND (@ExcluirID IS NULL OR FideicomisoID != @ExcluirID)";

                    using (SqlCommand cmd = new SqlCommand(queryCodigo, conn))
                    {
                        cmd.Parameters.AddWithValue("@Codigo", codigo);
                        cmd.Parameters.AddWithValue("@ExcluirID", excluirFideicomisoID.HasValue ? (object)excluirFideicomisoID.Value : DBNull.Value);

                        int count = (int)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            if (errProviderFideicomiso != null) errProviderFideicomiso.SetError(txtCodigo, $"Ya existe un fideicomiso con el código '{codigo}'.");
                            txtCodigo.Focus();
                            return false;
                        }
                    }

                    string queryNombre = @"SELECT COUNT(*) FROM Fideicomisos 
                                          WHERE Nombre = @Nombre 
                                          AND EsEliminado = 0 
                                          AND (@ExcluirID IS NULL OR FideicomisoID != @ExcluirID)";

                    using (SqlCommand cmd = new SqlCommand(queryNombre, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", nombre);
                        cmd.Parameters.AddWithValue("@ExcluirID", excluirFideicomisoID.HasValue ? (object)excluirFideicomisoID.Value : DBNull.Value);

                        int count = (int)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            if (errProviderFideicomiso != null) errProviderFideicomiso.SetError(txtNombre, $"Ya existe un fideicomiso con el nombre '{nombre}'.");
                            txtNombre.Focus();
                            return false;
                        }
                    }

                    string queryRNC = @"SELECT COUNT(*) FROM Fideicomisos 
                                       WHERE RNC = @RNC 
                                       AND EsEliminado = 0 
                                       AND (@ExcluirID IS NULL OR FideicomisoID != @ExcluirID)";

                    using (SqlCommand cmd = new SqlCommand(queryRNC, conn))
                    {
                        cmd.Parameters.AddWithValue("@RNC", rnc);
                        cmd.Parameters.AddWithValue("@ExcluirID", excluirFideicomisoID.HasValue ? (object)excluirFideicomisoID.Value : DBNull.Value);

                        int count = (int)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            if (errProviderFideicomiso != null) errProviderFideicomiso.SetError(txtRNC, $"Ya existe un fideicomiso con el RNC '{FormatRncDisplay(rnc)}'.");
                            txtRNC.Focus();
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                if (errProviderFideicomiso != null)
                    errProviderFideicomiso.SetError(btnGuardarFideicomiso, $"Error al validar unicidad: {ex.Message}");
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
            if (string.IsNullOrEmpty(digits)) return string.Empty;
            digits = Regex.Replace(digits, @"\D", string.Empty);
            if (digits.Length == 9) return $"{digits.Substring(0, 1)}-{digits.Substring(1, 2)}-{digits.Substring(3, 5)}-{digits.Substring(8, 1)}";
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

            if (errProviderFideicomiso != null) errProviderFideicomiso.Clear();
        }

        private void EstablecerModoModificacion()
        {
            esModificacion = true;

            btnGuardarFideicomiso.Text = "✏️ Modificar";
            btnGuardarFideicomiso.BackColor = Color.FromArgb(255, 193, 7);
            btnEliminarFideicomiso.Enabled = true;
            btnEliminarFideicomiso.BackColor = Color.FromArgb(220, 53, 69);

            txtNombre.Focus();

            if (errProviderFideicomiso != null) errProviderFideicomiso.Clear();
        }

        // ═══════════════════════════════════════════════════════════════
        // VALIDADOR NUMÉRICO
        // ═══════════════════════════════════════════════════════════════
        private void NumericInteger_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
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