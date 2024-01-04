﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace SistemaVentasBatia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        [HttpGet("[action]/{idOrden}")]
        public IActionResult DescargarReporteOrdenCompra(int idOrden = 0)
        {
            try
            {
                var url = ("http://192.168.2.4/Reporte?%2freporteordencompra&rs:Format=PDF&idOrden=" + idOrden.ToString());
                WebClient wc = new WebClient
                {
                    Credentials = new NetworkCredential("Administrador", "GrupoBatia@")
                };
                byte[] myDataBuffer = wc.DownloadData(url.ToString());
                return new FileContentResult(myDataBuffer, "application/pdf")
                {
                    FileDownloadName = "ReporteOrden" + idOrden.ToString() +".pdf"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el archivo PDF: {ex.Message}");
                return StatusCode(500, "Error al obtener el archivo PDF");
            }
        }
        [HttpGet("[action]/{idListado}")]
        public IActionResult DescargarReporteListadoMaterial(int idListado = 0)
        {
            try
            {
                var url = ("http://192.168.2.4/Reporte?%2freportelistadomateriales&rs:Format=PDF&idListado=" + idListado.ToString());
                WebClient wc = new WebClient
                {
                    Credentials = new NetworkCredential("Administrador", "GrupoBatia@")
                };
                byte[] myDataBuffer = wc.DownloadData(url.ToString());
                return new FileContentResult(myDataBuffer, "application/pdf")
                {
                    FileDownloadName = "ReporteListado" + idListado.ToString() + ".pdf"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el archivo PDF: {ex.Message}");
                return StatusCode(500, "Error al obtener el archivo PDF");
            }
        }
    }
}
