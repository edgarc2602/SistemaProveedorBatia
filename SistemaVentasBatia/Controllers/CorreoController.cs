using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaProveedoresBatia.DTOs;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class CorreoController : ControllerBase
    {


        private readonly ICorreoService _logic;

        public CorreoController(ICorreoService logic)
        {
            _logic = logic;
        }

        [HttpPost("[action]/{proveedor}")]
        public async Task<ResultadoDTO> EnviarCorreoComprador(RequisicionDTO requisicion, string proveedor)
        {
            // Obtiene el correo del comprador
            ResultadoDTO resultadoCorreos = await _logic.GetCorreoCompradorPorRequisicion(requisicion.IdRequisicion);
            
            string contactos = "diegot@grupobatia.com.mx";
            string asunto = "NOTIFICACION DE SINGA, Cambio de precios de requisicion por el proveedor";
            string titulo = "Cambio de precios de requisicion";
            string mensaje = $"Estimado comprador, el proveedor <b>{proveedor}</b> a realizado cambios en los productos de la requisicion no. {requisicion.IdRequisicion}";
            string mensaje_lista_xml = "";

            return await _logic.EnviaCorreo(contactos, asunto, titulo, mensaje, mensaje_lista_xml);
        }

    }
}
