﻿using AutoMapper;
using SistemaProveedoresBatia.DTOs;
using SistemaVentasBatia.Controllers;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Services
{
    public interface IEntregaService
    {
        
        Task<ListadoMaterialDTO> ObtenerListados(ListadoMaterialDTO listados, int mes, int anio, int idProveedor, int idEstado, int tipo);
        Task<List<DetalleMaterialDTO>> ObtenerMaterialesListado(int idListado);
        Task<ListadoAcuseEntregaDTO> ObtenerAcusesListado(int idListado);
    }

    public class EntregaService : IEntregaService
    {
        private readonly IEntregaRepository _entregaRepo;
        private readonly IMapper _mapper;

        public EntregaService(IEntregaRepository entregaRepo, IMapper mapper)
        {
            this._entregaRepo = entregaRepo;
            this._mapper = mapper;
        }
        public async Task<ListadoMaterialDTO> ObtenerListados(ListadoMaterialDTO listados, int mes, int anio, int idProveedor, int idEstado, int tipo)
        {
            listados.Rows = await _entregaRepo.ContarListados(mes, anio, idProveedor, idEstado, tipo);
            if (listados.Rows > 0)
            {
                listados.NumPaginas = (listados.Rows / 40);

                if (listados.Rows % 40 > 0)
                {
                    listados.NumPaginas++;
                }
                listados.Listas = _mapper.Map<List<ListadosDTO>>(await _entregaRepo.ObtenerListados(mes, anio, idProveedor, idEstado, tipo, listados.Pagina));
            }
            else
            {
                listados.Listas = new List<ListadosDTO>();
            }
            return listados;
        }

        public async Task<List<DetalleMaterialDTO>> ObtenerMaterialesListado(int idListado)
        {
            var materiales = new List<DetalleMaterialDTO>();
            materiales = _mapper.Map<List<DetalleMaterialDTO>>(await _entregaRepo.ObtenerMaterialesListado(idListado));
            return materiales;
        }

        public async Task<ListadoAcuseEntregaDTO> ObtenerAcusesListado(int idListado)
        {
            ListadoAcuseEntregaDTO listadoAcuses = new ListadoAcuseEntregaDTO();
            var acuses = new List<AcuseEntregaDTO>();
            acuses = _mapper.Map<List<AcuseEntregaDTO>>(await _entregaRepo.ObtieneAcusesListado(idListado));
            listadoAcuses.Acuses = acuses;
            listadoAcuses.IdListado = idListado;
            listadoAcuses.Carpeta = acuses[0].Carpeta;
            return listadoAcuses;
        }
    }
}
