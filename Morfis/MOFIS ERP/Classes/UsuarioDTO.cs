using System;

namespace MOFIS_ERP.Classes
{
    /// <summary>
    /// Data Transfer Object para transportar información del usuario
    /// Se usa para pasar datos entre capas sin exponer la estructura de la BD
    /// </summary>
    public class UsuarioDTO
    {
        public int UsuarioID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public int RolID { get; set; }
        public string NombreRol { get; set; }
        public bool Activo { get; set; }
        public bool DebeCambiarPassword { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? UltimoAcceso { get; set; }
    }
}