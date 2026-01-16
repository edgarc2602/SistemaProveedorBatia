using AutoMapper;
using Dapper;
using SistemaProveedoresBatia.DTOs;
using SistemaVentasBatia.Controllers;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Services
{
    public interface ICorreoService
    {
        Task<ResultadoDTO> EnviaCorreo(string contactos, string asunto, string titulo, string mensaje, string mensaje_liista_xml);
        Task<ResultadoDTO> GetCorreoCompradorPorRequisicion(int idRequisicion);
    }

    public class CorreoService : ICorreoService
    {
        private readonly ICorreoRepository _repo;
        private readonly IMapper _mapper;

        public CorreoService(ICorreoRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ResultadoDTO> EnviaCorreo(string contactos, string asunto, string titulo, string mensaje, string mensaje_lista_xml)
        {
            ResultadoDTO resultado = new ResultadoDTO();

            try
            {
                //using var connection = _ctx.CreateConnection(); // para el servidor
                using var connection = new SqlConnection("Server=192.168.2.3; Database=SINGA; User Id=sa; Password=S1ng42019;"); // para localhost

                connection.Open();
                var parameters = new DynamicParameters();

                parameters.Add("@correo_contactos", contactos, DbType.String, ParameterDirection.Input);
                parameters.Add("@correo_asunto", asunto, DbType.String, ParameterDirection.Input);
                parameters.Add("@cabecero_titulo", "Mensaje generado por SINGA", DbType.String, direction: ParameterDirection.Input);
                parameters.Add("@cuerpo_titulo", titulo, dbType: DbType.String, direction: ParameterDirection.Input);
                parameters.Add("@cuerpo_parrafo", mensaje, dbType: DbType.String, direction: ParameterDirection.Input);
                if (String.IsNullOrWhiteSpace(mensaje_lista_xml))
                {
                    parameters.Add("@cuerpo_lista", DBNull.Value, DbType.String, ParameterDirection.Input);
                }
                else
                {
                    parameters.Add("@cuerpo_lista", mensaje_lista_xml);
                }

                await connection.ExecuteAsync("sp_smtp_envia_correo", parameters, commandType: CommandType.StoredProcedure);

                resultado.Estatus = true;
                resultado.Mensaje = "Se ha enviado el correo de forma exitosa";
            }
            catch (Exception ex)
            {
                resultado.Estatus = false;
                resultado.MensajeError = "Error: " + ex.Message;
            }

            return resultado;
        }


        public async Task<ResultadoDTO> GetCorreoCompradorPorRequisicion(int idRequisicion) {
            return _mapper.Map<ResultadoDTO>(await _repo.GetCorreoCompradorPorRequisicion(idRequisicion));
        }
    }
}
