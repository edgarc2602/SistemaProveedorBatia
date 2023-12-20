using System.Drawing;

namespace SistemaVentasBatia.Models
{
    public class GraficaListado
    {
        public int Mes { get; set; }
        public int TotalListadosPorMes { get; set; }
        public int Alta { get; set; }
        public int Aprobado { get; set; }
        public int Despachado { get; set; }
        public int Entregado { get; set; }
        public int Cancelado { get; set; }
    }
}
