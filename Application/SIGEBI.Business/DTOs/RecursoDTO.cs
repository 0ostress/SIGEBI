namespace SIGEBI.Business.DTOs
{
    public class RecursoDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string ISBN { get; set; }
        public string Categoria { get; set; }
        public string Estado { get; set; }
        public int CantidadTotal { get; set; }
        public int CantidadDisponible { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string? ImagenUrl { get; set; }

        public string? Descripcion { get; set; }
    }
}