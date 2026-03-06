using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SistemaVentasBatia.Services
{
    public interface IRequisicionService
    {
        Task<ListaRequisicionesDTO> GetRequisiciones(int idProveedor, int pagina, int fltMes, int fltEstatus, int fltAnio);
        Task<RequisicionDetalleDTO> GetRequisicionDetalle(int idRequisicion);
        Task<ResultadoDTO> PutActualizaRequisicionNuevoPrecio(RequisicionDetalleDTO requisicion, float ivaNuevo, float subTotalNuevo, float totalNuevo, int idProveedor);
        Task<bool> NuevaRequisicion(
            int idOrdenCompra,
            int idProveedor,
            int idPersonal,
            string diasCredito,
            string xmlMovimiento,
            string xmlComprobante,
            IFormFile pdf,
            IFormFile xmlFile);
        
    }
    public class RequisicionService : IRequisicionService
    {
        private readonly IRequisicionRepository _repo;
        private readonly IMapper _mapper;
        private readonly DapperContext _ctx;


        public RequisicionService(IRequisicionRepository repoRequisicion, IMapper mapper, DapperContext context)
        {
            _repo = repoRequisicion;
            _mapper = mapper;
            _ctx = context;
        }

        public async Task<ListaRequisicionesDTO> GetRequisiciones(int idProveedor, int pagina, int fltMes, int fltEstatus, int fltAnio)
        {
            return _mapper.Map<ListaRequisicionesDTO>(await _repo.GetRequisiciones(idProveedor, pagina, fltMes, fltEstatus,fltAnio));
        }

        public async Task<RequisicionDetalleDTO> GetRequisicionDetalle(int idRequisicion)
        {
            return _mapper.Map<RequisicionDetalleDTO>(await _repo.GetRequisicionDetalle(idRequisicion));
        }

        public async Task<ResultadoDTO> PutActualizaRequisicionNuevoPrecio(RequisicionDetalleDTO requisicion, float ivaNuevo, float subTotalNuevo, float totalNuevo, int idProveedor) {
            var result = _mapper.Map<ResultadoDTO>(await _repo.PutActualizaRequisicionNuevoPrecio(_mapper.Map<RequisicionDetalle>(requisicion), ivaNuevo, subTotalNuevo, totalNuevo));

            var req = new Requisicion {
                IdRequisicion = requisicion.IdRequisicion,
                IdProveedor = idProveedor,
                IvaNuevo = ivaNuevo,
                SubTotalNuevo = subTotalNuevo,
                TotalNuevo = totalNuevo
            };
            await EnviarCorreo(req);

            return result;

        }
        public async Task<bool> NuevaRequisicion(
            int idOrdenCompra,
            int idProveedor,
            int idPersonal,
            string diasCredito,
            string xmlMovimiento,
            string xmlComprobante,
            IFormFile pdf,
            IFormFile xmlFile) {

            string xmlMovimientoNuevo = LeerXml(xmlFile, pdf, idOrdenCompra, idProveedor, idPersonal, diasCredito);

            try {

                xmlMovimiento = xmlMovimientoNuevo;
                //RUTA PROD
                var basePath = Path.Combine(
                    "C:\\inetpub\\wwwroot\\SINGA_APP\\Doctos\\compras\\",
                    idOrdenCompra.ToString(), "\\"
                );
                //RUTA TEST
                //var basePath = Path.Combine(
                //    @"C:\Users\LAP_Sistemas8\source\repos\SistemaProveedorBatia\SistemaVentasBatia\xml",
                //    idOrdenCompra.ToString()
                //);


                if(!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);

                var pdfPath = Path.Combine(basePath, $"{ pdf.FileName }.pdf");
                var xmlPath = Path.Combine(basePath, $"{xmlFile.FileName}.xml");

                if(pdf != null && pdf.Length > 0) {
                    using var stream = new FileStream(pdfPath, FileMode.Create);
                    await pdf.CopyToAsync(stream);
                }

                if(xmlFile != null && xmlFile.Length > 0) {
                    using var stream = new FileStream(xmlPath, FileMode.Create);
                    await xmlFile.CopyToAsync(stream);
                }

                int idRequisicion = await _repo.NuevaRequisicion(xmlMovimiento);
                if( idRequisicion != 0) {

                    var requisicion = new Requisicion {
                    IdRequisicion = idRequisicion
                    };

                    // NO ES NECESARIO ENVIAR CORREO AL SUBIR FACTURA
                    //await EnviarCorreo(requisicion);
                } else {
                    throw new Exception("No se pudo crear la requisicion");
                }
                return true;
            } catch(Exception ex) {
                throw ex;
                //return false;
            }
        }

        public string LeerXml(IFormFile xmlFile, IFormFile pdf, int idOrdenCompra, int idProveedor, int idPersonal, string diasCredito) {
            XDocument doc;
            using(var stream = xmlFile.OpenReadStream()) {
                doc = XDocument.Load(stream);
            }

            var comprobante = doc.Descendants()
                .FirstOrDefault(x => x.Name.LocalName == "Comprobante");

            var emisor = doc.Descendants()
                .FirstOrDefault(x => x.Name.LocalName == "Emisor");

            var receptor = doc.Descendants()
                .FirstOrDefault(x => x.Name.LocalName == "Receptor");

            var timbre = doc.Descendants()
                .FirstOrDefault(x => x.Name.LocalName == "TimbreFiscalDigital");

            var impuestos = doc.Descendants()
                .FirstOrDefault(x => x.Name.LocalName == "Impuestos");

            var trasladoIva = doc.Descendants()
                .Where(x =>
                    x.Name.LocalName == "Traslado" &&
                    x.Attribute("Impuesto")?.Value == "002" &&
                    !x.Ancestors().Any(a => a.Name.LocalName == "Concepto")
                )
                .FirstOrDefault();

            string folio = comprobante?.Attribute("Folio")?.Value ?? "";
            string subtotal = comprobante?.Attribute("SubTotal")?.Value ?? "";
            string total = comprobante?.Attribute("Total")?.Value ?? "";
            string fecha = comprobante?.Attribute("Fecha")?.Value ?? "";

            string rfcReceptor = receptor?.Attribute("Rfc")?.Value ?? "";

            string uuid = timbre?.Attribute("UUID")?.Value ?? "";

            //string iva = impuestos?.Attribute("TotalImpuestosTrasladados")?.Value ?? "0";
            string iva = trasladoIva?.Attribute("Importe")?.Value ?? "0";

            string fecfac = DateTime
                .Parse(fecha)
                .ToString("yyyyMMdd");


            var movimiento = new XElement("Movimiento",
            new XElement("salida",
                new XAttribute("orden", idOrdenCompra),
                new XAttribute("factura", folio),
                new XAttribute("uuid", uuid),
                new XAttribute("fecfac", fecfac),
                new XAttribute("usuario", idPersonal),
                new XAttribute("idproveedor", idProveedor),
                new XAttribute("dias", diasCredito),
                new XAttribute("sub", subtotal),
                new XAttribute("iva", iva),
                new XAttribute("total", total),
                new XAttribute("rfc", rfcReceptor)
            ),
                new XElement("archivo", new XAttribute("nombre", pdf.FileName)),
                new XElement("archivo", new XAttribute("nombre", xmlFile.FileName))
            );

            string xmlMovimiento = movimiento.ToString();

            return xmlMovimiento;



        }

        public async Task<bool> EnviarCorreo(Requisicion requisicion) {
            //string fechastring = DateTime.Now.ToString("dd/MM/yyyy");
            
            var correoComprador = await _repo.ObtenerCorreoComprador(requisicion.IdProveedor);
            var ProveedorNombre = await _repo.ObtenerNombreProveedor(requisicion.IdProveedor);
            //requisicion .nombreProveedor = ProveedorNombre;

            await PostEnviarNotificacionComprador(correoComprador, requisicion.IdRequisicion, ProveedorNombre);

            return true;
        }

        public async Task<Resultado> PostEnviarNotificacionComprador(string correoComprador,int idRequisicion, string nombreProveedor) {
            Resultado resultado = new Resultado();

            string correos = correoComprador;
            string asunto = $"NOTIFICACION DE SINGA, Cambio de precios de la requisicion No: {idRequisicion}";
            string titulo = "Cambio de precios de requisicion";
            string mensaje = "";
            mensaje = $"Buen dia, el proveedor {nombreProveedor}, a modificado los precios de la requisicion {idRequisicion}";

            try {
                using var connection = _ctx.CreateConnection();
                connection.Open();
                var parameters = new DynamicParameters();
                //parameters.Add("@Cabecero", "", DbType.String, ParameterDirection.Input);
                parameters.Add("@correo_contactos", correos, DbType.String, ParameterDirection.Input);
                parameters.Add("@correo_asunto", "", DbType.String, ParameterDirection.Input);
                parameters.Add("@cabecero_titulo", "Mensaje generado por SINGA", DbType.String, ParameterDirection.Input);
                parameters.Add("@cuerpo_titulo", titulo, DbType.String, ParameterDirection.Input);
                parameters.Add("@cuerpo_parrafo", mensaje, DbType.String, ParameterDirection.Input);
                parameters.Add("@cuerpo_lista", "", DbType.String, ParameterDirection.Input);

                await connection.ExecuteAsync("sp_smtp_envia_correo", parameters, commandType: CommandType.StoredProcedure);

                //int idMov = parameters.Get<int>("@IdRequisicion");
                resultado.Estatus = true;
                resultado.Mensaje = "Se han actualizado los precios de los productos correctamente";
            } catch(Exception ex) {
                resultado.Estatus = false;
                resultado.MensajeError = "Error: " + ex.Message;
            }

            return resultado;
        }
    }
}