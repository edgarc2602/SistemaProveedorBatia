using Dapper;
using Microsoft.AspNetCore.Server.HttpSys;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Repositories
{
    public interface IRequisicionRepository
    {
        Task<ListaRequisiciones> GetRequisiciones(int idProveedor, int pagina);
        Task<RequisicionDetalle> GetRequisicionDetalle(int idRequisicion);
        Task<Resultado> PutActualizaRequisicionNuevoPrecio(RequisicionDetalle requisicion, float ivaNuevo, float subTotalNuevo, float totalNuevo);
        Task<int> NuevaRequisicion(string requisicion);
        Task<string> ObtenerCorreoComprador(int idProveedor);
        Task<string> ObtenerNombreProveedor(int idProveedor);
    }

    public class RequisicionRepository : IRequisicionRepository {
        private readonly DapperContext _ctx;

        public RequisicionRepository(DapperContext context) {
            _ctx = context;
        }

        public async Task<ListaRequisiciones> GetRequisiciones(int idProveedor, int pagina) {
            ListaRequisiciones requisiciones = new ListaRequisiciones();
            requisiciones.Requisiciones = new List<Requisicion>();

            int paginacion = 50;

            string query = @"
                    -- Consulta para obtener el total de paginas de requisiciones por proveedor
                    select
	                    case
		                    when residuo = 0 then total_paginas
		                    when residuo > 0 then total_paginas + 1
	                    end as NumPaginas,
	                    total_registros as Rows,
	                    1 as Pagina
                    from (
	                    select
		                     count(a.id_requisicion) % 50 as residuo,
		                     count(a.id_requisicion) / 50 as total_paginas,
		                     count(a.id_requisicion) as total_registros
	                    from tb_requisicion as a
	                    left join tb_empleado as b on a.id_comprador = b.id_empleado
	                    where a.id_status <> 3
                        -- Solo trae los de la linea de negocio de limpieza
                        and a.id_lineanegocio = 2
	                    and a.id_proveedor = @idProveedor
                    ) as resultado
                ";

            string query1 = @"
                    -- Consulta de las requisiciones  por proveedor
						select
							IdRequisicion,
							IdProveedor,
                            IdOrdenCompra,
							Comprador,
							Comentarios,
							FechaAlta,
                            IdEstatus,
							Estatus,
                            Iva,
                            IvaPorcentaje,
                            SubTotal,
							Total,
							falta,
                            IvaNuevo,
                            SubtotalNuevo,
                            TotalNuevo
						from (
							select
								row_number()over(order by a.falta desc) as numero_fila,
								a.id_requisicion as IdRequisicion,
								a.id_proveedor as IdProveedor,
                                isnull(c.id_orden, 0) IdOrdenCompra,
								isnull(trim(trim(b.paterno) + ' ' + trim(b.materno) + ' ' + trim(b.nombre)), 'Sin comprador') as Comprador,
								a.observacion as Comentarios,
								a.falta,
								convert(varchar(10), a.falta, 103) + ' ' + convert(varchar(8), a.falta, 24) as FechaAlta,
                                a.id_status as IdEstatus,
								case
									when a.id_status = 1 then 'Alta'
									when a.id_status = 2 then 'Autorizada'
									when a.id_status = 3 then 'Rechazada'
									when a.id_status = 4 then 'Completa'
								end as Estatus,
                                a.iva as Iva,
                                a.piva as IvaPorcentaje,
                                a.subtotal as SubTotal,
								a.total AS Total,
                                isnull(a.iva_nuevo, 0) as IvaNuevo,
                                isnull(a.subtotal_nuevo, 0) as SubTotalNuevo,
                                isnull(a.total_nuevo, 0) as TotalNuevo                            
							from tb_requisicion as a
							left join tb_empleado as b on a.id_comprador = b.id_empleado
                            left join tb_ordencompra as c on a.id_requisicion = c.id_requisicion
							where a.id_status <> 3
                            -- Solo trae los de la linea de negocio de limpieza
                            and a.id_lineanegocio = 2
							and a.id_proveedor = @idProveedor
						) as resultado
						where numero_fila between ((@pagina - 1) * @paginacion + 1) and (@pagina * @paginacion)";

            try {
                using(var connection = _ctx.CreateConnection()) {
                    requisiciones = (await connection.QueryFirstAsync<ListaRequisiciones>(query, new { idProveedor }));

                    requisiciones.Requisiciones = (await connection.QueryAsync<Requisicion>(query1, new { idProveedor, paginacion, pagina })).ToList();
                }
            } catch(Exception ex) {
                throw ex;
            }
            return requisiciones;
        }


        public async Task<RequisicionDetalle> GetRequisicionDetalle(int idRequisicion) {
            RequisicionDetalle requisicionDetalle = new RequisicionDetalle();
            requisicionDetalle.IdRequisicion = idRequisicion;
            requisicionDetalle.Productos = new List<RequisicionProducto>();

            string query = @"
                    -- Consulta para obtener los materiales de la requisicion
                    select
						a.id_requisicion as IdRequisicion,
						a.clave as Clave,
						b.descripcion as Producto,
						a.cantidad as Cantidad,
						a.precio as Precio,
						isnull(a.precio_nuevo, 0) as PrecioNuevo
					from tb_requisiciond as a
					inner join tb_producto as b on a.clave = b.clave
					where a.id_requisicion = @idRequisicion
					order by descripcion
                ";

            try {
                using(var connection = _ctx.CreateConnection()) {
                    requisicionDetalle.Productos = (await connection.QueryAsync<RequisicionProducto>(query, new { idRequisicion })).ToList();
                }
            } catch(Exception ex) {
                throw ex;
            }
            return requisicionDetalle;
        }

        public async Task<Resultado> PutActualizaRequisicionNuevoPrecio(RequisicionDetalle requisicion, float ivaNuevo, float subTotalNuevo, float totalNuevo) {
            Resultado resultado = new Resultado();

            try {
                using var connection = _ctx.CreateConnection();
                connection.Open();
                var parameters = new DynamicParameters();

                Resultado XmlResultado = requisicion.ConvertirModeloXML(requisicion);

                if(XmlResultado.Estatus.Value) {
                    parameters.Add("@IdRequisicion", DbType.Int32, direction: ParameterDirection.Output);
                    parameters.Add("@Cabecero", new SqlXml(new System.Xml.XmlTextReader(XmlResultado.Mensaje, System.Xml.XmlNodeType.Document, null)), DbType.Xml, ParameterDirection.Input);
                    parameters.Add("@ivaNuevo", ivaNuevo, DbType.Single, direction: ParameterDirection.Input);
                    parameters.Add("@subTotalNuevo", subTotalNuevo, DbType.Single, direction: ParameterDirection.Input);
                    parameters.Add("@totalNuevo", totalNuevo, DbType.Single, direction: ParameterDirection.Input);
                    await connection.ExecuteAsync("sp_requisicion_actualiza_precio_nuevo", parameters, commandType: CommandType.StoredProcedure);

                    int idMov = parameters.Get<int>("@IdRequisicion");
                    resultado.Estatus = true;
                    resultado.Mensaje = "Se han actualizado los precios de los productos correctamente";
                } else {
                    resultado.Estatus = false;
                    resultado.Mensaje = "No se pudo serializar la informacion de la requisicion";
                    resultado.MensajeError = XmlResultado.MensajeError;
                }
            } catch(Exception ex) {
                resultado.Estatus = false;
                resultado.MensajeError = "Error: " + ex.Message;
            }

            return resultado;
        }


        public async Task<Resultado> PostEnviarNotificacionComprador(Requisicion requisicion) {
            Resultado resultado = new Resultado();

            string correos = "edgarc@grupobatia.com.mx";
            string asunto = $"NOTIFICACION DE SINGA, Cambio de precios de la requisicion No: {requisicion.IdRequisicion}";
            string titulo = "Cambio de precios de requisicion";
            string mensaje = "";
            mensaje = $"Buen dia, el proveedor {{nombre del proveedor}}, a modificado los precios de la requisicion {{no.requisicion}}";

            try {
                using var connection = _ctx.CreateConnection();
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Cabecero", "", DbType.String, ParameterDirection.Input);
                parameters.Add("@correo_contactos", correos, DbType.String, ParameterDirection.Input);
                parameters.Add("@correo_asunto", "", DbType.String, ParameterDirection.Input);
                parameters.Add("@cabecero_titulo", "Mensaje generado por SINGA", DbType.String, ParameterDirection.Input);
                parameters.Add("@cuerpo_titulo", titulo, DbType.String, ParameterDirection.Input);
                parameters.Add("@cuerpo_parrafo", mensaje, DbType.String, ParameterDirection.Input);

                await connection.ExecuteAsync("sp_smtp_envia_correo", parameters, commandType: CommandType.StoredProcedure);

                int idMov = parameters.Get<int>("@IdRequisicion");
                resultado.Estatus = true;
                resultado.Mensaje = "Se han actualizado los precios de los productos correctamente";
            } catch(Exception ex) {
                resultado.Estatus = false;
                resultado.MensajeError = "Error: " + ex.Message;
            }

            return resultado;
        }

        public async Task<int> NuevaRequisicion(string requisicion) {
            try {
                int idRequisicion = 0;
                //bool resultado = false;
                try {
                    using var connection = _ctx.CreateConnection();
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@Material", new SqlXml(new System.Xml.XmlTextReader(requisicion, System.Xml.XmlNodeType.Document, null)), DbType.Xml, ParameterDirection.Input);
                    parameters.Add(
                        "@IdMov",   
                        dbType: DbType.Int32,
                        direction: ParameterDirection.Output
                    );
                    await connection.ExecuteAsync("sp_recepcione", parameters, commandType: CommandType.StoredProcedure);
                     idRequisicion = parameters.Get<int>("@IdMov");
                    //if(idRequisicion == null) {
                    //    resultado = true;
                    //}
                } catch(Exception ex) {
                    throw ex;
                }
                return idRequisicion;
            }
            catch(Exception ex) {
                return 0;
                throw ex;
            }
            
        }
        public async Task<string> ObtenerCorreoComprador(int idProveedor) {
            try {
                using var connection = _ctx.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@idProveedor", idProveedor, DbType.Int32);

                string correoComprador = await connection
                    .QueryFirstOrDefaultAsync<string>(
                        "sp_obtener_comprador_proveedor",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                if (correoComprador == null) {
                    return "";
                }

                return correoComprador;
            } catch(Exception) {
                throw;
            }
        }
        public async Task<string> ObtenerNombreProveedor(int idProveedor) {
            try {

                string query = @"SELECT ISNULL(nombre,'') AS Nombre FROM tb_proveedor WHERE id_proveedor  = @idProveedor";
                string nombreProveedor = "";
                using(var connection = _ctx.CreateConnection()) {
                    nombreProveedor = await connection.QueryFirstOrDefaultAsync<string>(query, new { idProveedor });
                }
                return nombreProveedor;
            } catch(Exception ex) {
                throw ex;
            }
        }

    }
}