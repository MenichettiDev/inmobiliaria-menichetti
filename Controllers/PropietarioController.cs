using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories;

namespace InmobiliariaApp.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly PropietarioRepository _propietarioRepository;

        public PropietarioController(PropietarioRepository propietarioRepository)
        {
            _propietarioRepository = propietarioRepository;
        }

        // Método privado para verificar la autenticación
        private bool UsuarioAutenticado()
        {
            return HttpContext.Session.GetString("Usuario") != null;
        }

        // Método privado para obtener el rol del usuario
        private string ObtenerRolUsuario()
        {
            return HttpContext.Session.GetString("Rol") ?? string.Empty;
        }

        // Acción para listar todos los propietarios
        public IActionResult Listar()
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Index", "Login"); // Redirigir al login si no está autenticado
            }

            var propietarios = _propietarioRepository.GetAll();
            return View(propietarios);
        }

        // Acción para mostrar detalles de un propietario
        public IActionResult Detalles(int id)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            var propietario = _propietarioRepository.GetById(id);
            if (propietario == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el propietario
            }
            return View(propietario);
        }

        // Acción para mostrar el formulario de creación
        public IActionResult Insertar()
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        // Acción para procesar el formulario de creación
        [HttpPost]
        public IActionResult Insertar(Propietario propietario)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {
                _propietarioRepository.Add(propietario);
                return RedirectToAction("Listar"); // Redirige a la lista de propietarios
            }
            return View(propietario);
        }

        // Acción para mostrar el formulario de edición
        public IActionResult Editar(int id)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            var propietario = _propietarioRepository.GetById(id);
            if (propietario == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el propietario
            }
            return View(propietario);
        }

        // Acción para procesar el formulario de edición
        [HttpPost]
        public IActionResult Editar(int id, Propietario propietario)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            if (id != propietario.IdPropietario)
            {
                return BadRequest(); // Retorna un error 400 si los IDs no coinciden
            }

            if (ModelState.IsValid)
            {
                _propietarioRepository.Update(propietario);
                return RedirectToAction("Listar"); // Redirige a la lista de propietarios
            }
            return View(propietario);
        }

        // Acción para mostrar la vista de confirmación de eliminación
        public IActionResult Eliminar(int id)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            // Solo los administradores pueden eliminar
            if (ObtenerRolUsuario() != "Admin")
            {
                return Forbid(); // Denegar acceso si no es administrador
            }

            var propietario = _propietarioRepository.GetById(id);
            if (propietario == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el propietario
            }
            return View(propietario);
        }

        // Acción para confirmar la eliminación
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmed(int id)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Index", "Login");
            }

            // Solo los administradores pueden eliminar
            if (ObtenerRolUsuario() != "Admin")
            {
                return Forbid(); // Denegar acceso si no es administrador
            }

            _propietarioRepository.Delete(id);
            return RedirectToAction("Listar"); // Redirige a la lista de propietarios
        }
    }
}