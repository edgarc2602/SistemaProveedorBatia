﻿using AutoMapper;
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
    public interface IMaterialService
    {
        //Task<ServicioCotizacionDTO> ServicioGetById(int id);
    }

    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _materialRepo;
        private readonly IMapper _mapper;

        public MaterialService(IMaterialRepository materialRepo, IMapper mapper)
        {
            _materialRepo = materialRepo;
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
