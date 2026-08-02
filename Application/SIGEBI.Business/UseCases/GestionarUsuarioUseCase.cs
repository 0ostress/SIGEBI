using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Domain.Entities;
using SIGEBI.Persistence.Interfaces;

namespace SIGEBI.Business.UseCases
{
    public class GestionarUsuarioUseCase : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public GestionarUsuarioUseCase(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<UsuarioDTO>> ObtenerTodosAsync()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();

            return usuarios.Select(u => new UsuarioDTO
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email,
                Rol = u.Rol,
                Estado = u.Estado,
                FechaRegistro = u.FechaRegistro
            });
        }

        public async Task<UsuarioDTO> ObtenerPorIdAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);

            if (usuario == null)
                throw new Exception("Usuario no encontrado.");

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Rol = usuario.Rol,
                Estado = usuario.Estado,
                FechaRegistro = usuario.FechaRegistro
            };
        }

        public async Task<UsuarioDTO> RegistrarAsync(UsuarioDTO usuarioDto)
        {
            var existente = await _usuarioRepository.GetByEmailAsync(usuarioDto.Email);

            if (existente != null)
                throw new Exception("Ya existe un usuario registrado con ese correo electronico.");

            var nuevoUsuario = new Usuario
            {
                Nombre = usuarioDto.Nombre,
                Apellido = usuarioDto.Apellido,
                Email = usuarioDto.Email,
                PasswordHash = usuarioDto.PasswordHash,
                Rol = usuarioDto.Rol,
                Estado = "Activo",
                FechaRegistro = DateTime.Now
            };

            await _usuarioRepository.AddAsync(nuevoUsuario);

            usuarioDto.Id = nuevoUsuario.Id;
            usuarioDto.Estado = nuevoUsuario.Estado;
            usuarioDto.FechaRegistro = nuevoUsuario.FechaRegistro;

            return usuarioDto;
        }

        public async Task ActualizarAsync(UsuarioDTO usuarioDto)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioDto.Id);

            if (usuario == null)
                throw new Exception("Usuario no encontrado.");

            usuario.Nombre = usuarioDto.Nombre;
            usuario.Apellido = usuarioDto.Apellido;
            usuario.Email = usuarioDto.Email;
            usuario.Rol = usuarioDto.Rol;

            _usuarioRepository.Update(usuario);
        }

        public async Task CambiarEstadoAsync(int id, string nuevoEstado)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);

            if (usuario == null)
                throw new Exception("Usuario no encontrado.");

            usuario.Estado = nuevoEstado;

            _usuarioRepository.Update(usuario);
        }
    }
}