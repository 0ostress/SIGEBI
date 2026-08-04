using Microsoft.EntityFrameworkCore;
using SIGEBI.IOC;
using SIGEBI.Persistence.Context;
using SIGEBI.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("SigebiAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7077/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddScoped<UsuarioApiService>();
builder.Services.AddScoped<PrestamoApiService>();
builder.Services.AddScoped<PenalizacionApiService>();
builder.Services.AddScoped<RecursoApiService>();

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
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
