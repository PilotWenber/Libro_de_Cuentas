using System;

namespace Libro_de_Cuentas.Models
{
    public class Movimiento
    {
        public int Id { get; set; }
        public int PersonaId { get; set; }
        public string Tipo { get; set; } // "Deuda" o "Pago"
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
    }
}
