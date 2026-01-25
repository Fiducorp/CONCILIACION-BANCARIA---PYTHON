using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MOFIS_ERP.Classes;

namespace MOFIS_ERP.Forms.Contabilidad.CuentasPorPagar.CartasSolicitudes.Solicitud_de_pago
{
    public partial class FormAgregarProveedor : Form
    {
        private readonly FormSolicitudPago parentForm;
        private int? currentProveedorID = null;
        private string originalNumeroDocumentoClean = null;
        private bool modoEdicion = false;

        public FormAgregarProveedor(FormSolicitudPago parent)
        {
            InitializeComponent();
            parentForm = parent;
            ConfigurarEventos();
        }

        private void FormAgregarProveedor_Load(object sender, EventArgs e)
        {
            CargarTiposDocumento();
            CargarSiguienteProveedorID();
            modoEdicion = false;
            currentProveedorID = null;
            originalNumeroDocumentoClean = null;

            // Inicialmente deshabilitar eliminar
            if (btnEliminarProveedor != null)
                btnEliminarProveedor.Enabled = false;

            // Limpiar errores
            if (errProviderProveedor != null)
                errProviderProveedor.Clear();
        }

        private void ConfigurarEventos()
        {
            this.Load += FormAgregarProveedor_Load;

            if (txtTelefonoProv != null)
            {
                txtTelefonoProv.Enter += TxtTelefonoProv_Enter;
                txtTelefonoProv.Leave += TxtTelefonoProv_Leave;
            }

            if (txtNumeroDocumento != null)
            {
                txtNumeroDocumento.Enter += TxtNumeroDocumento_Enter;
                txtNumeroDocumento.Leave += TxtNumeroDocumento_Leave;
            }

            if (btnBuscarNumero != null) btnBuscarNumero.Click += BtnBuscarNumero_Click;
            if (btnEliminarProveedor != null) btnEliminarProveedor.Click += BtnEliminarProveedor_Click;
            if (btnGuardarProveedor != null) btnGuardarProveedor.Click += BtnGuardarProveedor_Click;
            if (btnCancelarProveedor != null) btnCancelarProveedor.Click += BtnCancelarProveedor_Click;
        }

        private void CargarTiposDocumento()
        {
            cboTipoDocumento.Items.Clear();
            cboTipoDocumento.Items.Add("RNC");
            cboTipoDocumento.Items.Add("CEDULA");
            // No seleccionar ninguno por defecto
            cboTipoDocumento.SelectedIndex = -1;
        }

        private void CargarSiguienteProveedorID()
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string q = "SELECT ISNULL(MAX(ProveedorID),0) + 1 FROM Proveedores";
                    using (SqlCommand cmd = new SqlCommand(q, conn))
                    {
                        int siguiente = Convert.ToInt32(cmd.ExecuteScalar());
                        txtIdProveedor.Text = $"PRV-{siguiente:D6}";
                    }
                }
            }
            catch
            {
                txtIdProveedor.Text = "PRV-000001";
            }
        }

        // Teléfono: quitar formato en Enter, validar y formatear en Leave
        private void TxtTelefonoProv_Enter(object sender, EventArgs e)
        {
            if (txtTelefonoProv == null) return;
            txtTelefonoProv.Text = Regex.Replace(txtTelefonoProv.Text ?? string.Empty, @"\D", string.Empty);
            txtTelefonoProv.SelectionStart = txtTelefonoProv.Text.Length;
            if (errProviderProveedor != null) errProviderProveedor.SetError(txtTelefonoProv, string.Empty);
        }

        private void TxtTelefonoProv_Leave(object sender, EventArgs e)
        {
            if (txtTelefonoProv == null) return;

            string cleaned = Regex.Replace(txtTelefonoProv.Text ?? string.Empty, @"\D", string.Empty);

            // Si se ingresó algo, debe ser exactamente 10 dígitos
            if (!string.IsNullOrEmpty(cleaned))
            {
                if (cleaned.Length != 10)
                {
                    if (errProviderProveedor != null)
                        errProviderProveedor.SetError(txtTelefonoProv, "Teléfono debe tener exactamente 10 dígitos.");
                }
                else
                {
                    if (errProviderProveedor != null)
                        errProviderProveedor.SetError(txtTelefonoProv, string.Empty);

                    txtTelefonoProv.Text = FormatPhone(cleaned);
                }
            }
            else
            {
                // Campo vacío -> limpiar error y valor
                if (errProviderProveedor != null) errProviderProveedor.SetError(txtTelefonoProv, string.Empty);
                txtTelefonoProv.Text = string.Empty;
            }
        }

        private string FormatPhone(string digits)
        {
            if (string.IsNullOrWhiteSpace(digits)) return string.Empty;
            string d = Regex.Replace(digits, @"\D", string.Empty);
            if (d.Length == 10)
                return $"({d.Substring(0,3)}) {d.Substring(3,3)}-{d.Substring(6,4)}";
            if (d.Length == 7)
                return $"{d.Substring(0,3)}-{d.Substring(3,4)}";
            return digits;
        }

        // Documento: quitar formato en Enter, validar/formatear en Leave
        private void TxtNumeroDocumento_Enter(object sender, EventArgs e)
        {
            if (txtNumeroDocumento == null) return;
            txtNumeroDocumento.Text = Regex.Replace(txtNumeroDocumento.Text ?? string.Empty, @"\D", string.Empty);
            txtNumeroDocumento.SelectionStart = txtNumeroDocumento.Text.Length;
            if (errProviderProveedor != null) errProviderProveedor.SetError(txtNumeroDocumento, string.Empty);
        }

        private void TxtNumeroDocumento_Leave(object sender, EventArgs e)
        {
            if (txtNumeroDocumento == null || cboTipoDocumento == null) return;

            string rawDigits = Regex.Replace(txtNumeroDocumento.Text ?? string.Empty, @"\D", string.Empty);

            // Validar según tipo seleccionado (si existe selección)
            string tipoSel = cboTipoDocumento.SelectedItem?.ToString().ToUpper() ?? string.Empty;
            if (!string.IsNullOrEmpty(tipoSel))
            {
                if (tipoSel == "RNC")
                {
                    if (!string.IsNullOrEmpty(rawDigits) && rawDigits.Length != 9)
                    {
                        if (errProviderProveedor != null) errProviderProveedor.SetError(txtNumeroDocumento, "RNC debe contener exactamente 9 dígitos.");
                    }
                    else if (errProviderProveedor != null)
                    {
                        errProviderProveedor.SetError(txtNumeroDocumento, string.Empty);
                    }
                }
                else if (tipoSel == "CEDULA")
                {
                    if (!string.IsNullOrEmpty(rawDigits) && rawDigits.Length != 11)
                    {
                        if (errProviderProveedor != null) errProviderProveedor.SetError(txtNumeroDocumento, "Cédula debe contener exactamente 11 dígitos.");
                    }
                    else if (errProviderProveedor != null)
                    {
                        errProviderProveedor.SetError(txtNumeroDocumento, string.Empty);
                    }
                }
            }
            else
            {
                // Sin tipo seleccionado: limpiar validación previa
                if (errProviderProveedor != null) errProviderProveedor.SetError(txtNumeroDocumento, string.Empty);
            }

            txtNumeroDocumento.Text = FormatDocumentoDisplay(rawDigits);
        }

        private string FormatDocumentoDisplay(string digits)
        {
            if (string.IsNullOrEmpty(digits)) return string.Empty;
            digits = Regex.Replace(digits, @"\D", string.Empty);
            if (digits.Length == 11)
                return $"{digits.Substring(0,3)}-{digits.Substring(3,7)}-{digits.Substring(10,1)}";
            if (digits.Length == 9)
                return $"{digits.Substring(0,1)}-{digits.Substring(1,2)}-{digits.Substring(3,5)}-{digits.Substring(8,1)}";
            return digits;
        }

        // Buscar proveedor por numero de documento (no cierra el formulario)
        private void BtnBuscarNumero_Click(object sender, EventArgs e)
        {
            if (txtNumeroDocumento == null) return;

            string documentoClean = Regex.Replace(txtNumeroDocumento.Text ?? string.Empty, @"\D", string.Empty);
            if (string.IsNullOrEmpty(documentoClean))
            {
                MessageBox.Show("Ingrese RNC o Cédula para buscar.", "Buscar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string q = @"
                        SELECT ProveedorID, Nombre, TipoDocumento, NumeroDocumento, Telefono, Email, CreadoPorUsuarioID, FechaCreacion
                        FROM Proveedores
                        WHERE (REPLACE(NumeroDocumento,'-','') = @num OR NumeroDocumento = @raw)
                          AND EsEliminado = 0";
                    using (SqlCommand cmd = new SqlCommand(q, conn))
                    {
                        cmd.Parameters.AddWithValue("@num", documentoClean);
                        cmd.Parameters.AddWithValue("@raw", txtNumeroDocumento.Text.Trim());
                        using (var rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                currentProveedorID = Convert.ToInt32(rdr["ProveedorID"]);
                                modoEdicion = true;
                                txtIdProveedor.Text = $"PRV-{currentProveedorID.Value:D6}";
                                txtNombreProv.Text = rdr["Nombre"]?.ToString();
                                string tipo = (rdr["TipoDocumento"]?.ToString() ?? "R").ToUpper();
                                cboTipoDocumento.SelectedItem = tipo == "C" ? "CEDULA" : "RNC";
                                string numeroRaw = (rdr["NumeroDocumento"]?.ToString() ?? string.Empty);
                                txtNumeroDocumento.Text = FormatDocumentoDisplay(Regex.Replace(numeroRaw, @"\D", string.Empty));
                                string telefono = (rdr["Telefono"]?.ToString() ?? string.Empty);
                                txtTelefonoProv.Text = string.IsNullOrEmpty(telefono) ? string.Empty : Regex.Replace(telefono, @"\D", string.Empty);
                                // Forzar formato si correcto
                                if (!string.IsNullOrEmpty(txtTelefonoProv.Text) && txtTelefonoProv.Text.Length == 10)
                                    txtTelefonoProv.Text = FormatPhone(txtTelefonoProv.Text);
                                txtEmailProv.Text = rdr["Email"]?.ToString();
                                originalNumeroDocumentoClean = Regex.Replace(numeroRaw, @"\D", string.Empty);

                                // Habilitar eliminar
                                if (btnEliminarProveedor != null)
                                    btnEliminarProveedor.Enabled = true;

                                // limpiar errores previos
                                if (errProviderProveedor != null) errProviderProveedor.Clear();
                            }
                            else
                            {
                                MessageBox.Show("Proveedor no encontrado.", "Buscar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error buscando proveedor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Eliminar proveedor (eliminación lógica), no cierra el formulario
        private void BtnEliminarProveedor_Click(object sender, EventArgs e)
        {
            if (!currentProveedorID.HasValue)
            {
                MessageBox.Show("Debe cargar un proveedor antes de eliminar.", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dr = MessageBox.Show("¿Confirma que desea eliminar este proveedor? Esta acción es lógica y quedará registrada en auditoría.", "Eliminar proveedor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr != DialogResult.Yes) return;

            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string q = @"
                        UPDATE Proveedores
                        SET EsEliminado = 1,
                            Activo = 0,
                            FechaEliminacion = GETDATE(),
                            EliminadoPorUsuarioID = @usuarioID
                        WHERE ProveedorID = @id";
                    using (SqlCommand cmd = new SqlCommand(q, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", currentProveedorID.Value);
                        cmd.Parameters.AddWithValue("@usuarioID", SesionActual.UsuarioID);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Registrar auditoría
                AuditoriaHelper.RegistrarAccion(SesionActual.UsuarioID, "ELIMINAR", "CONTABILIDAD", "Cuentas por Pagar", "FormAgregarProveedor", currentProveedorID, $"ProveedorID={currentProveedorID}");

                MessageBox.Show("Proveedor eliminado correctamente.", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refrescar la lista en el formulario padre y limpiar selección en este mini-form
                if (parentForm != null)
                {
                    parentForm.BeginInvoke(new Action(() => parentForm.RefrescarProveedores(null)));
                }

                // Limpiar formulario para nuevas operaciones
                CargarSiguienteProveedorID();
                txtNombreProv.Text = string.Empty;
                txtNumeroDocumento.Text = string.Empty;
                txtTelefonoProv.Text = string.Empty;
                txtEmailProv.Text = string.Empty;
                currentProveedorID = null;
                modoEdicion = false;
                originalNumeroDocumentoClean = null;

                // Deshabilitar eliminar
                if (btnEliminarProveedor != null)
                    btnEliminarProveedor.Enabled = false;

                if (errProviderProveedor != null) errProviderProveedor.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar proveedor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Guardar nuevo o modificar existente (según reglas solicitadas)
        private void BtnGuardarProveedor_Click(object sender, EventArgs e)
        {
            // Limpiar errores iniciales
            if (errProviderProveedor != null) errProviderProveedor.Clear();

            string nombre = (txtNombreProv.Text ?? string.Empty).Trim();
            string telefonoClean = Regex.Replace(txtTelefonoProv.Text ?? string.Empty, @"\D", string.Empty);
            string email = (txtEmailProv.Text ?? string.Empty).Trim();
            string numeroClean = Regex.Replace(txtNumeroDocumento.Text ?? string.Empty, @"\D", string.Empty);
            string tipoSeleccionado = (cboTipoDocumento.SelectedItem ?? string.Empty).ToString().ToUpper();
            string tipoDb = tipoSeleccionado == "CEDULA" ? "C" : "R"; // si no seleccionado, queda 'R' por fallback

            bool valid = true;

            if (string.IsNullOrEmpty(nombre))
            {
                if (errProviderProveedor != null) errProviderProveedor.SetError(txtNombreProv, "Nombre es obligatorio.");
                valid = false;
            }

            if (string.IsNullOrEmpty(numeroClean))
            {
                if (errProviderProveedor != null) errProviderProveedor.SetError(txtNumeroDocumento, "Número de documento es obligatorio.");
                valid = false;
            }
            else
            {
                // Validar según tipo seleccionado
                if (!string.IsNullOrEmpty(tipoSeleccionado))
                {
                    if (tipoDb == "R" && numeroClean.Length != 9)
                    {
                        if (errProviderProveedor != null) errProviderProveedor.SetError(txtNumeroDocumento, "RNC debe contener 9 dígitos.");
                        valid = false;
                    }
                    if (tipoDb == "C" && numeroClean.Length != 11)
                    {
                        if (errProviderProveedor != null) errProviderProveedor.SetError(txtNumeroDocumento, "Cédula debe contener 11 dígitos.");
                        valid = false;
                    }
                }
            }

            // Validar teléfono (si se ingresó debe ser exactamente 10)
            if (!string.IsNullOrEmpty(telefonoClean) && telefonoClean.Length != 10)
            {
                if (errProviderProveedor != null) errProviderProveedor.SetError(txtTelefonoProv, "Teléfono debe tener exactamente 10 dígitos.");
                valid = false;
            }

            // Validar email si existe
            if (!string.IsNullOrEmpty(email) && !IsValidEmail(email))
            {
                if (errProviderProveedor != null) errProviderProveedor.SetError(txtEmailProv, "Correo electrónico inválido.");
                valid = false;
            }

            if (!valid) return;

            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Verificar unicidad del numero de documento contra otros registros activos
                    string qCheck = "SELECT ProveedorID FROM Proveedores WHERE REPLACE(NumeroDocumento,'-','') = @num AND EsEliminado = 0";
                    using (SqlCommand cmdCheck = new SqlCommand(qCheck, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@num", numeroClean);
                        object found = cmdCheck.ExecuteScalar();
                        int foundId = found == null || found == DBNull.Value ? 0 : Convert.ToInt32(found);

                        if (modoEdicion && currentProveedorID.HasValue)
                        {
                            if (foundId != 0 && foundId != currentProveedorID.Value)
                            {
                                if (errProviderProveedor != null) errProviderProveedor.SetError(txtNumeroDocumento, "El número de documento ya está registrado para otro proveedor.");
                                return;
                            }
                        }
                        else
                        {
                            if (foundId != 0)
                            {
                                if (errProviderProveedor != null) errProviderProveedor.SetError(txtNumeroDocumento, "El número de documento ya está registrado.");
                                return;
                            }
                        }
                    }

                    // Determinar si tratar como nuevo cuando en modo edición se cambió el documento
                    bool tratarComoNuevo = false;
                    if (modoEdicion && currentProveedorID.HasValue)
                    {
                        if (!string.Equals(originalNumeroDocumentoClean ?? string.Empty, numeroClean, StringComparison.Ordinal))
                        {
                            tratarComoNuevo = true;
                        }
                    }

                    if (modoEdicion && currentProveedorID.HasValue && !tratarComoNuevo)
                    {
                        // Actualizar existente
                        string qUpd = @"
                            UPDATE Proveedores
                            SET Nombre = @nombre,
                                TipoDocumento = @tipo,
                                NumeroDocumento = @numero,
                                Telefono = @telefono,
                                Email = @email,
                                FechaModificacion = GETDATE(),
                                ModificadoPorUsuarioID = @usuarioID
                            WHERE ProveedorID = @id";
                        using (SqlCommand cmdUpd = new SqlCommand(qUpd, conn))
                        {
                            cmdUpd.Parameters.AddWithValue("@nombre", nombre);
                            cmdUpd.Parameters.AddWithValue("@tipo", tipoDb);
                            cmdUpd.Parameters.AddWithValue("@numero", numeroClean);
                            cmdUpd.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(telefonoClean) ? (object)DBNull.Value : telefonoClean);
                            cmdUpd.Parameters.AddWithValue("@email", string.IsNullOrEmpty(email) ? (object)DBNull.Value : email);
                            cmdUpd.Parameters.AddWithValue("@usuarioID", SesionActual.UsuarioID);
                            cmdUpd.Parameters.AddWithValue("@id", currentProveedorID.Value);
                            cmdUpd.ExecuteNonQuery();
                        }

                        // Auditoría
                        AuditoriaHelper.RegistrarAccion(SesionActual.UsuarioID, "EDITAR", "CONTABILIDAD", "Cuentas por Pagar", "FormAgregarProveedor", currentProveedorID, $"ProveedorID={currentProveedorID}; Nombre={nombre}; Numero={numeroClean}");

                        if (parentForm != null)
                        {
                            int idToSelect = currentProveedorID.Value;
                            parentForm.BeginInvoke(new Action(() => parentForm.RefrescarProveedores(idToSelect)));
                        }

                        MessageBox.Show("Proveedor actualizado correctamente.", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        return;
                    }
                    else
                    {
                        // Insertar nuevo registro
                        string qIns = @"
                            INSERT INTO Proveedores (Nombre, TipoDocumento, NumeroDocumento, Telefono, Email, Activo, FechaCreacion, CreadoPorUsuarioID)
                            VALUES (@nombre, @tipo, @numero, @telefono, @email, 1, GETDATE(), @usuarioID);
                            SELECT SCOPE_IDENTITY();";
                        using (SqlCommand cmdIns = new SqlCommand(qIns, conn))
                        {
                            cmdIns.Parameters.AddWithValue("@nombre", nombre);
                            cmdIns.Parameters.AddWithValue("@tipo", tipoDb);
                            cmdIns.Parameters.AddWithValue("@numero", numeroClean);
                            cmdIns.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(telefonoClean) ? (object)DBNull.Value : telefonoClean);
                            cmdIns.Parameters.AddWithValue("@email", string.IsNullOrEmpty(email) ? (object)DBNull.Value : email);
                            cmdIns.Parameters.AddWithValue("@usuarioID", SesionActual.UsuarioID);

                            object res = cmdIns.ExecuteScalar();
                            int newId = Convert.ToInt32(Convert.ToDecimal(res));

                            // Auditoría
                            AuditoriaHelper.RegistrarAccion(SesionActual.UsuarioID, "CREAR", "CONTABILIDAD", "Cuentas por Pagar", "FormAgregarProveedor", newId, $"ProveedorID={newId}; Nombre={nombre}; Numero={numeroClean}");

                            if (parentForm != null)
                            {
                                parentForm.BeginInvoke(new Action(() => parentForm.RefrescarProveedores(newId)));
                            }

                            MessageBox.Show("Proveedor guardado correctamente.", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error guardando proveedor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancelarProveedor_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var m = Regex.Match(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
                return m.Success;
            }
            catch
            {
                return false;
            }
        }
    }
}