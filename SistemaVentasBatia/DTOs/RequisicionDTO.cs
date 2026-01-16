namespace SistemaVentasBatia.DTOs
{
    public class RequisicionDTO
    {
        public int IdRequisicion { get; set; }
        public int IdProveedor { get; set; }
        public string Comprador { get; set; }
        public string Comentarios { get; set; }
        public string FechaAlta { get; set; }
        public int IdEstatus { get; set; }
        public string Estatus { get; set; }
        public float Iva { get; set; }
        public float IvaPorcentaje { get; set; }
        public float SubTotal { get; set; }
        public float Total { get; set; }
        public float IvaNuevo { get; set; }
        public float SubTotalNuevo { get; set; }
        public float TotalNuevo { get; set; }
        public int idOrdenCompra { get; set; }
    }
}
