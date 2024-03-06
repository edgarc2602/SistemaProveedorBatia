using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> Login(Acceso acceso);
        Task<bool> ConsultarUsuario(int idPersonal, string Nombres);
        Task<GraficaListadoAnio[]> ObtenerListadosAnio(int anio, int idProveedor);
        Task<GraficaListado> ObtenerListadosAnioMes(int anio,int mes, int idProveedor);
        Task<List<GraficaOrden>> ObtenerOrdenesAnio(int anio, int idProveedor);
        Task<GraficaOrden> ObtenerOrdenesAnioMes(int anio, int mes, int idProveedor);
        Task<string> ObtenerEvaluacionTiempoEntrega(int anio, int mes, int idProveedor);
        Task<int> ObtenerTotalListadosPorAnioMes(int anio, int mes, int idProveedor);
        Task<int> ObtenerTotalAcusesPorAnioMes(int anio, int mes, int idProveedor);
    }

    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DapperContext _ctx;

        public UsuarioRepository(DapperContext context)
        {
            _ctx = context;
        }

        public async Task<Usuario> Login(Acceso acceso)
        {
            Usuario usu;
            string query = @"
SELECT 
per_usuario Identificador, 
per_nombre Nombre, 
idpersonal as IdPersonal,
per_interno as IdInterno, 
per_status Estatus, 
id_empleado as IdEmpleado,
id_proveedor as IdProveedor
FROM personal where per_usuario = @Usuario and per_password = @Contrasena
"; // and per_status=0

            using (var connection = _ctx.CreateConnection())
            {
                usu = (await connection.QueryFirstOrDefaultAsync<Usuario>(query, acceso));
            }
            return usu;
        }

        public async Task<bool> ConsultarUsuario(int idPersonal, string Nombres)
        {
            var query = @"
           SELECT CASE
           WHEN COUNT(*) > 0 THEN 'true'
           ELSE 'false'
           END AS Resultado
           FROM Personal
           WHERE IdPersonal = @idPersonal AND Per_Nombre LIKE @Nombres;
";
            bool result = false;
            try
            {
                using var connection = _ctx.CreateConnection();
                result = await connection.QueryFirstOrDefaultAsync<bool>(query, new { idPersonal, Nombres = "%" + Nombres + "%" });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public async Task<GraficaListadoAnio[]> ObtenerListadosAnio(int anio, int idProveedor)
        {
            string query = @"
        SELECT 
            b.mes,
            COUNT(*) AS TotalListadosPorMes
        FROM 
            tb_cliente_inmueble a 
        LEFT OUTER JOIN		
            tb_listadomaterial b ON a.id_inmueble = b.id_inmueble
        LEFT OUTER JOIN 
            tb_proveedorinmueble e ON e.id_inmueble = a.id_inmueble
        WHERE 
            e.id_proveedor = @idProveedor
            AND a.id_status = 1
            AND a.materiales = 0
            AND b.anio = @anio
	        AND b.id_status IN (2,4)
        GROUP BY 
            b.mes
        ORDER BY 
            b.mes
    ";

            try
            {
                using var connection = _ctx.CreateConnection();
                var listadosanio = await connection.QueryAsync<GraficaListadoAnio>(query, new { anio, idProveedor });

                GraficaListadoAnio[] arreglo = new GraficaListadoAnio[12];
                for (int i = 0; i < 12; i++)
                {
                    arreglo[i] = new GraficaListadoAnio
                    {
                        Mes = i + 1,
                        TotalListadosPorMes = 0
                    };
                }

                // Rellenar el arreglo con los valores de la consulta
                foreach (var item in listadosanio)
                {
                    arreglo[item.Mes - 1] = item;
                }

                return arreglo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GraficaListado> ObtenerListadosAnioMes(int anio, int mes, int idProveedor)
        {
            string query = @"
        SELECT 
            b.mes,
            COUNT(*) AS TotalListadosPorMes,
            SUM(CASE WHEN b.id_status = 1 THEN 1 ELSE 0 END) AS Alta,
            SUM(CASE WHEN b.id_status = 2 THEN 1 ELSE 0 END) AS Aprobado,
            SUM(CASE WHEN b.id_status = 3 THEN 1 ELSE 0 END) AS Despachado,
            SUM(CASE WHEN b.id_status = 4 THEN 1 ELSE 0 END) AS Entregado,
            SUM(CASE WHEN b.id_status = 5 THEN 1 ELSE 0 END) AS Cancelado
        FROM 
            tb_cliente_inmueble a 
        LEFT OUTER JOIN 
            tb_listadomaterial b ON a.id_inmueble = b.id_inmueble
        LEFT OUTER JOIN 
            tb_proveedorinmueble e ON e.id_inmueble = a.id_inmueble
        WHERE 
            e.id_proveedor = @idProveedor
            AND a.id_status = 1 
            AND a.materiales = 0
            AND b.anio = @anio
            AND b.mes = @mes
	        AND b.id_status IN (2,4)
        GROUP BY 
            b.mes
        ORDER BY 
            b.mes";
            try
            {
                using var connection = _ctx.CreateConnection();

                GraficaListado listadosaniomes = await connection.QueryFirstOrDefaultAsync<GraficaListado>(query, new { anio, mes, idProveedor });
                if (listadosaniomes == null)
                {
                    GraficaListado listadosaniomes2 = new GraficaListado
                    {
                        TotalListadosPorMes = 0,
                        Alta = 0,
                        Aprobado = 0,
                        Despachado = 0,
                        Entregado = 0,
                        Cancelado = 0
                    };

                    return listadosaniomes2;
                }

                return listadosaniomes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<GraficaOrden>> ObtenerOrdenesAnio(int anio, int idProveedor)
        {
            string query = @"
WITH Meses AS (
    SELECT 1 AS numero_mes, 'Ene' AS nombre_mes
    UNION ALL SELECT 2, 'Feb'
    UNION ALL SELECT 3, 'Mar'
    UNION ALL SELECT 4, 'Abr'
    UNION ALL SELECT 5, 'May'
    UNION ALL SELECT 6, 'Jun'
    UNION ALL SELECT 7, 'Jul'
    UNION ALL SELECT 8, 'Ago'
    UNION ALL SELECT 9, 'Sep'
    UNION ALL SELECT 10, 'Oct'
    UNION ALL SELECT 11, 'Nov'
    UNION ALL SELECT 12, 'Dic'
)

SELECT 
    Meses.numero_mes AS Mes,
    COUNT(oc.id_orden) AS TotalOrdenesPorMes,
    ISNULL(SUM(oc.total), 0) AS Total
FROM 
    Meses
LEFT JOIN 
    tb_ordencompra oc ON MONTH(oc.falta) = Meses.numero_mes AND YEAR(oc.falta) = @anio AND oc.id_proveedor = @idProveedor
GROUP BY 
    Meses.numero_mes
ORDER BY 
    Meses.numero_mes;
";
            var ordenesanio = new List<GraficaOrden>();
            try
            {
                using var connection = _ctx.CreateConnection();
                ordenesanio = (await connection.QueryAsync<GraficaOrden>(query, new { anio, idProveedor })).ToList();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return ordenesanio;
        }

        public async Task<GraficaOrden> ObtenerOrdenesAnioMes(int anio, int mes, int idProveedor)
        {
            string query = @"
SELECT 
    ISNULL(COUNT(oc.falta), 0) AS TotalPorMes,
    ISNULL(SUM(CASE WHEN oc.id_status = 1 THEN 1 ELSE 0 END), 0) AS Alta,
    ISNULL(SUM(CASE WHEN oc.id_status = 2 THEN 1 ELSE 0 END), 0) AS Autorizada,
    ISNULL(SUM(CASE WHEN oc.id_status = 3 THEN 1 ELSE 0 END), 0) AS Rechazada,
    ISNULL(SUM(CASE WHEN oc.id_status = 4 THEN 1 ELSE 0 END), 0) AS Completa
FROM 
    tb_ordencompra oc
WHERE 
    MONTH(oc.falta) = @mes
    AND YEAR(oc.falta) = @anio
    AND oc.id_proveedor = @idProveedor
GROUP BY 
    MONTH(oc.falta), YEAR(oc.falta);
";
            try
            {
                using var connection = _ctx.CreateConnection();
                GraficaOrden ordenesaniomes = await connection.QueryFirstOrDefaultAsync<GraficaOrden>(query, new { anio, mes, idProveedor });
                if (ordenesaniomes == null)
                {
                    GraficaOrden ordenesaniomes2 = new GraficaOrden
                    {
                        TotalOrdenesPorMes = 0,
                        Alta = 0,
                        Autorizada = 0,
                        Rechazada = 0,
                        Completa = 0,
                        Despachada = 0,
                        EnRequisicion = 0
                    };

                    return ordenesaniomes2;
                }
                return ordenesaniomes;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> ObtenerEvaluacionTiempoEntrega(int anio, int mes,int idProveedor)
        {
            string query = @"
--Obtiene el total de entregas completadas que tienen fCalendario
DECLARE @TotalRegistros INT;
SELECT @TotalRegistros = COUNT(*)
FROM tb_cliente_inmueble a 
INNER JOIN tb_listadomaterial b ON a.id_inmueble = b.id_inmueble
INNER JOIN tb_proveedorinmueble e ON e.id_inmueble = a.id_inmueble
WHERE e.id_proveedor = @idProveedor AND 
      a.id_status = 1 AND 
      ISNULL(NULLIF(@mes, 0), b.mes) = b.mes AND
      ISNULL(NULLIF(@anio, 0), b.anio) = b.anio
	  AND b.id_status = 4
	  AND b.fcalendario IS NOT NULL;
	  PRINT @TotalRegistros
DECLARE @Iguales INT;
SELECT @Iguales = COUNT(*)
FROM tb_cliente_inmueble a 
LEFT OUTER JOIN tb_listadomaterial b ON a.id_inmueble = b.id_inmueble
INNER JOIN tb_proveedorinmueble e ON e.id_inmueble = a.id_inmueble
WHERE e.id_proveedor = @idProveedor AND 
      a.id_status = 1 AND 
      ISNULL(NULLIF(@mes, 0), b.mes) = b.mes AND
      ISNULL(NULLIF(@anio, 0), b.anio) = b.anio
	  AND b.fcalendario IS NOT NULL
      --AND CONVERT(DATE, b.fcalendario) >= CONVERT(DATE, b.fentrega)
	  AND CONVERT(DATE, b.fentrega) <= CONVERT(DATE, b.fcalendario)
	  AND b.id_status = 4
	  PRINT @Iguales
DECLARE @PorcentajeIguales DECIMAL(10, 2);

IF @TotalRegistros > 0
BEGIN
    SET @PorcentajeIguales = (CONVERT(DECIMAL(10, 2), @Iguales) / CONVERT(DECIMAL(10, 2), @TotalRegistros)) * 100;
END
ELSE
BEGIN
    SET @PorcentajeIguales = 0;
END
SELECT 
    @PorcentajeIguales AS PorcentajeIguales
";
            decimal porcentaje = 0;
            try
            {
                using var connection = _ctx.CreateConnection();
                porcentaje = await connection.QueryFirstOrDefaultAsync<decimal>(query, new { anio, mes, idProveedor });
            }
            catch(Exception ex)
            {
                throw ex;
            }
            string porcentajeFormateado = porcentaje.ToString("0.##");
            return porcentajeFormateado;
        }

        public async Task<int> ObtenerTotalListadosPorAnioMes(int anio, int mes, int idProveedor)
        {
            string query = @"
                SELECT COUNT(*) AS TotalRegistros
                FROM tb_listadomaterial a
                inner join tb_proveedorinmueble e ON e.id_inmueble = a.id_inmueble
                WHERE
                e.id_proveedor = @idProveedor and 
                a.id_status IN (2,4) AND
                ISNULL(NULLIF(@mes,0), a.mes) = a.mes AND
                ISNULL(NULLIF(@anio,0), a.anio) = a.anio
                ";
            int rows;
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    rows = await connection.QuerySingleAsync<int>(query, new { anio,mes, idProveedor });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rows;
        }
        public async Task<int> ObtenerTotalAcusesPorAnioMes(int anio, int mes, int idProveedor)
        {
            string query = @"
                SELECT 
                COUNT(*) AS AcusesCargados
                FROM tb_listadomaterial a
                INNER JOIN tb_listadomateriala b ON b.id_listado = a.id_listado
                inner join tb_proveedorinmueble e ON e.id_inmueble = a.id_inmueble
                WHERE
                e.id_proveedor = @idProveedor and 
                a.id_status IN (2,4) AND
                ISNULL(NULLIF(@mes,0), a.mes) = a.mes AND
                ISNULL(NULLIF(@anio,0), a.anio) = a.anio 
            ";
            int rows;
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    rows = await connection.QuerySingleAsync<int>(query, new { anio, mes, idProveedor });
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