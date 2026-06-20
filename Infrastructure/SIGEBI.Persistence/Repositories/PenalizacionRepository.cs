using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;
using SIGEBI.Persistence.Base;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Interfaces;

namespace SIGEBI.Persistence.Repositories
{
    public class PenalizacionRepository : BaseRepository<Penalizacion>, IPenalizacionRepository
    {
        private readonly SigebiContext _context;

        public PenalizacionRepository(SigebiContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Penalizacion>> GetAllAsync()
        {
            return await _context.Penalizaciones
                .Include(p => p.Usuario)
                .Include(p => p.Prestamo)
                .ToListAsync();
        }

        public async Task<Penalizacion> GetByIdAsync(int id)
        {
            return await _context.Penalizaciones
                .Include(p => p.Usuario)
                .Include(p => p.Prestamo)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Penalizacion>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Penalizaciones
                .Where(p => p.UsuarioId == usuarioId)
                .Include(p => p.Prestamo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Penalizacion>> GetActivasAsync()
        {
            return await _context.Penalizaciones
                .Where(p => p.Estado == "Activa")
                .Include(p => p.Usuario)
                .ToListAsync();
        }

        public async Task AddAsync(Penalizacion penalizacion)
        {
            await _context.Penalizaciones.AddAsync(penalizacion);
            await _context.SaveChangesAsync();
        }

        public void Update(Penalizacion penalizacion)
        {
            _context.Penalizaciones.Update(penalizacion);
            _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var penalizacion = await _context.Penalizaciones.FindAsync(id);
            if (penalizacion != null)
            {
                _context.Penalizaciones.Remove(penalizacion);
                await _context.SaveChangesAsync();
            }
        }
    }
}