using AutoMapper;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using SistemaVentasBatia.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;
using Microsoft.Extensions.Options;
using SistemaProveedoresBatia.Controllers;

namespace SistemaVentasBatia.Services
{
    public interface ICatalogosService
    {
        Task<List<CatalogoDTO>> ObtenerMeses();
    }

    public class CatalogosService : ICatalogosService
    {
        private readonly ICatalogosRepository catalogosRepo;
        private readonly IMapper mapper;

        public CatalogosService(ICatalogosRepository catalogosRepo, IMapper mapper)
        {
            this.catalogosRepo = catalogosRepo;
            this.mapper = mapper;
        }

        public async Task<List<CatalogoDTO>> ObtenerMeses()
        {
            var meses = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerMeses());
            return meses;
        }
    }
}
