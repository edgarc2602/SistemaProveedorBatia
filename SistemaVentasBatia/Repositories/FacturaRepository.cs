using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace SistemaVentasBatia.Repositories
{
    public interface IFacturaRepository
    {
        Task<List<OrdenCompra>> ObtenerOrdenesCompra(int idProveedor, string fechaInicio, string fechaFin, int pagina);
        Task<int> ContarOrdenesCompra(int idProveedor, string fechaInicio, string fechaFin);
    }

    public class FacturaRepository : IFacturaRepository
    {
        private readonly DapperContext _ctx;

        public FacturaRepository(DapperContext context)
        {
            _ctx = context;
        }

        public async Task<List<OrdenCompra>> ObtenerOrdenesCompra(int idProveedor, string fechaInicio, string fechaFin, int pagina)
        {
            var query = @"
SELECT * FROM (
SELECT 
ROW_NUMBER() over (order by a.falta DESC, b.nombre, c.nombre ) as rownum, 
id_orden IdOrden,
case when a.tipo = 1 then 'Materiales' else 'Servicios' end as Tipo,
e.descripcion as Estatus, 
convert(varchar(12), falta,103) as FechaAlta , 
b.nombre as Empresa	, 
isnull(c.nombre,'') as Proveedor, 
isnull(d.nombre,'') as Cliente,
f.Per_Nombre + ' ' + f.Per_Paterno as Elabora, 
a.Total Total, 
a.observacion Observacion, 
a.inventario Inventario
from tb_ordencompra a inner join tb_empresa b on a.id_empresa = b.id_empresa
left outer join tb_proveedor c on a.id_proveedor = c.id_proveedor
left outer join tb_cliente d on a.id_cliente = d.id_cliente
inner join tb_statusc e on a.id_status = e.id_status inner join personal f on a.ualta = f.IdPersonal
where a.id_proveedor = @idProveedor
AND (@fechaInicio IS NULL OR @fechaFin IS NULL OR falta BETWEEN @fechaInicio AND @fechaFin)
) as Ordenes
WHERE   
  RowNum >= ((@pagina - 1) * 40) + 1
  AND RowNum <= (@pagina * 40);
";
            var ordenes = new List<OrdenCompra>();
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    ordenes = (await connection.QueryAsync<OrdenCompra>(query, new { idProveedor, fechaInicio, fechaFin, pagina })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ordenes;
        }
        
        public async Task <int> ContarOrdenesCompra(int idProveedor, string fechaInicio, string fechaFin)
        {
            var query = @"
SELECT COUNT(*) AS TotalRows
FROM (
Select  
ROW_NUMBER() over (order by a.falta, b.nombre, c.nombre ) as rownum, 
id_orden IdOrden,
case when a.tipo = 1 then 'Materiales' else 'Servicios' end as Tipo,
e.descripcion as Estatus, 
convert(varchar(12), falta,103) as FechaAlta , 
b.nombre as Empresa	, 
isnull(c.nombre,'') as Proveedor, 
isnull(d.nombre,'') as Cliente,
f.Per_Nombre + ' ' + f.Per_Paterno as Elabora, 
a.Total Total, 
a.observacion Observacion, 
a.inventario Inventario
from tb_ordencompra a inner join tb_empresa b on a.id_empresa = b.id_empresa
left outer join tb_proveedor c on a.id_proveedor = c.id_proveedor
left outer join tb_cliente d on a.id_cliente = d.id_cliente
inner join tb_statusc e on a.id_status = e.id_status inner join personal f on a.ualta = f.IdPersonal
where a.id_proveedor = @idProveedor
AND (@fechaInicio IS NULL OR @fechaFin IS NULL OR falta BETWEEN @fechaInicio AND @fechaFin)
)
 AS TotalRowCount;
";
            var rows = 0;
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    rows = await connection.QuerySingleAsync<int>(query, new { idProveedor, fechaInicio, fechaFin});
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rows;
        }
    }
}
