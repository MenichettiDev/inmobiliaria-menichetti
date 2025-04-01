using System.ComponentModel.DataAnnotations;

namespace InmobiliariaApp.Models
{
   // Modelo Propietario
    public class Propietario
    {
        public int IdPropietario { get; set; }
        public string Dni { get; set; } // DNI del propietario
        public string Nombre { get; set; } 
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }

    }
}