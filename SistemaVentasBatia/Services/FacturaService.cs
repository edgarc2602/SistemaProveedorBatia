using AutoMapper;
using Microsoft.AspNetCore.Http;
using SistemaProveedoresBatia.Controllers;
using SistemaProveedoresBatia.DTOs;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Services
{
    public interface IFacturaService
    {
        Task<ListadoOrdenCompraDTO> ObtenerOrdenesCompra(ListadoOrdenCompraDTO ordenescompra, int idProveedor, string fechaInicio, string fechaFin);
        Task<decimal> ObtenerSumaFacturas(int idOrden);
        Task<List<FacturaDTO>> ObtenerFacturas(int idOrden);
        Task<bool> ExtraerDatosXML(IFormFile xml, int idOrden);
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

        public async Task<ListadoOrdenCompraDTO> ObtenerOrdenesCompra(ListadoOrdenCompraDTO ordenescompra, int idProveedor, string fechaInicio, string fechaFin)
        {
            ordenescompra.Rows = await _FacturaRepo.ContarOrdenesCompra(idProveedor, fechaInicio, fechaFin);
            if (ordenescompra.Rows > 0)
            {
                ordenescompra.NumPaginas = (ordenescompra.Rows / 40);

                if (ordenescompra.Rows % 40 > 0)
                {
                    ordenescompra.NumPaginas++;
                }
                ordenescompra.Ordenes = _mapper.Map<List<OrdenCompraDTO>>(await _FacturaRepo.ObtenerOrdenesCompra(idProveedor, fechaInicio, fechaFin, ordenescompra.Pagina));
            }
            else
            {
                ordenescompra.Ordenes = new List<OrdenCompraDTO>();
            }
            return ordenescompra;
        }

        public async Task<decimal> ObtenerSumaFacturas(int idOrden)
        {
            return await _FacturaRepo.ObtenerSumaFacturas(idOrden);
        }

        public async Task<List<FacturaDTO>> ObtenerFacturas(int idOrden)
        {
            var facturas = new List<FacturaDTO>();
            facturas = _mapper.Map<List<FacturaDTO>>(await _FacturaRepo.ObtenerFacturas(idOrden));
            return facturas;
        }

        public async Task<bool> ExtraerDatosXML(IFormFile xml, int idOrden)
        {
            await _FacturaRepo.ExtraerDatosXML(xml, idOrden);
            return true;
        }
    }
}
