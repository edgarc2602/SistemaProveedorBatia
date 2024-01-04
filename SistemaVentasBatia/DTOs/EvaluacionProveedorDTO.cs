namespace SistemaVentasBatia.Models
{
    public class EvaluacionProveedorDTO
    {
        public int  IdEvaluacionProveedor { get; set; }
        public int IdCaracteristica { get; set; }
        public string Descripcion { get; set; }
        public decimal Calificacion { get; set; }
        public string Criterios { get; set; }
    }
}
