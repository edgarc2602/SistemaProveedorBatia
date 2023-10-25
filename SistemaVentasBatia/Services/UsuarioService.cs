﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using SistemaVentasBatia.DTOs;
using SistemaVentasBatia.Models;
using SistemaVentasBatia.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SistemaVentasBatia.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioDTO> Login(AccesoDTO dto);
        Task<bool> Existe(AccesoDTO dto);

        Task<bool> InsertarUsuario(UsuarioRegistro usuario);
        Task<List<UsuarioGrafica>> ObtenerCotizacionesUsuarios();
        Task<List<UsuarioGraficaMensual>> ObtenerCotizacionesMensuales();
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

        public async Task<bool> InsertarUsuario(UsuarioRegistro usuario)
        {
            bool existe = false;
            bool result = false;
            existe = await _repo.ConsultarUsuario(usuario.IdPersonal, usuario.Nombres);
            if ( existe == true)
            {
                result = await _repo.InsertarUsuario(usuario);

            }
            else
            {
                result = false;
            }
            return result;
        }

        public async Task<List<UsuarioGrafica>> ObtenerCotizacionesUsuarios()
        {
            return await _repo.ObtenerCotizacionesUsuarios();
        }
        public async Task<List<UsuarioGraficaMensual>> ObtenerCotizacionesMensuales()
        {
            return await _repo.ObtenerCotizacionesMensuales();
        }
    }
}
