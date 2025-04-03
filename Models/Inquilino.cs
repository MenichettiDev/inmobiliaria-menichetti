namespace InmobiliariaApp.Models
{
    public class Inquilino
    {
        public int IdInquilino { get; set; }
        public required string Dni { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public required string Telefono { get; set; }
        public required string Email { get; set; }
    }
}