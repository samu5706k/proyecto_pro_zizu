
using gimnasio.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar el repositorio de entrenadores.
builder.Services.AddSingleton<EntrenadorRepositorio>();

// Registra el repositorio de Cliente
builder.Services.AddSingleton<ClienteRepositorio>();

// Registrar el repositorio de Administradores
builder.Services.AddSingleton<AdministradorRepositorio>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
