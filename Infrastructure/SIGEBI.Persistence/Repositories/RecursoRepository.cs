using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;
using SIGEBI.Persistence.Base;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Interfaces;

namespace SIGEBI.Persistence.Repositories
{
    public class RecursoRepository : BaseRepository<RecursoBibliografico>, IRecursoRepository
    {
        private readonly SigebiContext _context;

        public RecursoRepository(SigebiContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RecursoBibliografico>> GetAllAsync()
        {
            return await _context.RecursosBibliograficos.ToListAsync();
        }

        public async Task<RecursoBibliografico> GetByIdAsync(int id)
        {
            return await _context.RecursosBibliograficos.FindAsync(id);
        }

        public async Task<IEnumerable<RecursoBibliografico>> GetByDisponibilidadAsync()
        {
            return await _context.RecursosBibliograficos
                .Where(r => r.Estado == "Disponible")
                .ToListAsync();
        }

        public async Task AddAsync(RecursoBibliografico recurso)
        {
            await _context.RecursosBibliograficos.AddAsync(recurso);
            await _context.SaveChangesAsync();
        }

        public void Update(RecursoBibliografico recurso)
        {
            _context.RecursosBibliograficos.Update(recurso);
            _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var recurso = await _context.RecursosBibliograficos.FindAsync(id);
            if (recurso != null)
            {
                _context.RecursosBibliograficos.Remove(recurso);
                await _context.SaveChangesAsync();
            }
        }
    }
}