using SistemaProveedoresBatia.DTOs;
using System.Collections.Generic;

namespace SistemaVentasBatia.DTOs
{
    public class ListadoDetalleMaterialDTO
    {
        public List<DetalleMaterialDTO> Materiales { get; set; }
        public int Pagina { get; set; }
        public int Rows { get; set; }
        public int NumPaginas { get; internal set; }
    }
}

