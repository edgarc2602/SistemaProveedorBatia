using SistemaVentasBatia.Models;
using System.Collections.Generic;

namespace SistemaVentasBatia.DTOs
{
    public class OrdenCompraDetalleDTO
    {
        public int IdRequisicion { get; set; }

        public List<OrdenCompraProductoDTO> Productos { get; set; }
    }
}
