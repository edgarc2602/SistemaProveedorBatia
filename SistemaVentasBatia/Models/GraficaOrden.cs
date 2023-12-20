namespace SistemaVentasBatia.Models
{
    public class GraficaOrden
    {
        public int Mes { get; set; }
        public int TotalOrdenesPorMes { get; set; }
        public int Alta { get; set; }
        public int Autorizada { get; set; }
        public int Rechazada { get; set; }
        public int Completa { get; set; }
        public int Despachada { get; set; }
        public int EnRequisicion { get; set; }
    }
}
