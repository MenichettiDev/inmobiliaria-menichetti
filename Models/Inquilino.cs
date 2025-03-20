namespace InmobiliariaApp.Models
{
    public class Inquilino
    {
        public int IdInquilino { get; set; } // Identificador único
        public string Dni { get; set; } = string.Empty; // DNI del inquilino
        public string Nombre { get; set; } = string.Empty; // Nombre del inquilino
        public string Apellido { get; set; } = string.Empty; // Apellido del inquilino
        public string Telefono { get; set; } = string.Empty; // Teléfono de contacto
        public string Email { get; set; } = string.Empty; // Correo electrónico
    }
}