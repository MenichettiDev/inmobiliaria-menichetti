namespace InmobiliariaApp.Models
{
    // Modelo Inmueble
    public class Inmueble
    {
        public int IdInmueble { get; set; }
        public int IdPropietario { get; set; } // Clave foránea
        public string Direccion { get; set; }
        public string Coordenadas { get; set; }
        public string Uso { get; set; } // "comercial" o "residencial"
        public int IdTipoInmueble { get; set; } // Clave foránea
        public int Ambientes { get; set; }
        public decimal Precio { get; set; }
        public string Estado { get; set; } // "disponible", "suspendido", "ocupado"

        
    }
}