using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces
{
    public interface IRecursoService
    {
        Task<IEnumerable<RecursoDTO>> ObtenerTodosAsync();
        Task<IEnumerable<RecursoDTO>> ObtenerDisponiblesAsync();
        Task<RecursoDTO?> ObtenerPorIdAsync(int id);
        Task<RecursoDTO> RegistrarAsync(RecursoDTO recursoDto);
        Task<RecursoDTO> ActualizarAsync(RecursoDTO recursoDto);
        Task<bool> EliminarAsync(int id);
        Task<bool> CambiarDisponibilidadAsync(int id, bool disponible);
    }
}