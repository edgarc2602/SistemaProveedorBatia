using Microsoft.VisualBasic;
using System.Collections.Generic;

namespace SistemaVentasBatia.Models
{
    public class ListaEvaluacionProveedorDTO
    {
        public List<EvaluacionProveedorDTO> Evaluacion { get; set; }
        public int IdEvaluacionProveedor { get; set; }
        public int IdProveedor { get; set; }
        public int IdStatus { get; set; }
        public string FechaEvaluacion { get; set; }
        public string NumeroContrato { get; set; }
        public decimal Promedio { get; set; }
        public string TextoPromedio { get; set; }
        public int IdUsuario { get; set; }
    }
}
