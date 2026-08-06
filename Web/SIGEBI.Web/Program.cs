using Microsoft.EntityFrameworkCore;
using SIGEBI.IOC;
using SIGEBI.Persistence.Context;
using SIGEBI.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<AuthApiService>();

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("SigebiAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5200/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

builder.Services.AddScoped<UsuarioApiService>();
builder.Services.AddScoped<PrestamoApiService>();
builder.Services.AddScoped<PenalizacionApiService>();
builder.Services.AddScoped<RecursoApiService>();
builder.Services.AddScoped<AuthApiService>();
builder.Services.AddScoped<NotificacionApiService>();
builder.Services.AddScoped<ReporteApiService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<SigebiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddSigebiServices();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower();
    var publicPaths = new[] { "/auth/login", "/auth/registro" };

    if (publicPaths.Contains(path))
    {
        await next();
        return;
    }

    var token = context.Session.GetString("Token");
    if (token == null)
    {
        context.Response.Redirect("/Auth/Login");
        return;
    }

    var rol = context.Session.GetString("Rol");

    // Rutas solo para Admin
    var rutasSoloAdmin = new[] { "/usuarios" };
    if (rol != "Administrador" && rutasSoloAdmin.Any(r => path.StartsWith(r)))
    {
        context.Response.Redirect("/Home/Index");
        return;
    }

    // Rutas restringidas para Estudiante y Docente
    var rutasRestringidas = new[] { "/devoluciones", "/prestamos/pendientes", "/prestamos/index", "/penalizaciones/index", "/penalizaciones/activas" };
    if ((rol == "Estudiante" || rol == "Docente") && rutasRestringidas.Any(r => path.StartsWith(r)))
    {
        context.Response.Redirect("/Home/Index");
        return;
    }

    await next();


});




app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
