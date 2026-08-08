namespace SIGEBI.Domain.Entities
{
    public class Resena
    {
        public int Id { get; set; }
        public int RecursoId { get; set; }
        public int UsuarioId { get; set; }
        public int Estrellas { get; set; }
        public string Comentario { get; set; }
        public DateTime FechaCreacion { get; set; }

        // Relaciones
        public RecursoBibliografico Recurso { get; set; }
        public Usuario Usuario { get; set; }
    }
}