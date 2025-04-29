using InmobiliariaApp.Data;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Servicio de autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>//el sitio web valida con cookie
	{
		options.LoginPath = "/Usuario/Login";
		options.LogoutPath = "/Usuario/Logout";
		options.AccessDeniedPath = "/Home/Restringido";
		//options.ExpireTimeSpan = TimeSpan.FromMinutes(5);//Tiempo de expiración
	});
// 	.AddJwtBearer(options =>//la api web valida con token
// 	{
// 		options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
// 		{
// 			ValidateIssuer = true,
// 			ValidateAudience = true,
// 			ValidateLifetime = true,
// 			ValidateIssuerSigningKey = true,
// 			ValidIssuer = configuration["TokenAuthentication:Issuer"],
// 			ValidAudience = configuration["TokenAuthentication:Audience"],
// 			IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(
// 				configuration["TokenAuthentication:SecretKey"])),
// 		};
// 		// opción extra para usar el token en el hub y otras peticiones sin encabezado (enlaces, src de img, etc.)
// 		options.Events = new JwtBearerEvents
// 		{
// 			OnMessageReceived = context =>
// 			{
// 				// Leer el token desde el query string
// 				var accessToken = context.Request.Query["access_token"];
// 				// Si el request es para el Hub u otra ruta seleccionada...
// 				var path = context.HttpContext.Request.Path;
// 				if (!string.IsNullOrEmpty(accessToken) &&
// 					(path.StartsWithSegments("/chatsegurohub") ||
// 					path.StartsWithSegments("/api/propietarios/reset") ||
// 					path.StartsWithSegments("/api/propietarios/token")))
// 				{//reemplazar las urls por las necesarias ruta ⬆
// 					context.Token = accessToken;
// 				}
// 				return Task.CompletedTask;
// 			},
// 			OnTokenValidated = context =>
// 			{
// 				// Este evento se activa cuando el token es validado correctamente
// 				Console.WriteLine("Token válido para el usuario: " + context?.Principal?.Identity?.Name);
// 				// Aquí puedes realizar otras validaciones o acciones si es necesario
// 				return Task.CompletedTask;
// 			},
// 			OnAuthenticationFailed = context =>
// 			{
// 				// Este evento se activa cuando la autenticación falla
// 				Console.WriteLine("Error en la autenticación del token: " + context.Exception.Message);
// 				return Task.CompletedTask;
// 			}
// 		};
// 	});

builder.Services.AddAuthorization(options =>
{
	//options.AddPolicy("Empleado", policy => policy.RequireClaim(ClaimTypes.Role, "Administrador", "Empleado"));
	options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
});

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
builder.Services.AddScoped<ImagenRepository>();



var app = builder.Build();

app.UseStaticFiles(); // Esta línea es necesaria para servir archivos estáticos como imágenes


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

// Habilitar autenticación
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
