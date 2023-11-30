using SistemaVentasBatia.Models;
using System.Collections.Generic;

namespace SistemaVentasBatia.DTOs
{
    public class ListadoAcuseEntregaDTO
    {
        public List<AcuseEntregaDTO> Acuses { get; set; }
        public int IdListado { get; set; }
        public string Carpeta { get; set; }
    }
}
