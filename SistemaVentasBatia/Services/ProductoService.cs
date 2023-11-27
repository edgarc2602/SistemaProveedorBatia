using AutoMapper;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using SistemaVentasBatia.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;
using SistemaProveedoresBatia.Controllers;

namespace SistemaVentasBatia.Services
{
    public interface IProductoService
    {
        //Task<bool> AgregarServicio(string servicio, int idPersonal);
        
    }

    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository repo;
        private readonly IMapper mapper;

        public ProductoService(IProductoRepository repository, IMapper imapper)
        {
            repo = repository;
            mapper = imapper;
        }
        //public async Task<bool> DeleteEquipo(int id)
        //{
        //    return await repo.EliminarEquipoPuesto(id);
        //}
    }
}