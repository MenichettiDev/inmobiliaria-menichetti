using System;

namespace InmobiliariaApp.Models
{
    public class Contrato
    {
        public int IdContrato { get; set; }
        public int IdInquilino { get; set; } // Clave foránea
        public int IdInmueble { get; set; } // Clave foránea
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal MontoMensual { get; set; }
        public required string Estado { get; set; } // "vigente" o "terminado"
        public DateTime? FechaTerminacionAnticipada { get; set; } // Opcional
        public decimal? Multa { get; set; } // Opcional
        public int? CreadoPor { get; set; } // Clave foránea (Usuario)
        public int? ModificadoPor { get; set; } // Clave foránea (Usuario)
        public int? EliminadoPor { get; set; } // Clave foránea (Usuario)
        public int Activo { get; set; }
    }
}