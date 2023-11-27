using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace SistemaVentasBatia.Repositories
{
    public interface ICuentaRepository
    {
        //Task<decimal> ObtenerPrecioProductoPorClave(string clave);
    }

    public class CuentaRepository : ICuentaRepository
    {
        private readonly DapperContext _ctx;

        public CuentaRepository(DapperContext context)
        {
            _ctx = context;
        }

        //public async Task InsertarMaterialesCotizacion(List<MaterialCotizacion> materialCotizacion)
        //{
        //    var query = "insert into tb_cotiza_material (clave_producto, id_cotizacion, id_direccion_cotizacion, id_puesto_direccioncotizacion, precio_unitario, cantidad, total, importemensual, fecha_alta, id_personal, id_frecuencia) values";
        //    var row = "('{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}', {9}, {10}),";

        //    foreach (var material in materialCotizacion)
        //    {
        //        query += string.Format(row, material.ClaveProducto, material.IdCotizacion, material.IdDireccionCotizacion, material.IdPuestoDireccionCotizacion,
        //            material.PrecioUnitario, material.Cantidad, material.Total, material.ImporteMensual, DateTime.Now.ToString("yyyy/MM/dd"), material.IdPersonal, (int)material.IdFrecuencia);
        //    }

        //    query = query.Remove(query.Length - 1);

        //    try
        //    {
        //        using (var connection = _ctx.CreateConnection())
        //        {
        //            await connection.ExecuteAsync(query);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
