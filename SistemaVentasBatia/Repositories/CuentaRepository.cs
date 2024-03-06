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
        Task<int> ContarEstadoDeCuenta(int idProveedor);
        Task<List<EstadoDeCuenta>> GetEstadoDeCuenta(int idProveedor, int pagina);
        Task<List<ListaEvaluacionProveedor>> GetListaEvaluaciones(int idProveedor);
        Task<List<EvaluacionProveedor>> GetEvaluacionProveedor(int idEvaluacionProveedor);
    }

    public class CuentaRepository : ICuentaRepository
    {
        private readonly DapperContext _ctx;

        public CuentaRepository(DapperContext context)
        {
            _ctx = context;
        }

        public async Task<List<EstadoDeCuenta>> GetEstadoDeCuenta(int idProveedor, int pagina)
        {
            string query = @"
SELECT * FROM (
SELECT
ROW_NUMBER() over (order by a.fregistro, a.factura) as rownum, 
    b.razonsocial AS Nombre,
    a.factura AS Factura,
    a.total AS Total,
    a.pago AS Pago,
    (a.total - a.pago) AS Saldo,
    a.ffactura AS Ffactura,
    a.fvence AS Fvencimiento,
    c.dias AS DiasCredito,
    CASE
        WHEN DATEDIFF(day, a.fvence, GETDATE()) > 0 THEN DATEDIFF(day, a.fvence, GETDATE())
        ELSE 0
    END AS DiasVencido,
    CASE
        WHEN a.fvence >= GETDATE() THEN (a.total - a.pago)
        ELSE 0
    END AS Corriente,
    CASE
        WHEN FLOOR(DATEDIFF(day, a.fvence, GETDATE())) BETWEEN 1 AND 30 THEN (a.total - a.pago)
        ELSE 0
    END AS Mes1,
    CASE
        WHEN FLOOR(DATEDIFF(day, a.fvence, GETDATE())) BETWEEN 31 AND 60 THEN (a.total - a.pago)
        ELSE 0
    END AS Mes2,
    CASE
        WHEN FLOOR(DATEDIFF(day, a.fvence, GETDATE())) BETWEEN 61 AND 90 THEN (a.total - a.pago)
        ELSE 0
    END AS Mes3,
    CASE
        WHEN FLOOR(DATEDIFF(day, a.fvence, GETDATE())) >= 91 THEN (a.total - a.pago)
        ELSE 0
    END AS Mes4
	
FROM
    tb_provision a
INNER JOIN
    tb_proveedor b ON b.id_proveedor = a.id_proveedor
INNER JOIN
    tb_credito c ON c.id_credito = b.credito
WHERE
    a.id_proveedor = @idProveedor and  (a.total - a.pago) != 0
)
AS EstadosDeCuenta
WHERE   
  RowNum >= ((@pagina - 1) * 40) + 1
  AND RowNum <= (@pagina * 40);
";
            var estadodecuenta = new List<EstadoDeCuenta>();
            try
            {
                using var connection = _ctx.CreateConnection();
                estadodecuenta = (await connection.QueryAsync<EstadoDeCuenta>(query, new { idProveedor, pagina })).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return estadodecuenta;
        }

        public async Task<int> ContarEstadoDeCuenta(int idProveedor)
        {
            string query = @"
SELECT COUNT(*) AS TotalRows
FROM (
SELECT
    b.razonsocial AS Nombre,
    a.factura AS Factura,
    a.total AS Total,
    a.pago AS Pago,
    (a.total - a.pago) AS Saldo,
    a.ffactura AS Ffactura,
    a.fvence AS Fvencimiento,
    c.dias AS DiasCredito,
    CASE
        WHEN DATEDIFF(day, a.fvence, GETDATE()) > 0 THEN DATEDIFF(day, a.fvence, GETDATE())
        ELSE 0
    END AS DiasVencido,
    CASE
        WHEN FLOOR(DATEDIFF(day, a.fvence, GETDATE())) = 0 THEN (a.total - a.pago)
        ELSE 0
    END AS Corriente,
    CASE
        WHEN FLOOR(DATEDIFF(day, a.fvence, GETDATE())) BETWEEN 1 AND 30 THEN (a.total - a.pago)
        ELSE 0
    END AS Mes1,
    CASE
        WHEN FLOOR(DATEDIFF(day, a.fvence, GETDATE())) BETWEEN 31 AND 60 THEN (a.total - a.pago)
        ELSE 0
    END AS Mes2,
    CASE
        WHEN FLOOR(DATEDIFF(day, a.fvence, GETDATE())) BETWEEN 61 AND 90 THEN (a.total - a.pago)
        ELSE 0
    END AS Mes3,
    CASE
        WHEN FLOOR(DATEDIFF(day, a.fvence, GETDATE())) >= 91 THEN (a.total - a.pago)
        ELSE 0
    END AS Mes4
FROM
    tb_provision a
INNER JOIN
    tb_proveedor b ON b.id_proveedor = a.id_proveedor
INNER JOIN
    tb_credito c ON c.id_credito = b.credito
WHERE
    a.id_proveedor = @idProveedor and  (a.total - a.pago) != 0
)
 AS TotalRowCount;
";
            var rows = 0;
            try
            {
                using var connection = _ctx.CreateConnection();
                rows = await connection.QuerySingleAsync<int>(query, new { idProveedor});
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rows;
        }

        public async Task<List<ListaEvaluacionProveedor>> GetListaEvaluaciones(int idProveedor)
        {
            string query = @"
SET LANGUAGE Spanish;
SELECT 
id_evaluacionproveedor IdEvaluacionProveedor,
id_proveedor IdProveedor,
id_status IdStatus,
FORMAT(fecha_evaluacion, 'dddd, d MMMM, yyyy') AS FechaEvaluacion,
numero_contrato NumeroContrato,
promedio Promedio,
texto_promedio TextoPromedio,
id_usuario IdUsuario
FROM tb_evaluacionproveedor WHERE id_proveedor = @idProveedor ORDER BY fecha_evaluacion DESC
";
            var evaluaciones = new List<ListaEvaluacionProveedor>();
            try
            {
                using var connection = _ctx.CreateConnection();
                evaluaciones = (await connection.QueryAsync<ListaEvaluacionProveedor>(query, new { idProveedor})).ToList();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return evaluaciones;
        }

        public async Task <List<EvaluacionProveedor>> GetEvaluacionProveedor(int idEvaluacionProveedor)
        {
            string query = @"
SELECT 
a.id_evaluacionproveedor IdEvaluacionProveedor,
a.id_caracteristica IdCaracteristica,
b.descripcion Descripcion,
a.calificacion Calificacion,
b.criterios Criterios
FROM tb_evaluacion a
INNER JOIN tb_caracteristica b ON a.id_caracteristica = b.id_caracteristica
WHERE id_evaluacionproveedor =  @idEvaluacionProveedor ORDER BY a.id_caracteristica
";
            var evaluacion = new List<EvaluacionProveedor>();
            try
            {
                using var connection = _ctx.CreateConnection();
                evaluacion = (await connection.QueryAsync<EvaluacionProveedor>(query, new { idEvaluacionProveedor })).ToList();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return evaluacion;
        }
    }
}
