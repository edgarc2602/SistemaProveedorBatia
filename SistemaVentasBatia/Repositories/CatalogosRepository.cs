using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;
using Microsoft.AspNetCore.Server.IIS.Core;

namespace SistemaVentasBatia.Repositories
{
    public interface ICatalogosRepository
    {
        Task<List<Catalogo>> ObtenerEstados();
        Task<List<Catalogo>> ObtenerServicios();
        Task<List<Catalogo>> ObtenerMunicipios(int idMunicipio);
        Task<List<Catalogo>> ObtenerTiposInmueble();
        Task<List<Catalogo>> ObtenerCatalogoPuestos();
        Task<List<Catalogo>> ObtenerCatalogoServicios();
        Task<List<Catalogo>> ObtenerCatalogoTurnos();
        Task<List<Catalogo>> ObtenerCatalogoJornada();
        Task<List<Catalogo>> ObtenerCatalogoClase();
        Task<List<Catalogo>> ObtenerCatalogoDireccionesCotizacion(int idCotizacion);
        Task<List<Catalogo>> ObtenerCatalogoPuestosCotizacion(int idCotizacion);
        Task<List<Catalogo>> ObtenerCatalogoProductos(Servicio idServicio);
        Task<IEnumerable<Catalogo>> ObtenerCatalogoProductosByFamilia(Servicio idServicio, int[] familia);
        Task<List<Catalogo>> ObtenerMeses();
    }

    public class CatalogosRepository : ICatalogosRepository
    {
        private readonly DapperContext ctx;

        public CatalogosRepository(DapperContext ctx)
        {
            this.ctx = ctx;
        }

        public async Task<List<Catalogo>> ObtenerEstados()
        {
            var query = @"SELECT id_Estado Id, Estado Descripcion from tb_estados";

            var estados = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    estados = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return estados;
        }

        public async Task<List<Catalogo>> ObtenerServicios()
        {
            var query = @"SELECT id_servicioextra Id, descripcion Descripcion  FROM tb_servicioextra";


            var servicios = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    servicios = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return servicios;
        }



        public async Task<List<Catalogo>> ObtenerMunicipios(int idEstado)
        {
            var query = @"
SELECT es.id_municipio Id,
m.Municipio Descripcion FROM tb_estados_municipios es
INNER JOIN dbo.Municipios m ON m.Id_Municipio = es.id_municipio
WHERE es.id_estado = @idEstado ORDER BY m.Municipio";

            var municipios = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    municipios = (await connection.QueryAsync<Catalogo>(query, new { idEstado })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return municipios;
        }

        public async Task<List<Catalogo>> ObtenerTiposInmueble()
        {
            var query = @"SELECT id_tipoinmueble Id, descripcion Descripcion from tb_tipoinmueble where id_status = @idEstatus";

            var tiposInmueble = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    tiposInmueble = (await connection.QueryAsync<Catalogo>(query, new { idEstatus = EstatusTipoInmueble.Activo })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return tiposInmueble;
        }

        public async Task<List<Catalogo>> ObtenerCatalogoPuestos()
        {
            var query = @"SELECT id_puesto Id, descripcion Descripcion
                          FROM tb_puesto d
                          WHERE id_status = 1 AND cotizador = 1 ORDER BY Descripcion";

            var puestos = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    puestos = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return puestos;
        }

        public async Task<List<Catalogo>> ObtenerCatalogoServicios()
        {
            var query = @"SELECT IdTpoServicio Id, TS_Descripcion Descripcion FROM Tbl_TipoServicio";

            var servicios = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    servicios = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return servicios;
        }

        public async Task<List<Catalogo>> ObtenerCatalogoTurnos()
        {
            var query = @"SELECT id_turno Id, descripcion Descripcion
                          FROM tb_turno 
                          WHERE id_status = 1 AND cotizador = 1 ORDER BY Descripcion";

            var direcciones = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direcciones = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return direcciones;
        }

        public async Task<List<Catalogo>> ObtenerCatalogoJornada()
        {
            var query = @"SELECT 
id_jornada Id,
descripcion Descripcion
FROM tb_jornada";

            var direcciones = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direcciones = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return direcciones;
        }

        public async Task<List<Catalogo>> ObtenerCatalogoClase()
        {
            var query = @"SELECT 
id_clase Id,
descripcion Descripcion
FROM tb_clase";

            var direcciones = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direcciones = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return direcciones;
        }


        public async Task<List<Catalogo>> ObtenerCatalogoDireccionesCotizacion(int idCotizacion)
        {
            var query = @"SELECT dc.id_direccion_cotizacion Id, d.nombre_sucursal Descripcion
                          FROM tb_direccion_cotizacion dc
                          JOIN tb_direccion d on d.id_direccion = dc.id_direccion
                          WHERE dc.id_cotizacion = @idCotizacion";

            var direccionesCotizacion = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    direccionesCotizacion = (await connection.QueryAsync<Catalogo>(query, new { idCotizacion })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return direccionesCotizacion;
        }

        public async Task<List<Catalogo>> ObtenerCatalogoPuestosCotizacion(int idCotizacion)
        {
            var query = @"SELECT distinct p.id_puesto Id, p.descripcion Descripcion
                          FROM tb_puesto_direccion_cotizacion pdc
                          JOIN tb_direccion_cotizacion dc ON dc.id_direccion_cotizacion = pdc.id_direccion_cotizacion
                          JOIN tb_puesto p on p.id_puesto = pdc.id_puesto
                          WHERE dc.id_cotizacion = @idCotizacion ORDER BY Descripcion";

            var puestosCotizacion = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    puestosCotizacion = (await connection.QueryAsync<Catalogo>(query, new { idCotizacion })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return puestosCotizacion;
        }

        public async Task<List<Catalogo>> ObtenerCatalogoProductos(Servicio idServicio)
        {
            var query = @"SELECT clave Clave, descripcion Descripcion
                          FROM tb_producto                          
                          WHERE id_servicio = @idServicio and id_status = 1 ORDER BY Descripcion;";

            var puestosCotizacion = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    puestosCotizacion = (await connection.QueryAsync<Catalogo>(query, new { idServicio })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return puestosCotizacion;
        }

        public async Task<IEnumerable<Catalogo>> ObtenerCatalogoProductosByFamilia(Servicio idServicio, int[] familia)
        {
            var query = @"SELECT clave Clave, descripcion Descripcion
                          FROM tb_producto                          
                          WHERE id_servicio = @idServicio and id_familia in @familias and id_status = 1 ORDER BY Descripcion;";
            var listFamilia = familia.Select(x => x.ToString());

            var puestosCotizacion = new List<Catalogo>();

            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    puestosCotizacion = (await connection.QueryAsync<Catalogo>(query, new { idServicio, familias = listFamilia })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return puestosCotizacion;
        }

        public async Task<List<Catalogo>> ObtenerMeses()
        {
            var query = @"
SELECT 
id_mes Id,
descripcion Descripcion
From tb_mes
";
            var meses = new List<Catalogo>();
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    meses = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return meses;
        }

        public async Task<List<Catalogo>> ObtenerTipoListado()
        {
            var query = @"
SELECT 
id_tipo Id,
descripcion Descripcion
FROM tb_tipolistado 
";
            var tipoListados = new List<Catalogo>();
            try
            {
                using (var connection = ctx.CreateConnection())
                {
                    tipoListados = (await connection.QueryAsync<Catalogo>(query)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tipoListados;
        }
    }
}
