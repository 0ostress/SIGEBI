using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDTO>> ObtenerTodosAsync();
        Task<UsuarioDTO> ObtenerPorIdAsync(int id);
        Task<UsuarioDTO> RegistrarAsync(UsuarioDTO usuarioDto);
        Task ActualizarAsync(UsuarioDTO usuarioDto);
        Task CambiarEstadoAsync(int id, string nuevoEstado);
        Task EliminarAsync(int id);
    }
}