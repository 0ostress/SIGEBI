using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SIGEBI.API.Services;
using SIGEBI.IOC;
using SIGEBI.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);

// === Controladores ===
builder.Services.AddControllers();
builder.Services.AddScoped<TokenService>();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

// === Swagger ===
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SIGEBI API", Version = "v1" });
});

// === Base de datos ===
builder.Services.AddDbContext<SigebiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// === Servicios y Repositorios (IOC) ===
builder.Services.AddSigebiServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SIGEBI API v1");
    });
}

// Seed: crear Admin por defecto si no existe
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SigebiContext>();
    if (!context.Usuarios.Any(u => u.Rol == "Administrador"))
    {
        context.Usuarios.Add(new SIGEBI.Domain.Entities.Usuario
        {
            Nombre = "Admin",
            Apellido = "SIGEBI",
            Email = "admin@sigebi.edu.do",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123*"),
            Rol = "Administrador",
            Estado = "Activo",
            FechaRegistro = DateTime.Now
        });
        context.SaveChanges();
    }
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();