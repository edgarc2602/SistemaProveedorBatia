using System.Collections.Generic;

namespace SistemaVentasBatia.Models
{
    public class Resultado
    {
        public bool? Estatus { get; set; }
        public string? Mensaje { get; set; }
        public string? MensajeError { get; set; }
        public object? Objeto { get; set; }
        public List<object>? Objetos { get; set; }
    }
}
