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
        Task<List<Catalogo>> ObtenerMeses();
    }

    public class CatalogosRepository : ICatalogosRepository
    {
        private readonly DapperContext ctx;

        public CatalogosRepository(DapperContext ctx)
        {
            this.ctx = ctx;
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
                using var connection = ctx.CreateConnection();
                meses = (await connection.QueryAsync<Catalogo>(query)).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return meses;
        }
    }
}
