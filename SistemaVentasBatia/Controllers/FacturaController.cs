using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaProveedoresBatia.DTOs;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace SistemaVentasBatia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturaController : ControllerBase
    {
        private readonly IFacturaService _logic;
        public FacturaController(IFacturaService logic)
        {
            _logic = logic;
        }
        [HttpGet("[action]/{idProveedor}/{pagina}/{fechaInicio}/{fechaFin}/{idStatus}")]
        public async Task<ActionResult<ListadoOrdenCompraDTO>> ObtenerOrdenesCompra(int idProveedor = 0, int pagina = 1, string fechaInicio = null, string fechaFin = null, int idStatus = 0)
        {
            fechaInicio += " 00:00:00";
            fechaFin += " 23:59:59";
            var ordenescompra = new ListadoOrdenCompraDTO
            {
                Pagina = pagina
            };
            return await _logic.ObtenerOrdenesCompra(ordenescompra, idProveedor, fechaInicio, fechaFin, idStatus);
        }

        [HttpGet("[action]/{idOrden}")]
        public async Task<decimal> ObtenerSumaFacturas(int idOrden)
        {
            return await _logic.ObtenerSumaFacturas(idOrden);
        }

        private readonly string FolderPath = "C:\\Users\\LAP_Sistemas5\\Desktop\\SINGA_NEW\\Doctos\\compras\\";

        [HttpPost("[action]/{idOrden}")]
        public async Task<bool> InsertarFacturasCarpeta([FromForm] IFormFile xml, [FromForm] IFormFile pdf, int idOrden)
        {
            bool result;
            string directorio = FolderPath + idOrden.ToString();
            if (!Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio);
            }
            try
            {
                var pdfFilePath = Path.Combine(directorio, pdf.FileName);
                using (var stream = new FileStream(pdfFilePath, FileMode.Create))
                {
                    await pdf.CopyToAsync(stream);
                }
                var xmlFilePath = Path.Combine(directorio, xml.FileName);
                using (var stream = new FileStream(xmlFilePath, FileMode.Create))
                {
                    await xml.CopyToAsync(stream);
                }
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        [HttpPost("[action]")]
        public async Task<bool> InsertarFacturasXML([FromBody] XMLGraba xml)
        {
            string[] documentos = new string[2];
            documentos[0] = xml.PdfName;
            documentos[1] = xml.XmlName;
            bool result;
            var xmlDoc = new XmlDocument();
            var movimientoElement = xmlDoc.CreateElement("Movimiento");
            var salidaElement = xmlDoc.CreateElement("salida");
            salidaElement.SetAttribute("documento", "3");
            salidaElement.SetAttribute("almacen1", "0");
            salidaElement.SetAttribute("factura", xml.Factura);
            salidaElement.SetAttribute("cliente", xml.IdCliente.ToString());
            salidaElement.SetAttribute("almacen", "0");
            salidaElement.SetAttribute("orden", xml.IdOrden.ToString());
            salidaElement.SetAttribute("usuario", xml.IdPersonal.ToString());
            salidaElement.SetAttribute("fecfac", xml.FechaFactura);
            salidaElement.SetAttribute("idproveedor", xml.IdPersonal.ToString());
            salidaElement.SetAttribute("dias", xml.Dias.ToString());
            salidaElement.SetAttribute("sub", xml.SubTotal.ToString());
            salidaElement.SetAttribute("iva", xml.Iva.ToString());
            salidaElement.SetAttribute("total", xml.Total.ToString());
            salidaElement.SetAttribute("uuid", xml.Uuid);
            movimientoElement.AppendChild(salidaElement);
            foreach (var fileName in documentos)
            {
                var archivoElement = xmlDoc.CreateElement("archivo");
                archivoElement.SetAttribute("nombre", fileName);
                movimientoElement.AppendChild(archivoElement);
            }
            xmlDoc.AppendChild(movimientoElement);
            string xmlString = xmlDoc.OuterXml;
            result = await _logic.InsertarXML(xmlString);
            Console.WriteLine(xmlString);
            return result;
        }

        [HttpGet("[action]/{idOrden}")]
        public async Task<List<FacturaDTO>> ObtenerFacturas(int idOrden)
        {
            return await _logic.ObtenerFacturas(idOrden);
        }

        [HttpPost("[action]/{idTipoFolio}")]
        public async Task<XMLData> ObtenerDatosXML([FromForm] IFormFile xml, int idTipoFolio)
        {
            return await _logic.ExtraerDatosXML(xml, idTipoFolio);
        }

        [HttpGet("[action]/{idOrden}")]
        public async Task<DetalleOrdenCompra> ObtenerDetalleOrden(int idOrden)
        {
            return await _logic.ObtenerDetalleOrden(idOrden);
        }
        [HttpGet("[action]/{uuid}")]
        public async Task<bool> FacturaExiste(string uuid)
        {
            return await _logic.FacturaExiste(uuid);
        }

        [HttpPost("[action]/{idOrden}")]
        public async Task<bool> CambiarStatusOrdenCompleta(int idOrden)
        {
            return await _logic.CambiarStatusOrdenCompleta(idOrden);
        }

        [HttpGet("[action]")]
        public async Task<List<CatalogoDTO>> GetStatusOrdenCompra()
        {
            return await _logic.GetStatusOrdenCompra();
        }
    }
}