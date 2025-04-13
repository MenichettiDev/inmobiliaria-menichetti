namespace InmobiliariaApp.Models
{
    // Modelo Inmueble
    public class Inmueble
    {
        public int IdInmueble { get; set; }
        public int IdPropietario { get; set; } // Clave foránea
        public required string Direccion { get; set; }
        public string? Coordenadas { get; set; } 
        public required string Uso { get; set; } // "comercial" o "residencial"
        public int IdTipoInmueble { get; set; } // Clave foránea
        public int Ambientes { get; set; }
        public decimal Precio { get; set; }
        public required string Estado { get; set; } // "disponible", "suspendido", "ocupado"
        public required int Activo { get; set; } // "disponible", "suspendido", "ocupado"

        public string? Portada { get; set; }
    }
}