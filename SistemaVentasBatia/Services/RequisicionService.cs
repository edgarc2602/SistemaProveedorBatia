using AutoMapper;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Services
{
    public interface IRequisicionService
    {
        Task<ListaRequisicionesDTO> GetRequisiciones(int idProveedor, int pagina);
        Task<RequisicionDetalleDTO> GetRequisicionDetalle(int idRequisicion);
        Task<ResultadoDTO> PutActualizaRequisicionNuevoPrecio(RequisicionDetalleDTO requisicion, float ivaNuevo, float subTotalNuevo, float totalNuevo);
    }
    public class RequisicionService : IRequisicionService
    {
        private readonly IRequisicionRepository _repo;
        private readonly IMapper _mapper;

        public RequisicionService(IRequisicionRepository repoRequisicion, IMapper mapper)
        {
            _repo = repoRequisicion;
            _mapper = mapper;
        }

        public async Task<ListaRequisicionesDTO> GetRequisiciones(int idProveedor, int pagina)
        {
            return _mapper.Map<ListaRequisicionesDTO>(await _repo.GetRequisiciones(idProveedor, pagina));
        }

        public async Task<RequisicionDetalleDTO> GetRequisicionDetalle(int idRequisicion)
        {
            return _mapper.Map<RequisicionDetalleDTO>(await _repo.GetRequisicionDetalle(idRequisicion));
        }

        public async Task<ResultadoDTO> PutActualizaRequisicionNuevoPrecio(RequisicionDetalleDTO requisicion, float ivaNuevo, float subTotalNuevo, float totalNuevo) {
            return _mapper.Map<ResultadoDTO>(await _repo.PutActualizaRequisicionNuevoPrecio(_mapper.Map<RequisicionDetalle>(requisicion), ivaNuevo, subTotalNuevo, totalNuevo));
        }
    }
}