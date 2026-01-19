using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace MOFIS_ERP.Classes
{
    /// <summary>
    /// Clase helper para gestión de permisos del sistema
    /// Evalúa permisos basándose en la estructura: Usuario > Rol > Formulario > Acción
    /// </summary>
    public static class PermisosHelper
    {
        /// <summary>
        /// Verifica si el usuario actual tiene permiso para realizar una acción en un formulario
        /// </summary>
        /// <param name="codigoFormulario">Código del formulario (ej: FGEUSR)</param>
        /// <param name="codigoAccion">Código de la acción (ej: VIEW, CREATE, EDIT, DELETE)</param>
        /// <returns>True si tiene permiso, False si no</returns>
        public static bool TienePermiso(string codigoFormulario, string codigoAccion)
        {
            return TienePermiso(SesionActual.UsuarioID, codigoFormulario, codigoAccion);
        }

        /// <summary>
        /// Verifica si un usuario específico tiene permiso para realizar una acción en un formulario
        /// </summary>
        /// <param name="usuarioID">ID del usuario</param>
        /// <param name="codigoFormulario">Código del formulario (ej: FGEUSR)</param>
        /// <param name="codigoAccion">Código de la acción (ej: VIEW, CREATE, EDIT, DELETE)</param>
        /// <returns>True si tiene permiso, False si no</returns>
        public static bool TienePermiso(int usuarioID, string codigoFormulario, string codigoAccion)
        {
            // ROOT siempre tiene acceso total
            if (SesionActual.EsRoot())
                return true;

            try
            {
                using (var conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    // PASO 1: Verificar si existe un permiso ESPECÍFICO para el USUARIO
                    // Esto permite excepciones individuales que sobrescriben el permiso del rol
                    string sqlUsuario = @"
                        SELECT PU.Permitido
                        FROM PermisosUsuario PU
                        INNER JOIN CatalogoFormularios F ON PU.FormularioID = F.FormularioID
                        INNER JOIN CatalogoAcciones A ON PU.AccionID = A.AccionID
                        WHERE PU.UsuarioID = @UsuarioID
                          AND F.CodigoFormulario = @CodigoFormulario
                          AND A.CodigoAccion = @CodigoAccion
                          AND F.Activo = 1
                          AND A.Activo = 1
                          AND (PU.FechaExpiracion IS NULL OR PU.FechaExpiracion > GETDATE())";

                    using (var cmdUsuario = new SqlCommand(sqlUsuario, conn))
                    {
                        cmdUsuario.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        cmdUsuario.Parameters.AddWithValue("@CodigoFormulario", codigoFormulario);
                        cmdUsuario.Parameters.AddWithValue("@CodigoAccion", codigoAccion);

                        var resultadoUsuario = cmdUsuario.ExecuteScalar();

                        // Si existe un permiso específico para el usuario, usarlo
                        if (resultadoUsuario != null)
                        {
                            return Convert.ToBoolean(resultadoUsuario);
                        }
                    }

                    // PASO 2: Si no existe permiso de usuario, verificar permiso del ROL
                    string sqlRol = @"
                        SELECT PR.Permitido
                        FROM PermisosRol PR
                        INNER JOIN Usuarios U ON PR.RolID = U.RolID
                        INNER JOIN CatalogoFormularios F ON PR.FormularioID = F.FormularioID
                        INNER JOIN CatalogoAcciones A ON PR.AccionID = A.AccionID
                        WHERE U.UsuarioID = @UsuarioID
                          AND F.CodigoFormulario = @CodigoFormulario
                          AND A.CodigoAccion = @CodigoAccion
                          AND F.Activo = 1
                          AND A.Activo = 1";

                    using (var cmdRol = new SqlCommand(sqlRol, conn))
                    {
                        cmdRol.Parameters.AddWithValue("@UsuarioID", usuarioID);
                        cmdRol.Parameters.AddWithValue("@CodigoFormulario", codigoFormulario);
                        cmdRol.Parameters.AddWithValue("@CodigoAccion", codigoAccion);

                        var resultadoRol = cmdRol.ExecuteScalar();

                        // Si existe permiso del rol, usarlo
                        if (resultadoRol != null)
                        {
                            return Convert.ToBoolean(resultadoRol);
                        }
                    }

                    // PASO 3: Si no existe ni permiso de usuario ni de rol, DENEGAR por defecto
                    return false;
                }
            }
            catch (Exception ex)
            {
                // En caso de error, denegar acceso por seguridad
                System.Windows.Forms.MessageBox.Show(
                    $"Error al verificar permisos:\n\n{ex.Message}",
                    "Error de Permisos",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error
                );
                return false;
            }
        }

        /// <summary>
        /// Verifica si el usuario tiene múltiples permisos a la vez
        /// Útil para verificar varios permisos de forma eficiente
        /// </summary>
        /// <param name="codigoFormulario">Código del formulario</param>
        /// <param name="codigosAcciones">Array de códigos de acciones</param>
        /// <returns>Diccionario con el resultado de cada acción</returns>
        public static bool[] TienePermisos(string codigoFormulario, params string[] codigosAcciones)
        {
            bool[] resultados = new bool[codigosAcciones.Length];

            for (int i = 0; i < codigosAcciones.Length; i++)
            {
                resultados[i] = TienePermiso(codigoFormulario, codigosAcciones[i]);
            }

            return resultados;
        }

        /// <summary>
        /// Obtiene la información detallada de permisos para un formulario
        /// Útil para configurar la UI de un formulario completo
        /// </summary>
        public static PermisosFormulario ObtenerPermisosFormulario(string codigoFormulario)
        {
            return new PermisosFormulario
            {
                PuedeVer = TienePermiso(codigoFormulario, "VIEW"),
                PuedeCrear = TienePermiso(codigoFormulario, "CREATE"),
                PuedeEditar = TienePermiso(codigoFormulario, "EDIT"),
                PuedeEliminar = TienePermiso(codigoFormulario, "DELETE"),
                PuedeImprimir = TienePermiso(codigoFormulario, "PRINT"),
                PuedeReimprimir = TienePermiso(codigoFormulario, "REPRINT"),
                PuedeExportar = TienePermiso(codigoFormulario, "EXPORT"),
                PuedeAprobar = TienePermiso(codigoFormulario, "APPROVE"),
                PuedeRechazar = TienePermiso(codigoFormulario, "REJECT"),
                PuedeActivar = TienePermiso(codigoFormulario, "ACTIVATE"),
                PuedeResetear = TienePermiso(codigoFormulario, "RESET")
            };
        }
    }

    /// <summary>
    /// Clase que encapsula todos los permisos posibles de un formulario
    /// </summary>
    public class PermisosFormulario
    {
        public bool PuedeVer { get; set; }
        public bool PuedeCrear { get; set; }
        public bool PuedeEditar { get; set; }
        public bool PuedeEliminar { get; set; }
        public bool PuedeImprimir { get; set; }
        public bool PuedeReimprimir { get; set; }
        public bool PuedeExportar { get; set; }
        public bool PuedeAprobar { get; set; }
        public bool PuedeRechazar { get; set; }
        public bool PuedeActivar { get; set; }
        public bool PuedeResetear { get; set; }

        /// <summary>
        /// Verifica si el usuario tiene al menos un permiso
        /// </summary>
        public bool TieneAlgunPermiso()
        {
            return PuedeVer || PuedeCrear || PuedeEditar || PuedeEliminar ||
                   PuedeImprimir || PuedeReimprimir || PuedeExportar ||
                   PuedeAprobar || PuedeRechazar || PuedeActivar || PuedeResetear;
        }

        /// <summary>
        /// Verifica si el usuario tiene permisos completos (todos los CRUD)
        /// </summary>
        public bool TienePermisoCompleto()
        {
            return PuedeVer && PuedeCrear && PuedeEditar && PuedeEliminar;
        }
    }
}