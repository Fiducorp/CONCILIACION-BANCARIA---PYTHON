using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;

namespace MOFIS_ERP.Classes
{
    /// <summary>
    /// Clase auxiliar para registrar todas las acciones de auditoría en la base de datos
    /// </summary>
    public static class AuditoriaHelper
    {
        /// <summary>
        /// Registra una acción en la tabla de Auditoría
        /// </summary>
        /// <param name="usuarioID">ID del usuario que realiza la acción</param>
        /// <param name="accion">Acción realizada (LOGIN, LOGOUT, CREAR, EDITAR, ELIMINAR, VER, IMPRIMIR)</param>
        /// <param name="categoria">Categoría del sistema (SISTEMA, CONTABILIDAD, etc.)</param>
        /// <param name="modulo">Módulo específico (Usuarios, Roles, Cuentas por Pagar, etc.)</param>
        /// <param name="formulario">Nombre del formulario (opcional)</param>
        /// <param name="registroID">ID del registro afectado (opcional)</param>
        /// <param name="detalle">Información adicional (opcional)</param>
        public static void RegistrarAccion(int usuarioID, string accion, string categoria = null,
                                          string modulo = null, string formulario = null,
                                          int? registroID = null, string detalle = null)
        {
            try
            {
                string ip = ObtenerDireccionIP();
                string nombreMaquina = ObtenerNombreMaquina();

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"INSERT INTO Auditoria 
                                  (UsuarioID, Accion, Categoria, Modulo, Formulario, 
                                   RegistroID, Detalle, FechaHora, DireccionIP, NombreMaquina)
                                  VALUES 
                                  (@UsuarioID, @Accion, @Categoria, @Modulo, @Formulario, 
                                   @RegistroID, @Detalle, @FechaHora, @DireccionIP, @NombreMaquina)";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        cmd.Parameters.AddWithValue("@Accion", accion);
                        cmd.Parameters.AddWithValue("@Categoria", (object)categoria ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Modulo", (object)modulo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Formulario", (object)formulario ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@RegistroID", (object)registroID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Detalle", (object)detalle ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaHora", DateTime.Now);
                        cmd.Parameters.AddWithValue("@DireccionIP", ip);
                        cmd.Parameters.AddWithValue("@NombreMaquina", nombreMaquina);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // En un sistema de producción, aquí se debería registrar el error en un log
                // Por ahora, solo lo ignoramos para no interrumpir la operación principal
                System.Diagnostics.Debug.WriteLine($"Error en auditoría: {ex.Message}");
            }
        }

        /// <summary>
        /// Registra un LOGIN exitoso
        /// </summary>
        public static void RegistrarLogin(int usuarioID, string username)
        {
            RegistrarAccion(usuarioID, "LOGIN", "SISTEMA", "Autenticación", "FormLogin",
                          detalle: $"Usuario {username} inició sesión exitosamente");
        }

        /// <summary>
        /// Registra un LOGOUT
        /// </summary>
        public static void RegistrarLogout(int usuarioID, string username)
        {
            RegistrarAccion(usuarioID, "LOGOUT", "SISTEMA", "Autenticación", "FormLogin",
                          detalle: $"Usuario {username} cerró sesión");
        }

        /// <summary>
        /// Registra un intento de login fallido
        /// </summary>
        public static void RegistrarLoginFallido(string username)
        {
            try
            {
                string ip = ObtenerDireccionIP();
                string nombreMaquina = ObtenerNombreMaquina();

                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // Para intentos fallidos, usamos UsuarioID = -1 (usuario no válido)
                    string sql = @"INSERT INTO Auditoria 
                                  (UsuarioID, Accion, Categoria, Modulo, Formulario, 
                                   Detalle, FechaHora, DireccionIP, NombreMaquina)
                                  VALUES 
                                  (-1, 'LOGIN_FALLIDO', 'SISTEMA', 'Autenticación', 'FormLogin', 
                                   @Detalle, @FechaHora, @DireccionIP, @NombreMaquina)";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Detalle", $"Intento de login fallido para usuario: {username}");
                        cmd.Parameters.AddWithValue("@FechaHora", DateTime.Now);
                        cmd.Parameters.AddWithValue("@DireccionIP", ip);
                        cmd.Parameters.AddWithValue("@NombreMaquina", nombreMaquina);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Ignorar errores en auditoría de fallos
            }
        }

        /// <summary>
        /// Obtiene la dirección IP local de la máquina
        /// </summary>
        private static string ObtenerDireccionIP()
        {
            try
            {
                string hostName = Dns.GetHostName();
                IPAddress[] addresses = Dns.GetHostAddresses(hostName);

                foreach (IPAddress address in addresses)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return address.ToString();
                    }
                }

                return "127.0.0.1";
            }
            catch
            {
                return "Desconocida";
            }
        }

        /// <summary>
        /// Obtiene el nombre de la máquina
        /// </summary>
        private static string ObtenerNombreMaquina()
        {
            try
            {
                return Environment.MachineName;
            }
            catch
            {
                return "Desconocida";
            }
        }
    }
}