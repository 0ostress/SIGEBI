using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.UseCases;
using SIGEBI.Persistence.Interfaces;
using SIGEBI.Persistence.Repositories;

namespace SIGEBI.IOC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSigebiServices(this IServiceCollection services)
        {
            // repositorios, capa de persistencia
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IPrestamoRepository, PrestamoRepository>();
            services.AddScoped<IPenalizacionRepository, PenalizacionRepository>();
            services.AddScoped<IRecursoRepository, RecursoRepository>();

            // servicios de aplicacion, la capa de business
            services.AddScoped<IUsuarioService, GestionarUsuarioUseCase>();
            services.AddScoped<IPrestamoService, SolicitarPrestamoUseCase>();
            services.AddScoped<IPenalizacionService, AplicarPenalizacionUseCase>();

            // casos de uso sin interfaz propia que se registran directamente
            services.AddScoped<RegistrarDevolucionUseCase>();

            return services;
        }
    }
}