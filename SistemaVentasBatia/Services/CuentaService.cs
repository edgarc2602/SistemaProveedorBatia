using AutoMapper;
using SistemaProveedoresBatia.DTOs;
using SistemaVentasBatia.Controllers;
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
        Task<ListadoEstadoDeCuentaDTO> GetEstadoDeCuenta(ListadoEstadoDeCuentaDTO estadodecuenta, int idProveedor);
        Task<List<ListaEvaluacionProveedorDTO>> GetListadoEvaluacionProveedor(int idProveedor);
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

        public async Task<ListadoEstadoDeCuentaDTO> GetEstadoDeCuenta(ListadoEstadoDeCuentaDTO estadodecuenta, int idProveedor)
        {
            estadodecuenta.Rows = await _CuentaRepo.ContarEstadoDeCuenta(idProveedor);

            if (estadodecuenta.Rows > 0)
            {
                estadodecuenta.NumPaginas = (estadodecuenta.Rows / 40);

                if (estadodecuenta.Rows % 40 > 0)
                {
                    estadodecuenta.NumPaginas++;
                }
                estadodecuenta.EstadosDeCuenta = _mapper.Map<List<EstadoDeCuentaDTO>>(await _CuentaRepo.GetEstadoDeCuenta(idProveedor, estadodecuenta.Pagina));
            }
            else
            {
                estadodecuenta.EstadosDeCuenta = new List<EstadoDeCuentaDTO>();
            }
            return estadodecuenta;
        }

        public async Task <List<ListaEvaluacionProveedorDTO>> GetListadoEvaluacionProveedor( int idProveedor)
        {
            var evaluaciones = new List<ListaEvaluacionProveedorDTO>();
            evaluaciones = _mapper.Map<List<ListaEvaluacionProveedorDTO>>(await _CuentaRepo.GetListaEvaluaciones(idProveedor));
            foreach (var item in evaluaciones) {
                if (!string.IsNullOrEmpty(item.FechaEvaluacion))
                {
                    item.FechaEvaluacion = char.ToUpper(item.FechaEvaluacion[0]) + item.FechaEvaluacion.Substring(1);
                }
                item.Evaluacion = _mapper.Map<List<EvaluacionProveedorDTO>>(await _CuentaRepo.GetEvaluacionProveedor(item.IdEvaluacionProveedor));
            }
            return evaluaciones;
        }
    }
}
