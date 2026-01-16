using System.ComponentModel;

namespace SistemaVentasBatia.Models
{
    public class OrdenCompraProducto
    {
        public string Clave { get; set; }
        public string Producto { get; set; }
        public string UnidadMedida { get; set; }
        public float Cantidad { get; set; }
        public float Precio { get; set; }
    }
}
