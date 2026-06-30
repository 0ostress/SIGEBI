using SIGEBI.Domain.Entities;
using SIGEBI.Persistence.Interfaces;

namespace SIGEBI.Business.UseCases
{
    public class RegistrarDevolucionUseCase
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IRecursoRepository _recursoRepository;
        private readonly IPenalizacionRepository _penalizacionRepository;

        private const int DIAS_SUSPENSION_POR_RETRASO = 5;

        public RegistrarDevolucionUseCase(
            IPrestamoRepository prestamoRepository,
            IRecursoRepository recursoRepository,
            IPenalizacionRepository penalizacionRepository)
        {
            _prestamoRepository = prestamoRepository;
            _recursoRepository = recursoRepository;
            _penalizacionRepository = penalizacionRepository;
        }

        public async Task EjecutarAsync(int prestamoId)
        {
            // localizamos el prestamo activo
            var prestamo = await _prestamoRepository.GetByIdAsync(prestamoId);

            if (prestamo == null)
                throw new Exception("Prestamo no encontrado.");

            if (prestamo.Estado != "Activo")
                throw new Exception("El prestamo no se encuentra activo.");

            // cerrar el prestamo
            prestamo.FechaDevolucion = DateTime.Now;
            prestamo.Estado = "Devuelto";
            _prestamoRepository.Update(prestamo);

            // actualizar disponibilidad del ejemplar
            var recurso = await _recursoRepository.GetByIdAsync(prestamo.RecursoId);

            if (recurso != null)
            {
                recurso.CantidadDisponible += 1;
                recurso.Estado = "Disponible";
                _recursoRepository.Update(recurso);
            }

            // evaluamos si la devolucion es tardia
            bool esDevolucionTardia = prestamo.FechaDevolucion > prestamo.FechaVencimiento;

            if (esDevolucionTardia)
            {
                var penalizacion = new Penalizacion
                {
                    UsuarioId = prestamo.UsuarioId,
                    PrestamoId = prestamo.Id,
                    Tipo = "Suspension",
                    Estado = "Activa",
                    FechaAplicacion = DateTime.Now,
                    FechaResolucion = DateTime.Now.AddDays(DIAS_SUSPENSION_POR_RETRASO),
                    Descripcion = "Devolucion tardia del libro prestado."
                };

                await _penalizacionRepository.AddAsync(penalizacion);
            }
        }
    }
}