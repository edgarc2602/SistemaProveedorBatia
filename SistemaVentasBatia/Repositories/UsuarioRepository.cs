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
        //Task<bool> InsertarUsuario(UsuarioRegistro usuario);
        Task<bool> ConsultarUsuario(int idPersonal, string Nombres);
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
            string query = @"SELECT per_usuario Identificador, per_nombre Nombre, idpersonal as IdPersonal,
                    per_interno as IdInterno, per_status Estatus, id_empleado as IdEmpleado
                FROM personal where per_usuario = @Usuario and per_password = @Contrasena;"; // and per_status=0

            using (var connection = _ctx.CreateConnection())
            {
                usu = (await connection.QueryFirstOrDefaultAsync<Usuario>(query, acceso));
            }
            return usu;
        }

//        public async Task<bool> InsertarUsuario(UsuarioRegistro usuario)
//        {
//            var base64Data = usuario.Firma;

//            if (base64Data.StartsWith("data:image/jpeg;base64,"))
//            {
//                base64Data = base64Data.Substring("data:image/jpeg;base64,".Length);
//            }
//            if (base64Data.StartsWith("data:image/png;base64,"))
//            {
//                base64Data = base64Data.Substring("data:image/png;base64,".Length);
//            }
//            usuario.Firma = base64Data;

//            var query = @"
//INSERT INTO Autorizacion_ventas 
//(
//IdPersonal,
//per_autoriza,
//per_nombre,
//per_puesto,
//per_telefono,
//per_telefono_extension,
//per_telefonomovil,
//per_email,
//per_firma,
//per_revisa
//)
//VALUES
//(
//@IdPersonal,
//@Autoriza,
//@Nombres + ' ' + @Apellidos,
//@Puesto,
//@Telefono,
//@TelefonoExtension,
//@TelefonoMovil,
//@Email,
//@Firma,
//@Revisa
//)
//";
//            bool result = false;
//            int rowsAffected = 0;
//            try
//            {
//                using (var connection = _ctx.CreateConnection())
//                {
//                    rowsAffected = await connection.ExecuteAsync(query, usuario);
//                    if (rowsAffected > 0)
//                    {
//                        result = true;
//                    }
//                    else
//                    {
//                        result = false;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//            return result;
//        }

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
                using (var connection = _ctx.CreateConnection())
                {
                    result = await connection.QueryFirstOrDefaultAsync<bool>(query, new { idPersonal, Nombres = "%" + Nombres + "%" });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}