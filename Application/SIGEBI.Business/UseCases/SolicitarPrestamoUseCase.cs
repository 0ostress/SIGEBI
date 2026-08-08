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
        private readonly INotificacionRepository _notificacionRepository;

        private const int LIMITE_PRESTAMOS_SIMULTANEOS = 3;
        private const int DIAS_DURACION_PRESTAMO = 7;

        public SolicitarPrestamoUseCase(
            IPrestamoRepository prestamoRepository,
            IUsuarioRepository usuarioRepository,
            IRecursoRepository recursoRepository,
            INotificacionRepository notificacionRepository)
        {
            _prestamoRepository = prestamoRepository;
            _usuarioRepository = usuarioRepository;
            _recursoRepository = recursoRepository;
            _notificacionRepository = notificacionRepository;
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
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
                throw new Exception("Usuario no encontrado.");
            if (usuario.Estado != "Activo")
                throw new Exception("El usuario no se encuentra activo.");

            var prestamosActivos = await _prestamoRepository.GetByUsuarioIdAsync(usuarioId);
            var cantidadActivos = prestamosActivos.Count(p => p.Estado == "Activo");
            if (cantidadActivos >= LIMITE_PRESTAMOS_SIMULTANEOS)
                throw new Exception("El usuario alcanzo el limite de prestamos simultaneos.");

            var recurso = await _recursoRepository.GetByIdAsync(recursoId);
            if (recurso == null)
                throw new Exception("Recurso no encontrado.");
            if (recurso.CantidadDisponible <= 0)
                throw new Exception("El recurso no esta disponible actualmente.");

            var nuevoPrestamo = new Prestamo
            {
                UsuarioId = usuarioId,
                RecursoId = recursoId,
                FechaInicio = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddDays(DIAS_DURACION_PRESTAMO),
                Estado = "Pendiente"
            };

            // Reservar el ejemplar mientras esta pendiente
            recurso.CantidadDisponible -= 1;
            if (recurso.CantidadDisponible == 0)
                recurso.Estado = "Prestado";
            _recursoRepository.Update(recurso);

            // Notificar a todos los bibliotecarios y admins
            var todosUsuarios = await _usuarioRepository.GetAllAsync();
            var bibliotecarios = todosUsuarios.Where(u => u.Rol == "Bibliotecario" || u.Rol == "Administrador");

            foreach (var bibliotecario in bibliotecarios)
            {
                var notificacionBibliotecario = new Notificacion
                {
                    UsuarioId = bibliotecario.Id,
                    Tipo = "SolicitudPrestamo",
                    Mensaje = $"El usuario {usuario.Nombre} {usuario.Apellido} ha solicitado el prestamo del libro '{recurso.Titulo}'. Fecha de solicitud: {DateTime.Now.ToShortDateString()}.",
                    Leida = false,
                    FechaCreacion = DateTime.Now
                };
                await _notificacionRepository.AddAsync(notificacionBibliotecario);
            }
            return MapToDTO(nuevoPrestamo);
        }

        public async Task<bool> AprobarPrestamoAsync(int prestamoId, DateTime fechaVencimiento)
        {
            var prestamo = await _prestamoRepository.GetByIdAsync(prestamoId);
            if (prestamo == null || prestamo.Estado != "Pendiente")
                return false;

            var recurso = await _recursoRepository.GetByIdAsync(prestamo.RecursoId);
            if (recurso == null || recurso.CantidadDisponible <= 0)
                return false;

            prestamo.Estado = "Activo";
            prestamo.FechaVencimiento = fechaVencimiento;
            _prestamoRepository.Update(prestamo);

            recurso.CantidadDisponible -= 1;
            if (recurso.CantidadDisponible == 0)
                recurso.Estado = "Prestado";
            _recursoRepository.Update(recurso);

            // Crear notificacion para el usuario
            var notificacion = new Notificacion
            {
                UsuarioId = prestamo.UsuarioId,
                Tipo = "PrestamoAprobado",
                Mensaje = $"Tu solicitud de prestamo del libro '{recurso.Titulo}' ha sido aprobada. Debes devolverlo antes del {fechaVencimiento.ToShortDateString()}.",
                Leida = false,
                FechaCreacion = DateTime.Now
            };

            await _notificacionRepository.AddAsync(notificacion);
            return true;
        }

        public async Task RegistrarDevolucionAsync(int prestamoId)
        {
            throw new NotImplementedException("Este metodo se implementa en RegistrarDevolucionUseCase.");
        }

        public async Task NotificarVencimientosProximosAsync()
        {
            var todosPrestamos = await _prestamoRepository.GetAllAsync();
            var proximos = todosPrestamos.Where(p =>
                p.Estado == "Activo" &&
                p.FechaVencimiento.Date == DateTime.Now.AddDays(2).Date);

            foreach (var prestamo in proximos)
            {
                var recurso = await _recursoRepository.GetByIdAsync(prestamo.RecursoId);

                // Notificar al estudiante
                var notificacionEstudiante = new Notificacion
                {
                    UsuarioId = prestamo.UsuarioId,
                    Tipo = "VencimientoProximo",
                    Mensaje = $"Tu prestamo del libro '{recurso?.Titulo}' vence en 2 dias, el {prestamo.FechaVencimiento.ToShortDateString()}. Por favor devuelvelo a tiempo para evitar penalizaciones.",
                    Leida = false,
                    FechaCreacion = DateTime.Now
                };
                await _notificacionRepository.AddAsync(notificacionEstudiante);

                // Notificar a bibliotecarios y admins
                var todosUsuarios = await _usuarioRepository.GetAllAsync();
                var bibliotecarios = todosUsuarios.Where(u => u.Rol == "Bibliotecario" || u.Rol == "Administrador");
                foreach (var bibliotecario in bibliotecarios)
                {
                    var notificacionBibliotecario = new Notificacion
                    {
                        UsuarioId = bibliotecario.Id,
                        Tipo = "VencimientoProximo",
                        Mensaje = $"El prestamo del libro '{recurso?.Titulo}' del usuario {prestamo.UsuarioId} vence en 2 dias, el {prestamo.FechaVencimiento.ToShortDateString()}.",
                        Leida = false,
                        FechaCreacion = DateTime.Now
                    };
                    await _notificacionRepository.AddAsync(notificacionBibliotecario);
                }
            }
        }

        public async Task<bool> RechazarPrestamoAsync(int prestamoId, string motivo)
        {
            var prestamo = await _prestamoRepository.GetByIdAsync(prestamoId);

            if (prestamo == null || prestamo.Estado != "Pendiente")
                return false;

            prestamo.Estado = "Rechazado";
            _prestamoRepository.Update(prestamo);

            // Devolver el ejemplar reservado
            var recurso = await _recursoRepository.GetByIdAsync(prestamo.RecursoId);
            if (recurso != null)
            {
                recurso.CantidadDisponible += 1;
                if (recurso.Estado == "Prestado")
                    recurso.Estado = "Disponible";
                _recursoRepository.Update(recurso);
            }

            // Notificar al usuario
            var notificacion = new Notificacion
            {
                UsuarioId = prestamo.UsuarioId,
                Tipo = "PrestamoRechazado",
                Mensaje = $"Tu solicitud de prestamo del libro '{recurso?.Titulo}' fue rechazada. Motivo: {motivo}.",
                Leida = false,
                FechaCreacion = DateTime.Now
            };

            await _notificacionRepository.AddAsync(notificacion);
            return true;
        }

        public async Task<bool> RenovarPrestamoAsync(int prestamoId, int usuarioId)
        {
            var prestamo = await _prestamoRepository.GetByIdAsync(prestamoId);

            if (prestamo == null || prestamo.Estado != "Activo")
                return false;

            if (prestamo.UsuarioId != usuarioId)
                return false;

            if (prestamo.FechaVencimiento < DateTime.Now)
                return false;

            // Limite de 1 renovacion por prestamo
            if (prestamo.CantidadRenovaciones >= 1)
                throw new Exception("Este prestamo ya fue renovado una vez. No se permiten mas renovaciones.");

            prestamo.FechaVencimiento = prestamo.FechaVencimiento.AddDays(7);
            prestamo.CantidadRenovaciones += 1;
            _prestamoRepository.Update(prestamo);

            var recurso = await _recursoRepository.GetByIdAsync(prestamo.RecursoId);
            var notificacion = new Notificacion
            {
                UsuarioId = usuarioId,
                Tipo = "PrestamoRenovado",
                Mensaje = $"Tu prestamo del libro '{recurso?.Titulo}' ha sido renovado exitosamente. Nueva fecha de vencimiento: {prestamo.FechaVencimiento.ToShortDateString()}. Recuerda que solo puedes renovar una vez por prestamo.",
                Leida = false,
                FechaCreacion = DateTime.Now
            };
            await _notificacionRepository.AddAsync(notificacion);

            return true;
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
                Estado = p.Estado,
                CantidadRenovaciones = p.CantidadRenovaciones
            };
        }
    }
}