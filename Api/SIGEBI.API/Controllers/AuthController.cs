using Microsoft.AspNetCore.Mvc;
using SIGEBI.API.Services;
using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Entities;
using SIGEBI.Persistence.Interfaces;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly TokenService _tokenService;

        public AuthController(IUsuarioRepository usuarioRepository, TokenService tokenService)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByEmailAsync(loginDto.Email);

                if (usuario == null)
                    return Unauthorized(new { mensaje = "Correo no encontrado." });

                if (usuario.Estado != "Activo")
                    return Unauthorized(new { mensaje = "Tu cuenta está suspendida o inactiva." });

                var token = _tokenService.GenerarToken(usuario);

                return Ok(new
                {
                    token,
                    nombre = $"{usuario.Nombre} {usuario.Apellido}",
                    email = usuario.Email,
                    rol = usuario.Rol
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registro([FromBody] RegisterDTO registerDto)
        {
            try
            {
                var existente = await _usuarioRepository.GetByEmailAsync(registerDto.Email);

                if (existente != null)
                    return BadRequest(new { mensaje = "Ya existe un usuario registrado con ese correo." });

                var nuevoUsuario = new Usuario
                {
                    Nombre = registerDto.Nombre,
                    Apellido = registerDto.Apellido,
                    Email = registerDto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    Rol = registerDto.Rol ?? "Estudiante",
                    Estado = "Activo",
                    FechaRegistro = DateTime.Now
                };

                await _usuarioRepository.AddAsync(nuevoUsuario);
                return Ok(new { mensaje = "Usuario registrado exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}