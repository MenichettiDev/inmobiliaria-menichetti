namespace InmobiliariaApp.Models
{
    public class Inquilino
    {
        public int IdInquilino { get; set; }
        public string? Dni { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
    }
}