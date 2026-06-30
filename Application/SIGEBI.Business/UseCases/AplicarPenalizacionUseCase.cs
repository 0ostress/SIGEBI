using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Domain.Entities;
using SIGEBI.Persistence.Interfaces;

namespace SIGEBI.Business.UseCases
{
    public class AplicarPenalizacionUseCase : IPenalizacionService
    {
        private readonly IPenalizacionRepository _penalizacionRepository;
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        private const int DIAS_SUSPENSION_POR_VENCIMIENTO = 7;

        public AplicarPenalizacionUseCase(
            IPenalizacionRepository penalizacionRepository,
            IPrestamoRepository prestamoRepository,
            IUsuarioRepository usuarioRepository)
        {
            _penalizacionRepository = penalizacionRepository;
            _prestamoRepository = prestamoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<PenalizacionDTO>> ObtenerTodasAsync()
        {
            var penalizaciones = await _penalizacionRepository.GetAllAsync();
            return penalizaciones.Select(MapToDTO);
        }

        public async Task<IEnumerable<PenalizacionDTO>> ObtenerActivasAsync()
        {
            var penalizaciones = await _penalizacionRepository.GetActivasAsync();
            return penalizaciones.Select(MapToDTO);
        }

        public async Task<IEnumerable<PenalizacionDTO>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            var penalizaciones = await _penalizacionRepository.GetByUsuarioIdAsync(usuarioId);
            return penalizaciones.Select(MapToDTO);
        }

        public async Task AplicarPenalizacionAsync(int prestamoId)
        {
            // verificar que el prestamo exista y este vencido
            var prestamo = await _prestamoRepository.GetByIdAsync(prestamoId);

            if (prestamo == null)
                throw new Exception("Prestamo no encontrado.");

            if (prestamo.FechaVencimiento >= DateTime.Now)
                throw new Exception("El prestamo aun no esta vencido.");

            // marcar el prestamo como vencido
            prestamo.Estado = "Vencido";
            _prestamoRepository.Update(prestamo);

            // registramos la penalizacion
            var penalizacion = new Penalizacion
            {
                UsuarioId = prestamo.UsuarioId,
                PrestamoId = prestamo.Id,
                Tipo = "Suspension",
                Estado = "Activa",
                FechaAplicacion = DateTime.Now,
                FechaResolucion = DateTime.Now.AddDays(DIAS_SUSPENSION_POR_VENCIMIENTO),
                Descripcion = "Prestamo vencido sin devolucion registrada."
            };

            await _penalizacionRepository.AddAsync(penalizacion);

            // restringir al usuario para nuevos prestamos
            var usuario = await _usuarioRepository.GetByIdAsync(prestamo.UsuarioId);
            if (usuario != null)
            {
                usuario.Estado = "Suspendido";
                _usuarioRepository.Update(usuario);
            }
        }

        public async Task ResolverPenalizacionAsync(int penalizacionId)
        {
            var penalizacion = await _penalizacionRepository.GetByIdAsync(penalizacionId);

            if (penalizacion == null)
                throw new Exception("Penalizacion no encontrada.");

            penalizacion.Estado = "Resuelta";
            penalizacion.FechaResolucion = DateTime.Now;
            _penalizacionRepository.Update(penalizacion);

            // reactivamos al usuario si no tiene otras penalizaciones activas
            var penalizacionesActivas = await _penalizacionRepository.GetByUsuarioIdAsync(penalizacion.UsuarioId);
            bool tieneOtrasActivas = penalizacionesActivas.Any(p => p.Estado == "Activa" && p.Id != penalizacionId);

            if (!tieneOtrasActivas)
            {
                var usuario = await _usuarioRepository.GetByIdAsync(penalizacion.UsuarioId);
                if (usuario != null)
                {
                    usuario.Estado = "Activo";
                    _usuarioRepository.Update(usuario);
                }
            }
        }

        private PenalizacionDTO MapToDTO(Penalizacion p)
        {
            return new PenalizacionDTO
            {
                Id = p.Id,
                UsuarioId = p.UsuarioId,
                NombreUsuario = p.Usuario != null ? $"{p.Usuario.Nombre} {p.Usuario.Apellido}" : null,
                PrestamoId = p.PrestamoId,
                Tipo = p.Tipo,
                Estado = p.Estado,
                FechaAplicacion = p.FechaAplicacion,
                FechaResolucion = p.FechaResolucion,
                Descripcion = p.Descripcion
            };
        }
    }
}