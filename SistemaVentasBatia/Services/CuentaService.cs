using AutoMapper;
using SistemaProveedoresBatia.Controllers;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Services
{
    public interface ICuentaService
    {
        //Task<ServicioCotizacionDTO> ServicioGetById(int id);
    }

    public class CuentaService : ICuentaService
    {
        private readonly ICuentaRepository _CuentaRepo;
        private readonly IMapper _mapper;

        public CuentaService(ICuentaRepository CuentaRepo, IMapper mapper)
        {
            _CuentaRepo = CuentaRepo;
            _mapper = mapper;
        }
        //public async Task ActualizarMaterialCotizacion(MaterialCotizacionDTO materialVM)
        //{
        //    var material = _mapper.Map<MaterialCotizacion>(materialVM);

        //    material.Total = (material.PrecioUnitario * material.Cantidad) / (int)material.IdFrecuencia;

        //    await _materialRepo.ActualizarMaterialCotizacion(material);
        //}
    }
}
