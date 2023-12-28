using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentaController : ControllerBase
    {
        private readonly ICuentaService _logic;

        public CuentaController(ICuentaService logic)
        {
            _logic = logic;
        }

        [HttpGet("[action]/{idProveedor}/{pagina}")]
        public async Task <ListadoEstadoDeCuentaDTO> GetEstadoDeCuenta(int idProveedor = 0, int pagina = 1)
        {
            var estadodecuenta = new ListadoEstadoDeCuentaDTO
            {
                Pagina = pagina
            };
            return await _logic.GetEstadoDeCuenta(estadodecuenta,idProveedor);
        }
    }
}
