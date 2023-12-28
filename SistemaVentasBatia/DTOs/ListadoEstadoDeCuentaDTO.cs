using SistemaVentasBatia.Models;
using System.Collections.Generic;

namespace SistemaVentasBatia.DTOs
{
    public class ListadoEstadoDeCuentaDTO
    {
        public List<EstadoDeCuentaDTO> EstadosDeCuenta { get; set; }
        public int Pagina { get; set; }
        public int Rows { get; set; }
        public int NumPaginas { get; internal set; }
    }
}
