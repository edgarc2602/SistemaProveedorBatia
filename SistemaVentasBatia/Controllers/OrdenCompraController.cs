using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Services;
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenCompraController : ControllerBase
    {
        private readonly IOrdenCompraService _logic;

        public OrdenCompraController(IOrdenCompraService logic)
        {
            _logic = logic;
        }

        [HttpGet("[action]/{idOrdenCompra}")]
        public Task<ResultadoDTO> GetOrdenCompra(int idOrdenCompra)
        {
            return _logic.GetOrdenCompra(idOrdenCompra);
        }

        [HttpGet("[action]/{idOrdenCompra}")]
        public Task<ResultadoDTO> GetOrdenCompraDetalle(int idOrdenCompra) {
            return _logic.GetOrdenCompraDetalle(idOrdenCompra);
        }

        [HttpGet("[action]/{idOrdenCompra}")]
        public Task<ResultadoDTO> GetSolicitudRecursoOrdenCompra(int idOrdenCompra)
        {
            return _logic.GetSolicitudRecursoOrdenCompra(idOrdenCompra);
        }
    }
}