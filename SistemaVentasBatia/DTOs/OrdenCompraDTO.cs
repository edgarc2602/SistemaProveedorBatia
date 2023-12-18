using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaProveedoresBatia.DTOs
{
    public class OrdenCompraDTO
    {
        public int IdOrden { get; set; }
        public string Tipo { get; set; }
        public string Estatus { get; set; }
        public string FechaAlta { get; set; }
        public string Empresa { get; set; }
        public string Proveedor { get; set; }
        public string Cliente { get; set; }
        public string Elabora { get; set; }
        public decimal Total { get; set; }
        public string Observacion { get; set; }
        public int Inventario { get; set; }
        public decimal Facturado { get; set; }
    }
}
