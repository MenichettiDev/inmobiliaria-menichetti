using System.ComponentModel.DataAnnotations.Schema;
using Inmobiliaria_.Net_Core.Models;

namespace InmobiliariaApp.Models
{
    // Modelo Inmueble
    public class Inmueble
    {
        public int IdInmueble { get; set; }
        public string? Nombre { get; set; } // Clave foránea
        public int IdPropietario { get; set; } // Clave foránea
        public string? Direccion { get; set; }
        public string? Coordenadas { get; set; } 
        public string? Uso { get; set; } // "comercial" o "residencial"
        public int IdTipoInmueble { get; set; } // Clave foránea
        public int Ambientes { get; set; }
        public decimal Precio { get; set; }
        public string? Estado { get; set; } // "disponible", "suspendido", "ocupado"
        public int Activo { get; set; } // "disponible", "suspendido", "ocupado"

        public string? Portada { get; set; }
        [ForeignKey(nameof(Imagen.InmuebleId))]
		public IList<Imagen> Imagenes { get; set; } = new List<Imagen>();
        [ForeignKey(nameof(Propietario.IdPropietario))]
		public Propietario? Duenio { get; set; }
		public TipoInmueble? Tipo { get; set; }
    }
}