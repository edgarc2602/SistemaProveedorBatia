﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _logic;

        public UsuarioController(IUsuarioService logic)
        {
            _logic = logic;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<UsuarioDTO>> Login(AccesoDTO dto)
        {
            return await _logic.Login(dto);
        }

        [HttpGet("[action]/{anio}/{idProveedor}")]
        public async Task<List<GraficaListado>> ObtenerGraficaListadoAnio(int anio, int idProveedor)
        {
            return await _logic.ObtenerListadosAnio(anio, idProveedor);
        }

        [HttpGet("[action]/{anio}/{mes}/{idProveedor}")]
        public async Task<GraficaListado> ObtenerGraficaListadoAnioMes(int anio, int mes, int idProveedor)
        {
            return await _logic.ObtenerListadosAnioMes(anio, mes, idProveedor);
        }

        [HttpGet("[action]/{anio}/{idProveedor}")]
        public async Task<List<GraficaOrden>> ObtenerOrdenesAnio(int anio, int idProveedor)
        {
            return await _logic.ObtenerOrdenesAnio(anio, idProveedor);
        }

        [HttpGet("[action]/{anio}/{mes}/{idProveedor}")]
        public async Task<GraficaOrden> ObtenerOrdenesAnioMes(int anio, int mes, int idProveedor)
        {
            return await _logic.ObtenerOrdenesAnioMes(anio, mes, idProveedor);
        }
    } 
}
