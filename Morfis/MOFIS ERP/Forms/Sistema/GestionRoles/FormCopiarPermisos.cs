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
    public partial class FormCopiarPermisos : Form
    {
        public bool PermisosCopiados { get; private set; }
        private int rolOrigenID = 0;
        private int rolDestinoID = 0;

        public FormCopiarPermisos()
        {
            InitializeComponent();
            ConfigurarFormulario();
            CargarRoles();
        }

        private void ConfigurarFormulario()
        {
            // Estilos de botones
            btnCopiar.FlatAppearance.BorderSize = 0;
            btnCancelar.FlatAppearance.BorderSize = 0;

            // Eventos
            btnCopiar.Click += BtnCopiar_Click;
            btnCancelar.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            cmbRolOrigen.SelectedIndexChanged += CmbRolOrigen_SelectedIndexChanged;
            cmbRolDestino.SelectedIndexChanged += CmbRolDestino_SelectedIndexChanged;

            // Navegación con teclado
            this.AcceptButton = btnCopiar;
            this.CancelButton = btnCancelar;
        }

        private void CargarRoles()
        {
            try
            {
                cmbRolOrigen.Items.Clear();
                cmbRolDestino.Items.Clear();

                cmbRolOrigen.Items.Add("Seleccione un rol origen...");
                cmbRolDestino.Items.Add("Seleccione un rol destino...");

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"SELECT RolID, NombreRol, EsSistema
                                  FROM Roles
                                  WHERE Activo = 1
                                  ORDER BY EsSistema DESC, NombreRol";

                    using (var cmd = new SqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new ComboBoxRol
                            {
                                RolID = reader.GetInt32(0),
                                NombreRol = reader.GetString(1),
                                EsSistema = reader.GetBoolean(2)
                            };

                            cmbRolOrigen.Items.Add(item);
                            cmbRolDestino.Items.Add(item);
                        }
                    }
                }

                cmbRolOrigen.DisplayMember = "Display";
                cmbRolDestino.DisplayMember = "Display";
                cmbRolOrigen.SelectedIndex = 0;
                cmbRolDestino.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar roles:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbRolOrigen_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRolOrigen.SelectedIndex > 0 && cmbRolOrigen.SelectedItem is ComboBoxRol rol)
            {
                rolOrigenID = rol.RolID;
                ValidarSeleccion();
            }
            else
            {
                rolOrigenID = 0;
            }
        }

        private void CmbRolDestino_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRolDestino.SelectedIndex > 0 && cmbRolDestino.SelectedItem is ComboBoxRol rol)
            {
                rolDestinoID = rol.RolID;
                ValidarSeleccion();
            }
            else
            {
                rolDestinoID = 0;
            }
        }

        private void ValidarSeleccion()
        {
            // Validar que no se copie un rol a sí mismo
            if (rolOrigenID > 0 && rolDestinoID > 0 && rolOrigenID == rolDestinoID)
            {
                MessageBox.Show(
                    "No puede copiar permisos de un rol a sí mismo.\n\nSeleccione roles diferentes.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                cmbRolDestino.SelectedIndex = 0;
                rolDestinoID = 0;
            }
        }

        private void BtnCopiar_Click(object sender, EventArgs e)
        {
            // Validaciones
            if (rolOrigenID == 0)
            {
                MessageBox.Show("Debe seleccionar un rol origen.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRolOrigen.Focus();
                return;
            }

            if (rolDestinoID == 0)
            {
                MessageBox.Show("Debe seleccionar un rol destino.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRolDestino.Focus();
                return;
            }

            var rolOrigen = cmbRolOrigen.SelectedItem as ComboBoxRol;
            var rolDestino = cmbRolDestino.SelectedItem as ComboBoxRol;

            // PROTECCIÓN ESPECIAL PARA ROOT
            if (rolDestino.NombreRol.ToUpper() == "ROOT")
            {
                // Si el destino es ROOT, solo permitir si origen también es ROOT (mantener)
                // O si NO se va a sobrescribir (agregar permisos está bien)
                if (chkSobrescribir.Checked && rolOrigen.NombreRol.ToUpper() != "ROOT")
                {
                    MessageBox.Show(
                        "🔒 PROTECCIÓN DE SEGURIDAD\n\n" +
                        "No se permite SOBRESCRIBIR los permisos del rol ROOT.\n\n" +
                        "El rol ROOT debe mantener acceso total al sistema.\n\n" +
                        "Si desea modificar permisos de ROOT, hágalo manualmente desde el formulario principal.",
                        "Rol Protegido",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // Si no está marcado "sobrescribir", advertir que ROOT ya tiene todos los permisos
                if (!chkSobrescribir.Checked)
                {
                    MessageBox.Show(
                        "ℹ️ INFORMACIÓN\n\n" +
                        "El rol ROOT ya tiene acceso total a todos los formularios.\n\n" +
                        "No es necesario copiar permisos hacia ROOT.",
                        "Información",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }
            }

            // PROTECCIÓN ADICIONAL: Advertencia si origen es ROOT y destino no lo es
            if (rolOrigen.NombreRol.ToUpper() == "ROOT" && rolDestino.NombreRol.ToUpper() != "ROOT")
            {
                DialogResult advertenciaRoot = MessageBox.Show(
                    "⚠️ ADVERTENCIA DE SEGURIDAD\n\n" +
                    $"Está a punto de copiar TODOS los permisos de ROOT hacia '{rolDestino.NombreRol}'.\n\n" +
                    $"Esto dará acceso TOTAL al sistema a los usuarios con rol '{rolDestino.NombreRol}'.\n\n" +
                    "¿Está completamente seguro de esta acción?",
                    "⚠️ Confirmar Acción Crítica",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (advertenciaRoot != DialogResult.Yes)
                    return;
            }

            // Confirmación normal
            string accion = chkSobrescribir.Checked ? "SOBRESCRIBIR" : "agregar";
            string advertencia = chkSobrescribir.Checked
                ? "\n\n⚠️ ADVERTENCIA: Los permisos existentes en el rol destino serán ELIMINADOS y reemplazados."
                : "\n\nSe agregarán los permisos que no existan en el destino.";

            DialogResult confirmacion = MessageBox.Show(
                $"¿Está seguro de copiar permisos?\n\n" +
                $"Origen: {rolOrigen.NombreRol}\n" +
                $"Destino: {rolDestino.NombreRol}\n" +
                $"Acción: {accion.ToUpper()}" +
                advertencia,
                "Confirmar Copia de Permisos",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmacion != DialogResult.Yes) return;

            // Ejecutar copia
            try
            {
                this.Cursor = Cursors.WaitCursor;

                int permisosCopiados = CopiarPermisos(rolOrigenID, rolDestinoID, chkSobrescribir.Checked);

                // Auditoría
                AuditoriaHelper.RegistrarAccion(
                    SesionActual.UsuarioID,
                    "COPIAR_PERMISOS",
                    "SISTEMA",
                    "Gestión de Roles",
                    "FormCopiarPermisos",
                    registroID: rolDestinoID,
                    detalle: $"Permisos copiados desde '{rolOrigen.NombreRol}' hacia '{rolDestino.NombreRol}'. Total: {permisosCopiados}"
                );

                this.Cursor = Cursors.Default;

                MessageBox.Show(
                    $"✅ Permisos copiados exitosamente.\n\n" +
                    $"Total de permisos copiados: {permisosCopiados}\n\n" +
                    $"Desde: {rolOrigen.NombreRol}\n" +
                    $"Hacia: {rolDestino.NombreRol}",
                    "Copia Exitosa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                PermisosCopiados = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show($"Error al copiar permisos:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int CopiarPermisos(int origenID, int destinoID, bool sobrescribir)
        {
            int permisosCopiados = 0;

            using (var conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Si es sobrescribir, eliminar permisos existentes del destino
                        if (sobrescribir)
                        {
                            string sqlDelete = "DELETE FROM PermisosRol WHERE RolID = @RolDestino";
                            using (var cmdDelete = new SqlCommand(sqlDelete, conn, transaction))
                            {
                                cmdDelete.Parameters.AddWithValue("@RolDestino", destinoID);
                                cmdDelete.ExecuteNonQuery();
                            }
                        }

                        // Copiar permisos
                        string sqlCopy = @"
                            INSERT INTO PermisosRol (RolID, FormularioID, AccionID, Permitido, CreadoPorUsuarioID)
                            SELECT @RolDestino, FormularioID, AccionID, Permitido, @CreadoPor
                            FROM PermisosRol
                            WHERE RolID = @RolOrigen
                            AND NOT EXISTS (
                                SELECT 1 FROM PermisosRol PR2
                                WHERE PR2.RolID = @RolDestino
                                AND PR2.FormularioID = PermisosRol.FormularioID
                                AND PR2.AccionID = PermisosRol.AccionID
                            )";

                        using (var cmdCopy = new SqlCommand(sqlCopy, conn, transaction))
                        {
                            cmdCopy.Parameters.AddWithValue("@RolOrigen", origenID);
                            cmdCopy.Parameters.AddWithValue("@RolDestino", destinoID);
                            cmdCopy.Parameters.AddWithValue("@CreadoPor", SesionActual.UsuarioID);

                            permisosCopiados = cmdCopy.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return permisosCopiados;
        }

        // Clase auxiliar
        private class ComboBoxRol
        {
            public int RolID { get; set; }
            public string NombreRol { get; set; }
            public bool EsSistema { get; set; }
            public string Display => NombreRol + (EsSistema ? " (Sistema)" : " (Personalizado)");
        }
    }
}