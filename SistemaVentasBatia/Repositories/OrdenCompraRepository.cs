using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.HttpSys;
using System.Collections;

namespace SistemaVentasBatia.Repositories
{
    public interface IOrdenCompraRepository
    {
        Task<Resultado> GetOrdenCompra(int idOrdenCompra);
        Task<Resultado> GetOrdenCompraDetalle(int idOrdenCompra);
        Task<Resultado> GetSolicitudRecursoOrdenCompra(int idOrdenCompra);
    }

    public class OrdenCompraRepository : IOrdenCompraRepository
    {
        private readonly DapperContext _ctx;

        public OrdenCompraRepository(DapperContext context)
        {
            _ctx = context;
        }


        public async Task<Resultado> GetOrdenCompra(int idOrdenCompra)
        {
            Resultado resultado = new Resultado();
            OrdenCompra ordenCompra = new OrdenCompra();
            var query = @"
            SELECT 
	            a.id_orden as IdOrden,
	            case 
		            when a.tipo = 1 then 'Materiales' 
		            else 'Servicios' 
	            end as Tipo,
	            e.descripcion as Estatus, 
	            convert(varchar(12), a.falta,103) as FechaAlta,
	            b.nombre as Empresa,
	            isnull(c.nombre,'') as Proveedor,
                a.id_cliente as IdCliente,
	            isnull(d.nombre,'') as Cliente,
	            f.Per_Nombre + ' ' + f.Per_Paterno as Elabora, 
	            a.observacion Observacion, 
	            a.inventario Inventario,
                a.id_almacen as IdAlmacen,
                g.id_credito as IdCredito,
                g.descripcion as Credito,
                g.dias as DiasCredito,
	            0 as Facturado,
	            a.iva as Iva,
	            a.subtotal as SubTotal,
	            a.Total Total
            from tb_ordencompra as a 
            inner join tb_empresa as b on a.id_empresa = b.id_empresa
            left outer join tb_proveedor as c on a.id_proveedor = c.id_proveedor
            left outer join tb_cliente as d on a.id_cliente = d.id_cliente
            left outer join tb_statusc as e on a.id_status = e.id_status 
            inner join personal as f on a.ualta = f.IdPersonal
            inner join tb_credito as g on c.credito = g.id_credito
            WHERE a.id_orden = @IdOrdenCompra
            ";
            try
            {
                using var connection = _ctx.CreateConnection();
                ordenCompra = await connection.QuerySingleAsync<OrdenCompra>(query, new { idOrdenCompra });
                resultado.Estatus = true;
                resultado.Objeto = ordenCompra;
            }
            catch (Exception ex)
            {
                resultado.Estatus = false;
                resultado.Mensaje = "Error: " + ex.Message;
                resultado.MensajeError = ex.ToString();
            }
            return resultado;
        }


        public async Task<Resultado> GetOrdenCompraDetalle(int idOrdenCompra)
        {
            Resultado resultado = new Resultado();

            OrdenCompraDetalle ordenCompraDetalle = new OrdenCompraDetalle();
            ordenCompraDetalle.IdOrdenCompra = idOrdenCompra;
            ordenCompraDetalle.Productos = new List<OrdenCompraProducto>();

            string query = @"
                    -- Consulta para obtener los materiales de la orden de compra
                    select
	                    a.id_orden as IdOrdenCompra,
	                    a.clave as Clave,
	                    b.descripcion as Producto,
	                    c.descripcion as UnidadMedida,
	                    a.cantidad as Cantidad,
	                    a.precio as Precio
                    from tb_ordencomprad as a
                    inner join tb_producto as b on a.clave = b.clave
                    inner join tb_unidadmedida as c on b.id_unidad = c.id_unidad
                    where a.id_orden = @idOrdenCompra
                    order by b.descripcion
                    ";
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    ordenCompraDetalle.Productos = (await connection.QueryAsync<OrdenCompraProducto>(query, new { idOrdenCompra })).ToList();
                    resultado.Estatus = true;
                    resultado.Objeto = ordenCompraDetalle;
                }
            }
            catch (Exception ex)
            {
                resultado.Estatus = false;
                resultado.Mensaje = ex.Message;
                resultado.MensajeError = ex.ToString();
            }
            return resultado;
        }


        public async Task<Resultado> GetSolicitudRecursoOrdenCompra(int idOrdenCompra)
        {
            Resultado resultado = new Resultado();
            SolicitudRecursoOrdenCompra solicitudRecurso = new SolicitudRecursoOrdenCompra();
            string query = @"select 
	                            a.id_solicitud as IdSolicitudRecurso,
	                            a.id_status as IdEstatus,
	                            c.descripcion as Estatus
                            from tb_solicitudrecurso as a
                            inner join tb_solicitudrecurso_OC as b on a.id_solicitud = b.id_solicitud
                            inner join tb_statussr as c on a.id_status = c.id_status
                            where a.id_status not in (6, 9)
                            and b.id_orden = @idOrdenCompra";
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    solicitudRecurso = await connection.QueryFirstOrDefaultAsync<SolicitudRecursoOrdenCompra>(query, new { idOrdenCompra });
                    resultado.Estatus = true;
                    resultado.Objeto = solicitudRecurso;
                }
            }
            catch (Exception ex)
            {
                resultado.Estatus = false;
                resultado.Mensaje = ex.Message;
                resultado.MensajeError = ex.ToString();
            }

            return resultado;
        }



        public async Task<Resultado> PostSubirFactura(string xml1, string xml2)
        {
            Resultado resultado = new Resultado();
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    //ordenCompraDetalle.Productos = (await connection.QueryAsync<OrdenCompraProducto>(query, new { idOrdenCompra })).ToList();
                    //resultado.Estatus = true;
                    //resultado.Objeto = ordenCompraDetalle;
                }
            }
            catch (Exception ex)
            {
                resultado.Estatus = false;
                resultado.Mensaje = ex.Message;
                resultado.MensajeError = ex.ToString();
            }
            return resultado;
        }


    }
}