namespace SistemaVentasBatia.DTOs
{
    public class DetalleMaterialDTO
    {
        public int IdListado { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public string Unidad { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Total { get; set; }
    }
}
