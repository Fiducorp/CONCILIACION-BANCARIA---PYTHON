using System;
using System.Data.SqlClient;
using BCrypt.Net;

namespace MOFIS_ERP.Classes
{
    /// <summary>
    /// Clase auxiliar para manejar la autenticación de usuarios
    /// </summary>
    public static class AutenticacionHelper
    {
        /// <summary>
        /// Valida las credenciales de un usuario contra la base de datos
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>UsuarioDTO si las credenciales son válidas, null si son inválidas</returns>
        public static UsuarioDTO ValidarUsuario(string username, string password)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = @"SELECT 
                                    U.UsuarioID,
                                    U.Username,
                                    U.PasswordHash,
                                    U.NombreCompleto,
                                    U.Email,
                                    U.RolID,
                                    R.NombreRol,
                                    U.Activo,
                                    U.DebeCambiarPassword,
                                    U.FechaCreacion,
                                    U.UltimoAcceso
                                  FROM Usuarios U
                                  INNER JOIN Roles R ON U.RolID = R.RolID
                                  WHERE U.Username = @Username";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Verificar si el usuario está activo
                                bool activo = reader.GetBoolean(reader.GetOrdinal("Activo"));
                                if (!activo)
                                {
                                    return null; // Usuario inactivo
                                }

                                // Obtener el hash almacenado
                                string hashAlmacenado = reader.GetString(reader.GetOrdinal("PasswordHash"));

                                // Verificar la contraseña con BCrypt
                                if (BCrypt.Net.BCrypt.Verify(password, hashAlmacenado))
                                {
                                    // Credenciales válidas, crear DTO
                                    var usuario = new UsuarioDTO
                                    {
                                        UsuarioID = reader.GetInt32(reader.GetOrdinal("UsuarioID")),
                                        Username = reader.GetString(reader.GetOrdinal("Username")),
                                        PasswordHash = hashAlmacenado,
                                        NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto")),
                                        Email = reader.IsDBNull(reader.GetOrdinal("Email")) ?
                                                null : reader.GetString(reader.GetOrdinal("Email")),
                                        RolID = reader.GetInt32(reader.GetOrdinal("RolID")),
                                        NombreRol = reader.GetString(reader.GetOrdinal("NombreRol")),
                                        Activo = activo,
                                        DebeCambiarPassword = reader.GetBoolean(reader.GetOrdinal("DebeCambiarPassword")),
                                        FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                                        UltimoAcceso = reader.IsDBNull(reader.GetOrdinal("UltimoAcceso")) ?
                                                       (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UltimoAcceso"))
                                    };

                                    return usuario;
                                }
                            }
                        }
                    }
                }

                // Credenciales inválidas
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al validar usuario: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza la fecha de último acceso del usuario
        /// </summary>
        public static void ActualizarUltimoAcceso(int usuarioID)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string sql = "UPDATE Usuarios SET UltimoAcceso = @UltimoAcceso WHERE UsuarioID = @UsuarioID";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UltimoAcceso", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Ignorar errores al actualizar último acceso
            }
        }

        /// <summary>
        /// Cambia la contraseña de un usuario
        /// </summary>
        /// <param name="usuarioID">ID del usuario</param>
        /// <param name="passwordActual">Contraseña actual en texto plano</param>
        /// <param name="passwordNueva">Nueva contraseña en texto plano</param>
        /// <returns>True si el cambio fue exitoso, False si la contraseña actual es incorrecta</returns>
        public static bool CambiarPassword(int usuarioID, string passwordActual, string passwordNueva)
        {
            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // 1. Verificar contraseña actual
                    string sqlVerificar = "SELECT PasswordHash FROM Usuarios WHERE UsuarioID = @UsuarioID";
                    string hashAlmacenado;

                    using (var cmd = new SqlCommand(sqlVerificar, conn))
                    {
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        hashAlmacenado = cmd.ExecuteScalar()?.ToString();
                    }

                    if (string.IsNullOrEmpty(hashAlmacenado))
                        return false;

                    // Verificar contraseña actual con BCrypt
                    if (!BCrypt.Net.BCrypt.Verify(passwordActual, hashAlmacenado))
                        return false;

                    // 2. Actualizar con nueva contraseña
                    string nuevoHash = BCrypt.Net.BCrypt.HashPassword(passwordNueva);
                    string sqlActualizar = @"UPDATE Usuarios 
                                            SET PasswordHash = @PasswordHash, 
                                                DebeCambiarPassword = 0 
                                            WHERE UsuarioID = @UsuarioID";

                    using (var cmd = new SqlCommand(sqlActualizar, conn))
                    {
                        cmd.Parameters.AddWithValue("@PasswordHash", nuevoHash);
                        cmd.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cambiar contraseña: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Valida la fortaleza de una contraseña según los requisitos del sistema
        /// </summary>
        /// <param name="password">Contraseña a validar</param>
        /// <returns>Mensaje de error si la contraseña es débil, null si es válida</returns>
        public static string ValidarFortalezaPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "La contraseña no puede estar vacía";

            if (password.Length < 8)
                return "La contraseña debe tener al menos 8 caracteres";

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]"))
                return "La contraseña debe contener al menos una letra mayúscula";

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]"))
                return "La contraseña debe contener al menos una letra minúscula";

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[0-9]"))
                return "La contraseña debe contener al menos un número";

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]"))
                return "La contraseña debe contener al menos un carácter especial (!@#$%^&*)";

            return null; // Contraseña válida
        }
    }
}