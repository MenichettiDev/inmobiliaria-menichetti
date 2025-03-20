namespace InmobiliariaApp.Models
{
    public class Inmueble
    {
        public int IdInmueble { get; set; } // Identificador único
        public int IdPropietario { get; set; } // ID del propietario (relación)
        public string Direccion { get; set; } = string.Empty; // Dirección del inmueble
        public string Uso { get; set; } = string.Empty; // Uso (comercial o residencial)
        public string Tipo { get; set; } = string.Empty; // Tipo (local, depósito, casa, etc.)
        public int Ambientes { get; set; } // Cantidad de ambientes
        public decimal Precio { get; set; } // Precio del alquiler
        public bool Estado { get; set; } // Estado de disponibilidad
    }
}