using System.Collections.Generic;

namespace SistemaVentasBatia.Models
{
    public class OrdenCompraDetalle
    {
        public int IdOrdenCompra { get; set; }
        public List<OrdenCompraProducto> Productos { get; set; }
    }
}
