using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces
{
    public interface IPrestamoService
    {
        Task<IEnumerable<PrestamoDTO>> ObtenerTodosAsync();
        Task<PrestamoDTO> ObtenerPorIdAsync(int id);
        Task<IEnumerable<PrestamoDTO>> ObtenerPorUsuarioAsync(int usuarioId);
        Task<PrestamoDTO> SolicitarPrestamoAsync(int usuarioId, int recursoId);
        Task RegistrarDevolucionAsync(int prestamoId);
        Task<bool> AprobarPrestamoAsync(int prestamoId, DateTime fechaVencimiento);

        Task NotificarVencimientosProximosAsync();
    }
}