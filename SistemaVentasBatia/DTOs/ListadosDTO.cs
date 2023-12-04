using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaProveedoresBatia.DTOs
{
    public class ListadosDTO
    {
        public int IdInmueble { get; set; }
        public string NombreSucursal { get; set; }
        public string Prefijo { get; set; }
        public int IdListado { get; set; }
        public string Tipo { get; set; }
        public string Estatus { get; set; }
        public string FechaAlta { get; set; }
        public decimal Utilizado { get; set; }
        public string FechaEntrega { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public int IdCliente { get; set; }
        public int IdEstado { get; set; }
        public int TipoListado { get; set; }
    }
}
