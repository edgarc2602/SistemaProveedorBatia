using System.ComponentModel;

namespace SistemaVentasBatia.Models
{
    public class RequisicionProducto
    {
        public string Clave { get; set; }
        public string Producto { get; set; }
        public float Cantidad { get; set; }
        public float Precio { get; set; }
        public float PrecioNuevo { get; set; }
    }
}
