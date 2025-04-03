namespace InmobiliariaApp.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Rol { get; set; } // "Admin" o "Empleado"
    }
}