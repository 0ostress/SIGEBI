namespace SGA.Domain.Entities.Configuration
{
    public class Ruta
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public string Estado { get; set; } = "Activo";
    }
}