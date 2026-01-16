using AutoMapper.Configuration.Conventions;

namespace SistemaVentasBatia.Models
{
    public class SolicitudRecursoOrdenCompra
    {
        public int IdSolicitudRecurso { get; set; }
        public int IdEstatus { get; set; }
        public string Estatus { get; set; }
    }
}
