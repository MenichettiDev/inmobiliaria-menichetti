using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Configuration;

namespace InmobiliariaApp.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UsuarioRepository _usuarioRepo;
        private readonly IConfiguration _configuration;

        public UsuarioController(UsuarioRepository usuarioRepo, IConfiguration config)
        {
            _usuarioRepo = usuarioRepo;
            _configuration = config;
        }

        public IActionResult Listar()
        {
            var usuarios = _usuarioRepo.GetAll();
            return View(usuarios);
        }

        public IActionResult Detalles(int id)
        {
            var usuario = _usuarioRepo.GetById(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // GET: Usuario/Insertar
        public IActionResult Insertar()
        {
            return View();
        }

        // POST: Usuario/Insertar
        [HttpPost]
        public IActionResult Insertar(Usuario usuario)
        {
            try
            {
                // Validar el modelo
                if (!ModelState.IsValid)
                {
                    return View(usuario);
                }

                // Validar que la contraseña no sea nula o vacía
                if (string.IsNullOrWhiteSpace(usuario.Password))
                {
                    ModelState.AddModelError("Password", "La contraseña es obligatoria.");
                    return View(usuario);
                }

                // Salt fijo (a modo de aprendizaje)
                string salt = _configuration["Salt"]; // Lee el salt del archivo de configuración
                if (string.IsNullOrEmpty(salt))
                {
                    throw new InvalidOperationException("El valor de 'Salt' no está configurado en appsettings.json.");
                }

                // Hashear la contraseña usando el salt fijo
                string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: usuario.Password,
                    salt: Encoding.ASCII.GetBytes(salt),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

                // Asignar la contraseña hasheada al usuario
                usuario.Password = hashedPassword;

                // Guardar el usuario en la base de datos
                _usuarioRepo.Add(usuario);
                TempData["SuccessMessage"] = "Usuario cargado correctamente.";

                // Redirigir a la lista de usuarios
                return RedirectToAction("Listar");
            }
            catch (Exception ex)
            {
                // Loggear el error
                // _logger.LogError(ex, "Error al insertar un usuario.");
                ViewBag.MensajeError = "Ocurrió un error inesperado. Por favor, inténtalo más tarde." + ex.Message;
                return View(usuario);
            }
        }

        [Authorize]
        public IActionResult Editar(int? id)
        {
            int userId;

            if (id.HasValue)
            {
                userId = id.Value;
            }
            else
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (idClaim == null) return Unauthorized(); // o redirigir al login
                userId = int.Parse(idClaim.Value);
            }

            var usuario = _usuarioRepo.GetById(userId);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [HttpPost]
        public IActionResult Editar(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario) return BadRequest();

            if (ModelState.IsValid)
            {
                _usuarioRepo.Update(usuario);
                TempData["SuccessMessage"] = "Usuario modificado correctamente.";
                return RedirectToAction("Listar");
            }
            return View(usuario);
        }

        [Authorize(Policy = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var usuario = _usuarioRepo.GetById(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [HttpPost, ActionName("Eliminar")]
        [Authorize(Policy = "Administrador")]
        public IActionResult EliminarConfirmado(int id)
        {
            _usuarioRepo.Delete(id);
            TempData["SuccessMessage"] = "Usuario eliminado correctamente.";
            return RedirectToAction("Listar");
        }

        // Acción para mostrar el formulario de login
        public IActionResult Login()
        {
            return View();
        }

        // Acción para procesar el formulario de login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string nombreUsuario, string contrasenia)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(contrasenia))
            {
                ViewBag.MensajeError = "Nombre de usuario y contraseña son obligatorios.";
                return View();
            }
            try
            {
                // Buscar el usuario por email o nombre de usuario
                var usuario = _usuarioRepo.GetByEmail(nombreUsuario);

                // Hashear la contraseña que viene del form para compararla
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: contrasenia,
                    salt: Encoding.ASCII.GetBytes(_configuration["Salt"]), // importante: debe ser el mismo salt usado al guardar la contraseña
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

                if (usuario == null || usuario.Password != hashed)
                {
                    ViewBag.MensajeError = "Usuario o contraseña incorrectos.";
                    return View();
                }

                // Crear los claims (información que se guarda en la cookie)
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Email),
            // new Claim("FullName", usuario.Nombre + " " + usuario.Apellido),
            new Claim(ClaimTypes.Role, usuario.Rol), // importante si vas a usar roles
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString())

        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                );

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.MensajeError = "Error interno: " + ex.Message;
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Usuario");
        }


    }
}
