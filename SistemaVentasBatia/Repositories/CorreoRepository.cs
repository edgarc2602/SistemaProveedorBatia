using Dapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Repositories
{
    public interface ICorreoRepository
    {
        Task<Resultado> GetCorreoCompradorPorRequisicion(int idRequisicion);
    }

    public class CorreoRepository : ICorreoRepository
    {
        private readonly DapperContext _ctx;

        public CorreoRepository(DapperContext context)
        {
            _ctx = context;
        }

        public async Task<Resultado> GetCorreoCompradorPorRequisicion(int idRequisicion)
        {
            Resultado resultado = new Resultado();
            Correos correos = new Correos();

            string query = @"
                    -- Consulta para obtener los materiales de la requisicion
                    select distinct top 1
	                    c.Per_Email as Correo
                    from tb_requisicion as a
                    inner join tb_empleado as b on a.id_comprador = b.id_empleado
                    inner join Personal as c on b.id_empleado = c.id_empleado
                    where b.id_status = 2
                    and c.Per_Interno = 0 -- Si el usuario es de tipo interno
                    and a.id_requisicion = @idRequisicion
                ";

            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    correos.ListaCorreos = (await connection.QueryAsync<Correos>(query, new { idRequisicion })).ToList();
                    resultado.Estatus = true;
                    resultado.Objeto = correos;
                }
            }
            catch (Exception ex)
            {
                resultado.Estatus = false;
                resultado.Mensaje = "Error: " + ex.Message;
                resultado.MensajeError = ex.ToString();
            }
            return resultado;
        }

    }
}