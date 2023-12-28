using System;

namespace SistemaVentasBatia.Models
{
    public class EstadoDeCuentaDTO
    {
        public string Nombre { get; set; }
        public string Factura { get; set; }
        public decimal Total { get; set; }
        public decimal Pago { get; set; }
        public decimal Saldo { get; set; }
        public DateTime Ffactura { get; set; }
        public DateTime Fvencimiento { get; set; }
        public int DiasCredito { get; set; }
        public int DiasVencido { get; set; }
        public decimal Corriente { get; set; }
        public decimal Mes1 { get; set; }
        public decimal Mes2 { get; set; }
        public decimal Mes3 { get; set; }
        public decimal Mes4 { get; set; }
    }
}
