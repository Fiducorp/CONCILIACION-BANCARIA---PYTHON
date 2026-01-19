using System;
using System.Configuration;
using System.Data.SqlClient;

namespace MOFIS_ERP.Classes
{
    /// <summary>
    /// Clase estática para manejar la conexión a SQL Server
    /// Proporciona métodos centralizados para obtener conexiones a la base de datos
    /// </summary>
    public static class DatabaseConnection
    {
        // Nombre de la connection string en App.config
        private const string CONNECTION_STRING_NAME = "FiducorpERP";

        /// <summary>
        /// Obtiene la cadena de conexión desde App.config
        /// </summary>
        private static string ConnectionString
        {
            get
            {
                try
                {
                    var connectionString = ConfigurationManager.ConnectionStrings[CONNECTION_STRING_NAME]?.ConnectionString;

                    if (string.IsNullOrEmpty(connectionString))
                    {
                        throw new InvalidOperationException(
                            $"No se encontró la connection string '{CONNECTION_STRING_NAME}' en App.config");
                    }

                    return connectionString;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Error al obtener la connection string: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Obtiene una nueva conexión a la base de datos
        /// IMPORTANTE: El código que llama este método debe cerrar la conexión (usando 'using')
        /// </summary>
        /// <returns>SqlConnection configurada pero NO abierta</returns>
        public static SqlConnection GetConnection()
        {
            try
            {
                return new SqlConnection(ConnectionString);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Error al crear la conexión a la base de datos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Prueba la conexión a la base de datos
        /// </summary>
        /// <returns>True si la conexión es exitosa, False si falla</returns>
        public static bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return connection.State == System.Data.ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Prueba la conexión y devuelve el mensaje de error si falla
        /// </summary>
        /// <param name="errorMessage">Mensaje de error en caso de fallo</param>
        /// <returns>True si la conexión es exitosa</returns>
        public static bool TestConnection(out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();

                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        errorMessage = "Conexión exitosa";
                        return true;
                    }
                    else
                    {
                        errorMessage = "No se pudo establecer la conexión";
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                errorMessage = $"Error de SQL Server: {sqlEx.Message}";
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error general: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Obtiene el nombre del servidor desde la connection string
        /// </summary>
        public static string GetServerName()
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(ConnectionString);
                return builder.DataSource;
            }
            catch
            {
                return "Desconocido";
            }
        }

        /// <summary>
        /// Obtiene el nombre de la base de datos desde la connection string
        /// </summary>
        public static string GetDatabaseName()
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(ConnectionString);
                return builder.InitialCatalog;
            }
            catch
            {
                return "Desconocido";
            }
        }
    }
}