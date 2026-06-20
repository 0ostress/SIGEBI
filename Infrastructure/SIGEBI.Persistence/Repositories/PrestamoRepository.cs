using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;
using SIGEBI.Persistence.Base;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Interfaces;

namespace SIGEBI.Persistence.Repositories
{
    public class PrestamoRepository : BaseRepository<Prestamo>, IPrestamoRepository
    {
        private readonly SigebiContext _context;

        public PrestamoRepository(SigebiContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Prestamo>> GetAllAsync()
        {
            return await _context.Prestamos
                .Include(p => p.Usuario)
                .Include(p => p.Recurso)
                .ToListAsync();
        }

        public async Task<Prestamo> GetByIdAsync(int id)
        {
            return await _context.Prestamos
                .Include(p => p.Usuario)
                .Include(p => p.Recurso)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Prestamo>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Prestamos
                .Where(p => p.UsuarioId == usuarioId)
                .Include(p => p.Recurso)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prestamo>> GetVencidosAsync()
        {
            return await _context.Prestamos
                .Where(p => p.Estado == "Activo" && p.FechaVencimiento < DateTime.Now)
                .Include(p => p.Usuario)
                .Include(p => p.Recurso)
                .ToListAsync();
        }

        public async Task AddAsync(Prestamo prestamo)
        {
            await _context.Prestamos.AddAsync(prestamo);
            await _context.SaveChangesAsync();
        }

        public void Update(Prestamo prestamo)
        {
            _context.Prestamos.Update(prestamo);
            _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var prestamo = await _context.Prestamos.FindAsync(id);
            if (prestamo != null)
            {
                _context.Prestamos.Remove(prestamo);
                await _context.SaveChangesAsync();
            }
        }
    }
}