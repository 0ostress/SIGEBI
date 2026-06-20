using SIGEBI.Domain.Entities;

namespace SIGEBI.Persistence.Interfaces
{
    public interface IRecursoRepository
    {
        Task<IEnumerable<RecursoBibliografico>> GetAllAsync();
        Task<RecursoBibliografico> GetByIdAsync(int id);
        Task<IEnumerable<RecursoBibliografico>> GetByDisponibilidadAsync();
        Task AddAsync(RecursoBibliografico recurso);
        void Update(RecursoBibliografico recurso);
        Task DeleteAsync(int id);
    }
}