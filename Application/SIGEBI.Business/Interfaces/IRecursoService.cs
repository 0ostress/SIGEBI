using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces
{
    public interface IRecursoService
    {
        Task<IEnumerable<RecursoDTO>> ObtenerTodosAsync();
        Task<RecursoDTO?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<RecursoDTO>> ObtenerDisponiblesAsync();
        Task<RecursoDTO> RegistrarAsync(RecursoDTO recursoDto);
        Task<RecursoDTO> ActualizarAsync(RecursoDTO recursoDto);
        Task<bool> CambiarDisponibilidadAsync(int id, bool disponible);
        Task<bool> EliminarAsync(int id);
    }
}