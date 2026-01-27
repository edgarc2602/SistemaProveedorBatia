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
    public class RequisicionController : ControllerBase
    {
        private readonly IRequisicionService _logic;

        public RequisicionController(IRequisicionService logic)
        {
            _logic = logic;
        }

        [HttpGet("[action]/{idProveedor}/{pagina}")]
        public Task<ListaRequisicionesDTO> GetRequisiciones(int idProveedor, int pagina)
        {
            return _logic.GetRequisiciones(idProveedor, pagina);
        }

        [HttpGet("[action]/{idRequisicion}")]
        public Task<RequisicionDetalleDTO> GetRequisicionDetalle(int idRequisicion) {
            return _logic.GetRequisicionDetalle(idRequisicion);
        }

        [HttpPut("[action]/{ivaNuevo}/{subTotalNuevo}/{totalNuevo}/{idProveedor}")]
        public Task<ResultadoDTO> PutActualizaRequisicionNuevoPrecio([FromBody] RequisicionDetalleDTO requisicion, float ivaNuevo, float subTotalNuevo, float totalNuevo, int idProveedor) {
            return _logic.PutActualizaRequisicionNuevoPrecio(requisicion, ivaNuevo, subTotalNuevo, totalNuevo, idProveedor);
        }

        [HttpPost("[action]")]
        public async Task<bool> NuevaRequisicion(
            [FromForm] int idProveedor,
            [FromForm] int idOrdenCompra,
            [FromForm] int idPersonal,
            [FromForm] string diasCredito,
            [FromForm] string xmlMovimiento,
            [FromForm] string xmlComprobante,
            [FromForm] IFormFile pdf,
            [FromForm] IFormFile xmlFile) {
            return await _logic.NuevaRequisicion(
                idOrdenCompra,
                idProveedor,
                idPersonal,
                diasCredito,
                xmlMovimiento,
                xmlComprobante,
                pdf,
                xmlFile
            );
            //return _logic.NuevaRequisicion(requisicionNueva);
        }
    }
}