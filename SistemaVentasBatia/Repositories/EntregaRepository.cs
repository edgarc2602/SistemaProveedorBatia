﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using SistemaProveedoresBatia.DTOs;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Enums;
using SistemaVentasBatia.Models;
using SistemaProveedoresBatia;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Diagnostics;

namespace SistemaVentasBatia.Repositories
{
    public interface IEntregaRepository
    {
        Task<int> ContarListados(int mes, int anio, int idProveedor, int idEstado, int tipo);
        Task<List<Listados>> ObtenerListados(int mes, int anio, int idProveedor, int idEstado, int tipo, int pagina);
    }

    public class EntregaRepository : IEntregaRepository
    {
        private readonly DapperContext _ctx;

        public EntregaRepository(DapperContext context)
        {
            _ctx = context;
        }
        public async Task<List<Listados>> ObtenerListados(int mes, int anio, int idProveedor, int idEstado, int tipo, int pagina)
        {
            string query = @"
SELECT * FROM (
SELECT 
ROW_NUMBER() OVER ( ORDER BY a.nombre desc ) AS RowNum,
a.id_inmueble AS IdInmueble, 
a.nombre AS NombreSucursal, 
isnull(b.id_listado,0) AS IdListado, 
isnull(d.descripcion,'') AS Tipo, 
case 
when b.id_status = 1 Then 'Alta' 
when b.id_status = 2 then 'Aprobado'
when b.id_status = 3 then 'Despachado' 
when b.id_status = 4 then 'Entregado' 
when b.id_status = 5 then 'Cancelado' 
else 'No existe' end AS Estatus,
convert(varchar(12), b.falta, 103) AS FechaAlta,
cast(isnull(SUM(c.cantidad * c.precio),0) as numeric(12,2)) AS Utilizado, 
convert(varchar(12), fentrega, 103) AS FechaEntrega
from tb_cliente_inmueble a 
left outer join tb_listadomaterial b on a.id_inmueble = b.id_inmueble
left outer join tb_listadomateriald c on b.id_listado = c.id_listado 
left outer join tb_tipolistado d on b.tipo = d.id_tipo
inner join tb_proveedorinmueble e ON e.id_inmueble = a.id_inmueble
WHERE e.id_proveedor = @idProveedor and a.id_status = 1 and a.materiales = 0
-- and
--ISNULL(NULLIF(0,0), a.id_cliente) = a.id_cliente
AND
ISNULL(NULLIF(@idEstado,0), a.id_estado) = a.id_estado AND
ISNULL(NULLIF(@tipo,0), d.id_tipo) = d.id_tipo AND
ISNULL(NULLIF(@mes,0), b.mes) = b.mes AND
ISNULL(NULLIF(@anio,0), b.anio) = b.anio
group by 
a.id_inmueble, 
a.nombre, b.falta, 
b.id_listado, 
b.id_status, 
d.descripcion, 
fentrega
) as Listado
WHERE   RowNum >= ((@pagina - 1) * 10) + 1
AND RowNum <= (@pagina * 10)
ORDER BY RowNum
";
            var listado = new List<Listados>();

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    listado = (await connection.QueryAsync<Listados>(query, new { mes, anio, idProveedor, idEstado, tipo, pagina })).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listado;
        }

        public async Task<int> ContarListados(int mes, int anio, int idProveedor, int idEstado, int tipo)
        {
            var query = @"
SELECT COUNT(*) AS TotalRows
FROM (
    SELECT 
ROW_NUMBER() OVER ( ORDER BY a.nombre desc ) AS RowNum,
a.id_inmueble AS IdInmueble, 
a.nombre AS NombreSucursal, 
isnull(b.id_listado,0) AS IdListado, 
isnull(d.descripcion,'') AS Tipo, 
case 
when b.id_status = 1 Then 'Alta' 
when b.id_status = 2 then 'Aprobado'
when b.id_status = 3 then 'Despachado' 
when b.id_status = 4 then 'Entregado' 
when b.id_status = 5 then 'Cancelado' 
else 'No existe' end AS Estatus,
convert(varchar(12), b.falta, 103) AS FechaAlta,
cast(isnull(SUM(c.cantidad * c.precio),0) as numeric(12,2)) AS Utilizado, 
convert(varchar(12), fentrega, 103) AS FechaEntrega
from tb_cliente_inmueble a 
left outer join tb_listadomaterial b on a.id_inmueble = b.id_inmueble
left outer join tb_listadomateriald c on b.id_listado = c.id_listado 
left outer join tb_tipolistado d on b.tipo = d.id_tipo
inner join tb_proveedorinmueble e ON e.id_inmueble = a.id_inmueble
WHERE e.id_proveedor = @idProveedor and a.id_status = 1 and a.materiales = 0
-- and
--ISNULL(NULLIF(0,0), a.id_cliente) = a.id_cliente
AND
ISNULL(NULLIF(@idEstado,0), a.id_estado) = a.id_estado AND
ISNULL(NULLIF(@tipo,0), d.id_tipo) = d.id_tipo AND
ISNULL(NULLIF(@mes,0), b.mes) = b.mes AND
ISNULL(NULLIF(@anio,0), b.anio) = b.anio
group by 
a.id_inmueble, 
a.nombre, b.falta, 
b.id_listado, 
b.id_status, 
d.descripcion, 
fentrega
) AS TotalRowCount;
";
            var rows = 0;
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    rows = await connection.QuerySingleAsync<int>(query, new { mes, anio, idProveedor, idEstado, tipo });
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
