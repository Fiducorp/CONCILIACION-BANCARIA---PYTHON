using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MOFIS_ERP.Classes;

namespace MOFIS_ERP.Forms.Sistema.Auditoria
{
    /// <summary>
    /// Formulario modal para mostrar el detalle completo de un registro de auditoría
    /// </summary>
    public partial class FormDetalleAuditoria : Form
    {
        private DataRow registroAuditoria;

        // Colores corporativos
        private readonly Color colorCorporativo = Color.FromArgb(0, 120, 212);
        private readonly Color colorVerde = Color.FromArgb(34, 139, 34);
        private readonly Color colorGris = Color.FromArgb(108, 117, 125);
        private readonly Color colorGrisClaro = Color.FromArgb(240, 240, 240);

        /// <summary>
        /// Constructor que recibe el DataRow con la información del registro
        /// </summary>
        public FormDetalleAuditoria(DataRow registro)
        {
            InitializeComponent();
            registroAuditoria = registro;

            ConfigurarFormulario();
            CargarDatos();
        }

        /// <summary>
        /// Configuración inicial del formulario
        /// </summary>
        private void ConfigurarFormulario()
        {
            // Configurar propiedades del formulario
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;

            // Configurar hover de botones
            ConfigurarBotonHover(btnCerrar, colorGris);
            ConfigurarBotonHover(btnCopiarDetalle, colorCorporativo);
            ConfigurarBotonHover(btnVerUsuario, colorVerde);

            // Eventos
            btnCerrar.Click += BtnCerrar_Click;
            btnCopiarDetalle.Click += BtnCopiarDetalle_Click;
            btnVerUsuario.Click += BtnVerUsuario_Click;

            // Hacer el TextBox de detalle de solo lectura con scroll
            txtDetalle.ReadOnly = true;
            txtDetalle.ScrollBars = ScrollBars.Vertical;
            txtDetalle.BackColor = Color.White;
        }

        /// <summary>
        /// Configura el efecto hover de los botones
        /// </summary>
        private void ConfigurarBotonHover(Button btn, Color colorBase)
        {
            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Dark(colorBase, 0.1f);
            btn.MouseLeave += (s, e) => btn.BackColor = colorBase;
        }

        /// <summary>
        /// Carga los datos del registro en los controles
        /// </summary>
        private void CargarDatos()
        {
            try
            {
                // Información principal
                lblAuditoriaIDValor.Text = ObtenerValor("AuditoriaID");
                lblFechaHoraValor.Text = ObtenerValorFecha("FechaHora");
                lblAccionValor.Text = ObtenerValor("Accion");

                // Información del usuario
                lblUsuarioValor.Text = ObtenerValor("Username");

                // Información de ubicación en el sistema
                lblCategoriaValor.Text = ObtenerValor("Categoria");
                lblModuloValor.Text = ObtenerValor("Modulo");
                lblFormularioValor.Text = ObtenerValor("Formulario");

                // Información técnica
                lblIPValor.Text = ObtenerValor("DireccionIP");
                lblMaquinaValor.Text = ObtenerValor("NombreMaquina");

                // Detalle completo (en TextBox multilinea)
                string detalle = ObtenerValor("Detalle");
                txtDetalle.Text = string.IsNullOrEmpty(detalle)
                    ? "(Sin información adicional)"
                    : detalle;

                // Colorear la acción según tipo
                ColorearAccion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cargar datos del registro:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Obtiene el valor de una columna del DataRow
        /// </summary>
        private string ObtenerValor(string columna)
        {
            try
            {
                if (registroAuditoria.Table.Columns.Contains(columna))
                {
                    var valor = registroAuditoria[columna];

                    if (valor == null || valor == DBNull.Value)
                        return "(No especificado)";

                    return valor.ToString();
                }

                return "(No disponible)";
            }
            catch
            {
                return "(Error)";
            }
        }

        /// <summary>
        /// Obtiene el valor de una columna de fecha formateada
        /// </summary>
        private string ObtenerValorFecha(string columna)
        {
            try
            {
                if (registroAuditoria.Table.Columns.Contains(columna))
                {
                    var valor = registroAuditoria[columna];

                    if (valor == null || valor == DBNull.Value)
                        return "(No especificado)";

                    if (valor is DateTime fecha)
                        return fecha.ToString("dd/MM/yyyy HH:mm:ss");

                    return valor.ToString();
                }

                return "(No disponible)";
            }
            catch
            {
                return "(Error)";
            }
        }

        /// <summary>
        /// Colorea el label de acción según el tipo de operación
        /// </summary>
        private void ColorearAccion()
        {
            string accion = lblAccionValor.Text.ToUpper();

            if (accion.Contains("LOGIN") || accion.Contains("CREAR"))
            {
                lblAccionValor.ForeColor = colorVerde;
            }
            else if (accion.Contains("ELIMINAR") || accion.Contains("LOGIN_FALLIDO"))
            {
                lblAccionValor.ForeColor = Color.FromArgb(220, 53, 69); // Rojo
            }
            else if (accion.Contains("MODIFICAR") || accion.Contains("EDITAR"))
            {
                lblAccionValor.ForeColor = Color.FromArgb(255, 152, 0); // Naranja
            }
            else
            {
                lblAccionValor.ForeColor = colorCorporativo; // Azul por defecto
            }

            lblAccionValor.Font = new Font(lblAccionValor.Font, FontStyle.Bold);
        }

        /// <summary>
        /// Cierra el formulario
        /// </summary>
        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Copia el detalle completo al portapapeles
        /// </summary>
        private void BtnCopiarDetalle_Click(object sender, EventArgs e)
        {
            try
            {
                // Construir texto completo del registro
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("═══════════════════════════════════════════════════════");
                sb.AppendLine("           DETALLE DE AUDITORÍA - MOFIS ERP");
                sb.AppendLine("═══════════════════════════════════════════════════════");
                sb.AppendLine();
                sb.AppendLine($"ID de Registro: {lblAuditoriaIDValor.Text}");
                sb.AppendLine($"Fecha y Hora: {lblFechaHoraValor.Text}");
                sb.AppendLine($"Acción: {lblAccionValor.Text}");
                sb.AppendLine();
                sb.AppendLine("─────────────────────────────────────────────────────");
                sb.AppendLine("INFORMACIÓN DEL USUARIO");
                sb.AppendLine("─────────────────────────────────────────────────────");
                sb.AppendLine($"Usuario: {lblUsuarioValor.Text}");
                sb.AppendLine();
                sb.AppendLine("─────────────────────────────────────────────────────");
                sb.AppendLine("UBICACIÓN EN EL SISTEMA");
                sb.AppendLine("─────────────────────────────────────────────────────");
                sb.AppendLine($"Categoría: {lblCategoriaValor.Text}");
                sb.AppendLine($"Módulo: {lblModuloValor.Text}");
                sb.AppendLine($"Formulario: {lblFormularioValor.Text}");
                sb.AppendLine();
                sb.AppendLine("─────────────────────────────────────────────────────");
                sb.AppendLine("INFORMACIÓN TÉCNICA");
                sb.AppendLine("─────────────────────────────────────────────────────");
                sb.AppendLine($"Dirección IP: {lblIPValor.Text}");
                sb.AppendLine($"Nombre de Máquina: {lblMaquinaValor.Text}");
                sb.AppendLine();
                sb.AppendLine("─────────────────────────────────────────────────────");
                sb.AppendLine("DETALLE COMPLETO");
                sb.AppendLine("─────────────────────────────────────────────────────");
                sb.AppendLine(txtDetalle.Text);
                sb.AppendLine();
                sb.AppendLine("═══════════════════════════════════════════════════════");
                sb.AppendLine($"Copiado el: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                sb.AppendLine($"Por: {SesionActual.NombreCompleto} ({SesionActual.Username})");
                sb.AppendLine("═══════════════════════════════════════════════════════");

                // Copiar al portapapeles
                Clipboard.SetText(sb.ToString());

                // Mostrar confirmación
                MessageBox.Show(
                    "✅ Detalle completo copiado al portapapeles",
                    "Copiado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al copiar al portapapeles:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Muestra información adicional del usuario que realizó la acción
        /// </summary>
        private void BtnVerUsuario_Click(object sender, EventArgs e)
        {
            string username = lblUsuarioValor.Text;

            if (string.IsNullOrEmpty(username) || username == "(No especificado)")
            {
                MessageBox.Show(
                    "No hay información de usuario disponible.",
                    "Información",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            try
            {
                // Buscar información del usuario en la base de datos
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"
                        SELECT 
                            U.UsuarioID,
                            U.Username,
                            U.NombreCompleto,
                            U.Email,
                            R.NombreRol,
                            U.Activo,
                            U.FechaCreacion,
                            U.UltimoAcceso
                        FROM Usuarios U
                        INNER JOIN Roles R ON U.RolID = R.RolID
                        WHERE U.Username = @Username";

                    using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var infoUsuario = new System.Text.StringBuilder();
                                infoUsuario.AppendLine("═══════════════════════════════════════");
                                infoUsuario.AppendLine("      INFORMACIÓN DEL USUARIO");
                                infoUsuario.AppendLine("═══════════════════════════════════════");
                                infoUsuario.AppendLine();
                                infoUsuario.AppendLine($"ID: {reader["UsuarioID"]}");
                                infoUsuario.AppendLine($"Usuario: {reader["Username"]}");
                                infoUsuario.AppendLine($"Nombre: {reader["NombreCompleto"]}");
                                infoUsuario.AppendLine($"Email: {(reader["Email"] == DBNull.Value ? "(No especificado)" : reader["Email"])}");
                                infoUsuario.AppendLine($"Rol: {reader["NombreRol"]}");
                                infoUsuario.AppendLine($"Estado: {(Convert.ToBoolean(reader["Activo"]) ? "✅ Activo" : "❌ Inactivo")}");
                                infoUsuario.AppendLine($"Creado: {Convert.ToDateTime(reader["FechaCreacion"]):dd/MM/yyyy}");

                                if (reader["UltimoAcceso"] != DBNull.Value)
                                {
                                    infoUsuario.AppendLine($"Último acceso: {Convert.ToDateTime(reader["UltimoAcceso"]):dd/MM/yyyy HH:mm:ss}");
                                }

                                MessageBox.Show(
                                    infoUsuario.ToString(),
                                    "Información del Usuario",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information
                                );
                            }
                            else
                            {
                                MessageBox.Show(
                                    $"No se encontró información para el usuario: {username}",
                                    "Usuario no encontrado",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al obtener información del usuario:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}