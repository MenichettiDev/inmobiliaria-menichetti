namespace InmobiliariaApp.Models
{
    public class Propietario
    {
        public int Id_propietario { get; set; } // Identificador único
        public string Dni { get; set; } = string.Empty; // DNI del propietario
        public string Apellido { get; set; } = string.Empty; // Apellido del propietario
        public string Nombre { get; set; } = string.Empty; // Nombre del propietario
        public string Telefono { get; set; } = string.Empty; // Teléfono de contacto
        public string Email { get; set; } = string.Empty; // Correo electrónico
    }
}