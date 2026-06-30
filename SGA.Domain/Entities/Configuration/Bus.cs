namespace SGA.Domain.Entities.Configuration
{
    public class Bus
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public string Estado { get; set; } = "Activo";
    }
}
