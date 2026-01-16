using SistemaVentasBatia.Models;
using System.Collections.Generic;

namespace SistemaVentasBatia.DTOs
{
    public class RequisicionDetalleDTO
    {
        public int IdRequisicion { get; set; }

        public List<RequisicionProductoDTO> Productos { get; set; }
    }
}
