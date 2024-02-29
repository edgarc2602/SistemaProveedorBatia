﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using SistemaVentasBatia.Controllers;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioDTO> Login(AccesoDTO dto);
        Task<bool> Existe(AccesoDTO dto);
        Task<GraficaListadoAnio[]> ObtenerListadosAnio(int anio, int idProveedor);
        Task<GraficaListado> ObtenerListadosAnioMes(int anio, int mes, int idProveedor);
        Task<List<GraficaOrden>> ObtenerOrdenesAnio(int anio, int idProveedor);
        Task<GraficaOrden> ObtenerOrdenesAnioMes(int anio, int mes, int idProveedor);
        Task<string> ObtenerEvaluacionTiempoEntrega(int anio, int mes, int idProveedor);
        Task<decimal> ObtenerPorcentajeCargaAcuses(int anio, int mes, int idProveedor);
    }
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;
        private readonly IMapper _mapper;

        public UsuarioService(IUsuarioRepository repoUsuario, IMapper mapper)
        {
            _repo = repoUsuario;
            _mapper = mapper;
        }

        public async Task<bool> Existe(AccesoDTO dto)
        {
            bool existe = false;
            try
            {
                Acceso acc = _mapper.Map<Acceso>(dto);
                UsuarioDTO usu = _mapper.Map<UsuarioDTO>(await _repo.Login(acc));
                if (usu == null)
                {
                    existe = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return existe;
        }

        public async Task<UsuarioDTO> Login(AccesoDTO dto)
        {
            UsuarioDTO usu;
            try
            {
                Acceso acc = _mapper.Map<Acceso>(dto);
                usu = _mapper.Map<UsuarioDTO>(await _repo.Login(acc));
                if (usu == null)
                {
                    throw new CustomException("Usuario no Existe");
                }
                if (usu.Estatus != 0)
                {
                    throw new CustomException("Usuario inactivo");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return usu;
        }

        public  Task<GraficaListadoAnio[]> ObtenerListadosAnio(int anio, int idProveedor)
        {
            return  _repo.ObtenerListadosAnio(anio, idProveedor);
        }

        public async Task<GraficaListado> ObtenerListadosAnioMes(int anio, int mes, int idProveedor)
        {
            return await _repo.ObtenerListadosAnioMes(anio, mes, idProveedor);
        }

        public async Task<List<GraficaOrden>> ObtenerOrdenesAnio(int anio, int idProveedor)
        {
            return await _repo.ObtenerOrdenesAnio(anio, idProveedor);
        }

        public async Task<GraficaOrden> ObtenerOrdenesAnioMes(int anio, int mes, int idProveedor)
        {
            return await _repo.ObtenerOrdenesAnioMes(anio, mes, idProveedor);
        }

        public async Task<string> ObtenerEvaluacionTiempoEntrega(int anio, int mes, int idProveedor)
        {
            return await _repo.ObtenerEvaluacionTiempoEntrega(anio, mes, idProveedor);
        }

        public async Task<decimal> ObtenerPorcentajeCargaAcuses(int anio, int mes, int idProveedor)
        {
            int totalListados;
            int totalAcuses;
            decimal porcentajeCargaAcuses;
            totalListados = await _repo.ObtenerTotalListadosPorAnioMes(anio, mes, idProveedor);
            totalAcuses = await _repo.ObtenerTotalAcusesPorAnioMes (anio, mes, idProveedor);
            porcentajeCargaAcuses = ((decimal)totalAcuses / totalListados) * 100M;
            string porcentajeFormateado = porcentajeCargaAcuses.ToString("0.00");
            decimal porcentajeFormateadoDecimal = decimal.Parse(porcentajeFormateado);

            return porcentajeFormateadoDecimal;
        }
    }
}
