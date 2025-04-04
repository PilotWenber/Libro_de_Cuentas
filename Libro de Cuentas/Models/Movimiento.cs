using System;

namespace Libro_de_Cuentas.Models
{
    public class Movimiento
    {
        public int Id { get; set; }
        public int PersonaId { get; set; }
        public string Tipo { get; set; } // "deuda" o "pago"
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
    }
}
