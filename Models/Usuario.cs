namespace InmobiliariaApp.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; } // "Admin" o "Empleado"
    }
}