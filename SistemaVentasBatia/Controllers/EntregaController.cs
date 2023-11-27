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

        [HttpGet("[action]/{mes}/{anio}/{idPersonal}/{idEstado}/{tipo}/{pagina}")]
        public async Task<ActionResult<ListadoMaterialDTO>> ObtenerListados(int mes, int anio, int idPersonal, int idEstado, int tipo, int pagina = 1)
        {
            ListadoMaterialDTO listados = new ListadoMaterialDTO();
            listados.Pagina = pagina;
            return await _logic.ObtenerListados(listados,mes,anio,idPersonal,idEstado,tipo);
        }
    }
}
