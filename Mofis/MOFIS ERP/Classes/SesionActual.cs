using System;

namespace MOFIS_ERP.Classes
{
    /// <summary>
    /// Clase estática que mantiene la información del usuario actualmente logueado
    /// Actúa como sesión global de la aplicación
    /// </summary>
    public static class SesionActual
    {
        #region Propiedades del Usuario

        /// <summary>
        /// ID del usuario en la base de datos
        /// </summary>
        public static int UsuarioID { get; set; }

        /// <summary>
        /// Nombre de usuario (login)
        /// </summary>
        public static string Username { get; set; }

        /// <summary>
        /// Nombre completo del usuario
        /// </summary>
        public static string NombreCompleto { get; set; }

        /// <summary>
        /// Email del usuario
        /// </summary>
        public static string Email { get; set; }

        /// <summary>
        /// ID del rol asignado
        /// </summary>
        public static int RolID { get; set; }

        /// <summary>
        /// Nombre del rol (ROOT, ADMIN, CONTADOR)
        /// </summary>
        public static string NombreRol { get; set; }

        /// <summary>
        /// Fecha y hora del login
        /// </summary>
        public static DateTime FechaLogin { get; set; }

        #endregion

        #region Métodos de Control de Sesión

        /// <summary>
        /// Verifica si hay una sesión activa
        /// </summary>
        public static bool HaySesionActiva()
        {
            return UsuarioID > 0 && !string.IsNullOrEmpty(Username);
        }

        /// <summary>
        /// Inicializa la sesión con los datos del usuario
        /// </summary>
        public static void IniciarSesion(int usuarioID, string username, string nombreCompleto,
                                         string email, int rolID, string nombreRol)
        {
            UsuarioID = usuarioID;
            Username = username;
            NombreCompleto = nombreCompleto;
            Email = email;
            RolID = rolID;
            NombreRol = nombreRol;
            FechaLogin = DateTime.Now;
        }

        /// <summary>
        /// Cierra la sesión actual limpiando todos los datos
        /// </summary>
        public static void CerrarSesion()
        {
            UsuarioID = 0;
            Username = string.Empty;
            NombreCompleto = string.Empty;
            Email = string.Empty;
            RolID = 0;
            NombreRol = string.Empty;
            FechaLogin = DateTime.MinValue;
        }

        /// <summary>
        /// Verifica si el usuario actual tiene un rol específico
        /// </summary>
        public static bool TieneRol(string nombreRol)
        {
            return NombreRol.Equals(nombreRol, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Verifica si el usuario actual es ROOT
        /// </summary>
        public static bool EsRoot()
        {
            return TieneRol("ROOT");
        }

        /// <summary>
        /// Verifica si el usuario actual es ADMIN
        /// </summary>
        public static bool EsAdmin()
        {
            return TieneRol("ADMIN");
        }

        /// <summary>
        /// Verifica si el usuario actual es CONTADOR
        /// </summary>
        public static bool EsContador()
        {
            return TieneRol("CONTADOR");
        }

        /// <summary>
        /// Obtiene un resumen de la sesión actual para mostrar en UI
        /// </summary>
        public static string ObtenerResumenSesion()
        {
            if (!HaySesionActiva())
                return "Sin sesión activa";

            return $"{NombreCompleto} ({NombreRol}) - Sesión iniciada: {FechaLogin:dd/MM/yyyy HH:mm}";
        }

        #endregion
    }
}