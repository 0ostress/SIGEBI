using SIGEBI.Domain.Entities;

namespace SIGEBI.Persistence.Interfaces
{
    public interface IPenalizacionRepository
    {
        Task<IEnumerable<Penalizacion>> GetAllAsync();
        Task<Penalizacion> GetByIdAsync(int id);
        Task<IEnumerable<Penalizacion>> GetByUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<Penalizacion>> GetActivasAsync();
        Task AddAsync(Penalizacion penalizacion);
        void Update(Penalizacion penalizacion);
        Task DeleteAsync(int id);
    }
}