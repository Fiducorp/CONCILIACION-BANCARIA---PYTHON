using System;

namespace MOFIS_ERP.Classes
{
    /// <summary>
    /// Constantes para todas las acciones de auditoría del sistema MOFIS-ERP
    /// Organizado por categoría y módulo para mantener consistencia
    /// </summary>
    public static class AuditoriaAcciones
    {
        // ═══════════════════════════════════════════════════════════════
        // CATEGORÍA: SISTEMA
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Acciones del módulo de Autenticación
        /// </summary>
        public static class Autenticacion
        {
            public const string LOGIN = "LOGIN";
            public const string LOGOUT = "LOGOUT";
            public const string LOGIN_FALLIDO = "LOGIN_FALLIDO";
            public const string CAMBIAR_PASSWORD = "CAMBIAR_PASSWORD";
        }

        /// <summary>
        /// Acciones del módulo de Gestión de Usuarios
        /// </summary>
        public static class GestionUsuarios
        {
            public const string CREAR_USUARIO = "CREAR_USUARIO";
            public const string MODIFICAR_USUARIO = "MODIFICAR_USUARIO";
            public const string ELIMINAR_USUARIO = "ELIMINAR_USUARIO";
            public const string ACTIVAR_USUARIO = "ACTIVAR_USUARIO";
            public const string DESACTIVAR_USUARIO = "DESACTIVAR_USUARIO";
            public const string RESET_PASSWORD_USUARIO = "RESET_PASSWORD_USUARIO";
        }

        /// <summary>
        /// Acciones del módulo de Gestión de Roles
        /// </summary>
        public static class GestionRoles
        {
            public const string CREAR_ROL = "CREAR_ROL";
            public const string ELIMINAR_ROL = "ELIMINAR_ROL";
            public const string MODIFICAR_PERMISOS_ROL = "MODIFICAR_PERMISOS_ROL";
            public const string MODIFICAR_PERMISOS_USUARIO = "MODIFICAR_PERMISOS_USUARIO";
            public const string COPIAR_PERMISOS = "COPIAR_PERMISOS";
            public const string CONSULTAR_PERMISOS = "CONSULTAR_PERMISOS";
            public const string CONFIRMAR_PASSWORD_CAMBIO_CRITICO = "CONFIRMAR_PASSWORD_CAMBIO_CRITICO";
            public const string GENERAR_REPORTE_PERMISOS = "GENERAR_REPORTE_PERMISOS";
            public const string GENERAR_REPORTE_MATRIZ = "GENERAR_REPORTE_MATRIZ";
            public const string GENERAR_REPORTE_EXCEPCIONES = "GENERAR_REPORTE_EXCEPCIONES";
            public const string GENERAR_REPORTE_ROL = "GENERAR_REPORTE_ROL";
            public const string EXPORTAR_PERMISOS_EXCEL = "EXPORTAR_PERMISOS_EXCEL";
            public const string EXPORTAR_PERMISOS_PDF = "EXPORTAR_PERMISOS_PDF";
        }

        /// <summary>
        /// Acciones del módulo de Auditoría General
        /// </summary>
        public static class AuditoriaGeneral
        {
            public const string CONSULTAR_AUDITORIA = "CONSULTAR_AUDITORIA";
            public const string VER_DETALLE_AUDITORIA = "VER_DETALLE_AUDITORIA";
            public const string EXPORTAR_AUDITORIA_EXCEL = "EXPORTAR_AUDITORIA_EXCEL";
            public const string EXPORTAR_AUDITORIA_PDF = "EXPORTAR_AUDITORIA_PDF";
            public const string FILTRAR_AUDITORIA = "FILTRAR_AUDITORIA";
        }

        // ═══════════════════════════════════════════════════════════════
        // HELPER: CATEGORÍAS Y MÓDULOS DEL SISTEMA
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Categorías principales del sistema (solo las que existen actualmente)
        /// </summary>
        public static class Categorias
        {
            public const string SISTEMA = "SISTEMA";
        }

        /// <summary>
        /// Módulos del sistema (solo los que existen actualmente)
        /// </summary>
        public static class Modulos
        {
            public const string AUTENTICACION = "Autenticación";
            public const string GESTION_USUARIOS = "Gestión de Usuarios";
            public const string GESTION_ROLES = "Gestión de Roles";
            public const string AUDITORIA_GENERAL = "Auditoría General";
        }
    }
}
