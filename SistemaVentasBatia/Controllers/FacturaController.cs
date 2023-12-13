using AutoMapper;
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
using System.Xml.Serialization;

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
        [HttpGet("[action]/{idProveedor}/{pagina}/{fechaInicio}/{fechaFin}")]
        public async Task<ActionResult<ListadoOrdenCompraDTO>> ObtenerOrdenesCompra(int idProveedor = 0, int pagina = 1, string fechaInicio = null, string fechaFin = null)
        {
            fechaInicio = fechaInicio + " 00:00:00";
            fechaFin = fechaFin + " 23:59:59";
            ListadoOrdenCompraDTO ordenescompra = new ListadoOrdenCompraDTO();
            ordenescompra.Pagina = pagina;
            return await _logic.ObtenerOrdenesCompra(ordenescompra, idProveedor, fechaInicio, fechaFin);
        }

        [HttpGet("[action]/{idOrden}")]
        public async Task<decimal> ObtenerSumaFacturas(int idOrden)
        {
            return await _logic.ObtenerSumaFacturas(idOrden);
        }


        private readonly string FolderPath = "C:\\Users\\LAP_Sistemas5\\Desktop\\SINGA_NEW\\Doctos\\compras\\";

        [HttpPost("[action]/{idOrden}")]
        public async Task<bool> InsertarFacturas([FromForm] IFormFile xml, [FromForm] IFormFile pdf, int idOrden)
        {
            string directorio = FolderPath + idOrden.ToString();
            bool result;
            if (!Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio);
            }
            try
            {
                //Guardar PDF
                var pdfFilePath = Path.Combine(directorio, pdf.FileName);
                using (var stream = new FileStream(pdfFilePath, FileMode.Create))
                {
                    await pdf.CopyToAsync(stream);
                }
                //Guardar XML
                var xmlFilePath = Path.Combine(directorio, xml.FileName);
                using (var stream = new FileStream(xmlFilePath, FileMode.Create))
                {
                    await xml.CopyToAsync(stream);
                }
                result = await _logic.ExtraerDatosXML(xml, idOrden);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("[action]/{idOrden}")]
        public async Task<List<FacturaDTO>> ObtenerFacturas(int idOrden)
        {
            return await _logic.ObtenerFacturas(idOrden);
        }

        [HttpPost("[action]")]
        public async Task<bool> InsertarFacturas([FromForm] IFormFile xml)
        {
        }
    }
}
