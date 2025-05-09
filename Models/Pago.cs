using System;

namespace InmobiliariaApp.Models
{
    public class Pago
    {
        public int IdPago { get; set; }
        public int IdContrato { get; set; } // Clave foránea
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaPago { get; set; }
        public int NumeroCuota { get; set; }
        public decimal Importe { get; set; }
        public string? Detalle { get; set; }
        public string? Estado { get; set; }
        public int? CreadoPor { get; set; } // Clave foránea (Usuario)
        public int? ModificadoPor { get; set; } // Clave foránea (Usuario)
        public int? EliminadoPor { get; set; } // Clave foránea (Usuario)
        public Contrato? Contrato { get; set; } // navegación
    }
}