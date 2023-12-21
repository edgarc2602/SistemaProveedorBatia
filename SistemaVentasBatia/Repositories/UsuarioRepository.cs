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
        Task<List<GraficaListado>> ObtenerListadosAnio(int anio, int idProveedor);
        Task<GraficaListado> ObtenerListadosAnioMes(int anio,int mes, int idProveedor);
        Task<List<GraficaOrden>> ObtenerOrdenesAnio(int anio, int idProveedor);
        Task<GraficaOrden> ObtenerOrdenesAnioMes(int anio, int mes, int idProveedor);
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

        public async Task<List<GraficaListado>> ObtenerListadosAnio(int anio, int idProveedor)
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
GROUP BY 
    b.mes
ORDER BY 
    b.mes
";
            var listadosanio = new List<GraficaListado>();
            try
            {
                using var connection = _ctx.CreateConnection();
                listadosanio = (await connection.QueryAsync<GraficaListado>(query, new {anio, idProveedor})).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listadosanio;
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
SELECT 
    m.numero_mes AS Mes,
    COUNT(oc.mes) AS TotalOrdenesPorMes,
    SUM(CASE WHEN oc.id_status = 1 THEN 1 ELSE 0 END) AS Alta,
    SUM(CASE WHEN oc.id_status = 2 THEN 1 ELSE 0 END) AS Autorizada,
    SUM(CASE WHEN oc.id_status = 3 THEN 1 ELSE 0 END) AS Rechazada,
    SUM(CASE WHEN oc.id_status = 4 THEN 1 ELSE 0 END) AS Completa
FROM 
    (VALUES 
        (1, 'Ene'), (2, 'Feb'), (3, 'Mar'), (4, 'Abr'), (5, 'May'), (6, 'Jun'),
        (7, 'Jul'), (8, 'Ago'), (9, 'Sep'), (10, 'Oct'), (11, 'Nov'), (12, 'Dic')
    ) AS m(numero_mes, nombre_mes)
LEFT JOIN 
    tb_ordencompra oc ON oc.mes = m.numero_mes AND oc.anio = @anio AND oc.id_proveedor = @idProveedor
GROUP BY 
    m.numero_mes, m.nombre_mes
ORDER BY 
    m.numero_mes;
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
    ISNULL(COUNT(oc.mes), 0) AS TotalPorMes,
    ISNULL(SUM(CASE WHEN oc.id_status = 1 THEN 1 ELSE 0 END), 0) AS Alta,
    ISNULL(SUM(CASE WHEN oc.id_status = 2 THEN 1 ELSE 0 END), 0) AS Autorizada,
    ISNULL(SUM(CASE WHEN oc.id_status = 3 THEN 1 ELSE 0 END), 0) AS Rechazada,
    ISNULL(SUM(CASE WHEN oc.id_status = 4 THEN 1 ELSE 0 END), 0) AS Completa
FROM 
    tb_ordencompra oc
WHERE 
    oc.mes = @mes
    AND oc.anio = @anio
    AND oc.id_proveedor = @idProveedor
GROUP BY 
    oc.mes, oc.anio;

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
    }
}