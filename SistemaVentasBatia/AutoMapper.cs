using AutoMapper;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Enums;
using SistemaProveedoresBatia.DTOs;

namespace SistemaVentasBatia
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Catalogo, CatalogoDTO>();
            CreateMap<CatalogoDTO, Catalogo>();

            CreateMap<DiaSemana, string>().ConstructUsing(o => o.ToString());
            CreateMap<Frecuencia, string>().ConstructUsing(o => o.ToString());

            CreateMap<AccesoDTO, Acceso>();
            CreateMap<Acceso, AccesoDTO>();
            CreateMap<UsuarioDTO, Usuario>();
            CreateMap<Usuario, UsuarioDTO>();
            CreateMap<Listados, ListadosDTO>();
            CreateMap<ListadosDTO, Listados>();
            CreateMap<OrdenCompraDTO, OrdenCompra>();
            CreateMap<OrdenCompra, OrdenCompraDTO>();
            CreateMap<DetalleMaterial, DetalleMaterialDTO>();
            CreateMap<DetalleMaterialDTO, DetalleMaterial>();
            CreateMap<AcuseEntrega, AcuseEntregaDTO>();
            CreateMap<AcuseEntregaDTO, AcuseEntrega>();
            CreateMap<Factura, FacturaDTO>();
            CreateMap<FacturaDTO, Factura>();
            CreateMap<EstadoDeCuenta, EstadoDeCuentaDTO>();
            CreateMap<EstadoDeCuentaDTO, EstadoDeCuenta>();
            CreateMap<ListaEvaluacionProveedor, ListaEvaluacionProveedorDTO>();
            CreateMap<ListaEvaluacionProveedorDTO, ListaEvaluacionProveedor>();
            CreateMap<EvaluacionProveedor, EvaluacionProveedorDTO>();
            CreateMap<EvaluacionProveedorDTO, EvaluacionProveedor>();
            CreateMap<DashOrdenMes, DashOrdenMesDTO>();
            CreateMap<DashOrdenMesDTO, DashOrdenMes>();

            CreateMap<ListaRequisiciones, ListaRequisicionesDTO>();
            CreateMap<ListaRequisicionesDTO, ListaRequisiciones>();
            CreateMap<Requisicion, RequisicionDTO>();
            CreateMap<RequisicionDTO, Requisicion>();
            CreateMap<RequisicionDetalle, RequisicionDetalleDTO>();
            CreateMap<RequisicionDetalleDTO, RequisicionDetalle>();
            CreateMap<RequisicionProducto, RequisicionProductoDTO>();
            CreateMap<RequisicionProductoDTO, RequisicionProducto>();
            CreateMap<Resultado, ResultadoDTO>();
            CreateMap<ResultadoDTO, Resultado>();
            CreateMap<Correos, CorreosDTO>();
            CreateMap<CorreosDTO, Correos>();
            CreateMap<OrdenCompra, OrdenCompraDTO>();
            CreateMap<OrdenCompraDTO, OrdenCompra>();
            CreateMap<OrdenCompraDetalle, OrdenCompraDetalleDTO>();
            CreateMap<OrdenCompraDetalleDTO, OrdenCompraDetalle>();
            CreateMap<OrdenCompraProducto, OrdenCompraProductoDTO>();
            CreateMap<OrdenCompraProductoDTO, OrdenCompraProducto>();
            CreateMap<SolicitudRecursoOrdenCompra,SolicitudRecursoOrdenCompraDTO>();
            CreateMap<SolicitudRecursoOrdenCompraDTO, SolicitudRecursoOrdenCompra>();
        }
    }
}