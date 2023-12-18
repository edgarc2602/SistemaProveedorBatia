using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml.Schema;
using Dapper;
using Microsoft.AspNetCore.Http;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace SistemaVentasBatia.Repositories
{
    public interface IFacturaRepository
    {
        Task<List<OrdenCompra>> ObtenerOrdenesCompra(int idProveedor, string fechaInicio, string fechaFin, int pagina);
        Task<int> ContarOrdenesCompra(int idProveedor, string fechaInicio, string fechaFin);
        Task<decimal> ObtenerSumaFacturas(int idOrden);
        Task<List<Factura>> ObtenerFacturas(int idOrden);
        Task<XMLData> ExtraerDatosXML(IFormFile xml, int idTipoFolio);
        Task<DetalleOrdenCompra> ObtenerDetalleOrden(int idOrden);
        Task<bool> InsertarXML(string xmlString);
        Task<bool> FacturaExiste(string uuid);
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
ROW_NUMBER() over (order by a.id_orden desc ) as rownum, 
a.id_orden IdOrden,
case when a.tipo = 1 then 'Materiales' else 'Servicios' end as Tipo,
e.descripcion as Estatus, 
convert(varchar(12), a.falta,103) as FechaAlta ,  
b.nombre as Empresa	,
isnull(c.nombre,'') as Proveedor, 
isnull(d.nombre,'') as Cliente,
f.Per_Nombre + ' ' + f.Per_Paterno as Elabora, 
a.Total Total, 
a.observacion Observacion, 
a.inventario Inventario,
CAST(SUM(ISNULL(r.total, 0)) AS decimal(18, 2)) Facturado
from tb_ordencompra a inner join tb_empresa b on a.id_empresa = b.id_empresa
left outer join tb_proveedor c on a.id_proveedor = c.id_proveedor
left outer join tb_cliente d on a.id_cliente = d.id_cliente
left outer join tb_statusc e on a.id_status = e.id_status inner join personal f on a.ualta = f.IdPersonal
left outer join tb_recepcion r on r.id_orden = a.id_orden
WHERE a.id_proveedor = @idProveedor
        AND (@fechaInicio IS NULL OR @fechaFin IS NULL OR falta BETWEEN @fechaInicio AND @fechaFin)
    GROUP BY 
        a.id_orden, 
        a.tipo,
        e.descripcion,
        a.falta,
        b.nombre,
        c.nombre,
        d.nombre,
        f.Per_Nombre,
        f.Per_Paterno,
        a.Total,
        a.observacion,
        a.inventario
) AS Ordenes
WHERE   
  RowNum >= ((@pagina - 1) * 40) + 1
  AND RowNum <= (@pagina * 40);
";
            var ordenes = new List<OrdenCompra>();
            try
            {
                using var connection = _ctx.CreateConnection();
                ordenes = (await connection.QueryAsync<OrdenCompra>(query, new { idProveedor, fechaInicio, fechaFin, pagina })).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ordenes;
        }

        public async Task<int> ContarOrdenesCompra(int idProveedor, string fechaInicio, string fechaFin)
        {
            var query = @"
SELECT COUNT(*) AS TotalRows
FROM (
Select  
ROW_NUMBER() over (order by id_orden desc ) as rownum, 
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
                using var connection = _ctx.CreateConnection();
                rows = await connection.QuerySingleAsync<int>(query, new { idProveedor, fechaInicio, fechaFin });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rows;
        }

        public async Task<decimal> ObtenerSumaFacturas(int idOrden)
        {
            var query = @"SELECT CAST(SUM(ISNULL(total, 0)) AS decimal(18, 2)) Total
                        FROM tb_recepcion
                        WHERE id_orden = @idOrden
";
            decimal total;
            try
            {
                using var connection = _ctx.CreateConnection();
                total = await connection.QuerySingleAsync<decimal>(query, new { idOrden });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return total;
        }

        public async Task<List<Factura>> ObtenerFacturas(int idOrden)
        {
            string query = @"
SELECT 
id_orden IdOrden,
id_recepcion IdRecepcion,
documento Documento
FROM tb_recepcion_factura WHERE id_orden = @idOrden
";

            var facturas = new List<Factura>();
            try
            {
                using var connection = _ctx.CreateConnection();
                facturas = (await connection.QueryAsync<Factura>(query, new { idOrden })).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return facturas;
        }

        public async Task<XMLData> ExtraerDatosXML(IFormFile xml, int idTipoFolio)
        {
            var XMLData = new XMLData();
            try
            {
                XDocument xDoc = XDocument.Load(xml.OpenReadStream());

                XElement comprobante = xDoc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Comprobante" && e.Name.NamespaceName.StartsWith("http://www.sat.gob.mx/cfd/"));

                if (comprobante != null)
                {
                    XNamespace cfdiNamespace = comprobante.Name.Namespace;
                    XNamespace tfd = "http://www.sat.gob.mx/TimbreFiscalDigital";

                    string subTotal = comprobante.Attribute("SubTotal")?.Value;
                    if (idTipoFolio == 1) //si es serielizada
                    {
                        string Serie = comprobante.Attribute("Serie")?.Value;
                        string Factura = comprobante.Attribute("Folio")?.Value;
                        string Folio = Serie + Factura;
                        XMLData.Factura = Folio;

                        XElement complemento = comprobante.Element(cfdiNamespace + "Complemento");
                        if (complemento != null)
                        {
                            XElement timbrefiscaldigital = complemento.Element(tfd + "TimbreFiscalDigital");
                            string UUID = timbrefiscaldigital.Attribute("UUID")?.Value;
                            XMLData.Uuid = UUID;
                            string fechaTimbrado = timbrefiscaldigital.Attribute("FechaTimbrado")?.Value;
                            DateTime fecha = DateTime.Parse(fechaTimbrado);
                            string fechaFormateada = fecha.ToString("yyyy-MM-dd");
                            XMLData.FechaFactura = fechaFormateada;
                        }
                    }
                    else
                    {
                        XElement complemento = comprobante.Element(cfdiNamespace + "Complemento");
                        if (complemento != null)
                        {
                            XElement timbrefiscaldigital = complemento.Element(tfd + "TimbreFiscalDigital");
                            string UUID = timbrefiscaldigital.Attribute("UUID")?.Value;
                            XMLData.Uuid = UUID;
                            string fechaTimbrado = timbrefiscaldigital.Attribute("FechaTimbrado")?.Value;
                            DateTime fecha = DateTime.Parse(fechaTimbrado);
                            string fechaFormateada = fecha.ToString("yyyy-MM-dd");
                            XMLData.FechaFactura = fechaFormateada;
                        }
                    }

                    XMLData.SubTotal = decimal.Parse(subTotal);

                    XElement impuestos = comprobante.Element(cfdiNamespace + "Impuestos");

                    if (impuestos != null)
                    {
                        XElement traslados = impuestos.Element(cfdiNamespace + "Traslados");

                        if (traslados != null)
                        {
                            XElement traslado = traslados.Element(cfdiNamespace + "Traslado");

                            if (traslado != null)
                            {
                                string impuesto = traslado.Attribute("Importe")?.Value;
                                XMLData.Iva = decimal.Parse(impuesto);
                                decimal total = decimal.Parse(subTotal) + decimal.Parse(impuesto);
                                XMLData.Total = total;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return XMLData;

        }

        public async Task<DetalleOrdenCompra> ObtenerDetalleOrden(int idOrden)
        {
            string query = @"
select
a.id_orden IdOrden, 
a.id_requisicion IdRequisicion, 
a.id_proveedor IdProveedor, 
a.id_cliente IdCliente,
b.nombre Proveedor, 
c.nombre Empresa, 
e.nombre Cliente, 
a.subtotal SubTotal, 
a.iva Iva, 
a.total Total, 
a.id_status Status,
d.dias Dias,
CAST(SUM(ISNULL(f.total, 0)) AS decimal(18, 2)) Facturado
from tb_ordencompra a inner join tb_proveedor b on a.id_proveedor = b.id_proveedor
inner join tb_empresa c on a.id_empresa = c.id_empresa
inner join tb_credito d on b.credito = d.id_credito
left outer join tb_cliente e on a.id_cliente = e.id_cliente
left outer join tb_recepcion f on f.id_orden = a.id_orden
where a.id_orden = @idOrden
GROUP BY
    a.id_orden,
    a.id_requisicion,
    a.id_proveedor,
    a.id_cliente,
    a.subtotal,
    a.iva,
    a.total,
    a.id_status,
    b.nombre,
    c.nombre,
    e.nombre,
    d.dias
";
            var detalle = new DetalleOrdenCompra();
            try
            {
                using var connection = _ctx.CreateConnection();
                detalle = await connection.QueryFirstAsync<DetalleOrdenCompra>(query, new { idOrden });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return detalle;
        }

        public async Task<bool> InsertarXML(string xml)
        {
            bool result;
            try
            {
                using (var connection = _ctx.CreateConnection())
                {
                    connection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("@Material", new SqlXml(new System.Xml.XmlTextReader(xml, System.Xml.XmlNodeType.Document, null)), DbType.Xml, ParameterDirection.Input);
                    parameters.Add("@IdMov", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("sp_recepcione", parameters, commandType: CommandType.StoredProcedure);

                    int idMov = parameters.Get<int>("@IdMov");
                    Console.WriteLine("ID Movimiento generado: " + idMov);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Console.WriteLine("Error: " + ex.Message);
            }
            return result;
        }

        public async Task<bool> FacturaExiste(string uuid)
        {
            var query = @"
        SELECT CASE 
            WHEN EXISTS (
                SELECT 1
                FROM tb_recepcion
                WHERE uuid = @uuid
            )
            THEN CAST(1 AS BIT)
            ELSE CAST(0 AS BIT)
        END AS Existe
    ";
            try
            {
                using var connection = _ctx.CreateConnection();
                var result = await connection.ExecuteScalarAsync<bool>(query, new { uuid });
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
