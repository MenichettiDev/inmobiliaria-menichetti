using System;

namespace InmobiliariaApp.Models
{
    public class Pago
    {
        public int IdPago { get; set; } // Identificador único
        public int IdContrato { get; set; } // ID del contrato (relación)
        public DateTime FechaPago { get; set; } // Fecha en la que se realizó el pago
        public decimal Importe { get; set; } // Importe del pago
    }
}