using SIGEBI.Domain.Entities;

namespace SIGEBI.Persistence.Interfaces
{
    public interface INotificacionRepository
    {
        Task<IEnumerable<Notificacion>> GetByUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<Notificacion>> GetNoLeidasAsync(int usuarioId);
        Task AddAsync(Notificacion notificacion);
        Task MarcarComoLeidaAsync(int notificacionId);
    }
}