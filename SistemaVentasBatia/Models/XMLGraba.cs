namespace SistemaVentasBatia.Models
{
    public class XMLGraba
    {
        public string Factura { get; set; }
        public int IdCliente { get; set; }
        public int IdOrden { get; set; }
        public int IdPersonal { get; set; }
        public string FechaFactura { get; set; }
        public int Dias { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public string PdfName { get; set; }
        public string XmlName { get; set; }
        public string Uuid { get; set; }
    }
}
