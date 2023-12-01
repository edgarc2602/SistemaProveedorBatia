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


        private readonly string _imageFolderPath = "C:/Users/LAP_Sistemas5/Desktop/SINGA_NEW/Doctos/entrega/";

        [HttpGet("getimage/{archivo}/{carpeta}")]
        public IActionResult GetImage(string archivo, string carpeta)
        {
            string imagePath = _imageFolderPath + carpeta + '/' + archivo;

            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound();
            }
            var imageBytes = System.IO.File.ReadAllBytes(imagePath);
            return File(imageBytes, "image/jpeg"); // Asegúrate de que el tipo MIME sea correcto para tu imagen
        }



        [HttpPost("[action]/{idListado}/{selectedFileName}")]
        public async Task<bool> GuardarAcuse(int idListado, string selectedFileName, IFormFile file)
        {
            string carpeta = "F" + DateTime.Now.ToString("yyyy_MM");
            bool result;
            string directorio = _imageFolderPath + '/' + carpeta;

            if (Directory.Exists(directorio))
            {
                try
                {
                    if (file == null || file.Length == 0)
                    {
                        throw new Exception();
                    }

                    var filePath = Path.Combine(directorio, file.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        result = await _logic.InsertaAcuse(idListado, carpeta, selectedFileName);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                Directory.CreateDirectory(directorio);
                try
                {
                    if (file == null || file.Length == 0)
                    {
                        throw new Exception();
                    }

                    var filePath = Path.Combine(directorio, file.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        result = await _logic.InsertaAcuse(idListado, carpeta, selectedFileName);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            
        }
    }
}
