using AutoMapper;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Services
{
    public interface IOrdenCompraService
    {
        Task<ResultadoDTO> GetOrdenCompra(int idOrdenCompra);
        Task<ResultadoDTO> GetOrdenCompraDetalle(int idOrdenCompra);
        Task<ResultadoDTO> GetSolicitudRecursoOrdenCompra(int idOrdenCompra);
    }
    public class OrdenCompraService : IOrdenCompraService
    {
        private readonly IOrdenCompraRepository _repo;
        private readonly IMapper _mapper;

        public OrdenCompraService(IOrdenCompraRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ResultadoDTO> GetOrdenCompra(int idOrdenCompra) {
            return _mapper.Map<ResultadoDTO>(await _repo.GetOrdenCompra(idOrdenCompra));
        }

        public async Task<ResultadoDTO> GetOrdenCompraDetalle(int idOrdenCompra)
        {
            return _mapper.Map<ResultadoDTO>(await _repo.GetOrdenCompraDetalle(idOrdenCompra));
        }

        public async Task<ResultadoDTO> GetSolicitudRecursoOrdenCompra(int idOrdenCompra)
        {
            return _mapper.Map<ResultadoDTO>(await _repo.GetSolicitudRecursoOrdenCompra(idOrdenCompra));
        }
    }
}