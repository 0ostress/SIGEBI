namespace SIGEBI.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Rol { get; set; } 
        public string Estado { get; set; } 
        public DateTime FechaRegistro { get; set; }

        public ICollection<Prestamo> Prestamos { get; set; }
        public ICollection<Penalizacion> Penalizaciones { get; set; }
        public ICollection<Notificacion> Notificaciones { get; set; }
    }
}