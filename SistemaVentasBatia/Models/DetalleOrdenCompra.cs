namespace SistemaVentasBatia.Models
{
    public class DetalleOrdenCompra
    {
        public int IdOrden { get; set; }
        public int IdRequisicion { get; set; }
        public int IdProveedor { get; set; }
        public int IdCliente { get; set; }
        public string Proveedor { get; set; }
        public string Empresa { get; set; }
        public string Cliente { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public int Status { get; set; }
        public int Dias { get; set; }
    }
}