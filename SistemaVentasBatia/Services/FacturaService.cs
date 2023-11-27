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
    public interface IFacturaService
    {
        //Task<ServicioCotizacionDTO> ServicioGetById(int id);
    }

    public class FacturaService : IFacturaService
    {
        private readonly IFacturaRepository _FacturaRepo;
        private readonly IMapper _mapper;

        public FacturaService(IFacturaRepository FacturaRepo, IMapper mapper)
        {
            _FacturaRepo = FacturaRepo;
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
