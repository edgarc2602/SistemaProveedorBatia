using System.Collections.Generic;

namespace SistemaVentasBatia.DTOs
{
    public class ListaRequisicionesDTO
    {
        public int Pagina { get; set; }
        public int NumPaginas { get; set; }
        public int Rows { get; set; }
        public List<RequisicionDTO> Requisiciones { get; set; }
    }
}
