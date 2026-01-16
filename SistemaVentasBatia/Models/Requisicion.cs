using System.IO;
using System.Text;
using System.Xml.Serialization;
using System;

namespace SistemaVentasBatia.Models
{
    public class Requisicion
    {
        public int IdRequisicion { get; set; }
        public int IdProveedor { get; set; }
        public string Comprador { get; set; }
        public string Comentarios { get; set; }
        public string FechaAlta { get; set; }
        public int IdEstatus { get; set; }
        public string Estatus { get; set; }
        public float Iva { get; set; }
        public float IvaPorcentaje { get; set; }
        public float SubTotal { get; set; }
        public float Total { get; set; }
        public float IvaNuevo { get; set; }
        public float SubTotalNuevo { get; set; }
        public float TotalNuevo { get; set; }
        public int IdOrdenCompra { get; set; }

        // Funcion que devuelve el modelo en formado XML
        public Resultado ConvertirModeloXML(Requisicion requisicion)
        {

            Resultado resultado = new Resultado();

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(RequisicionDetalle));
                using (StringWriter stringWriter = new StringWriter(new StringBuilder()))
                {
                    serializer.Serialize(stringWriter, requisicion);
                    resultado.Estatus = true;
                    resultado.Mensaje = stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                resultado.Estatus = false;
                resultado.Mensaje = "Error: " + ex.Message;
                resultado.MensajeError = ex.ToString();
            }

            return resultado;
        }
    }
}
