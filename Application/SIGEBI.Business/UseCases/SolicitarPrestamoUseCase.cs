using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Domain.Entities;
using SIGEBI.Persistence.Interfaces;

namespace SIGEBI.Business.UseCases
{
    public class SolicitarPrestamoUseCase : IPrestamoService
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRecursoRepository _recursoRepository;

        private const int LIMITE_PRESTAMOS_SIMULTANEOS = 3;
        private const int DIAS_DURACION_PRESTAMO = 7;

        public SolicitarPrestamoUseCase(
            IPrestamoRepository prestamoRepository,
            IUsuarioRepository usuarioRepository,
            IRecursoRepository recursoRepository)
        {
            _prestamoRepository = prestamoRepository;
            _usuarioRepository = usuarioRepository;
            _recursoRepository = recursoRepository;
        }

        public async Task<IEnumerable<PrestamoDTO>> ObtenerTodosAsync()
        {
            var prestamos = await _prestamoRepository.GetAllAsync();
            return prestamos.Select(MapToDTO);
        }

        public async Task<PrestamoDTO> ObtenerPorIdAsync(int id)
        {
            var prestamo = await _prestamoRepository.GetByIdAsync(id);

            if (prestamo == null)
                throw new Exception("Prestamo no encontrado.");

            return MapToDTO(prestamo);
        }

        public async Task<IEnumerable<PrestamoDTO>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            var prestamos = await _prestamoRepository.GetByUsuarioIdAsync(usuarioId);
            return prestamos.Select(MapToDTO);
        }

        public async Task<PrestamoDTO> SolicitarPrestamoAsync(int usuarioId, int recursoId)
        {
            //  verificar estado del usuario 
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);

            if (usuario == null)
                throw new Exception("Usuario no encontrado.");

            if (usuario.Estado != "Activo")
                throw new Exception("El usuario no se encuentra activo. No puede solicitar prestamos.");

            // verificar limite de prestamos simultaneos 
            var prestamosActivos = await _prestamoRepository.GetByUsuarioIdAsync(usuarioId);
            var cantidadActivos = prestamosActivos.Count(p => p.Estado == "Activo");

            if (cantidadActivos >= LIMITE_PRESTAMOS_SIMULTANEOS)
                throw new Exception("El usuario alcanzo el limite de prestamos simultaneos.");

            // verificar disponibilidad de los ejemplares
            var recurso = await _recursoRepository.GetByIdAsync(recursoId);

            if (recurso == null)
                throw new Exception("Recurso no encontrado.");

            if (recurso.CantidadDisponible <= 0)
                throw new Exception("El recurso no esta disponible actualmente.");

            // registrar el prestamo
            var nuevoPrestamo = new Prestamo
            {
                UsuarioId = usuarioId,
                RecursoId = recursoId,
                FechaInicio = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddDays(DIAS_DURACION_PRESTAMO),
                Estado = "Activo"
            };

            await _prestamoRepository.AddAsync(nuevoPrestamo);

            // actualizamos la disponibilidad de los ejemplares o el ejemplar
            recurso.CantidadDisponible -= 1;
            if (recurso.CantidadDisponible == 0)
                recurso.Estado = "Prestado";

            _recursoRepository.Update(recurso);

            return MapToDTO(nuevoPrestamo);
        }

        public async Task RegistrarDevolucionAsync(int prestamoId)
        {
            throw new NotImplementedException("Este metodo se implementa en RegistrarDevolucionUseCase.");
        }

        private PrestamoDTO MapToDTO(Prestamo p)
        {
            return new PrestamoDTO
            {
                Id = p.Id,
                UsuarioId = p.UsuarioId,
                NombreUsuario = p.Usuario != null ? $"{p.Usuario.Nombre} {p.Usuario.Apellido}" : null,
                RecursoId = p.RecursoId,
                TituloRecurso = p.Recurso != null ? p.Recurso.Titulo : null,
                FechaInicio = p.FechaInicio,
                FechaVencimiento = p.FechaVencimiento,
                FechaDevolucion = p.FechaDevolucion,
                Estado = p.Estado
            };
        }
    }
}