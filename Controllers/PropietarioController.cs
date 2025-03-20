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

        // Acción para listar todos los propietarios
        public IActionResult Listar()
        {
            var propietarios = _propietarioRepository.GetAll();
            return View(propietarios);
        }

        // Acción para mostrar detalles de un propietario
        public IActionResult Detalles(int id)
        {
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
            return View();
        }

        // Acción para procesar el formulario de creación
        [HttpPost]
        public IActionResult Insertar(Propietario propietario)
        {
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
            _propietarioRepository.Delete(id);
            return RedirectToAction("Listar"); // Redirige a la lista de propietarios
        }
    }
}