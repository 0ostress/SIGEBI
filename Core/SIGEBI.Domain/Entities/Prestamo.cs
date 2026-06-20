namespace SIGEBI.Domain.Entities
{
    public class Prestamo
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int RecursoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public string Estado { get; set; } 

        
        public Usuario Usuario { get; set; }
        public RecursoBibliografico Recurso { get; set; }
        public Penalizacion Penalizacion { get; set; }
    }
}