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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();