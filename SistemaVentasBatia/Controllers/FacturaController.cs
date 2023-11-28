using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaProveedoresBatia.DTOs;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    }
}
