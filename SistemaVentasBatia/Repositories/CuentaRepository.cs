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

    }

    public class CuentaRepository : ICuentaRepository
    {
        //private readonly DapperContext _ctx;

        //public CuentaRepository(DapperContext context)
        //{
        //    _ctx = context;
        //}
    }
}
