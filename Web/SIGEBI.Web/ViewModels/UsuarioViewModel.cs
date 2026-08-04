namespace SIGEBI.Web.ViewModels
{
    public class UsuarioViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Rol { get; set; }
        public string Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}