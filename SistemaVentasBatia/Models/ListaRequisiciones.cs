using System.Collections.Generic;

namespace SistemaVentasBatia.Models
{
    public class ListaRequisiciones
    {
        public int Pagina { get; set; }
        public int NumPaginas { get; set; }
        public int Rows { get; set; }
        public List<Requisicion> Requisiciones { get; set; }
    }
}
