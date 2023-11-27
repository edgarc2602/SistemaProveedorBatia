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
    public class FacturaController : ControllerBase
    {
        private readonly IFacturaService _logic;

        public FacturaController(IFacturaService logic)
        {
            _logic = logic;
        }

        //[HttpGet("[action]")]
        //public async Task<ActionResult<UsuarioDTO>> Login(AccesoDTO dto)
        //{
        //    return await _logic.Login(dto);
        //}




        //[HttpPost("[action]")]
        //public async Task<ActionResult<bool>> AgregarUsuario([FromBody] UsuarioRegistro usuario)
        //{
        //    bool result = false;
        //    result = await _logic.InsertarUsuario(usuario);
        //    return true;
        //}
    }
}
