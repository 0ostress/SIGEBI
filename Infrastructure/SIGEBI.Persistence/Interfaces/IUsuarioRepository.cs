using SIGEBI.Domain.Entities;

namespace SIGEBI.Persistence.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario> GetByIdAsync(int id);
        Task<Usuario> GetByEmailAsync(string email);
        Task AddAsync(Usuario usuario);
        void Update(Usuario usuario);
        Task DeleteAsync(int id);
    }
}