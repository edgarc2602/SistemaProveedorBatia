﻿using AutoMapper;
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
        Task<List<CatalogoDTO>> ObtenerEstados();
        Task<List<CatalogoDTO>> ObtenerServicios();
        Task<List<CatalogoDTO>> ObtenerMunicipios (int idEstado);
        Task<List<CatalogoDTO>> ObtenerTiposInmueble();
        Task<List<CatalogoDTO>> ObtenerCatalogoPuestos();
        Task<List<CatalogoDTO>> ObtenerCatalogoServicios();
        Task<List<CatalogoDTO>> ObtenerCatalogoTurnos();
        Task<List<CatalogoDTO>> ObtenerCatalogoSucursalesCotizacion(int idCotizacion);
        Task<List<CatalogoDTO>> ObtenerCatalogoPuestosCotizacion(int idCotizacion);
        Task<List<CatalogoDTO>> ObtenerCatalogoProductos(Servicio servicio);
        Task<List<CatalogoDTO>> ObtenerCatalogoJornada();
        Task<List<CatalogoDTO>> ObtenerCatalogoClase();
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

        public async Task<List<CatalogoDTO>> ObtenerEstados()
        {
            var estados = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerEstados());

            return estados;
        }

        public async Task<List<CatalogoDTO>> ObtenerServicios()
        {
            var servicios = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerServicios());

            return servicios;
        }

        public async Task<List<CatalogoDTO>> ObtenerMunicipios(int idEstado)
        {
            var municipios = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerMunicipios(idEstado));

            return municipios;
        }

        public async Task<List<CatalogoDTO>> ObtenerTiposInmueble()
        {
            var tiposInmueble = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerTiposInmueble());

            return tiposInmueble;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoPuestos()
        {
            var puestos = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoPuestos());

            return puestos;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoServicios()
        {
            var servicios = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoServicios());

            return servicios;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoTurnos()
        {
            var turnos = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoTurnos());

            return turnos;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoJornada()
        {
            var turnos = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoJornada());

            return turnos;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoClase()
        {
            var turnos = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoClase());

            return turnos;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoSucursalesCotizacion(int idCotizacion)
        {
            var sucursales = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoDireccionesCotizacion(idCotizacion));

            return sucursales;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoPuestosCotizacion(int idCotizacion)
        {
            var puestos = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoPuestosCotizacion(idCotizacion));

            return puestos;
        }

        public async Task<List<CatalogoDTO>> ObtenerCatalogoProductos(Servicio servicio)
        {
            var productos = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerCatalogoProductos(servicio));

            return productos;
        }

        public async Task<List<CatalogoDTO>> ObtenerMeses()
        {
            var meses = mapper.Map<List<CatalogoDTO>>(await catalogosRepo.ObtenerMeses());
            return meses;
        }
    }
}
