using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Persistence.Context
{
    public class SigebiContext : DbContext
    {
        public SigebiContext(DbContextOptions<SigebiContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<RecursoBibliografico> RecursosBibliograficos { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }
        public DbSet<Penalizacion> Penalizaciones { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Resena> Resenas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<RecursoBibliografico>().ToTable("RecursosBibliograficos");
            modelBuilder.Entity<Prestamo>().ToTable("Prestamos");
            modelBuilder.Entity<Penalizacion>().ToTable("Penalizaciones");
            modelBuilder.Entity<Notificacion>().ToTable("Notificaciones");

            // Prestamo -> Usuario (sin cascada)
            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Prestamos)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prestamo -> Recurso (sin cascada)
            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Recurso)
                .WithMany(r => r.Prestamos)
                .HasForeignKey(p => p.RecursoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Penalizacion -> Usuario (sin cascada)
            modelBuilder.Entity<Penalizacion>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Penalizaciones)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Penalizacion -> Prestamo (sin cascada)
            modelBuilder.Entity<Penalizacion>()
                .HasOne(p => p.Prestamo)
                .WithOne(p => p.Penalizacion)
                .HasForeignKey<Penalizacion>(p => p.PrestamoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Notificacion -> Usuario (sin cascada)
            modelBuilder.Entity<Notificacion>()
                .HasOne(n => n.Usuario)
                .WithMany(u => u.Notificaciones)
                .HasForeignKey(n => n.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Resena>().ToTable("Resenas");

            modelBuilder.Entity<Resena>()
                .HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Resena>()
                .HasOne(r => r.Recurso)
                .WithMany()
                .HasForeignKey(r => r.RecursoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}