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

            // Pasamos el userId al ViewBag
            ViewBag.UserId = userId;

            return View(usuario);
        }

        //Editar validando cambio de contraseña
        [HttpPost]
        public IActionResult Editar(Usuario usuario, string PasswordActual, string NuevaPassword, string ConfirmarPassword)
        {

            try
            {
                var usuarioExistente = _usuarioRepo.GetById(usuario.IdUsuario);
                if (usuarioExistente == null)
                {
                    ViewBag.ErrorMessage = "Usuario no encontrado.";
                    return View(usuario);
                }

                usuarioExistente.Email = usuario.Email;
                usuarioExistente.Rol = usuario.Rol;

                // Solo si quiere cambiar la contraseña
                if (!string.IsNullOrEmpty(PasswordActual) || !string.IsNullOrEmpty(NuevaPassword) || !string.IsNullOrEmpty(ConfirmarPassword))
                {
                    // Hasheamos la contraseña actual para compararla
                    string hashedActual = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: PasswordActual,
                        salt: Encoding.ASCII.GetBytes(_configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 10000,
                        numBytesRequested: 256 / 8));

                    if (hashedActual != usuarioExistente.Password)
                    {
                        ViewBag.ErrorMessage = "La contraseña actual es incorrecta.";
                        return View(usuario);
                    }

                    if (NuevaPassword != ConfirmarPassword)
                    {
                        ViewBag.ErrorMessage = "La nueva contraseña y la confirmación no coinciden.";
                        return View(usuario);
                    }

                    // Hasheamos la nueva contraseña antes de guardarla
                    string hashedNueva = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: NuevaPassword,
                        salt: Encoding.ASCII.GetBytes(_configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 10000,
                        numBytesRequested: 256 / 8));

                    usuarioExistente.Password = hashedNueva;
                }

                usuario = usuarioExistente;
                _usuarioRepo.Update(usuarioExistente);

                TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
                return RedirectToAction("Listar");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error al editar el Usuario: {ex.Message}";

                return View("Editar", usuario);
            }

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

        [HttpPost]
        public async Task<IActionResult> ActualizarFoto(int IdUsuario, IFormFile FotoPerfilFile, [FromServices] IWebHostEnvironment environment)
        {
            try
            {
                if (FotoPerfilFile != null && FotoPerfilFile.Length > 0)
                {
                    var usuario = _usuarioRepo.GetById(IdUsuario);

                    if (usuario == null)
                    {
                        return Json(new { success = false, message = "Usuario no encontrado." });
                    }

                    // Borrar foto anterior si existe
                    if (!string.IsNullOrEmpty(usuario.FotoPerfil))
                    {
                        var rutaFoto = Path.Combine(environment.WebRootPath, usuario.FotoPerfil.TrimStart('/'));
                        if (System.IO.File.Exists(rutaFoto))
                        {
                            System.IO.File.Delete(rutaFoto);
                        }
                    }

                    var uploadsPath = Path.Combine(environment.WebRootPath, "Uploads", "Usuarios");
                    if (!Directory.Exists(uploadsPath))
                        Directory.CreateDirectory(uploadsPath);

                    var fileName = $"perfil_{IdUsuario}{Path.GetExtension(FotoPerfilFile.FileName)}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        FotoPerfilFile.CopyTo(stream);
                    }

                    var fotoUrl = Path.Combine("/Uploads/Usuarios/", fileName).Replace("\\", "/");
                    _usuarioRepo.ActualizarFotoPerfil(IdUsuario, fotoUrl);

                    return Json(new { success = true, fotoUrl = fotoUrl }); // Retorna la nueva URL de la foto
                }

                return Json(new { success = false, message = "No se ha recibido la foto." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al actualizar la foto: " + ex.Message });
            }
        }

    }
}
