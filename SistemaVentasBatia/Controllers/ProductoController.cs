using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService logic;

        public ProductoController(IProductoService service)
        {
            logic = service;
        }

        //[HttpGet("[action]/{id}")]
        //public async Task<IEnumerable<ProductoItemDTO>> GetMaterial(int id)
        //{
        //    return await logic.GetMaterialPuesto(id);
        //}

        //[HttpPost("[action]")]
        //public async Task<ActionResult<MaterialPuestoDTO>> PostMaterial(MaterialPuestoDTO dto)
        //{
        //    await logic.CreateMaterial(dto);
        //    return dto;
        //}

        //[HttpGet("[action]/{servicio}/{idPersonal}")]
        //public async Task<ActionResult<bool>> AgregarServicio(string servicio = "", int idPersonal = 0)
        //{
        //    return await logic.AgregarServicio(servicio,idPersonal);
        //}
    }
}
