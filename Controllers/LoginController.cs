using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories;

namespace InmobiliariaApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly UsuarioRepository _usuarioRepository;

        public LoginController(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        // Acción para mostrar el formulario de login
        public IActionResult Index()
        {
            return View();
        }

        // Acción para procesar el formulario de login
        [HttpPost]
        public IActionResult Index(string nombreUsuario, string contrasenia)
        {
            // Buscar el usuario en la base de datos
            var usuario = _usuarioRepository.GetByNombreUsuario(nombreUsuario);

            if (usuario != null && usuario.Password == contrasenia)
            {
                // Autenticación exitosa
                HttpContext.Session.SetString("Usuario", usuario.NombreUsuario);
                HttpContext.Session.SetString("Rol", usuario.Rol);

                return RedirectToAction("Index", "Home"); // Redirigir al inicio
            }

            // Autenticación fallida
            ViewBag.MensajeError = "Nombre de usuario o contraseña incorrectos.";
            return View();
        }

        // Acción para cerrar sesión
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Limpiar la sesión
            return RedirectToAction("Index"); // Redirigir al login
        }
    }
}