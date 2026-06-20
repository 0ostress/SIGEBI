using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Persistence.Context
{
    public class SigebiContext : DbContext
    {
        public SigebiContext(DbContextOptions<SigebiContext> options) : base(options)
        {
        }

        // Tablas de la base de datos
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<RecursoBibliografico> RecursosBibliograficos { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }
        public DbSet<Penalizacion> Penalizaciones { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuracion de tablas
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<RecursoBibliografico>().ToTable("RecursosBibliograficos");
            modelBuilder.Entity<Prestamo>().ToTable("Prestamos");
            modelBuilder.Entity<Penalizacion>().ToTable("Penalizaciones");
            modelBuilder.Entity<Notificacion>().ToTable("Notificaciones");
        }
    }
}