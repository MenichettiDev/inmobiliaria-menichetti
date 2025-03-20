using System;

namespace InmobiliariaApp.Models
{
    public class Contrato
    {
        public int IdContrato { get; set; } // Identificador único
        public int IdInquilino { get; set; } // ID del inquilino (relación)
        public int IdInmueble { get; set; } // ID del inmueble (relación)
        public DateTime FechaInicio { get; set; } // Fecha de inicio del contrato
        public DateTime FechaFin { get; set; } // Fecha de finalización del contrato
        public decimal MontoMensual { get; set; } // Monto mensual del alquiler
        public bool Vigente { get; set; } // Estado del contrato (vigente o no)
    }
}