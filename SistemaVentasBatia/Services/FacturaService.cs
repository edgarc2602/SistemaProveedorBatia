using AutoMapper;
using Microsoft.AspNetCore.Http;
using SistemaVentasBatia.Controllers;
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
        Task<ListadoOrdenCompraDTO> ObtenerOrdenesCompra(ListadoOrdenCompraDTO ordenescompra, int idProveedor, string fechaInicio, string fechaFin, int idStatus);
        Task<decimal> ObtenerSumaFacturas(int idOrden);
        Task<List<FacturaDTO>> ObtenerFacturas(int idOrden);
        Task<XMLData> ExtraerDatosXML(IFormFile xml, int idTipoFolio);
        Task<DetalleOrdenCompra> ObtenerDetalleOrden(int idOrden);
        Task<bool> InsertarXML(string xmlString);
        Task<bool> FacturaExiste(string uuid);
        Task<bool> CambiarStatusOrdenCompleta(int idOrden);
        Task<List<CatalogoDTO>> GetStatusOrdenCompra();
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

        public async Task<ListadoOrdenCompraDTO> ObtenerOrdenesCompra(ListadoOrdenCompraDTO ordenescompra, int idProveedor, string fechaInicio, string fechaFin, int idStatus)
        {
            ordenescompra.Rows = await _FacturaRepo.ContarOrdenesCompra(idProveedor, fechaInicio, fechaFin, idStatus);
            if (ordenescompra.Rows > 0)
            {
                ordenescompra.NumPaginas = (ordenescompra.Rows / 40);

                if (ordenescompra.Rows % 40 > 0)
                {
                    ordenescompra.NumPaginas++;
                }
                ordenescompra.Ordenes = _mapper.Map<List<OrdenCompraDTO>>(await _FacturaRepo.ObtenerOrdenesCompra(idProveedor, fechaInicio, fechaFin, ordenescompra.Pagina,idStatus));
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
            _ = new List<FacturaDTO>();
            List<FacturaDTO> facturas = _mapper.Map<List<FacturaDTO>>(await _FacturaRepo.ObtenerFacturas(idOrden));
            return facturas;
        }

        public async Task<XMLData> ExtraerDatosXML(IFormFile xml, int idTipoFolio)
        {
            return await _FacturaRepo.ExtraerDatosXML(xml, idTipoFolio);
        }

        public async Task<DetalleOrdenCompra> ObtenerDetalleOrden(int idOrden)
        {
            return await _FacturaRepo.ObtenerDetalleOrden(idOrden);
        }

        public async Task<bool> InsertarXML(string xmlString)
        {
            return await _FacturaRepo.InsertarXML(xmlString);
        }

        public async Task<bool> FacturaExiste(string uuid)
        {
            return await _FacturaRepo.FacturaExiste(uuid);
        }

        public async Task<bool> CambiarStatusOrdenCompleta(int idOrden)
        {
            return await _FacturaRepo.CambiarStatusOrdenCompleta(idOrden);
        }

        public async Task<List<CatalogoDTO>> GetStatusOrdenCompra()
        {
            var statusc = new List<CatalogoDTO>();
            statusc = _mapper.Map<List<CatalogoDTO>>(await _FacturaRepo.GetStatusOrdenCompra());
            return statusc;
        }
    }
}
