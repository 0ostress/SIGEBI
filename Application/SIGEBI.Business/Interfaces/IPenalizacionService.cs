using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces
{
    public interface IPenalizacionService
    {
        Task<IEnumerable<PenalizacionDTO>> ObtenerTodasAsync();
        Task<IEnumerable<PenalizacionDTO>> ObtenerActivasAsync();
        Task<IEnumerable<PenalizacionDTO>> ObtenerPorUsuarioAsync(int usuarioId);
        Task AplicarPenalizacionAsync(int prestamoId);
        Task ResolverPenalizacionAsync(int penalizacionId);
    }
}