using InmobiliariaApp.Data;
using InmobiliariaApp.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar la clase de conexión a la base de datos
builder.Services.AddSingleton<DatabaseConnection>();

//Registramos los repositorios
builder.Services.AddScoped<PropietarioRepository>();
builder.Services.AddScoped<InmuebleRepository>();
builder.Services.AddScoped<InquilinoRepository>();
builder.Services.AddScoped<ContratoRepository>();
builder.Services.AddScoped<PagoRepository>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<TipoInmuebleRepository>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddSession(); // Agregar el servicio de sesión



var app = builder.Build();


//Middleware de sesiones
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
