namespace SIGEBI.Business.DTOs
{
    public class RecursoDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int CantidadTotal { get; set; }
        public int CantidadDisponible { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}