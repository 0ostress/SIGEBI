namespace SIGEBI.Business.DTOs
{
    public class PenalizacionDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public int PrestamoId { get; set; }
        public string Tipo { get; set; }
        public string Estado { get; set; }
        public DateTime FechaAplicacion { get; set; }
        public DateTime FechaResolucion { get; set; }
        public string Descripcion { get; set; }
    }
}