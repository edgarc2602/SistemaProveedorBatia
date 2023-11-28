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
        }
    }
}