using SIGEBI.Domain.Entities;

namespace SIGEBI.Persistence.Interfaces
{
    public interface IPrestamoRepository
    {
        Task<IEnumerable<Prestamo>> GetAllAsync();
        Task<Prestamo> GetByIdAsync(int id);
        Task<IEnumerable<Prestamo>> GetByUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<Prestamo>> GetVencidosAsync();
        Task AddAsync(Prestamo prestamo);
        void Update(Prestamo prestamo);
        Task DeleteAsync(int id);
    }
}