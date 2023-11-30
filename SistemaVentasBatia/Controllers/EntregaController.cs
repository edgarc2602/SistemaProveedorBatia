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

namespace SistemaProveedoresBatia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntregaController : ControllerBase
    {
        private readonly IEntregaService _logic;

        public EntregaController(IEntregaService logic)
        {
            _logic = logic;
        }

        [HttpGet("[action]/{mes}/{anio}/{idProveedor}/{idEstado}/{tipo}/{pagina}")]
        public async Task<ActionResult<ListadoMaterialDTO>> ObtenerListados(int mes, int anio, int idProveedor, int idEstado, int tipo, int pagina = 1)
        {
            ListadoMaterialDTO listados = new ListadoMaterialDTO();
            listados.Pagina = pagina;
            return await _logic.ObtenerListados(listados, mes, anio, idProveedor, idEstado, tipo);
        }

        [HttpGet("[action]/{idListado}")]
        public async Task<List<DetalleMaterialDTO>> ObtenerMaterialesListado(int idListado)
        {
            return await _logic.ObtenerMaterialesListado(idListado);
        }

        [HttpGet("[action]/{idListado}")]
        public async Task<ListadoAcuseEntregaDTO> ObtenerAcusesListado(int idListado)
        {            
            return await _logic.ObtenerAcusesListado(idListado);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GuardarAcuse([FromBody] IFormFile formData)
        {
            try
            {
                if (formData == null || formData.Length == 0)
                {
                    return BadRequest("No se ha recibido ningún archivo.");
                }

                // Procesar el archivo, por ejemplo, guardarlo en el servidor
                var filePath = Path.Combine("RUTA_DE_TU_CARPETA_DESTINO", formData.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formData.CopyToAsync(stream);
                }

                // Realizar otras operaciones con el archivo si es necesario

                return Ok("Archivo guardado con éxito.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al guardar el archivo: {ex.Message}");
            }
            //return await _logic.GuardarAcuse(formData);
        }

        private readonly string _imageFolderPath = "C:/Users/LAP_Sistemas5/Desktop/SINGA_NEW/Doctos/entrega/";

        [HttpGet("getimage/{archivo}/{carpeta}")]
        public IActionResult GetImage(string archivo, string carpeta)
        {
            var imagePath = Path.Combine(_imageFolderPath, carpeta, archivo);

            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound();
            }
            var imageBytes = System.IO.File.ReadAllBytes(imagePath);
            return File(imageBytes, "image/jpeg"); // Asegúrate de que el tipo MIME sea correcto para tu imagen
        }
    }
}
