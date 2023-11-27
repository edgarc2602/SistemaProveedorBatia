using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaVentasBatia.Enums;

namespace SistemaVentasBatia.Repositories
{
    public interface IProductoRepository
    {
        //Task AgregarMaterialPuesto(MaterialPuesto mat);
        
    }

    public class ProductoRepository : IProductoRepository
    {
        private readonly DapperContext ctx;

        public ProductoRepository(DapperContext context)
        {
            ctx = context;
        }

        //public async Task<bool> ActualizarEquipoPuesto(MaterialPuesto equi)
        //{
        //    bool reg = false;
        //    var query = @"update tb_cotiza_equipo set clave = @ClaveProducto,
					   //     id_puesto = @IdPuesto, id_frecuencia = @IdFrecuencia, cantidad = @Cantidad
					   // where id_equipo_puesto = @IdMaterialPuesto;";
        //    try
        //    {
        //        using (var connection = ctx.CreateConnection())
        //        {
        //            await connection.ExecuteScalarAsync<int>(query, equi);
        //            reg = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //    return reg;
        //}
    }
}