using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Domain.Entities;
using SIGEBI.Persistence.Interfaces;

namespace SIGEBI.Business.UseCases
{
    public class GestionarRecursoUseCase : IRecursoService
    {
        private readonly IRecursoRepository _recursoRepository;

        public GestionarRecursoUseCase(IRecursoRepository recursoRepository)
        {
            _recursoRepository = recursoRepository;
        }

        public async Task<IEnumerable<RecursoDTO>> ObtenerTodosAsync()
        {
            var recursos = await _recursoRepository.GetAllAsync();
            return recursos.Select(MapToDto);
        }

        public async Task<RecursoDTO?> ObtenerPorIdAsync(int id)
        {
            var recurso = await _recursoRepository.GetByIdAsync(id);
            return recurso is null ? null : MapToDto(recurso);
        }

        public async Task<IEnumerable<RecursoDTO>> ObtenerDisponiblesAsync()
        {
            var recursos = await _recursoRepository.GetByDisponibilidadAsync();
            return recursos.Select(MapToDto);
        }

        public async Task<RecursoDTO> RegistrarAsync(RecursoDTO recursoDto)
        {
            if (string.IsNullOrWhiteSpace(recursoDto.Titulo))
                throw new ArgumentException("El título del recurso es obligatorio.");

            if (recursoDto.CantidadTotal < 0)
                throw new ArgumentException("La cantidad total no puede ser negativa.");

            var recurso = new RecursoBibliografico
            {
                Titulo = recursoDto.Titulo,
                Autor = recursoDto.Autor,
                ISBN = recursoDto.ISBN,
                Categoria = recursoDto.Categoria,
                Estado = string.IsNullOrWhiteSpace(recursoDto.Estado) ? "Disponible" : recursoDto.Estado,
                CantidadTotal = recursoDto.CantidadTotal,
                CantidadDisponible = recursoDto.CantidadDisponible == 0
                    ? recursoDto.CantidadTotal
                    : recursoDto.CantidadDisponible,
                FechaRegistro = DateTime.UtcNow,
                Descripcion = recursoDto.Descripcion
            };

            await _recursoRepository.AddAsync(recurso);
            return MapToDto(recurso);
        }

        public async Task<RecursoDTO> ActualizarAsync(RecursoDTO recursoDto)
        {
            var recursoExistente = await _recursoRepository.GetByIdAsync(recursoDto.Id)
                ?? throw new KeyNotFoundException($"No se encontró el recurso con id {recursoDto.Id}.");

            recursoExistente.Titulo = recursoDto.Titulo;
            recursoExistente.Autor = recursoDto.Autor;
            recursoExistente.ISBN = recursoDto.ISBN;
            recursoExistente.Categoria = recursoDto.Categoria;
            recursoExistente.Estado = recursoDto.Estado;
            recursoExistente.CantidadTotal = recursoDto.CantidadTotal;
            recursoExistente.CantidadDisponible = recursoDto.CantidadDisponible;
            recursoExistente.Descripcion = recursoDto.Descripcion;

            if (!string.IsNullOrEmpty(recursoDto.ImagenUrl))
                recursoExistente.ImagenUrl = recursoDto.ImagenUrl;

            _recursoRepository.Update(recursoExistente);
            return MapToDto(recursoExistente);
        }

        public async Task<bool> CambiarDisponibilidadAsync(int id, bool disponible)
        {
            var recurso = await _recursoRepository.GetByIdAsync(id);
            if (recurso is null) return false;

            recurso.Estado = disponible ? "Disponible" : "No disponible";
            _recursoRepository.Update(recurso);
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var recurso = await _recursoRepository.GetByIdAsync(id);
            if (recurso is null) return false;

            // Verificar que no tenga prestamos activos
            var prestamosActivos = recurso.Prestamos?.Any(p => p.Estado == "Activo" || p.Estado == "Pendiente");
            if (prestamosActivos == true)
                throw new Exception("No se puede eliminar un recurso con prestamos activos o pendientes.");

            await _recursoRepository.DeleteAsync(id);
            return true;
        }

        private static RecursoDTO MapToDto(RecursoBibliografico recurso)
        {
            return new RecursoDTO
            {
                Id = recurso.Id,
                Titulo = recurso.Titulo,
                Autor = recurso.Autor,
                ISBN = recurso.ISBN,
                Categoria = recurso.Categoria,
                Estado = recurso.Estado,
                CantidadTotal = recurso.CantidadTotal,
                CantidadDisponible = recurso.CantidadDisponible,
                FechaRegistro = recurso.FechaRegistro,
                ImagenUrl = recurso.ImagenUrl,
                Descripcion = recurso.Descripcion
            };
        }
    }
}