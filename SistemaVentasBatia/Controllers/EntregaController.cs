using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaProveedoresBatia.DTOs;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Services;
using System.Collections.Generic;
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
            return await _logic.ObtenerListados(listados,mes,anio,idProveedor,idEstado,tipo);
        }

        [HttpGet("[action]/{idListado}")]
        public async Task <List<DetalleMaterialDTO>> ObtenerMaterialesListado(int idListado)
        {
            return await _logic.ObtenerMaterialesListado(idListado);
        }
    }
}
