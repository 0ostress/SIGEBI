namespace SIGEBI.Web.ViewModels
{
    public class PrestamoViewModel
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public int RecursoId { get; set; }
        public string TituloRecurso { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public string Estado { get; set; }
        public bool EstaVencido => FechaVencimiento < DateTime.Now && Estado == "Activo";
    }
}