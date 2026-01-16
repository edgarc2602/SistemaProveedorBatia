using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;

namespace SistemaVentasBatia.Models
{
    public class RequisicionDetalle
    {
        public int IdRequisicion { get; set; }

        public List<RequisicionProducto> Productos { get; set; }


        // Funcion que devuelve el modelo en formado XML
        public Resultado ConvertirModeloXML(RequisicionDetalle requisicion)
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
